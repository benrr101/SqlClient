// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NETFRAMEWORK && !NET8_0_OR_GREATER

using System;
using System.Runtime.InteropServices;

namespace Interop_TEMP.Windows.SspiCli
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CredHandle
    {
        private IntPtr _dwLower;
        private IntPtr _dwUpper;

        public bool IsZero
        {
            get { return _dwLower == IntPtr.Zero && _dwUpper == IntPtr.Zero; }
        }

        internal void SetToInvalid()
        {
            _dwLower = IntPtr.Zero;
            _dwUpper = IntPtr.Zero;
        }

        public override string ToString()
        {
            { return _dwLower.ToString("x") + ":" + _dwUpper.ToString("x"); }
        }
    }
}

#endif
