using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Backsight.Editor
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void testButton1_Click(object sender, EventArgs e)
        {
            testButton1.Enabled = !testButton1.Enabled;
        }

        private void testButton2_Click(object sender, EventArgs e)
        {
            testButton2.Enabled = !testButton2.Enabled;
        }

        private void TestForm_Shown(object sender, EventArgs e)
        {
            Application.Idle +=new EventHandler(Application_Idle);
        }

        private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Idle -= Application_Idle;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            testButton1.Invalidate();
            testButton2.Invalidate();
        }
    }
}