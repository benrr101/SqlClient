using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClientX.IO;

#nullable enable
#if NET8_0_OR_GREATER

namespace Microsoft.Data.SqlClientX
{
    internal class TdsParserX
    {
        private readonly ITdsStreamReader _reader;
        private readonly ITdsStreamWriter _writer;
        private TdsParserState _state;

        internal TdsParserX(TdsReader reader, TdsWriter writer)
        {
            _reader = reader;
            _writer = writer;

            _state = TdsParserState.Idle;
        }

        internal async ValueTask ExecuteSqlBatch(string text, bool isAsync, CancellationToken ct)
        {
            // Operation is only permitted in idle state
            if (!InterlockedTransition(TdsParserState.Idle, TdsParserState.Executing))
            {
                throw new InvalidOperationException();
            }

            try
            {
                // Send the batch request to the server
                _writer.PacketType = TdsStreamPacketType.SqlBatch;

                await WriteAllHeaders(isAsync, ct).ConfigureAwait(false);
                await _writer.WriteStringAsync(text, isAsync, ct).ConfigureAwait(false);

                await _writer.FlushPacketAsync(isAsync, ct).ConfigureAwait(false);
            }
            catch
            {
                // Transition to broken state
                _state = TdsParserState.Broken;
                throw;
            }

            // Transition to read metadata state
            _state = TdsParserState.ExecutingRequestSent;

            if (!isAsync)
            {
                // In sync codepaths, we can't continue executing in the background
                return;
            }

            // @TODO: Schedule reading metadata for async mechanism
        }

        internal async ValueTask<object> ReadMetadata(bool isAsync, CancellationToken ct)
        {
            // Operation is only permitted in executing request sent state
            if (!InterlockedTransition(TdsParserState.ExecutingRequestSent, TdsParserState.ExecutingRedingMetadata))
            {
                throw new InvalidOperationException();
            }

            try
            {
                // Read the col data from the stream



            }
            catch
            {
                // Transition to broken state
                _state = TdsParserState.Broken;
                throw;
            }

            // Transition to metadata read state
        }

        internal async ValueTask<object> ReadRow(bool isAsync, CancellationToken ct)
        {

        }

        private bool InterlockedTransition(TdsParserState desiredSourceState, TdsParserState desiredTargetState)
        {
            ref int location = ref Unsafe.As<TdsParserState, int>(ref _state);
            int sourceValue =
                Interlocked.CompareExchange(ref location, (int)desiredTargetState, (int)desiredTargetState);
            return sourceValue == (int)desiredSourceState;
        }

        private async ValueTask<bool> InterlockedStateTransition(TdsParserState source)

        private async ValueTask<object> ReadMetadata(bool isAsync, CancellationToken ct)
        {

        }

        /// <summary>
        /// Writes headers for the packet as per the ALL_HEADERS rule in the TDS specification.
        /// </summary>
        /// <remarks>
        /// This method does not set a packet header or flush the packet when completed. Those
        /// tasks are expected to be completed by the caller.
        /// </remarks>
        private async ValueTask WriteAllHeaders(bool isAsync, CancellationToken ct)
        {
            // @TODO: Ensure this is only performed on >2005 servers (which since that version is no longer supported, do we need to do this?)

            // Determine total length of the headers
            const int marsHeaderLength =
                4 + // Header length
                2 + // Header type
                8 + // Transaction descriptor
                4; // Outstanding request count

            // @TODO: Add length of notifications, trace activity if necessary
            int totalLength = marsHeaderLength;
            await _writer.WriteIntAsync(totalLength, isAsync, ct);

            // Write query notifications
            // @TODO: Send the query notifications header

            // Write transaction descriptor "MARS"
            // @TODO: Support writing non-auto-commit transaction descriptors
            const int autoCommitTransactionDescriptor = 0;
            const int autoCommitOutstandingRequestCount = 1;
            await _writer.WriteIntAsync(autoCommitOutstandingRequestCount, isAsync, ct);
            await _writer.WriteUnsignedLongAsync(autoCommitTransactionDescriptor, isAsync, ct);

            // Write trace activity
            // @TODO: Send trace activity
        }
    }
}

#endif
