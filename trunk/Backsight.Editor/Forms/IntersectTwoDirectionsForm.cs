using System;
using System.Windows.Forms;

namespace Backsight.Editor.Forms
{
    partial class IntersectTwoDirectionsForm : IntersectForm
    {
        /// <summary>
        /// Creates a new <c>IntersectForm</c>
        /// </summary>
        /// <param name="cmd">The command displaying this dialog (not null)</param>
        /// <param name="title">The string to display in the form's title bar</param>
        internal IntersectTwoDirectionsForm(CommandUI cmd, string title)
            : base(cmd, title)
        {
            InitializeComponent();
        }
    }
}

