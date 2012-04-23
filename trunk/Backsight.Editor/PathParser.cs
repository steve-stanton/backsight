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

using Backsight.Editor.Observations;

namespace Backsight.Editor
{
    /// <summary>
    /// Parses the data entry string for a connection path.
    /// </summary>
    /// <remarks>Previously part of the <c>PathForm</c> dialog</remarks>
    class PathParser
    {
        #region Static

        /// <summary>
        /// Attempts to parse the supplied string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="entryUnit">The initial default entry units</param>
        /// <returns>The items in the parsed path</returns>
        /// <exception cref="Exception">If any parsing problem is encountered</exception>
        static internal PathItem[] GetPathItems(string str, DistanceUnit entryUnit)
        {
            PathParser pp = new PathParser(str, entryUnit);
            return pp.m_Items.ToArray();
        }

        static internal Leg[] CreateLegs(string str, DistanceUnit entryUnit)
        {
            PathItem[] items = GetPathItems(str, entryUnit);
            return CreateLegs(items);
        }

        static internal Leg[] CreateLegs(PathItem[] items)
        {
            // Count the number of legs.
            int numLeg = PathItem.GetMaxLegNumber(items);
            if (numLeg == 0)
                throw new Exception("PathParser.Create -- No connection legs");

            List<Leg> result = new List<Leg>(numLeg);

            // Create each leg.

            int legnum = 0;       // Current leg number
            int nexti = 0;        // Index of the start of the next leg

            for (int si = 0; si < items.Length; si = nexti)
            {
                // Skip if no leg number (could be new units spec).
                if (items[si].LegNumber == 0)
                {
                    nexti = si + 1;
                    continue;
                }

                // Confirm the leg count is valid.
                if (legnum + 1 > numLeg)
                    throw new Exception("PathParser.Create -- Bad number of path legs.");

                // Create the leg.
                Leg newLeg;
                if (items[si].ItemType == PathItemType.BC)
                    newLeg = CreateCircularLeg(items, si, out nexti);
                else
                    newLeg = CreateStraightLeg(items, si, out nexti);

                // Exit if we failed to create the leg.
                if (newLeg == null)
                    throw new Exception("PathParser.Create -- Unable to create leg");

                result.Add(newLeg);
            }

            // Confirm we created the number of legs we expected.
            if (numLeg != result.Count)
                throw new Exception("PathParser.Create -- Unexpected number of legs");

            return result.ToArray();
        }

