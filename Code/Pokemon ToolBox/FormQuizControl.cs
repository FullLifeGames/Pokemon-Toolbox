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
    public partial class FormQuizControl : Form
    {
        string[] a;

        bool b = true;

        public FormQuizControl(string[] s)
        {
            a = s;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (b)
            {
                a[0] = "text";
                next();
            }
            else
            {
                a[1] = "Pokemon";
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (b)
            {
                a[0] = "button";
                next();
            }
            else
            {
                a[1] = "Stats";
                this.Close();
            }
        }

        private void next()
        {
            b = false;
            label1.Text = "                  Mode?";
            button1.Text = "Pokemon";
            button2.Text = "Stats";
        }
    }
}
