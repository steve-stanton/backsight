using System;

namespace Backsight.Editor.Content
{
    public abstract class Edit : IContentElement
    {
        /// <summary>
        /// The internal ID for this edit (provides the session ID and the sequence number
        /// of the edit within that session)
        /// </summary>
        public Id EditId;

        public virtual void WriteAttributes(ContentWriter writer)
        {
            writer.WriteString("Id", EditId.AttributeString);
        }

        public virtual void ReadAttributes(ContentReader reader)
        {
            EditId = new Id(reader.ReadString("Id"));
        }

        public virtual void WriteChildElements(ContentWriter writer)
        {
        }

        public virtual void ReadChildElements(ContentReader reader)
        {
        }
    }
}