        /// <summary>
        /// Creates a circular leg.
        /// </summary>
        /// <param name="items">Array of path items.</param>
        /// <param name="si">Index to the item where the leg data starts.</param>
        /// <param name="nexti">Index of the item where the next leg starts.</param>
        /// <returns>The new leg.</returns>
        static CircularLeg CreateCircularLeg(PathItem[] items, int si, out int nexti)
        {
            // Confirm that the first item refers to the BC.
            if (items[si].ItemType != PathItemType.BC)
                throw new Exception("PathParser.CreateCircularLeg - Not starting at BC");

            // The BC has to be followed by at least 3 items: angle, radius
            // and EC (add an extra 1 to account for 0-based indexing).
            if (items.Length < si + 4)
                throw new Exception("PathParser.CreateCircularLeg - Insufficient curve data");

            double bangle = 0.0;		// Angle at BC
            double cangle = 0.0;		// Central angle
            double eangle = 0.0;		// Angle at EC
            bool twoangles = false;	    // True if bangle & eangle are both defined.
            bool clockwise = true;		// True if curve is clockwise
            int irad = 0;				// Index of the radius item
            bool cul = false;			// True if cul-de-sac case

            // Point to item following the BC.
            nexti = si + 1;
            PathItemType type = items[nexti].ItemType;

            // If the angle following the BC is a central angle
            if (type == PathItemType.CentralAngle)
            {
                // We have a cul-de-sac
                cul = true;

                // Get the central angle.
                cangle = items[nexti].Value;
                nexti++;
            }
            else if (type == PathItemType.BcAngle)
            {
                // Get the entry angle.
                bangle = items[nexti].Value;
                nexti++;

                // Does an exit angle follow?
                if (items[nexti].ItemType == PathItemType.EcAngle)
                {
                    eangle = items[nexti].Value;
                    twoangles = true;
                    nexti++;
                }
            }
            else
            {
                // The field after the BC HAS to be an angle.
                throw new ApplicationException("Angle does not follow BC");
            }

            // Must be followed by radius.
            if (items[nexti].ItemType != PathItemType.Radius)
                throw new ApplicationException("Radius does not follow angle");

            // Get the radius
            Distance radius = items[nexti].GetDistance();
            irad = nexti;
            nexti++;

            // The item after the radius indicates whether the curve is counterclockwise.
            if (items[nexti].ItemType == PathItemType.CounterClockwise)
            {
                nexti++;
                clockwise = false;
            }

            // Get the leg ID.
            int legnum = items[si].LegNumber;

            // How many distances have we got?
            int ndist = 0;
            for (; nexti < items.Length && items[nexti].LegNumber == legnum; nexti++)
            {
                if (items[nexti].IsDistance)
                    ndist++;
            }

            // Create the leg.
            CircularLeg leg = new CircularLeg(radius, clockwise, ndist);
            CircularLegMetrics metrics = leg.Metrics;

            // Set the entry angle or the central angle, depending on what we have.
            if (cul)
                metrics.SetCentralAngle(cangle);
            else
                metrics.SetEntryAngle(bangle);

            // Assign second angle if we have one.
            if (twoangles)
                metrics.SetExitAngle(eangle);

            // Assign each distance, starting one after the radius.
            ndist = 0;
            for (int i = irad + 1; i < nexti; i++)
            {
                Distance dist = items[i].GetDistance();
                if (dist != null)
                {
                    // See if there is a qualifier after the distance
                    LegItemFlag qual = LegItemFlag.Null;
                    if (i + 1 < nexti)
                    {
                        PathItemType nexttype = items[i + 1].ItemType;
                        if (nexttype == PathItemType.MissConnect)
                            qual = LegItemFlag.MissConnect;
                        if (nexttype == PathItemType.OmitPoint)
                            qual = LegItemFlag.OmitPoint;
                    }

                    leg.SetDistance(dist, ndist, qual);
                    ndist++;
                }
            }

            // Return the new leg.
            return leg;
        }

        /// <summary>
        /// Creates a straight leg.
        /// </summary>
        /// <param name="items">Array of path items.</param>
        /// <param name="si">Index to the item where the leg data starts.</param>
        /// <param name="nexti">Index of the item where the next leg starts.</param>
        /// <returns>The new leg.</returns>
        static StraightLeg CreateStraightLeg(PathItem[] items, int si, out int nexti)
        {
            // Get the leg ID.
            int legnum = items[si].LegNumber;

            // How many distances have we got?
            int ndist = 0;
            for (nexti = si; nexti < items.Length && items[nexti].LegNumber == legnum; nexti++)
            {
                if (items[nexti].IsDistance)
                    ndist++;
            }

            // Create the leg.
            StraightLeg leg = new StraightLeg(ndist);

            // Assign each distance.
            ndist = 0;
            for (int i = si; i < nexti; i++)
            {
                Distance d = items[i].GetDistance();
                if (d != null)
                {
                    // See if there is a qualifier after the distance
                    LegItemFlag qual = LegItemFlag.Null;
                    if ((i + 1) < nexti)
                    {
                        PathItemType nexttype = items[i + 1].ItemType;

                        if (nexttype == PathItemType.MissConnect)
                            qual = LegItemFlag.MissConnect;

                        if (nexttype == PathItemType.OmitPoint)
                            qual = LegItemFlag.OmitPoint;
                    }

                    leg.SetDistance(d, ndist, qual);
                    ndist++;
                }

            }

            // If the first item is an angle, remember it as part of the leg.
            if (items[si].ItemType == PathItemType.Angle)
                leg.StartAngle = items[si].Value;
            else if (items[si].ItemType == PathItemType.Deflection)
                leg.SetDeflection(items[si].Value);

            // Return a reference to the new leg
            return leg;
        }

        #endregion

        #region Class data

        /// <summary>
        /// Parsed path items
        /// </summary>
        readonly List<PathItem> m_Items;

        /// <summary>
        /// The current data entry units
        /// </summary>
        DistanceUnit m_Units;

