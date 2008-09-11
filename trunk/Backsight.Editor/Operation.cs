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
using System.Diagnostics;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Data;

using Backsight.Editor.Database;
using Backsight.Data;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="23-OCT-1997" was="CeOperation" />
    /// <summary>
    /// Base class for any sort of editing operation.
    /// </summary>
    abstract class Operation : IFeatureDependent, IXmlContent
    {
        #region Static

        /// <summary>
        /// The item number of the current editing operation.
        /// </summary>
        static uint s_CurrentEditSequence = 0;

        /// <summary>
        /// The item number of the current editing operation.
        /// </summary>
        internal static uint CurrentEditSequence
        {
            get { return s_CurrentEditSequence; }
            set { s_CurrentEditSequence = value; }
        }

        #endregion

        #region Class data

        /// <summary>
        /// The session in which this operation was originally defined.
        /// </summary>
        Session m_Session;

        /// <summary>
        /// The item sequence number of this operation (starts at 1 for each session).
        /// </summary>
        uint m_Sequence;

        /// <summary>
        /// Flag bits
        /// </summary>
        OperationFlag m_Flag;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new editing operation as oart of the current default session. This constructor
        /// should be called during de-serialization from the database.
        /// </summary>
        protected Operation()
        {
            m_Session = Session.CurrentSession;
            if (m_Session==null)
                throw new ArgumentNullException("Editing session is not defined");

            m_Session.Add(this);
            m_Sequence = s_CurrentEditSequence;
        }

        /// <summary>
        /// Creates a new editing operation as part of the supplied session. This constructor
        /// should be called during regular editing work.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        protected Operation(Session s)
        {
            if (s==null)
                throw new ArgumentNullException();

            s.Add(this);
            m_Session = s;
            m_Sequence = Session.ReserveNextItem();
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        abstract public string Name { get; }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        abstract internal Distance GetDistance(LineFeature line);

        /// <summary>
        /// The features created by this editing operation (may be an empty array, but
        /// never null).
        /// </summary>
        /// <remarks>
        /// All features created by an operation <b>must</b> be explicitly referenced by
        /// the concrete operation class. This is necessary because the reference that
        /// the <c>Feature</c> class has to it's creating operation is not persisted
        /// (not serialized). The <c>Operation.OnLoad</c> method is responsible for
        /// restoring the <c>Feature->Operation</c> reference, and that depends on the
        /// <c>Operation.Features</c> property.
        /// </remarks>
        abstract internal Feature[] Features { get; }

        /// <summary>
        /// The number of features created (or deleted) by this edit. This implementation
        /// returns the number of created features - the <see cref="DeletionOperation"/>
        /// class overrides
        /// </summary>
        /// <remarks>This property is used by <see cref="SessionForm"/> to show
        /// the number of feature involved in each edit.</remarks>
        public virtual uint FeatureCount
        {
            get { return (uint)Features.Length; }
        }

        /// <summary>
        /// Operation sequence number.
        /// </summary>
        public uint EditSequence
        {
            get { return m_Sequence; }
        }

        /// <summary>
        /// The external ID of this edit is a concatenation of the
        /// <see cref="Session.Id"/> and <see cref="EditSequence"/> properties
        /// (seperated with a period).
        /// </summary>
        internal string DataId
        {
            get { return String.Format("{0}.{1}", m_Session.Id, m_Sequence); }
        }

        /// <summary>
        /// Initializes this operation upon loading of the session that contains it.
        /// Any overrides should call this version up front.
        /// </summary>
        /// <param name="container">The editing session that contains this operation</param>
        /*
        internal virtual void OnLoad(Session container)
        {
            m_Sequence = container.MapModel.ReserveNextOpSequence();

            Feature[] createdFeatures = this.Features;
            foreach (Feature f in createdFeatures)
                f.OnLoad(this, true);
        }
        */

        /// <summary>
        /// The session in which this operation was originally defined.
        /// </summary>        
        internal Session Session
        {
            get { return m_Session; }
        }

        /// <summary>
        /// The coordinate system of the map model this operation is part of.
        /// A shortcut to <see cref="MapModel.CoordinateSystem"/>
        /// </summary>
        internal ICoordinateSystem CoordinateSystem
        {
            get { return MapModel.CoordinateSystem; }
        }

        /// <summary>
        /// The map model this operation is part of.
        /// A shortcut to <see cref="Session.MapModel"/>
        /// </summary>
        internal CadastralMapModel MapModel
        {
            get { return (m_Session==null ? null : m_Session.MapModel); }
        }

        public void OnMove(Feature f)
        {
            throw new NotImplementedException("Operation.OnMove");
        }

        /// <summary>
        /// Convenience method that marks a line as "moved" (first checks whether
        /// the specified line is null). This is normally called by implementations
        /// of the <c>Intersect</c> method.
        /// </summary>
        /// <param name="line">The line to mark as moved (may be null)</param>
        protected void SetMoved(LineFeature line)
        {
            if (line!=null)
                line.IsMoved = true;
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        abstract internal EditingActionId EditId { get; }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        abstract internal bool Undo();

        /// <summary>
        /// Rollback any "extra things" known to this operation. Should be called at
        /// start of implementation of the <c>Rollback</c> method.
        /// </summary>
        protected void OnRollback()
        {
            // As I recall, the only extra things were topological lines that were split
            // as a result of the edit. This should (probably) be handled some other way
            // in the new system...

            /*
	        // Return if there are no extra things.
	        if ( !m_pMore ) return;

	        // Mark the extra things for deletion.
	        CeListIter loop(m_pMore);
	        CeClass* pThing;

	        for ( pThing=(CeClass*)loop.GetHead(); pThing; pThing=(CeClass*)loop.GetNext() ) {

		        CeFeature* pFeat = dynamic_cast<CeFeature*>(pThing);
		        if ( pFeat )
			        pFeat->SetDeleted();
		        else
			        ShowMessage("CeOperation::OnRollback\nUnexpected object encountered");
	        }

	        // Delete the list.
	        delete m_pMore;
	        m_pMore = 0;
             */
        }

        /// <summary>
        /// Convenience method that marks a feature as "being undone" (first checks whether
        /// the specified feature is null). This is normally called by implementations
        /// of the <c>Rollback(void)</c> method.
        /// </summary>
        /// <param name="line">The feature to mark for undo (may be null)</param>
        protected void Rollback(Feature f)
        {
            if (f!=null && f.Creator==this)
                f.Undo();
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        abstract internal bool Rollforward();

        /// <summary>
        /// Is a flag bit set?
        /// </summary>
        /// <param name="flag">The flag(s) to check for (may be a combination of more
        /// than one flag)</param>
        /// <returns>True if any of the supplied flag bits are set</returns>
        bool IsFlagSet(OperationFlag flag)
        {
            return ((m_Flag & flag)!=0);
        }

        /// <summary>
        /// Sets flag bit(s)
        /// </summary>
        /// <param name="flag">The flag bit(s) to set</param>
        /// <param name="setting">True to set, false to clear</param>
        void SetFlag(OperationFlag flag, bool setting)
        {
            if (setting)
                m_Flag |= flag;
            else
                m_Flag &= (~flag);
        }

        /// <summary>
        /// Has this operation been marked as changed. This indicates whether the
        /// operation needs to be re-executed as part of rollforward processing.
        /// </summary>
        protected bool IsChanged
        {
            get { return IsFlagSet(OperationFlag.Changed); }
        }

        /// <summary>
        /// Clears the "changed" status for this operation. All derived classes should
        /// call this function at the end of their implementation of <c>Rollforward</c>.
        /// </summary>
        /// <returns>True (always)</returns>
        protected bool OnRollforward()
        {
            SetFlag(OperationFlag.Changed, false);
	        return true;
        }

        #region IFeatureDependent Members

        public void OnPreMove(Feature f)
        {
            throw new NotImplementedException("Operation.OnPreMove");
        }

        public void OnPostMove(Feature f)
        {
            SetFlag(OperationFlag.Changed, true);
        }

        #endregion

        /// <summary>
        /// Performs processing that should be performed at the end of the <c>Execute
        /// </c> method of each editing operation. Features that were created will initially
        /// get indexed via a call to <see cref="CadastralMapModel.AddToIndex"/>. Followed
        /// by calls to <see cref="AddReferences"/>,  <see cref="PrepareForIntersect"/>, and
        /// <see cref="CadastralMapModel.CleanEdit"/>.
        /// </summary>
        protected void Complete()
        {
            // Index features that were created (and ensure the map extent has been
            // expanded to include the new features)
            Feature[] feats = Features;
            MapModel.AddToIndex(feats);

            // Assign 1-based creation sequence to each created feature
            uint numItem = Session.WorkingSession.NumItem;
            for (uint i = 0; i < feats.Length; i++)
            {
                numItem++;
                feats[i].CreatorSequence = numItem;
            }

            // Point referenced features to this editing operation
            AddReferences();

            // Mark any new topological lines as "moved" so that they will be
            // intersected with the map
            PrepareForIntersect(feats);

            // Ensure the map structure has been updated to account for the new data.
            MapModel.CleanEdit();

            // Ensure the item count for the session has been updated
            Session.WorkingSession.NumItem = numItem;

            // Save the edit to the database
            SaveOperation();
        }

        /// <summary>
        /// Saves an editing operation in the database. This writes to the <c>Edits</c>
        /// table and updates the timestamp in the <c>Sessions</c> table.
        /// </summary>
        /// <param name="op">The edit to save</param>
        void SaveOperation()
        {
            Trace.Write("Saving to database");

            // Save the last edit in the database
            string x = this.ToXml();

            /*
            using (StreamWriter sw = File.CreateText(@"C:\Temp\LastEdit.txt"))
            {
                sw.Write(x);
            }
             */

            Transaction.Execute(delegate
            {
                // Insert the edit
                SqlCommand c = new SqlCommand();
                c.Connection = Transaction.Connection.Value;
                c.CommandText = "INSERT INTO [ced].[Edits] ([SessionId], [EditSequence], [Data])" +
                                    " VALUES (@sessionId, @editSequence, @data)";
                c.Parameters.Add(new SqlParameter("@sessionId", SqlDbType.Int));
                c.Parameters.Add(new SqlParameter("@editSequence", SqlDbType.Int));
                c.Parameters.Add(new SqlParameter("@data", SqlDbType.Xml));
                c.Parameters[0].Value = Session.WorkingSession.Id;
                c.Parameters[1].Value = m_Sequence;
                c.Parameters[2].Value = x;
                c.ExecuteNonQuery();

                // Update the end-time associated with the session
                Session.WorkingSession.UpdateEndTime();
            });
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// <para/>
        /// This is called by the <see cref="Complete"/> method, to ensure
        /// that the referenced features are cross-referenced to the editing operations
        /// that depend on them.
        /// </summary>
        abstract public void AddReferences();

        /// <summary>
        /// Prepares the supplied features for intersect detection that should be
        /// performed by <see cref="CadastralMapModel.CleanEdit"/>. This modifies
        /// line features by setting the <see cref="LineFeature.IsMoved"/> property.
        /// Called by the <see cref="Complete"/> method
        /// </summary>
        /// <param name="fa">The features that may contain lines that need to be prepared</param>
        void PrepareForIntersect(Feature[] fa)
        {
            foreach (Feature f in fa)
            {
                LineFeature line = (f as LineFeature);
                if (line!=null && line.IsTopological)
                    line.IsMoved = true;
            }
        }

        /// <summary>
        /// Cuts any reference made to this operation for an observed distance that was
        /// specified using an <see cref="OffsetPoint"/> object.
        /// </summary>
        /// <param name="dist">The distance observation. If this is not an <c>OffsetPoint</c>
        /// object, this function does nothing.</param>
        protected void CutOffsetRef(Observation dist)
        {
            // Return if the distance observation is not for an offset point
            OffsetPoint off = (dist as OffsetPoint);
            if (off == null)
                return;

            // Get the offset point and cut the reference it has to this operation.
            PointFeature point = off.Point;
            if (point != null)
                point.CutOp(this);
        }

        #region IXmlContent Members

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        abstract public void WriteContent(XmlContentWriter writer);

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader"/> during deserialization from XML (just
        /// after the default constructor has been invoked).
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public virtual void ReadContent(XmlContentReader reader)
        {
            m_Session = Session.CurrentSession;
            m_Sequence = Operation.CurrentEditSequence;
            m_Flag = 0;
        }

        #endregion

        /// <summary>
        /// Represents this editing operation in XML (suitable for inserting
        /// into the database)
        /// </summary>
        /// <returns>The XML for this edit</returns>
        internal string ToXml()
        {
            return ToXml(false);
        }

        /// <summary>
        /// Represents this editing operation in XML, with optional indentation of elements.
        /// </summary>
        /// <param name="indent">Should the XML be indented or not?</param>
        /// <returns>The XML for this edit</returns>
        internal string ToXml(bool indent)
        {
            // This will end up calling the implementation of WriteContent
            XmlContentWriter.TargetNamespace = "Backsight";
            return XmlContentWriter.GetXml("Edit", indent, this);
        }
    }
}
