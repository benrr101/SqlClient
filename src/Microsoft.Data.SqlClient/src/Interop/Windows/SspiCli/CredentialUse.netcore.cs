// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

namespace Interop_TEMP.Windows.SspiCli
{
    internal enum CredentialUse
    {
        SECPKG_CRED_INBOUND = 0x1,
        SECPKG_CRED_OUTBOUND = 0x2,
        SECPKG_CRED_BOTH = 0x3,
    }
}

#endif
