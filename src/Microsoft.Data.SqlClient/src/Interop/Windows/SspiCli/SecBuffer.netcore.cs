// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

using System;
using System.Net.Security;
using System.Runtime.InteropServices;

namespace Interop_TEMP.Windows.SspiCli
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SecBuffer
    {
        public int cbBuffer;
        public SecurityBufferType BufferType;
        public IntPtr pvBuffer;

        public static readonly int Size = sizeof(SecBuffer);
    }
}

#endif
