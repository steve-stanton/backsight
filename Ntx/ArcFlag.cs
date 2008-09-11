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

namespace Ntx
{
    /// <summary>
    /// Flag bits for line super-descriptors.
    /// </summary>
    [Flags]
    enum ArcFlag : uint
    {
#if (WIN32 || _VMS)
        IsHeadBad       = 0x00000001,   // Bit  0 = PDFHB
        IsTailBad       = 0x00000002,   // Bit  1 = PDFTB
        IsHeadLinked 	= 0x00000004,   // Bit  2 = PDFHL
        IsTailLinked 	= 0x00000008,	// Bit  3 = PDFTL
        IsLeftWalked 	= 0x00000010,	// Bit  4 = PDFLW
        IsRightWalked 	= 0x00000020,	// Bit  5 = PDFRW
        IsBit6 			= 0x00000040,	// Bit  6
        IsBit7			= 0x00000080,	// Bit  7
        IsHeadNode 		= 0x00000100,	// Bit  8 = PDFHN
        IsTailNode 		= 0x00000200,	// Bit  9 = PDFTN
        IsHeadTerminal 	= 0x00000400,	// Bit 10 = PDFHT
        IsTailTerminal 	= 0x00000800,	// Bit 11 = PDFTT
        IsLeftIsland 	= 0x00001000,	// Bit 12 = PDFLO
        IsRightIsland 	= 0x00002000,	// Bit 13 = PDFRO
        IsJunction 		= 0x00004000,	// Bit 14 = PDFJN
        IsPath 			= 0x00008000,	// Bit 15 = PDFPH
        IsBit16 		= 0x00010000,	// Bit 16
        IsBit17 		= 0x00020000,	// Bit 17
        IsTopSelected 	= 0x00040000,	// Bit 18 = PDFTS
        IsTopSuppressed = 0x00080000,	// Bit 19 = PDFTX
        IsBit20 		= 0x00100000,	// Bit 20
        IsBit21         = 0x00200000,	// Bit 21
        IsBit22 		= 0x00400000,	// Bit 22
        IsBit23 		= 0x00800000,	// Bit 23
        IsBit24 		= 0x01000000,	// Bit 24
        IsBit25			= 0x02000000,	// Bit 25
        IsBit26			= 0x04000000,	// Bit 26
        IsBit27			= 0x08000000,	// Bit 27
        IsBit28			= 0x10000000,	// Bit 28
        IsScratch3		= 0x20000000,	// Bit 29
        IsScratch2		= 0x40000000,	// Bit 30
        IsScratch1		= 0x80000000,	// Bit 31
#else
        IsHeadBad       = 0x80000000,   // Bit  0 = PDFHB
        IsTailBad       = 0x40000000,   // Bit  1 = PDFTB
        IsHeadLinked 	= 0x20000000,   // Bit  2 = PDFHL
        IsTailLinked 	= 0x10000000,	// Bit  3 = PDFTL
        IsLeftWalked 	= 0x08000000,	// Bit  4 = PDFLW
        IsRightWalked 	= 0x04000000,	// Bit  5 = PDFRW
        IsBit6 			= 0x02000000,	// Bit  6
        IsBit7			= 0x01000000,	// Bit  7
        IsHeadNode 		= 0x00800000,	// Bit  8 = PDFHN
        IsTailNode 		= 0x00400000,	// Bit  9 = PDFTN
        IsHeadTerminal 	= 0x00200000,	// Bit 10 = PDFHT
        IsTailTerminal 	= 0x00100000,	// Bit 11 = PDFTT
        IsLeftIsland 	= 0x00080000,	// Bit 12 = PDFLO
        IsRightIsland 	= 0x00040000,	// Bit 13 = PDFRO
        IsJunction 		= 0x00020000,	// Bit 14 = PDFJN
        IsPath 			= 0x00010000,	// Bit 15 = PDFPH
        IsBit16 		= 0x00008000,	// Bit 16
        IsBit17 		= 0x00004000,	// Bit 17
        IsTopSelected 	= 0x00002000,	// Bit 18 = PDFTS
        IsTopSuppressed = 0x00001000,	// Bit 19 = PDFTX
        IsBit20 		= 0x00000800,	// Bit 20
        IsBit21         = 0x00000400,	// Bit 21
        IsBit22 		= 0x00000200,	// Bit 22
        IsBit23 		= 0x00000100,	// Bit 23
        IsBit24 		= 0x00000080,	// Bit 24
        IsBit25			= 0x00000040,	// Bit 25
        IsBit26			= 0x00000020,	// Bit 26
        IsBit27			= 0x00000010,	// Bit 27
        IsBit28			= 0x00000008,	// Bit 28
        IsScratch3		= 0x00000004,	// Bit 29
        IsScratch2		= 0x00000002,	// Bit 30
        IsScratch1		= 0x00000001,	// Bit 31
#endif
    }
}
