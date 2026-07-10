namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Thrown by a service when an entity can't be deleted because other records still
    /// reference it (a foreign-key dependency). Controllers translate this into an HTTP 409
    /// (Conflict) carrying <see cref="System.Exception.Message"/> so the UI can show the user
    /// a clear, actionable reason instead of an opaque 500 from the database FK violation.
    /// </summary>
    public class EntityInUseException : Exception
    {
        public EntityInUseException(string message) : base(message) { }
    }
}
