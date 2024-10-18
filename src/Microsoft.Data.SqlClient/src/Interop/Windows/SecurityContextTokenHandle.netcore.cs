// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

using System;
using System.Threading;
using Interop_TEMP.Windows.Kernel32;
using Microsoft.Win32.SafeHandles;

namespace Interop_TEMP.Windows
{
    #if DEBUG
    internal sealed class SecurityContextTokenHandle : DebugCriticalHandleZeroOrMinusOneIsInvalid
    #else
    internal sealed class SecurityContextTokenHandle : CriticalHandleZeroOrMinusOneIsInvalid
    #endif
    {
        private int _disposed;

        private SecurityContextTokenHandle() : base()
        {
        }

        internal IntPtr DangerousGetHandle()
        {
            return handle;
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                if (Interlocked.Increment(ref _disposed) == 1)
                {
                    return Kernel32.Kernel32.CloseHandle(handle);
                }
            }
            return true;
        }
    }
}

#endif
