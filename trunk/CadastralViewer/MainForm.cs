using System;
using System.Windows.Forms;
using System.IO;

using CadastralViewer.Xml;
using CadastralViewer.Properties;
using System.Xml;
using System.Xml.Serialization;

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
            string data = File.ReadAllText(fileName);

            try
            {
                Content.Validate(data);
                LoadValidData(data);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void LoadValidData(string data)
        {
            //XmlDocument doc = new XmlDocument();
            //doc.Load(data);

            using (StringReader sr = new StringReader(data))
            {
                using (XmlReader xr = XmlReader.Create(sr))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(GeoSurveyPacketData));
                    GeoSurveyPacketData packet = (GeoSurveyPacketData)xs.Deserialize(xr);
                }
            }

            MessageBox.Show("ok");
        }
    }
}