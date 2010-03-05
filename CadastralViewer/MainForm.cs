using System;
using System.Windows.Forms;
using System.IO;

using CadastralViewer.Xml;
using CadastralViewer.Properties;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace CadastralViewer
{
    public partial class MainForm : Form
    {
        List<string> m_Errors;

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
                //Content.Validate(data);
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

            XmlSchema schema = GetSchema();
            m_Errors = new List<string>();

            using (StringReader sr = new StringReader(data))
            {
                XmlReaderSettings xrs = new XmlReaderSettings();
                xrs.ConformanceLevel = ConformanceLevel.Document;
                xrs.ValidationType = ValidationType.Schema;
                xrs.Schemas.Add(schema);
                xrs.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);

                XmlReader reader = XmlReader.Create(sr, xrs);
                while (reader.Read()) { }

                //using (XmlReader xr = XmlReader.Create(sr))
                //{
                //    XmlSerializer xs = new XmlSerializer(typeof(GeoSurveyPacketData));
                //    GeoSurveyPacketData packet = (GeoSurveyPacketData)xs.Deserialize(xr);
                //}
            }

            if (m_Errors.Count == 0)
                MessageBox.Show("ok");
            else
            {
                MessageBox.Show("Error count="+m_Errors.Count);

                StringBuilder sb = new StringBuilder(1000);

                foreach (string s in m_Errors)
                {
                    sb.Append(s);
                    sb.Append(System.Environment.NewLine);

                    if (sb.Length > 1000)
                        break;
                }

                MessageBox.Show(sb.ToString());
            }
        }

        /// <summary>
        /// Obtains the XML schema for ArcGIS cadastral content
        /// </summary>
        /// <returns>The schema defined by <c>ArcCadastral.xsd</c></returns>
        /// <exception cref="XmlSchemaException">If the schema cannot be loaded from the assembly
        /// holding this class</exception>
        static XmlSchema GetSchema()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream fs = a.GetManifestResourceStream("CadastralViewer.Xml.ArcCadastral.xsd"))
            {
                return XmlSchema.Read(fs, null);
            }
        }

        void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (!m_Errors.Contains(e.Message))
                m_Errors.Add(e.Message);
        }
    }
}