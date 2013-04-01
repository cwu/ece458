namespace SPID {
    partial class SpidForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.addToModelsGroupBox = new System.Windows.Forms.GroupBox();
            this.addSessionToProtocolModelsButton = new System.Windows.Forms.Button();
            this.existingProtocolComboBox = new System.Windows.Forms.ComboBox();
            this.newProtocolTextBox = new System.Windows.Forms.TextBox();
            this.newProtocolRadioButton = new System.Windows.Forms.RadioButton();
            this.existingProtocolRadioButton = new System.Windows.Forms.RadioButton();
            this.protocolIdentificationListView = new System.Windows.Forms.ListView();
            this.identificationProtocolColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.identificationDivergenceColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.identificationMatchStrengthColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importProtocolModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openPcapFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveProtocolModelDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.protocolIdentificationLabel = new System.Windows.Forms.Label();
            this.protocolModelPropertiesGroupBox = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.updateProtocolModelButton = new System.Windows.Forms.Button();
            this.defaultPortsListBox = new System.Windows.Forms.ListBox();
            this.defaultPortsListContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeSelectedPortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.addNewPortButton = new System.Windows.Forms.Button();
            this.newPortNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.protocolNameTextBox = new System.Windows.Forms.TextBox();
            this.resetAllProtocolModelFingerprintsButton = new System.Windows.Forms.Button();
            this.protocolModelsListView = new System.Windows.Forms.ListView();
            this.protocolColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.sessionsColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.observationsColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.protocolModelsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeProtocolModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.protocolModelLabel = new System.Windows.Forms.Label();
            this.openPcapFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.openProtocolModelDatabase = new System.Windows.Forms.OpenFileDialog();
            this.saveProtocolModelDatabaseDialog = new System.Windows.Forms.SaveFileDialog();
            this.addToModelsGroupBox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.protocolModelPropertiesGroupBox.SuspendLayout();
            this.defaultPortsListContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newPortNumericUpDown)).BeginInit();
            this.protocolModelsContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // addToModelsGroupBox
            // 
            this.addToModelsGroupBox.Controls.Add(this.addSessionToProtocolModelsButton);
            this.addToModelsGroupBox.Controls.Add(this.existingProtocolComboBox);
            this.addToModelsGroupBox.Controls.Add(this.newProtocolTextBox);
            this.addToModelsGroupBox.Controls.Add(this.newProtocolRadioButton);
            this.addToModelsGroupBox.Controls.Add(this.existingProtocolRadioButton);
            this.addToModelsGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.addToModelsGroupBox.Enabled = false;
            this.addToModelsGroupBox.Location = new System.Drawing.Point(0, 335);
            this.addToModelsGroupBox.Name = "addToModelsGroupBox";
            this.addToModelsGroupBox.Size = new System.Drawing.Size(232, 72);
            this.addToModelsGroupBox.TabIndex = 3;
            this.addToModelsGroupBox.TabStop = false;
            this.addToModelsGroupBox.Text = "Add Session to Protocol Models";
            // 
            // addSessionToProtocolModelsButton
            // 
            this.addSessionToProtocolModelsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addSessionToProtocolModelsButton.Enabled = false;
            this.addSessionToProtocolModelsButton.Location = new System.Drawing.Point(182, 19);
            this.addSessionToProtocolModelsButton.Name = "addSessionToProtocolModelsButton";
            this.addSessionToProtocolModelsButton.Size = new System.Drawing.Size(44, 47);
            this.addSessionToProtocolModelsButton.TabIndex = 4;
            this.addSessionToProtocolModelsButton.Text = "Add";
            this.addSessionToProtocolModelsButton.UseVisualStyleBackColor = true;
            this.addSessionToProtocolModelsButton.Click += new System.EventHandler(this.addSessionToProtocolModelsButton_Click);
            // 
            // existingProtocolComboBox
            // 
            this.existingProtocolComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.existingProtocolComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.existingProtocolComboBox.FormattingEnabled = true;
            this.existingProtocolComboBox.Location = new System.Drawing.Point(73, 19);
            this.existingProtocolComboBox.Name = "existingProtocolComboBox";
            this.existingProtocolComboBox.Size = new System.Drawing.Size(103, 21);
            this.existingProtocolComboBox.TabIndex = 3;
            this.existingProtocolComboBox.SelectedIndexChanged += new System.EventHandler(this.existingProtocolComboBox_SelectedIndexChanged);
            // 
            // newProtocolTextBox
            // 
            this.newProtocolTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.newProtocolTextBox.Location = new System.Drawing.Point(73, 46);
            this.newProtocolTextBox.Name = "newProtocolTextBox";
            this.newProtocolTextBox.Size = new System.Drawing.Size(103, 20);
            this.newProtocolTextBox.TabIndex = 2;
            this.newProtocolTextBox.Text = "[new protocol name]";
            this.newProtocolTextBox.TextChanged += new System.EventHandler(this.newProtocolTextBox_TextChanged);
            this.newProtocolTextBox.Enter += new System.EventHandler(this.newProtocolTextBox_Enter);
            // 
            // newProtocolRadioButton
            // 
            this.newProtocolRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newProtocolRadioButton.AutoSize = true;
            this.newProtocolRadioButton.Location = new System.Drawing.Point(6, 46);
            this.newProtocolRadioButton.Name = "newProtocolRadioButton";
            this.newProtocolRadioButton.Size = new System.Drawing.Size(47, 17);
            this.newProtocolRadioButton.TabIndex = 1;
            this.newProtocolRadioButton.TabStop = true;
            this.newProtocolRadioButton.Text = "New";
            this.newProtocolRadioButton.UseVisualStyleBackColor = true;
            this.newProtocolRadioButton.CheckedChanged += new System.EventHandler(this.newProtocolRadioButton_CheckedChanged);
            // 
            // existingProtocolRadioButton
            // 
            this.existingProtocolRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.existingProtocolRadioButton.AutoSize = true;
            this.existingProtocolRadioButton.Location = new System.Drawing.Point(6, 20);
            this.existingProtocolRadioButton.Name = "existingProtocolRadioButton";
            this.existingProtocolRadioButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.existingProtocolRadioButton.Size = new System.Drawing.Size(61, 17);
            this.existingProtocolRadioButton.TabIndex = 0;
            this.existingProtocolRadioButton.TabStop = true;
            this.existingProtocolRadioButton.Text = "Existing";
            this.existingProtocolRadioButton.UseVisualStyleBackColor = true;
            this.existingProtocolRadioButton.CheckedChanged += new System.EventHandler(this.existingProtocolRadioButton_CheckedChanged);
            // 
            // protocolIdentificationListView
            // 
            this.protocolIdentificationListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.protocolIdentificationListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.identificationProtocolColumnHeader,
            this.identificationDivergenceColumnHeader,
            this.identificationMatchStrengthColumnHeader});
            this.protocolIdentificationListView.Location = new System.Drawing.Point(3, 16);
            this.protocolIdentificationListView.Name = "protocolIdentificationListView";
            this.protocolIdentificationListView.Size = new System.Drawing.Size(226, 313);
            this.protocolIdentificationListView.TabIndex = 0;
            this.protocolIdentificationListView.UseCompatibleStateImageBehavior = false;
            this.protocolIdentificationListView.View = System.Windows.Forms.View.Details;
            // 
            // identificationProtocolColumnHeader
            // 
            this.identificationProtocolColumnHeader.Text = "Protocol";
            // 
            // identificationDivergenceColumnHeader
            // 
            this.identificationDivergenceColumnHeader.Text = "Divergence";
            // 
            // identificationMatchStrengthColumnHeader
            // 
            this.identificationMatchStrengthColumnHeader.Text = "Match Strength";
            this.identificationMatchStrengthColumnHeader.Width = 73;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 431);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(632, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(632, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importProtocolModelToolStripMenuItem,
            this.openPcapFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveProtocolModelDatabaseToolStripMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileMenuItem.Text = "File";
            // 
            // importProtocolModelToolStripMenuItem
            // 
            this.importProtocolModelToolStripMenuItem.Name = "importProtocolModelToolStripMenuItem";
            this.importProtocolModelToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.importProtocolModelToolStripMenuItem.Text = "Import Protocol Model Database";
            this.importProtocolModelToolStripMenuItem.Click += new System.EventHandler(this.importProtocolModelToolStripMenuItem_Click);
            // 
            // openPcapFileToolStripMenuItem
            // 
            this.openPcapFileToolStripMenuItem.Name = "openPcapFileToolStripMenuItem";
            this.openPcapFileToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.openPcapFileToolStripMenuItem.Text = "Open Pcap File";
            this.openPcapFileToolStripMenuItem.Click += new System.EventHandler(this.openPcapFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(236, 6);
            // 
            // saveProtocolModelDatabaseToolStripMenuItem
            // 
            this.saveProtocolModelDatabaseToolStripMenuItem.Name = "saveProtocolModelDatabaseToolStripMenuItem";
            this.saveProtocolModelDatabaseToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.saveProtocolModelDatabaseToolStripMenuItem.Text = "Save Protocol Model Database";
            this.saveProtocolModelDatabaseToolStripMenuItem.Click += new System.EventHandler(this.saveProtocolModelDatabaseToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.addToModelsGroupBox);
            this.splitContainer1.Panel1.Controls.Add(this.protocolIdentificationLabel);
            this.splitContainer1.Panel1.Controls.Add(this.protocolIdentificationListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.protocolModelPropertiesGroupBox);
            this.splitContainer1.Panel2.Controls.Add(this.resetAllProtocolModelFingerprintsButton);
            this.splitContainer1.Panel2.Controls.Add(this.protocolModelsListView);
            this.splitContainer1.Panel2.Controls.Add(this.protocolModelLabel);
            this.splitContainer1.Size = new System.Drawing.Size(632, 407);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.SplitterWidth = 7;
            this.splitContainer1.TabIndex = 6;
            // 
            // protocolIdentificationLabel
            // 
            this.protocolIdentificationLabel.AutoSize = true;
            this.protocolIdentificationLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.protocolIdentificationLabel.Location = new System.Drawing.Point(0, 0);
            this.protocolIdentificationLabel.Name = "protocolIdentificationLabel";
            this.protocolIdentificationLabel.Size = new System.Drawing.Size(151, 13);
            this.protocolIdentificationLabel.TabIndex = 4;
            this.protocolIdentificationLabel.Text = "Session Protocol IDentification";
            // 
            // protocolModelPropertiesGroupBox
            // 
            this.protocolModelPropertiesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.protocolModelPropertiesGroupBox.Controls.Add(this.label3);
            this.protocolModelPropertiesGroupBox.Controls.Add(this.label2);
            this.protocolModelPropertiesGroupBox.Controls.Add(this.updateProtocolModelButton);
            this.protocolModelPropertiesGroupBox.Controls.Add(this.defaultPortsListBox);
            this.protocolModelPropertiesGroupBox.Controls.Add(this.label1);
            this.protocolModelPropertiesGroupBox.Controls.Add(this.addNewPortButton);
            this.protocolModelPropertiesGroupBox.Controls.Add(this.newPortNumericUpDown);
            this.protocolModelPropertiesGroupBox.Controls.Add(this.protocolNameTextBox);
            this.protocolModelPropertiesGroupBox.Enabled = false;
            this.protocolModelPropertiesGroupBox.Location = new System.Drawing.Point(3, 255);
            this.protocolModelPropertiesGroupBox.Name = "protocolModelPropertiesGroupBox";
            this.protocolModelPropertiesGroupBox.Size = new System.Drawing.Size(387, 149);
            this.protocolModelPropertiesGroupBox.TabIndex = 5;
            this.protocolModelPropertiesGroupBox.TabStop = false;
            this.protocolModelPropertiesGroupBox.Text = "Selected Protocol Model Properties";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Default port(s)";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Protocol name";
            // 
            // updateProtocolModelButton
            // 
            this.updateProtocolModelButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.updateProtocolModelButton.Location = new System.Drawing.Point(87, 120);
            this.updateProtocolModelButton.Name = "updateProtocolModelButton";
            this.updateProtocolModelButton.Size = new System.Drawing.Size(291, 23);
            this.updateProtocolModelButton.TabIndex = 10;
            this.updateProtocolModelButton.Text = "Update Protocol Model";
            this.updateProtocolModelButton.UseVisualStyleBackColor = true;
            this.updateProtocolModelButton.Click += new System.EventHandler(this.updateProtocolModelButton_Click);
            // 
            // defaultPortsListBox
            // 
            this.defaultPortsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.defaultPortsListBox.ContextMenuStrip = this.defaultPortsListContextMenuStrip;
            this.defaultPortsListBox.FormattingEnabled = true;
            this.defaultPortsListBox.Location = new System.Drawing.Point(87, 45);
            this.defaultPortsListBox.Name = "defaultPortsListBox";
            this.defaultPortsListBox.Size = new System.Drawing.Size(59, 69);
            this.defaultPortsListBox.Sorted = true;
            this.defaultPortsListBox.TabIndex = 9;
            // 
            // defaultPortsListContextMenuStrip
            // 
            this.defaultPortsListContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeSelectedPortToolStripMenuItem});
            this.defaultPortsListContextMenuStrip.Name = "defaultPortsListContextMenuStrip";
            this.defaultPortsListContextMenuStrip.Size = new System.Drawing.Size(191, 26);
            // 
            // removeSelectedPortToolStripMenuItem
            // 
            this.removeSelectedPortToolStripMenuItem.Name = "removeSelectedPortToolStripMenuItem";
            this.removeSelectedPortToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.removeSelectedPortToolStripMenuItem.Text = "Remove selected port";
            this.removeSelectedPortToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedPortToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(152, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Add new port";
            // 
            // addNewPortButton
            // 
            this.addNewPortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addNewPortButton.Location = new System.Drawing.Point(151, 91);
            this.addNewPortButton.Name = "addNewPortButton";
            this.addNewPortButton.Size = new System.Drawing.Size(71, 23);
            this.addNewPortButton.TabIndex = 7;
            this.addNewPortButton.Text = "<< Add Port";
            this.addNewPortButton.UseVisualStyleBackColor = true;
            this.addNewPortButton.Click += new System.EventHandler(this.addNewPortButton_Click);
            // 
            // newPortNumericUpDown
            // 
            this.newPortNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newPortNumericUpDown.Location = new System.Drawing.Point(152, 65);
            this.newPortNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.newPortNumericUpDown.Name = "newPortNumericUpDown";
            this.newPortNumericUpDown.Size = new System.Drawing.Size(71, 20);
            this.newPortNumericUpDown.TabIndex = 5;
            // 
            // protocolNameTextBox
            // 
            this.protocolNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.protocolNameTextBox.Location = new System.Drawing.Point(87, 19);
            this.protocolNameTextBox.Name = "protocolNameTextBox";
            this.protocolNameTextBox.Size = new System.Drawing.Size(291, 20);
            this.protocolNameTextBox.TabIndex = 1;
            // 
            // resetAllProtocolModelFingerprintsButton
            // 
            this.resetAllProtocolModelFingerprintsButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resetAllProtocolModelFingerprintsButton.Location = new System.Drawing.Point(3, 226);
            this.resetAllProtocolModelFingerprintsButton.Name = "resetAllProtocolModelFingerprintsButton";
            this.resetAllProtocolModelFingerprintsButton.Size = new System.Drawing.Size(387, 23);
            this.resetAllProtocolModelFingerprintsButton.TabIndex = 4;
            this.resetAllProtocolModelFingerprintsButton.Text = "Reset All Protocol Model Fingerprints";
            this.resetAllProtocolModelFingerprintsButton.UseVisualStyleBackColor = true;
            this.resetAllProtocolModelFingerprintsButton.Click += new System.EventHandler(this.resetAllProtocolModelFingerprintsButton_Click);
            // 
            // protocolModelsListView
            // 
            this.protocolModelsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.protocolModelsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.protocolColumnHeader,
            this.sessionsColumnHeader,
            this.observationsColumnHeader});
            this.protocolModelsListView.ContextMenuStrip = this.protocolModelsContextMenuStrip;
            this.protocolModelsListView.FullRowSelect = true;
            this.protocolModelsListView.HideSelection = false;
            this.protocolModelsListView.Location = new System.Drawing.Point(3, 16);
            this.protocolModelsListView.MultiSelect = false;
            this.protocolModelsListView.Name = "protocolModelsListView";
            this.protocolModelsListView.Size = new System.Drawing.Size(387, 204);
            this.protocolModelsListView.TabIndex = 3;
            this.protocolModelsListView.UseCompatibleStateImageBehavior = false;
            this.protocolModelsListView.View = System.Windows.Forms.View.Details;
            this.protocolModelsListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.protocolModelsListView_ItemSelectionChanged);
            // 
            // protocolColumnHeader
            // 
            this.protocolColumnHeader.Text = "Protocol";
            // 
            // sessionsColumnHeader
            // 
            this.sessionsColumnHeader.Text = "Sessions";
            // 
            // observationsColumnHeader
            // 
            this.observationsColumnHeader.Text = "Observations";
            // 
            // protocolModelsContextMenuStrip
            // 
            this.protocolModelsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeProtocolModelToolStripMenuItem});
            this.protocolModelsContextMenuStrip.Name = "protocolModelsContextMenuStrip";
            this.protocolModelsContextMenuStrip.Size = new System.Drawing.Size(198, 26);
            // 
            // removeProtocolModelToolStripMenuItem
            // 
            this.removeProtocolModelToolStripMenuItem.Name = "removeProtocolModelToolStripMenuItem";
            this.removeProtocolModelToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.removeProtocolModelToolStripMenuItem.Text = "Remove protocol model";
            this.removeProtocolModelToolStripMenuItem.Click += new System.EventHandler(this.removeProtocolModelToolStripMenuItem_Click);
            // 
            // protocolModelLabel
            // 
            this.protocolModelLabel.AutoSize = true;
            this.protocolModelLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.protocolModelLabel.Location = new System.Drawing.Point(0, 0);
            this.protocolModelLabel.Name = "protocolModelLabel";
            this.protocolModelLabel.Size = new System.Drawing.Size(83, 13);
            this.protocolModelLabel.TabIndex = 2;
            this.protocolModelLabel.Text = "Protocol Models";
            // 
            // openPcapFileDialog
            // 
            this.openPcapFileDialog.DefaultExt = "*.pcap";
            this.openPcapFileDialog.Filter = "Pcap files (*.pcap, *.cap, *.dump)|*.pcap;*.cap;*.dump|All files (*.*)|*.*";
            // 
            // openProtocolModelDatabase
            // 
            this.openProtocolModelDatabase.Filter = "Protocol Model Databases (*.xml)|*.xml";
            // 
            // saveProtocolModelDatabaseDialog
            // 
            this.saveProtocolModelDatabaseDialog.Filter = "Protocol Model Databases (*.xml)|*.xml";
            // 
            // SpidForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpidForm";
            this.Text = "SPID Algorithm Proof-of-Concept 0.1";
            this.addToModelsGroupBox.ResumeLayout(false);
            this.addToModelsGroupBox.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.protocolModelPropertiesGroupBox.ResumeLayout(false);
            this.protocolModelPropertiesGroupBox.PerformLayout();
            this.defaultPortsListContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.newPortNumericUpDown)).EndInit();
            this.protocolModelsContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox addToModelsGroupBox;
        private System.Windows.Forms.ListView protocolIdentificationListView;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importProtocolModelToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label protocolModelLabel;
        private System.Windows.Forms.Label protocolIdentificationLabel;
        private System.Windows.Forms.RadioButton newProtocolRadioButton;
        private System.Windows.Forms.RadioButton existingProtocolRadioButton;
        private System.Windows.Forms.ComboBox existingProtocolComboBox;
        private System.Windows.Forms.TextBox newProtocolTextBox;
        private System.Windows.Forms.Button addSessionToProtocolModelsButton;
        private System.Windows.Forms.ToolStripMenuItem openPcapFileToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openPcapFileDialog;
        private System.Windows.Forms.OpenFileDialog openProtocolModelDatabase;
        private System.Windows.Forms.ListView protocolModelsListView;
        private System.Windows.Forms.ColumnHeader protocolColumnHeader;
        private System.Windows.Forms.ColumnHeader sessionsColumnHeader;
        private System.Windows.Forms.ColumnHeader observationsColumnHeader;
        private System.Windows.Forms.ColumnHeader identificationProtocolColumnHeader;
        private System.Windows.Forms.ColumnHeader identificationDivergenceColumnHeader;
        private System.Windows.Forms.ColumnHeader identificationMatchStrengthColumnHeader;
        private System.Windows.Forms.Button resetAllProtocolModelFingerprintsButton;
        private System.Windows.Forms.GroupBox protocolModelPropertiesGroupBox;
        private System.Windows.Forms.TextBox protocolNameTextBox;
        private System.Windows.Forms.NumericUpDown newPortNumericUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button addNewPortButton;
        private System.Windows.Forms.ListBox defaultPortsListBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button updateProtocolModelButton;
        private System.Windows.Forms.ContextMenuStrip defaultPortsListContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedPortToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip protocolModelsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem removeProtocolModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveProtocolModelDatabaseToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveProtocolModelDatabaseDialog;
    }
}

