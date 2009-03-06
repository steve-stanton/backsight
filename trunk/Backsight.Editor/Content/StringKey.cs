using System;

namespace Backsight.Editor.Content
{
    public struct StringKey : IContentAttribute
    {
        public string Value;

        public StringKey(string value)
        {
            Value = value;
        }

        public string AttributeString
        {
            get { return Value; }
        }
    }
}
