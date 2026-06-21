using System.Runtime.InteropServices;
using System.Security.Cryptography;
using DoorsWeb.Licensing;

namespace DoorsWeb.Keygen;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        // Headless mode: `DoorsWeb.Keygen --newkeys [outputDir]` mints a fresh ECDSA P-256 pair and
        // exits, so CI / scripts can rotate keys without the GUI.
        if (args.Length > 0 && args[0].Equals("--newkeys", StringComparison.OrdinalIgnoreCase))
            return NewKeys(args.Length > 1 ? args[1] : Directory.GetCurrentDirectory());

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
        return 0;
    }

    // ---- CLI: generate a key pair -------------------------------------------------------------

    private static int NewKeys(string outputDir)
    {
        // This is a WinExe (GUI subsystem); attaching to the launching console lets our output show
        // when run from a terminal. Falls back silently to file output when there's no console.
        AttachConsole(ATTACH_PARENT_PROCESS);

        try
        {
            Directory.CreateDirectory(outputDir);
            var (privatePem, publicPem) = LicenseKeys.CreateKeyPair();

            using var ec = LicenseKeys.LoadPublicKey(publicPem);
            var publicBase64 = LicenseKeys.ExportPublicKeyBase64(ec);

            var privatePath = Path.Combine(outputDir, "doorsweb.private.pem");
            var publicPath = Path.Combine(outputDir, "doorsweb.public.pem");
            var base64Path = Path.Combine(outputDir, "doorsweb.public.base64.txt");

            File.WriteAllText(privatePath, privatePem);
            File.WriteAllText(publicPath, publicPem);
            File.WriteAllText(base64Path, publicBase64);

            Console.WriteLine();
            Console.WriteLine("Generated a new ECDSA P-256 license key pair:");
            Console.WriteLine($"  private key : {privatePath}   (KEEP SECRET — never commit or ship)");
            Console.WriteLine($"  public key  : {publicPath}");
            Console.WriteLine($"  public b64  : {base64Path}");
            Console.WriteLine();
            Console.WriteLine("Put this single line into appsettings Licensing:PublicKey —");
            Console.WriteLine(publicBase64);
            Console.WriteLine();
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to generate keys: {ex.Message}");
            return 1;
        }
    }

    private const int ATTACH_PARENT_PROCESS = -1;

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AttachConsole(int dwProcessId);
}
