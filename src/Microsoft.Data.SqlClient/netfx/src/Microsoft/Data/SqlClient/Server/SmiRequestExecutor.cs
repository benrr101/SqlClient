// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Data.SqlTypes;
using System.Transactions;

namespace Microsoft.Data.SqlClient.Server
{

    internal abstract class SmiRequestExecutor : SmiTypedGetterSetter, ITypedSettersV3, ITypedSetters, ITypedGetters, IDisposable
    {

        #region SMI active methods as of V210

        #region Overall control methods
        public virtual void Close(
            SmiEventSink eventSink
        )
        {
            // Adding as of V3

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V2- and hasn't implemented V3 yet.
            //  2) Server didn't implement V3 on some interface, but negotiated V3+.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        internal virtual SmiEventStream Execute(
            SmiConnection connection,                     // Assigned connection
            long transactionId,                  // Assigned transaction
            Transaction associatedTransaction,          // SysTx transaction associated with request, if any.
            CommandBehavior behavior,                       // CommandBehavior,   
            SmiExecuteType executeType                     // Type of execute called (NonQuery/Pipe/Reader/Row, etc)
        )
        {
            // Adding as of V210

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V200- and hasn't implemented V210 yet.
            //  2) Server didn't implement V210 on some interface, but negotiated V210+.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        #endregion

        #region Supported access method types (Get] vs. Set)

        // RequestExecutor only supports setting parameter values, not getting
        internal override bool CanGet
        {
            get
            {
                return false;
            }
        }

        internal override bool CanSet
        {
            get
            {
                return true;
            }
        }

        #endregion

        // SmiRequestExecutor and it's subclasses should NOT override Getters from SmiTypedGetterSetter
        //  Calls against those methods on a Request Executor are not allowed.

        #region Value setters

        // Set DEFAULT bit for parameter
        internal abstract void SetDefault(int ordinal);

        // SmiRequestExecutor subclasses must implement all Setters from SmiTypedGetterSetter
        //  SmiRequestExecutor itself does not need to implement these, since it inherits the default implementation from 
        //      SmiTypedGetterSetter

        #endregion
        #endregion

        #region Obsolete as of V210
        internal virtual SmiEventStream Execute(
            SmiConnection connection,                     // Assigned connection
            long transactionId,                  // Assigned transaction
            CommandBehavior behavior,                       // CommandBehavior,   
            SmiExecuteType executeType                     // Type of execute called (NonQuery/Pipe/Reader/Row, etc)
        )
        {
            // Obsoleting as of V210

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V210+ (and doesn't implement it).
            //  2) Server doesn't implement this method, but negotiated V200-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }
        #endregion

        #region OBSOLETE STUFF that never shipped without obsolete attribute

        //
        //  IDisposable
        //
        public virtual void Dispose()
        {
            // ******** OBSOLETING from SMI -- use close instead.
            //  Intended to be removed (along with removing inheriting IDisposable) prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        // Check to see if parameter's DEFAULT bit is set
        internal virtual bool IsSetAsDefault(int ordinal)
        {
            // ******** OBSOLETING from SMI -- Not needed.
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        // Get the count of parameters
        public virtual int Count
        {
            get
            {
                // ******** OBSOLETING from SMI -- front end needs to keep track of input param metadata itself.  Outparam metadata comes with ParametersAvailable event.
                //  Intended to be removed prior to RTM.

                // Implement body with throw because there are only a couple of ways to get to this code:
                //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
                //  2) Server didn't implement V2- on some interface and negotiated V2-.
                throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
            }
        }

        // Get the meta data associated with the parameter.
        public virtual SmiParameterMetaData GetMetaData(int ordinal)
        {
            // ******** OBSOLETING from SMI -- front end needs to keep track of input param metadata itself.  Outparam metadata comes with ParametersAvailable event.
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        //
        //  ITypedGetters methods (for output parameters)  (OBSOLETE)
        //
        public virtual bool IsDBNull(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlDbType GetVariantType(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual bool GetBoolean(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual byte GetByte(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual long GetBytes(int ordinal, long fieldOffset, byte[] buffer, int bufferOffset, int length)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual char GetChar(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual long GetChars(int ordinal, long fieldOffset, char[] buffer, int bufferOffset, int length)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual short GetInt16(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual int GetInt32(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual long GetInt64(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual float GetFloat(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual double GetDouble(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual string GetString(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual decimal GetDecimal(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual DateTime GetDateTime(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual Guid GetGuid(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlBoolean GetSqlBoolean(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlByte GetSqlByte(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlInt16 GetSqlInt16(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlInt32 GetSqlInt32(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlInt64 GetSqlInt64(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlSingle GetSqlSingle(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlDouble GetSqlDouble(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlMoney GetSqlMoney(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlDateTime GetSqlDateTime(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlDecimal GetSqlDecimal(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlString GetSqlString(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlBinary GetSqlBinary(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlGuid GetSqlGuid(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlChars GetSqlChars(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlBytes GetSqlBytes(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlXml GetSqlXml(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlXml GetSqlXmlRef(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlBytes GetSqlBytesRef(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual SqlChars GetSqlCharsRef(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use ITypedGettersV3 in ParametersAvailable event instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        //
        //  ITypedSetters methods
        //
        public virtual void SetDBNull(int ordinal)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetBoolean(int ordinal, bool value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetByte(int ordinal, byte value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetBytes(int ordinal, long fieldOffset, byte[] buffer, int bufferOffset, int length)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetChar(int ordinal, char value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetChars(int ordinal, long fieldOffset, char[] buffer, int bufferOffset, int length)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetInt16(int ordinal, short value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetInt32(int ordinal, int value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetInt64(int ordinal, long value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetFloat(int ordinal, float value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetDouble(int ordinal, double value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetString(int ordinal, string value)
        {
            // Implemented as virtual method to allow transport to remove it's implementation

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V2 and dropped support for V1.
            //  2) Server didn't implement V1 on some interface and negotiated V1.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetString(int ordinal, string value, int offset)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetDecimal(int ordinal, decimal value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetDateTime(int ordinal, DateTime value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetGuid(int ordinal, Guid value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlBoolean(int ordinal, SqlBoolean value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlByte(int ordinal, SqlByte value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlInt16(int ordinal, SqlInt16 value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlInt32(int ordinal, SqlInt32 value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlInt64(int ordinal, SqlInt64 value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlSingle(int ordinal, SqlSingle value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlDouble(int ordinal, SqlDouble value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlMoney(int ordinal, SqlMoney value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlDateTime(int ordinal, SqlDateTime value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlDecimal(int ordinal, SqlDecimal value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlString(int ordinal, SqlString value)
        {
            // Implemented as empty virtual method to allow transport to remove it's implementation

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V2 and dropped support for V1.
            //  2) Server didn't implement V1 on some interface and negotiated V1.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlString(int ordinal, SqlString value, int offset)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlBinary(int ordinal, SqlBinary value)
        {
            // Implemented as empty virtual method to allow transport to remove it's implementation

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V2 and dropped support for V1.
            //  2) Server didn't implement V1 on some interface and negotiated V1.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlBinary(int ordinal, SqlBinary value, int offset)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlGuid(int ordinal, SqlGuid value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlChars(int ordinal, SqlChars value)
        {
            // Implemented as empty virtual method to allow transport to remove it's implementation

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V2 and dropped support for V1.
            //  2) Server didn't implement V1 on some interface and negotiated V1.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlChars(int ordinal, SqlChars value, int offset)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlBytes(int ordinal, SqlBytes value)
        {
            // Implemented as empty virtual method to allow transport to remove it's implementation

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V2 and dropped support for V1.
            //  2) Server didn't implement V1 on some interface and negotiated V1.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlBytes(int ordinal, SqlBytes value, int offset)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }

        public virtual void SetSqlXml(int ordinal, SqlXml value)
        {
            // ******** OBSOLETING from SMI -- use related ITypedSettersV3 method instead
            //  Intended to be removed prior to RTM.

            // Implement body with throw because there are only a couple of ways to get to this code:
            //  1) Client is calling this method even though the server negotiated for V3+ and dropped support for V2-.
            //  2) Server didn't implement V2- on some interface and negotiated V2-.
            throw Microsoft.Data.Common.ADP.InternalError(Microsoft.Data.Common.ADP.InternalErrorCode.UnimplementedSMIMethod);
        }
        #endregion
    }
}
