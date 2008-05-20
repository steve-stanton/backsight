using System;
using System.Collections.Generic;
using System.Text;

namespace TestX
{
    class Second : Base
    {
        public int Id;
        public string Name;

        internal override string TestType
        {
            get { return "The Second Type"; }
        }

        internal override string ToXml()
        {
            return String.Format(
            "<?xml version=\"1.0\"?> <Second xmlns=\"TestSpace\" Id=\"{0}\" Name=\"{1}\"/>",
            Id, Name);
        }
    }
}
