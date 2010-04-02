using System;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// Factory for generating XML
    /// </summary>
    class DataFactory
    {
        internal string ToXml<T>(T op) where T : Operation
        {
            string typeName = op.GetType().Name;
            const string TAIL = "Operation";
            if (!typeName.EndsWith(TAIL))
                throw new NotSupportedException();

            typeName = "Backsight.Editor.Xml." + typeName.Substring(0, typeName.Length - TAIL.Length) + "Data";
            //System.Windows.Forms.MessageBox.Show(typeName);

            Assembly a = Assembly.GetExecutingAssembly();
            Type t = a.GetType(typeName, true);

            ConstructorInfo ci = t.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null,
                new Type[] { op.GetType() }, null);

            if (ci == null)
                throw new NotImplementedException();

            OperationData sed = (OperationData)ci.Invoke(new object[] { op });

            StringBuilder sb = new StringBuilder(1000);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(sb, xws))
            {
                // Wrap the serializable edit in an EditType object (means we always know what to
                // cast the result to upon deserialization)

                EditData e = new EditData();
                e.Operation = new OperationData[] { sed };
                XmlSerializer xs = new XmlSerializer(typeof(EditData));
                xs.Serialize(writer, e);
            }

            System.Windows.Forms.MessageBox.Show(sb.ToString());
            return sb.ToString();
        }
    }
}
