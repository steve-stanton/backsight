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

namespace Backsight.Editor.Operations;

/// <summary>
/// Tagging interface for editing operations that may be recalled using the
/// Cadastral Editor's command recall facility (the <c>Edit - Recall</c> command).
/// </summary>
/// <remarks>Tagging the editing operation is merely a convenience, arising from
/// the fact that editing commands are wired up to the user interface via the
/// <see cref="EditingAction"/> class (which identifies each edit through an editing ID).
/// However, the editing operation does not need to do anything to accommodate command
/// recall - rather, it is the UI class that runs the edit that needs to do stuff.
/// </remarks>
interface IRecallable;