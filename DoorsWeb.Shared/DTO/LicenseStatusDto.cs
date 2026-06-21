namespace DoorsWeb.Shared.DTO
{
    /// <summary>
    /// The current licensing state, as shown in the System Settings → License panel. Decoded and
    /// validated server-side from the signed license key; the client only displays it.
    /// </summary>
    public class LicenseStatusDto
    {
        /// <summary>A license key string has been entered (regardless of whether it's valid).</summary>
        public bool HasKey { get; set; }

        /// <summary>The key's signature verified and its contents parsed.</summary>
        public bool IsValid { get; set; }

        /// <summary>The license is valid but past its expiry date (app is read-only).</summary>
        public bool IsExpired { get; set; }

        /// <summary>Valid and not expired — full functionality, subject to the door/card limits.</summary>
        public bool IsActive { get; set; }

        /// <summary>Why the key is not valid / not active (null when active).</summary>
        public string? Message { get; set; }

        /// <summary>Whether these limits come from a real license (false = unlicensed default tier).</summary>
        public bool IsLicensed { get; set; }

        public string? Customer { get; set; }
        public string? LicenseId { get; set; }

        /// <summary>Effective limits currently in force.</summary>
        public int MaxDoors { get; set; }
        public int MaxCards { get; set; }

        public DateTime? ExpiryUtc { get; set; }
        public DateTime? IssuedUtc { get; set; }

        /// <summary>Current usage, so the panel can show "12 / 50 doors".</summary>
        public int DoorCount { get; set; }
        public int CardCount { get; set; }
    }
}
