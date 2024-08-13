// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Data.SqlClientX.IO
{
    /// <summary>
    /// An interface for a TDS stream that implements methods for writing.
    /// </summary>
    internal interface ITdsWriteStream : ITdsStream
    {
        /// <summary>
        /// Gets or sets the type of the packet that the stream is currently writing.
        /// </summary>
        TdsStreamPacketType? PacketHeaderType { get; set; }

        /// <summary>
        /// Gets a writer associated with this write stream.
        /// </summary>
        ITdsWriter Writer { get; }

        /// <summary>
        /// Writes a byte to the stream.
        /// </summary>
        /// <param name="value">The value to be written.</param>
        /// <param name="isAsync">Whether the operation should be performed asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        ValueTask WriteByteAsync(byte value, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Writes a contiguous segment of memory to the underlying stream.
        /// </summary>
        /// <remarks>
        /// Since <see cref="TdsStream"/> should be inheriting from <see cref="Stream"/> with its
        /// own implementations of Write/WriteAsync, this method is technically unnecessary. It is
        /// provided as a convenience to allow callers to have a one-stop-shop for reading bytes
        /// from the underlying stream without worrying about asynchronicity.
        /// </remarks>
        /// <param name="buffer">Contiguous segment of memory to write to the stream.</param>
        /// <param name="isAsync">Whether the operation should be performed asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        ValueTask WriteBytesAsync(Memory<byte> buffer, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Queues a cancellation request to the stream, which will be sent to the server.
        /// </summary>
        void QueueCancellation();
    }
}
