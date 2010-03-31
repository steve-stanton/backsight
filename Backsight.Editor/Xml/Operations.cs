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
    abstract public partial class OperationType
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

    public partial class AttachPointType
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

    public partial class DeletionType
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

    public partial class GetControlType
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

    public partial class ImportType
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

    public partial class IntersectDirectionAndDistanceType
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

    public partial class IntersectDirectionAndLineType
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

    public partial class IntersectTwoDirectionsType
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

    public partial class IntersectTwoDistancesType
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

    public partial class IntersectTwoLinesType
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

    public partial class LineExtensionType
    {
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

    public partial class LineSubdivisionType
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

    public partial class MovePolygonPositionType
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

    public partial class MoveTextType
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

    public partial class NewArcType
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

    public partial class NewCircleType
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

    public partial class NewPointType
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

    public partial class NewSegmentType
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

    public partial class NewTextType
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

    public partial class ParallelLineType
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

    public partial class PathType
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

    public partial class PolygonSubdivisionType
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

    public partial class PropertyChangeType
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

    public partial class RadialType
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

    public partial class SetTopologyType
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

    public partial class SimpleLineSubdivisionType
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

    public partial class TextRotationType
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

    public partial class TrimLineType
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

    public partial class UpdateType
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
