namespace DoorsWeb.Licensing
{
    /// <summary>
    /// The data a license grants. This is exactly what gets signed: changing any field invalidates
    /// the signature, so the limits and expiry cannot be tampered with after issue. Keep it small —
    /// the whole thing is serialized into the license key string.
    /// </summary>
    public sealed class LicensePayload
    {
        /// <summary>Opaque order / license identifier (e.g. a GUID or the payment order id).</summary>
        public string LicenseId { get; set; } = string.Empty;

        /// <summary>Who the license was issued to (shown in the app's License panel).</summary>
        public string Customer { get; set; } = string.Empty;

        /// <summary>Maximum number of doors the installation may have.</summary>
        public int MaxDoors { get; set; }

        /// <summary>Maximum number of cardholders (access cards) the installation may have.</summary>
        public int MaxCards { get; set; }

        /// <summary>When the license expires (UTC). After this the app drops to read-only.</summary>
        public DateTime ExpiryUtc { get; set; }

        /// <summary>When the license was issued (UTC).</summary>
        public DateTime IssuedUtc { get; set; }
    }
}
