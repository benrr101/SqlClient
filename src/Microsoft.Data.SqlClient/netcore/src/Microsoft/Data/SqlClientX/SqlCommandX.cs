using System;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Microsoft.Data.Common;
using Microsoft.Data.SqlClient;

#if NET8_0_OR_GREATER

namespace Microsoft.Data.SqlClientX
{
    /// <inheritdoc />
    public class SqlCommandX : DbCommand
    {
        #region Properties
        
        public new SqlConnectionX Connection { get; set; }
        
        public override string CommandText { get; set; }
        
        public override int CommandTimeout { get; set; }
        
        public override CommandType CommandType { get; set; }

        /// <inheritdoc />
        protected override DbConnection DbConnection
        {
            get => Connection;
            set
            {
                if (value is not SqlConnectionX sqlConnection)
                {
                    throw new ArgumentException(nameof(value));
                }

                Connection = sqlConnection;
            }
        }
        
        protected override DbParameterCollection DbParameterCollection { get; }
        
        public override bool DesignTimeVisible { get; set; }
        
        protected override DbTransaction DbTransaction { get; set; }
        
        public override UpdateRowSource UpdatedRowSource { set; get; }

        #endregion
        
        #region Methods
        
        public override void Cancel()
        {
            throw new System.NotImplementedException();
        }

        public new SqlDataReader ExecuteReader() =>
            ExecuteReader(CommandBehavior.Default);

        public new SqlDataReader ExecuteReader(CommandBehavior commandBehavior)
        {
            
        }
        
        public override int ExecuteNonQuery()
        {
            throw new System.NotImplementedException();
        }
        
        public override object ExecuteScalar()
        {
            throw new System.NotImplementedException();
        }

        public override void Prepare()
        {
            throw new System.NotImplementedException();
        }

        protected override DbParameter CreateDbParameter()
        {
            throw new System.NotImplementedException();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
            ExecuteReader(behavior);

        private TdsParserX GetParser(string methodName)
        {
            if (Connection is null)
            {
                throw ADP.ConnectionRequired(methodName);
            }

            return Connection.GetParser(methodName);
        }
        
        #endregion
    }
}

#endif
