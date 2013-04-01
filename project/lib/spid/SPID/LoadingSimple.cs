using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SPID {
    public partial class LoadingSimple : Form {
        public LoadingSimple() {
            InitializeComponent();
            //this.progressBar1.
        }

        public LoadingSimple(string labelText) : this(){
            this.label1.Text=labelText;
        }

        public void AppendLine(string line) {
            this.textBox1.AppendText(line+"\r\n");
        }
    }
}