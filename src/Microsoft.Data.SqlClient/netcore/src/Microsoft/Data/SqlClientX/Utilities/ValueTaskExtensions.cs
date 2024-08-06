// // Licensed to the .NET Foundation under one or more agreements.
// // The .NET Foundation licenses this file to you under the MIT license.
// // See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

#nullable enable
#if NET8_0_OR_GREATER

namespace Microsoft.Data.SqlClientX.Utilities
{
    public static class ValueTaskExtensions
    {
        public static T GetSyncResult<T>(this ValueTask<T> valueTask)
        {
            if (!valueTask.IsCompleted)
            {
                throw new InvalidOperationException();
            }

            return valueTask.Result;
        }
    }
}

#endif
