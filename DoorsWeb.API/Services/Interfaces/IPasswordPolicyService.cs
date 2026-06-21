using DoorsWeb.Shared.Auth;

namespace DoorsWeb.API.Services.Interfaces
{
    /// <summary>
    /// Server-side password-policy enforcement. Combines the shared <see cref="PasswordPolicy"/>
    /// length rule with an offline screen against a bundled list of known-breached/common passwords,
    /// so the actual stupid passwords are rejected without any outbound network call.
    /// </summary>
    public interface IPasswordPolicyService
    {
        /// <summary>True when the password appears on the bundled breached/common-password list.</summary>
        bool IsBreached(string password);

        /// <summary>
        /// Validates a candidate password against the full policy (length, then breach list) and
        /// returns the first failure, or success.
        /// </summary>
        PasswordValidationResult Validate(string? password);
    }
}