        /// <summary>
        /// Does the last parsed item signify an omitted point?
        /// </summary>
        bool m_Omit;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PathParser"/> class, and
        /// attempts to parse the supplied string.
        /// </summary>
        /// <param name="str">The string to parse</param>
        /// <param name="entryUnit">The initial default entry units</param>
        PathParser(string str, DistanceUnit entryUnit)
        {
            m_Items = new List<PathItem>();
            m_Omit = false;
            m_Units = entryUnit;

            ParseString(str);
        }

        #endregion

        /// <summary>
        /// Parses the string that represents the definition of a connection path.
        /// </summary>
        /// <param name="str">The string to parse</param>
        void ParseString(string str)
        {
            // Pick out each successive word (delimited by white space).
            string[] words = str.Split(new char[] { '\t', ' ' });
            foreach (string w in words)
                ParseWord(w);

            // Validate the path items
            ValidPath();

            // Assign leg numbers to each path item.
            SetLegs();
        }

        void ParseWord(string str)
        {
            // Return if string is empty (could be empty if this function
            // has been called recursively from below).
            str = str.Trim();
            int nc = str.Length;
            if (nc == 0)
                return;

            // If we have a new default units specification, make it
            // the default. There should be whitespace after the "..."
            if (str.Contains("..."))
            {
                DistanceUnit unit = GetUnits(str, true);
                PathItem item = new PathItem(PathItemType.Units, unit, 0.0);
                AddItem(item);
                return;
            }

            // If we have a counter-clockwise indicator, just remember it
            // and parse anything that comes after it.
            if (nc >= 2 && String.Compare(str.Substring(0, 2), "cc", true) == 0)
            {
                AddItem(PathItemType.CounterClockwise);
                ParseWord(str.Substring(2));
                return;
            }

            // If we have a BC, remember it & parse anything that follows.
            if (str[0] == '(')
            {
                AddItem(PathItemType.BC);
                ParseWord(str.Substring(1));
                return;
            }

            // If we have a EC, remember it & parse anything that follows.
            if (str[0] == ')')
            {
                AddItem(PathItemType.EC);
                ParseWord(str.Substring(1));
                return;
            }

            // If we have a single slash character (possibly followed by
            // a digit or a decimal point), record the single slash &
            // parse anything that follows.

            if (str[0] == '/')
            {
                // Check for a free-standing slash, or a slash that is
                // followed by a numeric digit or decimal point.

                if (nc == 1 || Char.IsDigit(str, 1) || str[1] == '.')
                {
                    AddItem(PathItemType.Slash);
                    ParseWord(str.Substring(1));
                    return;
                }

                // More than one character, or what follows is not a digit.
                // So we are dealing with either a miss-connect, or an
                // omit-point. In either case, there should be whitespace
                // after that.

                if (nc == 2)
                {
                    if (str[1] == '-')
                    {
                        AddItem(PathItemType.MissConnect);
                        return;
                    }

                    if (str[1] == '*')
                    {
                        AddItem(PathItemType.OmitPoint);
                        return;
                    }
                }
                // Allow CADCOR-style data entry
                else if (nc == 3)
                {
                    if (String.Compare(str, "/mc", true) == 0)
                    {
                        AddItem(PathItemType.MissConnect);
                        return;
                    }

                    if (String.Compare(str, "/op", true) == 0)
                    {
                        AddItem(PathItemType.OmitPoint);
                        return;
                    }
                }

                string msg = String.Format("Unexpected qualifier '{0}'", str);
                throw new ApplicationException(msg);
            }

            // If we have a multiplier, it must be immediately followed
            // by a numeric (integer) value.

            if (str[0] == '*')
            {
                if (nc == 1)
                    throw new ApplicationException("Unexpected '*' character");

                // Pick up the repeat count (not sure if the digits need to be
                // followed by white space, or whether non-numeric digits are valid,
                // so pick up only the digits).
                string num = GetIntDigits(str.Substring(1));

                // Error if repeat count is less than 2.
                int repeat;
                if (!Int32.TryParse(num, out repeat) || repeat < 2)
                {
                    string msg = String.Format("Unexpected repeat count in '{0}'", str);
                    throw new ApplicationException(msg);
                }

                if (repeat < 2)
                {
                    string msg = String.Format("Unexpected repeat count in '{0}'", str);
                    throw new ApplicationException(msg);
                }

                // Duplicate the last item using the repeat count.
                AddRepeats(repeat);

                // Continue parsing after the repeat count.
                ParseWord(str.Substring(1+num.Length));
                return;
            }

            // If the string contains an embedded qualifier (a "*" or a "/"
            // character), process the portion of any string prior to the
            // qualifier. Note that we have just handled the cases where
            // the qualifier was at the very start of the string.

            int starIndex = str.IndexOf('*');
            int slashIndex = str.IndexOf('/');

            if (starIndex >= 0 || slashIndex >= 0)
            {
                int qualIndex = starIndex;
                if (qualIndex < 0 || (slashIndex>=0 && qualIndex > slashIndex))
                    qualIndex = slashIndex;

                // Process the stuff prior to the qualifier.
                string copy = str.Substring(0, qualIndex);
                ParseWord(copy);

                // Process the stuff, starting with the qualifier character
                ParseWord(str.Substring(qualIndex));
                return;
            }

            // Process this string. We should have either a value or an angle.
            if (str.IndexOf('-') >= 0 || IsLastItemBC())
            {
                // If the string contains a "c" character, it's a central
                // angle; process the string only as far as that.
                PathItemType type = PathItemType.Angle;

                int caIndex = str.ToUpper().IndexOf('C');
                if (caIndex>=0)
                {
                    str = str.Substring(0, caIndex);
                    type = PathItemType.CentralAngle;
                }
                else
                {
                    // Check if it's a deflection (if so, strip out the "d").
                    int dIndex = str.ToUpper().IndexOf('D');
                    if (dIndex >= 0)
                    {
                        str = str.Substring(0, dIndex) + str.Substring(dIndex + 1);
                        type = PathItemType.Deflection;
                    }
                }

                // Try to parse an angular value into radians.
                double radval;
                if (RadianValue.TryParse(str, out radval))
                {
                    PathItem item = new PathItem(type, null, radval);
                    AddItem(item);
                    return;
                }

                // Bad angle.
                string msg = String.Format("Malformed angle '{0}'", str);
                throw new ApplicationException(msg);
            }
            else
            {
                // Get the current distance units.
                DistanceUnit unit = GetUnits(null, false);

                // Grab characters that look like a floating point number
                string num = GetDoubleDigits(str);
                double val;
                if (!Double.TryParse(num, out val))
                {
                    string msg = String.Format("Malformed value '{0}'", str);
                    throw new ApplicationException(msg);
                }

                // If we didn't get right to the end, we may have distance units,
                // or the ")" character indicating an EC.
                if (num.Length < str.Length && str[num.Length] != ')')
                {
                    unit = GetUnits(str.Substring(num.Length), false);
                    if (unit == null)
                    {
                        string msg = String.Format("Malformed value '{0}'", str);
                        throw new ApplicationException(msg);
                    }
                }

                PathItem item = new PathItem(PathItemType.Value, unit, val);
                AddItem(item);

                if (str.Length>num.Length && str[num.Length] == ')')
                    ParseWord(str.Substring(num.Length));

                return;
            }
        }

