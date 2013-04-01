namespace SPID {
    partial class LoadingProcess {
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
            this.pcapFileProgressBar = new System.Windows.Forms.ProgressBar();
            this.frameBufferProgressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.completedProtocolModelsPercentProgressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pcapFileProgressBar
            // 
            this.pcapFileProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pcapFileProgressBar.Location = new System.Drawing.Point(3, 16);
            this.pcapFileProgressBar.Name = "pcapFileProgressBar";
            this.pcapFileProgressBar.Size = new System.Drawing.Size(286, 21);
            this.pcapFileProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pcapFileProgressBar.TabIndex = 0;
            // 
            // frameBufferProgressBar
            // 
            this.frameBufferProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frameBufferProgressBar.Location = new System.Drawing.Point(3, 16);
            this.frameBufferProgressBar.Name = "frameBufferProgressBar";
            this.frameBufferProgressBar.Size = new System.Drawing.Size(286, 21);
            this.frameBufferProgressBar.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pcapFileProgressBar);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(292, 40);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File read progress";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.frameBufferProgressBar);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 40);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(292, 40);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Frames to add to sessions";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.completedProtocolModelsPercentProgressBar);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 80);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(292, 40);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Protocol models to display";
            // 
            // completedProtocolModelsPercentProgressBar
            // 
            this.completedProtocolModelsPercentProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.completedProtocolModelsPercentProgressBar.Location = new System.Drawing.Point(3, 16);
            this.completedProtocolModelsPercentProgressBar.Name = "completedProtocolModelsPercentProgressBar";
            this.completedProtocolModelsPercentProgressBar.Size = new System.Drawing.Size(286, 21);
            this.completedProtocolModelsPercentProgressBar.TabIndex = 0;
            // 
            // LoadingProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 122);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadingProcess";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "LoadingProcess";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoadingProcess_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pcapFileProgressBar;
        private System.Windows.Forms.ProgressBar frameBufferProgressBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ProgressBar completedProtocolModelsPercentProgressBar;
    }
}