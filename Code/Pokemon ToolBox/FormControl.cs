using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pokemon_ToolBox
{
    public partial class FormControl : Form
    {
        string[] control;

        public FormControl(string[] s)
        {
            control = s;
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            control[0] = "Damagecalc";
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            control[0] = "Quiz";
            this.Close();
        }
    }
}
