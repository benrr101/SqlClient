// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Data;
using System.Data.Common;

namespace Microsoft.Data.SqlClientX
{
    public class SqlConnection : DbConnection
    {
        #region Properties

        /// <inheritdoc />
        public override string ConnectionString { get; set; }

        /// <inheritdoc />
        public override string Database { get; }

        /// <inheritdoc />
        public override string DataSource { get; }

        /// <inheritdoc />
        public override string ServerVersion { get; }

        /// <inheritdoc />
        public override ConnectionState State { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Close()
        {
            TODO_IMPLEMENT_ME();
        }

        public override Task CloseAsync()
        {
            TODO_IMPLEMENT_ME();
        }

        /// <inheritdoc />
        protected override DbCommand CreateDbCommand()
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public override void Open()
        {
            TODO_IMPLEMENT_ME();
        }

        public override void OpenAsync()
        {
            TODO_IMPLEMENT_ME();
        }

        #endregion
    }
}
