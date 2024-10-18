// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET8_0_OR_GREATER

using Interop_TEMP.Windows.SspiCli;

namespace System.Net
{
    internal static class ContextFlagsAdapterPal
    {
        private readonly struct ContextFlagMapping
        {
            public readonly ContextFlags Win32Flag;
            public readonly ContextFlagsPal ContextFlag;

            public ContextFlagMapping(ContextFlags win32Flag, ContextFlagsPal contextFlag)
            {
                Win32Flag = win32Flag;
                ContextFlag = contextFlag;
            }
        }

        private static readonly ContextFlagMapping[] s_contextFlagMapping = new[]
        {
            new ContextFlagMapping(ContextFlags.AcceptExtendedError, ContextFlagsPal.AcceptExtendedError),
            new ContextFlagMapping(ContextFlags.AcceptIdentify, ContextFlagsPal.AcceptIdentify),
            new ContextFlagMapping(ContextFlags.AcceptIntegrity, ContextFlagsPal.AcceptIntegrity),
            new ContextFlagMapping(ContextFlags.AcceptStream, ContextFlagsPal.AcceptStream),
            new ContextFlagMapping(ContextFlags.AllocateMemory, ContextFlagsPal.AllocateMemory),
            new ContextFlagMapping(ContextFlags.AllowMissingBindings, ContextFlagsPal.AllowMissingBindings),
            new ContextFlagMapping(ContextFlags.Confidentiality, ContextFlagsPal.Confidentiality),
            new ContextFlagMapping(ContextFlags.Connection, ContextFlagsPal.Connection),
            new ContextFlagMapping(ContextFlags.Delegate, ContextFlagsPal.Delegate),
            new ContextFlagMapping(ContextFlags.InitExtendedError, ContextFlagsPal.InitExtendedError),
            new ContextFlagMapping(ContextFlags.InitIdentify, ContextFlagsPal.InitIdentify),
            new ContextFlagMapping(ContextFlags.InitManualCredValidation, ContextFlagsPal.InitManualCredValidation),
            new ContextFlagMapping(ContextFlags.InitIntegrity, ContextFlagsPal.InitIntegrity),
            new ContextFlagMapping(ContextFlags.InitStream, ContextFlagsPal.InitStream),
            new ContextFlagMapping(ContextFlags.InitUseSuppliedCreds, ContextFlagsPal.InitUseSuppliedCreds),
            new ContextFlagMapping(ContextFlags.MutualAuth, ContextFlagsPal.MutualAuth),
            new ContextFlagMapping(ContextFlags.ProxyBindings, ContextFlagsPal.ProxyBindings),
            new ContextFlagMapping(ContextFlags.ReplayDetect, ContextFlagsPal.ReplayDetect),
            new ContextFlagMapping(ContextFlags.SequenceDetect, ContextFlagsPal.SequenceDetect),
            new ContextFlagMapping(ContextFlags.UnverifiedTargetName, ContextFlagsPal.UnverifiedTargetName),
            new ContextFlagMapping(ContextFlags.UseSessionKey, ContextFlagsPal.UseSessionKey),
            new ContextFlagMapping(ContextFlags.Zero, ContextFlagsPal.None),
        };

        internal static ContextFlagsPal GetContextFlagsPalFromInterop(ContextFlags win32Flags)
        {
            ContextFlagsPal flags = ContextFlagsPal.None;
            foreach (ContextFlagMapping mapping in s_contextFlagMapping)
            {
                if ((win32Flags & mapping.Win32Flag) == mapping.Win32Flag)
                {
                    flags |= mapping.ContextFlag;
                }
            }

            return flags;
        }

        internal static ContextFlags GetInteropFromContextFlagsPal(ContextFlagsPal flags)
        {
            ContextFlags win32Flags = ContextFlags.Zero;
            foreach (ContextFlagMapping mapping in s_contextFlagMapping)
            {
                if ((flags & mapping.ContextFlag) == mapping.ContextFlag)
                {
                    win32Flags |= mapping.Win32Flag;
                }
            }

            return win32Flags;
        }
    }
}

#endif
