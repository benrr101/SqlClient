﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Microsoft.Data.SqlClientX.IO
{
    /// <summary>
    /// A stream which handles reading TDS messages from the wire, and extracting the data from the 
    /// packets in the message packets.
    /// </summary>
    internal class TdsReadStream : Stream, ITdsReadStream
    {
        // TODO: Handle Cancellation tokens in all async paths.
        // TODO: For large data reads, is it possible to write to the incoming buffer? This needs to be benchmarked because 
        // we will need the packet header to be read in the buffer. In this case we could either ask underlying stream to provide
        // only the header data, and then post another data read into the incoming byte array from Read* APIs. Will this be faster 
        // than copying the data from the buffer to the incoming byte array? Need to benchmark.

        #region Private Fields
        
        private Stream _underlyingStream;

        // The buffer to hold the TDS read data.
        private byte[] _readBuffer;

        // The read pointer inside the buffer.
        private int _readIndex = 0;

        // The end of the data index in the buffer.
        private int _readBufferDataEnd = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// The constructor for the TdsReadStream.
        /// </summary>
        /// <param name="underlyingStream">The underlying stream to read from</param>
        public TdsReadStream(Stream underlyingStream)
        {
            _underlyingStream = underlyingStream;

            Reader = new TdsReader(this);
            _readBuffer = new byte[TdsEnums.DEFAULT_LOGIN_PACKET_SIZE];
        }

        #endregion

        #region Public Properties
        
        /// <inheritdoc />
        public override bool CanRead => _underlyingStream != null;

        /// <inheritdoc />
        public override bool CanSeek => false;

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override long Length => throw new NotSupportedException();

        /// <inheritdoc />
        public virtual int PacketDataLeft { get; private set; }

        /// <inheritdoc />
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public ITdsReader Reader { get; private set; }
        
        /// <inheritdoc />
        public virtual byte ReadPacketHeaderType { get; private set; }

        /// <inheritdoc />
        public virtual byte ReadPacketStatus { get; private set; }

        /// <inheritdoc />
        public virtual int Spid { get; private set; }

        #endregion

        #region Public Methods
        
        /// <inheritdoc />
        public override async ValueTask DisposeAsync()
        {
            await _underlyingStream.DisposeAsync();
            _underlyingStream = null;
            _readBuffer = null;
        }

        /// <inheritdoc />
        public override void Flush() => throw new NotSupportedException();

        /// <inheritdoc />
        public async ValueTask<byte> PeekByteAsync(bool isAsync, CancellationToken ct)
        {
            // If we have logically finished reading the packet, or if we have 
            // reached the end of the buffer, then we need to position the buffer at the beginning of next
            // packet start.
            await PrepareBufferAsync(isAsync, ct).ConfigureAwait(false);
            return _readBuffer[_readIndex];
        }

        /// <inheritdoc />
        public override int Read(Span<byte> buffer)
        {
            int lengthToFill = buffer.Length;
            int totalRead = 0;
            while (lengthToFill > 0)
            {
                ValueTask vt = PrepareBufferAsync(isAsync: false, CancellationToken.None);
                Debug.Assert(vt.IsCompletedSuccessfully,
                    "The Value task should have completed successfully, since the call was synchronous");

                int lengthToCopy = MinDataAvailableBeforeRead(lengthToFill);
                ReadOnlySpan<byte> copyFrom = _readBuffer.AsSpan(_readIndex, lengthToCopy);
                copyFrom.CopyTo(buffer.Slice(totalRead, lengthToFill));
                totalRead += lengthToCopy;
                lengthToFill -= lengthToCopy;
                AdvanceBufferOnRead(lengthToCopy);
            }
            return totalRead;
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count) =>
            Read(buffer.AsSpan(offset, count));

        /// <inheritdoc />
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken ct)
        {
            int lengthToFill = buffer.Length;
            int totalRead = 0;
            while (lengthToFill > 0)
            {
                await PrepareBufferAsync(isAsync: true, ct).ConfigureAwait(false);
                int lengthToCopy = MinDataAvailableBeforeRead(lengthToFill);
                ReadOnlyMemory<byte> copyFrom = new ReadOnlyMemory<byte>(_readBuffer, _readIndex, lengthToCopy);
                copyFrom.CopyTo(buffer.Slice(totalRead, lengthToFill));
                totalRead += lengthToCopy;
                lengthToFill -= lengthToCopy;
                AdvanceBufferOnRead(lengthToCopy);
            }
            return totalRead;
        }

        /// <inheritdoc />
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken ct)
        {
            return await ReadAsync(buffer.AsMemory(offset, count), ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public ValueTask<int> ReadBytesAsync(Memory<byte> buffer, bool isAsync, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return isAsync
                ? ReadAsync(buffer, ct)
                : new ValueTask<int>(Read(buffer.Span));
        }
        
        /// <inheritdoc />
        public void ReplaceUnderlyingStream(Stream stream) => 
            _underlyingStream = stream;

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin) => 
            throw new NotSupportedException();

        /// <inheritdoc />
        public override void SetLength(long value) => 
            throw new NotSupportedException();

        /// <inheritdoc />
        public void SetPacketSize(int bufferSize) =>
            _readBuffer = new byte[bufferSize];

        /// <inheritdoc />
        public async ValueTask SkipReadBytesAsync(int skipCount, bool isAsync, CancellationToken ct)
        {
            int lengthLeftToSkip = skipCount;
            while (lengthLeftToSkip > 0)
            {
                await PrepareBufferAsync(isAsync, ct).ConfigureAwait(false);
                int skippableMinLength = MinDataAvailableBeforeRead(lengthLeftToSkip);
                lengthLeftToSkip -= skippableMinLength;
                AdvanceBufferOnRead(skippableMinLength);
            }
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count) =>
            throw new NotSupportedException();

        /// <inheritdoc />
        public async ValueTask<byte> ReadByteAsync(bool isAsync, CancellationToken ct)
        {
            await PrepareBufferAsync(isAsync, ct).ConfigureAwait(false);
            byte result = _readBuffer[_readIndex];
            AdvanceBufferOnRead(1);
            return result;
        }

        #endregion
        
        #region Protected Methods

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _underlyingStream.Dispose();
                _underlyingStream = null;
                _readBuffer = null;
                Reader = null;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Advances the buffer read index, and the packet data left count, by the specified length.
        /// </summary>
        /// <param name="length">The length to advance the packet by.</param>
        private void AdvanceBufferOnRead(int length)
        {
            _readIndex += length;
            PacketDataLeft -= length;
        }

        /// <summary>
        /// Computes the minimum byte count available for copying into the buffer.
        /// </summary>
        /// <param name="maxByteCountExpected">The maximum bytes count expected by the caller.</param>
        /// <returns></returns>
        private int MinDataAvailableBeforeRead(int maxByteCountExpected)
        {
            // We can only read the minimum of what is left in the packet,
            // what is left in the buffer, and what we need to fill
            // If we have the max Byte Count available, then we read it
            // else we will read either the data in packet, or the 
            // data in buffer, whichever is smaller.
            // If the data spans multiple packets, then the caller will go ahead and post a network read.
            return Math.Min(Math.Min(PacketDataLeft, _readBufferDataEnd - _readIndex), maxByteCountExpected);
        }

        /// <summary>
        /// Prepares the Read buffer with more data if the buffer or the packet is empty. 
        /// This method is called, when the data from existing buffer is completely read, or there 
        /// is less data available in the buffer, than what is needed to complete the read call.
        /// </summary>
        /// <param name="isAsync"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async ValueTask PrepareBufferAsync(bool isAsync, CancellationToken ct)
        {
            bool shouldReadMoreData = PacketDataLeft == 0 || _readBufferDataEnd == _readIndex;

            // Fail fast in case we don't need to prepare the buffer.
            if (!shouldReadMoreData)
            {
                return;
            }
            // We have read all the data from the packet as stated in the header, this means that we have to 
            // process the next packet header.
            if (PacketDataLeft == 0 && _readBufferDataEnd > _readIndex)
            {
                await ProcessHeaderAsync(isAsync, ct).ConfigureAwait(false);
            }

            // There is no data left in the buffer.
            if (_readIndex == _readBufferDataEnd)
            {
                // 1.1 If we have left over data indicated in the packet header, then we simply need to get data from the network.
                if (PacketDataLeft > 0)
                {
                    _readBufferDataEnd = isAsync
                        ? await _underlyingStream.ReadAsync(_readBuffer, ct).ConfigureAwait(false)
                        : _underlyingStream.Read(_readBuffer);
                    _readIndex = 0;
                }
                // 1.2. There is no data left as indicated by packet header and the buffer is empty.
                else if (PacketDataLeft == 0)
                {
                    _readBufferDataEnd = isAsync
                        ? await _underlyingStream.ReadAsync(_readBuffer, ct).ConfigureAwait(false)
                        : _underlyingStream.Read(_readBuffer);

                    _readIndex = 0;

                    await ProcessHeaderAsync(isAsync, ct).ConfigureAwait(false);

                    // 1.3. After processing the packet header, there is a possibility that the transport read didn't
                    // return any more data for the packet. In that case, post another read to have packet data ready..
                    if (_readBufferDataEnd == _readIndex)
                    {
                        _readBufferDataEnd = isAsync
                            ? await _underlyingStream.ReadAsync(_readBuffer, ct).ConfigureAwait(false)
                            : _underlyingStream.Read(_readBuffer);

                        _readIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the header of the packet, and extracts the data from the header.
        /// If needed, this function will read more data from the network to complete the header.
        /// </summary>
        /// <param name="isAsync">Whether this method is invoked asynchronously.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        private async ValueTask ProcessHeaderAsync(bool isAsync, CancellationToken ct)
        {
            int headerDataAvailable = _readBufferDataEnd - _readIndex;
            int bytesNeededToCompleteHeader = TdsEnums.HEADER_LEN - headerDataAvailable;

            // We have less than the header length available in the buffer, so we need to read more data, to atleast complete 
            // the header.
            if (headerDataAvailable < TdsEnums.HEADER_LEN)
            {
                // We move the header information to the beginning of the buffer.
                Buffer.BlockCopy(_readBuffer, _readIndex, _readBuffer, 0, headerDataAvailable);
                _readBufferDataEnd = headerDataAvailable;
                _readIndex = 0;

                while (bytesNeededToCompleteHeader > 0)
                {
                    int bytesRead = isAsync
                        ? await _underlyingStream.ReadAsync(_readBuffer.AsMemory(_readBufferDataEnd), ct).ConfigureAwait(false)
                        : _underlyingStream.Read(_readBuffer.AsSpan(_readBufferDataEnd));
                    
                    // Reduce the number of bytes needed
                    bytesNeededToCompleteHeader -= bytesRead;
                    _readBufferDataEnd += bytesRead;
                }
            }

            ReadPacketHeaderType = _readBuffer[_readIndex];
            ReadPacketStatus = _readBuffer[_readIndex + 1];
            PacketDataLeft = BinaryPrimitives.ReadUInt16BigEndian(_readBuffer.AsSpan(_readIndex + TdsEnums.HEADER_LEN_FIELD_OFFSET, 2)) - TdsEnums.HEADER_LEN;
            Spid = BinaryPrimitives.ReadUInt16BigEndian(_readBuffer.AsSpan(_readIndex + TdsEnums.SPID_OFFSET, 2));
            // Position the read index to the start of the packet data.
            _readIndex += TdsEnums.HEADER_LEN;
        }

        #endregion
    }
}
