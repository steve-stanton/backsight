// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using Backsight.Editor.Xml;

namespace Backsight.Editor
{
    public class XmlContentReader
    {
        #region Class data

        /// <summary>
        /// The model that's being loaded.
        /// </summary>
        readonly CadastralMapModel m_Model;

        /// <summary>
        /// The object that actually does the reading.
        /// </summary>
        XmlReader m_Reader;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>XmlContentReader</c> that wraps the supplied
        /// reading tool.
        /// </summary>
        /// <param name="model">The model to load</param>
        /// <param name="numItem">The estimated number of items that will be
        /// loaded (used to initialize an index of the loaded data)</param>
        internal XmlContentReader(CadastralMapModel model, uint numItem)
        {
            m_Model = model;
            m_Reader = null;
        }

        #endregion

        /// <summary>
        /// Loads the content of an editing operation. Prior to call, the current editing session
        /// must be defined using the <see cref="Session.CurrentSession"/> property.
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <param name="data">The data that describes the edit</param>
        /// <returns>The created editing object</returns>
        internal Operation ReadEdit(Session s, XmlReader data)
        {
            try
            {
                m_Model.IsLoading = true;
                m_Reader = data;

                XmlSerializer xs = new XmlSerializer(typeof(EditType));
                EditType et = (EditType)xs.Deserialize(data);
                Debug.Assert(et.Operation.Length==1);
                OperationType ot = et.Operation[0];
                Operation result = ot.LoadOperation(s);

                // Note that calculated geometry is NOT defined at this stage. That happens
                // when the model is asked to index the data.

                // Associate referenced features with the edit
                result.AddReferences();

                return result;
            }

            finally
            {
                m_Reader = null;
                m_Model.IsLoading = false;
            }
        }
    }
}
