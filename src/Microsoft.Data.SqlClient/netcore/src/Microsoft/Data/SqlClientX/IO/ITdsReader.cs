using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Data.SqlClientX.IO
{
    /// <summary>
    /// Interface for a class that implements methods for read various data types from a TDS stream.
    /// </summary>
    public interface ITdsReader
    {
        /// <summary>
        /// Reads a single byte from the underlying TDS stream.
        /// </summary>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns a single byte from the stream. If
        /// <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<byte> ReadByteAsync(bool isAsync, CancellationToken ct);
        
        /// <summary>
        /// Reads bytes from the underlying TDS stream into the provided <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">
        /// Bytes read from the stream will be placed in this buffer. Bytes will be read in
        /// starting at index 0 and continue to the length of the buffer or number of bytes
        /// available from the stream, whichever comes first.
        /// </param>
        /// <param name="isAsync">Whether the call should be made asynchronously or synchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns the number of bytes read from the stream. If
        /// <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<int> ReadBytesAsync(Memory<byte> buffer, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Reads a single unicode character from the underlying TDS stream, little-endian.
        /// </summary>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns a single unicode character from the stream. If
        /// <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<char> ReadCharAsync(bool isAsync, CancellationToken ct);
        
        /// <summary>
        /// Reads unicode characters from the underlying TDS stream, little-endian.
        /// </summary>
        /// <param name="length">Desired number of characters to read.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns an array of unicode characters read from the
        /// stream. If <paramref name="isAsync"/> is <c>false</c>, the task will be completed
        /// synchronously.
        /// </returns>
        ValueTask<char[]> ReadCharsAsync(int length, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Reads a double precision floating point value from the underlying TDS stream,
        /// little-endian.
        /// </summary>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns a single double from the stream. If
        /// <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<double> ReadDoubleAsync(bool isAsync, CancellationToken ct);
        
        /// <summary>
        /// Reads a single precision floating point value from the underlying TDS stream,
        /// little-endian.
        /// </summary>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns a single float from the stream. If
        /// <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<float> ReadFloatAsync(bool isAsync, CancellationToken ct);
        
        /// <summary>
        /// Reads an int value from the underlying TDS stream, little-endian.
        /// </summary>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns a single int from the stream. If
        /// <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<int> ReadIntAsync(bool isAsync, CancellationToken ct);

        /// <summary>
        /// Reads a long value from the underlying TDS stream, little-endian.
        /// </summary>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns a single long from the stream. If
        /// <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<long> ReadLongAsync(bool isAsync, CancellationToken ct);
        
        /// <summary>
        /// Reads a short value from the underlying TDS stream, little-endian.
        /// </summary>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns a single short from the stream. If
        /// <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<short> ReadShortAsync(bool isAsync, CancellationToken ct);

        /// <summary>
        /// Reads a unicode string from the underlying TDS stream, little endian.
        /// </summary>
        /// <param name="length">Length of the string to read, in characters.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns a unicode string from the stream. If
        /// <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<string> ReadStringAsync(int length, bool isAsync, CancellationToken ct);
        
        /// <summary>
        /// Reads a string from the underlying TDS stream, using the provided
        /// <paramref name="encoding"/> to decode the bytes into a string.
        /// </summary>
        /// <param name="length">Length of the string to read, in characters.</param>
        /// <param name="encoding">Encoding to use to decode the bytes into a string.</param>
        /// <param name="isPlp">Whether the string is PLP data.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns a string of the desired encoding from the stream.
        /// If <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<string> ReadStringAsync(int length, Encoding encoding, bool isPlp, bool isAsync, CancellationToken ct);
        
        /// <summary>
        /// Reads an unsigned int value from the underlying TDS stream, little-endian.
        /// </summary>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns a single uint from the stream. If
        /// <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<uint> ReadUIntAsync(bool isAsync, CancellationToken ct);

        /// <summary>
        /// Reads an unsigned long value from the underlying TDS stream, little-endian.
        /// </summary>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns a single ulong from the stream. If
        /// <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<ulong> ReadULongAsync(bool isAsync, CancellationToken ct);
        
        /// <summary>
        /// Reads an unsigned short value from the underlying TDS stream, little-endian.
        /// </summary>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <see cref="ValueTask"/> that returns a single ushort from the stream. If
        /// <paramref name="isAsync"/> is <c>false</c> the task will be completed synchronously.
        /// </returns>
        ValueTask<ushort> ReadUShortAsync(bool isAsync, CancellationToken ct);
    }
}
