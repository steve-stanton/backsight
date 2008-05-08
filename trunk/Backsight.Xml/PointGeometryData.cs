using System;
using System.Xml.Serialization;

namespace Backsight.Xml
{
    /// <summary>
    /// The geometry for a 2D point, expressed in microns in some unspecified projected
    /// coordinate system.
    /// </summary>
    [XmlRoot("PointGeometry")]
    public class PointGeometryData
    {
        [XmlAttribute]
        public ulong X;

        [XmlAttribute]
        public ulong Y;
    }
}
