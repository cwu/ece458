using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SPID {
    public partial class ConfigurationForm : Form {
        private Configuration config;

        public ConfigurationForm(Configuration config) {
            InitializeComponent();
            this.config=config;
            this.propertyGrid1.SelectedObject=config;
        }

        private void okButton_Click(object sender, EventArgs e) {
            config.Save();
            this.Close();
            MessageBox.Show("You need to restart the SPID application for the configuration change to take place");
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            config.Load();
            this.Close();
        }

        private void ConfigurationForm_Load(object sender, EventArgs e) {
            config.Load();
        }
    }
}