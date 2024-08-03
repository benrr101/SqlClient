// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Data.ProviderBase
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Data.Common;
    using Microsoft.Data.SqlClient;

    internal abstract class DbConnectionFactory
    {
        private Dictionary<DbConnectionPoolKey, DbConnectionPoolGroup> _connectionPoolGroups;
        private readonly List<DbConnectionPool> _poolsToRelease;
        private readonly List<DbConnectionPoolGroup> _poolGroupsToRelease;
        private readonly DbConnectionPoolCounters _performanceCounters;
        private readonly Timer _pruningTimer;

        private const int PruningDueTime = 4 * 60 * 1000;           // 4 minutes
        private const int PruningPeriod = 30 * 1000;           // thirty seconds

        private static int _objectTypeCount; // EventSource counter
        internal readonly int _objectID = System.Threading.Interlocked.Increment(ref _objectTypeCount);

        // s_pendingOpenNonPooled is an array of tasks used to throttle creation of non-pooled connections to
        // a maximum of Environment.ProcessorCount at a time.
        static int s_pendingOpenNonPooledNext = 0;
        static Task<DbConnectionInternal>[] s_pendingOpenNonPooled = new Task<DbConnectionInternal>[Environment.ProcessorCount];
        static Task<DbConnectionInternal> s_completedTask;

        protected DbConnectionFactory() : this(DbConnectionPoolCountersNoCounters.SingletonInstance) { }

        protected DbConnectionFactory(DbConnectionPoolCounters performanceCounters)
        {
            _performanceCounters = performanceCounters;
            _connectionPoolGroups = new Dictionary<DbConnectionPoolKey, DbConnectionPoolGroup>();
            _poolsToRelease = new List<DbConnectionPool>();
            _poolGroupsToRelease = new List<DbConnectionPoolGroup>();
            _pruningTimer = CreatePruningTimer();
        }

        internal DbConnectionPoolCounters PerformanceCounters
        {
            get { return _performanceCounters; }
        }

        abstract public DbProviderFactory ProviderFactory
        {
            get;
        }

        internal int ObjectID
        {
            get
            {
                return _objectID;
            }
        }

        public void ClearAllPools()
        {
            using (TryEventScope.Create(nameof(DbConnectionFactory)))
            {
                Dictionary<DbConnectionPoolKey, DbConnectionPoolGroup> connectionPoolGroups = _connectionPoolGroups;
                foreach (KeyValuePair<DbConnectionPoolKey, DbConnectionPoolGroup> entry in connectionPoolGroups)
                {
                    DbConnectionPoolGroup poolGroup = entry.Value;
                    if (poolGroup != null)
                    {
                        poolGroup.Clear();
                    }
                }
            }

        }

        public void ClearPool(DbConnection connection)
        {
            ADP.CheckArgumentNull(connection, "connection");
            using (TryEventScope.Create("<prov.DbConnectionFactory.ClearPool|API> {0}", GetObjectId(connection)))
            {
                DbConnectionPoolGroup poolGroup = GetConnectionPoolGroup(connection);
                if (poolGroup != null)
                {
                    poolGroup.Clear();
                }
            }
        }

        public void ClearPool(DbConnectionPoolKey key)
        {
            Debug.Assert(key != null, "key cannot be null");
            ADP.CheckArgumentNull(key.ConnectionString, "key.ConnectionString");
            using (TryEventScope.Create("<prov.DbConnectionFactory.ClearPool|API> connectionString"))
            {
                DbConnectionPoolGroup poolGroup;
                Dictionary<DbConnectionPoolKey, DbConnectionPoolGroup> connectionPoolGroups = _connectionPoolGroups;
                if (connectionPoolGroups.TryGetValue(key, out poolGroup))
                {
                    poolGroup.Clear();
                }
            }
        }

        internal virtual DbConnectionPoolProviderInfo CreateConnectionPoolProviderInfo(DbConnectionOptions connectionOptions)
        {
            return null;
        }

        virtual protected DbMetaDataFactory CreateMetaDataFactory(DbConnectionInternal internalConnection, out bool cacheMetaDataFactory)
        {
            // providers that support GetSchema must override this with a method that creates a meta data
            // factory appropriate for them.
            cacheMetaDataFactory = false;
            throw ADP.NotSupported();
        }

        internal DbConnectionInternal CreateNonPooledConnection(DbConnection owningConnection, DbConnectionPoolGroup poolGroup, DbConnectionOptions userOptions)
        {
            Debug.Assert(owningConnection != null, "null owningConnection?");
            Debug.Assert(poolGroup != null, "null poolGroup?");

            DbConnectionOptions connectionOptions = poolGroup.ConnectionOptions;
            DbConnectionPoolGroupProviderInfo poolGroupProviderInfo = poolGroup.ProviderInfo;
            DbConnectionPoolKey poolKey = poolGroup.PoolKey;

            DbConnectionInternal newConnection = CreateConnection(connectionOptions, poolKey, poolGroupProviderInfo, null, owningConnection, userOptions);
            if (newConnection != null)
            {
                PerformanceCounters.HardConnectsPerSecond.Increment();
                newConnection.MakeNonPooledObject(owningConnection, PerformanceCounters);
            }
            SqlClientEventSource.Log.TryTraceEvent("<prov.DbConnectionFactory.CreateNonPooledConnection|RES|CPOOL> {0}, Non-pooled database connection created.", ObjectID);
            return newConnection;
        }

        internal DbConnectionInternal CreatePooledConnection(DbConnectionPool pool, DbConnection owningObject, DbConnectionOptions options, DbConnectionPoolKey poolKey, DbConnectionOptions userOptions)
        {
            Debug.Assert(pool != null, "null pool?");
            DbConnectionPoolGroupProviderInfo poolGroupProviderInfo = pool.PoolGroup.ProviderInfo;

            DbConnectionInternal newConnection = CreateConnection(options, poolKey, poolGroupProviderInfo, pool, owningObject, userOptions);

            if (newConnection != null)
            {
                PerformanceCounters.HardConnectsPerSecond.Increment();
                newConnection.MakePooledConnection(pool);
            }
            SqlClientEventSource.Log.TryTraceEvent("<prov.DbConnectionFactory.CreatePooledConnection|RES|CPOOL> {0}, Pooled database connection created.", ObjectID);
            return newConnection;
        }

        virtual internal DbConnectionPoolGroupProviderInfo CreateConnectionPoolGroupProviderInfo(DbConnectionOptions connectionOptions)
        {
            return null;
        }

        private Timer CreatePruningTimer()
        {
            TimerCallback callback = new TimerCallback(PruneConnectionPoolGroups);
            return new Timer(callback, null, PruningDueTime, PruningPeriod);
        }

        protected DbConnectionOptions FindConnectionOptions(DbConnectionPoolKey key)
        {
            Debug.Assert(key != null, "key cannot be null");
            if (!ADP.IsEmpty(key.ConnectionString))
            {
                DbConnectionPoolGroup connectionPoolGroup;
                Dictionary<DbConnectionPoolKey, DbConnectionPoolGroup> connectionPoolGroups = _connectionPoolGroups;
                if (connectionPoolGroups.TryGetValue(key, out connectionPoolGroup))
                {
                    return connectionPoolGroup.ConnectionOptions;
                }
            }
            return null;
        }

        // GetCompletedTask must be called from within s_pendingOpenPooled lock
        static Task<DbConnectionInternal> GetCompletedTask()
        {
            if (s_completedTask == null)
            {
                TaskCompletionSource<DbConnectionInternal> source = new TaskCompletionSource<DbConnectionInternal>();
                source.SetResult(null);
                s_completedTask = source.Task;
            }
            return s_completedTask;
        }

        internal bool TryGetConnection(DbConnection owningConnection, TaskCompletionSource<DbConnectionInternal> retry, DbConnectionOptions userOptions, DbConnectionInternal oldConnection, out DbConnectionInternal connection)
        {
            Debug.Assert(owningConnection != null, "null owningConnection?");

            DbConnectionPoolGroup poolGroup;
            DbConnectionPool connectionPool;
            connection = null;

            // SQLBU 431251:
            //  Work around race condition with clearing the pool between GetConnectionPool obtaining pool
            //  and GetConnection on the pool checking the pool state.  Clearing the pool in this window
            //  will switch the pool into the ShuttingDown state, and GetConnection will return null.
            //  There is probably a better solution involving locking the pool/group, but that entails a major
            //  re-design of the connection pooling synchronization, so is postponed for now.

            // VSDD 674236: use retriesLeft to prevent CPU spikes with incremental sleep
            // start with one msec, double the time every retry
            // max time is: 1 + 2 + 4 + ... + 2^(retries-1) == 2^retries -1 == 1023ms (for 10 retries)
            int retriesLeft = 10;
            int timeBetweenRetriesMilliseconds = 1;

            do
            {
                poolGroup = GetConnectionPoolGroup(owningConnection);
                // Doing this on the callers thread is important because it looks up the WindowsIdentity from the thread.
                connectionPool = GetConnectionPool(owningConnection, poolGroup);
                if (connectionPool == null)
                {
                    // If GetConnectionPool returns null, we can be certain that
                    // this connection should not be pooled via DbConnectionPool
                    // or have a disabled pool entry.
                    poolGroup = GetConnectionPoolGroup(owningConnection); // previous entry have been disabled

                    if (retry != null)
                    {
                        Task<DbConnectionInternal> newTask;
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        lock (s_pendingOpenNonPooled)
                        {

                            // look for an available task slot (completed or empty)
                            int idx;
                            for (idx = 0; idx < s_pendingOpenNonPooled.Length; idx++)
                            {
                                Task task = s_pendingOpenNonPooled[idx];
                                if (task == null)
                                {
                                    s_pendingOpenNonPooled[idx] = GetCompletedTask();
                                    break;
                                }
                                else if (task.IsCompleted)
                                {
                                    break;
                                }
                            }

                            // if didn't find one, pick the next one in round-robbin fashion
                            if (idx == s_pendingOpenNonPooled.Length)
                            {
                                idx = s_pendingOpenNonPooledNext++ % s_pendingOpenNonPooled.Length;
                            }

                            // now that we have an antecedent task, schedule our work when it is completed.
                            // If it is a new slot or a completed task, this continuation will start right away.
                            // BUG? : If we have timed out task on top of running task, then new task could be started
                            // on top of that, since we are only checking the top task. This will lead to starting more threads
                            // than intended.
                            newTask = s_pendingOpenNonPooled[idx].ContinueWith((_) =>
                            {
                                System.Transactions.Transaction originalTransaction = ADP.GetCurrentTransaction();
                                try
                                {
                                    ADP.SetCurrentTransaction(retry.Task.AsyncState as System.Transactions.Transaction);
                                    var newConnection = CreateNonPooledConnection(owningConnection, poolGroup, userOptions);
                                    if (oldConnection != null && oldConnection.State == ConnectionState.Open)
                                    {
                                        oldConnection.PrepareForReplaceConnection();
                                        oldConnection.Dispose();
                                    }
                                    return newConnection;
                                }
                                finally
                                {
                                    ADP.SetCurrentTransaction(originalTransaction);
                                }
                            }, cancellationTokenSource.Token, TaskContinuationOptions.LongRunning, TaskScheduler.Default);

                            // Place this new task in the slot so any future work will be queued behind it
                            s_pendingOpenNonPooled[idx] = newTask;
                        }

                        // Set up the timeout (if needed)
                        if (owningConnection.ConnectionTimeout > 0)
                        {
                            int connectionTimeoutMilliseconds = owningConnection.ConnectionTimeout * 1000;
                            cancellationTokenSource.CancelAfter(connectionTimeoutMilliseconds);
                        }

                        // once the task is done, propagate the final results to the original caller
                        newTask.ContinueWith((task) =>
                        {
                            cancellationTokenSource.Dispose();
                            if (task.IsCanceled)
                            {
                                retry.TrySetException(ADP.ExceptionWithStackTrace(ADP.NonPooledOpenTimeout()));
                            }
                            else if (task.IsFaulted)
                            {
                                retry.TrySetException(task.Exception.InnerException);
                            }
                            else
                            {
                                if (retry.TrySetResult(task.Result))
                                {
                                    PerformanceCounters.NumberOfNonPooledConnections.Increment();
                                }
                                else
                                {
                                    // The outer TaskCompletionSource was already completed
                                    // Which means that we don't know if someone has messed with the outer connection in the middle of creation
                                    // So the best thing to do now is to destroy the newly created connection
                                    task.Result.DoomThisConnection();
                                    task.Result.Dispose();
                                }
                            }
                        }, TaskScheduler.Default);

                        return false;
                    }

                    connection = CreateNonPooledConnection(owningConnection, poolGroup, userOptions);
                    PerformanceCounters.NumberOfNonPooledConnections.Increment();
                }
                else
                {
                    if (((SqlClient.SqlConnection)owningConnection).ForceNewConnection)
                    {
                        Debug.Assert(!(oldConnection is DbConnectionClosed), "Force new connection, but there is no old connection");
                        connection = connectionPool.ReplaceConnection(owningConnection, userOptions, oldConnection);
                    }
                    else
                    {
                        if (!connectionPool.TryGetConnection(owningConnection, retry, userOptions, out connection))
                        {
                            return false;
                        }
                    }

                    if (connection == null)
                    {
                        // connection creation failed on semaphore waiting or if max pool reached
                        if (connectionPool.IsRunning)
                        {
                            SqlClientEventSource.Log.TryTraceEvent("<prov.DbConnectionFactory.GetConnection|RES|CPOOL> {0}, GetConnection failed because a pool timeout occurred.", ObjectID);

                            // If GetConnection failed while the pool is running, the pool timeout occurred.
                            throw ADP.PooledOpenTimeout();
                        }
                        else
                        {
                            // We've hit the race condition, where the pool was shut down after we got it from the group.
                            // Yield time slice to allow shut down activities to complete and a new, running pool to be instantiated
                            //  before retrying.
                            System.Threading.Thread.Sleep(timeBetweenRetriesMilliseconds);
                            timeBetweenRetriesMilliseconds *= 2; // double the wait time for next iteration
                        }
                    }
                }
            } while (connection == null && retriesLeft-- > 0);

            if (connection == null)
            {
                SqlClientEventSource.Log.TryTraceEvent("<prov.DbConnectionFactory.GetConnection|RES|CPOOL> {0}, GetConnection failed because a pool timeout occurred and all retries were exhausted.", ObjectID);

                // exhausted all retries or timed out - give up
                throw ADP.PooledOpenTimeout();
            }

            return true;
        }

        private DbConnectionPool GetConnectionPool(DbConnection owningObject, DbConnectionPoolGroup connectionPoolGroup)
        {
            // if poolgroup is disabled, it will be replaced with a new entry

            Debug.Assert(owningObject != null, "null owningObject?");
            Debug.Assert(connectionPoolGroup != null, "null connectionPoolGroup?");

            // It is possible that while the outer connection object has
            // been sitting around in a closed and unused state in some long
            // running app, the pruner may have come along and remove this
            // the pool entry from the master list.  If we were to use a
            // pool entry in this state, we would create "unmanaged" pools,
            // which would be bad.  To avoid this problem, we automagically
            // re-create the pool entry whenever it's disabled.

            // however, don't rebuild connectionOptions if no pooling is involved - let new connections do that work
            if (connectionPoolGroup.IsDisabled && connectionPoolGroup.PoolGroupOptions != null)
            {
                SqlClientEventSource.Log.TryTraceEvent("<prov.DbConnectionFactory.GetConnectionPool|RES|INFO|CPOOL> {0}, DisabledPoolGroup={1}", ObjectID, connectionPoolGroup.ObjectID);

                // reusing existing pool option in case user originally used SetConnectionPoolOptions
                DbConnectionPoolGroupOptions poolOptions = connectionPoolGroup.PoolGroupOptions;

                // get the string to hash on again
                DbConnectionOptions connectionOptions = connectionPoolGroup.ConnectionOptions;
                Debug.Assert(connectionOptions != null, "prevent expansion of connectionString");

                connectionPoolGroup = GetConnectionPoolGroup(connectionPoolGroup.PoolKey, poolOptions, ref connectionOptions);
                Debug.Assert(connectionPoolGroup != null, "null connectionPoolGroup?");
                SetConnectionPoolGroup(owningObject, connectionPoolGroup);
            }
            DbConnectionPool connectionPool = connectionPoolGroup.GetConnectionPool(this);
            return connectionPool;
        }

        internal DbConnectionPoolGroup GetConnectionPoolGroup(DbConnectionPoolKey key, DbConnectionPoolGroupOptions poolOptions, ref DbConnectionOptions userConnectionOptions)
        {
            if (ADP.IsEmpty(key.ConnectionString))
            {
                return (DbConnectionPoolGroup)null;
            }

            DbConnectionPoolGroup connectionPoolGroup;
            Dictionary<DbConnectionPoolKey, DbConnectionPoolGroup> connectionPoolGroups = _connectionPoolGroups;
            if (!connectionPoolGroups.TryGetValue(key, out connectionPoolGroup) || (connectionPoolGroup.IsDisabled && connectionPoolGroup.PoolGroupOptions != null))
            {
                // If we can't find an entry for the connection string in
                // our collection of pool entries, then we need to create a
                // new pool entry and add it to our collection.

                DbConnectionOptions connectionOptions = CreateConnectionOptions(key.ConnectionString, userConnectionOptions);
                if (connectionOptions == null)
                {
                    throw ADP.InternalConnectionError(ADP.ConnectionError.ConnectionOptionsMissing);
                }

                string expandedConnectionString = key.ConnectionString;
                if (userConnectionOptions == null)
                { // we only allow one expansion on the connection string

                    userConnectionOptions = connectionOptions;
                    expandedConnectionString = connectionOptions.Expand();

                    // if the expanded string is same instance (default implementation), then use the already created options
                    if ((object)expandedConnectionString != (object)key.ConnectionString)
                    {
                        // CONSIDER: caching the original string to reduce future parsing
                        DbConnectionPoolKey newKey = (DbConnectionPoolKey)((ICloneable)key).Clone();
                        newKey.ConnectionString = expandedConnectionString;
                        return GetConnectionPoolGroup(newKey, null, ref userConnectionOptions);
                    }
                }

                // We don't support connection pooling on Win9x; it lacks too many of the APIs we require.
                if (poolOptions == null && ADP.s_isWindowsNT)
                {
                    if (connectionPoolGroup != null)
                    {
                        // reusing existing pool option in case user originally used SetConnectionPoolOptions
                        poolOptions = connectionPoolGroup.PoolGroupOptions;
                    }
                    else
                    {
                        // Note: may return null for non-pooled connections
                        poolOptions = CreateConnectionPoolGroupOptions(connectionOptions);
                    }
                }

                lock (this)
                {
                    connectionPoolGroups = _connectionPoolGroups;
                    if (!connectionPoolGroups.TryGetValue(key, out connectionPoolGroup))
                    {
                        DbConnectionPoolGroup newConnectionPoolGroup = new DbConnectionPoolGroup(connectionOptions, key, poolOptions);
                        newConnectionPoolGroup.ProviderInfo = CreateConnectionPoolGroupProviderInfo(connectionOptions);

                        // build new dictionary with space for new connection string
                        Dictionary<DbConnectionPoolKey, DbConnectionPoolGroup> newConnectionPoolGroups = new Dictionary<DbConnectionPoolKey, DbConnectionPoolGroup>(1 + connectionPoolGroups.Count);
                        foreach (KeyValuePair<DbConnectionPoolKey, DbConnectionPoolGroup> entry in connectionPoolGroups)
                        {
                            newConnectionPoolGroups.Add(entry.Key, entry.Value);
                        }

                        // lock prevents race condition with PruneConnectionPoolGroups
                        newConnectionPoolGroups.Add(key, newConnectionPoolGroup);
                        PerformanceCounters.NumberOfActiveConnectionPoolGroups.Increment();
                        connectionPoolGroup = newConnectionPoolGroup;
                        _connectionPoolGroups = newConnectionPoolGroups;
                    }
                    else
                    {
                        Debug.Assert(!connectionPoolGroup.IsDisabled, "Disabled pool entry discovered");
                    }
                }
                Debug.Assert(connectionPoolGroup != null, "how did we not create a pool entry?");
                Debug.Assert(userConnectionOptions != null, "how did we not have user connection options?");
            }
            else if (userConnectionOptions == null)
            {
                userConnectionOptions = connectionPoolGroup.ConnectionOptions;
            }
            return connectionPoolGroup;
        }

        internal DbMetaDataFactory GetMetaDataFactory(DbConnectionPoolGroup connectionPoolGroup, DbConnectionInternal internalConnection)
        {
            Debug.Assert(connectionPoolGroup != null, "connectionPoolGroup may not be null.");

            // get the matadatafactory from the pool entry. If it does not already have one
            // create one and save it on the pool entry
            DbMetaDataFactory metaDataFactory = connectionPoolGroup.MetaDataFactory;

            // consider serializing this so we don't construct multiple metadata factories
            // if two threads happen to hit this at the same time.  One will be GC'd
            if (metaDataFactory == null)
            {
                bool allowCache = false;
                metaDataFactory = CreateMetaDataFactory(internalConnection, out allowCache);
                if (allowCache)
                {
                    connectionPoolGroup.MetaDataFactory = metaDataFactory;
                }
            }
            return metaDataFactory;
        }

        private void PruneConnectionPoolGroups(object state)
        {
            // when debugging this method, expect multiple threads at the same time
            SqlClientEventSource.Log.TryAdvancedTraceEvent("<prov.DbConnectionFactory.PruneConnectionPoolGroups|RES|INFO|CPOOL> {0}", ObjectID);

            // First, walk the pool release list and attempt to clear each
            // pool, when the pool is finally empty, we dispose of it.  If the
            // pool isn't empty, it's because there are active connections or
            // distributed transactions that need it.
            lock (_poolsToRelease)
            {
                if (_poolsToRelease.Count != 0)
                {
                    DbConnectionPool[] poolsToRelease = _poolsToRelease.ToArray();
                    foreach (DbConnectionPool pool in poolsToRelease)
                    {
                        if (pool != null)
                        {
                            pool.Clear();

                            if (pool.Count == 0)
                            {
                                _poolsToRelease.Remove(pool);
                                SqlClientEventSource.Log.TryAdvancedTraceEvent("<prov.DbConnectionFactory.PruneConnectionPoolGroups|RES|INFO|CPOOL> {0}, ReleasePool={1}", ObjectID, pool.ObjectID);
                                PerformanceCounters.NumberOfInactiveConnectionPools.Decrement();
                            }
                        }
                    }
                }
            }

            // Next, walk the pool entry release list and dispose of each
            // pool entry when it is finally empty.  If the pool entry isn't
            // empty, it's because there are active pools that need it.
            lock (_poolGroupsToRelease)
            {
                if (_poolGroupsToRelease.Count != 0)
                {
                    DbConnectionPoolGroup[] poolGroupsToRelease = _poolGroupsToRelease.ToArray();
                    foreach (DbConnectionPoolGroup poolGroup in poolGroupsToRelease)
                    {
                        if (poolGroup != null)
                        {
                            int poolsLeft = poolGroup.Clear(); // may add entries to _poolsToRelease

                            if (poolsLeft == 0)
                            {
                                _poolGroupsToRelease.Remove(poolGroup);
                                SqlClientEventSource.Log.TryAdvancedTraceEvent("<prov.DbConnectionFactory.PruneConnectionPoolGroups|RES|INFO|CPOOL> {0}, ReleasePoolGroup={1}", ObjectID, poolGroup.ObjectID);
                                PerformanceCounters.NumberOfInactiveConnectionPoolGroups.Decrement();
                            }
                        }
                    }
                }
            }

            // Finally, we walk through the collection of connection pool entries
            // and prune each one.  This will cause any empty pools to be put
            // into the release list.
            lock (this)
            {
                Dictionary<DbConnectionPoolKey, DbConnectionPoolGroup> connectionPoolGroups = _connectionPoolGroups;
                Dictionary<DbConnectionPoolKey, DbConnectionPoolGroup> newConnectionPoolGroups = new Dictionary<DbConnectionPoolKey, DbConnectionPoolGroup>(connectionPoolGroups.Count);

                foreach (KeyValuePair<DbConnectionPoolKey, DbConnectionPoolGroup> entry in connectionPoolGroups)
                {
                    if (entry.Value != null)
                    {
                        Debug.Assert(!entry.Value.IsDisabled, "Disabled pool entry discovered");

                        // entries start active and go idle during prune if all pools are gone
                        // move idle entries from last prune pass to a queue for pending release
                        // otherwise process entry which may move it from active to idle
                        if (entry.Value.Prune())
                        { // may add entries to _poolsToRelease
                            PerformanceCounters.NumberOfActiveConnectionPoolGroups.Decrement();
                            QueuePoolGroupForRelease(entry.Value);
                        }
                        else
                        {
                            newConnectionPoolGroups.Add(entry.Key, entry.Value);
                        }
                    }
                }
                _connectionPoolGroups = newConnectionPoolGroups;
            }
        }

        internal void QueuePoolForRelease(DbConnectionPool pool, bool clearing)
        {
            // Queue the pool up for release -- we'll clear it out and dispose
            // of it as the last part of the pruning timer callback so we don't
            // do it with the pool entry or the pool collection locked.
            Debug.Assert(pool != null, "null pool?");

            // set the pool to the shutdown state to force all active
            // connections to be automatically disposed when they
            // are returned to the pool
            pool.Shutdown();

            lock (_poolsToRelease)
            {
                if (clearing)
                {
                    pool.Clear();
                }
                _poolsToRelease.Add(pool);
            }
            PerformanceCounters.NumberOfInactiveConnectionPools.Increment();
        }

        internal void QueuePoolGroupForRelease(DbConnectionPoolGroup poolGroup)
        {
            Debug.Assert(poolGroup != null, "null poolGroup?");
            SqlClientEventSource.Log.TryTraceEvent("<prov.DbConnectionFactory.QueuePoolGroupForRelease|RES|INFO|CPOOL> {0}, poolGroup={1}", ObjectID, poolGroup.ObjectID);

            lock (_poolGroupsToRelease)
            {
                _poolGroupsToRelease.Add(poolGroup);
            }
            PerformanceCounters.NumberOfInactiveConnectionPoolGroups.Increment();
        }

        virtual protected DbConnectionInternal CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningConnection, DbConnectionOptions userOptions)
        {
            return CreateConnection(options, poolKey, poolGroupProviderInfo, pool, owningConnection);
        }

        abstract protected DbConnectionInternal CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningConnection);

        abstract protected DbConnectionOptions CreateConnectionOptions(string connectionString, DbConnectionOptions previous);

        abstract protected DbConnectionPoolGroupOptions CreateConnectionPoolGroupOptions(DbConnectionOptions options);

        abstract internal DbConnectionPoolGroup GetConnectionPoolGroup(DbConnection connection);

        abstract internal DbConnectionInternal GetInnerConnection(DbConnection connection);

        abstract protected int GetObjectId(DbConnection connection);

        abstract internal void PermissionDemand(DbConnection outerConnection);

        abstract internal void SetConnectionPoolGroup(DbConnection outerConnection, DbConnectionPoolGroup poolGroup);

        abstract internal void SetInnerConnectionEvent(DbConnection owningObject, DbConnectionInternal to);

        abstract internal bool SetInnerConnectionFrom(DbConnection owningObject, DbConnectionInternal to, DbConnectionInternal from);

        abstract internal void SetInnerConnectionTo(DbConnection owningObject, DbConnectionInternal to);
    }
}