        /// <summary>
        /// Gets or sets the units for distance observations.
        /// </summary>
        /// <param name="str">The string containing the units specification. The
        /// characters up to the first white space character (or "." character) must
        /// match one of the abbreviations for the desired units. Pass in a null
        /// pointer (the default) if you just want to get the current default
        /// units.</param>
        /// <param name="makedef">True if the units obtained should be regarded as
        /// the new default. (default was false).</param>
        /// <returns>The corresponding units.</returns>
        DistanceUnit GetUnits(string str, bool makedef)
        {
            if (str != null)
            {
                // Pick up characters that represent the abbreviation for
                // the units. Break on any white space or a "." character.

                string abbrev = String.Empty;
                foreach (char c in str)
                {
                    if (Char.IsWhiteSpace(c) || c == '.')
                        break;

                    abbrev += c;
                }

                // Try to match the abbreviation to one of the unit
                // types known to the map.
                DistanceUnit match = MatchUnits(abbrev);

                // Issue message if there was no match.
                if (match == null)
                {
                    string msg = String.Format("No units with abbreviation '{0}'", abbrev);
                    throw new ApplicationException(msg);
                }

                // If the units should be made the new default, do it so
                // long as the units were obtained.
                if (makedef && match != null)
                    m_Units = match;

                // Return the units (if any)
                return match;
            }
            else
            {
                // If the default was not previously defined, pick up
                // the map's current default.
                if (m_Units == null)
                    m_Units = EditingController.Current.EntryUnit;

                return m_Units;
            }
        }

