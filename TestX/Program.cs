using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

using Backsight.Data;
using System.Data.SqlTypes;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;

namespace TestX
{
    class Program
    {
        static void Main(string[] args)
        {
            string constr = @"server='localhost\sqlexpress';Trusted_Connection=true;" +
                            @"multipleactiveresultsets=false;Initial Catalog=Foresight";
            AdapterFactory.ConnectionString = constr;

            string fTypeName, sTypeName;

            using (IConnection ic = AdapterFactory.GetConnection())
            {
                // Insert test objects into test table
                SqlConnection c = ic.Value;
                Console.WriteLine("connection open");

                SqlCommand cmd = new SqlCommand("DELETE FROM dbo.Test", c);
                cmd.ExecuteNonQuery();

                FirstType f = new FirstType();
                f.Id = 1;
                f.Name = "A";
                //fTypeName = f.GetType().AssemblyQualifiedName;
                //Console.WriteLine(fTypeName);

                SecondType s = new SecondType();
                s.Id = 2;
                s.Name = "B";
                //sTypeName = s.GetType().AssemblyQualifiedName;
                //Console.WriteLine(sTypeName);

                string xf = GetXml(f);
                Console.WriteLine(xf);
                string sql = String.Format("insert into dbo.test (data) values (N'{0}')", xf);
                                //f.ToXml());
                cmd = new SqlCommand(sql, c);
                cmd.ExecuteNonQuery();

                string xs = GetXml(s);
                Console.WriteLine(xs);
                sql = String.Format("insert into dbo.test (data) values (N'{0}')", xs);
                                //s.ToXml());
                cmd = new SqlCommand(sql, c);
                cmd.ExecuteNonQuery();

                Console.WriteLine("rows inserted");
            }

            // Now get them back!

            //XmlSchemaSet xss = new XmlSchemaSet();
            // See http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=395012&SiteID=1
            IXmlNamespaceResolver xnameRes = null;

            //System.Xml.Schema.XmlSchemaCollection

            using (IConnection ic = AdapterFactory.GetConnection())
            {
                SqlConnection c = ic.Value;
                string sql = "SELECT TestId, Data FROM dbo.Test";
                SqlCommand cmd = new SqlCommand(sql, c);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        SqlXml sx = reader.GetSqlXml(1);
                        Console.WriteLine(sx.Value);

                        using (XmlReader xr = sx.CreateReader())
                        {
                            BaseType b = FromXml(xr);
                            Console.WriteLine("Got back a "+b.GetType().Name);
                            Console.WriteLine(b.ToString());
                            break;
                        }

                        /*
                        using (XmlReader xr = sx.CreateReader())
                        {
                            //IXmlSchemaInfo xinfo = xr.SchemaInfo;
                            //Console.WriteLine("CanResolveEntity=" + xr.CanResolveEntity);

                            while (xr.Read())
                            {
                                Console.WriteLine("reader node type="+xr.NodeType);
                                Console.WriteLine("name="+xr.Name);

                                Type t;
                                if (xr.Name=="First")
                                    t = Type.GetType(fTypeName);
                                else
                                    t = Type.GetType(sTypeName);

                                //object o = xr.ReadContentAsObject();
                                //object o = xr.ReadElementContentAsObject();
                                object o = xr.ReadElementContentAs(t, xnameRes);
                                if (o==null)
                                    Console.WriteLine("element not read");
                                else
                                {
                                    string msg = String.Format("id={0} is a {1}", id, o.GetType().Name);
                                    Console.WriteLine(msg);
                                    Console.WriteLine(o.ToString());
                                }
                            }
                        }
                         */
                    }
                }
            }
        }

        static string GetXml(BaseType b)
        {
            Type t = b.GetType();
            Console.WriteLine("serialize "+t.Name);
            XmlSerializer xs = new XmlSerializer(t);
            StringBuilder sb = new StringBuilder(100);
            XmlWriterSettings xws = new XmlWriterSettings();
            using (XmlWriter xw = XmlWriter.Create(sb, xws))
            {
                xs.Serialize(xw, b);
            }
            return sb.ToString();
        }

        static BaseType FromXml(XmlReader xr)
        {
            // TODO - need to get the proper type, could get it through xr.Name perhaps
            XmlSerializer xs = new XmlSerializer(typeof(FirstType));
            //XmlSerializer xs = new XmlSerializer(typeof(BaseType)); // err
            return (BaseType)xs.Deserialize(xr);
        }
    }
}
