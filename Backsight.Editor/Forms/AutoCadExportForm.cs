using System;
using System.IO;
using System.Windows.Forms;

using Backsight.Editor.Properties;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for obtaining parameters for an export to AutoCad (DXF)
    /// </summary>
    /// <was>CdAcadExport</was>
    public partial class AutoCadExportForm : Form
    {
        #region Class data

        /// <summary>
        /// The object that may be used to do the writing (null if the user
        /// decided to cancel).
        /// </summary>
        DxfWriter m_Writer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCadExportForm"/> class.
        /// </summary>
        public AutoCadExportForm()
        {
            InitializeComponent();
        }

        #endregion

        private void AutoCadExportForm_Load(object sender, EventArgs e)
        {
            // Check for the last entity translation file that was used.
            // If we got something, but the file no longer exists, blank
            // out the name we got and remove it from the settings.

            string entFile = Settings.Default.EntityTranslation;
            if (!String.IsNullOrEmpty(entFile) && !File.Exists(entFile))
            {
                Settings.Default.EntityTranslation = String.Empty;
                Settings.Default.Save();
            }
            entFileTextBox.Text = entFile;

            // Select the default export format from the version combo (the
            // values in it are defined as part of the resource).
            versionComboBox.SelectedText = Settings.Default.AutoCadVersion;

            // Display the name of the currently active layer.
            themeLabel.Text = EditingController.Current.ActiveLayer.Name;

            // Set the check mark to indicate that arcs should be
            // approximated as polylines.
            arcPolylineCheckBox.Checked = true;

            // Assume we want to export polygon data
            polDataRadioButton.Checked = true;
            othDataRadioButton.Checked = false;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dial = new OpenFileDialog())
            {
                dial.Title = "Pick Entity Translation File";
                dial.Filter = "Entity translation files (*.ent)|*.ent|All Files (*.*)|*.*";
                dial.DefaultExt = "ent";

                if (dial.ShowDialog(this) == DialogResult.OK)
                    entFileTextBox.Text = dial.FileName;
            }

            okButton.Focus();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string entFile = entFileTextBox.Text;
            if (!String.IsNullOrEmpty(entFile) && !File.Exists(entFile))
            {
                MessageBox.Show("Specified entity translation file does not exist");
                entFileTextBox.Focus();
                return;
            }

            // Are we going to be doing a topological export or not?
            bool isTopological = polDataRadioButton.Checked;

            // Are we going to write arcs as polylines? If so, check
            // environment variable to see if a non-standard arc
            // tolerance has been specified.
            double arcTol = 0.0;
            if (arcPolylineCheckBox.Checked)
            {
                // The default tolerance is 1mm on the ground.
                arcTol = 0.001;

                // If the environment variable is set, pick up the specified value.
                string s = System.Environment.GetEnvironmentVariable("CED_ARCTOL");
                if (!String.IsNullOrEmpty(s))
                {
                    double envtol = Double.Parse(s);

                    // Do a sanity check.
                    if (envtol < 0.000001 || envtol > 1.0)
                    {
                        string msg = String.Format(
                            "Arc tolerance (CED_ARCTOL = {0}) is not in the acceptable range (1 micron to 1 metre).",
                            envtol);

                        MessageBox.Show(msg);
                        return;
                    }

                    arcTol = envtol;
                }
            }

            // Get the output file spec
            string jobSpec = EditingController.Current.Project.Name;
            string fileName = Path.ChangeExtension(jobSpec, ".dxf");

            using (SaveFileDialog dial = new SaveFileDialog())
            {
                dial.Title = "Save AutoCad file";
                dial.Filter = "AutoCAD DXF (.dxf)|*.dxf|All files (*.*)|*.*";
                dial.DefaultExt = "dxf";
                dial.FileName = Path.GetFileName(fileName);

                string dir = Path.GetDirectoryName(fileName);
                if (Directory.Exists(dir))
                    dial.InitialDirectory = dir;

                if (dial.ShowDialog(this) != DialogResult.OK)
                    return;

                fileName = dial.FileName;
            }

            // Only remember the entity translation file if it's non-blank.
            if (!String.IsNullOrEmpty(entFile))
                Settings.Default.EntityTranslation = entFile;

            // Remember default version for next time
            string version = versionComboBox.SelectedText;
            Settings.Default.AutoCadVersion = version;
            Settings.Default.Save();

            // Create the writer with the specified values
            m_Writer = new DxfWriter();
            m_Writer.FileName = fileName;
            m_Writer.ArcTolerance = arcTol;
            m_Writer.EntityTranslationFileName = entFile;
            m_Writer.IsTopological = isTopological;
            m_Writer.Version = version;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// An object that may be used to write a DXF file using the
        /// parameters specified via this dialog (defined only when
        /// the user OKs the dialog).
        /// </summary>
        internal DxfWriter Writer
        {
            get { return m_Writer; }
        }
    }
}
