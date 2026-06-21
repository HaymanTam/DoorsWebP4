namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Thrown when a password fails the policy (too short, or on the breached/common list).
    /// Controllers translate it into a 400 Bad Request carrying the user-facing message.
    /// </summary>
    public class PasswordPolicyException : Exception
    {
        public PasswordPolicyException(string message) : base(message) { }
    }
}
