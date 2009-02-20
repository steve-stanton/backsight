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
using System.ComponentModel;
using System.Drawing.Design;

namespace Backsight.Forms
{
    /// <summary>
    /// Most of the classes here were taken from a download described in
    /// http://www.codeproject.com/cs/miscctrl/Dynamic_Propertygrid.asp.
    /// I've changed the class naming, tweaked it a bit, and added a
    /// few extra options. Apart from that, it's fairly basic.
    /// </summary>
    public class AdhocProperty
    {
		private readonly string sName;
		private bool bReadOnly;
		private readonly bool bVisible;
		private object objValue = null;
        private String m_DisplayName;
        private String m_Description;
        private String m_Category;
        private TypeConverter m_Converter;
        private UITypeEditor m_Editor;

        /// <summary>
        /// Creates a property that's not readonly, and which is visible
        /// </summary>
        /// <param name="sName">The property name</param>
        /// <param name="value">The initial property value</param>
        public AdhocProperty(string sName, object value)
            : this(sName, value, false, true)
        {
        }

		public AdhocProperty(string sName, object value, bool bReadOnly, bool bVisible )
		{
			this.sName = sName;
			this.objValue = value;
			this.bReadOnly = bReadOnly;
			this.bVisible = bVisible;
            m_DisplayName = this.sName;
            m_Description = this.sName;
            m_Category = String.Empty;
            m_Converter = null;
            m_Editor = null;
		}

		public bool ReadOnly
		{
			get { return bReadOnly; }
            set { bReadOnly = value; }
		}

		public string Name
		{
			get { return sName; }
		}

        public String DisplayName
        {
            get { return m_DisplayName; }
            set { m_DisplayName = value; }
        }

        public String Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        public String Category
        {
            get { return m_Category; }
            set { m_Category = value; }
        }

        public TypeConverter Converter
        {
            get { return m_Converter; }
            set { m_Converter = value; }
        }

        public UITypeEditor Editor
        {
            get { return m_Editor; }
            set { m_Editor = value; }
        }

		public bool Visible
		{
			get { return bVisible; }
		}

		public object Value
		{
			get { return objValue; }
			set { objValue = value; }
		}
    }

    public class AdhocPropertyDescriptor : PropertyDescriptor
    {
        AdhocProperty m_Property;
        public AdhocPropertyDescriptor(ref AdhocProperty myProperty, Attribute[] attrs)
            : base(myProperty.Name, attrs)
        {
            m_Property = myProperty;
        }

        #region PropertyDescriptor specific

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override object GetValue(object component)
        {
            return m_Property.Value;
        }

        public override string Description
        {
            get { return m_Property.Description; }
        }

        public override string Category
        {
            get { return m_Property.Category; }
        }

        public override string DisplayName
        {
            get { return m_Property.DisplayName; }
        }

        public override bool IsReadOnly
        {
            get { return m_Property.ReadOnly; }
        }

        public override void ResetValue(object component)
        {
            //Have to implement
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override void SetValue(object component, object value)
        {
            m_Property.Value = value;
        }

        public override Type PropertyType
        {
            get { return m_Property.Value.GetType(); }
        }

        public override TypeConverter Converter
        {
            get
            {
                TypeConverter tc = m_Property.Converter;
                return (tc == null ? base.Converter : tc);
            }
        }

        #endregion
    }

    /// <summary>
    /// A list of properties (suitable for selecting into a <c>PropertyGrid</c>)
    /// </summary>
    public class AdhocPropertyList : List<AdhocProperty>, ICustomTypeDescriptor
    {
        public AdhocPropertyList(int capacity)
            : base(capacity)
        {
        }

        public void RemoveByPropertyName(String name)
        {
            this.RemoveAll
              (delegate(AdhocProperty p) { return (p.Name == name); });
        }

        #region TypeDescriptor Implementation
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptor[] newProps = new PropertyDescriptor[this.Count];
            for(int i=0; i<this.Count; i++)
            {
                AdhocProperty prop = this[i];
                newProps[i] = new AdhocPropertyDescriptor(ref prop, attributes);
            }

            return new PropertyDescriptorCollection(newProps);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion
    }
}
