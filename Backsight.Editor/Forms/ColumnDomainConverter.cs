// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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
using System.ComponentModel;

using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="20-FEB-2009"/>
    /// <summary>
    /// A type converter that may be used to provide drop-down property support
    /// for an <see cref="IColumnDomain"/> (when applied to an item in a <c>PropertyGrid</c>)
    /// </summary>
    class ColumnDomainConverter : StringConverter
    {
        #region Class data

        /// <summary>
        /// The column domain that is being converted
        /// </summary>
        readonly IColumnDomain m_ColumnDomain;

        /// <summary>
        /// The values that will be returned by a call to the <see cref="GetStandardValues"/> override.
        /// </summary>
        readonly StandardValuesCollection m_Values;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDomainConverter"/> class.
        /// </summary>
        /// <param name="cd">The column domain that is being converted (not null)</param>
        public ColumnDomainConverter(IColumnDomain cd)
        {
            if (cd == null)
                throw new ArgumentNullException();

            m_ColumnDomain = cd;
            IDomainTable dt = cd.Domain;
            string[] lookups = dt.LookupValues;
            m_Values = new StandardValuesCollection(lookups);
        }

        #endregion

        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked from a list,
        /// using the specified context.
        /// </summary>
        /// <param name="context">The formatting context</param>
        /// <returns>
        /// True (always), meaning that the <see cref="GetStandardValues"/> override will be used to obtain
        /// domain values.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is designed for when
        /// provided with a format context.
        /// </summary>
        /// <param name="context">The formatting context</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"></see> that holds a standard
        /// set of valid values.
        /// </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return m_Values;
        }

        /// <summary>
        /// Performs a lookup on the domain table associated with this converter
        /// </summary>
        /// <param name="shortValue">The lookup code to use (expected to be one of the values selected
        /// from the set of values returned by <see cref="GetStandardValues"/></param>
        /// <returns>The expanded value for the lookup (blank if not found)</returns>
        public string Lookup(string shortValue)
        {
            return m_ColumnDomain.Domain.Lookup(shortValue);
        }

        /// <summary>
        /// The column domain that is being converted
        /// </summary>
        public IColumnDomain ColumnDomain
        {
            get { return m_ColumnDomain; }
        }
    }
}
