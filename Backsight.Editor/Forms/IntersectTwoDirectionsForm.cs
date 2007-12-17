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

        private void IntersectTwoDirectionsForm_Shown(object sender, EventArgs e)
        {
            // Initialize the first page last, to ensure focus is on the initial text box
            // of the first page.
            getDirection2.InitializeControl(this, 2);
            getDirection1.InitializeControl(this, 1);
        }

        internal override void OnDraw(PointFeature point)
        {
            getDirection1.OnDrawAll();
            getDirection2.OnDrawAll();
        }

        /// <summary>
        /// Reacts to the selection of a point feature.
        /// </summary>
        /// <param name="point">The point (if any) that has been selected.</param>
        internal override void OnSelectPoint(PointFeature point)
        {
            GetDirectionControl gdc = GetVisibleDirectionControl();
            if (gdc!=null)
                gdc.OnSelectPoint(point);
        }

        /// <summary>
        /// Reacts to the selection of a line feature.
        /// </summary>
        /// <param name="line">The line (if any) that has been selected.</param>
        internal override void OnSelectLine(LineFeature line)
        {
            GetDirectionControl gdc = GetVisibleDirectionControl();
            if (gdc!=null)
                gdc.OnSelectLine(line);
        }

        GetDirectionControl GetVisibleDirectionControl()
        {
            if (getDirection1.Visible)
                return getDirection1;

            if (getDirection2.Visible)
                return getDirection2;

            return null;
        }
    }
}

