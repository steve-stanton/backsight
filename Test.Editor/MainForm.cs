using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Microsoft.Win32;

using Backsight.SqlServer;
using Backsight.Data;

namespace Test.Editor
{
    public partial class MainForm : Form
    {
        public MainForm(string[] args)
        {
            /*
            // If user double-clicked on a file, it should appear as an argument. In that
            // case, attempt to open it
            JobFile jf = null;
            if (args!=null && args.Length>0 && File.Exists(args[0]))
            {
                try
                {
                    jf = JobFile.CreateInstance(args[0]);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            */
            InitializeComponent();
        }

        string GetConnectionString()
        {
            string hklm = Registry.LocalMachine + @"\Software\Backsight";
            object o = Registry.GetValue(hklm, "ConnectionString", String.Empty);
            string cs = (o==null ? String.Empty : o.ToString());

            if (String.IsNullOrEmpty(cs))
            {
                ConnectionForm dial = new ConnectionForm();
                if (dial.ShowDialog() == DialogResult.OK)
                    cs = dial.ConnectionString;
            }

            return cs;
        }

        /// <summary>
        /// Saves a global setting
        /// </summary>
        /// <param name="settingName">The name of the setting</param>
        /// <param name="val">The value to save</param>
        public static void Write(string settingName, string val)
        {
            //Registry.SetValue(s_UserRoot, settingName, val);
        }
    }
}