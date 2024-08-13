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
    /// A Stream abstraction over the TDS protocol.
    /// The stream can be used to read and write TDS messages on a 
    /// SQL Server physical connection.
    /// The stream is responsible for abstracting away the calls to IO and handling the packet 
    /// header from the consumers of TDS protocol.
    /// The stream responds to the request of the callers, but it doesn't guarantee the correctness of the underlying TDS protocol correctness.
    /// e.g. If the protocol states that there are N bytes that should be read, and the stream is asked to return
    /// N + 1 bytes, then the stream will timeout trying to get N+1 bytes, or it will return N+1 bytes, if the 
    /// N+1 byte is available.
    /// </summary>
    // @TODO: This class provides is a composition of two classes and provides no unique functionality. The underlying implementations could be rolled into a single class using partials.
    internal class TdsStream : Stream, ITdsWriteStream, ITdsReadStream
    {
        // TODO: Handle Cancellation tokens in all async paths.
        private TdsReadStream _readStream;
        private TdsWriteStream _writeStream;

        /// <summary>
        /// Constructor for instantiating the TdsStream
        /// </summary>
        /// <param name="writeStream">The stream for outgoing TDS packets</param>
        /// <param name="readStream">The stream for reading incoming TDS packets.</param>
        public TdsStream(TdsWriteStream writeStream, TdsReadStream readStream) : base()
        {
            _readStream = readStream;
            _writeStream = writeStream;

            Reader = readStream.Reader;
            Writer = writeStream.Writer;
        }
        
        #region Properties
        
        /// <inheritdoc />
        public override bool CanRead => _readStream != null && _readStream.CanRead;
        
        /// <inheritdoc />
        public override bool CanSeek => _readStream != null && _readStream.CanSeek;

        /// <inheritdoc />
        public override bool CanWrite => _writeStream != null && _writeStream.CanWrite;

        /// <inheritdoc />
        public override long Length => throw new NotSupportedException();

        /// <summary>
        /// Indicates if the cancellation is sent to the server.
        /// </summary>
        public virtual bool IsCancellationSent { get; internal set; }

        /// <inheritdoc />
        public int PacketDataLeft => _readStream.PacketDataLeft;

        /// <inheritdoc />
        public TdsStreamPacketType? PacketHeaderType 
        {
            get => _writeStream.PacketHeaderType;
            set => _writeStream.PacketHeaderType = value; 
        }

        /// <inheritdoc />
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public ITdsReader Reader { get; private set; }
        
        /// <inheritdoc />
        public byte ReadPacketStatus => _readStream.ReadPacketStatus;

        /// <inheritdoc />
        public byte ReadPacketHeaderType => _readStream.ReadPacketHeaderType;

        /// <inheritdoc />
        public int Spid => _readStream.Spid;
        
        /// <inheritdoc />
        public ITdsWriter Writer { get; private set; }

        #endregion
        
        #region Methods

        /// <inheritdoc />
        public override async ValueTask DisposeAsync()
        {
            await _readStream.DisposeAsync().ConfigureAwait(false);
            await _writeStream.DisposeAsync().ConfigureAwait(false);
            _readStream = null;
            _writeStream = null;
            Reader = null;
            Writer = null;
        }
        
        /// <inheritdoc />
        public override void Flush() => 
            _writeStream.Flush();
        
        /// <inheritdoc />
        /// <remarks>
        /// Called explicitly by the consumers to flush the stream, which marks the TDS packet as
        /// the last packet in the message and sends it to the server.
        /// </remarks>
        public override async Task FlushAsync(CancellationToken ct)
            => await _writeStream.FlushAsync(ct).ConfigureAwait(false);
        
        /// <inheritdoc />
        public virtual ValueTask<byte> PeekByteAsync(bool isAsync, CancellationToken ct) =>
            _readStream.PeekByteAsync(isAsync, ct);
        
        /// <inheritdoc />
        public virtual void QueueCancellation() =>
            _writeStream.QueueCancellation();
        
        /// <inheritdoc />
        public override int Read(Span<byte> buffer) => 
            _readStream.Read(buffer);

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count) => 
            _readStream.Read(buffer, offset, count);

        /// <inheritdoc />
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken ct) =>
            _readStream.ReadAsync(buffer, ct);
        
        /// <inheritdoc />
        public virtual ValueTask<byte> ReadByteAsync(bool isAsync, CancellationToken ct) =>
            _readStream.ReadByteAsync(isAsync, ct);
        
        /// <inheritdoc />
        public ValueTask<int> ReadBytesAsync(Memory<byte> buffer, bool isAsync, CancellationToken ct) =>
            _readStream.ReadBytesAsync(buffer, isAsync, ct);
        
        /// <inheritdoc />
        public void ReplaceUnderlyingStream(Stream stream)
        {
            _writeStream.ReplaceUnderlyingStream(stream);
            _readStream.ReplaceUnderlyingStream(stream);
        }
        
        /// <summary>
        /// Resets the stream.
        /// </summary>
        /// <remarks>
        /// Useful in some cases for TDS implementation, where we don't want to consume all the
        /// data in the stream, but want to make it available for the next set of operations.
        /// </remarks>
        public virtual void Reset() =>
            throw new NotImplementedException();
        
        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin) => 
            _readStream.Seek(offset, origin);

        /// <inheritdoc />
        public override void SetLength(long value) =>
            throw new NotSupportedException();
        
        /// <inheritdoc />
        public void SetPacketSize(int packetSize)
        {
            _readStream.SetPacketSize(packetSize);
            _writeStream.SetPacketSize(packetSize);
        }

        /// <inheritdoc />
        public virtual ValueTask SkipReadBytesAsync(int skipCount, bool isAsync, CancellationToken ct) =>
            _readStream.SkipReadBytesAsync(skipCount, isAsync, ct);
        
        /// <inheritdoc />
        public override void Write(ReadOnlySpan<byte> buffer) =>
            _writeStream.Write(buffer);

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count) =>
            _writeStream.Write(buffer, offset, count);

        /// <inheritdoc />
        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct) =>
            await _writeStream.WriteAsync(buffer, ct).ConfigureAwait(false);

        /// <inheritdoc />
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken ct) =>
            await _writeStream.WriteAsync(buffer.AsMemory(offset, count), ct).ConfigureAwait(false);

        /// <inheritdoc />
        public ValueTask WriteByteAsync(byte value, bool isAsync, CancellationToken ct) => 
            _writeStream.WriteByteAsync(value, isAsync, ct);

        /// <inheritdoc />
        public ValueTask WriteBytesAsync(Memory<byte> buffer, bool isAsync, CancellationToken ct) =>
            _writeStream.WriteBytesAsync(buffer, isAsync, ct); 
        
        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _readStream?.Dispose();
                _writeStream?.Dispose();
                _readStream = null;
                _writeStream = null;
                Reader = null;
                Writer = null;
            }
            base.Dispose(disposing);
        }
        
        #endregion
    }
}
