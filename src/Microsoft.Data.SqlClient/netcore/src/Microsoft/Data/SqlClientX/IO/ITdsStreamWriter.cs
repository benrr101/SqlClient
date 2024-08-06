// // Licensed to the .NET Foundation under one or more agreements.
// // The .NET Foundation licenses this file to you under the MIT license.
// // See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Data.SqlClientX.IO
{
    public interface ITdsStreamWriter
    {
        TdsStreamPacketType PacketType { get; set; }

        ValueTask FlushPacketAsync(bool isAsync, CancellationToken ct);

        ValueTask WriteByteAsync(byte value, bool isAsync, CancellationToken ct);

        ValueTask WriteBytesAsync(Memory<byte> data, bool isAsync, CancellationToken ct);

        ValueTask WriteDoubleAsync(double value, bool isAsync, CancellationToken ct);

        ValueTask WriteFloatAsync(float value, bool isAsync, CancellationToken ct);

        ValueTask WriteIntAsync(int value, bool isAsync, CancellationToken ct);

        ValueTask WriteLongAsync(long value, bool isAsync, CancellationToken ct);

        ValueTask WriteShortAsync(int value, bool isAsync, CancellationToken ct);

        ValueTask WriteShortAsync(short value, bool isAsync, CancellationToken ct);

        ValueTask WriteStringAsync(string value, bool isAsync, CancellationToken ct);

        ValueTask WriteUnsignedIntAsync(uint value, bool isAsync, CancellationToken ct);

        ValueTask WriteUnsignedLongAsync(ulong value, bool isAsync, CancellationToken ct);

        ValueTask WriteUnsignedShortAsync(ushort value, bool isAsync, CancellationToken ct);
    }
}
