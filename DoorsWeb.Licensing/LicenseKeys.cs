using System.Security.Cryptography;

namespace DoorsWeb.Licensing
{
    /// <summary>
    /// Helpers for the ECDSA P-256 key pair behind the license signing.
    /// <list type="bullet">
    /// <item>The <b>private</b> key lives only with the issuer (the keygen app / payment website)
    /// and signs license keys. Treat it like a password — never ship it in the application.</item>
    /// <item>The <b>public</b> key is embedded in the application (config <c>Licensing:PublicKey</c>)
    /// and only verifies. It is safe to distribute.</item>
    /// </list>
    /// </summary>
    public static class LicenseKeys
    {
        private static ECDsa NewCurve() => ECDsa.Create(ECCurve.NamedCurves.nistP256);

        /// <summary>Creates a fresh key pair, returned as PEM strings (private = PKCS#8, public = SPKI).</summary>
        public static (string PrivatePem, string PublicPem) CreateKeyPair()
        {
            using var ec = NewCurve();
            return (ec.ExportPkcs8PrivateKeyPem(), ec.ExportSubjectPublicKeyInfoPem());
        }

        /// <summary>
        /// The public key as a single-line Base64 of its SubjectPublicKeyInfo, convenient for
        /// pasting into a JSON config value (no PEM line breaks).
        /// </summary>
        public static string ExportPublicKeyBase64(ECDsa key)
        {
            ArgumentNullException.ThrowIfNull(key);
            return Convert.ToBase64String(key.ExportSubjectPublicKeyInfo());
        }

        /// <summary>Loads a private key from a PKCS#8 PEM string (for signing).</summary>
        public static ECDsa LoadPrivateKeyPem(string pem)
        {
            var ec = NewCurve();
            ec.ImportFromPem(pem);
            return ec;
        }

        /// <summary>
        /// Loads a public key (for verifying) from either a PEM block (contains "BEGIN") or a
        /// single-line Base64 SubjectPublicKeyInfo. Throws if the value can't be parsed.
        /// </summary>
        public static ECDsa LoadPublicKey(string pemOrBase64)
        {
            if (string.IsNullOrWhiteSpace(pemOrBase64))
                throw new ArgumentException("Public key is empty.", nameof(pemOrBase64));

            var ec = NewCurve();
            if (pemOrBase64.Contains("BEGIN", StringComparison.Ordinal))
            {
                ec.ImportFromPem(pemOrBase64);
            }
            else
            {
                var der = Convert.FromBase64String(pemOrBase64.Trim());
                ec.ImportSubjectPublicKeyInfo(der, out _);
            }
            return ec;
        }
    }
}
