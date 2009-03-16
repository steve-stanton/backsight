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
using System.Collections.Generic;

using Backsight.Editor.Observations;


namespace Backsight.Editor.Operations
{
    /// <summary>
    /// Editing operation that transfers data from a <see cref="FileImportSource"/> to
    /// the current map model.
    /// </summary>
    class ImportOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The features that were imported (null until the <see cref="Execute"/> method
        /// is called).
        /// </summary>
        Feature[] m_Data;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor, for use during deserialization
        /// </summary>
        public ImportOperation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Import"/> class
        /// that refers to nothing.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal ImportOperation(Session s)
            : base(s)
        {
            m_Data = null;
        }

        #endregion

        /// <summary>
        /// Executes this import by loading data from the supplied source.
        /// </summary>
        /// <param name="source">The data source to use for the import</param>
        internal void Execute(FileImportSource source)
        {
            m_Data = source.Load(this);
            Complete();
        }

        internal override Feature[] Features
        {
            get { return m_Data; }
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.DataImport; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Data import"; }
        }

        public override void AddReferences()
        {
            // No direct references
        }

        internal override Distance GetDistance(LineFeature line)
        {
            return null; // nothing to do
        }

        internal override bool Undo()
        {
            base.OnRollback();

        	// Get rid of the features that were created.
            foreach (Feature f in m_Data)
                Rollback(f);

            return true;
        }

        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Nothing to do

            return true;
        }

        /// <summary>
        /// The string that will be used as the xsi:type for this content.
        /// </summary>
        /// <remarks>Implements IXmlContent</remarks>
        public override string XmlTypeName
        {
            get { return "ImportType"; }
        }

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
            base.WriteChildElements(writer);

            foreach (Feature f in m_Data)
                writer.WriteElement("Feature", f);
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
            m_Data = reader.ReadArray<Feature>("Feature");
        }

        /// <summary>
        /// Calculates the geometry for any features created by this edit.
        /// </summary>
        public override void CalculateGeometry()
        {
            // Nothing to do
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>Null (always), since this edit doesn't supersede any lines.</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            return null;
        }
    }
}
