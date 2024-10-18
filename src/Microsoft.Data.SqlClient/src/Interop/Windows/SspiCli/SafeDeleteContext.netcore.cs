// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

using System.Runtime.InteropServices;
using Interop_TEMP.Windows;
using Interop_TEMP.Windows.SspiCli;

namespace System.Net.Security
{
    //
    // Implementation of handles that are dependent on DeleteSecurityContext
    //
#if DEBUG
    internal abstract partial class SafeDeleteContext : DebugSafeHandle
    {
#else
    internal abstract partial class SafeDeleteContext : SafeHandle
    {
#endif
        //
        // ATN: _handle is internal since it is used on PInvokes by other wrapper methods.
        //      However all such wrappers MUST manually and reliably adjust refCounter of SafeDeleteContext handle.
        //
        internal CredHandle _handle;

        protected SafeDeleteContext() : base(IntPtr.Zero, true)
        {
            _handle = new CredHandle();
        }

        public override bool IsInvalid
        {
            get
            {
                return IsClosed || _handle.IsZero;
            }
        }

        public override string ToString()
        {
            return _handle.ToString();
        }

#if DEBUG
        //This method should never be called for this type
        public new IntPtr DangerousGetHandle()
        {
            throw new InvalidOperationException();
        }
#endif
    }
}

#endif
