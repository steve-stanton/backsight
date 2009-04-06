/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="02-FEB-2007" />
    /// <summary>
    /// IDs for the edits supported by the Cadastral Editor.
    /// </summary>
    public enum EditingActionId
    {
        Null                =0,
        DataImport          =1,
        LineSubdivision     =2,
        Annotation          =3,     // not supported
        //LineIntersection    =4,
        Network             =5,     // not supported
        RadialStakeout      =6,
        Extension           =7,
        BankTraverse        =8,     // not supported
        ClosedTraverse      =9,     // not supported
        OpenTraverse        =10,    // not supported
        ParallelTraverse    =11,    // not supported
        DirIntersect        =12,
        DistIntersect       =13,
        DirDistIntersect    =14,
        NewPoint            =15,
        NewText             =16,
        MoveLabel           =17,
        Deletion            =18,
        Update              =19,  
        NewLine             =20,
        Path                =21,
        Split               =22,
        PolygonSubdivision  =23,
        SetLabelRotation    =24,
        GetBackground       =25,
        Truncate            =26,
        GetControl          =27,
        ArcClip             =28,    // not supported (was in FieldArea)
        ArcSplit            =29,    // not supported (was in FieldArea)
        NewCircle           =30,
        LineIntersect       =31,
        DirLineIntersect    =32,
        LineExtend          =33,
        Radial              =34,
        SetTheme            =35,
        SetTopology         =36,
        PointOnLine         =37,
        Parallel            =38,
        Trim                =39,
        AttachPoint         =40,
        NewCircularArc      =41,
        MovePolygonPosition =42,
        PropertyChange      =43,
    }
}
