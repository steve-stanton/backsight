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
using System.Data;
using System.Diagnostics;

using EnvData=Backsight.Data.BacksightDataSet;
using Backsight.Environment;

namespace Backsight.Data
{
    public abstract class EnvironmentData : IEnvironmentFactory
    {
        #region Class data

        /// <summary>
        /// The data for the Backsight environment
        /// </summary>
        private readonly EnvData m_Data;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>EnvironmentData</c> with nothing in it.
        /// </summary>
        protected EnvironmentData()
        {
            m_Data = new EnvData();
        }

        /// <summary>
        /// Creates a new <c>EnvironmentData</c> that refers to the supplied data.
        /// </summary>
        /// <param name="data">The data to use</param>
        protected EnvironmentData(EnvironmentData data)
        {
            m_Data = data.m_Data;
        }

        #endregion

        /*
        protected void Initialize()
        {
            m_Data.Clear();
            m_Data.AddInitialRows();
        }
        */

        public EnvData Data
        {
            get { return m_Data; }
        }

        public virtual int ReserveId()
        {
            EnvData.SysIdDataTable t = m_Data.SysId;
            Debug.Assert(t.Rows.Count==1);
            t[0].LastId++;
            return t[0].LastId;
        }

        public virtual bool ReleaseId(int id)
        {
            if (id<=0)
                throw new ArgumentOutOfRangeException();

            EnvData.SysIdDataTable t = m_Data.SysId;
            Debug.Assert(t.Rows.Count==1);

            if (t[0].LastId!=id)
                return false;

            t[0].LastId--;
            return true;
        }

        public int LastId
        {
            get
            {
                EnvData.SysIdDataTable tab = m_Data.SysId;
                return (tab.Rows.Count==0 ? 0 : tab[0].LastId);
            }
        }

        public IEditEntity CreateEntity()
        {
            EnvData.EntityRow row = EnvData.EntityRow.CreateEntityRow(m_Data);
            row.EntityId = ReserveId();
            return row;
        }

        public IEditFont CreateFont()
        {
            EnvData.FontRow row = EnvData.FontRow.CreateFontRow(m_Data);
            row.FontId = ReserveId();
            return row;
        }

        public IEditIdGroup CreateIdGroup()
        {
            EnvData.IdGroupRow row = EnvData.IdGroupRow.CreateIdGroupRow(m_Data);
            row.GroupId = ReserveId();
            return row;
        }

        public IEditLayer CreateLayer()
        {
            EnvData.LayerRow row = EnvData.LayerRow.CreateLayerRow(m_Data);
            row.LayerId = ReserveId();
            return row;
        }

        public IEditTheme CreateTheme()
        {
            EnvData.ThemeRow row = EnvData.ThemeRow.CreateThemeRow(m_Data);
            row.ThemeId = ReserveId();
            return row;
        }

        /// <summary>
        /// Implements <c>IEnvironmentContainer.Factory</c> on behalf of
        /// derived classes.
        /// </summary>
        public IEnvironmentFactory Factory
        {
            get { return this; }
        }

        /// <summary>
        /// Has the content of this container been modified since it was loaded?
        /// </summary>
        public bool IsModified
        {
            get { return m_Data.HasChanges(); }
        }

        /// <summary>
        /// Is this container empty?
        /// </summary>
        public bool IsEmpty
        {
            get { return (this.LastId==0); }
        }

        /// <summary>
        /// A name for this container (may be blank)
        /// </summary>
        public string Name
        {
            get { return m_Data.DataSetName; }
            set { m_Data.DataSetName = value; }
        }

        public IEntity[] EntityTypes
        {
            get { return (IEntity[])m_Data.Entity.Select(); }
        }

        public IFont[] Fonts
        {
            get { return (IFont[])m_Data.Font.Select(); }
        }

        public IIdGroup[] IdGroups
        {
            get { return (IIdGroup[])m_Data.IdGroup.Select(); }
        }

        public ILayer[] Layers
        {
            get { return (ILayer[])m_Data.Layer.Select(); }
        }

        public ITheme[] Themes
        {
            get { return (ITheme[])m_Data.Theme.Select(); }
        }

        public IProperty[] Properties
        {
            get { return (IProperty[])m_Data.Property.Select(); }
        }

        /*
        T[] Copy<T, F>(F[] rows) where F : T
        {
            T[] result = new T[rows.Length];

            for (int i=0; i<result.Length; i++)
            {
                result[i] = rows[i];
            }

            return result;
        }
        */

        /// <summary>
        /// Copies a list to another one where the elements are expressed as
        /// something that is implemented by the elements in the source list.
        /// 
        /// Not sure if this is really required. What I would really like to do
        /// is define a property something like this (but it isn't valid C#):
        /// 
        ///   IList<T> Schemas { get; } where T : ISchema
        /// 
        /// </summary>
        /// <typeparam name="T">The type for the elements in the returned list.</typeparam>
        /// <typeparam name="F">The type of elements in the source list (which implement
        /// interface T).</typeparam>
        /// <param name="list"></param>
        /// <returns>A copy of the supplied list, expressed in terms of T.</returns>
        /*
        private IList<T> Copy<T, F>(List<F> list) where F : T
        {
            IList<T> res = new List<T>(list.Count);
            foreach (F item in list)
            {
                res.Add(item);
            }
            return res;
        }
         */
    }
}
