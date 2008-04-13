/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor.Operations
{
    /// <written by="Bob Bruce" on="29-JUN-1998" was="CeSetLabelRotation" />
    /// <summary>
    /// Edit to define the default angle for subsequently added text.
    /// </summary>
    [Serializable]
    class TextRotationOperation : Operation
    {
        #region Class Data
        #endregion

        #region Constructors
        #endregion

        public override string Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override Distance GetDistance(LineFeature line)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override Feature[] Features
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override EditingActionId EditId
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override bool Undo()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool Rollforward()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void AddReferences()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void Execute(IPosition p1, IPosition p2)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
