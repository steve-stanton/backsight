// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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
using Backsight.Editor.Operations;

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// Base class for any sort of serialized editing operation.
    /// </summary>
    /// <remarks>The remainder of this class is auto-generated, and may be found
    /// in the <c>Edits.cs</c> file.</remarks>
    abstract public partial class OperationData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        abstract internal Operation LoadOperation(Session s);

        /// <summary>
        /// Converts XML data into the input for an editing operation.
        /// </summary>
        /// <param name="loader">Deserialization helper</param>
        /// <returns>The input for the editing operation</returns>
        /// <remarks>This should ultimately be declared as an abstract method</remarks>
        internal virtual OperationInput GetInput(ILoader loader)
        {
            throw new NotImplementedException();
        }
    }

    public partial class AttachPointData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new AttachPointOperation(s, this);
        }

        /// <summary>
        /// Converts XML data into the input for an editing operation.
        /// </summary>
        /// <param name="loader">Deserialization helper</param>
        /// <returns>The input for the editing operation</returns>
        /// <remarks>This should ultimately be declared as an abstract method</remarks>
        internal override OperationInput GetInput(ILoader loader)
        {
            LineFeature line = loader.Find<LineFeature>(this.Line);
            return new AttachPointInput(line, this.PositionRatio);
        }
    }

    public partial class DeletionData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new DeletionOperation(s, this);
        }
    }

    public partial class GetControlData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new GetControlOperation(s, this);
        }
    }

    public partial class ImportData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new ImportOperation(s, this);
        }
    }

    public partial class IntersectDirectionAndDistanceData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new IntersectDirectionAndDistanceOperation(s, this);
        }
    }

    public partial class IntersectDirectionAndLineData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new IntersectDirectionAndLineOperation(s, this);
        }
    }

    public partial class IntersectTwoDirectionsData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new IntersectTwoDirectionsOperation(s, this);
        }
    }

    public partial class IntersectTwoDistancesData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new IntersectTwoDistancesOperation(s, this);
        }
    }

    public partial class IntersectTwoLinesData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new IntersectTwoLinesOperation(s, this);
        }
    }

    public partial class LineExtensionData
    {
        internal LineExtensionData(LineExtensionOperation op)
        {
            this.Id = "123:45";
            this.Line = "45:67";
            this.ExtendFromEnd = true;
            this.Distance = new DistanceData(new Backsight.Editor.Observations.Distance(123.0, new DistanceUnit(DistanceUnitType.Meters), false));
            this.NewPoint = new CalculatedFeatureData();
        }

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new LineExtensionOperation(s, this);
        }
    }

    public partial class LineSubdivisionData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new LineSubdivisionOperation(s, this);
        }
    }

    public partial class MovePolygonPositionData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new MovePolygonPositionOperation(s, this);
        }
    }

    public partial class MoveTextData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new MoveTextOperation(s, this);
        }
    }

    public partial class NewArcData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new NewLineOperation(s, this);
        }
    }

    public partial class NewCircleData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new NewCircleOperation(s, this);
        }
    }

    public partial class NewPointData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new NewPointOperation(s, this);
        }
    }

    public partial class NewSegmentData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new NewLineOperation(s, this);
        }
    }

    public partial class NewTextData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new NewTextOperation(s, this);
        }
    }

    public partial class ParallelLineData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new ParallelOperation(s, this);
        }
    }

    public partial class PathData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new PathOperation(s, this);
        }
    }

    public partial class PolygonSubdivisionData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new PolygonSubdivisionOperation(s, this);
        }
    }

    public partial class PropertyChangeData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new PropertyChangeOperation(s, this);
        }
    }

    public partial class RadialData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new RadialOperation(s, this);
        }
    }

    public partial class SetTopologyData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new SetTopologyOperation(s, this);
        }
    }

    public partial class SimpleLineSubdivisionData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new SimpleLineSubdivisionOperation(s, this);
        }
    }

    public partial class TextRotationData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new TextRotationOperation(s, this);
        }
    }

    public partial class TrimLineData
    {
        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new TrimLineOperation(s, this);
        }
    }

    public partial class UpdateData
    {
        /// <summary>
        /// Loads this update into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new UpdateOperation(s, this);
        }
    }
}
