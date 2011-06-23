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

        /// <summary>
        /// The file spec for the AutoCad file.
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// The AutoCad version to create.
        /// </summary>
        internal string Version { get; set; }

        /// <summary>
        /// Entity translation file to use (blank if none).
        /// </summary>
        internal string EntityTranslationFileName { get; set; }

        /// <summary>
        /// Exporting just topological stuff?
        /// </summary>
        internal bool IsTopological { get; set; }

        /// <summary>
        /// Tolerance for approximating arcs (a value of 0.0 means that arcs should
        /// NOT be approximated).
        /// </summary>
        internal double ArcTolerance { get; set; }

        //bool TranslateColors = true;

        DxfDocument m_Dxf;
        Layer m_Layer;

        #endregion

        #region Constructors

        internal DxfWriter()
        {
            this.FileName = null;
            this.Version = null;
            this.EntityTranslationFileName = null;
            this.IsTopological = true;
            this.ArcTolerance = 0.001;
        }

        #endregion

        internal void WriteFile()
        {
            if (String.IsNullOrEmpty(this.FileName))
                throw new InvalidOperationException("Output file name has not been specified");

            DxfVersion v = DxfVersion.AutoCad2007;
            if (!String.IsNullOrEmpty(this.Version))
            {
                if (this.Version == "2007")
                    v = DxfVersion.AutoCad2007;
                else if (this.Version == "2004")
                    v = DxfVersion.AutoCad2004;
                else if (this.Version == "2000")
                    v = DxfVersion.AutoCad2000;
                else if (this.Version == "12")
                    v = DxfVersion.AutoCad12;
                else
                    throw new InvalidOperationException("Unsupported AutoCad version: " + this.Version);
            }

            // Create output model and pick up some stock items
            m_Dxf = new DxfDocument();

            //m_ContinuousLineType = m_Dxf
            //m_ModelSpace = m_Dxf
            m_Layer = new Layer("TestLayer");

            CadastralMapModel mapModel = CadastralMapModel.Current;
            mapModel.Index.QueryWindow(null, SpatialType.Point, WritePoint);
            mapModel.Index.QueryWindow(null, SpatialType.Line, WriteLine);
            mapModel.Index.QueryWindow(null, SpatialType.Text, WriteText);
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

            m_Dxf.Save(this.FileName, v);
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

        Vector3d GetVector(IPointGeometry p)
        {
            return new Vector3d(p.X, p.Y, 0.0);
        }

        bool WriteText(ISpatialObject item)
        {
            TextFeature text = (item as TextFeature);
            TextGeometry geom = text.TextGeometry;
            Text acText = new Text(geom.Text, GetVector(geom.Position), geom.Height);
            acText.Rotation = (float)geom.Rotation.Degrees;
            acText.Layer = m_Layer;

            m_Dxf.AddEntity(acText);
            return true;
        }
    }
}
