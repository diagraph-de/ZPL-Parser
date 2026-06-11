namespace ZPLParser.Demo;

partial class MainForm
{
    private System.Windows.Forms.SplitContainer _rootSplit = null!;
    private System.Windows.Forms.FlowLayoutPanel _leftToolbar = null!;
    private System.Windows.Forms.Button _overviewButton = null!;
    private System.Windows.Forms.Button _graphicsButton = null!;
    private System.Windows.Forms.Button _barcodeMixButton = null!;
    private System.Windows.Forms.Button _openZplButton = null!;
    private System.Windows.Forms.Button _refreshButton = null!;
    private System.Windows.Forms.Button _copyZplButton = null!;
    private System.Windows.Forms.TextBox _zplInput = null!;
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
        _zplInput = new System.Windows.Forms.TextBox();
        _leftToolbar = new System.Windows.Forms.FlowLayoutPanel();
        _overviewButton = new System.Windows.Forms.Button();
        _graphicsButton = new System.Windows.Forms.Button();
        _barcodeMixButton = new System.Windows.Forms.Button();
        _openZplButton = new System.Windows.Forms.Button();
        _refreshButton = new System.Windows.Forms.Button();
        _copyZplButton = new System.Windows.Forms.Button();
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
        // 
        // _rootSplit.Panel2
        // 
        _rootSplit.Panel2.Controls.Add(_rightTabs);
        _rootSplit.Size = new System.Drawing.Size(1055, 542);
        _rootSplit.SplitterDistance = 615;
        _rootSplit.TabIndex = 0;
        // 
        // _zplInput
        // 
        _zplInput.AcceptsReturn = true;
        _zplInput.AcceptsTab = true;
        _zplInput.Dock = System.Windows.Forms.DockStyle.Fill;
        _zplInput.Font = new System.Drawing.Font("Consolas", 10F);
        _zplInput.Location = new System.Drawing.Point(0, 39);
        _zplInput.Multiline = true;
        _zplInput.Name = "_zplInput";
        _zplInput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        _zplInput.Size = new System.Drawing.Size(615, 503);
        _zplInput.TabIndex = 0;
        _zplInput.WordWrap = false;
        // 
        // _leftToolbar
        // 
        _leftToolbar.Controls.Add(_overviewButton);
        _leftToolbar.Controls.Add(_graphicsButton);
        _leftToolbar.Controls.Add(_barcodeMixButton);
        _leftToolbar.Controls.Add(_openZplButton);
        _leftToolbar.Controls.Add(_refreshButton);
        _leftToolbar.Controls.Add(_copyZplButton);
        _leftToolbar.Dock = System.Windows.Forms.DockStyle.Top;
        _leftToolbar.Location = new System.Drawing.Point(0, 0);
        _leftToolbar.Name = "_leftToolbar";
        _leftToolbar.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
        _leftToolbar.Size = new System.Drawing.Size(615, 39);
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
        // _openZplButton
        // 
        _openZplButton.AutoSize = true;
        _openZplButton.Location = new System.Drawing.Point(258, 6);
        _openZplButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        _openZplButton.Name = "_openZplButton";
        _openZplButton.Size = new System.Drawing.Size(75, 23);
        _openZplButton.TabIndex = 3;
        _openZplButton.TabStop = false;
        _openZplButton.Text = "Open ZPL";
        // 
        // _refreshButton
        // 
        _refreshButton.AutoSize = true;
        _refreshButton.Location = new System.Drawing.Point(341, 6);
        _refreshButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        _refreshButton.Name = "_refreshButton";
        _refreshButton.Size = new System.Drawing.Size(75, 23);
        _refreshButton.TabIndex = 4;
        _refreshButton.TabStop = false;
        _refreshButton.Text = "Refresh";
        // 
        // _copyZplButton
        // 
        _copyZplButton.AutoSize = true;
        _copyZplButton.Location = new System.Drawing.Point(424, 6);
        _copyZplButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        _copyZplButton.Name = "_copyZplButton";
        _copyZplButton.Size = new System.Drawing.Size(75, 23);
        _copyZplButton.TabIndex = 5;
        _copyZplButton.TabStop = false;
        _copyZplButton.Text = "Copy ZPL";
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
        _rightTabs.Size = new System.Drawing.Size(436, 542);
        _rightTabs.TabIndex = 1;
        // 
        // _previewTab
        // 
        _previewTab.Controls.Add(_previewHost);
        _previewTab.Controls.Add(_previewToolbar);
        _previewTab.Location = new System.Drawing.Point(4, 22);
        _previewTab.Name = "_previewTab";
        _previewTab.Size = new System.Drawing.Size(428, 516);
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
        _previewHost.Size = new System.Drawing.Size(428, 480);
        _previewHost.TabIndex = 0;
        // 
        // _previewPictureBox
        // 
        _previewPictureBox.BackColor = System.Drawing.Color.White;
        _previewPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
        _previewPictureBox.Location = new System.Drawing.Point(16, 16);
        _previewPictureBox.Name = "_previewPictureBox";
        _previewPictureBox.Size = new System.Drawing.Size(396, 448);
        _previewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
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
        _previewToolbar.Size = new System.Drawing.Size(428, 36);
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
        _fitToWindowCheckBox.Location = new System.Drawing.Point(321, 6);
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
        _normalizedZplTab.Size = new System.Drawing.Size(428, 516);
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
        _normalizedZplTextBox.Size = new System.Drawing.Size(428, 516);
        _normalizedZplTextBox.TabIndex = 0;
        _normalizedZplTextBox.WordWrap = false;
        // 
        // _treeTab
        // 
        _treeTab.Controls.Add(_treeTextBox);
        _treeTab.Location = new System.Drawing.Point(4, 22);
        _treeTab.Name = "_treeTab";
        _treeTab.Size = new System.Drawing.Size(428, 516);
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
        _treeTextBox.Size = new System.Drawing.Size(428, 516);
        _treeTextBox.TabIndex = 0;
        _treeTextBox.WordWrap = false;
        // 
        // _elementsTab
        // 
        _elementsTab.Controls.Add(_elementsGrid);
        _elementsTab.Location = new System.Drawing.Point(4, 22);
        _elementsTab.Name = "_elementsTab";
        _elementsTab.Size = new System.Drawing.Size(428, 516);
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
        _elementsGrid.Size = new System.Drawing.Size(428, 516);
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
        _rootSplit.Panel1.ResumeLayout(false);
        _rootSplit.Panel1.PerformLayout();
        _rootSplit.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_rootSplit).EndInit();
        _rootSplit.ResumeLayout(false);
        _leftToolbar.ResumeLayout(false);
        _leftToolbar.PerformLayout();
        _rightTabs.ResumeLayout(false);
        _previewTab.ResumeLayout(false);
        _previewHost.ResumeLayout(false);
        _previewHost.PerformLayout();
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
