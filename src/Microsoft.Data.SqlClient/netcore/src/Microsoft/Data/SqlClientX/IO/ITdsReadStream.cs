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
    /// An interface for a TDS stream that implements methods for reading.
    /// </summary>
    internal interface ITdsReadStream : ITdsStream
    {
        /// <summary>
        /// Gets the number of bytes left in the current packet.
        /// </summary>
        int PacketDataLeft { get; }

        /// <summary>
        /// Gets the type of the packet currently buffered in the stream.
        /// </summary>
        byte ReadPacketHeaderType { get; }

        /// <summary>
        /// Gets the status field of the packet currently buffered in the stream.
        /// </summary>
        byte ReadPacketStatus { get; }

        /// <summary>
        /// Gets a reader associated with this read stream.
        /// </summary>
        ITdsReader Reader { get; }

        /// <summary>
        /// Gets the SPID of the connection on which this stream is operating.
        /// </summary>
        int Spid { get; }

        /// <summary>
        /// Peeks the next byte in the stream, without consuming it.
        /// The next call to read will return the same byte but it 
        /// will consume it.
        /// </summary>
        /// <param name="isAsync"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        ValueTask<byte> PeekByteAsync(bool isAsync, CancellationToken ct);

        /// <summary>
        /// Reads a byte from the stream.
        /// </summary>
        /// <param name="isAsync">Indicates if the operation should be async.</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        ValueTask<byte> ReadByteAsync(bool isAsync, CancellationToken ct);

        /// <summary>
        /// Reads bytes from the stream into a contiguous segment of memory.
        /// </summary>
        /// <remarks>
        /// Since <see cref="TdsStream"/> should be inheriting from <see cref="Stream"/> with its
        /// own implementations of Read/ReadAsync, this method is technically unnecessary. It is
        /// provided as a convenience to allow callers to have a one-stop-shop for writing bytes to
        /// the underlying stream without worrying about asynchronicity.
        /// </remarks>
        /// <param name="buffer">Contiguous segment of memory to read bytes from the stream into.</param>
        /// <param name="isAsync">Whether the operation should be performed asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Number of bytes read from the stream into the buffer. If <paramref name="isAsync"/> is
        /// <c>false</c> the value will be returned synchronously.
        /// </returns>
        ValueTask<int> ReadBytesAsync(Memory<byte> buffer, bool isAsync, CancellationToken ct);

        /// <summary>
        /// A convenience method to skip the bytes in the stream,
        /// by allowing buffer manipulation, instead of making the consumer
        /// allocate buffers to read and discard the bytes.
        /// </summary>
        /// <param name="skipCount">Number of bytes to skip</param>
        /// <param name="isAsync">If the method should be called Asynchronously.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        public ValueTask SkipReadBytesAsync(int skipCount, bool isAsync, CancellationToken ct);
    }
}
