// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

using System.Runtime.InteropServices;

namespace Interop_TEMP.Windows.SspiCli
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SecBufferDesc
    {
        public readonly int ulVersion;
        public readonly int cBuffers;
        public void* pBuffers;

        public SecBufferDesc(int count)
        {
            ulVersion = 0;
            cBuffers = count;
            pBuffers = null;
        }
    }
}

#endif
