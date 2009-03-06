using System;

namespace Backsight.Editor.Content
{
    public struct GroupKey : IContentAttribute
    {
        int GroupId;
        uint Value;

        public GroupKey(int groupId, uint value)
        {
            GroupId = groupId;
            Value = value;
        }

        public GroupKey(string s)
        {
            string[] tokens = s.Split('@');
            if (tokens.Length != 2)
                throw new ArgumentException();

            Value = uint.Parse(tokens[0]);
            GroupId = int.Parse(tokens[1]);
        }

        public string AttributeString
        {
            get { return String.Format("{0}@{1}", Value, GroupId); }
        }
    }
}
