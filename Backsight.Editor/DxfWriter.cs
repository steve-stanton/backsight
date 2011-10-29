using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using netDxf;
using netDxf.Tables;
using netDxf.Header;
using netDxf.Entities;
using Backsight.Editor.AutoCad;

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
        /// The AutoCad layers
        /// </summary>
        AcLayerEntitySet m_Layers;

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

        /// <summary>
        /// AutoCad line type for a solid (continuous) line.
        /// </summary>
        LineType m_ContinuousLineType;

        DxfDocument m_Dxf;
        Layer m_Layer;

        /// <summary>
        /// Should observed angles be written?
        /// </summary>
        bool m_ShowAngles;

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

            // Form entity->AutoCad layer index
            m_Layers = new AcLayerEntitySet();

            // If we have an entity translation file, adjust the AutoCad layer
            // names to correspond to the translations. If an entity type has
            // no translation, the output layer name will be the same as the
            // name of the entity type.
            if (!String.IsNullOrEmpty(this.EntityTranslationFileName))
                m_Layers.TranslateLayerNames(this.EntityTranslationFileName);

            // Get info for exporting fonts.
            /*
	CeFontFile ffile;
	if ( !ffile.Create() )
		ShowMessage("Cannot load font file.\nText will be exported without font information.");
             */

            // Are line annotations relevant? If so, get the font for them (assuming
            // the font is ALWAYS Arial).
            /*
	AD_SHPTB* pAnnoStyle = 0;
	UINT4 annoflags = 0;
	if ( !isTopological ) annoflags = pMap->GetLineAnnoFlags();
	if ( annoflags ) pAnnoStyle = ffile.GetShapeFile(DwgHandle,"Arial");
             */

            // Create output model and pick up some stock items
            m_Dxf = new DxfDocument();

            m_ContinuousLineType = LineType.Continuous;
            //m_ModelSpace = m_Dxf

            CadastralMapModel mapModel = CadastralMapModel.Current;

            // Convert points if doing a non-topological export
            if (!this.IsTopological)
            {
                // Will we be writing out observed angles?
                //const LOGICAL showAngles = ((annoflags & SHOW_OBSV_ANGLE) != 0);

                mapModel.Index.QueryWindow(null, SpatialType.Point, WritePoint);
            }

            // Convert polygon labels (or misc text)
            mapModel.Index.QueryWindow(null, SpatialType.Text, WriteText);

            mapModel.Index.QueryWindow(null, SpatialType.Line, WriteLine);

            m_Dxf.Save(this.FileName, v);
        }

        bool WritePoint(ISpatialObject item)
        {
            PointFeature pf = (PointFeature)item;

            // Skip if the AutoCad layer cannot be determined
            Layer layer = m_Layers.GetLayer(pf, m_ContinuousLineType, false);
            if (layer != null)
            {
                Point p = new Point();
                p.Location = GetVector(pf.Geometry);
                p.Layer = layer;

                m_Dxf.AddEntity(p);

                if (m_ShowAngles)
                    ExportAngles(pf);

            }

            return true;
        }

        /// <summary>
        /// Exports any angle observations associated with a point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>True if annotation was written.</returns>
        bool ExportAngles(PointFeature p)
        {
            // Skip if there is no AutoCad layer for the specified point
            // (the annotations will be going onto the same layer).
            Layer layer = m_Layers.GetLayer(p, m_ContinuousLineType, true);
            if (layer == null)
                return false;

            throw new NotImplementedException("DxfWriter.ExportAngles");

            // Get the point to create a set of miscellaneous text objects.
            //MiscTextGeometry[] text = p.CreateAngleText(

            //return true;
        }

        /*

//
//	@parm	AutoCad entity.
//	@parm	Cross-reference of CED entity types to AutoCad layers.
//	@parm	The style for the annotations.
//	@parm	The point involved.
//
//	@rdesc	TRUE if annotation was written.
LOGICAL CeAutoCad::ExportAngles ( const AD_DB_HANDLE DwgHandle
								, AD_VMADDR pEntityList
								, PAD_ENT_HDR pEntityHeader
								, PAD_ENT pEntity
								, CacadLayerEntitySet* pLESet
								, AD_SHPTB* pStyle
								, const CePoint& pt ) const {

	// Skip if there is no AutoCad layer for the specified point
	// (the annotations will be going onto the same layer).
	AD_OBJHANDLE* pLayerHandle = pLESet->GetpLayerHandle(&pt,
		m_ContinuousLineTypeHandle,TRUE);
	if ( !pLayerHandle ) return FALSE;

	// Get the point to create a set of miscellaneous text
	// objects.
	CPtrList text;
	UINT4 nAngle = pt.CreateAngleText(text,FALSE);
	if ( nAngle==0 ) return FALSE;

	// Export each item of text (deleting each item as we go).
	POSITION pos = text.GetHeadPosition();
	while ( pos ) {
		CeMiscText* pText = (CeMiscText*)text.GetNext(pos);

		// What's the actual text string?
		CString text;
		pText->GetText(text);

		// If the text position is to the west of the point,
		// align it on the right.
		FLOAT8 xt = pText->GetEasting();
		FLOAT8 yt = pText->GetNorthing();
		INT4 halign = AD_TEXT_JUST_LEFT;

		// Take special care with vertical text (allow up to 1mm
		// of roundoff).
		if ( (xt+0.001) < pt.GetEasting() ) halign = AD_TEXT_JUST_RIGHT;

		// Pull out values for function call below.
		CeVertex posn(xt,yt);
		FLOAT8 grheight = pText->GetHeight();
		FLOAT8 rotation = pText->GetRotation();

		CacadText AcadText(DwgHandle,pEntityList,pEntityHeader,pEntity);
		AcadText.ExportAngleDistText(pStyle,pLayerHandle,pText,text,posn
									,grheight,rotation
									,halign,AD_TEXT_VALIGN_MIDDLE);

		delete pText;
	}

	// Ensure pointer to the text have been removed too.
	text.RemoveAll();

	return TRUE;
*/

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

            if (this.IsTopological)
            {
                ExportKey(text);

            }
            else
            {
            }

            // Skip if the AutoCad layer cannot be determined
            Layer layer = m_Layers.GetLayer(text, m_ContinuousLineType, this.IsTopological);
            if (layer != null)
            {
                TextGeometry geom = text.TextGeometry;
                Text acText = new Text(geom.Text, GetVector(geom.Position), geom.Height);
                acText.Rotation = (float)geom.Rotation.Degrees;
                acText.Layer = layer;

                m_Dxf.AddEntity(acText);
            }
            return true;
        }

        void ExportKey(TextFeature text)
        {
            Layer layer = m_Layers.GetLayer(text, m_ContinuousLineType, true);
            if (layer == null)
                return;

            // Get the label's key string. It HAS to be defined.
            string keystr = text.FormattedKey;
            if (String.IsNullOrEmpty(keystr))
                return;

            // Get the label's reference position.
            IPointGeometry refpos = text.GetPolPosition();

            // Define the position for AutoCad (bottom right corner).


            TextGeometry geom = text.TextGeometry;
            Text acText = new Text(geom.Text, GetVector(geom.Position), geom.Height);
            acText.Rotation = (float)geom.Rotation.Degrees;
            acText.Layer = layer;

            m_Dxf.AddEntity(acText);
        }
    }
}
