/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Data;

using Backsight.Environment;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="20-OCT-1999" was="CeNewLabelEx"/>
    /// <summary>
    /// A new polygon label that is created on a derived layer so as to
    /// replace an existing label that was originally shared with the base layers.
    /// </summary>
    class ReplaceTextOperation : NewTextOperation
    {
        #region Class data

        /// <summary>
        /// The text label that was superseded as a consequence of adding
        /// an extra label to a derived layer.
        /// </summary>
        TextFeature m_OldText; // readonly

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor (for serialization)
        /// </summary>
        public ReplaceTextOperation()
        {
        }

        /// <summary>
        /// Creates a new <c>ReplaceTextOperation</c> that refers to the text that's
        /// being replaced, but which doesn't yet refer to new text.
        /// </summary>
        internal ReplaceTextOperation(TextFeature oldText)
            : base()
        {
            m_OldText = oldText;
        }

        #endregion

        /// <summary>
        /// Executes the new label operation. This replaces an old label on a base
        /// theme that is being superseded by the new label (the old label should
        /// not be key text, since what gets created here will also be key text).
        /// </summary>
        /// <param name="vtx">The position of the new label.</param>
        /// <param name="ent">The entity type for the new label.</param>
        /// <param name="pol">The polygon that the label falls inside.</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The width of the text, in meters on the ground.</param>
        /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
        internal void Execute(IPosition vtx, IEntity ent, Polygon pol,
                            double height, double width, double rotation)
        {
            // Confirm the old label has an ID.
            FeatureId fid = m_OldText.Id;
            if (fid==null)
                throw new Exception("ReplaceTextOperation.Execute - ID is not available.");

            // De-activate the old label.
            SetOldLabel();

            // Add the new label on the current editing layer.
            CreateLabel(vtx, ent, fid, pol, height, width, rotation);
        }

        /// <summary>
        /// Creates a key-text label.
        /// </summary>
        /// <param name="vtx">The reference position of the label.</param>
        /// <param name="ent">The entity type for the new label.</param>
        /// <param name="id">The ID for the new label.</param>
        /// <param name="pol">The polygon that the label relates to.</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The width of the text, in meters on the ground.</param>
        /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
        void CreateLabel(IPosition vtx, IEntity ent, FeatureId id, Polygon pol,
                            double height, double width, double rotation)
        {
            // Get the map to add a new label to the current editing layer (without any ID).
            TextFeature label = MapModel.AddKeyLabel(this, ent, vtx, height, width, rotation);

            // Relate the new label to the specified ID and vice versa.
            label.SetId(id);
            id.AddReference(label);

            // The label MUST be topological, so make sure it's marked as such.
            label.SetTopology(true);

            // Relate the label to the specified polygon & vice versa.
            pol.ClaimLabel(label);

            // Hold reference to the new label.
            SetText(label);

            Complete();
        }

        /// <summary>
        /// Executes the new label operation.
        /// </summary>
        /// <param name="vtx">The position of the new label.</param>
        /// <param name="ent">The entity type for the new label.</param>
        /// <param name="row">The transient row to use for creating a row for the new label.</param>
        /// <param name="atemplate">The template to use in creating the RowText for the new label.</param>
        /// <param name="pol">The polygon that the label falls inside.</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The width of the text, in meters on the ground.</param>
        /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
        internal void Execute(IPosition vtx, IEntity ent, DataRow row, ITemplate atemplate, Polygon pol,
                            double height, double width, double rotation)
        {
            // Confirm the old label has an ID.
            FeatureId fid = m_OldText.Id;
            if (fid == null)
                throw new Exception("ReplaceTextOperation.Execute - ID is not available.");

            // De-activate the old label.
            SetOldLabel();

            // Add the new label on the current editing layer.
            CreateLabel(vtx, ent, row, atemplate, fid, m_OldText.IsForeignId, pol, height, width, rotation);
        }

        /// <summary>
        /// Creates a row-text label.
        /// </summary>
        /// <param name="vtx">The reference position of the label.</param>
        /// <param name="ent">The entity type for the new label.</param>
        /// <param name="row">The transient row to use for creating a row for the new label.</param>
        /// <param name="atemplate">The template to use in creating the RowText for the new label.</param>
        /// <param name="id">The ID for the new label.</param>
        /// <param name="isForeign">Is the ID foreign?</param>
        /// <param name="pol">The polygon that the label relates to.</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The width of the text, in meters on the ground.</param>
        /// <param name="rotation">The clockwise rotation of the text, in radians from the horizontal.</param>
        void CreateLabel(IPosition vtx, IEntity ent, DataRow row, ITemplate atemplate,
                            FeatureId id, bool isForeign, Polygon pol,
                            double height, double width, double rotation)
        {
            // Get the map to add a new label to the current editing layer (without any ID).
            TextFeature label = MapModel.AddRowLabel(this, ent, vtx, row, atemplate, height, width, rotation);

            // Relate the new label to the specified ID and vice versa.
            label.SetId(id);
            id.AddReference(label);

            // Relate the row to the ID and vice versa.
            //row.SetId(id);

            // The label MUST be topological, so make sure it's marked as such.
            label.SetTopology(true);

            // Relate the label to the specified polygon & vice versa.
            pol.ClaimLabel(label);

            // Hold reference to the new label.
            SetText(label);

            Complete();
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Restore the original label.
            m_OldText.Restore();

            return true;
        }

        /// <summary>
        /// De-activates the label that is being replaced.
        /// </summary>
        void SetOldLabel()
        {
            // De-activate the old label (given that the active
            // editing layer is something like the Assessment layer,
            // but the label is shared with the Plan and Ownership
            // layers, this will also create a label that belongs
            // exclusively to Plan+Ownership).

            m_OldText.IsInactive = true;
        }

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);
            writer.WriteFeatureReference("OldText", m_OldText);
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
            base.WriteChildElements(writer);
        }

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader"/> during deserialization from XML (just
        /// after the default constructor has been invoked).
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadContent(XmlContentReader reader)
        {
            // TODO: I believe the old text should be getting deactivated when the
            // replacement text becomes known. However, the Execute method doesn't
            // appear to do that, so for the time being, I won't do it here either.

            m_OldText = reader.ReadFeatureByReference<TextFeature>("OldText");
            base.ReadContent(reader);
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadAttributes(XmlContentReader reader)
        {
            base.ReadAttributes(reader);
        }

        /// <summary>
        /// Defines any child content related to this instance. This will be called after
        /// all attributes have been defined via <see cref="ReadAttributes"/>.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadChildElements(XmlContentReader reader)
        {
            base.ReadChildElements(reader);
        }

        /// <summary>
        /// Calculates the geometry for any features created by this edit.
        /// </summary>
        public override void CalculateGeometry()
        {
            // Nothing to do
        }
    }
}
