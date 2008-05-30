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

            using (IConnection ic = AdapterFactory.GetConnection())
            {
                // Confirm that all classes derived from Base are part of the database schema

                // Insert test objects into test table
                SqlConnection c = ic.Value;
                Console.WriteLine("connection open");

                SqlCommand cmd = new SqlCommand("DELETE FROM dbo.Test", c);
                cmd.ExecuteNonQuery();

                First f1 = new First();
                f1.Id = 1;
                f1.Name = "A";

                First f2 = new First();
                f2.Id = 2;
                f2.Name = "B";

                First f3 = new First();
                f3.Id = 3;
                f3.Name = "C";

                First f4 = new First();
                f4.Id = 4;
                f4.Name = "D";

                MyArcClass ac = new MyArcClass();
                ac.Radius = 990;
                ac.Center = 991;
                ac.AbValue = 100;

                MySegClass seg = new MySegClass();
                seg.Start = 200;
                seg.End = 201;
                seg.AbValue = 101;

                f3.More1 = ac;
                f2.More2 = seg;
                f4.More1 = ac;
                f4.More2 = seg;

                Insert(c, f1);
                Insert(c, f2);
                Insert(c, f3);
                Insert(c, f4);

                Console.WriteLine("rows inserted");
            }

            // Now get them back!

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
                            Base b = Base.FromXml(xr);
                            Console.WriteLine(b.ToString());
                        }
                    } 
                }
            }
        }

        static void Insert(SqlConnection c, Base b)
        {
            //string x = GetXml(b);
            string x = b.ToXml();
            Console.WriteLine(x);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = c;
            cmd.CommandText = "insert into dbo.test (data) values (@data)";
            cmd.Parameters.Add(new SqlParameter("@data", System.Data.SqlDbType.Xml));
            cmd.Parameters[0].Value = x;
            cmd.ExecuteNonQuery();
        }
    }
}
