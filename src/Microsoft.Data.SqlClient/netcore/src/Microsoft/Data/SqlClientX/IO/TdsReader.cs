// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Data.SqlClientX.IO
{
    /// <summary>
    /// This class provides helper methods for reading bytes using <see cref="TdsStream"/>
    /// It extends <see cref="TdsBufferManager"/> that manages allocations of bytes buffer for better memory management.
    /// </summary>
    internal sealed class TdsReader : TdsBufferManager, ITdsReader
    {
        private readonly TdsReadStream _tdsStream;
        private readonly BinaryReader _reader;

        /// <summary>
        /// Instantiate TdsReader with <see cref="TdsStream" />
        /// </summary>
        /// <param name="stream">Tds Stream instance to work with.</param>
        public TdsReader(TdsReadStream stream)
        {
            _tdsStream = stream;
            _reader = new BinaryReader(stream, Encoding.Unicode);
        }

        #region Public APIs

        /// <inheritdoc />
        public ValueTask<byte> ReadByteAsync(bool isAsync, CancellationToken ct) =>
            _tdsStream.ReadByteAsync(isAsync, ct);

        /// <inheritdoc />
        public ValueTask<int> ReadBytesAsync(Memory<byte> buffer, bool isAsync, CancellationToken ct) =>
            _tdsStream.ReadBytesAsync(buffer, isAsync, ct);

        /// <inheritdoc />
        public ValueTask<char> ReadCharAsync(bool isAsync, CancellationToken ct)
            => isAsync
            ? ReadCharInternalAsync(ct)
            : new ValueTask<char>(_reader.ReadChar());

        /// <inheritdoc />
        public async ValueTask<char[]> ReadCharsAsync(int length, bool isAsync, CancellationToken ct)
        {
            if (!isAsync)
            {
                return _reader.ReadChars(length);
            }

            // Calculate byte length for char array
            int byteLength = sizeof(char) * length;

            // Allocate byte array to store the result
            byte[] bytes = new byte[byteLength];

            // Read bytes asynchronously
            await ReadBytesAsync(new ArraySegment<byte>(bytes), isAsync, ct).ConfigureAwait(false);

            // Convert bytes to char array
            char[] chars = new char[length];
            Encoding.Unicode.GetChars(bytes, chars);
            return chars;
        }

        /// <inheritdoc />
        public ValueTask<double> ReadDoubleAsync(bool isAsync, CancellationToken ct)
            => isAsync
                ? ReadDoubleInternalAsync(ct)
                : new ValueTask<double>(_reader.ReadDouble());

        /// <inheritdoc />
        public ValueTask<float> ReadFloatAsync(bool isAsync, CancellationToken ct)
            => isAsync
                ? ReadSingleInternalAsync(ct)
                : new ValueTask<float>(_reader.ReadSingle());

        /// <inheritdoc />
        public ValueTask<int> ReadIntAsync(bool isAsync, CancellationToken ct)
            => isAsync
                ? ReadInt32InternalAsync(ct)
                : new ValueTask<int>(_reader.ReadInt32());

        /// <inheritdoc />
        public ValueTask<long> ReadLongAsync(bool isAsync, CancellationToken ct)
            => isAsync
                ? ReadInt64InternalAsync(ct)
                : new ValueTask<long>(_reader.ReadInt64());

        /// <inheritdoc />
        public ValueTask<short> ReadShortAsync(bool isAsync, CancellationToken ct)
            => isAsync
                ? ReadInt16InternalAsync(ct)
                : new ValueTask<short>(_reader.ReadInt16());

        /// <inheritdoc />
        public ValueTask<uint> ReadUIntAsync(bool isAsync, CancellationToken ct)
            => isAsync
                ? ReadUInt32InternalAsync(ct)
                : new ValueTask<uint>(_reader.ReadUInt32());

        /// <inheritdoc />
        public ValueTask<ulong> ReadULongAsync(bool isAsync, CancellationToken ct)
            => isAsync
                ? ReadUInt64InternalAsync(ct)
                : new ValueTask<ulong>(_reader.ReadUInt64());

        /// <inheritdoc />
        public ValueTask<ushort> ReadUShortAsync(bool isAsync, CancellationToken ct)
            => isAsync
                ? ReadUInt16InternalAsync(ct)
                : new ValueTask<ushort>(_reader.ReadUInt16());

        /// <inheritdoc />
        public async ValueTask<string> ReadStringAsync(int length, bool isAsync, CancellationToken ct)
        {
            int byteLength = length * 2; // 2 bytes per char
            byte[] buffer = new byte[byteLength];

            int bytesRead = isAsync
                ? await _tdsStream.ReadAsync(buffer.AsMemory(0, byteLength), ct).ConfigureAwait(false)
                : _tdsStream.Read(buffer.AsSpan(0, byteLength));

            return Encoding.Unicode.GetString(buffer, 0, bytesRead);
        }

        /// <inheritdoc />
        public ValueTask<string> ReadStringAsync(int length, Encoding encoding, bool isPlp, bool isAsync, CancellationToken ct)
            // TODO Implement PLP reading support
            => throw new NotImplementedException();

        #endregion

        #region Private helpers

        private ValueTask<char> ReadCharInternalAsync(CancellationToken ct)
        {
            Memory<byte> buffer = GetBuffer(sizeof(char));
            ValueTask<int> task = ReadBytesAsync(buffer, true, ct);

            return task.IsCompleted
                ? new ValueTask<char>(doWork())
                : new ValueTask<char>(task.AsTask().ContinueWith((task) => doWork()));

            char doWork()
            {
                byte byte1 = buffer.Span[1];
                byte byte0 = buffer.Span[0];

                return (char)((byte1 << 8) + byte0);
            }
        }

        private ValueTask<short> ReadInt16InternalAsync(CancellationToken ct)
        {
            Memory<byte> buffer = GetBuffer(sizeof(short));
            ValueTask<int> task = ReadBytesAsync(buffer, true, ct);

            return task.IsCompleted
                ? new ValueTask<short>(doWork())
                : new ValueTask<short>(task.AsTask().ContinueWith((task) => doWork()));

            short doWork() => BinaryPrimitives.ReadInt16LittleEndian(buffer.Span);
        }

        private ValueTask<int> ReadInt32InternalAsync(CancellationToken ct)
        {
            Memory<byte> buffer = GetBuffer(sizeof(int));
            ValueTask<int> task = ReadBytesAsync(buffer, true, ct);

            return task.IsCompleted
                ? new ValueTask<int>(doWork())
                : new ValueTask<int>(task.AsTask().ContinueWith((task) => doWork()));

            int doWork() => BinaryPrimitives.ReadInt32LittleEndian(buffer.Span);
        }

        private ValueTask<long> ReadInt64InternalAsync(CancellationToken ct)
        {
            Memory<byte> buffer = GetBuffer(sizeof(long));
            ValueTask<int> task = ReadBytesAsync(buffer, true, ct);

            return task.IsCompleted
                ? new ValueTask<long>(doWork())
                : new ValueTask<long>(task.AsTask().ContinueWith((task) => doWork()));

            long doWork() => BinaryPrimitives.ReadInt64LittleEndian(buffer.Span);
        }

        private ValueTask<ushort> ReadUInt16InternalAsync(CancellationToken ct)
        {
            Memory<byte> buffer = GetBuffer(sizeof(ushort));
            ValueTask<int> task = ReadBytesAsync(buffer, true, ct);

            return task.IsCompleted
                ? new ValueTask<ushort>(doWork())
                : new ValueTask<ushort>(task.AsTask().ContinueWith((task) => doWork()));

            ushort doWork() => BinaryPrimitives.ReadUInt16LittleEndian(buffer.Span);
        }

        private ValueTask<uint> ReadUInt32InternalAsync(CancellationToken ct)
        {
            Memory<byte> buffer = GetBuffer(sizeof(uint));
            ValueTask<int> task = ReadBytesAsync(buffer, true, ct);

            return task.IsCompleted
                ? new ValueTask<uint>(doWork())
                : new ValueTask<uint>(task.AsTask().ContinueWith((task) => doWork()));

            uint doWork() => BinaryPrimitives.ReadUInt32LittleEndian(buffer.Span);
        }

        private ValueTask<ulong> ReadUInt64InternalAsync(CancellationToken ct)
        {
            Memory<byte> buffer = GetBuffer(sizeof(ulong));
            ValueTask<int> task = ReadBytesAsync(buffer, true, ct);

            return task.IsCompleted
                ? new ValueTask<ulong>(doWork())
                : new ValueTask<ulong>(task.AsTask().ContinueWith((task) => doWork()));

            ulong doWork() => BinaryPrimitives.ReadUInt64LittleEndian(buffer.Span);
        }

        private ValueTask<float> ReadSingleInternalAsync(CancellationToken ct)
        {
            Memory<byte> buffer = GetBuffer(sizeof(float));
            ValueTask<int> task = ReadBytesAsync(buffer, true, ct);

            return task.IsCompleted
                ? new ValueTask<float>(doWork())
                : new ValueTask<float>(task.AsTask().ContinueWith((task) => doWork()));

            float doWork() => BinaryPrimitives.ReadSingleLittleEndian(buffer.Span);
        }

        private ValueTask<double> ReadDoubleInternalAsync(CancellationToken ct)
        {
            Memory<byte> buffer = GetBuffer(sizeof(double));
            ValueTask<int> task = ReadBytesAsync(buffer, true, ct);

            return task.IsCompleted
                ? new ValueTask<double>(doWork())
                : new ValueTask<double>(task.AsTask().ContinueWith((task) => doWork()));

            double doWork() => BinaryPrimitives.ReadDoubleLittleEndian(buffer.Span);
        }

        #endregion
    }
}
