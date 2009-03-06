using System;
using System.Xml;

namespace Backsight.Editor.Content
{
    public class AttachPoint : Edit
    {
        public Id Line;
        public uint PositionRatio;
        public SpatialItem Point;

        public override void WriteAttributes(ContentWriter writer)
        {
            base.WriteAttributes(writer);
            writer.WriteString("Line", Line.AttributeString);
            writer.WriteUnsignedInt("PositionRatio", PositionRatio);
        }

        public override void WriteChildElements(ContentWriter writer)
        {
            base.WriteChildElements(writer);
            writer.WriteElement("Point", Point);
        }

        public override void ReadAttributes(ContentReader reader)
        {
            base.ReadAttributes(reader);
            Line = new Id(reader.ReadString("Line"));
            PositionRatio = reader.ReadUnsignedInt("PositionRatio");
        }

        public override void ReadChildElements(ContentReader reader)
        {
            base.ReadChildElements(reader);
            Point = reader.ReadElement<SpatialItem>("Point");
        }
    }
}
