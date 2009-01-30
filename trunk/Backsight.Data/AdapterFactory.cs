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
using System.Data.SqlClient;
using System.Reflection;

namespace Backsight.Data
{
	/// <written by="Steve Stanton" on="10-NOV-2006" />
    /// <summary>
    /// Creates database adapters that are associated with the database connection 
    /// defined through the <see cref="ConnectionFactory"/> class.
    /// </summary>
    public static class AdapterFactory
    {
        /// <summary>
        /// Creates a new adapter and associates it with a database connection (<see>GetSqlConnection</see>)
        /// </summary>
        /// <typeparam name="T">The type of adapter to create</typeparam>
        /// <returns>The newly created adapter</returns>
        public static T Create<T>() where T : new()
        {
            return Create<T>(ConnectionFactory.Create().Value);
        }

        /// <summary>
        /// Creates an adapter that refers to a specific connection
        /// </summary>
        /// <typeparam name="T">The type of adapter to create</typeparam>
        /// <param name="c">The connection to associate the adapter with</param>
        /// <returns>The newly created adapter</returns>
        /// <exception cref="ArgumentException">If the supplied adapter type doesn't have a "Connection" property</exception>
        private static T Create<T>(SqlConnection c) where T : new()
        {
            // By default, the connection property in generated code is internal, not public, so you
            // need to search using BindingFlags.NonPublic. The mention of BindingFlags.Public is there
            // just in case the property has been tweaked to be public (since searching for non-public
            // properties doesn't locate the public ones!).

            T a = new T();
            PropertyInfo pi = a.GetType().GetProperty("Connection", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (pi==null)
                throw new ArgumentException("Adapter does not have a 'Connection' property");

            pi.SetValue(a, c, null);
            return a;
        }
    }
}
