// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

using System;
using System.Runtime.InteropServices;

namespace Interop_TEMP.Windows.Crypt32
{
    internal static class Crypt32
    {
        private const string DllName = "crypt32.dll";

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CertFreeCertificateContext(IntPtr pCertContext);
    }
}

#endif
