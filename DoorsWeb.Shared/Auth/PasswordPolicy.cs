namespace DoorsWeb.Shared.Auth
{
    /// <summary>
    /// Password rules shared by the client (live feedback) and the API (enforcement). Follows NIST
    /// SP 800-63B guidance: favour length over character-class complexity, screen against a list of
    /// known-breached/common passwords, and never expire passwords on a schedule.
    ///
    /// The <see cref="MinLength"/> length check runs on both client and server. The breached/common
    /// screen is server-only (the ~1M-entry list lives on the API), so the client gates saving on
    /// length and surfaces any breach rejection returned by the API.
    /// </summary>
    public static class PasswordPolicy
    {
        /// <summary>
        /// Minimum password length, applied uniformly to every account (no per-role tiers — a flat
        /// 12 for Supers and ordinary users alike).
        /// </summary>
        public const int MinLength = 12;

        /// <summary>One-line guidance shown beneath password inputs. Encourages passphrases.</summary>
        public const string Guidance =
            "Use at least 12 characters. A short phrase of a few words is easy to remember and hard "
            + "to guess — length matters more than mixing in symbols.";

        /// <summary>Message returned when a password is shorter than <see cref="MinLength"/>.</summary>
        public static string TooShortMessage => $"Password must be at least {MinLength} characters.";

        /// <summary>Message returned when a password matches a known-breached/common password.</summary>
        public const string BreachedMessage =
            "This password is on a list of known-breached or commonly used passwords. "
            + "Choose something less common.";

        /// <summary>
        /// Length-only check — the part both client and server can perform without the offline
        /// breach list.
        /// </summary>
        public static PasswordValidationResult ValidateLength(string? password) =>
            string.IsNullOrEmpty(password) || password.Length < MinLength
                ? PasswordValidationResult.Fail(TooShortMessage)
                : PasswordValidationResult.Ok();
    }

    /// <summary>Outcome of a password-policy check.</summary>
    public sealed class PasswordValidationResult
    {
        public bool IsValid { get; init; }

        /// <summary>User-facing reason the password was rejected; null when <see cref="IsValid"/>.</summary>
        public string? Error { get; init; }

        public static PasswordValidationResult Ok() => new() { IsValid = true };
        public static PasswordValidationResult Fail(string error) => new() { IsValid = false, Error = error };
    }
}
