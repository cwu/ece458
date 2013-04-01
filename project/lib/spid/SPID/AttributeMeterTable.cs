using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SPID {
    public partial class AttributeMeterTable : Form {
        public AttributeMeterTable(DataTable dt) {
            InitializeComponent();
            this.dataGridView1.DataSource=dt;
        }

        public AttributeMeterTable(double[,] matrix) {
            InitializeComponent();
            this.dataGridView1.DataSource=matrix;
        }
    }
}