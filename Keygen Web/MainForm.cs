using System.Security.Cryptography;
using DoorsWeb.Licensing;

namespace DoorsWeb.Keygen;

/// <summary>
/// The keygen window. Load (or generate) the ECDSA private signing key, fill in the customer /
/// limits / expiry, and produce a signed license key string. The matching public key is shown so
/// it can be pasted into the app's <c>Licensing:PublicKey</c> setting.
/// <para>
/// This tool is for the license issuer only — anyone with the private key can mint valid keys, so
/// it must never be shipped with the product.
/// </para>
/// </summary>
public partial class MainForm : Form
{
    // The loaded signing key. Null until a key is loaded or generated; signing is disabled meanwhile.
    private ECDsa? _privateKey;

    public MainForm()
    {
        InitializeComponent();

        txtLicenseId.Text = Guid.NewGuid().ToString("N");
        dtExpiry.Value = DateTime.Now.Date.AddYears(1);
    }

    // ---- signing key ------------------------------------------------------------------------

    private void btnLoadKey_Click(object? sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            Title = "Load private signing key (PKCS#8 PEM)",
            Filter = "PEM key files (*.pem)|*.pem|All files (*.*)|*.*"
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            var pem = File.ReadAllText(dlg.FileName);
            var key = LicenseKeys.LoadPrivateKeyPem(pem);
            SetPrivateKey(key, $"Loaded: {Path.GetFileName(dlg.FileName)}");
        }
        catch (Exception ex)
        {
            Error("That file is not a valid private key.\n\n" + ex.Message);
        }
    }

    private void btnNewKey_Click(object? sender, EventArgs e)
    {
        using var dlg = new SaveFileDialog
        {
            Title = "Save NEW private signing key — keep this secret",
            Filter = "PEM key files (*.pem)|*.pem",
            FileName = "doorsweb.private.pem"
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            var (privatePem, publicPem) = LicenseKeys.CreateKeyPair();

            var privatePath = dlg.FileName;
            var publicPath = Path.Combine(
                Path.GetDirectoryName(privatePath) ?? ".",
                Path.GetFileNameWithoutExtension(privatePath) + ".public.pem");

            File.WriteAllText(privatePath, privatePem);
            File.WriteAllText(publicPath, publicPem);

            var key = LicenseKeys.LoadPrivateKeyPem(privatePem);
            SetPrivateKey(key, $"Generated: {Path.GetFileName(privatePath)}");

            MessageBox.Show(this,
                "New key pair written:\n\n" +
                $"Private (KEEP SECRET): {privatePath}\n" +
                $"Public: {publicPath}\n\n" +
                "Paste the public key shown below into the app's Licensing:PublicKey setting. " +
                "Never commit or distribute the private key.",
                "Key pair created", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            Error("Could not generate / save the key pair.\n\n" + ex.Message);
        }
    }

    // Adopts a freshly loaded/generated key: disposes any previous one, shows the matching public
    // key (base64 SPKI for appsettings) and enables signing.
    private void SetPrivateKey(ECDsa key, string status)
    {
        _privateKey?.Dispose();
        _privateKey = key;

        txtPublic.Text = LicenseKeys.ExportPublicKeyBase64(key);
        lblKeyStatus.Text = status;
        lblKeyStatus.ForeColor = Color.ForestGreen;
        btnGenerate.Enabled = true;
    }

    private void btnCopyPublic_Click(object? sender, EventArgs e) => CopyToClipboard(txtPublic.Text);

    // ---- license details --------------------------------------------------------------------

    private void btnNewId_Click(object? sender, EventArgs e) =>
        txtLicenseId.Text = Guid.NewGuid().ToString("N");

    private void btnGenerate_Click(object? sender, EventArgs e)
    {
        if (_privateKey is null)
        {
            Error("Load or generate a signing key first.");
            return;
        }

        var customer = txtCustomer.Text.Trim();
        if (customer.Length == 0)
        {
            Error("Enter a customer name.");
            txtCustomer.Focus();
            return;
        }

        var licenseId = txtLicenseId.Text.Trim();
        if (licenseId.Length == 0)
            licenseId = Guid.NewGuid().ToString("N");

        // Expire at the END of the chosen day, in UTC, so a "2026-12-31" license is valid through
        // that whole day regardless of the issuer's time zone.
        var expiryUtc = DateTime.SpecifyKind(dtExpiry.Value.Date.AddDays(1).AddSeconds(-1), DateTimeKind.Utc);

        var payload = new LicensePayload
        {
            LicenseId = licenseId,
            Customer = customer,
            MaxDoors = (int)numMaxDoors.Value,
            MaxCards = (int)numMaxCards.Value,
            ExpiryUtc = expiryUtc,
            IssuedUtc = DateTime.UtcNow
        };

        try
        {
            txtKey.Text = LicenseToken.Sign(payload, _privateKey);
            btnCopyKey.Enabled = true;
            btnSaveLic.Enabled = true;
        }
        catch (Exception ex)
        {
            Error("Could not sign the license.\n\n" + ex.Message);
        }
    }

    // ---- output -----------------------------------------------------------------------------

    private void btnCopyKey_Click(object? sender, EventArgs e) => CopyToClipboard(txtKey.Text);

    private void btnSaveLic_Click(object? sender, EventArgs e)
    {
        if (txtKey.Text.Length == 0) return;

        var suggested = Sanitize(txtCustomer.Text.Trim());
        using var dlg = new SaveFileDialog
        {
            Title = "Save license key",
            Filter = "License files (*.lic)|*.lic|Text files (*.txt)|*.txt",
            FileName = (suggested.Length == 0 ? "license" : suggested) + ".lic"
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            File.WriteAllText(dlg.FileName, txtKey.Text);
        }
        catch (Exception ex)
        {
            Error("Could not save the file.\n\n" + ex.Message);
        }
    }

    // ---- helpers ----------------------------------------------------------------------------

    private void CopyToClipboard(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        try { Clipboard.SetText(text); }
        catch (Exception ex) { Error("Could not copy to the clipboard.\n\n" + ex.Message); }
    }

    private void Error(string message) =>
        MessageBox.Show(this, message, "License Keygen", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private static string Sanitize(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }
}
