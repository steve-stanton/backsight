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
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Text;
using System.IO;

using Backsight.Environment;
using Backsight.Data;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="30-JAN-2003" was="CeAttachPoint" />
    /// <summary>
    /// Operation to attach a point to a line.
    /// </summary>
    [Serializable]
    class AttachPointOperation : Operation, IXmlSerializable
    {
        /// <summary>
        /// The max value stored for <c>m_PositionRatio</c>
        /// </summary>
        const uint MAX_POSITION_RATIO = 1000000000;

        #region Class data

        /// <summary>
        /// The line the point should appear on 
        /// </summary>
        LineFeature m_Line;

        /// <summary>
        /// The position ratio of the attached point. A point coincident with the start
        /// of the line is a value of 0. A point at the end of the line is a value of
        /// 1 billion  (1,000,000,000).
        /// </summary>
        uint m_PositionRatio;

        /// <summary>
        /// The point that was created 
        /// </summary>
        PointFeature m_Point;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>AttachPointOperation</c> with null values for everything.
        /// </summary>
        internal AttachPointOperation()
        {
            m_Line = null;
            m_PositionRatio = 0;
            m_Point = null;
        }

        #endregion

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Attach point to line"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get { return new Feature[] { m_Point }; }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.AttachPoint; }
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            if (m_Line!=null)
                m_Line.AddOp(this);
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();
            Rollback(m_Point);
            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Re-calculate the position of the attached point & move it.
            IPosition xpos = Calculate();
            m_Point.Move(xpos);

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Calculates the position of the attached point.
        /// </summary>
        /// <returns></returns>
        IPosition Calculate()
        {
            Debug.Assert(m_PositionRatio <= MAX_POSITION_RATIO);

            // Get the current length of the line the point is attached to
            double len = m_Line.Length.Meters;

            // Get the distance to the attached point
            double dist = len * ((double)(m_PositionRatio)/(double)MAX_POSITION_RATIO);

            // Get the position for the point
            IPosition xpos;
            if (m_Line.LineGeometry.GetPosition(new Length(dist), out xpos))
                return xpos;

            throw new Exception("Unable to calculate position of attached point");
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="line">The line to attach the point to</param>
        /// <param name="posn">The position on the line at which to attach the point</param>
        /// <param name="type">The entity type for the point.</param>
        internal void Execute(LineFeature line, IPosition posn, IEntity type)
        {
            // Get the distance to the supplied position (confirming that it does fall on the line)
            LineGeometry g = line.LineGeometry;
            double lineLen = g.Length.Meters;
            double posnLen = g.GetLength(posn).Meters;
            if (posnLen < 0.0)
                throw new Exception("Position does not appear to coincide with line.");

            // Remember the line the point is getting attached to
            m_Line = line;

            // Express the position as a position ratio in the range [0,1 billion]
            double prat = posnLen/lineLen;
            m_PositionRatio = (uint)(prat * (double)MAX_POSITION_RATIO);
            Debug.Assert(m_PositionRatio>=0);
            Debug.Assert(m_PositionRatio<=MAX_POSITION_RATIO);

            // Add the point to the map.
            m_Point = MapModel.AddPoint(posn, type, this);

            // If necessary, assign the new point the next available ID.
            m_Point.SetNextId();

            WriteXml(); // TEST

            // Peform standard completion steps
            Complete();
        }

        /// <summary>
        /// The line the point should appear on 
        /// </summary>
        internal LineFeature Line
        {
            get { return m_Line; }
        }

        /// <summary>
        /// The point that was created (defined on a call to <see cref="Execute"/>)
        /// </summary>
        internal PointFeature NewPoint
        {
            get { return m_Point; }
        }
        /*
        internal void WriteXml()
        {
            XmlSerializer xs = new XmlSerializer(typeof(AttachPointOperation));
            using (System.IO.StreamWriter s = System.IO.File.CreateText(@"C:\Temp\Test.xml"))
            {
                xs.Serialize(s, this);
            }
        }
        */

        internal void WriteXml()
        {
            //IAttachPoint a = new AttachPointAdapter(this);
            //AttachPointData.WriteXml(a, @"C:\Temp\Test.xml");

            /*
            attachPoint edit = new attachPoint();
            edit.positionRatio = m_PositionRatio;
            edit.line = new dataHandle();
            edit.line.item = 1;
            edit.line.job = 123;
            edit.point = new dataHandle();
            edit.point.item = 2;
            edit.point.job = 44;

            XmlSerializer xs = new XmlSerializer(typeof(attachPoint));
            using (System.IO.StreamWriter s = System.IO.File.CreateText(@"C:\Temp\Test.xml"))
            {
                xs.Serialize(s, edit);
            }
             */
            using (StreamWriter sw = File.CreateText(@"C:\Temp\Test.xml"))
            {
                sw.Write(ToXml());
            }
            /*
            AttachPointTest edit = new AttachPointTest();
            edit.EditSequence = EditSequence;
            //edit.Line = m_Line;
            edit.PositionRatio = m_PositionRatio;
            //edit.Point = m_Point;
            //MapModel.WriteEdit(edit);

            XmlSerializer xs = new XmlSerializer(typeof(AttachPointTest));
            using (System.IO.StreamWriter s = System.IO.File.CreateText(@"C:\Temp\Test.xml"))
            {
                xs.Serialize(s, edit);
            }
             */
        }
        /*
        private const string ns = "http://www.backsight.org";

        public static XmlQualifiedName GetXmlSchema(XmlSchemaSet xs)
        {
            // This method is called by the framework to get the schema for this type.
            // We return an existing schema from disk.

            XmlSerializer schemaSerializer = new XmlSerializer(typeof(XmlSchema));
            string xsdPath = null;
            // NOTE: replace the string with your own path.
            xsdPath = System.Web.HttpContext.Current.Server.MapPath("EditingSchema.xsd");
            XmlSchema s = (XmlSchema)schemaSerializer.Deserialize(
                new XmlTextReader(xsdPath), null);
            xs.XmlResolver = new XmlUrlResolver();
            xs.Add(s);

            return new XmlQualifiedName("attachPoint", ns);
        }
        */

        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute("EditSequence");
            writer.WriteValue(EditSequence);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("PositionRatio");
            writer.WriteValue(m_PositionRatio);
            writer.WriteEndAttribute();

            //edit.Line = m_Line;
            //edit.Point = m_Point;
        }

        #endregion

        /// <summary>
        /// The position ratio of the attached point. A point coincident with the start
        /// of the line is a value of 0. A point at the end of the line is a value of
        /// 1 billion  (1,000,000,000).
        /// </summary>
        internal uint PositionRatio
        {
            get { return m_PositionRatio; }
        }

        internal string ToXml()
        {
            StringBuilder sb = new StringBuilder(200);
            XmlWriter xw = XmlWriter.Create(sb);

            /*
            writer.WriteStartAttribute("EditSequence");
            writer.WriteValue(EditSequence);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("PositionRatio");
            writer.WriteValue(m_PositionRatio);
            writer.WriteEndAttribute();
            */
            //xw.WriteStartElement("AttachPoint");
            xw.WriteQualifiedName("AttachPoint", "Backsight");
            xw.WriteAttributeString("Line", m_Line.DataId);
            xw.WriteAttributeString("PositionRatio", m_PositionRatio.ToString());
            xw.WriteElementString("Point", m_Point.XmlData());
            xw.WriteEndElement();

            return sb.ToString();
        }
    }
}
