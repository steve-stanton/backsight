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
    [Flags]
    enum Flag : uint
    {
#if (WIN32 || _VMS)
        IsLinked		= 0x00000001,	// Bit  0 = PDFPL
        IsLogicLinked	= 0x00000002,	// Bit  1 = PDFLL
        IsSelected		= 0x00000004,	// Bit  2 = PDFSE
        IsSuppressed	= 0x00000008,	// Bit  3 = PDFSU
        IsMasked		= 0x00000010,	// Bit  4 = PDFMK
        IsDeleted		= 0x00000020,	// Bit  5 = PDFMD
        IsClosedLine	= 0x00000040,	// Bit  6 = PDFCL
        IsBit7			= 0x00000080,	// Bit  7 	 
        IsDominant		= 0x00000100,	// Bit  8 = PDFDM
        IsNode			= 0x00000200,	// Bit  9 = PDFND
        IsArc			= 0x00000400,	// Bit 10 = PDFAR
        IsLabel			= 0x00000800,	// Bit 11 = PDFDL
        IsOuterLabel	= 0x00001000,	// Bit 12 = PDFDO
        Is3D			= 0x00002000,	// Bit 13 = PDFZ 
        IsReadonly		= 0x00004000,	// Bit 14 = PDFRD
        IsSymbolized	= 0x00008000,	// Bit 15 = PDFSY
        IsIndexed 		= 0x00010000,	// Bit 16 = PDFIX
        IsInCubelFile	= 0x00020000,	// Bit 17 = PDFCU
        IsBit18			= 0x00040000,	// Bit 18        
        IsCombination	= 0x00080000,	// Bit 19 = PDFCO
        IsNew			= 0x00100000,	// Bit 20 = PDFNW
        IsPut			= 0x00200000,	// Bit 21        
        IsDisplaced		= 0x00400000,	// Bit 22 = PDFDI
        IsExaggeration	= 0x00800000,	// Bit 23 = PDFEX
        IsBit24			= 0x01000000,	// Bit 24 	 
        IsBit25			= 0x02000000,	// Bit 25 	 
        IsBit26			= 0x04000000,	// Bit 26 	 
        IsBit27			= 0x08000000,	// Bit 27 	 
        IsBit28			= 0x10000000,	// Bit 28 	 
        IsScratch3		= 0x20000000,	// Bit 29 = PDFS3
        IsScratch2		= 0x40000000,	// Bit 30 = PDFS2
        IsScratch1		= 0x80000000,	// Bit 31 = PDFS1
#else
        IsLinked		= 0x80000000,	// Bit  0 = PDFPL
        IsLogicLinked	= 0x40000000,	// Bit  1 = PDFLL
        IsSelected		= 0x20000000,	// Bit  2 = PDFSE
        IsSuppressed	= 0x10000000,	// Bit  3 = PDFSU
        IsMasked		= 0x08000000,	// Bit  4 = PDFMK
        IsDeleted		= 0x04000000,	// Bit  5 = PDFMD
        IsClosedLine	= 0x02000000,	// Bit  6 = PDFCL
        IsBit7			= 0x01000000,	// Bit  7 	 
        IsDominant		= 0x00800000,	// Bit  8 = PDFDM
        IsNode			= 0x00400000,	// Bit  9 = PDFND
        IsArc			= 0x00200000,	// Bit 10 = PDFAR
        IsLabel			= 0x00100000,	// Bit 11 = PDFDL
        IsOuterLabel	= 0x00080000,	// Bit 12 = PDFDO
        Is3D			= 0x00040000,	// Bit 13 = PDFZ 
        IsReadonly		= 0x00020000,	// Bit 14 = PDFRD
        IsSymbolized	= 0x00010000,	// Bit 15 = PDFSY
        IsIndexed 		= 0x00008000,	// Bit 16 = PDFIX
        IsInCubelFile	= 0x00004000,	// Bit 17 = PDFCU
        IsBit18			= 0x00002000,	// Bit 18        
        IsCombination	= 0x00001000,	// Bit 19 = PDFCO
        IsNew			= 0x00000800,	// Bit 20 = PDFNW
        IsPut			= 0x00000400,	// Bit 21        
        IsDisplaced		= 0x00000200,	// Bit 22 = PDFDI
        IsExaggeration	= 0x00000100,	// Bit 23 = PDFEX
        IsBit24			= 0x00000080,	// Bit 24 	 
        IsBit25			= 0x00000040,	// Bit 25 	 
        IsBit26			= 0x00000020,	// Bit 26 	 
        IsBit27			= 0x00000010,	// Bit 27 	 
        IsBit28			= 0x00000008,	// Bit 28 	 
        IsScratch3		= 0x00000004,	// Bit 29 = PDFS3
        IsScratch2		= 0x00000002,	// Bit 30 = PDFS2
        IsScratch1		= 0x00000001,	// Bit 31 = PDFS1
#endif
    }
}
