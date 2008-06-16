/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

using Backsight.Editor.Properties;
using Backsight.Editor.Operations;
using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    public partial class NtxImportForm : Form, ITranslate
    {
        /// <summary>
        /// Feature code translations.
        /// </summary>
        TranslationFile m_Translator;

        public NtxImportForm()
        {
            InitializeComponent();
            m_Translator = null;
        }

        private void NtxImportForm_Shown(object sender, EventArgs e)
        {
            statusStrip.Visible = false;
            string fileName = Settings.Default.FeatureCodeTranslation;
            if (File.Exists(fileName))
                translationTextBox.Text = fileName;
        }

        private void browseNtxButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dial = new OpenFileDialog();
            dial.InitialDirectory = Settings.Default.NtxImportFolder;
            dial.Filter = "NTX files (*.ntx)|*.ntx|All files (*)|*";

            if (dial.ShowDialog()==DialogResult.OK)
            {
                ntxFileTextBox.Text = dial.FileName;
                Settings.Default.NtxImportFolder = Path.GetDirectoryName(dial.FileName);
                Settings.Default.Save();
            }
        }

        private void browseFeatureTTbutton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dial = new OpenFileDialog();
            string fileName = Settings.Default.FeatureCodeTranslation;
            if (!String.IsNullOrEmpty(fileName))
                dial.InitialDirectory = Path.GetDirectoryName(fileName);
            dial.Filter = "Entity translation files (*.ent)|*.ent|All files (*)|*";

            if (dial.ShowDialog()==DialogResult.OK)
            {
                translationTextBox.Text = dial.FileName;
                Settings.Default.FeatureCodeTranslation = dial.FileName;
                Settings.Default.Save();
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            // Confirm stuff is defined
            string ntxFile = ntxFileTextBox.Text.Trim();
            string translationFile = translationTextBox.Text.Trim();

            if (ntxFile.Length==0)
            {
                MessageBox.Show("You must first specify the name of the input file.");
                return;
            }

            // Load any translation file that's been specified
            if (translationFile.Length>0)
            {
                m_Translator = new TranslationFile();
                m_Translator.Load(translationFile);
            }

            ForwardingTraceListener trace = new ForwardingTraceListener(ShowLoadProgress);

            try
            {
                statusStrip.Visible = true;
                loadButton.Enabled = false;
                closeButton.Enabled = false;
                Trace.Listeners.Add(trace);
                LoadNtx(ntxFile);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally
            {
                Trace.Listeners.Remove(trace);
                statusStrip.Visible = false;
                closeButton.Enabled = true;
            }
        }

        void ShowLoadProgress(string msg)
        {
            loadProgressLabel.Text = msg;
            statusStrip.Refresh();
        }

        void LoadNtx(string ntxFile)
        {
            Debug.Assert(Session.CurrentSession!=null);

            // If the model is currently empty, we'll want to draw an overview upon
            // completion of the import (otherwise we'll refresh using the currently
            // displayed extent).
            bool wasEmpty = CadastralMapModel.Current.IsEmpty;

            Import i = null;
            EditingController c = EditingController.Current;

            try
            {
                i = new Import(Session.CurrentSession);
                NtxImport ni = new NtxImport(ntxFile, this);
                i.Execute(ni);
                Trace.Write("Map model updates completed");
            }

            catch (Exception ex)
            {
                Session.CurrentSession.Remove(i);
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }

            // Re-assigning the current model has the desired effect of causing
            // an overview display...
            if (wasEmpty)
                c.MapModel = CadastralMapModel.Current;
            else
                c.RefreshAllDisplays();

            SaveTranslations();
        }

        void SaveTranslations()
        {
            if (m_Translator==null || m_Translator.IsSaved)
                return;

            if (MessageBox.Show("Do you want to save feature code translations?", "Save translations?",
                MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            SaveFileDialog dial = new SaveFileDialog();
            dial.Filter = "Entity translation files (*.ent)|*.ent|All files (*)|*";
            dial.DefaultExt = "ent";

            if (!String.IsNullOrEmpty(Settings.Default.FeatureCodeTranslation))
            {
                string dir = Path.GetDirectoryName(Settings.Default.FeatureCodeTranslation);
                if (!String.IsNullOrEmpty(dir))
                    dial.InitialDirectory = dir;
            }

            if (dial.ShowDialog() == DialogResult.OK)
            {
                m_Translator.Save(dial.FileName);
                Settings.Default.FeatureCodeTranslation = dial.FileName;
                Settings.Default.Save();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region ITranslate Members

        public IEntity FindEntityTypeByExternalName(string alias, SpatialType type)
        {
            IEntity result = (m_Translator==null ? null : m_Translator.Translate(alias));
            if (result!=null)
                return result;

            // Ask the user to pick a suitable translation
            EntityTranslationForm dial = new EntityTranslationForm(alias, type);
            dial.ShowDialog();
            result = dial.Result;
            dial.Dispose();

            // Remember what was specified
            if (result!=null)
            {
                if (m_Translator==null)
                    m_Translator = new TranslationFile();

                m_Translator.Add(alias, result);
            }

            return result;
        }

        #endregion
    }
}
