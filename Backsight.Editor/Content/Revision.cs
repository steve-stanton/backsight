using System;
using System.Xml;

namespace Backsight.Editor.Content
{
    /// <summary>
    /// A revision to an edit.
    /// </summary>
    public class Revision : Edit
    {
        /// <summary>
        /// The edit that was revised
        /// </summary>
        internal Id OriginalEdit;

        /// <summary>
        /// The revised edit
        /// </summary>
        public Edit RevisedEdit;

        //public Revision()
        //{
        //}

        public override void WriteAttributes(ContentWriter writer)
        {
            base.WriteAttributes(writer);
            writer.WriteString("Original", OriginalEdit.AttributeString);
        }

        public override void ReadAttributes(ContentReader reader)
        {
            base.ReadAttributes(reader);
            OriginalEdit = new Id(reader.ReadString("Original"));
        }

        public override void WriteChildElements(ContentWriter writer)
        {
            base.WriteChildElements(writer);
            writer.WriteElement("Revision", RevisedEdit);
        }

        public override void ReadChildElements(ContentReader reader)
        {
            base.ReadChildElements(reader);
            RevisedEdit = reader.ReadElement<Edit>("Revision");
        }
    }
}
