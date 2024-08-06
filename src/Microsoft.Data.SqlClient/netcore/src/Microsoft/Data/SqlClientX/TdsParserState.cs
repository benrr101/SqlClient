namespace Microsoft.Data.SqlClientX
{
    public enum TdsParserState
    {
        Idle = 0,
        Executing,
        Broken,
        Closed
    }
}
