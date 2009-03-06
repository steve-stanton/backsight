using System;

namespace Backsight.Editor.Content
{
    public class SpatialItem : IContentElement
    {
        public Id ItemId;
        public IContentAttribute Key;
        public int Type;

        //public FeatureInfo()
        //{
        //}

        public void WriteAttributes(ContentWriter writer)
        {
            writer.WriteString("Id", ItemId.AttributeString);
            writer.WriteInt("Type", Type);
            writer.WriteString("Key", Key.AttributeString);
        }

        public void WriteChildElements(ContentWriter writer)
        {
        }

        public void ReadAttributes(ContentReader reader)
        {
            ItemId = new Id(reader.ReadString("Id"));
            Type = reader.ReadInt("Type");

            string s = reader.ReadString("Key");
            if (s.Contains("@"))
                Key = new GroupKey(s);
            else
                Key = new StringKey(s);
        }

        public void ReadChildElements(ContentReader reader)
        {
        }
    }
}
