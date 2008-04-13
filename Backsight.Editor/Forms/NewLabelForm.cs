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
using System.Windows.Forms;

using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdNewLabel" />
    /// <summary>
    /// Dialog that lets the user specify a polygon label
    /// </summary>
    partial class NewLabelForm : Form
    {
        #region Class data

        /// <summary>
        /// The schema for the polygon attribute data
        /// </summary>
        //Schema m_Schema;

        /// <summary>
        /// The template for the polygon attribute data
        /// </summary>
        //Template m_Template;

        /// <summary>
        /// Entity type for the polygons.
        /// </summary>
        IEntity m_PolygonType;

        /// <summary>
        /// True if all applicable schemas are listed.
        /// </summary>
        bool m_IsAllSchemas;

        /// <summary>
        /// True if adding labels that correspond to the polygon ID.
        /// </summary>
        bool m_IsUseId;

        /// <summary>
        /// True if no attributes.
        /// </summary>
        bool m_IsNoAttr;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>NewLabelForm</c>
        /// </summary>
        internal NewLabelForm()
        {
            InitializeComponent();

            /*
            m_Text = String.Empty;
            m_Entity = null;
            m_Label = label;

            // If we're doing an update, grab initial values.
            if (m_Label!=null)
            {
                m_Text = m_Label.TextGeometry.Text;
                m_Entity = m_Label.EntityType;
            }
             */
        }

        #endregion

        /*
	virtual CeEntity*			GetpEntity		( void ) const { return m_pPolygonType; }
	virtual	CeSchema*			GetpSchema		( void ) const { return m_pSchema; }
	virtual	CeTemplate*			GetpTemplate	( void ) const { return m_pTemplate; }
  
private:

	virtual	void				ListSchemas		( void );
	virtual	void				ListTemplates	( void );
	virtual void				UseId			( const LOGICAL isUseId=TRUE );
*/
    }
}