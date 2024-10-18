// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

using System;
using System.Net.Security;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class SspiCli
    {
        internal const uint SECQOP_WRAP_NO_ENCRYPT = 0x80000001;

        internal const int SEC_I_RENEGOTIATE = 0x90321;

        internal const int SECPKG_NEGOTIATION_COMPLETE = 0;
        internal const int SECPKG_NEGOTIATION_OPTIMISTIC = 1;

        

        internal enum Endianness
        {
            SECURITY_NETWORK_DREP = 0x00,
            SECURITY_NATIVE_DREP = 0x10,
        }

        internal enum CredentialUse
        {
            SECPKG_CRED_INBOUND = 0x1,
            SECPKG_CRED_OUTBOUND = 0x2,
            SECPKG_CRED_BOTH = 0x3,
        }

        // wincrypt.h
        [StructLayout(LayoutKind.Sequential)]
        internal struct CERT_CHAIN_ELEMENT
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

        // schannel.h
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct SecPkgContext_IssuerListInfoEx
        {
            public SafeHandle aIssuers;
            public uint cIssuers;

            public unsafe SecPkgContext_IssuerListInfoEx(SafeHandle handle, byte[] nativeBuffer)
            {
                aIssuers = handle;
                fixed (byte* voidPtr = nativeBuffer)
                {
                    // TODO (Issue #3114): Properly marshal the struct instead of assuming no padding.
                    cIssuers = *((uint*)(voidPtr + IntPtr.Size));
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SCHANNEL_CRED
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

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct SecBuffer
        {
            public int cbBuffer;
            public SecurityBufferType BufferType;
            public IntPtr pvBuffer;

            public static readonly int Size = sizeof(SecBuffer);
        }

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
}

#endif
