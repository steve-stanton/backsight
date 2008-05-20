using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

using Backsight.Data;
using System.Data.SqlTypes;
using System.Xml;

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

                string sql = String.Format("insert into dbo.test (data) values ('{0}')",
                                f.ToXml());
                cmd = new SqlCommand(sql, c);
                cmd.ExecuteNonQuery();

                sql = String.Format("insert into dbo.test (data) values ('{0}')",
                                s.ToXml());
                cmd = new SqlCommand(sql, c);
                cmd.ExecuteNonQuery();

                Console.WriteLine("rows inserted");
            }

            // Now get them back!

            using (IConnection ic = AdapterFactory.GetConnection())
            {
                SqlConnection c = ic.Value;
                //string sql = "SELECT TestId, Data FROM dbo.Test FOR XML";// AUTO";
                string sql = "SELECT Data FROM dbo.Test FOR XML AUTO";
                SqlCommand cmd = new SqlCommand(sql, c);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //int id = reader.GetInt32(0);
                        //SqlXml sx = reader.GetSqlXml(1);
                        int id = 0;
                        SqlXml sx = reader.GetSqlXml(0);
                        Console.WriteLine(sx.Value);
                        using (XmlReader xr = sx.CreateReader())
                        {
                            Console.WriteLine("CanResolveEntity=" + xr.CanResolveEntity);

                            while (xr.Read())
                            {
                                //object o = xr.ReadContentAsObject();
                                object o = xr.ReadElementContentAsObject();
                                //object o = xr.ReadElementContentAs(typeof(Base), null);
                                string msg = String.Format("id={0} is a {1}", id, o.GetType().Name);
                                Console.WriteLine(msg);
                                Console.WriteLine(o.ToString());
                            }
                        }
                    }
                }
            }
        }
    }
}
