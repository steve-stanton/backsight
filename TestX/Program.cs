using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

//using Microsoft.Data.SqlXml;

using Backsight.Data;

namespace TestX
{
    class Program
    {
        static Dictionary<RuntimeTypeHandle, XmlSerializer> s_TypeSerializers;
        static string s_AssemblyName;

        static void Main(string[] args)
        {
            s_AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            // Confirm that all classes derived from Base are properly defined in the Base class (so
            // that xml serialization will work properly)
            Base.CheckIncludedTypes();

            s_TypeSerializers = Base.GetSerializers();
            /*
            Type[] types = Base.GetDerivedTypes();
            if (types.Length==0)
                Console.WriteLine("No derived types");
            else
            {
                foreach (Type t in types)
                    Console.WriteLine(t.Name);
            }
            return;
            */

            string constr = @"server='localhost\sqlexpress';Trusted_Connection=true;" +
                            @"multipleactiveresultsets=false;Initial Catalog=Foresight";
            AdapterFactory.ConnectionString = constr;

            using (IConnection ic = AdapterFactory.GetConnection())
            {
                // Confirm that all classes derived from Base are part of the database schema


                // Insert test objects into test table
                SqlConnection c = ic.Value;
                Console.WriteLine("connection open");

                SqlCommand cmd = new SqlCommand("DELETE FROM dbo.Test", c);
                cmd.ExecuteNonQuery();

                First f = new First();
                f.Id = 1;
                f.Name = "A";

                Second s = new Second();
                s.Id = 2;
                s.Name = "B";

                Third third = new Third();
                third.Id = 3;
                third.Name = "C";

                Insert(c, f);
                Insert(c, s);
                //Insert(c, third);

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

        static void Insert(SqlConnection c, Base b)
        {
            string x = GetXml(b);
            Console.WriteLine(x);
            /*
            string sql = String.Format("insert into dbo.test (data) values (N'{0}')", x);
            SqlCommand cmd = new SqlCommand(sql, c);
            cmd.ExecuteNonQuery();
            */
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = c;
            cmd.CommandText = "insert into dbo.test (data) values (@data)";
            cmd.Parameters.Add(new SqlParameter("@data", System.Data.SqlDbType.Xml));
            cmd.Parameters[0].Value = x;
            cmd.ExecuteNonQuery();

            /*
            StringBuilder sb = new StringBuilder(1000);
            using (XmlWriter xw = XmlWriter.Create(sb))
            {
                Type t = b.GetType();
                XmlSerializer xs = GetSerializer(t);
                xs.Serialize(xw, b);

                SqlXmlCommand xcmd = new SqlXmlCommand(AdapterFactory.ConnectionString);
                xcmd.CommandType = SqlXmlCommandType.Sql;
                xcmd.CommandText = "insert into dbo.test (data) values (@data)";
                SqlXmlParameter xp = xcmd.CreateParameter();
                xp.Name = "@data";
                xp.Value = sb.ToString();
                Console.WriteLine(sb.ToString());

                xcmd.ExecuteNonQuery();
            }
             */
        }

        static string GetXml(Base b)
        {
            Type t = b.GetType();
            XmlSerializer xs = GetSerializer(t);
            StringBuilder sb = new StringBuilder(1000);
            using (XmlWriter xw = XmlWriter.Create(sb))
            {
                xs.Serialize(xw, b);
            }

            // Get rid of verbose baggage that isn't needed (makes it more difficult to see
            // the info I'm actually interested in). Don't see any way to suppress them as
            // part of the actual serialization (would be nice). I believe these xmlns values
            // are included in a schema collection that's built into SqlServer.
            string s = sb.ToString();
            s = s.Replace(" encoding=\"utf-16\"", String.Empty);
            s = s.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ", String.Empty);
            s = s.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" ", String.Empty);
            return s;
        }

        static Base FromXml(XmlReader xr)
        {
            xr.Read();

            /*
            Type t = xr.ValueType; // it's initially String, after xr.Read it's Object (which isn't
                                    // good enough for feeding into the XmlSerializer cstr)
            */

            // Note that the name passed to GetType isn't assembly qualified, so it will only look
            // in the calling assembly and mscorlib.dll (see
            // http://blogs.msdn.com/suzcook/archive/2003/05/30/using-type-gettype-typename.aspx)

            string typeName = String.Format("{0}.{1}", s_AssemblyName, xr.Name);
            Type t = Type.GetType(typeName);
            if (t==null)
                Console.WriteLine("didn't get type for "+typeName);

            XmlSerializer xs = GetSerializer(t);
            return (Base)xs.Deserialize(xr);
        }

        static XmlSerializer GetSerializer(Type t)
        {
            if (t==null)
                return null;
            else
                return s_TypeSerializers[t.TypeHandle];
        }

    }
}