        /// <summary>
        /// Converts a string that represents a distance unit abbreviation into
        /// a DistanceUnit reference (to one of the objects known to the enclosing map).
        /// </summary>
        /// <param name="abbrev">The units abbreviation.</param>
        /// <returns>The corresponding units (null if the abbreviation was not found).</returns>
        DistanceUnit MatchUnits(string abbrev)
        {
            EditingController ec = EditingController.Current;
            return ec.GetUnit(abbrev);
        }

        /// <summary>
        /// Holds on to an additional path item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void AddItem(PathItem item)
        {
            // If no items have been added, ensure omitted flag has been
            // freshly initialized.
            if (m_Items.Count == 0)
                m_Omit = false;

            // Ignore an attempt to add 2 miss-connects in a row (ValidPath
            // will complain).
            PathItemType type = item.ItemType;
            if (m_Items.Count > 0 &&
                type == PathItemType.MissConnect &&
                m_Items[m_Items.Count - 1].ItemType == PathItemType.MissConnect)
                return;

            // Add the supplied item into the list.
            m_Items.Add(item);

            // If we have just appended a PAT_VALUE, append an additional
            // miss-connect item if we previously omitted a point.
            if (m_Omit && type == PathItemType.Value)
            {
                m_Omit = false;
                AddItem(PathItemType.MissConnect);
            }

            // Remember whether we just omitted a point.
            if (type == PathItemType.OmitPoint)
                m_Omit = true;
        }

        /// <summary>
        /// Holds on to an additional path item. Good for items that
        /// do not have an associated value.
        /// </summary>
        /// <param name="type">The type of item to add.</param>
        void AddItem(PathItemType type)
        {
            PathItem item = new PathItem(type, null, 0.0);
            AddItem(item);
        }

