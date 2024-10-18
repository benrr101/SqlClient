// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

using System;
using System.Runtime.InteropServices;

namespace Interop_TEMP.Windows.SspiCli
{
    // wincrypt.h
    [StructLayout(LayoutKind.Sequential)]
    internal struct CertChainElement
    {
        public uint cbSize;
        public IntPtr pCertContext;
        // Since this structure is allocated by unmanaged code, we can
        // omit the fields below since we don't need to access them
        // CERT_TRUST_STATUS   TrustStatus;
        // IntPtr                pRevocationInfo;
        // IntPtr                pIssuanceUsage;
        // IntPtr                pApplicationUsage;
    }
}

#endif
