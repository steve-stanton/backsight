// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System;
using System.Text;
using System.Drawing;

using Backsight.Editor.Properties;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="18-NOV-1999" was="CeTextCheck" />
    /// <summary>
    /// The result of a check on an item of text.
    /// </summary>
    class TextCheck : CheckItem
    {
        #region Static

        /// <summary>
        /// Performs checks for a text label.
        /// </summary>
        /// <param name="label">The label to check.</param>
        /// <returns>The problem(s) that were found.</returns>
        internal static CheckType CheckLabel(TextFeature label)
        {
            CheckType types = CheckType.Null;
            Polygon p = label.Container;

            if (p==null)
                types |= CheckType.NoPolygonForLabel;
            else
            {
                // Does the polygon point back? If not, we've got a multi-label.
                if (p.LabelCount>1 && p.Label!=label)
                    types |= CheckType.MultiLabel;
            }

            // Does the label have at least one row of attribute data?
            FeatureId fid = label.FeatureId;
            if (fid==null || fid.RowCount==0)
                types |= CheckType.NoAttributes;

            return types;
        }

        #endregion

        #region Class data

        /// <summary>
        /// The text the check relates to (never null).
        /// </summary>
        readonly TextFeature m_Label;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>TextCheck</c> that relates to the specified text.
        /// </summary>
        /// <param name="label">The text the check relates to (not null).</param>
        /// <param name="types">The type(s) of check this item corresponds to</param>
        /// <exception cref="ArgumentNullException">If <paramref name="label"/> is null</exception>
        internal TextCheck(TextFeature label, CheckType types)
            : base(types)
        {
            if (label==null)
                throw new ArgumentNullException();

            m_Label = label;
        }

        #endregion

        /// <summary>
        /// Gets the spacing between icons representing check markers.
        /// </summary>
        /// <returns>The X-shift between successive check markers</returns>
        double GetIconSpacing(ISpatialDisplay display)
        {
            double size = IconSize(display);
            return (size * 1.1);
        }

        /// <summary>
        /// Repaint icon(s) representing this check result.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        internal override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            // Draw stuff that's now irrelevant
            RenderPaintOuts(display, style);

            // Return if the label has been de-activated.
            if (m_Label.IsInactive)
                return;

            // Remember the display position of the label (top left corner).
            IPosition p = m_Label.Position;
            Place = p;

            // Figure out a position that is a bit to the left (a bit bigger than an icon)
            double shift = GetIconSpacing(display);
            p = new Position(p.X-shift, p.Y);

            // Draw icon(s).
            CheckType types = Types;
            if ((types & CheckType.NoPolygonForLabel)!=0)
            {
                style.Render(display, p, Resources.CheckNoPolygonForLabelIcon);
                p = new Position(p.X-shift, p.Y);
            }

            if ((types & CheckType.NoAttributes)!=0)
            {
                style.Render(display, p, Resources.CheckNoAttributesIcon);
                p = new Position(p.X-shift, p.Y);
            }

            if ((types & CheckType.MultiLabel)!=0)
            {
                style.Render(display, p, Resources.CheckMultiLabelIcon);
                p = new Position(p.X-shift, p.Y);
            }
        }

        /// <summary>
        /// The overlay icon drawn on top of any painted out icons.
        /// </summary>
        internal override Icon PaintOutIcon
        {
            get { return Resources.CheckPolygonIgnoreIcon; }
        }

        /// <summary>
        /// Paints out those results that no longer apply.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        /// <param name="newTypes">The new results</param>
        internal override void PaintOut(ISpatialDisplay display, IDrawStyle style, CheckType newTypes)
        {
            // Get the reference position last used to paint stuff
            IPosition p = this.Place;

            double shift = GetIconSpacing(display);
            p = new Position(p.X-shift, p.Y);
            CheckType oldTypes = Types;

            if (IsPaintOut(CheckType.NoPolygonForLabel, oldTypes, newTypes))
            {
                AddPaintOut(p, Resources.CheckNoPolygonForLabelIcon);
                p = new Position(p.X-shift, p.Y);
            }

            if (IsPaintOut(CheckType.NoAttributes, oldTypes, newTypes))
            {
                AddPaintOut(p, Resources.CheckNoAttributesIcon);
                p = new Position(p.X-shift, p.Y);
            }

            if (IsPaintOut(CheckType.MultiLabel, oldTypes, newTypes))
            {
                AddPaintOut(p, Resources.CheckMultiLabelIcon);
                p = new Position(p.X-shift, p.Y);
            }
        }

        /// <summary>
        /// Rechecks this result.
        /// </summary>
        /// <returns>The result(s) that still apply.</returns>
        internal override CheckType ReCheck()
        {
            // No errors if the label is now non-topological.
            if (!m_Label.IsTopological)
                return CheckType.Null;

            // No errors if it's been deleted.
            if (m_Label.IsInactive)
                return CheckType.Null;

            // Return if the result has been cleared.
            if (Types == CheckType.Null)
                return CheckType.Null;

	        // Recheck
	        return CheckLabel(m_Label);
        }

        /// <summary>
        /// A textual explanation about this check result.
        /// </summary>
        internal override string Explanation
        {
            get
            {
                if (m_Label.IsInactive)
                    return "(label deleted)";

                string keystr = m_Label.FormattedKey;
                CheckType types = Types;
                StringBuilder sb = new StringBuilder();

                if ((types & CheckType.NoPolygonForLabel)!=0)
                    sb.Append(String.Format("Label '{0}' has no enclosing polygon", keystr) + System.Environment.NewLine);

                if ((types & CheckType.NoAttributes)!=0)
                    sb.Append(String.Format("Label '{0}' has no attributes", keystr) + System.Environment.NewLine);

                if ((types & CheckType.MultiLabel)!=0)
                {
                    sb.Append("More than one label for polygon" + System.Environment.NewLine);
                    Polygon enc = m_Label.Container;
                    if (enc != null)
                    {
                        TextFeature[] labels = enc.GetAllLabels();
                        foreach (TextFeature label in labels)
                            sb.Append(label.TextGeometry.Text + System.Environment.NewLine);
                    }
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Returns a display position for this check. This is the position to
        /// use for auto-centering the draw. It is not necessarily the same as
        /// the position of the problem icon.
        /// </summary>
        internal override IPosition Position
        {
            // Return the display position of the label (top-left corner).
            get { return m_Label.Position; }
        }

        /// <summary>
        /// Selects the object that this check relates to.
        /// </summary>
        internal override void Select()
        {
            // If the label either has no attributes, or it is inside a polygon that has
            // more than one label, select the enclosing polygon.
            Selection ss = new Selection();

            CheckType types = Types;
            if ((types & CheckType.NoAttributes)!=0 || (types & CheckType.MultiLabel)!=0)
            {
                Polygon pol = m_Label.Container;
                if (pol!=null)
                    ss.Add(pol);
            }

            // Select the label too
            ss.Add(m_Label);

            EditingController.Current.SetSelection(ss);

            // Leave the focus with the view (to allow label deletion).
            EditingController.Current.ActiveDisplay.MapPanel.Focus();
        }
    }
}