        /// <summary>
        /// Validates path items. Prior to call, the path should be parsed by
        /// making a series of calls to ParseWord. This generates a set of
        /// items that are generated without consideration to their context.
        /// This function validates the context, elaborating on the meaning
        /// of PAT_ANGLE and PAT_VALUE item codes.
        /// </summary>
        /// <returns></returns>
        void ValidPath()
        {
            // The path must contain at least one item.
            if (m_Items == null || m_Items.Count == 0)
                throw new Exception("Path has not been specified.");

            // All PAT_VALUE's outside of a curve definition should have
            // type PAT_DISTANCE. Within curves, it's a bit more complicated.

            int ibc = 0;        // Index of last BC
            bool curve = false;	// Not in a curve to start with

            for (int i = 0; i < m_Items.Count; i++)
            {
                PathItem item = m_Items[i];

                switch (item.ItemType)
                {
                    case PathItemType.BC:
                        {
                            // If we have a BC, confirm that we are not already in
                            // a curve. Also confirm that there are at least 3 items
                            // after the BC (enough for an angle, a radius, and an EC).

                            if (curve)
                                throw new ApplicationException("Nested curve detected");

                            curve = true;

                            if ((ibc + 4) > m_Items.Count)
                                throw new ApplicationException("BC not followed by angle, radius, and EC");

                            ibc = i;
                            break;
                        }

                    case PathItemType.EC:
                        {
                            // If we have an EC, confirm that we were in a curve.

                            if (!curve)
                                throw new ApplicationException("EC was not preceded by BC");

                            curve = false;
                            break;
                        }

                    case PathItemType.Value:
                        {
                            // If not in a curve, change all PAT_VALUE types to
                            // PAT_DISTANCE types. Inside a curve, PAT_VALUES may
                            // actually be angles that need to be converted into
                            // radians.

                            // All values must point to the data entry units.
                            if (item.Units==null)
                                throw new ApplicationException("Value has no unit of measurement");

                            if (!curve)
                                item.ItemType = PathItemType.Distance;
                            else
                            {
                                // The value immediately after the BC is always an angle.
                                if (i == (ibc + 1))
                                {
                                    item.ItemType = PathItemType.BcAngle;
                                    item.Value *= MathConstants.DEGTORAD;
                                }
                                else if (i == (ibc + 2))
                                {
                                    // Could be an angle, or a radius. If the NEXT
                                    // item is a value, we must have an exit angle.

                                    if (m_Items[i + 1].ItemType == PathItemType.Value)
                                    {
                                        item.ItemType = PathItemType.EcAngle;
                                        item.Value *= MathConstants.DEGTORAD;
                                    }
                                    else
                                        item.ItemType = PathItemType.Radius;
                                }
                                else if (i == (ibc + 3))
                                    item.ItemType = PathItemType.Radius;
                                else
                                    item.ItemType = PathItemType.Distance;
                            }

                            break;
                        }

                    case PathItemType.Deflection:
                    case PathItemType.Angle:
                        {
                            // Angles inside curve definitions have to be qualified. If
                            // they appear, they MUST follow immediately after the BC.
                            // For angles NOT in a curve, you can only have one angle
                            // at a time.

                            if (curve)
                            {
                                // Can't have deflections inside a curve.
                                if (item.ItemType == PathItemType.Deflection)
                                    throw new ApplicationException("Deflection not allowed within curve definition");

                                if (i == (ibc + 1))
                                    item.ItemType = PathItemType.BcAngle;
                                else if (i == (ibc + 2))
                                    item.ItemType = PathItemType.EcAngle;
                                else
                                    throw new ApplicationException("Extraneous angle inside curve definition");
                            }
                            else
                            {
                                if (i > 0 && m_Items[i-1].ItemType == PathItemType.Angle)
                                    throw new ApplicationException("More than 1 angle at the end of a straight");

                                // Also, it makes no sense to have an angle right after an EC.
                                if (i > 0 && m_Items[i - 1].ItemType == PathItemType.EC)
                                    throw new ApplicationException("Angle after EC makes no sense");
                            }

                            break;
                        }

                    case PathItemType.Slash:
                        {
                            // A free-standing slash character is only valid within
                            // a curve definition. It has to appear at a specific
                            // location in the sequence.
                            //
                            // BC -> BCAngle -> Radius -> Slash
                            // BC -> BCAngle -> Radius -> CCMarker -> Slash
                            // BC -> BCAngle -> ECAngle -> Radius -> CCMarker -> Slash
                            //
                            // In other words, it can come at ibc+3 through ibc+5.

                            if (!curve)
                                throw new ApplicationException("Extraneous '/' character");

                            if (i < ibc + 3 || i > ibc + 5)
                                throw new ApplicationException("Misplaced '/' character");

                            break;
                        }

                    case PathItemType.CounterClockwise:
                        {
                            // Counter-clockwise indicator. Similar to PAT_SLASH, it has
                            // a specific range of valid positions with respect to the BC.

                            if (!curve)
                                throw new ApplicationException("Counter-clockwise indicator detected outside curve definition");

                            if (i < ibc + 3 || i > ibc + 4)
                                throw new ApplicationException("Misplaced 'cc' characters");

                            break;
                        }

                    case PathItemType.CentralAngle:
                        {
                            // A central angle is valid only within a curve definition
                            // and must be immediately after the BC.

                            if (!curve)
                                throw new ApplicationException("Central angle detected outside curve definition");

                            if (i != ibc + 1)
                                throw new ApplicationException("Central angle does not follow immediately after BC");

                            break;
                        }

                    case PathItemType.MissConnect:
                    case PathItemType.OmitPoint:
                        {
                            // Miss-connections & omit points must always follow on from a PAT_DISTANCE.

                            if (i == 0 || m_Items[i - 1].ItemType != PathItemType.Distance)
                                throw new ApplicationException("Miss-Connect or Omit-Point is not preceded by a distance");

                            break;
                        }

                    case PathItemType.Units:
                        {
                            // No checks
                            break;
                        }

                    default:
                        {
                            // All item types generated via ParseWord should have been
                            // listed above, even if there is no check. If any got missed,
                            // drop through to a message, but keep going.

                            string msg = String.Format("PathForm.ValidPath - Unhandled check for {0}", item.ItemType);
                            throw new Exception(msg);
                            break;
                        }

                } // end switch

            } // next item

            // Error if we got to the end, and any curve was not closed.
            if (curve)
                throw new ApplicationException("Circular arc does not have an EC");
        }

        /// <summary>
        /// Returns the numeric digits (if any) at the start of a string
        /// </summary>
        /// <param name="str">The string that should be starting with some digits (e.g. 1234abc)</param>
        /// <returns>The leading numeric digits (blank if the first character in the supplied
        /// string is not a digit)</returns>
        string GetIntDigits(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!Char.IsDigit(str, i))
                {
                    if (i == 0)
                        return String.Empty;
                    else
                        return str.Substring(0, i);
                }
            }

