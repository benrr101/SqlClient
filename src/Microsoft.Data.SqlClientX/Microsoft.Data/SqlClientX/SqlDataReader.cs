// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Collections;
using System.Data;
using System.Data.Common;

namespace Microsoft.Data.SqlClientX
{
    public class SqlDataReader : DbDataReader, IDataReader
    {
        #region Properties

        /// <inheritdoc />
        public override int Depth { get; }

        /// <inheritdoc />
        public override int FieldCount { get; }

        /// <inheritdoc />
        public override object this[int ordinal] => TODO_IMPLEMENT_ME;

        /// <inheritdoc />
        public override object this[string name] => TODO_IMPLEMENT_ME;

        /// <inheritdoc />
        public override int RecordsAffected { get; }

        /// <inheritdoc />
        public override bool HasRows { get; }

        /// <inheritdoc />
        public override bool IsClosed { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override bool GetBoolean(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override decimal GetDecimal(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override double GetDouble(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type GetFieldType(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override float GetFloat(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override short GetInt16(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetInt32(int ordinal)
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public override long GetInt64(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string GetName(int ordinal)
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public override int GetOrdinal(string name)
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public override string GetString(int ordinal)
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public override object GetValue(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override bool IsDBNull(int ordinal)
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public override bool NextResult()
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public override bool Read()
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            return TODO_IMPLEMENT_ME;
        }

        #endregion
    }
}
