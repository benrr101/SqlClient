// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET8_0_OR_GREATER

using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Common;

#nullable enable

namespace Microsoft.Data.SqlClientX
{
    /// <summary>
    /// Represents a physical connection with the database.
    /// </summary>
    internal class SqlConnector
    {
        private static int SpoofedServerProcessId = 1;
        
        private TdsParser? _tdsParser;

        internal SqlConnector(SqlConnectionX owningConnection, SqlDataSource dataSource)
        {
            OwningConnection = owningConnection;
            DataSource = dataSource;
            //TODO: Set this based on the real server process id.
            //We only set this in client code right now to simulate different processes and to differentiate internal connections.
            ServerProcessId = Interlocked.Increment(ref SpoofedServerProcessId);
        }

        #region Properties

        /// <summary>
        /// The data source that generated this connector.
        /// </summary>
        internal SqlDataSource DataSource { get; }

        internal bool IsBroken => State == ConnectionState.Broken;
        
        internal bool IsClosed => State == ConnectionState.Closed;
        
        internal bool IsOpen => State == ConnectionState.Open;

        internal SqlConnectionX? OwningConnection { get; set; }
        
        //TODO: set this based on login info
        internal int ServerProcessId { get; private set; }

        /// <summary>
        /// The server version this connector is connected to.
        /// </summary>
        internal string ServerVersion => throw new NotImplementedException();

        /// <summary>
        /// Represents the current state of this connection.
        /// </summary>
        /// TODO: set and change state appropriately
        internal ConnectionState State = ConnectionState.Open;

        #endregion

        /// <summary>
        /// Closes this connection. If this connection is pooled, it is cleaned and returned to the pool.
        /// </summary>
        /// <returns>A Task indicating the result of the operation.</returns>
        /// <exception cref="NotImplementedException"></exception>
        internal void Close()
        {
            throw new NotImplementedException();
        }

        internal TdsParser GetParser(string methodName)
        {
            if (_tdsParser is null)
            {
                throw ADP.OpenConnectionRequired(methodName, State);
            }

            return _tdsParser;
        }
        
        /// <summary>
        /// Opens this connection.
        /// </summary>
        /// <param name = "timeout">The connection timeout for this operation.</param>
        /// <param name = "isAsync">Whether this method should run asynchronously.</param>
        /// <param name = "cancellationToken">The token used to cancel an ongoing asynchronous call.</param>
        /// <returns>A Task indicating the result of the operation.</returns>
        /// <exception cref="NotImplementedException"></exception>
        internal ValueTask Open(TimeSpan timeout, bool isAsync, CancellationToken cancellationToken)
        {
            //TODO: Simulates the work that will be done to open the connection.
            //Remove when open is implemented.

            if (isAsync)
            {
                Task WaitTask = Task.Delay(200);
                return new ValueTask(WaitTask);
            }
            else
            {
                Thread.Sleep(200);
                return ValueTask.CompletedTask;
            }
        }
        
        /// <summary>
        /// Returns this connection to the data source that generated it.
        /// </summary>
        internal void Return() => DataSource.ReturnInternalConnection(this);
    }
}

#endif
