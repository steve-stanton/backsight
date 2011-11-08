// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Backsight.Editor.Forms;
using System.IO;
//using netDxf.Tables;

namespace Backsight.Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //netDxf.DxfDocument dd = new netDxf.DxfDocument();
            //MessageBox.Show("LineTypes.Count="+dd.LineTypes.Count);
            //LineType lt = LineType.Continuous;
            //MessageBox.Show(lt.Name);

            Application.Run(new MainForm(args));
            /*
            EditSerializer es = new EditSerializer();

            DistanceUnit du = new DistanceUnit(DistanceUnitType.Feet);
            Backsight.Editor.Observations.Distance dist = new Backsight.Editor.Observations.Distance("123", du);
            Backsight.Editor.AnnotatedDistance d = new Backsight.Editor.AnnotatedDistance(dist, true);
            Backsight.Editor.Observations.OffsetDistance od = new Backsight.Editor.Observations.OffsetDistance(d, true);

            Backsight.Editor.Xml.ObservationData t = Backsight.Editor.Xml.DataFactory.Instance.ToData<Backsight.Editor.Xml.ObservationData>(od);

            try
            {
                //string[] a = new string[] { "Animal", "Dog", "Cat" };
                float[] a = new float[] { 1.2f, 3.4f, 5.6f };
                es.WriteSimpleArray<float>("Test", a);

                //Observation[] a = new Observation[] { dist, d, od };
                //es.WritePersistentArray<Backsight.Editor.Observation>("Test", a);
                string testString = es.Writer.ToString();
                MessageBox.Show(testString);

                File.WriteAllText(@"C:\Temp\Test.txt", testString);

                EditDeserializer eds = new EditDeserializer();

                using (StringReader sr = new StringReader(testString))
                {
                    eds.Reader = new TextEditReader(sr);
                    float[] res = eds.ReadSimpleArray<float>("Test");
                    //Backsight.Editor.Observation[] res = eds.ReadPersistentArray<Backsight.Editor.Observation>("Test");

                    es.Writer = new TextEditWriter();
                    //es.WritePersistentArray<Backsight.Editor.Observation>("Result", res);
                    es.WriteSimpleArray<float>("Result", res);
                    MessageBox.Show(es.Writer.ToString());
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
*/
        }
    }
}
