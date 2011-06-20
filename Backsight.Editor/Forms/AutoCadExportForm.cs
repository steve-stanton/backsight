using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Backsight.Editor.Properties;
using System.IO;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for exporting to AutoCad (DXF)
    /// </summary>
    /// <was>CdAcadExport</was>
    public partial class AutoCadExportForm : Form
    {
        #region Class data

        /// <summary>
        /// The file spec for the AutoCad file.
        /// </summary>
        string m_AcadSpec;

        /// <summary>
        /// The AutoCad version to create.
        /// </summary>
        string m_Version;

        /// <summary>
        /// Entity translation file to use (blank if none).
        /// </summary>
        string m_EntFile;

        /// <summary>
        /// Exporting just topological stuff?
        /// </summary>
        bool m_IsTopology;

        /// <summary>
        /// Tolerance for approximating arcs (a value of 0.0 means that arcs should
        /// NOT be approximated).
        /// </summary>
        double m_ArcTol;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCadExportForm"/> class.
        /// </summary>
        public AutoCadExportForm()
        {
            InitializeComponent();

            // Check for the last entity translation file that was used.
            // If we got something, but the file no longer exists, blank
            // out the name we got and remove it from the settings.

            m_EntFile = Settings.Default.EntityTranslation;
            if (!String.IsNullOrEmpty(m_EntFile) && !File.Exists(m_EntFile))
            {
                m_EntFile = String.Empty;
                Settings.Default.EntityTranslation = String.Empty;
                Settings.Default.Save();
            }

            m_Version = Settings.Default.AutoCadVersion;

            // Remember the AutoCad file spec
            string jobSpec = EditingController.Current.JobFile.Name;
            m_AcadSpec = Path.ChangeExtension(jobSpec, ".dxf");

            // Assume topological export.
            m_IsTopology = true;

            // Assume that arcs will be approximated.
            m_ArcTol = 0.001;
        }

        #endregion

        private void AutoCadExportForm_Load(object sender, EventArgs e)
        {

        }

        private void browseButton_Click(object sender, EventArgs e)
        {

        }
    }
}
