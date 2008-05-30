using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using Backsight;

namespace TestX
{
    class First : Base
    {
        public int Id;
        public string Name;
        public MyAbClass More1;
        public MyAbClass More2;

        internal First()
        {
        }

        public First(XmlContentReader reader)
        {
            ReadContent(reader);
        }

        public override string ToString()
        {
            string m1 = (More1==null ? String.Empty : More1.ToString());
            string m2 = (More2==null ? String.Empty : More2.ToString());

            return String.Format("ID={0}, Name={1}: M1=[{2}], M2=[{3}]", Id, Name, m1, m2);
        }

        public override void WriteContent(Backsight.XmlContentWriter writer)
        {
            writer.WriteInt("Id", Id);
            writer.WriteString("Name", Name);
            writer.WriteElement("More1", More1);
            writer.WriteElement("More2", More2);
        }

        public override void ReadContent(XmlContentReader reader)
        {
            Id = reader.ReadInt("Id");
            Name = reader.ReadString("Name");
            More1 = (MyAbClass)reader.ReadElement("More1");
            More2 = (MyAbClass)reader.ReadElement("More2");
        }
    }
}
