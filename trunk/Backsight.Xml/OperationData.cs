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
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Backsight.Xml
{
    /// <summary>
    /// Base class for serialization of any sort of edit.
    /// </summary>
    [XmlInclude(typeof(AttachPointData))]
    [XmlType(TypeName = "Operation", Namespace = "Backsight")]
    public abstract class OperationData
    {
        #region Static

        /// <summary>
        /// Obtains the class types derived from this class (in the current assembly).
        /// Ignores abstract classes.
        /// </summary>
        /// <returns>All concrete types in the class hierarchy (not just immediate sub-classes)</returns>
        static Type[] GetDerivedTypes()
        {
            return GetDerivedTypes<OperationData>();
        }

        /// <summary>
        /// Obtains types for classes derived from a specific class (in the current assembly).
        /// </summary>
        /// <typeparam name="T">The base class of interest</typeparam>
        /// <returns>All concrete types in the class hierarchy (not just immediate sub-classes)</returns>
        static Type[] GetDerivedTypes<T>()
        {
            List<Type> result = new List<Type>();

            Type baseType = typeof(T);
            foreach (Type t in baseType.Assembly.GetTypes())
            {
                if (t != null && t.IsSubclassOf(baseType) && !t.IsAbstract)
                    result.Add(t);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Obtains types that have been declared in this class with the <c>XmlIncludeAttribute</c>.
        /// This should correspond to the types returned by <see cref="GetDerivedTypes"/>.
        /// </summary>
        /// <returns>The types declared at the top of this class using
        /// the <c>XmlIncludeAttribute</c></returns>
        static Type[] GetIncludedTypes()
        {
            List<Type> result = new List<Type>();

            Type t = typeof(OperationData);
            object[] atts = t.GetCustomAttributes(false);
            foreach (object o in atts)
            {
                if (o is XmlIncludeAttribute)
                {
                    XmlIncludeAttribute xia = (XmlIncludeAttribute)o;
                    result.Add(xia.Type);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Confirms that all classes derived from this one have been correctly tagged
        /// with the <c>XmlIncludeAttribute</c>.
        /// </summary>
        /// <exception cref="Exception">If any derived class is not declared with
        /// the <c>XmlIncludeAttribute</c> (should be at the top of this class)</exception>
        static void CheckIncludedTypes()
        {
            Type[] derivedTypes = GetDerivedTypes();
            Type[] includedTypes = GetIncludedTypes();

            foreach (Type dt in derivedTypes)
            {
                if (Array.IndexOf<Type>(includedTypes, dt) < 0)
                    throw new Exception(String.Format("Class '{0}' is not defined in the OperationData class", dt.Name));
            }
        }

        #endregion
    }
}
