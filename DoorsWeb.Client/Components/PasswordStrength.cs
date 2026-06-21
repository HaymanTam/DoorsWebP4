using DoorsWeb.Shared.Auth;

namespace DoorsWeb.Client.Components;

/// <summary>How strong a password is, ordered weakest → strongest.</summary>
public enum PasswordStrengthLevel
{
    None = 0,
    Weak = 1,
    Fair = 2,
    Good = 3,
    Strong = 4,
}

/// <summary>
/// Lightweight, client-side password feedback used by <see cref="PasswordStrengthMeter"/> and the
/// Users &amp; Passwords save-gate. Following NIST SP 800-63B guidance it scores on <em>length only</em>
/// — no character-class complexity rules, which backfire by pushing users toward predictable patterns.
/// It is a usability aid: the API is the real boundary and additionally screens against a list of
/// known-breached/common passwords, surfacing any rejection on save.
/// </summary>
public static class PasswordStrength
{
    /// <summary>Hard minimum length, shared with the API via <see cref="PasswordPolicy"/>.</summary>
    public const int MinLength = PasswordPolicy.MinLength;

    /// <summary>Minimum level a password must reach before it can be saved (i.e. meets the length floor).</summary>
    public const PasswordStrengthLevel MinimumToSave = PasswordStrengthLevel.Fair;

    /// <summary>Scores a password on length, from <see cref="PasswordStrengthLevel.None"/> to Strong.</summary>
    public static PasswordStrengthLevel Evaluate(string? password)
    {
        if (string.IsNullOrEmpty(password)) return PasswordStrengthLevel.None;

        int length = password.Length;
        if (length < MinLength) return PasswordStrengthLevel.Weak;       // below the hard floor
        if (length < MinLength + 4) return PasswordStrengthLevel.Fair;   // 12–15
        if (length < MinLength + 8) return PasswordStrengthLevel.Good;   // 16–19
        return PasswordStrengthLevel.Strong;                             // 20+
    }

    /// <summary>True when the password meets the length floor and is allowed to be saved.</summary>
    public static bool MeetsMinimum(string? password) =>
        !string.IsNullOrEmpty(password) && password.Length >= MinLength;

    /// <summary>Short human label for a level.</summary>
    public static string Label(PasswordStrengthLevel level) => level switch
    {
        PasswordStrengthLevel.Weak => "Too short",
        PasswordStrengthLevel.Fair => "Fair",
        PasswordStrengthLevel.Good => "Good",
        PasswordStrengthLevel.Strong => "Strong",
        _ => "",
    };

    /// <summary>Bootstrap/Tabler contextual colour suffix (used as bg-/text-).</summary>
    public static string ColorClass(PasswordStrengthLevel level) => level switch
    {
        PasswordStrengthLevel.Weak => "danger",
        PasswordStrengthLevel.Fair => "warning",
        PasswordStrengthLevel.Good => "info",
        PasswordStrengthLevel.Strong => "success",
        _ => "secondary",
    };

    /// <summary>Fill width of the meter bar, 0–100.</summary>
    public static int Percent(PasswordStrengthLevel level) => (int)level * 25;
}
