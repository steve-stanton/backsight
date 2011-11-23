// <remarks>
// Copyright 2011 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="17-NOV-2011"/>
    /// <summary>
    /// Numeric values that are used to identify the various data fields involved in
    /// the definition of edits.
    /// </summary>
    /// <remarks>
    /// These values are used to help avoid potential typos that might not get caught until
    /// run-time. As things currently stand, these values do not get persisted, so you can
    /// re-arrange them as you please. This may need to change if a binary writer is provided.</remarks>
    internal enum DataField : ushort
    {
        Empty = 0,
        X,
        Y,
        Z,
        Id,
        Point,
        Points,
        NewPoint,
        Line,
        Line1,
        Line2,
        Lines,
        NewLine,
        NewLine1,
        NewLine2,
        Text,
        Length,
        Features,
        Result,
        Test,
        Offset,
        Face1,
        Face2,
        Type,
        Radius,
        Direction,
        Direction1,
        Direction2,
        Edit,
        Distance,
        Distance1,
        Distance2,
        From,
        To,
        DirLine,
        DistLine,
        ForeignKey,
        Key,
        Arc,
        ClosingPoint,
        Topological,
        Left,
        PolygonX,
        PolygonY,
        PositionRatio,
        Center,
        CloseTo,
        SplitAfter,
        SplitAfter1,
        SplitAfter2,
        SplitBefore,
        SplitBefore1,
        SplitBefore2,
        Default,
        From1,
        From2,
        RefLine,
        ReverseArc,
        Term1,
        Term2,
        DeactivatedLabel,
        End,
        Entity,
        EntryString,
        FirstArc,
        Flipped,
        Label,
        NewX,
        NewY,
        OldX,
        OldY,
        RevisedEdit,
        Value,
        Start,
        Table,
        Template,
        Width,
        Backsight,
        Base,
        Clockwise,
        Data,
        DefaultEntryUnit,
        Delete,
        Font,
        Height,
        OldPolygonX,
        OldPolygonY,
        Rotation,
        EntryFromEnd,
        ExtendFromEnd,
        Fixed,
        Unit,
        UpdatedPoint
    }
}
