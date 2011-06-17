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
            mapModel.Index.QueryWindow(null, SpatialType.Line, WriteLine);
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
            p.Location = new Vector3d(pg.X, pg.Y, 0.0);
            p.Layer = m_Layer;

            m_Dxf.AddEntity(p);
            return true;
        }

        bool WriteLine(ISpatialObject item)
        {
            LineFeature line = (LineFeature)item;
            WriteLineGeometry(line.LineGeometry);
            return true;
        }

        void WriteLineGeometry(LineGeometry geom)
        {
            if (geom is SegmentGeometry)
                WriteSegment((SegmentGeometry)geom);
            else if (geom is ArcGeometry)
                WriteArc((ArcGeometry)geom);
            else if (geom is MultiSegmentGeometry)
                WriteMultiSegment((MultiSegmentGeometry)geom);
            else if (geom is SectionGeometry)
                WriteSection((SectionGeometry)geom);
            else
                throw new NotImplementedException("Unexpected line geometry type: " + geom.GetType().Name);
        }

        void WriteSegment(SegmentGeometry line)
        {
            Line acLine = new Line();
            IPointGeometry start = line.Start;
            acLine.StartPoint = new Vector3d(start.X, start.Y, 0.0);
            IPointGeometry end = line.End;
            acLine.EndPoint = new Vector3d(end.X, end.Y, 0.0);
            acLine.Layer = m_Layer;
            m_Dxf.AddEntity(acLine);
        }

        void WriteArc(ArcGeometry line)
        {
            IPointGeometry center = line.Circle.Center;
            double bcAngle = GetAngle(center, line.BC);
            double ecAngle = GetAngle(center, line.EC);

            Arc acLine = new Arc();
            acLine.Center = new Vector3d(center.X, center.Y, 0.0);
            acLine.Radius = line.Circle.Radius;

            // AutoCad arcs are *always* drawn counter-clockwise
            if (line.IsClockwise)
            {
                acLine.StartAngle = ecAngle;
                acLine.EndAngle = bcAngle;
            }
            else
            {
                acLine.StartAngle = bcAngle;
                acLine.EndAngle = ecAngle;
            }

            acLine.Layer = m_Layer;
            m_Dxf.AddEntity(acLine);
        }

        double GetAngle(IPointGeometry center, IPointGeometry endPoint)
        {
            double ex = endPoint.X - center.X;
            double ey = endPoint.Y - center.Y;

            if (ex == 0.0 && ey == 0.0)
                return 0.0;
            else
                return Math.Atan2(ey, ex) * MathConstants.RADTODEG;
        }

        void WriteMultiSegment(MultiSegmentGeometry line)
        {
            IPointGeometry[] pts = line.Data;
            List<PolylineVertex> acVertexList = new List<PolylineVertex>(pts.Length);
            foreach (IPointGeometry p in pts)
                acVertexList.Add(new PolylineVertex((float)p.X, (float)p.Y));

            Polyline acLine = new Polyline(acVertexList);
            acLine.Layer = m_Layer;
            m_Dxf.AddEntity(acLine);
        }

        void WriteSection(SectionGeometry line)
        {
            WriteLineGeometry(line.Make());
        }
    }
}
