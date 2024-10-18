// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

using System;
using System.Net.Security;
using System.Runtime.InteropServices;
using Interop_TEMP.Windows.SChannel;

namespace Interop_TEMP.Windows.SspiCli
{
    internal static class SspiCli
    {
        private const string DllName = "sspicli.dll";
        
        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern int EncryptMessage(
            ref Interop.SspiCli.CredHandle contextHandle,
            [In] uint qualityOfProtection,
            [In, Out] ref Interop.SspiCli.SecBufferDesc inputOutput,
            [In] uint sequenceNumber);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern unsafe int DecryptMessage(
            [In] ref Interop.SspiCli.CredHandle contextHandle,
            [In, Out] ref Interop.SspiCli.SecBufferDesc inputOutput,
            [In] uint sequenceNumber,
            uint* qualityOfProtection);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern int QuerySecurityContextToken(
            ref Interop.SspiCli.CredHandle phContext,
            [Out] out SecurityContextTokenHandle handle);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern int FreeContextBuffer([In] IntPtr contextBuffer);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern int FreeCredentialsHandle(ref Interop.SspiCli.CredHandle handlePtr);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern int DeleteSecurityContext(ref Interop.SspiCli.CredHandle handlePtr);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern unsafe int AcceptSecurityContext(
            ref Interop.SspiCli.CredHandle credentialHandle,
            [In] void* inContextPtr,
            [In] Interop.SspiCli.SecBufferDesc* inputBuffer,
            [In] Interop.SspiCli.ContextFlags inFlags,
            [In] Interop.SspiCli.Endianness endianness,
            ref Interop.SspiCli.CredHandle outContextPtr,
            [In, Out] ref Interop.SspiCli.SecBufferDesc outputBuffer,
            [In, Out] ref Interop.SspiCli.ContextFlags attributes,
            out long timeStamp);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern unsafe int QueryContextAttributesW(
            ref Interop.SspiCli.CredHandle contextHandle,
            [In] Interop.SspiCli.ContextAttribute attribute,
            [In] void* buffer);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern int SetContextAttributesW(
            ref Interop.SspiCli.CredHandle contextHandle,
            [In] Interop.SspiCli.ContextAttribute attribute,
            [In] byte[] buffer,
            [In] int bufferSize);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern int EnumerateSecurityPackagesW(
            [Out] out int pkgnum,
            [Out] out SafeFreeContextBuffer_SECURITY handle);

        [DllImport(DllName, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern unsafe int AcquireCredentialsHandleW(
            [In] string principal,
            [In] string moduleName,
            [In] int usage,
            [In] void* logonID,
            [In] IntPtr zero,
            [In] void* keyCallback,
            [In] void* keyArgument,
            ref Interop.SspiCli.CredHandle handlePtr,
            [Out] out long timeStamp);

        [DllImport(DllName, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern unsafe int AcquireCredentialsHandleW(
            [In] string principal,
            [In] string moduleName,
            [In] int usage,
            [In] void* logonID,
            [In] SafeSspiAuthDataHandle authdata,
            [In] void* keyCallback,
            [In] void* keyArgument,
            ref Interop.SspiCli.CredHandle handlePtr,
            [Out] out long timeStamp);

        [DllImport(DllName, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern unsafe int AcquireCredentialsHandleW(
            [In] string principal,
            [In] string moduleName,
            [In] int usage,
            [In] void* logonID,
            [In] ref Interop.SspiCli.SCHANNEL_CRED authData,
            [In] void* keyCallback,
            [In] void* keyArgument,
            ref Interop.SspiCli.CredHandle handlePtr,
            [Out] out long timeStamp);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern unsafe int InitializeSecurityContextW(
            ref Interop.SspiCli.CredHandle credentialHandle,
            [In] void* inContextPtr,
            [In] byte* targetName,
            [In] Interop.SspiCli.ContextFlags inFlags,
            [In] int reservedI,
            [In] Interop.SspiCli.Endianness endianness,
            [In] Interop.SspiCli.SecBufferDesc* inputBuffer,
            [In] int reservedII,
            ref Interop.SspiCli.CredHandle outContextPtr,
            [In, Out] ref Interop.SspiCli.SecBufferDesc outputBuffer,
            [In, Out] ref Interop.SspiCli.ContextFlags attributes,
            out long timeStamp);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern unsafe int CompleteAuthToken(
            [In] void* inContextPtr,
            [In, Out] ref Interop.SspiCli.SecBufferDesc inputBuffers);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern unsafe int ApplyControlToken(
            [In] void* inContextPtr,
            [In, Out] ref Interop.SspiCli.SecBufferDesc inputBuffers);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        internal static extern SecurityStatus SspiFreeAuthIdentity([In] IntPtr authData);

        [DllImport(DllName, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SecurityStatus SspiEncodeStringsAsAuthIdentity(
            [In] string userName,
            [In] string domainName,
            [In] string password,
            [Out] out SafeSspiAuthDataHandle authData);
    }
}

#endif
