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
using System.Diagnostics;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="13-JUL-1997" />
    /// <summary>
    /// An ID acts as a cross-reference between multiple features and multiple rows.
    /// </summary>
    abstract class FeatureId
    {
        #region Class data

        /// <summary>
        /// Either a reference to the single feature that has this key, or a reference to
        /// a list of multiple features that have the same key.
        /// </summary>
        IPossibleList<Feature> m_Features;

        /// <summary>
        /// Either a reference to the single row that has this key, or a reference to
        /// a list of multiple rows that have the same key.
        /// <para/>
        /// There is no constraint that says the feature ID column has to be a unique
        /// key within any given table (although that is desirable). In situations where
        /// the ID is associated with more than one row, it is therefore possible that
        /// multiple rows come from the same table.
        /// </summary>
        IPossibleList<Row> m_Rows;

        #endregion

        #region Constructors

        protected FeatureId()
        {
            m_Features = null;
            m_Rows = null;
        }

        #endregion

        /// <summary>
        /// Relates this ID to the specified feature (and vice versa)
        /// </summary>
        /// <param name="f">The feature that has this ID</param>
        internal void Add(Feature f)
        {
            if (f.Id != null)
            {
                if (f.Id == this)
                    return;

                f.Id.Cut(f);
            }

            AddReference(f);
            f.SetId(this);
        }

        /// <summary>
        /// Removes the association of this ID with the specified feature
        /// </summary>
        /// <param name="f">The feature that should be assigned a null ID</param>
        internal void Cut(Feature f)
        {
            Debug.Assert(f.Id == this);
            CutReference(f);
            f.SetId(null);
        }

        public override string ToString()
        {
            return FormattedKey;
        }

        /// <summary>
        /// The user-perceived ID value
        /// </summary>
        internal abstract string FormattedKey { get; }

        public bool IsInactive
        {
            get { return (m_Rows==null && m_Features==null); }
        }

        internal IPossibleList<Row> Rows
        {
            get { return m_Rows; }
        }

        internal int RowCount
        {
            get { return (m_Rows==null ? 0 : m_Rows.Count); }
        }

        IPossibleList<Feature> Features
        {
            get { return m_Features; }
        }

        /// <summary>
        /// Adds a reference from this ID to a row. 
        /// </summary>
        /// <param name="row">The row to point to.</param>
        internal void AddReference(Row row)
        {
            m_Rows = (m_Rows==null ? row : m_Rows.Add(row));

            // Check whether any associated features are instances of TextFeature that
            // have RowTextContent geometry (a placeholder class that is meant to exist
            // only during deserialization from the database). If so, see whether the
            // geometry can now be replaced with the "proper" RowTextGeometry.

            if (m_Features != null)
            {
                foreach (Feature f in m_Features)
                {
                    TextFeature tf = (f as TextFeature);
                    if (tf != null)
                    {
                        RowTextContent content = (tf.TextGeometry as RowTextContent);
                        if (content != null && content.TableId == row.Table.Id)
                            tf.TextGeometry = new RowTextGeometry(row, content); 
                    }
                }
            }
        }

        /// <summary>
        /// Cuts a reference from this ID to a row of attributes.
        /// </summary>
        /// <param name="row">The row to cut.</param>
        void CutReference(Row row)
        {
            if (m_Rows!=null)
                m_Rows = m_Rows.Remove(row);
        }

        /// <summary>
        /// Adds a reference from this ID to a spatial feature.
        /// </summary>
        /// <param name="feature">The feature to point to.</param>
        public void AddReference(Feature feature)
        {
            m_Features = (m_Features==null ? feature : m_Features.Add(feature));
        }

        /// <summary>
        /// Cuts a reference from this ID to a spatial feature. 
        /// </summary>
        /// <param name="feature">The feature to cut.</param>
        public void CutReference(Feature feature)
        {
            if (m_Features!=null)
                m_Features = m_Features.Remove(feature);
        }

        /// <summary>
        /// Is this ID associated with a SINGLE row of attribute data?
        /// </summary>
        bool HasRow
        {
            get { return (m_Rows!=null && m_Rows.Count==1); }
        }

        /// <summary>
        /// Is this ID associated with more than one row of attribute data?
        /// </summary>
        bool HasRowList
        {
            get { return (m_Rows!=null && m_Rows.Count>1); }
        }

        /// <summary>
        /// The list of rows associated with this ID (null if the ID points to
        /// zero or one ID).
        /// </summary>
        IPossibleList<Row> RowList
        {
            get { return (this.HasRowList ? m_Rows : null); }
        }

        /// <summary>
        /// The single row of attribute data associated with this ID (null if the
        /// ID points to no rows, or more than one row)
        /// </summary>
        Row Row
        {
            get { return (this.HasRow ? (Row)m_Rows : null); }
        }

        /// <summary>
        /// Checks whether this ID points back to a specific feature.
        /// </summary>
        /// <param name="feat"></param>
        /// <returns></returns>
        bool IsReferredTo(Feature feat)
        {
            if (m_Features==null)
                return false;

            foreach(Feature f in m_Features)
            {
                if (object.ReferenceEquals(f,feat))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Does any clean-up after a feature has just cut the reference that
        /// this ID makes to it. The ID will check if it refers to any additional
        /// features and, if not, any attached rows will be deleted.
        /// </summary>
        void Clean()
        {
	        // Return if this ID still refers to spatial features.
	        if (m_Features!=null)
                return;

	        // Detach every row (if any).
            m_Rows = null;
        }

        /// <summary>
        /// Tries to find a row that is attached to this ID, and which has a specific schema.
        /// </summary>
        /// <param name="schema">The schema to search for.</param>
        /// <returns>The matching row (null if not found).</returns>
        /*
        IRow GetRow(ISchema schema)
        {
            if (m_Rows!=null)
            {
                foreach(IRow row in m_Rows)
                {
                    if (schema.Id == row.Schema.Id)
                        return row;
                }
            }

            return null;
        }
         */

        /// <summary>
        /// Creates an ID for an extracted feature. An extracted ID points only to
        /// ONE feature. If this ID is associated with any Row objects and more than
        /// one feature, the rows will be duplicated for each of the features that
        /// get extracted.
        /// </summary>
        /// <param name="xref">Info about the extract.</param>
        /// <param name="exFeat">The extracted feature.</param>
        /// <returns>The extract ID that was created.</returns>
        FeatureId Extract(ExTranslation xref, Feature exFeat)
        {
            throw new NotImplementedException("FeatureId.Extract");
         /*
        	// Create a new ID.
	        FeatureId ex = new FeatureId(0);

            // Assign the key.
	        ex.m_Key = m_Key;

	        // Point to the specified feature.
	        ex.m_Features = exFeat;

	        // If this ID refers to any rows, create copies of the
	        // rows and attached them to the extract ID.
	        if (m_Rows!=null)
            {
                foreach(Row row in m_Rows)
                {
			        // Extract the row.
			        Row exRow = row.Extract(xref,exFeat);

			        // Assign the extract ID to the row (kind of a chicken
			        // and egg case here -- the row can't get the ID from
			        // the extract feature because we're still in the
			        // process of creating the ID for the feature).

			        // Setting the ID defines a two-way cross-reference
			        // between the row and the ID.
			        exRow.Id = ex;
		        }
	        }

	        // Return the ID we created.
	        return ex;
          */
        }

        /// <summary>
        /// Gets any labels associated with a row to remove themselves from
        /// the spatial index. This occurs just before the row is about to
        /// be changed in some way.
        /// </summary>
        /// <param name="row">The row that is about to change.</param>
        void RemoveIndex(Row row)
        {
	        if (m_Features!=null)
            {
                foreach(Feature f in m_Features)
                    f.RemoveIndex(row);
            }
        }

        /// <summary>
        /// Gets any labels associated with a row to add themselves into the
        /// spatial index. This occurs after a row has just been changed in
        /// some way.
        ///
        /// This call should be made at some point soon after a prior call
        /// to <c>FeatureId.RemoveIndex</c>
        /// </summary>
        /// <param name="row">The row that has been changed.</param>
        void AddIndex(Row row)
        {
	        if (m_Features!=null)
            {
                foreach(Feature f in m_Features)
                    f.AddIndex(row);
            }
        }

        /// <summary>
        /// Eliminates duplicate rows attached to this Id, returning the number
        /// of duplicate rows that were found and killed.
        /// </summary>
        /// <param name="numDupRows">Number of duplicate rows that were found and killed.</param>
        /// <returns>Whether or not any duplicates were found and eliminated</returns>
        bool KillDupRows(out int numDupRows)
        {
            numDupRows = 0;
            if (m_Rows==null)
                return false;

	        //int index = 0;
            throw new NotImplementedException("FeatureId.KillDupRows");
            /*
	        // now make a temporary copy of the object list to hold the unique
	        // Rows and the Rows which had associated CeRowText items
	        CeObjectList* pTempList = new CeObjectList(
		        *dynamic_cast<const CeObjectList* const>(m_pRows));
	        CeListIter loop(pTempList);
	        // here's a pointer to a list to hold the pointers to the Rows which will
	        // be deleted
	        CeObjectList* pDeleteList = 0;

	        CeRow* pRow;
	        CeRow* pRowNext;

	        INT4 ICount = 1;
	        if(pTempList) pRow = (CeRow*)loop.GetHead();
	        while (pRow && pTempList && ICount < pTempList->GetCount()) {
		        // first loop through the list to get the next main item
		        pRow = (CeRow*)loop.GetHead();
		        for(INT4 Index = 1; Index < ICount; Index++)
			        pRow = (CeRow*)loop.GetNext();

		        // if there is a next item in the list then can continue to look for
		        // dups
		        if(pRow) pRowNext = (CeRow*)loop.GetNext();
		        else pRowNext = 0;

		        if (!pRowNext) pRow = 0; // done comparing
		        else { // still have items in list, start looking for dups
			        while (pRowNext) {
				        if(pRow->IsSameRow(pRowNext) && !pRowNext->HasRowText()) {
					        if(!pDeleteList) pDeleteList = new CeObjectList();
					        if(pDeleteList) pDeleteList->Append(pRowNext);
					        pTempList->CutRef(*pRowNext);
				        }
				        pRowNext = (CeRow*)loop.GetNext();
			        }
		        }
		        ICount++; // increment for main pointer
	        }
	        if(pDeleteList) { // have Rows to be deleted - kill them
		        numDupRows = pDeleteList->GetCount();
		        CeListIter loop(pDeleteList);
		        for(pRow = (CeRow*)loop.GetHead(); pRow; pRow = (CeRow*)loop.GetNext())
				        delete pRow;
	        }
	        if(pTempList) delete pTempList; // delete the temporary list
	        if(pDeleteList) delete pDeleteList; // delete the delete list 
            */
            //return (numDupRows>0 ? true : false);
        }

        /// <summary>
        /// Determines if an ID already has a row with data the same as the one
        /// in the parameter list
        /// </summary>
        /// <param name="schema">Schema for Row to be tested for being already present</param>
        /// <param name="data">Data for the Row to be tested for being already present</param>
        /// <returns>Whether or not this ID already has a row with the same data</returns>
        /*
        bool HasThisData(ISchema schema, object[] data)
        {
            if (m_Rows!=null)
            {
                foreach(Row row in m_Rows)
                {
                    if (row.IsSameData(schema, data))
                        return true;
                }
            }

            return false;
        }
         */

        /// <summary>
        /// Logs this ID as part of a map comparison.
        /// </summary>
        /// <param name="cc">The comparison tool</param>
        /*
        void Log(Comparison cc)
        {
            m_Key.Log(cc);
        }
         */
    }
}
