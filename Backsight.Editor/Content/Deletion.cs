using System;

namespace Backsight.Editor.Content
{
    public class Deletion : Edit
    {
        public Id[] Deletions;

        public override void WriteAttributes(ContentWriter writer)
        {
        }

        public override void ReadAttributes(ContentReader reader)
        {
        }

        public override void WriteChildElements(ContentWriter writer)
        {
            writer.WriteIdArray("Id", Deletions);
        }

        public override void ReadChildElements(ContentReader reader)
        {
            Deletions = reader.ReadIdArray("Id");
        }
    }
}
