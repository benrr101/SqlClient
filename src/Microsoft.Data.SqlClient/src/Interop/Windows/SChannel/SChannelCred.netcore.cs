// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

using System;
using System.Runtime.InteropServices;

namespace Interop_TEMP.Windows.SChannel
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SChannelCred
    {
        public const int CurrentVersion = 0x4;

        public int dwVersion;
        public int cCreds;

        // ptr to an array of pointers
        // There is a hack done with this field.  AcquireCredentialsHandle requires an array of
        // certificate handles; we only ever use one.  In order to avoid pinning a one element array,
        // we copy this value onto the stack, create a pointer on the stack to the copied value,
        // and replace this field with the pointer, during the call to AcquireCredentialsHandle.
        // Then we fix it up afterwards.  Fine as long as all the SSPI credentials are not
        // supposed to be threadsafe.
        public IntPtr paCred;

        public IntPtr hRootStore;               // == always null, OTHERWISE NOT RELIABLE
        public int cMappers;
        public IntPtr aphMappers;               // == always null, OTHERWISE NOT RELIABLE
        public int cSupportedAlgs;
        public IntPtr palgSupportedAlgs;       // == always null, OTHERWISE NOT RELIABLE
        public int grbitEnabledProtocols;
        public int dwMinimumCipherStrength;
        public int dwMaximumCipherStrength;
        public int dwSessionLifespan;
        public Flags dwFlags;
        public int reserved;

        [Flags]
        public enum Flags
        {
            Zero = 0,
            SCH_CRED_NO_SYSTEM_MAPPER = 0x02,
            SCH_CRED_NO_SERVERNAME_CHECK = 0x04,
            SCH_CRED_MANUAL_CRED_VALIDATION = 0x08,
            SCH_CRED_NO_DEFAULT_CREDS = 0x10,
            SCH_CRED_AUTO_CRED_VALIDATION = 0x20,
            SCH_SEND_AUX_RECORD = 0x00200000,
            SCH_USE_STRONG_CRYPTO = 0x00400000,
        }
    }
}

#endif
