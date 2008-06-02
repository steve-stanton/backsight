using System;
using System.Xml;
using Backsight;
using Backsight.Editor;

namespace TestX
{
    class MySegClass : MyAbClass
    {
        public int Start;
        public int End;

        internal MySegClass()
        {
        }

        public MySegClass(XmlContentReader reader)
        {
            ReadContent(reader);
        }

        public override string ToString()
        {
            return String.Format("AbValue={0} Start={1} End={2}", AbValue, Start, End);
        }

        public override void WriteContent(XmlContentWriter writer)
        {
            base.WriteContent(writer);
            writer.WriteInt("Start", Start);
            writer.WriteInt("End", End);
        }

        public override void ReadContent(XmlContentReader reader)
        {
            base.ReadContent(reader);
            Start = reader.ReadInt("Start");
            End = reader.ReadInt("End");
        }
    }
}
