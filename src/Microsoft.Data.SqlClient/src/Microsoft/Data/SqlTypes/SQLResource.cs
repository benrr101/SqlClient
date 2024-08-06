// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Data.SqlTypes
{

    using System;
    using Microsoft.Data;

    internal sealed class SQLResource
    {

        private SQLResource() { /* prevent utility class from being instantiated*/ }

        internal static readonly string NullString = StringsHelper.GetString(Strings.SqlMisc_NullString);

        internal static readonly string MessageString = StringsHelper.GetString(Strings.SqlMisc_MessageString);

        internal static readonly string ArithOverflowMessage = StringsHelper.GetString(Strings.SqlMisc_ArithOverflowMessage);

        internal static readonly string DivideByZeroMessage = StringsHelper.GetString(Strings.SqlMisc_DivideByZeroMessage);

        internal static readonly string NullValueMessage = StringsHelper.GetString(Strings.SqlMisc_NullValueMessage);

        internal static readonly string TruncationMessage = StringsHelper.GetString(Strings.SqlMisc_TruncationMessage);

        internal static readonly string DateTimeOverflowMessage = StringsHelper.GetString(Strings.SqlMisc_DateTimeOverflowMessage);

        internal static readonly string ConcatDiffCollationMessage = StringsHelper.GetString(Strings.SqlMisc_ConcatDiffCollationMessage);

        internal static readonly string CompareDiffCollationMessage = StringsHelper.GetString(Strings.SqlMisc_CompareDiffCollationMessage);

        internal static readonly string InvalidFlagMessage = StringsHelper.GetString(Strings.SqlMisc_InvalidFlagMessage);

        internal static readonly string NumeToDecOverflowMessage = StringsHelper.GetString(Strings.SqlMisc_NumeToDecOverflowMessage);

        internal static readonly string ConversionOverflowMessage = StringsHelper.GetString(Strings.SqlMisc_ConversionOverflowMessage);

        internal static readonly string InvalidDateTimeMessage = StringsHelper.GetString(Strings.SqlMisc_InvalidDateTimeMessage);

        internal static readonly string TimeZoneSpecifiedMessage = StringsHelper.GetString(Strings.SqlMisc_TimeZoneSpecifiedMessage);

        internal static readonly string InvalidArraySizeMessage = StringsHelper.GetString(Strings.SqlMisc_InvalidArraySizeMessage);

        internal static readonly string InvalidPrecScaleMessage = StringsHelper.GetString(Strings.SqlMisc_InvalidPrecScaleMessage);

        internal static readonly string FormatMessage = StringsHelper.GetString(Strings.SqlMisc_FormatMessage);

        internal static readonly string NotFilledMessage = StringsHelper.GetString(Strings.SqlMisc_NotFilledMessage);

        internal static readonly string AlreadyFilledMessage = StringsHelper.GetString(Strings.SqlMisc_AlreadyFilledMessage);

        internal static readonly string ClosedXmlReaderMessage = StringsHelper.GetString(Strings.SqlMisc_ClosedXmlReaderMessage);

        internal static string InvalidOpStreamClosed(string method)
        {
            return StringsHelper.GetString(Strings.SqlMisc_InvalidOpStreamClosed, method);
        }

        internal static string InvalidOpStreamNonWritable(string method)
        {
            return StringsHelper.GetString(Strings.SqlMisc_InvalidOpStreamNonWritable, method);
        }

        internal static string InvalidOpStreamNonReadable(string method)
        {
            return StringsHelper.GetString(Strings.SqlMisc_InvalidOpStreamNonReadable, method);
        }

        internal static string InvalidOpStreamNonSeekable(string method)
        {
            return StringsHelper.GetString(Strings.SqlMisc_InvalidOpStreamNonSeekable, method);
        }
    } // SqlResource

} // namespace System
