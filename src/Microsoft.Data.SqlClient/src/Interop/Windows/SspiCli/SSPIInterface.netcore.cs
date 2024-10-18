// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

using System.Net.Security;
using System.Runtime.InteropServices;
using Interop_TEMP.Windows;
using Interop_TEMP.Windows.SChannel;
using Interop_TEMP.Windows.SspiCli;

namespace System.Net
{
    // SspiCli SSPI interface.
    internal interface SSPIInterface
    {
        SecurityPackageInfoClass[] SecurityPackages { get; set; }
        int EnumerateSecurityPackages(out int pkgnum, out SafeFreeContextBuffer pkgArray);
        int AcquireCredentialsHandle(string moduleName, CredentialUse usage, ref SafeSspiAuthDataHandle authdata, out SafeFreeCredentials outCredential);
        int AcquireDefaultCredential(string moduleName, CredentialUse usage, out SafeFreeCredentials outCredential);
        int AcquireCredentialsHandle(string moduleName, CredentialUse usage, ref SChannelCred authdata, out SafeFreeCredentials outCredential);
        int AcceptSecurityContext(SafeFreeCredentials credential, ref SafeDeleteContext context, SecurityBuffer[] inputBuffers, ContextFlags inFlags, Endianness endianness, SecurityBuffer outputBuffer, ref ContextFlags outFlags);
        int InitializeSecurityContext(ref SafeFreeCredentials credential, ref SafeDeleteContext context, string targetName, ContextFlags inFlags, Endianness endianness, SecurityBuffer inputBuffer, SecurityBuffer outputBuffer, ref ContextFlags outFlags);
        int InitializeSecurityContext(SafeFreeCredentials credential, ref SafeDeleteContext context, string targetName, ContextFlags inFlags, Endianness endianness, SecurityBuffer[] inputBuffers, SecurityBuffer outputBuffer, ref ContextFlags outFlags);
        int EncryptMessage(SafeDeleteContext context, ref SecBufferDesc inputOutput, uint sequenceNumber);
        int DecryptMessage(SafeDeleteContext context, ref SecBufferDesc inputOutput, uint sequenceNumber);
        int MakeSignature(SafeDeleteContext context, ref SecBufferDesc inputOutput, uint sequenceNumber);
        int VerifySignature(SafeDeleteContext context, ref SecBufferDesc inputOutput, uint sequenceNumber);

        int QueryContextChannelBinding(SafeDeleteContext phContext, ContextAttribute attribute, out SafeFreeContextBufferChannelBinding refHandle);
        int QueryContextAttributes(SafeDeleteContext phContext, ContextAttribute attribute, byte[] buffer, Type handleType, out SafeHandle refHandle);
        int QuerySecurityContextToken(SafeDeleteContext phContext, out SecurityContextTokenHandle phToken);
        int CompleteAuthToken(ref SafeDeleteContext refContext, SecurityBuffer[] inputBuffers);
        int ApplyControlToken(ref SafeDeleteContext refContext, SecurityBuffer[] inputBuffers);
    }
}

#endif
