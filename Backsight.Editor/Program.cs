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
            Application.Run(new MainForm(args));

            /*
            DistanceUnit du = new DistanceUnit(DistanceUnitType.Feet);
            Backsight.Editor.Observations.Distance d = new Backsight.Editor.Observations.Distance("123", du);
            Backsight.Editor.Observations.OffsetDistance od = new Backsight.Editor.Observations.OffsetDistance(d, true);

            // If the resultant type is known (and not abstract), you can omit the header tag. Just remember to
            // specify the datatype when you deserialize.
            System.Yaml.YamlConfig yc = new System.Yaml.YamlConfig();
            yc.OmitTagForRootNode = true;
            System.Yaml.Serialization.YamlSerializer ys = new System.Yaml.Serialization.YamlSerializer(yc);
            Backsight.Editor.Xml.ObservationData t = Backsight.Editor.Xml.DataFactory.Instance.ToData<Backsight.Editor.Xml.ObservationData>(od);

            ulong testVal = 12345678901234567890L;
            UpdateItem[] changes = new UpdateItem[2];
            changes[0] = new UpdateItem("Name1", t);
            changes[1] = new UpdateItem("Name2", testVal);

            try
            {
                string s = ys.Serialize(changes);
                //string s = Backsight.Editor.Xml.DataFactory.Instance.ObservationToString<Backsight.Editor.Observations.OffsetDistance>(od);
                MessageBox.Show(s);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                //Observation res = Backsight.Editor.Xml.DataFactory.Instance.StringToObservation(s);
                object[] resArray = (object[])ys.Deserialize(s, typeof(Backsight.Editor.Xml.OffsetDistanceData));
                //MessageBox.Show("Number of elements="+resArray.Length);
                object res = resArray[0];
                MessageBox.Show(res.GetType().Name);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
             */

        }
    }
}
