using System.Security.Cryptography;
using DoorsWeb.API.Licensing;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Licensing;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Validates the signed license key (stored in system settings) against the public key from
    /// configuration (<c>Licensing:PublicKey</c>) and exposes the resulting limits. Singleton: the
    /// public key is loaded once; the parsed state is cached and only recomputed when the stored key
    /// string changes, so verification doesn't run on every request.
    /// <para>
    /// With no key (or an invalid/unverifiable one) the app runs in an "unlicensed" default tier with
    /// small door/card limits but full read/write. A valid key grants its limits; once it expires the
    /// app goes read-only (enforced by <see cref="Middleware.LicenseReadOnlyMiddleware"/>).
    /// </para>
    /// </summary>
    public sealed class LicenseService : ILicenseService
    {
        private readonly ISystemSettingsService _settings;
        private readonly ILogger<LicenseService> _logger;
        private readonly ECDsa? _publicKey;
        private readonly int _unlicensedMaxDoors;
        private readonly int _unlicensedMaxCards;

        private readonly object _gate = new();
        private string? _cachedKey;
        private LicenseState? _cachedState;

        public LicenseService(ISystemSettingsService settings, IConfiguration config, ILogger<LicenseService> logger)
        {
            _settings = settings;
            _logger = logger;

            _unlicensedMaxDoors = config.GetValue<int?>("Licensing:UnlicensedMaxDoors") ?? 5;
            _unlicensedMaxCards = config.GetValue<int?>("Licensing:UnlicensedMaxCards") ?? 25;

            var publicKey = config["Licensing:PublicKey"];
            if (!string.IsNullOrWhiteSpace(publicKey))
            {
                try { _publicKey = LicenseKeys.LoadPublicKey(publicKey); }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Licensing:PublicKey is set but could not be parsed; license keys cannot be verified.");
                }
            }
            else
            {
                _logger.LogWarning("Licensing:PublicKey is not configured; running in unlicensed mode and any license key will be rejected.");
            }
        }

        public bool IsReadOnly => GetState().IsExpired;

        public LicenseState GetState()
        {
            var key = _settings.Get().License?.Key;
            lock (_gate)
            {
                if (_cachedState is not null && string.Equals(_cachedKey, key, StringComparison.Ordinal))
                    return _cachedState;

                var state = Compute(key);
                _cachedKey = key;
                _cachedState = state;
                return state;
            }
        }

        private LicenseState Compute(string? key)
        {
            // Unlicensed: no key entered.
            if (string.IsNullOrWhiteSpace(key))
                return Unlicensed(hasKey: false, "No license key — running in unlicensed mode.");

            // A key is present but we have no public key to check it with.
            if (_publicKey is null)
                return Unlicensed(hasKey: true, "No verification key configured on the server (Licensing:PublicKey).");

            if (!LicenseToken.TryVerify(key, _publicKey, out var payload, out var error) || payload is null)
                return Unlicensed(hasKey: true, error ?? "License key is invalid.");

            var expired = DateTime.UtcNow > payload.ExpiryUtc;
            return new LicenseState
            {
                HasKey = true,
                IsValid = true,
                IsLicensed = true,
                IsExpired = expired,
                IsActive = !expired,
                Message = expired ? $"License expired on {payload.ExpiryUtc:yyyy-MM-dd} — the system is read-only." : null,
                Customer = payload.Customer,
                LicenseId = payload.LicenseId,
                MaxDoors = payload.MaxDoors,
                MaxCards = payload.MaxCards,
                ExpiryUtc = payload.ExpiryUtc,
                IssuedUtc = payload.IssuedUtc
            };
        }

        private LicenseState Unlicensed(bool hasKey, string message) => new()
        {
            HasKey = hasKey,
            IsValid = false,
            IsLicensed = false,
            IsExpired = false,
            IsActive = false,
            Message = message,
            MaxDoors = _unlicensedMaxDoors,
            MaxCards = _unlicensedMaxCards
        };

        public void EnforceDoorLimit(int currentDoorCount)
        {
            var s = GetState();
            if (currentDoorCount >= s.MaxDoors)
                throw new LicenseLimitException(
                    $"Door limit reached: your {(s.IsLicensed ? "license" : "unlicensed installation")} allows " +
                    $"{s.MaxDoors} door(s). Remove a door or upgrade your license to add more.");
        }

        public void EnforceCardLimit(int currentCardCount)
        {
            var s = GetState();
            if (currentCardCount >= s.MaxCards)
                throw new LicenseLimitException(
                    $"Card limit reached: your {(s.IsLicensed ? "license" : "unlicensed installation")} allows " +
                    $"{s.MaxCards} card(s). Remove a card or upgrade your license to add more.");
        }
    }
}
