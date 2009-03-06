using System;

namespace Backsight.Editor.Content
{
    public class Id : IContentAttribute
    {
        InternalIdValue Value;

        public Id(string s)
        {
            Value = new InternalIdValue(s);
        }

        public string AttributeString
        {
            get { return Value.ToString(); }
        }
    }
}
