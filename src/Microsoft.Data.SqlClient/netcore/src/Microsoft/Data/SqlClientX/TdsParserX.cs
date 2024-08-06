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
        private readonly TdsReader _reader;
        private readonly TdsWriter _writer;
        private TdsParserState _state;
        
        internal TdsParserX(TdsReader reader, TdsWriter writer)
        {
            _reader = reader;
            _writer = writer;

            _state = TdsParserState.Idle;
        }

        internal ValueTask ExecuteSqlBatch(string text, bool isAsync, CancellationToken ct)
        {
            // Operation is only permitted in idle state
            if (!InterlockedTransition(TdsParserState.Idle, TdsParserState.Executing))
            {
                throw new InvalidOperationException();
            }

            int executeState = 0;
            while (executeState >= 0)
            {
                switch (executeState)
                {
                    case 0:
                        // Step 0: Send the batch request to the server

                        executeState = 1;
                        break;

                    case 1:
                        break;
                }
            }
        }

        private bool InterlockedTransition(TdsParserState desiredSourceState, TdsParserState desiredTargetState)
        {
            ref int location = ref Unsafe.As<TdsParserState, int>(ref _state);
            int sourceValue = Interlocked.CompareExchange(ref location, (int)desiredTargetState, (int)desiredTargetState);
            return sourceValue == (int)desiredSourceState;
        }

        private ValueTask WriteRpcBatchHeaders()
        {
            // @TODO: Ensure this is only performed on >2005 servers (which since that version is no longer supported, do we need to do this?)

        }
    }
}

#endif