            // The entire string consists of numeric digits (or it's empty)
            return str;
        }

        /// <summary>
        /// Repeats the last path item a specific number of times. The thing to
        /// repeat HAS to be of type PAT_VALUE (or possibly a PAT_MC that has been
        /// automatically inserted by <see cref="AddItem"/>).
        /// </summary>
        /// <param name="repeat">The number of times the last item should appear (we
        /// will append n-1 copies of the last item).</param>
        void AddRepeats(int repeat)
        {
            // Confirm that we have something to repeat.
            if (m_Items.Count == 0)
                throw new ApplicationException("Nothing to repeat");

            // If the last item was a PAT_MC, get to the value before that.
            int prev = m_Items.Count - 1;
            PathItemType type = m_Items[prev].ItemType;
            if (type == PathItemType.MissConnect && prev > 0)
            {
                prev--;
                type = m_Items[prev].ItemType;
            }

            // It can only be a PAT_VALUE.
            if (type != PathItemType.Value)
                throw new ApplicationException("Unexpected repeat multiplier");

            // Make copies of the last value.
            for (int i = 1; i < repeat; i++)
            {
                PathItem copy = new PathItem(m_Items[prev]);
                AddItem(copy);
            }
        }

        /// <summary>
        /// Checks if the last path item is a BC.
        /// </summary>
        /// <returns>True if the last parsed item represents the BC of a circular arc</returns>
        bool IsLastItemBC()
        {
            if (m_Items.Count == 0)
                return false;

            PathItem lastItem = m_Items[m_Items.Count - 1];
            return (lastItem.ItemType == PathItemType.BC);
        }

        /// <summary>
        /// Returns a portion of a string that contains numeric characters
        /// </summary>
        /// <param name="s">The string that starts with a numeric substring</param>
        /// <returns>The numeric string starting at the supplied index (a
        /// blank string if the character at that position is not a number,
        /// a period, or a minus sign).</returns>
        /// <remarks>This may not handle i18n (e.g. decimal places may
        /// actually be commas).</remarks>
        string GetDoubleDigits(string s)
        {
            int nChar = 0;

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                // Allow '-' character only at the start
                if ((c == '-' && i == 0) || c == '.' || Char.IsNumber(c))
                    nChar++;
                else
                    break;
            }

            if (nChar == 0)
                return String.Empty;

            return s.Substring(0, nChar);
        }

        /// <summary>
        /// Associates each path item with a leg sequence number.
        /// </summary>
        /// <returns>The number of legs found.</returns>
        int SetLegs()
        {
            int nleg = 0;

            // Note that PathItemType.Units may not get a leg number.

            for (int i = 0; i < m_Items.Count; ) // not i++
            {
                PathItemType type = m_Items[i].ItemType;

                if (type == PathItemType.Distance ||
                    type == PathItemType.Angle ||
                    type == PathItemType.Deflection ||
                    type == PathItemType.BC)
                {
                    // If we have a distance or angle item, increment the leg 
                    // sequence number until we hit an angle or a BC. In the case of
                    // an angle, it always comes at the START of a leg.

                    if (type == PathItemType.Distance ||
                        type == PathItemType.Angle ||
                        type == PathItemType.Deflection)
                    {
                        nleg++;
                        m_Items[i].LegNumber = nleg;
                        for (i++; i < m_Items.Count; i++)
                        {
                            if (m_Items[i].ItemType == PathItemType.Angle ||
                                m_Items[i].ItemType == PathItemType.Deflection ||
                                m_Items[i].ItemType == PathItemType.BC)
                                break;

                            m_Items[i].LegNumber = nleg;
                        }
                    }
                    else
                    {
                        // We have a BC, so increment the leg sequence number
                        // & scan until we hit the EC.

                        nleg++;
                        for (; i < m_Items.Count; i++)
                        {
                            m_Items[i].LegNumber = -nleg;	// negated
                            if (m_Items[i].ItemType == PathItemType.EC)
                            {
                                i++;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    m_Items[i].LegNumber = nleg;
                    i++;
                }
            }

            return nleg;
        }
    }
}
