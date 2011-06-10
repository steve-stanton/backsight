using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using netDxf;
using netDxf.Tables;
using netDxf.Header;
using netDxf.Entities;

namespace Backsight.Editor
{
    class DxfWriter
    {
        #region Class data

        DxfDocument m_Dxf;
        Layer m_Layer;

        #endregion

        #region Constructors

        internal DxfWriter()
        {
        }

        #endregion

        internal void WriteFile(string path)
        {
            m_Dxf = new DxfDocument();
            m_Layer = new Layer("TestLayer");

            CadastralMapModel mapModel = CadastralMapModel.Current;
            mapModel.Index.QueryWindow(null, SpatialType.Point, WritePoint);
            /*
            LineType cont = LineType.Continuous;
            if (cont == null)
                MessageBox.Show("no line type");
            else
                MessageBox.Show("LineType.Name=" + cont.Name);

            dxf.AddEntity(cont);
            //dxf.AddEntity(cont);
             */
            /*
            LineType lineType = dxf.GetLineType("Steve");
            if (lineType == null)
                MessageBox.Show("cannot get continuous line type");
            else
                MessageBox.Show("ok");
             */

            m_Dxf.Save(path, DxfVersion.AutoCad2007);
        }

        bool WritePoint(ISpatialObject item)
        {
            PointFeature pf = (PointFeature)item;
            IPointGeometry pg = pf.Geometry;

            Point p = new Point();
            p.Location = new Vector3f((float)pg.X, (float)pg.Y, 0.0f);
            p.Layer = m_Layer;

            m_Dxf.AddEntity(p);
            return true;
        }
    }
}
