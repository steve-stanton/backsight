using System;
using System.Collections.Generic;
using System.Text;

namespace TestX
{
    class First : Base
    {
        public int Id;
        public string Name;

        internal override string TestType
        {
            get { return "The First Type"; }
        }

        internal override string ToXml()
        {
            return String.Format(
            "<?xml version=\"1.0\"?> <First xmlns=\"TestSpace\" Id=\"{0}\" Name=\"{1}\"/>",
            Id, Name);
        }
    }
}
