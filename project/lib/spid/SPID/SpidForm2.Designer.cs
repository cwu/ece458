namespace SPID {
    partial class SpidForm2 {
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pcapFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trainingDataFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.protocolModelDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trainingDataFolderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.protocolModelDatabaseToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProtocolModelDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutSPIDAlgorithmProofofConceptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.classifiedSessionsGroupBox = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.sessionsListView = new System.Windows.Forms.ListView();
            this.clientIpColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.clientPortColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.serverIpColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.serverPortColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.startTimeColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.modelObservationsColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.sessionProtocolColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.sessionProtocolDetailsListView = new System.Windows.Forms.ListView();
            this.protocolModelColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.divergenceColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.matchPercentageColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.protocolModelsGroupBox = new System.Windows.Forms.GroupBox();
            this.protocolModelsListView = new System.Windows.Forms.ListView();
            this.protocolColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.sessionsColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.observationsColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.trainingDataFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.openPcapFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveProtocolModelDatabaseFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openProtocolModelDatabaseFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.classifiedSessionsGroupBox.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.protocolModelsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(632, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.appendToolStripMenuItem,
            this.saveProtocolModelDatabaseToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pcapFileToolStripMenuItem,
            this.trainingDataFolderToolStripMenuItem,
            this.protocolModelDatabaseToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // pcapFileToolStripMenuItem
            // 
            this.pcapFileToolStripMenuItem.Name = "pcapFileToolStripMenuItem";
            this.pcapFileToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.pcapFileToolStripMenuItem.Text = "Pcap File";
            this.pcapFileToolStripMenuItem.Click += new System.EventHandler(this.pcapFileToolStripMenuItem_Click);
            // 
            // trainingDataFolderToolStripMenuItem
            // 
            this.trainingDataFolderToolStripMenuItem.Name = "trainingDataFolderToolStripMenuItem";
            this.trainingDataFolderToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.trainingDataFolderToolStripMenuItem.Text = "Training Data Folder";
            this.trainingDataFolderToolStripMenuItem.Click += new System.EventHandler(this.trainingDataFolderToolStripMenuItem_Click);
            // 
            // protocolModelDatabaseToolStripMenuItem
            // 
            this.protocolModelDatabaseToolStripMenuItem.Name = "protocolModelDatabaseToolStripMenuItem";
            this.protocolModelDatabaseToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.protocolModelDatabaseToolStripMenuItem.Text = "Protocol Model Database";
            this.protocolModelDatabaseToolStripMenuItem.Click += new System.EventHandler(this.protocolModelDatabaseToolStripMenuItem_Click);
            // 
            // appendToolStripMenuItem
            // 
            this.appendToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trainingDataFolderToolStripMenuItem1,
            this.protocolModelDatabaseToolStripMenuItem1});
            this.appendToolStripMenuItem.Name = "appendToolStripMenuItem";
            this.appendToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.appendToolStripMenuItem.Text = "Append";
            // 
            // trainingDataFolderToolStripMenuItem1
            // 
            this.trainingDataFolderToolStripMenuItem1.Name = "trainingDataFolderToolStripMenuItem1";
            this.trainingDataFolderToolStripMenuItem1.Size = new System.Drawing.Size(204, 22);
            this.trainingDataFolderToolStripMenuItem1.Text = "Training Data Folder";
            this.trainingDataFolderToolStripMenuItem1.Click += new System.EventHandler(this.trainingDataFolderToolStripMenuItem1_Click);
            // 
            // protocolModelDatabaseToolStripMenuItem1
            // 
            this.protocolModelDatabaseToolStripMenuItem1.Name = "protocolModelDatabaseToolStripMenuItem1";
            this.protocolModelDatabaseToolStripMenuItem1.Size = new System.Drawing.Size(204, 22);
            this.protocolModelDatabaseToolStripMenuItem1.Text = "Protocol Model Database";
            this.protocolModelDatabaseToolStripMenuItem1.Click += new System.EventHandler(this.protocolModelDatabaseToolStripMenuItem1_Click);
            // 
            // saveProtocolModelDatabaseToolStripMenuItem
            // 
            this.saveProtocolModelDatabaseToolStripMenuItem.Name = "saveProtocolModelDatabaseToolStripMenuItem";
            this.saveProtocolModelDatabaseToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.saveProtocolModelDatabaseToolStripMenuItem.Text = "Save Protocol Model Database";
            this.saveProtocolModelDatabaseToolStripMenuItem.Click += new System.EventHandler(this.saveProtocolModelDatabaseToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.configurationToolStripMenuItem.Text = "Configuration";
            this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutSPIDAlgorithmProofofConceptToolStripMenuItem,
            this.licenseToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutSPIDAlgorithmProofofConceptToolStripMenuItem
            // 
            this.aboutSPIDAlgorithmProofofConceptToolStripMenuItem.Name = "aboutSPIDAlgorithmProofofConceptToolStripMenuItem";
            this.aboutSPIDAlgorithmProofofConceptToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.aboutSPIDAlgorithmProofofConceptToolStripMenuItem.Text = "About SPID Algorithm Proof-of-Concept";
            this.aboutSPIDAlgorithmProofofConceptToolStripMenuItem.Click += new System.EventHandler(this.aboutSPIDAlgorithmProofofConceptToolStripMenuItem_Click);
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            this.licenseToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.licenseToolStripMenuItem.Text = "License";
            this.licenseToolStripMenuItem.Click += new System.EventHandler(this.licenseToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 431);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(632, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.classifiedSessionsGroupBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.protocolModelsGroupBox);
            this.splitContainer1.Size = new System.Drawing.Size(632, 407);
            this.splitContainer1.SplitterDistance = 434;
            this.splitContainer1.TabIndex = 2;
            // 
            // classifiedSessionsGroupBox
            // 
            this.classifiedSessionsGroupBox.Controls.Add(this.splitContainer2);
            this.classifiedSessionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.classifiedSessionsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.classifiedSessionsGroupBox.Name = "classifiedSessionsGroupBox";
            this.classifiedSessionsGroupBox.Size = new System.Drawing.Size(434, 407);
            this.classifiedSessionsGroupBox.TabIndex = 1;
            this.classifiedSessionsGroupBox.TabStop = false;
            this.classifiedSessionsGroupBox.Text = "Classified Sessions (200 first displayed)";
            this.classifiedSessionsGroupBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.classifiedSessionsGroupBox_DragDrop);
            this.classifiedSessionsGroupBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.classifiedSessionsGroupBox_DragEnter);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(3, 16);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.sessionsListView);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.sessionProtocolDetailsListView);
            this.splitContainer2.Size = new System.Drawing.Size(428, 388);
            this.splitContainer2.SplitterDistance = 198;
            this.splitContainer2.TabIndex = 0;
            // 
            // sessionsListView
            // 
            this.sessionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clientIpColumnHeader,
            this.clientPortColumnHeader,
            this.serverIpColumnHeader,
            this.serverPortColumnHeader,
            this.startTimeColumnHeader,
            this.modelObservationsColumnHeader,
            this.sessionProtocolColumnHeader});
            this.sessionsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionsListView.FullRowSelect = true;
            this.sessionsListView.Location = new System.Drawing.Point(0, 0);
            this.sessionsListView.MultiSelect = false;
            this.sessionsListView.Name = "sessionsListView";
            this.sessionsListView.Size = new System.Drawing.Size(428, 198);
            this.sessionsListView.TabIndex = 0;
            this.sessionsListView.UseCompatibleStateImageBehavior = false;
            this.sessionsListView.View = System.Windows.Forms.View.Details;
            this.sessionsListView.SelectedIndexChanged += new System.EventHandler(this.sessionsListView_SelectedIndexChanged);
            // 
            // clientIpColumnHeader
            // 
            this.clientIpColumnHeader.Text = "Client IP";
            // 
            // clientPortColumnHeader
            // 
            this.clientPortColumnHeader.Text = "C. Port";
            // 
            // serverIpColumnHeader
            // 
            this.serverIpColumnHeader.Text = "Server IP";
            // 
            // serverPortColumnHeader
            // 
            this.serverPortColumnHeader.Text = "S. Port";
            // 
            // startTimeColumnHeader
            // 
            this.startTimeColumnHeader.Text = "Start Time";
            // 
            // modelObservationsColumnHeader
            // 
            this.modelObservationsColumnHeader.Text = "Observations";
            // 
            // sessionProtocolColumnHeader
            // 
            this.sessionProtocolColumnHeader.Text = "Protocol";
            // 
            // sessionProtocolDetailsListView
            // 
            this.sessionProtocolDetailsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.protocolModelColumnHeader,
            this.divergenceColumnHeader,
            this.matchPercentageColumnHeader});
            this.sessionProtocolDetailsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionProtocolDetailsListView.Location = new System.Drawing.Point(0, 0);
            this.sessionProtocolDetailsListView.Name = "sessionProtocolDetailsListView";
            this.sessionProtocolDetailsListView.Size = new System.Drawing.Size(428, 186);
            this.sessionProtocolDetailsListView.TabIndex = 0;
            this.sessionProtocolDetailsListView.UseCompatibleStateImageBehavior = false;
            this.sessionProtocolDetailsListView.View = System.Windows.Forms.View.Details;
            // 
            // protocolModelColumnHeader
            // 
            this.protocolModelColumnHeader.Text = "Protocol Model";
            this.protocolModelColumnHeader.Width = 100;
            // 
            // divergenceColumnHeader
            // 
            this.divergenceColumnHeader.Text = "Divergence (avg)";
            this.divergenceColumnHeader.Width = 100;
            // 
            // matchPercentageColumnHeader
            // 
            this.matchPercentageColumnHeader.Text = "Match Percentage";
            this.matchPercentageColumnHeader.Width = 220;
            // 
            // protocolModelsGroupBox
            // 
            this.protocolModelsGroupBox.Controls.Add(this.protocolModelsListView);
            this.protocolModelsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.protocolModelsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.protocolModelsGroupBox.Name = "protocolModelsGroupBox";
            this.protocolModelsGroupBox.Size = new System.Drawing.Size(194, 407);
            this.protocolModelsGroupBox.TabIndex = 1;
            this.protocolModelsGroupBox.TabStop = false;
            this.protocolModelsGroupBox.Text = "Protocol Models";
            // 
            // protocolModelsListView
            // 
            this.protocolModelsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.protocolColumnHeader,
            this.sessionsColumnHeader,
            this.observationsColumnHeader});
            this.protocolModelsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.protocolModelsListView.Location = new System.Drawing.Point(3, 16);
            this.protocolModelsListView.Name = "protocolModelsListView";
            this.protocolModelsListView.Size = new System.Drawing.Size(188, 388);
            this.protocolModelsListView.TabIndex = 0;
            this.protocolModelsListView.UseCompatibleStateImageBehavior = false;
            this.protocolModelsListView.View = System.Windows.Forms.View.Details;
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
            // openPcapFileDialog
            // 
            this.openPcapFileDialog.DefaultExt = "*.pcap";
            this.openPcapFileDialog.Filter = "Pcap files (*.pcap, *.cap, *.dump)|*.pcap;*.cap;*.dump|All files (*.*)|*.*";
            // 
            // saveProtocolModelDatabaseFileDialog
            // 
            this.saveProtocolModelDatabaseFileDialog.Filter = "Protocol Model Databases (*.xml)|*.xml";
            // 
            // openProtocolModelDatabaseFileDialog
            // 
            this.openProtocolModelDatabaseFileDialog.Filter = "Protocol Model Databases (*.xml)|*.xml";
            // 
            // SpidForm2
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpidForm2";
            this.Text = "SPID Algorithm Proof-of-Concept 0.4.6";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.classifiedSessionsGroupBox_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.classifiedSessionsGroupBox_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.classifiedSessionsGroupBox.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.protocolModelsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trainingDataFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem protocolModelDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pcapFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProtocolModelDatabaseToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView protocolModelsListView;
        private System.Windows.Forms.ListView sessionsListView;
        private System.Windows.Forms.ColumnHeader protocolColumnHeader;
        private System.Windows.Forms.ColumnHeader sessionsColumnHeader;
        private System.Windows.Forms.ColumnHeader observationsColumnHeader;
        private System.Windows.Forms.GroupBox protocolModelsGroupBox;
        private System.Windows.Forms.GroupBox classifiedSessionsGroupBox;
        private System.Windows.Forms.ColumnHeader clientIpColumnHeader;
        private System.Windows.Forms.ColumnHeader clientPortColumnHeader;
        private System.Windows.Forms.ColumnHeader serverIpColumnHeader;
        private System.Windows.Forms.ColumnHeader serverPortColumnHeader;
        private System.Windows.Forms.ColumnHeader sessionProtocolColumnHeader;
        private System.Windows.Forms.FolderBrowserDialog trainingDataFolderBrowserDialog;
        private System.Windows.Forms.ColumnHeader startTimeColumnHeader;
        private System.Windows.Forms.OpenFileDialog openPcapFileDialog;
        private System.Windows.Forms.ColumnHeader modelObservationsColumnHeader;
        private System.Windows.Forms.ListView sessionProtocolDetailsListView;
        private System.Windows.Forms.ColumnHeader protocolModelColumnHeader;
        private System.Windows.Forms.ColumnHeader divergenceColumnHeader;
        private System.Windows.Forms.ColumnHeader matchPercentageColumnHeader;
        private System.Windows.Forms.SaveFileDialog saveProtocolModelDatabaseFileDialog;
        private System.Windows.Forms.OpenFileDialog openProtocolModelDatabaseFileDialog;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutSPIDAlgorithmProofofConceptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem protocolModelDatabaseToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem trainingDataFolderToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem licenseToolStripMenuItem;
    }
}