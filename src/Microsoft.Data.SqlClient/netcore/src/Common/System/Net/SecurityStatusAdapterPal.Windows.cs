// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET8_0_OR_GREATER

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Interop_TEMP.Windows.SChannel;

namespace System.Net
{
    internal static class SecurityStatusAdapterPal
    {
        private const int StatusDictionarySize = 41;

#if DEBUG
        static SecurityStatusAdapterPal()
        {
            Debug.Assert(s_statusDictionary.Count == StatusDictionarySize, $"Expected size {StatusDictionarySize}, got size {s_statusDictionary.Count}");
        }
#endif

        private static readonly BidirectionalDictionary<SecurityStatus, SecurityStatusPalErrorCode> s_statusDictionary = 
            new BidirectionalDictionary<SecurityStatus, SecurityStatusPalErrorCode>(StatusDictionarySize)
        {
            { SecurityStatus.AlgorithmMismatch, SecurityStatusPalErrorCode.AlgorithmMismatch },
            { SecurityStatus.ApplicationProtocolMismatch, SecurityStatusPalErrorCode.ApplicationProtocolMismatch },
            { SecurityStatus.BadBinding, SecurityStatusPalErrorCode.BadBinding },
            { SecurityStatus.BufferNotEnough, SecurityStatusPalErrorCode.BufferNotEnough },
            { SecurityStatus.CannotInstall, SecurityStatusPalErrorCode.CannotInstall },
            { SecurityStatus.CannotPack, SecurityStatusPalErrorCode.CannotPack },
            { SecurityStatus.CertExpired, SecurityStatusPalErrorCode.CertExpired },
            { SecurityStatus.CertUnknown, SecurityStatusPalErrorCode.CertUnknown },
            { SecurityStatus.CompAndContinue, SecurityStatusPalErrorCode.CompAndContinue },
            { SecurityStatus.CompleteNeeded, SecurityStatusPalErrorCode.CompleteNeeded },
            { SecurityStatus.ContextExpired, SecurityStatusPalErrorCode.ContextExpired },
            { SecurityStatus.ContinueNeeded, SecurityStatusPalErrorCode.ContinueNeeded },
            { SecurityStatus.CredentialsNeeded, SecurityStatusPalErrorCode.CredentialsNeeded },
            { SecurityStatus.DowngradeDetected, SecurityStatusPalErrorCode.DowngradeDetected },
            { SecurityStatus.IllegalMessage, SecurityStatusPalErrorCode.IllegalMessage },
            { SecurityStatus.IncompleteCredentials, SecurityStatusPalErrorCode.IncompleteCredentials },
            { SecurityStatus.IncompleteMessage, SecurityStatusPalErrorCode.IncompleteMessage },
            { SecurityStatus.InternalError, SecurityStatusPalErrorCode.InternalError },
            { SecurityStatus.InvalidHandle, SecurityStatusPalErrorCode.InvalidHandle },
            { SecurityStatus.InvalidToken, SecurityStatusPalErrorCode.InvalidToken },
            { SecurityStatus.LogonDenied, SecurityStatusPalErrorCode.LogonDenied },
            { SecurityStatus.MessageAltered, SecurityStatusPalErrorCode.MessageAltered },
            { SecurityStatus.NoAuthenticatingAuthority, SecurityStatusPalErrorCode.NoAuthenticatingAuthority },
            { SecurityStatus.NoImpersonation, SecurityStatusPalErrorCode.NoImpersonation },
            { SecurityStatus.NoCredentials, SecurityStatusPalErrorCode.NoCredentials },
            { SecurityStatus.NotOwner, SecurityStatusPalErrorCode.NotOwner },
            { SecurityStatus.OK, SecurityStatusPalErrorCode.OK },
            { SecurityStatus.OutOfMemory, SecurityStatusPalErrorCode.OutOfMemory },
            { SecurityStatus.OutOfSequence, SecurityStatusPalErrorCode.OutOfSequence },
            { SecurityStatus.PackageNotFound, SecurityStatusPalErrorCode.PackageNotFound },
            { SecurityStatus.QopNotSupported, SecurityStatusPalErrorCode.QopNotSupported },
            { SecurityStatus.Renegotiate, SecurityStatusPalErrorCode.Renegotiate },
            { SecurityStatus.SecurityQosFailed, SecurityStatusPalErrorCode.SecurityQosFailed },
            { SecurityStatus.SmartcardLogonRequired, SecurityStatusPalErrorCode.SmartcardLogonRequired },
            { SecurityStatus.TargetUnknown, SecurityStatusPalErrorCode.TargetUnknown },
            { SecurityStatus.TimeSkew, SecurityStatusPalErrorCode.TimeSkew },
            { SecurityStatus.UnknownCredentials, SecurityStatusPalErrorCode.UnknownCredentials },
            { SecurityStatus.UnsupportedPreauth, SecurityStatusPalErrorCode.UnsupportedPreauth },
            { SecurityStatus.Unsupported, SecurityStatusPalErrorCode.Unsupported },
            { SecurityStatus.UntrustedRoot, SecurityStatusPalErrorCode.UntrustedRoot },
            { SecurityStatus.WrongPrincipal, SecurityStatusPalErrorCode.WrongPrincipal }
        };

        internal static SecurityStatusPal GetSecurityStatusPalFromNativeInt(int win32SecurityStatus)
        {
            return GetSecurityStatusPalFromInterop((SecurityStatus)win32SecurityStatus);
        }

        internal static SecurityStatusPal GetSecurityStatusPalFromInterop(SecurityStatus win32SecurityStatus, bool attachException = false)
        {
            SecurityStatusPalErrorCode statusCode;

            if (!s_statusDictionary.TryGetForward(win32SecurityStatus, out statusCode))
            {
                Debug.Fail("Unknown Interop.SecurityStatus value: " + win32SecurityStatus);
                throw new InternalException();
            }

            if (attachException)
            {
                return new SecurityStatusPal(statusCode, new Win32Exception((int)win32SecurityStatus));
            }
            else
            {
                return new SecurityStatusPal(statusCode);
            }
        }

        internal static SecurityStatus GetInteropFromSecurityStatusPal(SecurityStatusPal status)
        {
            SecurityStatus interopStatus;
            if (!s_statusDictionary.TryGetBackward(status.ErrorCode, out interopStatus))
            {
                Debug.Fail("Unknown SecurityStatus value: " + status);
                throw new InternalException();
            }
            return interopStatus;
        }
    }
}

#endif
