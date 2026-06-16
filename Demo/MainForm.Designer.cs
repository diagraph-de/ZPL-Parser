namespace ZPLParser.Demo;

partial class MainForm
{
    private System.Windows.Forms.SplitContainer _rootSplit = null!;
    private System.Windows.Forms.FlowLayoutPanel _leftToolbar = null!;
    private System.Windows.Forms.Button _overviewButton = null!;
    private System.Windows.Forms.Button _graphicsButton = null!;
    private System.Windows.Forms.Button _barcodeMixButton = null!;
    private System.Windows.Forms.Button _labelButton = null!;
    private System.Windows.Forms.Button _openZplButton = null!;
    private System.Windows.Forms.Button _refreshButton = null!;
    private System.Windows.Forms.Button _copyZplButton = null!;
    private System.Windows.Forms.RichTextBox _zplInput = null!;
    private System.Windows.Forms.Panel _leftFooterPanel = null!;
    private System.Windows.Forms.GroupBox _previewSettingsGroup = null!;
    private System.Windows.Forms.TableLayoutPanel _previewSettingsLayout = null!;
    private System.Windows.Forms.Label _printDensityLabel = null!;
    private System.Windows.Forms.ComboBox _printDensityComboBox = null!;
    private System.Windows.Forms.Label _printQualityLabel = null!;
    private System.Windows.Forms.ComboBox _printQualityComboBox = null!;
    private System.Windows.Forms.Label _labelSizeLabel = null!;
    private System.Windows.Forms.TextBox _labelWidthTextBox = null!;
    private System.Windows.Forms.Label _labelSizeSeparatorLabel = null!;
    private System.Windows.Forms.TextBox _labelHeightTextBox = null!;
    private System.Windows.Forms.ComboBox _labelUnitComboBox = null!;
    private System.Windows.Forms.Label _showLabelLabel = null!;
    private System.Windows.Forms.NumericUpDown _showLabelIndexNumeric = null!;
    private System.Windows.Forms.Label _showLabelOfLabel = null!;
    private System.Windows.Forms.NumericUpDown _showLabelTotalNumeric = null!;
    private System.Windows.Forms.Label _apiSettingsLabel = null!;
    private System.Windows.Forms.TextBox _apiHostTextBox = null!;
    private System.Windows.Forms.TextBox _apiKeyTextBox = null!;
    private System.Windows.Forms.CheckBox _rememberLastLabelCheckBox = null!;
    private System.Windows.Forms.TabControl _rightTabs = null!;
    private System.Windows.Forms.TabPage _previewTab = null!;
    private System.Windows.Forms.Panel _previewToolbar = null!;
    private System.Windows.Forms.Label _zoomLabel = null!;
    private System.Windows.Forms.ComboBox _zoomComboBox = null!;
    private System.Windows.Forms.CheckBox _fitToWindowCheckBox = null!;
    private System.Windows.Forms.Panel _previewHost = null!;
    private System.Windows.Forms.PictureBox _previewPictureBox = null!;
    private System.Windows.Forms.TabPage _normalizedZplTab = null!;
    private System.Windows.Forms.TextBox _normalizedZplTextBox = null!;
    private System.Windows.Forms.TabPage _treeTab = null!;
    private System.Windows.Forms.TextBox _treeTextBox = null!;
    private System.Windows.Forms.TabPage _elementsTab = null!;
    private System.Windows.Forms.DataGridView _elementsGrid = null!;
    private System.Windows.Forms.Label _statusLabel = null!;

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        _rootSplit = new System.Windows.Forms.SplitContainer();
        _zplInput = new System.Windows.Forms.RichTextBox();
        _leftToolbar = new System.Windows.Forms.FlowLayoutPanel();
        _overviewButton = new System.Windows.Forms.Button();
        _graphicsButton = new System.Windows.Forms.Button();
        _barcodeMixButton = new System.Windows.Forms.Button();
        _labelButton = new System.Windows.Forms.Button();
        _openZplButton = new System.Windows.Forms.Button();
        _refreshButton = new System.Windows.Forms.Button();
        _copyZplButton = new System.Windows.Forms.Button();
        _leftFooterPanel = new System.Windows.Forms.Panel();
        _previewSettingsGroup = new System.Windows.Forms.GroupBox();
        _previewSettingsLayout = new System.Windows.Forms.TableLayoutPanel();
        _printDensityLabel = new System.Windows.Forms.Label();
        _printDensityComboBox = new System.Windows.Forms.ComboBox();
        _printQualityLabel = new System.Windows.Forms.Label();
        _printQualityComboBox = new System.Windows.Forms.ComboBox();
        _labelSizeLabel = new System.Windows.Forms.Label();
        _labelWidthTextBox = new System.Windows.Forms.TextBox();
        _labelSizeSeparatorLabel = new System.Windows.Forms.Label();
        _labelHeightTextBox = new System.Windows.Forms.TextBox();
        _labelUnitComboBox = new System.Windows.Forms.ComboBox();
        _showLabelLabel = new System.Windows.Forms.Label();
        _showLabelIndexNumeric = new System.Windows.Forms.NumericUpDown();
        _showLabelOfLabel = new System.Windows.Forms.Label();
        _showLabelTotalNumeric = new System.Windows.Forms.NumericUpDown();
        _apiSettingsLabel = new System.Windows.Forms.Label();
        _apiHostTextBox = new System.Windows.Forms.TextBox();
        _apiKeyTextBox = new System.Windows.Forms.TextBox();
        _rememberLastLabelCheckBox = new System.Windows.Forms.CheckBox();
        _rightTabs = new System.Windows.Forms.TabControl();
        _previewTab = new System.Windows.Forms.TabPage();
        _previewHost = new System.Windows.Forms.Panel();
        _previewPictureBox = new System.Windows.Forms.PictureBox();
        _previewToolbar = new System.Windows.Forms.Panel();
        _zoomLabel = new System.Windows.Forms.Label();
        _zoomComboBox = new System.Windows.Forms.ComboBox();
        _fitToWindowCheckBox = new System.Windows.Forms.CheckBox();
        _normalizedZplTab = new System.Windows.Forms.TabPage();
        _normalizedZplTextBox = new System.Windows.Forms.TextBox();
        _treeTab = new System.Windows.Forms.TabPage();
        _treeTextBox = new System.Windows.Forms.TextBox();
        _elementsTab = new System.Windows.Forms.TabPage();
        _elementsGrid = new System.Windows.Forms.DataGridView();
        _statusLabel = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)_rootSplit).BeginInit();
        _rootSplit.Panel1.SuspendLayout();
        _rootSplit.Panel2.SuspendLayout();
        _rootSplit.SuspendLayout();
        _leftToolbar.SuspendLayout();
        _leftFooterPanel.SuspendLayout();
        _previewSettingsGroup.SuspendLayout();
        _previewSettingsLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_showLabelIndexNumeric).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_showLabelTotalNumeric).BeginInit();
        _rightTabs.SuspendLayout();
        _previewTab.SuspendLayout();
        _previewHost.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_previewPictureBox).BeginInit();
        _previewToolbar.SuspendLayout();
        _normalizedZplTab.SuspendLayout();
        _treeTab.SuspendLayout();
        _elementsTab.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_elementsGrid).BeginInit();
        SuspendLayout();
        // 
        // _rootSplit
        // 
        _rootSplit.Dock = System.Windows.Forms.DockStyle.Fill;
        _rootSplit.Location = new System.Drawing.Point(0, 0);
        _rootSplit.Name = "_rootSplit";
        // 
        // _rootSplit.Panel1
        // 
        _rootSplit.Panel1.Controls.Add(_zplInput);
        _rootSplit.Panel1.Controls.Add(_leftToolbar);
        _rootSplit.Panel1.Controls.Add(_leftFooterPanel);
        // 
        // _rootSplit.Panel2
        // 
        _rootSplit.Panel2.Controls.Add(_rightTabs);
        _rootSplit.Size = new System.Drawing.Size(1055, 542);
        _rootSplit.SplitterDistance = 612;
        _rootSplit.TabIndex = 0;
        // 
        // _zplInput
        // 
        _zplInput.AcceptsTab = true;
        _zplInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        _zplInput.DetectUrls = false;
        _zplInput.Dock = System.Windows.Forms.DockStyle.Fill;
        _zplInput.Font = new System.Drawing.Font("Consolas", 10F);
        _zplInput.Location = new System.Drawing.Point(0, 39);
        _zplInput.Name = "_zplInput";
        _zplInput.Size = new System.Drawing.Size(612, 291);
        _zplInput.TabIndex = 0;
        _zplInput.Text = "";
        _zplInput.WordWrap = false;
        // 
        // _leftToolbar
        // 
        _leftToolbar.Controls.Add(_overviewButton);
        _leftToolbar.Controls.Add(_graphicsButton);
        _leftToolbar.Controls.Add(_barcodeMixButton);
        _leftToolbar.Controls.Add(_labelButton);
        _leftToolbar.Controls.Add(_openZplButton);
        _leftToolbar.Controls.Add(_refreshButton);
        _leftToolbar.Controls.Add(_copyZplButton);
        _leftToolbar.Dock = System.Windows.Forms.DockStyle.Top;
        _leftToolbar.Location = new System.Drawing.Point(0, 0);
        _leftToolbar.Name = "_leftToolbar";
        _leftToolbar.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
        _leftToolbar.Size = new System.Drawing.Size(612, 39);
        _leftToolbar.TabIndex = 1;
        _leftToolbar.WrapContents = false;
        // 
        // _overviewButton
        // 
        _overviewButton.AutoSize = true;
        _overviewButton.Location = new System.Drawing.Point(8, 6);
        _overviewButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        _overviewButton.Name = "_overviewButton";
        _overviewButton.Size = new System.Drawing.Size(75, 23);
        _overviewButton.TabIndex = 0;
        _overviewButton.TabStop = false;
        _overviewButton.Text = "Overview";
        // 
        // _graphicsButton
        // 
        _graphicsButton.AutoSize = true;
        _graphicsButton.Location = new System.Drawing.Point(91, 6);
        _graphicsButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        _graphicsButton.Name = "_graphicsButton";
        _graphicsButton.Size = new System.Drawing.Size(75, 23);
        _graphicsButton.TabIndex = 1;
        _graphicsButton.TabStop = false;
        _graphicsButton.Text = "Graphics";
        // 
        // _barcodeMixButton
        // 
        _barcodeMixButton.AutoSize = true;
        _barcodeMixButton.Location = new System.Drawing.Point(174, 6);
        _barcodeMixButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        _barcodeMixButton.Name = "_barcodeMixButton";
        _barcodeMixButton.Size = new System.Drawing.Size(76, 23);
        _barcodeMixButton.TabIndex = 2;
        _barcodeMixButton.TabStop = false;
        _barcodeMixButton.Text = "Barcode Mix";
        // 
        // _labelButton
        // 
        _labelButton.AutoSize = true;
        _labelButton.Location = new System.Drawing.Point(258, 6);
        _labelButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        _labelButton.Name = "_labelButton";
        _labelButton.Size = new System.Drawing.Size(75, 23);
        _labelButton.TabIndex = 3;
        _labelButton.TabStop = false;
        _labelButton.Text = "Label";
        // 
        // _openZplButton
        // 
        _openZplButton.AutoSize = true;
        _openZplButton.Location = new System.Drawing.Point(341, 6);
        _openZplButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        _openZplButton.Name = "_openZplButton";
        _openZplButton.Size = new System.Drawing.Size(75, 23);
        _openZplButton.TabIndex = 4;
        _openZplButton.TabStop = false;
        _openZplButton.Text = "Open ZPL";
        // 
        // _refreshButton
        // 
        _refreshButton.AutoSize = true;
        _refreshButton.Location = new System.Drawing.Point(424, 6);
        _refreshButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        _refreshButton.Name = "_refreshButton";
        _refreshButton.Size = new System.Drawing.Size(75, 23);
        _refreshButton.TabIndex = 5;
        _refreshButton.TabStop = false;
        _refreshButton.Text = "Refresh";
        // 
        // _copyZplButton
        // 
        _copyZplButton.AutoSize = true;
        _copyZplButton.Location = new System.Drawing.Point(507, 6);
        _copyZplButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        _copyZplButton.Name = "_copyZplButton";
        _copyZplButton.Size = new System.Drawing.Size(75, 23);
        _copyZplButton.TabIndex = 6;
        _copyZplButton.TabStop = false;
        _copyZplButton.Text = "Copy ZPL";
        // 
        // _leftFooterPanel
        // 
        _leftFooterPanel.Controls.Add(_previewSettingsGroup);
        _leftFooterPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
        _leftFooterPanel.Location = new System.Drawing.Point(0, 330);
        _leftFooterPanel.Name = "_leftFooterPanel";
        _leftFooterPanel.Padding = new System.Windows.Forms.Padding(8, 4, 8, 8);
        _leftFooterPanel.Size = new System.Drawing.Size(612, 212);
        _leftFooterPanel.TabIndex = 2;
        // 
        // _previewSettingsGroup
        // 
        _previewSettingsGroup.Controls.Add(_previewSettingsLayout);
        _previewSettingsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
        _previewSettingsGroup.Location = new System.Drawing.Point(8, 4);
        _previewSettingsGroup.Name = "_previewSettingsGroup";
        _previewSettingsGroup.Size = new System.Drawing.Size(599, 200);
        _previewSettingsGroup.TabIndex = 0;
        _previewSettingsGroup.TabStop = false;
        _previewSettingsGroup.Text = "Preview Settings";
        // 
        // _previewSettingsLayout
        // 
        _previewSettingsLayout.ColumnCount = 5;
        _previewSettingsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
        _previewSettingsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
        _previewSettingsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
        _previewSettingsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
        _previewSettingsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
        _previewSettingsLayout.Controls.Add(_printDensityLabel, 0, 0);
        _previewSettingsLayout.Controls.Add(_printDensityComboBox, 1, 0);
        _previewSettingsLayout.Controls.Add(_printQualityLabel, 2, 0);
        _previewSettingsLayout.Controls.Add(_printQualityComboBox, 3, 0);
        _previewSettingsLayout.Controls.Add(_labelSizeLabel, 0, 1);
        _previewSettingsLayout.Controls.Add(_labelWidthTextBox, 1, 1);
        _previewSettingsLayout.Controls.Add(_labelSizeSeparatorLabel, 2, 1);
        _previewSettingsLayout.Controls.Add(_labelHeightTextBox, 3, 1);
        _previewSettingsLayout.Controls.Add(_labelUnitComboBox, 4, 1);
        _previewSettingsLayout.Controls.Add(_showLabelLabel, 0, 2);
        _previewSettingsLayout.Controls.Add(_showLabelIndexNumeric, 1, 2);
        _previewSettingsLayout.Controls.Add(_showLabelOfLabel, 2, 2);
        _previewSettingsLayout.Controls.Add(_showLabelTotalNumeric, 3, 2);
        _previewSettingsLayout.Controls.Add(_apiSettingsLabel, 0, 3);
        _previewSettingsLayout.Controls.Add(_apiHostTextBox, 1, 3);
        _previewSettingsLayout.Controls.Add(_apiKeyTextBox, 3, 3);
        _previewSettingsLayout.Controls.Add(_rememberLastLabelCheckBox, 1, 4);
        _previewSettingsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        _previewSettingsLayout.Location = new System.Drawing.Point(3, 19);
        _previewSettingsLayout.Name = "_previewSettingsLayout";
        _previewSettingsLayout.RowCount = 5;
        _previewSettingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
        _previewSettingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
        _previewSettingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
        _previewSettingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
        _previewSettingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
        _previewSettingsLayout.Size = new System.Drawing.Size(593, 178);
        _previewSettingsLayout.TabIndex = 0;
        // 
        // _printDensityLabel
        // 
        _printDensityLabel.AutoSize = true;
        _printDensityLabel.Dock = System.Windows.Forms.DockStyle.Fill;
        _printDensityLabel.Location = new System.Drawing.Point(3, 0);
        _printDensityLabel.Name = "_printDensityLabel";
        _printDensityLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
        _printDensityLabel.Size = new System.Drawing.Size(75, 28);
        _printDensityLabel.TabIndex = 0;
        _printDensityLabel.Text = "Print Density:";
        // 
        // _printDensityComboBox
        // 
        _printDensityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        _printDensityComboBox.FormattingEnabled = true;
        _printDensityComboBox.Items.AddRange(new object[] { "6 dpmm (152 dpi)", "8 dpmm (203 dpi)", "12 dpmm (300 dpi)", "24 dpmm (600 dpi)" });
        _printDensityComboBox.Location = new System.Drawing.Point(84, 3);
        _printDensityComboBox.Name = "_printDensityComboBox";
        _printDensityComboBox.Size = new System.Drawing.Size(130, 21);
        _printDensityComboBox.TabIndex = 1;
        // 
        // _printQualityLabel
        // 
        _printQualityLabel.AutoSize = true;
        _printQualityLabel.Dock = System.Windows.Forms.DockStyle.Fill;
        _printQualityLabel.Location = new System.Drawing.Point(220, 0);
        _printQualityLabel.Name = "_printQualityLabel";
        _printQualityLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
        _printQualityLabel.Size = new System.Drawing.Size(69, 28);
        _printQualityLabel.TabIndex = 2;
        _printQualityLabel.Text = "Print Quality:";
        // 
        // _printQualityComboBox
        // 
        _printQualityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        _printQualityComboBox.FormattingEnabled = true;
        _printQualityComboBox.Items.AddRange(new object[] { "Grayscale", "Bitonal" });
        _printQualityComboBox.Location = new System.Drawing.Point(295, 3);
        _printQualityComboBox.Name = "_printQualityComboBox";
        _printQualityComboBox.Size = new System.Drawing.Size(120, 21);
        _printQualityComboBox.TabIndex = 3;
        // 
        // _labelSizeLabel
        // 
        _labelSizeLabel.AutoSize = true;
        _labelSizeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
        _labelSizeLabel.Location = new System.Drawing.Point(3, 28);
        _labelSizeLabel.Name = "_labelSizeLabel";
        _labelSizeLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
        _labelSizeLabel.Size = new System.Drawing.Size(75, 28);
        _labelSizeLabel.TabIndex = 4;
        _labelSizeLabel.Text = "Label Size:";
        // 
        // _labelWidthTextBox
        // 
        _labelWidthTextBox.Location = new System.Drawing.Point(84, 31);
        _labelWidthTextBox.Name = "_labelWidthTextBox";
        _labelWidthTextBox.Size = new System.Drawing.Size(56, 20);
        _labelWidthTextBox.TabIndex = 5;
        _labelWidthTextBox.Text = "4";
        // 
        // _labelSizeSeparatorLabel
        // 
        _labelSizeSeparatorLabel.AutoSize = true;
        _labelSizeSeparatorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
        _labelSizeSeparatorLabel.Location = new System.Drawing.Point(220, 28);
        _labelSizeSeparatorLabel.Name = "_labelSizeSeparatorLabel";
        _labelSizeSeparatorLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
        _labelSizeSeparatorLabel.Size = new System.Drawing.Size(12, 28);
        _labelSizeSeparatorLabel.TabIndex = 6;
        _labelSizeSeparatorLabel.Text = "x";
        // 
        // _labelHeightTextBox
        // 
        _labelHeightTextBox.Location = new System.Drawing.Point(295, 31);
        _labelHeightTextBox.Name = "_labelHeightTextBox";
        _labelHeightTextBox.Size = new System.Drawing.Size(56, 20);
        _labelHeightTextBox.TabIndex = 7;
        _labelHeightTextBox.Text = "6";
        // 
        // _labelUnitComboBox
        // 
        _labelUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        _labelUnitComboBox.FormattingEnabled = true;
        _labelUnitComboBox.Items.AddRange(new object[] { "inches", "mm", "cm" });
        _labelUnitComboBox.Location = new System.Drawing.Point(357, 31);
        _labelUnitComboBox.Name = "_labelUnitComboBox";
        _labelUnitComboBox.Size = new System.Drawing.Size(58, 21);
        _labelUnitComboBox.TabIndex = 8;
        // 
        // _showLabelLabel
        // 
        _showLabelLabel.AutoSize = true;
        _showLabelLabel.Dock = System.Windows.Forms.DockStyle.Fill;
        _showLabelLabel.Location = new System.Drawing.Point(3, 56);
        _showLabelLabel.Name = "_showLabelLabel";
        _showLabelLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
        _showLabelLabel.Size = new System.Drawing.Size(75, 28);
        _showLabelLabel.TabIndex = 9;
        _showLabelLabel.Text = "Show Label:";
        // 
        // _showLabelIndexNumeric
        // 
        _showLabelIndexNumeric.Location = new System.Drawing.Point(84, 59);
        _showLabelIndexNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        _showLabelIndexNumeric.Name = "_showLabelIndexNumeric";
        _showLabelIndexNumeric.Size = new System.Drawing.Size(56, 20);
        _showLabelIndexNumeric.TabIndex = 10;
        _showLabelIndexNumeric.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // _showLabelOfLabel
        // 
        _showLabelOfLabel.AutoSize = true;
        _showLabelOfLabel.Dock = System.Windows.Forms.DockStyle.Fill;
        _showLabelOfLabel.Location = new System.Drawing.Point(220, 56);
        _showLabelOfLabel.Name = "_showLabelOfLabel";
        _showLabelOfLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
        _showLabelOfLabel.Size = new System.Drawing.Size(52, 28);
        _showLabelOfLabel.TabIndex = 11;
        _showLabelOfLabel.Text = "of";
        // 
        // _showLabelTotalNumeric
        // 
        _showLabelTotalNumeric.Location = new System.Drawing.Point(295, 59);
        _showLabelTotalNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        _showLabelTotalNumeric.Name = "_showLabelTotalNumeric";
        _showLabelTotalNumeric.Size = new System.Drawing.Size(56, 20);
        _showLabelTotalNumeric.TabIndex = 12;
        _showLabelTotalNumeric.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // _apiSettingsLabel
        // 
        _apiSettingsLabel.AutoSize = true;
        _apiSettingsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
        _apiSettingsLabel.Location = new System.Drawing.Point(3, 84);
        _apiSettingsLabel.Name = "_apiSettingsLabel";
        _apiSettingsLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
        _apiSettingsLabel.Size = new System.Drawing.Size(75, 28);
        _apiSettingsLabel.TabIndex = 13;
        _apiSettingsLabel.Text = "Renderer Settings:";
        _apiSettingsLabel.Visible = false;
        // 
        // _apiHostTextBox
        // 
        _previewSettingsLayout.SetColumnSpan(_apiHostTextBox, 2);
        _apiHostTextBox.Location = new System.Drawing.Point(84, 87);
        _apiHostTextBox.Name = "_apiHostTextBox";
        _apiHostTextBox.Size = new System.Drawing.Size(185, 20);
        _apiHostTextBox.TabIndex = 14;
        _apiHostTextBox.Visible = false;
        // 
        // _apiKeyTextBox
        // 
        _previewSettingsLayout.SetColumnSpan(_apiKeyTextBox, 2);
        _apiKeyTextBox.Location = new System.Drawing.Point(295, 87);
        _apiKeyTextBox.Name = "_apiKeyTextBox";
        _apiKeyTextBox.Size = new System.Drawing.Size(180, 20);
        _apiKeyTextBox.TabIndex = 15;
        _apiKeyTextBox.UseSystemPasswordChar = true;
        _apiKeyTextBox.Visible = false;
        // 
        // _rememberLastLabelCheckBox
        // 
        _rememberLastLabelCheckBox.AutoSize = true;
        _previewSettingsLayout.SetColumnSpan(_rememberLastLabelCheckBox, 3);
        _rememberLastLabelCheckBox.Location = new System.Drawing.Point(84, 113);
        _rememberLastLabelCheckBox.Name = "_rememberLastLabelCheckBox";
        _rememberLastLabelCheckBox.Size = new System.Drawing.Size(207, 17);
        _rememberLastLabelCheckBox.TabIndex = 16;
        _rememberLastLabelCheckBox.Text = "Remember my last label (stored locally)";
        _rememberLastLabelCheckBox.UseVisualStyleBackColor = true;
        // 
        // _rightTabs
        // 
        _rightTabs.Controls.Add(_previewTab);
        _rightTabs.Controls.Add(_normalizedZplTab);
        _rightTabs.Controls.Add(_treeTab);
        _rightTabs.Controls.Add(_elementsTab);
        _rightTabs.Dock = System.Windows.Forms.DockStyle.Fill;
        _rightTabs.Location = new System.Drawing.Point(0, 0);
        _rightTabs.Name = "_rightTabs";
        _rightTabs.SelectedIndex = 0;
        _rightTabs.Size = new System.Drawing.Size(439, 542);
        _rightTabs.TabIndex = 1;
        // 
        // _previewTab
        // 
        _previewTab.Controls.Add(_previewHost);
        _previewTab.Controls.Add(_previewToolbar);
        _previewTab.Location = new System.Drawing.Point(4, 22);
        _previewTab.Name = "_previewTab";
        _previewTab.Size = new System.Drawing.Size(431, 516);
        _previewTab.TabIndex = 0;
        _previewTab.Text = "Preview";
        // 
        // _previewHost
        // 
        _previewHost.AutoScroll = true;
        _previewHost.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
        _previewHost.Controls.Add(_previewPictureBox);
        _previewHost.Dock = System.Windows.Forms.DockStyle.Fill;
        _previewHost.Location = new System.Drawing.Point(0, 36);
        _previewHost.Name = "_previewHost";
        _previewHost.Padding = new System.Windows.Forms.Padding(16);
        _previewHost.Size = new System.Drawing.Size(431, 480);
        _previewHost.TabIndex = 0;
        // 
        // _previewPictureBox
        // 
        _previewPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        _previewPictureBox.BackColor = System.Drawing.Color.White;
        _previewPictureBox.Location = new System.Drawing.Point(19, 16);
        _previewPictureBox.Name = "_previewPictureBox";
        _previewPictureBox.Size = new System.Drawing.Size(396, 448);
        _previewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        _previewPictureBox.TabIndex = 0;
        _previewPictureBox.TabStop = false;
        // 
        // _previewToolbar
        // 
        _previewToolbar.Controls.Add(_zoomLabel);
        _previewToolbar.Controls.Add(_zoomComboBox);
        _previewToolbar.Controls.Add(_fitToWindowCheckBox);
        _previewToolbar.Dock = System.Windows.Forms.DockStyle.Top;
        _previewToolbar.Location = new System.Drawing.Point(0, 0);
        _previewToolbar.Name = "_previewToolbar";
        _previewToolbar.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
        _previewToolbar.Size = new System.Drawing.Size(431, 36);
        _previewToolbar.TabIndex = 1;
        // 
        // _zoomLabel
        // 
        _zoomLabel.AutoSize = true;
        _zoomLabel.Dock = System.Windows.Forms.DockStyle.Left;
        _zoomLabel.Location = new System.Drawing.Point(8, 6);
        _zoomLabel.Name = "_zoomLabel";
        _zoomLabel.Padding = new System.Windows.Forms.Padding(0, 7, 4, 0);
        _zoomLabel.Size = new System.Drawing.Size(38, 20);
        _zoomLabel.TabIndex = 0;
        _zoomLabel.Text = "Zoom";
        _zoomLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // _zoomComboBox
        // 
        _zoomComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        _zoomComboBox.Items.AddRange(new object[] { "Fit", "25%", "50%", "75%", "100%", "150%", "200%" });
        _zoomComboBox.Location = new System.Drawing.Point(52, 9);
        _zoomComboBox.Name = "_zoomComboBox";
        _zoomComboBox.Size = new System.Drawing.Size(90, 21);
        _zoomComboBox.TabIndex = 0;
        // 
        // _fitToWindowCheckBox
        // 
        _fitToWindowCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        _fitToWindowCheckBox.AutoSize = true;
        _fitToWindowCheckBox.Checked = true;
        _fitToWindowCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        _fitToWindowCheckBox.Location = new System.Drawing.Point(324, 6);
        _fitToWindowCheckBox.Name = "_fitToWindowCheckBox";
        _fitToWindowCheckBox.Padding = new System.Windows.Forms.Padding(8, 5, 0, 0);
        _fitToWindowCheckBox.Size = new System.Drawing.Size(96, 22);
        _fitToWindowCheckBox.TabIndex = 1;
        _fitToWindowCheckBox.Text = "Fit to window";
        // 
        // _normalizedZplTab
        // 
        _normalizedZplTab.Controls.Add(_normalizedZplTextBox);
        _normalizedZplTab.Location = new System.Drawing.Point(4, 22);
        _normalizedZplTab.Name = "_normalizedZplTab";
        _normalizedZplTab.Size = new System.Drawing.Size(543, 516);
        _normalizedZplTab.TabIndex = 1;
        _normalizedZplTab.Text = "Normalized ZPL";
        // 
        // _normalizedZplTextBox
        // 
        _normalizedZplTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
        _normalizedZplTextBox.Font = new System.Drawing.Font("Consolas", 9F);
        _normalizedZplTextBox.Location = new System.Drawing.Point(0, 0);
        _normalizedZplTextBox.Multiline = true;
        _normalizedZplTextBox.Name = "_normalizedZplTextBox";
        _normalizedZplTextBox.ReadOnly = true;
        _normalizedZplTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        _normalizedZplTextBox.Size = new System.Drawing.Size(543, 516);
        _normalizedZplTextBox.TabIndex = 0;
        _normalizedZplTextBox.WordWrap = false;
        // 
        // _treeTab
        // 
        _treeTab.Controls.Add(_treeTextBox);
        _treeTab.Location = new System.Drawing.Point(4, 22);
        _treeTab.Name = "_treeTab";
        _treeTab.Size = new System.Drawing.Size(543, 516);
        _treeTab.TabIndex = 2;
        _treeTab.Text = "Parser Tree";
        // 
        // _treeTextBox
        // 
        _treeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
        _treeTextBox.Font = new System.Drawing.Font("Consolas", 9F);
        _treeTextBox.Location = new System.Drawing.Point(0, 0);
        _treeTextBox.Multiline = true;
        _treeTextBox.Name = "_treeTextBox";
        _treeTextBox.ReadOnly = true;
        _treeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        _treeTextBox.Size = new System.Drawing.Size(543, 516);
        _treeTextBox.TabIndex = 0;
        _treeTextBox.WordWrap = false;
        // 
        // _elementsTab
        // 
        _elementsTab.Controls.Add(_elementsGrid);
        _elementsTab.Location = new System.Drawing.Point(4, 22);
        _elementsTab.Name = "_elementsTab";
        _elementsTab.Size = new System.Drawing.Size(543, 516);
        _elementsTab.TabIndex = 3;
        _elementsTab.Text = "Elements";
        // 
        // _elementsGrid
        // 
        _elementsGrid.AllowUserToAddRows = false;
        _elementsGrid.AllowUserToDeleteRows = false;
        _elementsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
        _elementsGrid.Location = new System.Drawing.Point(0, 0);
        _elementsGrid.MultiSelect = false;
        _elementsGrid.Name = "_elementsGrid";
        _elementsGrid.ReadOnly = true;
        _elementsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        _elementsGrid.Size = new System.Drawing.Size(543, 516);
        _elementsGrid.TabIndex = 0;
        // 
        // _statusLabel
        // 
        _statusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
        _statusLabel.Location = new System.Drawing.Point(0, 542);
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
        _statusLabel.Size = new System.Drawing.Size(1055, 24);
        _statusLabel.TabIndex = 1;
        _statusLabel.Text = "Ready";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1055, 566);
        Controls.Add(_rootSplit);
        Controls.Add(_statusLabel);
        Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
        Name = "MainForm";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "ZPL Parser Demo";
        WindowState = System.Windows.Forms.FormWindowState.Maximized;
        _rootSplit.Panel1.ResumeLayout(false);
        _rootSplit.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_rootSplit).EndInit();
        _rootSplit.ResumeLayout(false);
        _leftToolbar.ResumeLayout(false);
        _leftToolbar.PerformLayout();
        _leftFooterPanel.ResumeLayout(false);
        _previewSettingsGroup.ResumeLayout(false);
        _previewSettingsLayout.ResumeLayout(false);
        _previewSettingsLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_showLabelIndexNumeric).EndInit();
        ((System.ComponentModel.ISupportInitialize)_showLabelTotalNumeric).EndInit();
        _rightTabs.ResumeLayout(false);
        _previewTab.ResumeLayout(false);
        _previewHost.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_previewPictureBox).EndInit();
        _previewToolbar.ResumeLayout(false);
        _previewToolbar.PerformLayout();
        _normalizedZplTab.ResumeLayout(false);
        _normalizedZplTab.PerformLayout();
        _treeTab.ResumeLayout(false);
        _treeTab.PerformLayout();
        _elementsTab.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_elementsGrid).EndInit();
        ResumeLayout(false);
    }
}
