namespace Microsoft.Data.SqlClientX
{
    public enum TdsParserState
    {
        Idle = 0,

        Executing,
        ExecutingRequestSent,
        ExecutingRedingMetadata,

        Broken,

        Closed
    }
}
