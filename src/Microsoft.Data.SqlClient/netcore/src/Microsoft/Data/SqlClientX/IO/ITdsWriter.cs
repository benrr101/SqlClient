using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Data.SqlClientX.IO
{
    /// <summary>
    /// Interface for a class that implements methods for writing various data types to a TDS stream.
    /// </summary>
    public interface ITdsWriter
    {
        /// <summary>
        /// Writes a byte to the underlying TDS stream.
        /// </summary>
        /// <param name="value">Value to write</param>
        /// <param name="isAsync">Whether the method should be executed asynchronously.</param>
        /// <param name="ct">Cancellation token</param>
        ValueTask WriteByteAsync(byte value, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Writes bytes directly to the underlying TDS stream.
        /// </summary>
        /// <param name="bytes">Bytes to write.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        ValueTask WriteBytesAsync(Memory<byte> bytes, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Writes double precision floating point value to the underlying TDS stream, as
        /// little-endian.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        ValueTask WriteDoubleAsync(double value, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Writes a single precision floating point value to the underlying TDS stream, as
        /// little-endian.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        ValueTask WriteFloatAsync(float value, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Writes int value to the underlying TDS stream, as little-endian.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        ValueTask WriteIntAsync(int value, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Writes long value to the underlying TDS stream, as little-endian.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        ValueTask WriteLongAsync(long value, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Writes the least significant 2 bytes as short value to the underlying TDS stream, as
        /// little-endian.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        ValueTask WriteShortAsync(int value, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Writes short value to out to the underlying TDS stream, as little-endian.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        ValueTask WriteShortAsync(short value, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Writes unsigned int value to the underlying TDS stream, as little-endian.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        ValueTask WriteUIntAsync(uint value, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Writes unsigned long value to the underlying TDS stream, as little-endian.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        ValueTask WriteULongAsync(ulong value, bool isAsync, CancellationToken ct);

        /// <summary>
        /// Writes unsigned short value to the underlying TDS stream, as little-endian.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="isAsync">Whether caller method is executing asynchronously.</param>
        /// <param name="ct">Cancellation token.</param>
        ValueTask WriteUShortAsync(ushort value, bool isAsync, CancellationToken ct);
    }
}
