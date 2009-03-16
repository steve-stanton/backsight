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
using System.Collections.Generic;
using System.Reflection;

using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <summary>
    /// A mapping that relates XML type names to the corresponding constructors (for use
    /// during deserialization using the <see cref="XmlContentReader"/> class).
    /// </summary>
    static class ContentMapping
    {
        #region Data

        /// <summary>
        /// Cross-reference of type names to the corresponding constructor.
        /// </summary>
        static Dictionary<string, ConstructorInfo> s_Mapping;

        #endregion

        /// <summary>
        /// Loads all "well-known" type mappings.
        /// </summary>
        static void LoadMappings()
        {
            s_Mapping = new Dictionary<string,ConstructorInfo>(20);

            AddMapping(new AttachPointOperation());
            AddMapping(new ImportOperation());
            AddMapping(new GetControlOperation());

            AddMapping("PointType", typeof(PointFeature));

            AddMapping("LineType", typeof(LineFeature));
            AddMapping("ArcType", typeof(LineFeature));
            AddMapping("MultiSegmentType", typeof(LineFeature));
            AddMapping("SectionType", typeof(LineFeature));

            AddMapping("TextType", typeof(TextFeature));
            AddMapping("MiscTextType", typeof(TextFeature));
            AddMapping("RowTextType", typeof(TextFeature));
        }

        static void AddMapping(Content c)
        {
            s_Mapping.Add(c.XmlTypeName, c.GetType().GetConstructor(Type.EmptyTypes));
        }

        static void AddMapping(string xmlTypeName, Type t)
        {
            s_Mapping.Add(xmlTypeName, t.GetConstructor(Type.EmptyTypes));
        }

        /// <summary>
        /// Creates 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlTypeName"></param>
        /// <returns></returns>
        internal static T GetInstance<T>(string xmlTypeName) where T : Content
        {
            if (s_Mapping == null)
                LoadMappings();

            // If we haven't previously encountered the type, look up a default constructor
            if (!s_Mapping.ContainsKey(xmlTypeName))
            {
                Type t = FindType(xmlTypeName);
                if (t == null)
                    throw new Exception("Cannot create object with xml type: " + xmlTypeName);

                // Confirm that the type extends Content
                if (!IsDerivedFromContent(t))
                    throw new Exception("Type does not extend Content class: " + t.FullName);

                // Locate default constructor
                ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
                if (ci == null)
                    throw new Exception("Cannot locate default constructor for type: " + t.FullName);

                s_Mapping.Add(xmlTypeName, ci);
            }

            // Create the instance
            ConstructorInfo c = s_Mapping[xmlTypeName];
            return (T)c.Invoke(new object[0]);
        }

        /// <summary>
        /// Checks whether the supplied type is derived from the <see cref="Content"/> class.
        /// </summary>
        /// <param name="t">The type to check</param>
        /// <returns>True if the supplied type is derived from <c>Content</c></returns>
        static bool IsDerivedFromContent(Type t)
        {
            while (t != null)
            {
                if (t is Content)
                    return true;

                t = t.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Attempts to locate the <c>Type</c> corresponding to the supplied type name
        /// </summary>
        /// <param name="typeName">The name of the type (unqualified with any assembly stuff)</param>
        /// <returns>The correspond class type (null if not found in application domain)</returns>
        /// <remarks>This stuff is in pretty murky territory for me. I would guess it's possible
        /// that the same type name could appear in more than one assembly, so there could be
        /// some ambiguity.</remarks>
        static Type FindType(string typeName)
        {
            // The type is most likely part of the assembly that holds the Content class.
            // Note: I thought it might be sufficient to just call Type.GetType("Backsight.Editor."+typeName),
            // but that doesn't find Operation classes (it only works if you specify the sub-folder name too).
            Type result = FindType(typeof(Content).Assembly, typeName);
            if (result != null)
                return result;

            // If things get moved about though, it's a bit more complicated...
            Assembly[] aa = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in aa)
            {
                result = FindType(a, typeName);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Searches the supplied assembly for a <c>Type</c> corresponding to the
        /// supplied type name
        /// </summary>
        /// <param name="a">The assembly to search</param>
        /// <param name="typeName">The name of the type (unqualified with any assembly stuff)</param>
        /// <returns>The corresponding type (null if not found)</returns>
        static Type FindType(Assembly a, string typeName)
        {
            foreach (Type type in a.GetTypes())
            {
                if (type.Name == typeName)
                    return type;
            }

            return null;
        }
    }
}
