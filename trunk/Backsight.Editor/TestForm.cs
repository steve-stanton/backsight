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
        List<TreeNode> m_Sel = new List<TreeNode>();

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

            TreeNode root = new TreeNode("Root");
            TreeNode[] rest = new TreeNode[4];
            rest[0] = new TreeNode("Steve");
            rest[1] = new TreeNode("Gosia");
            rest[2] = new TreeNode("Ola");
            rest[3] = new TreeNode("Zosia");

            //rest[0].BackColor = Color.Red;

            root.Nodes.AddRange(rest);
            treeView.Nodes.Add(root);
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

        private void treeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            TreeNode node = e.Node;

            if (m_Sel.Contains(node))
            //if (node.Text == "Ola" || node.Text == "Zosia")
            {
                //node.BackColor = Color.Orange;
                //e.DrawDefault = true;
                Rectangle r = e.Bounds;
                e.Graphics.FillRectangle(Brushes.Yellow, r);
                e.Graphics.DrawString(node.Text, treeView.Font, Brushes.Red, r.Left, r.Top);
            }
            else
                e.DrawDefault = true;
        }

        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {


        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeView.SelectedNode;
            if (node != null)
            {
                if (!m_Sel.Remove(node))
                    m_Sel.Add(node);

                treeView.SelectedNode = null;
            }
        }
    }
}