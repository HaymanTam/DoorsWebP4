namespace DoorsWeb.API.Licensing
{
    /// <summary>
    /// Thrown by a create operation when adding the row would exceed the licensed door or card
    /// count. Controllers translate it into a 409 Conflict ProblemDetails.
    /// </summary>
    public sealed class LicenseLimitException : Exception
    {
        public LicenseLimitException(string message) : base(message) { }
    }
}
