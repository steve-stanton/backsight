using System;
using System.Windows.Forms;

using Backsight.Editor.Properties;

namespace Backsight.Editor
{
    public class EditingHelpProvider : HelpProvider
    {
        public EditingHelpProvider()
        {
            base.HelpNamespace = Settings.Default.HelpFile;
        }
    }
}
