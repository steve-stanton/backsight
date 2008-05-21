using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;

using Backsight.Data;

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

                //FirstType f = new FirstType();
                First f = new First();
                f.Id = 1;
                f.Name = "A";
                //fTypeName = f.GetType().AssemblyQualifiedName;
                //Console.WriteLine(fTypeName);

                //SecondType s = new SecondType();
                Second s = new Second();
                s.Id = 2;
                s.Name = "B";
                //sTypeName = s.GetType().AssemblyQualifiedName;
                //Console.WriteLine(sTypeName);

                string xf = GetXml(f);
                Console.WriteLine(xf);
                string sql = String.Format("insert into dbo.test (data) values (N'{0}')", xf);
                cmd = new SqlCommand(sql, c);
                cmd.ExecuteNonQuery();

                //string xs = s.ToXml(); //GetXml(s);
                //string xs = GetXml(s).Replace("<Second ", "<Second xmlns=\"TestSpace\" ");
                string xs = GetXml(s);
                Console.WriteLine(xs);
                sql = String.Format("insert into dbo.test (data) values (N'{0}')", xs);
                cmd = new SqlCommand(sql, c);
                cmd.ExecuteNonQuery();

                Console.WriteLine("rows inserted");
            }

            // Now get them back!

            //XmlSchemaSet xss = new XmlSchemaSet();
            // See http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=395012&SiteID=1
            IXmlNamespaceResolver xnameRes = null;

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

                        using (XmlReader xr = sx.CreateReader())
                        {
                            // It's a fragment
                            //Console.WriteLine("ConformanceLevel="+xr.Settings.ConformanceLevel);
                            Base b = FromXml(xr);
                            Console.WriteLine("Got back a " + b.GetType().Name);
                            Console.WriteLine(b.ToString());
                        }
                    } 
                }
            }
        }

        static string GetXml(Base b)
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

        static Base FromXml(XmlReader xr)
        {
            // TODO - need to get the proper type, could get it through xr.Name perhaps
            //xr.MoveToElement();

            xr.Read();

            Type t = xr.ValueType; // it's String
            if (t == null)
                Console.WriteLine("no value type");
            else
                Console.WriteLine("value type=" + t.Name);

            string name = xr.Name;
            Console.WriteLine("FromXml="+name);
            //Type t = Type.GetType("TestX." + name);

            XmlSerializer xs;
            if (name=="First")
                xs = new XmlSerializer(typeof(First));
            else
                xs = new XmlSerializer(typeof(Second));

            return (Base)xs.Deserialize(xr);
        }
    }
}
