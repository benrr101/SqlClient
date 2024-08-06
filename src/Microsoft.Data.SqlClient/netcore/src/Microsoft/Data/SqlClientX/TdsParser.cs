using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClientX.IO;

namespace Microsoft.Data.SqlClientX
{
    internal class TdsParser
    {
        private readonly ITdsStream _stream;
        private TdsParserState _state;
        
        internal TdsParser(ITdsStream stream)
        {
            _state = TdsParserState.Idle;
            _stream = stream;
        }

        internal ValueTask Execute()
        {
            // Operation is only permitted in idle state
            if (!InterlockedTransition(TdsParserState.Idle, TdsParserState.Executing))
            {
                throw new InvalidOperationException();
            }
            
            
        }

        private bool InterlockedTransition(TdsParserState desiredSourceState, TdsParserState desiredTargetState)
        {
            ref int location = ref Unsafe.As<TdsParserState, int>(ref _state);
            int sourceValue = Interlocked.CompareExchange(ref location, (int)desiredTargetState, (int)desiredTargetState);
            return sourceValue == (int)desiredSourceState;
        }
    }
}
