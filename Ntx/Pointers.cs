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
    static class Pointers
    {
        // Define offsets into integer records (0-based)
 
        internal const int PDLNST   =0;     // Length of DAD (in words)
        internal const int PDTMNO   =1;     // Theme number
        internal const int PDSRNO   =1;     // User number
        internal const int PDKEY    =2;     // Key
        internal const int PDFC     =2;     // Feature code
        internal const int PDMINX   =5;     // Minimum X
        internal const int PDMINY   =6;     // Minimum Y
        internal const int PDMAXX   =7;     // Maximum X
        internal const int PDMAXY   =8;     // Maximum Y
        internal const int PDMINZ   =9;     // Minimum Z
        internal const int PDMAXZ   =10;    // Maximum Z
        internal const int PDFLAG   =11;    // Flag word
        internal const int PDCODE   =12;    // Data code
        internal const int PDSRID   =13;    // Source ID
        internal const int PDENC    =16;    // Enclosing polygon
        internal const int PDBPTR   =16;    // Prev desc [BPTR]
        internal const int PDISL    =17;    // First island
        internal const int PDFPTR   =17;    // Next desc [FPTR]
        internal const int PDFCOA   =18;    // First component arc
        internal const int PDBDES   =18;    // First DES in chain
        internal const int PDREC    =18;    // Data record [REC]
        internal const int PDEDES   =19;    // Last DES in chain
        internal const int PDOFFS   =19;    // Data offset [OFFSET]
        internal const int PDLBPT   =19;    // Pointer to label
        internal const int PDHNUM   =20;    // Header number [SRCHD]
        internal const int PDVIR    =21;    // Virtual DAD (unused)
        internal const int PDREAL   =22;    // Real DAD (unused)
        internal const int PDSUD    =23;    // Super-descriptor [SUDPTR]
        internal const int PDLENG   =23;    // Length
        internal const int PDNFLG   =23;    // Node flag [PDNFLG]
        internal const int PDHDAR   =24;    // Head arc [PDHDAR]
        internal const int PDAREA   =25;    // Area
        internal const int PDAFLG   =27;    // Arc flag
        internal const int PDLB     =28;    // Line left of line begin
        internal const int PDNXIS   =29;    // Next island
        internal const int PDRB     =29;    // Line right of line begin
        internal const int PDLE     =30;    // Line left of line end
        internal const int PDRE     =31;    // Line right of line end
        internal const int PDPOL    =32;    // Polygon on left of line
        internal const int PDPOR    =33;    // Polygon on right of line
        internal const int PDBNOD   =34;    // Node at line begin
        internal const int PDENOD   =35;    // Node at line end

        // Define pointers into data headers.
 
        internal const int PQHLEN   =0;
        internal const int PQRLEN   =1;
        internal const int PQSCAL   =2;
        internal const int PQLNWT   =3;
        internal const int PQSTRT   =4;
        internal const int PQSTOP   =6;
        internal const int PQXINC   =8;
        internal const int PQYINC   =9;
        internal const int PQSMTH   =10;
        internal const int PQSTAZ   =11;
        internal const int PQSTPZ   =12;
        internal const int PQZINC   =13;

        internal const int PQFONT   =4;
        internal const int PQSIZE   =5;

        internal const int PQREFX   =6;
        internal const int PQREFY   =7;
        internal const int PQREFZ   =8;

        internal const int PQRAST   =3;
        internal const int PQBPPX   =4;

        internal const int PQCTYP   =4;
        internal const int PQCSTA   =5;		// BC
        internal const int PQCCEN   =7;		// Centre
        internal const int PQCEND   =9;		// EC
        internal const int PQCRAD   =11;	// Radius
        internal const int PQCARC   =12;	// Length (?)

        // Offsets in data records for block text.

        internal const int PQTETP   =3;
        internal const int PTEXREF  =4;
        internal const int PTEYREF  =5;
        internal const int PTEZREF  =6;
        internal const int PTEZFLG  =7;
        internal const int PTEANG   =8;
        internal const int PTEJUS   =9;
        internal const int PTECH1   =0;
        internal const int PTECH2   =1;

        // Offsets in data records for names

        internal const int PNAPOS   =0;
        internal const int PNANCH	=2;

        // Offsets in data records for symbols
 
        internal const int PSYOPC   =0;
        internal const int PSYID    =2;
        internal const int PSYDSK   =2;
        internal const int PSYNUM   =3;
        internal const int PSYANS   =4;
        internal const int PSYFLG   =5;
        internal const int PSYKEY   =6;

        // Offsets in data records for soundings
        
        internal const int PSGOPC   =0;
        internal const int PSGDEP   =2;
        internal const int PSGFLG   =3;
        internal const int PSGANG   =4;
        internal const int PSGKEY   =5;
        internal const int PSGTR    =6;
        internal const int PSGTRU   =7;
        internal const int PSGYCO   =11;
        internal const int PSGTIM   =12;
        internal const int PSGACF   =13;
        internal const int PSGLID   =14;
        internal const int PSGDRA   =15;

        // Offsets in data records for spot heights

        internal const int PSPTRU   =0;
        internal const int PSPFLG   =3;
        internal const int PSPPLB   =4;
        internal const int PSPSYM   =6;
        internal const int PSPANS   =8;
        internal const int PSPSPA   =9;
        internal const int PSPALB   =10;
        internal const int PSPKEY   =11;

        //	Offsets to fields in NTX headers
        
        internal const int PHMLEN	=0;	    // Length of main header
        internal const int PHDLEN   =1;	    // Length of descriptors
        internal const int PHFMT	=2;	    // Format ID
        internal const int PHFID	=3;	    // File ID
        internal const int PHFTTL	=6;	    // File title
        internal const int PHXCRD	=26;	// Coord system for horizontal positions
        internal const int PHZCRD	=27;	// Coord system for heights and depths
        internal const int PHXRES	=28;	// X resolution
        internal const int PHXOFF	=34;	// X offset
        internal const int PHYRES	=40;	// Y resolution (should match X-res)
        internal const int PHYOFF	=46;	// Y offset
        internal const int PHZRES	=52;	// Z resolution
        internal const int PHZOFF	=58;	// Z offset
        internal const int PHPROJ	=64;	// Projection
        internal const int PHSCAL	=65;	// Scale
        internal const int PHCLON	=71;	// Central meridian
        internal const int PHSLT1	=77;	// Scaling latitude 1
        internal const int PHSLT2	=83;	// Scaling latitude 2
        internal const int PHELPS	=89;	// Ellipsoid definition
        internal const int PHDATM	=90;	// Datum
        internal const int PHSFAC	=91;	// Scaling factor for UTM or TM
        internal const int PHALXY	=97;	// Alignment line in XY
        internal const int PHALLL	=101;	// Alignment line in lat/long
        internal const int PHCRXY	=125;	// Corners in XY
        internal const int PHCRLL	=133;	// Corners in lat/long
        internal const int PHSGUN	=181;	// Sounding and spot height display units
        internal const int PHDATE	=182;	// Date of last editting
        internal const int PHTIME	=183;	// Time of last editting
        internal const int PHTZON	=184;	// Time zone in minutes (+/- from GMT)
        internal const int PHFLSY	=185;	// False Northing (Y) for projection
        internal const int PHFLSX	=192;	// False Easting (X) for projection
    }
}
