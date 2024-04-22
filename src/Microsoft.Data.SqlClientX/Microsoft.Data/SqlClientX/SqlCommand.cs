// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Data;
using System.Data.Common;

namespace Microsoft.Data.SqlClientX
{
    public class SqlCommand : DbCommand
    {
        #region Properties

        /// <inheritdoc />
        public override string CommandText { get; set; }

        /// <inheritdoc />
        public override int CommandTimeout { get; set; }

        /// <inheritdoc />
        public override CommandType CommandType { get; set; }

        /// <inheritdoc />
        protected override DbConnection? DbConnection { get; set; }

        /// <inheritdoc />
        protected override DbParameterCollection DbParameterCollection { get; }

        /// <inheritdoc />
        protected override DbTransaction? DbTransaction { get; set; }

        /// <inheritdoc />
        public override bool DesignTimeVisible { get; set; }

        /// <inheritdoc />
        public override UpdateRowSource UpdatedRowSource { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override DbParameter CreateDbParameter()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        prote

        /// <inheritdoc />
        public override object? ExecuteScalar()
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public override Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public override void Prepare()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Helper Methods

        /// <inheritdoc />
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return TODO_IMPLEMENT_ME;
        }

        protected override Task<DbDataReader> ExecuteDbDataReaderAsync(
            CommandBehavior behavior,
            CancellationToken cancellationToken)
        {
            return TODO_IMPLEMENT_ME;
        }

        #endregion
    }
}
