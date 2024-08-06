// // Licensed to the .NET Foundation under one or more agreements.
// // The .NET Foundation licenses this file to you under the MIT license.
// // See the LICENSE file in the project root for more information.

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Data.SqlClientX
{
    /// <inheritdoc />
    public class SqlDataReaderX : DbDataReader
    {
        private readonly TdsParserX _parser;

        private SqlDataReaderX(TdsParserX parser)
        {
            _parser = parser;
        }

        internal static ValueTask<SqlDataReaderX> FromExecutedParser(
            TdsParserX parser,
            bool isAsync,
            CancellationToken ct)
        {
            var reader = new SqlDataReaderX(parser);
            return reader;
        }
    }
}
