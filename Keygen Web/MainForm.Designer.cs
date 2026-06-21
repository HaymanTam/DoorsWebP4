namespace DoorsWeb.Keygen;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            _privateKey?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        grpKey = new GroupBox();
        lblKeyStatus = new Label();
        btnLoadKey = new Button();
        btnNewKey = new Button();
        lblPublic = new Label();
        txtPublic = new TextBox();
        btnCopyPublic = new Button();

        grpDetails = new GroupBox();
        lblCustomer = new Label();
        txtCustomer = new TextBox();
        lblLicenseId = new Label();
        txtLicenseId = new TextBox();
        btnNewId = new Button();
        lblMaxDoors = new Label();
        numMaxDoors = new NumericUpDown();
        lblMaxCards = new Label();
        numMaxCards = new NumericUpDown();
        lblExpiry = new Label();
        dtExpiry = new DateTimePicker();

        grpOutput = new GroupBox();
        btnGenerate = new Button();
        txtKey = new TextBox();
        btnCopyKey = new Button();
        btnSaveLic = new Button();

        grpKey.SuspendLayout();
        grpDetails.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numMaxDoors).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numMaxCards).BeginInit();
        grpOutput.SuspendLayout();
        SuspendLayout();

        // grpKey
        grpKey.Controls.Add(lblKeyStatus);
        grpKey.Controls.Add(btnLoadKey);
        grpKey.Controls.Add(btnNewKey);
        grpKey.Controls.Add(lblPublic);
        grpKey.Controls.Add(txtPublic);
        grpKey.Controls.Add(btnCopyPublic);
        grpKey.Location = new Point(12, 12);
        grpKey.Size = new Size(636, 200);
        grpKey.TabIndex = 0;
        grpKey.TabStop = false;
        grpKey.Text = "Signing Key (private — issuer only)";

        lblKeyStatus.AutoSize = true;
        lblKeyStatus.Location = new Point(15, 28);
        lblKeyStatus.Text = "No signing key loaded — load or generate one.";
        lblKeyStatus.ForeColor = Color.Firebrick;

        btnLoadKey.Location = new Point(15, 52);
        btnLoadKey.Size = new Size(160, 30);
        btnLoadKey.TabIndex = 1;
        btnLoadKey.Text = "Load Private Key…";
        btnLoadKey.UseVisualStyleBackColor = true;
        btnLoadKey.Click += btnLoadKey_Click;

        btnNewKey.Location = new Point(185, 52);
        btnNewKey.Size = new Size(200, 30);
        btnNewKey.TabIndex = 2;
        btnNewKey.Text = "Generate New Key Pair…";
        btnNewKey.UseVisualStyleBackColor = true;
        btnNewKey.Click += btnNewKey_Click;

        lblPublic.AutoSize = true;
        lblPublic.Location = new Point(15, 95);
        lblPublic.Text = "Public key (paste into appsettings Licensing:PublicKey):";

        txtPublic.Location = new Point(15, 118);
        txtPublic.Size = new Size(606, 40);
        txtPublic.Multiline = true;
        txtPublic.ReadOnly = true;
        txtPublic.ScrollBars = ScrollBars.Vertical;
        txtPublic.Font = new Font("Consolas", 8.5f);

        btnCopyPublic.Location = new Point(15, 162);
        btnCopyPublic.Size = new Size(160, 28);
        btnCopyPublic.TabIndex = 3;
        btnCopyPublic.Text = "Copy Public Key";
        btnCopyPublic.UseVisualStyleBackColor = true;
        btnCopyPublic.Click += btnCopyPublic_Click;

        // grpDetails
        grpDetails.Controls.Add(lblCustomer);
        grpDetails.Controls.Add(txtCustomer);
        grpDetails.Controls.Add(lblLicenseId);
        grpDetails.Controls.Add(txtLicenseId);
        grpDetails.Controls.Add(btnNewId);
        grpDetails.Controls.Add(lblMaxDoors);
        grpDetails.Controls.Add(numMaxDoors);
        grpDetails.Controls.Add(lblMaxCards);
        grpDetails.Controls.Add(numMaxCards);
        grpDetails.Controls.Add(lblExpiry);
        grpDetails.Controls.Add(dtExpiry);
        grpDetails.Location = new Point(12, 224);
        grpDetails.Size = new Size(636, 175);
        grpDetails.TabIndex = 4;
        grpDetails.TabStop = false;
        grpDetails.Text = "License Details";

        lblCustomer.AutoSize = true;
        lblCustomer.Location = new Point(15, 33);
        lblCustomer.Text = "Customer";

        txtCustomer.Location = new Point(140, 30);
        txtCustomer.Size = new Size(480, 23);
        txtCustomer.TabIndex = 5;

        lblLicenseId.AutoSize = true;
        lblLicenseId.Location = new Point(15, 66);
        lblLicenseId.Text = "License ID";

        txtLicenseId.Location = new Point(140, 63);
        txtLicenseId.Size = new Size(360, 23);
        txtLicenseId.TabIndex = 6;

        btnNewId.Location = new Point(510, 62);
        btnNewId.Size = new Size(110, 25);
        btnNewId.TabIndex = 7;
        btnNewId.Text = "New GUID";
        btnNewId.UseVisualStyleBackColor = true;
        btnNewId.Click += btnNewId_Click;

        lblMaxDoors.AutoSize = true;
        lblMaxDoors.Location = new Point(15, 99);
        lblMaxDoors.Text = "Max Doors";

        numMaxDoors.Location = new Point(140, 96);
        numMaxDoors.Size = new Size(120, 23);
        numMaxDoors.Minimum = 1;
        numMaxDoors.Maximum = 100000;
        numMaxDoors.Value = 10;
        numMaxDoors.TabIndex = 8;

        lblMaxCards.AutoSize = true;
        lblMaxCards.Location = new Point(300, 99);
        lblMaxCards.Text = "Max Cardholders";

        numMaxCards.Location = new Point(430, 96);
        numMaxCards.Size = new Size(120, 23);
        numMaxCards.Minimum = 1;
        numMaxCards.Maximum = 1000000;
        numMaxCards.Value = 100;
        numMaxCards.TabIndex = 9;

        lblExpiry.AutoSize = true;
        lblExpiry.Location = new Point(15, 132);
        lblExpiry.Text = "Expiry (UTC date)";

        dtExpiry.Location = new Point(140, 129);
        dtExpiry.Size = new Size(200, 23);
        dtExpiry.Format = DateTimePickerFormat.Short;
        dtExpiry.TabIndex = 10;

        // grpOutput
        grpOutput.Controls.Add(btnGenerate);
        grpOutput.Controls.Add(txtKey);
        grpOutput.Controls.Add(btnCopyKey);
        grpOutput.Controls.Add(btnSaveLic);
        grpOutput.Location = new Point(12, 411);
        grpOutput.Size = new Size(636, 175);
        grpOutput.TabIndex = 11;
        grpOutput.TabStop = false;
        grpOutput.Text = "License Key";

        btnGenerate.Location = new Point(15, 30);
        btnGenerate.Size = new Size(200, 32);
        btnGenerate.TabIndex = 12;
        btnGenerate.Text = "Generate License Key";
        btnGenerate.UseVisualStyleBackColor = true;
        btnGenerate.Enabled = false;
        btnGenerate.Click += btnGenerate_Click;

        txtKey.Location = new Point(15, 72);
        txtKey.Size = new Size(606, 60);
        txtKey.Multiline = true;
        txtKey.ReadOnly = true;
        txtKey.ScrollBars = ScrollBars.Vertical;
        txtKey.Font = new Font("Consolas", 8.5f);

        btnCopyKey.Location = new Point(15, 138);
        btnCopyKey.Size = new Size(140, 28);
        btnCopyKey.TabIndex = 13;
        btnCopyKey.Text = "Copy Key";
        btnCopyKey.UseVisualStyleBackColor = true;
        btnCopyKey.Enabled = false;
        btnCopyKey.Click += btnCopyKey_Click;

        btnSaveLic.Location = new Point(165, 138);
        btnSaveLic.Size = new Size(140, 28);
        btnSaveLic.TabIndex = 14;
        btnSaveLic.Text = "Save .lic…";
        btnSaveLic.UseVisualStyleBackColor = true;
        btnSaveLic.Enabled = false;
        btnSaveLic.Click += btnSaveLic_Click;

        // MainForm
        AutoScaleDimensions = new SizeF(7f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(660, 600);
        Controls.Add(grpKey);
        Controls.Add(grpDetails);
        Controls.Add(grpOutput);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "DoorsWeb License Keygen";

        grpKey.ResumeLayout(false);
        grpKey.PerformLayout();
        grpDetails.ResumeLayout(false);
        grpDetails.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numMaxDoors).EndInit();
        ((System.ComponentModel.ISupportInitialize)numMaxCards).EndInit();
        grpOutput.ResumeLayout(false);
        grpOutput.PerformLayout();
        ResumeLayout(false);
    }

    private GroupBox grpKey;
    private Label lblKeyStatus;
    private Button btnLoadKey;
    private Button btnNewKey;
    private Label lblPublic;
    private TextBox txtPublic;
    private Button btnCopyPublic;

    private GroupBox grpDetails;
    private Label lblCustomer;
    private TextBox txtCustomer;
    private Label lblLicenseId;
    private TextBox txtLicenseId;
    private Button btnNewId;
    private Label lblMaxDoors;
    private NumericUpDown numMaxDoors;
    private Label lblMaxCards;
    private NumericUpDown numMaxCards;
    private Label lblExpiry;
    private DateTimePicker dtExpiry;

    private GroupBox grpOutput;
    private Button btnGenerate;
    private TextBox txtKey;
    private Button btnCopyKey;
    private Button btnSaveLic;
}
