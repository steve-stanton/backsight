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
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;

using Backsight.Environment;

namespace Backsight.Editor
{
    /// <summary>
    /// Fronts an instance of some object that implements an interface that extends
    /// <c>IEnvironmentItem</c>. Whereas the facade is considered to be something
    /// that needs to be persisted, the associated object is not.
    /// <para>
    /// The facade does not know how to obtain the associated object. Upon re-retrieval
    /// of a facade from wherever it is stored, the associated object must be re-defined
    /// by higher-level application logic (if you don't do this, the properties exposed
    /// by the facade will correspond to default values).
    /// </para></summary>
    /// <typeparam name="D">The type for the associated object</typeparam>
    class EnvironmentItemFacade<D> : DataStub where D : IEnvironmentItem
    {
        /// <summary>
        /// Utility method for searching a list for a specific ID
        /// </summary>
        /// <param name="list">The list to search</param>
        /// <param name="id">The ID to look for</param>
        /// <returns>The first matching item in the list (null if not found)</returns>
        internal static D FindById(IList<D> list, int id)
        {
            foreach (D d in list)
            {
                if (d.Id == id)
                    return d;
            }

            return default(D);
        }

        #region Class data

        /// <summary>
        /// The object to delegate to. Must be set using the <c>Data</c> property
        /// before working with the facade (otherwise results will correspond to
        /// default values).
        /// </summary>
        [NonSerialized]
        private D m_Data;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a brand new <c>SchemaItemFacade</c> that corresponds to some
        /// object retrieved from the schema database.
        /// </summary>
        /// <param name="data">The object this facade will delegate to.</param>
        /// <returns>A new (persistent) facade that fronts the supplied object.</returns>
        /// <exception cref="ArgumentNullException">If the delegate is null, or has
        /// an ID of zero.</exception>
        protected EnvironmentItemFacade(D data)
            : base(data.Id)
        {
            if (data==null)
                throw new ArgumentNullException();

            m_Data = data;

            // If we've been supplied a facade, drill down to get to the real data.
            while (m_Data is EnvironmentItemFacade<D>)
            {
                m_Data = (m_Data as EnvironmentItemFacade<D>).Data;
            }
        }

        #endregion

        /// <summary>
        /// The object to delegate to. If you don't set the property before working with
        /// this facade, all other properties will have their default values.
        /// </summary>
        [Browsable(false)]
        public D Data
        {
            get { return m_Data; }
            set { m_Data = value; }
        }
    }
}
