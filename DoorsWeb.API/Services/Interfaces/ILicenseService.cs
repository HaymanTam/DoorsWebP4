namespace DoorsWeb.API.Services.Interfaces
{
    /// <summary>
    /// The current, validated licensing state. Effective limits always have a value: when there is
    /// no valid license they fall back to the unlicensed default tier.
    /// </summary>
    public sealed class LicenseState
    {
        public bool HasKey { get; init; }
        public bool IsValid { get; init; }
        public bool IsExpired { get; init; }
        public bool IsActive { get; init; }
        public bool IsLicensed { get; init; }
        public string? Message { get; init; }

        public string? Customer { get; init; }
        public string? LicenseId { get; init; }
        public int MaxDoors { get; init; }
        public int MaxCards { get; init; }
        public DateTime? ExpiryUtc { get; init; }
        public DateTime? IssuedUtc { get; init; }
    }

    /// <summary>
    /// Validates the installation's license key (signed, verified with the embedded public key) and
    /// enforces its limits. A valid-but-expired license puts the app into read-only mode.
    /// </summary>
    public interface ILicenseService
    {
        /// <summary>Current state, recomputed when the stored key changes (cheap, cached by key string).</summary>
        LicenseState GetState();

        /// <summary>True when a valid license has passed its expiry date — writes must be blocked.</summary>
        bool IsReadOnly { get; }

        /// <summary>Throws <see cref="Licensing.LicenseLimitException"/> if a door can't be added at the current count.</summary>
        void EnforceDoorLimit(int currentDoorCount);

        /// <summary>Throws <see cref="Licensing.LicenseLimitException"/> if a card can't be added at the current count.</summary>
        void EnforceCardLimit(int currentCardCount);
    }
}
