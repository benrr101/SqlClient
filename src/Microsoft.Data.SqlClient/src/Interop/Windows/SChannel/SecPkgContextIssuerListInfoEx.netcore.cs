// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

using System;
using System.Runtime.InteropServices;

namespace Interop_TEMP.Windows.SChannel
{
    // schannel.h
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SecPkgContextIssuerListInfoEx
    {
        public SafeHandle aIssuers;
        public uint cIssuers;

        public SecPkgContextIssuerListInfoEx(SafeHandle handle, byte[] nativeBuffer)
        {
            aIssuers = handle;
            fixed (byte* voidPtr = nativeBuffer)
            {
                // TODO (Issue #3114): Properly marshal the struct instead of assuming no padding.
                cIssuers = *((uint*)(voidPtr + IntPtr.Size));
            }
        }
    }
}

#endif
