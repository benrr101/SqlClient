// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Data.SqlClientX.IO
{
    internal enum TdsStreamPacketType
    {
        SqlBatch = 0x01,

        Login = 0x02,

        Rpc = 0x03,

        TabularResult = 0x04,

        // 0x05 is unused

        Attention = 0x06,

        BulkLoad = 0x07,

        FedAuthToken = 0x08,

        // 0x09-0x0D are unused

        TransactionManagerRequest = 0x0E,

        // 0x0F is unsed

        Login7 = 0x10,

        SspiMessage = 0x11,

        PreLogin = 0x12,
    }
}
