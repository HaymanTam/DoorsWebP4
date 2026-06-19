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
/// Lightweight, client-side password-strength scoring used by <see cref="PasswordStrengthMeter"/>
/// and the Users &amp; Passwords save-gate. Scores on length + character-class variety; it is a
/// usability aid, not a cryptographic guarantee.
/// </summary>
public static class PasswordStrength
{
    /// <summary>Minimum level a password must reach before it can be saved.</summary>
    public const PasswordStrengthLevel MinimumToSave = PasswordStrengthLevel.Fair;

    /// <summary>Scores a password from <see cref="PasswordStrengthLevel.None"/> to Strong.</summary>
    public static PasswordStrengthLevel Evaluate(string? password)
    {
        if (string.IsNullOrEmpty(password)) return PasswordStrengthLevel.None;

        bool lower = false, upper = false, digit = false, symbol = false;
        foreach (var c in password)
        {
            if (char.IsLower(c)) lower = true;
            else if (char.IsUpper(c)) upper = true;
            else if (char.IsDigit(c)) digit = true;
            else symbol = true;
        }
        int classes = (lower ? 1 : 0) + (upper ? 1 : 0) + (digit ? 1 : 0) + (symbol ? 1 : 0);
        int length = password.Length;

        if (length < 8) return PasswordStrengthLevel.Weak;      // too short, regardless of variety
        if (classes <= 1) return PasswordStrengthLevel.Weak;    // e.g. all letters or all digits
        if (classes == 2) return length >= 12 ? PasswordStrengthLevel.Good : PasswordStrengthLevel.Fair;
        // 3+ character classes
        return length >= 12 ? PasswordStrengthLevel.Strong : PasswordStrengthLevel.Good;
    }

    /// <summary>True when the password is strong enough to persist.</summary>
    public static bool MeetsMinimum(string? password) => Evaluate(password) >= MinimumToSave;

    /// <summary>Short human label for a level.</summary>
    public static string Label(PasswordStrengthLevel level) => level switch
    {
        PasswordStrengthLevel.Weak => "Weak",
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
