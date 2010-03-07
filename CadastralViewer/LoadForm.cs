using System;
using System.Windows.Forms;
using System.IO;
using System.Xml.Schema;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;

namespace CadastralViewer
{
    /// <summary>
    /// Dialog for displaying information about an attempt to load
    /// a Cadastral XML file.
    /// </summary>
    public partial class LoadForm : Form
    {
        #region Class data

        /// <summary>
        /// The name of the Cadastral XML file 
        /// </summary>
        readonly string m_FileName;

        /// <summary>
        /// Problems detected during the load
        /// </summary>
        readonly List<string> m_Errors;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadForm"/> class.
        /// </summary>
        /// <param name="fileName">The name of the Cadastral XML file</param>
        public LoadForm(string fileName)
        {
            InitializeComponent();
            m_FileName = fileName;
            m_Errors = new List<string>();
        }

        #endregion

        private void LoadForm_Shown(object sender, EventArgs e)
        {
            // Load the cadastral schema
            XmlSchema schema = GetSchema();

            ShowMessage("Loading data");
            string data = File.ReadAllText(m_FileName);

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

            if (m_Errors.Count == 1)
                ShowMessage("1 problem detected");
            else
                ShowMessage(m_Errors.Count+" problems detected");
        }

        void ShowMessage(string msg)
        {
            listBox.Items.Add(msg);
            listBox.Refresh();
            Application.DoEvents();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            if (m_Errors.Count == 0)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;

            Close();
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
            {
                m_Errors.Add(e.Message);
                ShowMessage(e.Message);
            }
        }
    }
}