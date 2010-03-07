using System;
using System.Windows.Forms;
using System.IO;

using CadastralViewer.Properties;

namespace CadastralViewer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void fileExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void fileOpenMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dial = new OpenFileDialog())
            {
                string lastFolder = Settings.Default.LastFolder;
                if (!String.IsNullOrEmpty(lastFolder) && Directory.Exists(lastFolder))
                    dial.InitialDirectory = lastFolder;

                dial.DefaultExt = "xml";
                dial.Filter = "Cadastral files (*.xml)|*.xml|All files (*.*)|*.*";

                if (dial.ShowDialog() == DialogResult.OK)
                {
                    OpenCadastralFile(dial.FileName);

                    Settings.Default.LastFolder = Path.GetDirectoryName(dial.FileName);
                    Settings.Default.Save();
                }
            }
        }

        void OpenCadastralFile(string fileName)
        {
            using (LoadForm dial = new LoadForm(fileName))
            {
                if (dial.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Loaded data ok");
                }
            }
        }
    }
}