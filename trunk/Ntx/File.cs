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
using System.IO;
using System.Diagnostics;

namespace Ntx
{
    /// <summary>
    /// An NTX file.
    /// </summary>
    public class File
    {
        #region Constants

        /// <summary>
        /// Number of 32-bit words in a block
        /// </summary>
        internal const int BLOCKSIZE=256;

        /// <summary>
        /// Number of bytes in a block
        /// </summary>
        internal const int BLOCKBYTES=1024;

        /// <summary>
        /// Number of words in descriptors
        /// </summary>
        internal const int DESL=16;

        /// <summary>
        /// Max number of words per DAD
        /// </summary>
        internal const int MAXLEN=300;

        /// <summary>
        /// Max number of words in an uncompacted delta-line DAD
        /// </summary>
        internal const int MAXYC1=1300;

        /// <summary>
        /// End of data marker
        /// </summary>
        internal const uint ECC=0x80000000;

        /// <summary>
        /// Offset to super-descriptor codes
        /// </summary>
        internal const int OFFSUD=1000;

        /// <summary>
        /// Min number of positions to allocate for m_Positions array
        /// </summary>
        internal const int MINALLOC=1024;

        #endregion

        #region Class data

        /// <summary>
        /// The file spec
        /// </summary>
	    string m_FileSpec;

        /// <summary>
        /// The opened file
        /// </summary>
        FileStream m_File;

        /// <summary>
        /// Buffer read from file.
        /// </summary>
    	byte[] m_RawBlock = new byte[BLOCKBYTES];
        int[] m_Block = new int[BLOCKSIZE];
 
        /// <summary>
        /// Next word to parse in the buffer.
        /// </summary>
	    byte m_NextWord;

        /// <summary>
        /// Header for the file
        /// </summary>
        Header m_Header = new Header();

        /// <summary>
        /// Last super-descriptor
        /// </summary>
        int[] m_SuperDesc = new int[DESL];

        /// <summary>
        /// Last graphics descriptor
        /// </summary>
	    int[] m_Desc = new int[DESL];

        /// <summary>
        /// Last data array
        /// </summary>
	    int[] m_Data = new int[MAXLEN];

        /// <summary>
        /// The last line
        /// </summary>
        Line m_Line = new Line();

        /// <summary>
        /// The last name
        /// </summary>
        Name m_Name = new Name();

        /// <summary>
        /// The last symbol
        /// </summary>
	    Symbol m_Symbol = new Symbol();

        /// <summary>
        /// Either m_Line, m_Name, or m_Symbol
        /// </summary>
        Feature m_Feature;

        /// <summary>
        /// Coordinate list for last feature
        /// </summary>
        Position[] m_Positions = new Position[MINALLOC];

        /// <summary>
        /// Number of positions defined in the m_Positions array
        /// </summary>
	    int m_NumPosition;

        /// <summary>
        /// Total number of bytes read so far.
        /// </summary>
	    uint m_BytesRead;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public File()
        {
        	m_FileSpec = String.Empty;
	        m_File = null;
        	m_NextWord = 0;
	        m_Feature = null;
        	m_Desc[0] = 0;
	        m_SuperDesc[0] = 0;
	        m_Data[0] = 0;
	        m_BytesRead = 0;
            m_NumPosition = 0;
        }

        #endregion

        public Header Header
        {
            get { return m_Header; }
        }

        /// <summary>
        /// Opens an ntx file (readonly)
        /// </summary>
        /// <param name="file">The NTX file specification (including any file extension).</param>
        /// <returns></returns>
        public bool Open(string file)
        {
            // Remember the file spec
            m_FileSpec = file;

            try
            {
                m_File = new FileStream(m_FileSpec, FileMode.Open);
                m_NextWord = 0;

                // Load the mainheader
                if (!m_Header.Read(this))
                {
                    this.Close();
                    return false;
                }
                return true;
            }

            catch {}
            return false;
        }

        /// <summary>
        /// Closes an ntx file (that was open readonly). 
        /// </summary>
        public void Close()
        {
            if (m_File!=null)
            {
                m_File.Close();
                m_File = null;
            }
        }

        unsafe void ReadArray(int[] buf)
        {
            fixed(int* pb = &buf[0])
            {
                Read((uint)buf.Length, pb);
            }
        }

        unsafe void Read(int nword, int[] buf)
        {
            Debug.Assert(nword>0);
            Debug.Assert(nword <= buf.Length);

            fixed(int* pb = &buf[0])
            {
                Read((uint)nword, pb);
            }
        }

        /// <summary>
        /// Reads the specified number of words from file into a buffer. The
        /// buffer is expected to be large enough to hold the requested number
        /// of 32-bit words.
        /// </summary>
        /// <param name="nword">The number of 32-bit words to read.</param>
        /// <param name="buf">The array to load the data into.</param>
        unsafe internal void Read(uint nword, int* buf)
        {
#if (!_VMS)
            byte[] filler = new byte[4];	// Junk stuff on UNIX & WIN32
#endif
            int bufIndex = 0;

            for (uint i=0; i<nword; i++, bufIndex++, m_NextWord++)
            {
                // Read next logical block from NTX if we have reached the
                // end. This relies on the fact that m_NextWord is UINT1, so
                // it will wrap when the count reaches 256 (which happens to
                // be the number of 32-bit words in each logical block).

                if (m_NextWord==0)
                {
#if (!_VMS)
                    m_File.Read(filler, 0, filler.Length);
                    m_BytesRead += (uint)filler.Length;
#endif

                    // Read array of bytes, then convert to an int array (since most
                    // other stuff works with int)
                    m_File.Read(m_RawBlock, 0, BLOCKBYTES);
                    for (int b=0, r=0; b<BLOCKSIZE; b++, r+=4)
                    {
                        m_Block[b] = ((int)m_RawBlock[r+3]) << 24 |
                                     ((int)m_RawBlock[r+2]) << 16 |
                                     ((int)m_RawBlock[r+1]) <<  8 |
                                     ((int)m_RawBlock[r  ]);
                    }
                    m_BytesRead += BLOCKBYTES;

#if (!_VMS)
                    m_File.Read(filler, 0, filler.Length);
                    m_BytesRead += (uint)filler.Length;
#endif
                }

                buf[bufIndex] = m_Block[m_NextWord];
            }
        }

        /// <summary>
        /// Checks if a file is currently open.
        /// </summary>
        /// <returns></returns>
        bool IsOpen()
        {
            return (m_File!=null);
        }

        /// <summary>
        /// Sees if the file contains more stuff.
        /// </summary>
        /// <returns>True if more stuff has been read. False at end of file.</returns>
        public bool GetMore()
        {
            return (this.GetNext()!=null);
        }

        /// <summary>
        /// Reads the next feature from the file.
        /// </summary>
        /// <returns>The next feature (null at end of file).</returns>
        Feature GetNext()
        {
            // Deactivate any feature previously found
	        m_Feature = null;

            // While we haven't got to something we can understand do
            while (m_Desc[0]==0)
            {
                // Read the next super-descriptor or descriptor
		        ReadArray(m_Desc);

                // Return if no more data in the file
		        if ((uint)m_Desc[0]==ECC)
                    return null;

                // If we have a super-descriptor, remember it, and read
                // the graphics descriptor that follows. Otherwise
                // remember that there was no super-descriptor.

		        if (m_Desc[Pointers.PDCODE] > OFFSUD)
                {
                    Array.Copy(m_Desc, m_SuperDesc, DESL);
        			ReadArray(m_Desc);
		        }
		        else
			        m_SuperDesc[0] = 0;

                // Read the data array as well
		        Read(m_Desc[Pointers.PDLNST]-DESL, m_Data);

                // Exit loop if the descriptor is something we can parse
		        if ( m_Desc[Pointers.PDCODE] == (int)Ntx.DataType.DeltaLine ||
			         m_Desc[Pointers.PDCODE] == (int)Ntx.DataType.Line ||
			         m_Desc[Pointers.PDCODE] == (int)Ntx.DataType.Name ||
			         m_Desc[Pointers.PDCODE] == (int)Ntx.DataType.Symbol)
                    break;

		        m_Desc[0] = 0;
            }

            // We now have a graphics descriptor, a data array, and
            // possibly a super-descriptor record. Parse the data.

            switch (m_Desc[Pointers.PDCODE])
            {
                case (int)Ntx.DataType.DeltaLine:
                case (int)Ntx.DataType.Line:
                {
		            SetLine();
		            break;
                }
                case (int)Ntx.DataType.Name:
                {
	                SetName();
	                break;
                }
                case (int)Ntx.DataType.Symbol:
                {
	                SetSymbol();
	                break;
                }
            }

            // If we just generated a symbol, and there are more symbols in
            // the DAD, don't de-activate the DAD. Otherwise de-activate by
            // setting the string length to zero. Note that the symbol
            // constructor adjusts the string length to account for the
            // fact that the symbol has been returned as a feature.

            if ( m_Desc[Pointers.PDCODE] == (int)Ntx.DataType.Symbol &&
		         m_Desc[Pointers.PDLNST] >= (DESL+m_Data[Pointers.PQHLEN]+m_Data[Pointers.PQRLEN]))
            {
		        // do nothing
	        }
	        else
                m_Desc[0] = 0;

            // Return the feature we created.
            Debug.Assert(m_Feature!=null);
        	return m_Feature;
        }

        /// <summary>
        /// Returns the position in the first data group. For symbols.
        /// </summary>
        Position FirstPosition
        {
            get
            {
                // Point to the start of the first data group.
                int p = m_Data[Pointers.PQHLEN];

                // Return a position, depending on whether the DAD has elevation
                // info or not.
                if (IsSet(Flag.Is3D))
                    return new Position(m_Header.XToGround(m_Data[p]),
                                        m_Header.YToGround(m_Data[p+1]));
                else
                    return new Position(m_Header.XToGround(m_Data[p]),
                                        m_Header.YToGround(m_Data[p+1]),
                                        m_Header.ZToGround(m_Data[p+2]));
            }
        }

        /// <summary>
        /// Checks if symbol is a topological node or not.
        /// </summary>
        private bool IsNode
        {
            get { return IsSet(Flag.IsNode); }
        }

        private bool IsSet(Flag f)
        {
            return IsSet(m_Desc[Pointers.PDFLAG], f);
        }

        private bool IsSet(int val, Flag f)
        {
            return ((uint)val & (uint)f) != 0;
        }

        /// <summary>
        /// Returns theme number. If there isn't one, you get the user number instead.
        /// </summary>
        private int Theme
        {
            get
            {
	            if ( m_SuperDesc[0] != 0 )
		            return m_SuperDesc[Pointers.PDTMNO];
	            else
		            return m_Desc[Pointers.PDSRNO];
            }
        }

        /// <summary>
        /// Returns the user number. 
        /// </summary>
        private int UserNum
        {
            get
            {
            	return m_Desc[Pointers.PDSRNO];
            }
        }

        /// <summary>
        /// Return what type of feature this is (but could be something
        ///	else if the file is screwed up).
        /// </summary>
        public int DataType
        {
            get
            {
	            if ( m_Desc[Pointers.PDCODE] == (int)Ntx.DataType.DeltaLine )
		            return (int)Ntx.DataType.Line;
	            else
		            return m_Desc[Pointers.PDCODE];
            }
        }

        /// <summary>
        /// Returns the feature code. 
        /// </summary>
        public string FeatureCode
        {
            get { return GetString(m_Desc, Pointers.PDFC, 12); }
        }

        unsafe private string GetString(int[] buf, int startIndex, int nByte)
        {
            fixed(int* pb = &buf[startIndex])
            {
                return Util.I4ch(pb, nByte);
            }
        }

        /// <summary>
        /// Returns the source ID.
        /// </summary>
        private string SourceId
        {
            get { return GetString(m_Desc, Pointers.PDSRID, 12); }
        }

        /// <summary>
        /// Returns the key (if any). If the feature has no key, or it is not marked
        /// as indexed, you get back a null string.
        /// </summary>
        private string Key
        {
            get
            {
                // For symbols, the key is in the data group. Otherwise its
                // in the superdescriptor.

                if ( m_Desc[Pointers.PDCODE] == (int)Ntx.DataType.Symbol )
                {
                    if (IsSet(Flag.IsIndexed))
                    {
                        int startIndex = m_Data[Pointers.PQHLEN] + Pointers.PSYKEY;
                        return GetString(m_Data, startIndex, 12);
                    }
                }
                else
                {
                    if (m_SuperDesc[0] != 0 && IsSet(m_SuperDesc[Pointers.PDFLAG], Flag.IsIndexed))
                        return GetString(m_SuperDesc, Pointers.PDKEY, 12);
                }

                return String.Empty;
            }
        }

        /// <summary>
        /// Gets the reference position for a name. 
        /// </summary>
        private Position RefPosition
        {
            get
            {
                if (m_Desc[Pointers.PDCODE] != (int)Ntx.DataType.Name)
                    return new Position();

                int p;
                if ( m_Data[Pointers.PQHLEN]-1 < Pointers.PQREFY )
                    p = m_Data[Pointers.PQHLEN];
                else
                    p = Pointers.PQREFX;

                return new Position(m_Header.XToGround(m_Data[p]),
                                    m_Header.YToGround(m_Data[p+1]));
            }
        }

        /// <summary>
        /// Gets a name string. Names cannot span DADs and, given the max
        /// sizes of DADs, this means that a name cannot exceed 87 characters.
        /// </summary>
        private string NameText
        {
            get
            {
                // How many data groups do we have?
	            int ngrp = this.NumGroup;
                Debug.Assert(ngrp>0);

                char[] chars = new char[ngrp];
                int ic=0;
                
                for( int startIndex = m_Data[Pointers.PQHLEN];
                     ic < chars.Length;
                     startIndex+=m_Data[Pointers.PQRLEN], ic++)
                {
                    int word = m_Data[startIndex + Pointers.PNANCH];
                    unsafe
                    {
                        char* wordChars = (char*)&word;
                        chars[ic] = wordChars[1]; 
                    }
                }

                return new String(chars);
            }
        }

        /// <summary>
        /// Checks if a name is a polygon label. 
        /// </summary>
        private bool IsLabel
        {
            get { return IsSet(Flag.IsLabel); }
        }

        /// <summary>
        /// Gets the height of the chars in a name (in meters on the ground).
        /// </summary>
        private float Height
        {
            get
            {
                if (m_Desc[Pointers.PDCODE] != (int)Ntx.DataType.Name)
                    return 0.0F;

        		int size = m_Data[Pointers.PQSIZE];
		        return (float)m_Header.DistanceToGround(size);
            }
        }

        /// <summary>
        /// Gets the rotation of the first character in a name (measured in minutes from the horizontal).
        /// </summary>
        private int FirstRotation
        {
            get
            {
                // Get pointer to the first data group.
                int p = m_Data[Pointers.PQHLEN];

                // Get the word that contains the angle and the character, and pull out 
                // the 2 bytes holding the angle (the bytes involved may vary, depending on the platform).
                int anch = m_Data[p+Pointers.PNANCH];
                int angle = (anch & 0x0000FFFF);

                // CARIS reckons the angle counter-clockwise, but we want it clockwise!
                return (360*60) - angle;
            }
        }

        /// <summary>
        /// Returns the number of data groups in the current DAD. 
        /// </summary>
        private int NumGroup
        {
            get
            {
                return (m_Desc[Pointers.PDLNST]-DESL-m_Data[Pointers.PQHLEN]) / m_Data[Pointers.PQRLEN];
            }
        }

        /// <summary>
        /// Checks whether a line is a topological arc or not. 
        /// </summary>
        private bool IsArc
        {
            get { return IsSet(Flag.IsArc); }
        }

        /// <summary>
        /// Check whether a line contains circular curve info or not (based on the size 
        /// of the data header). Only point-to-point lines can hold this sort of info.
        /// </summary>
        private bool IsCurve
        {
            get
            {
                return (m_Desc[Pointers.PDCODE] == (int)Ntx.DataType.Line 
                            && m_Data[Pointers.PQHLEN]-1 >= Pointers.PQCRAD);
            }
        }

        /// <summary>
        /// Gets the radius of a circular curve (in meters on the ground).
        /// </summary>
        private double Radius
        {
            get
            {
                return m_Header.DistanceToGround(m_Data[Pointers.PQCRAD]);
            }
        }

        /// <summary>
        /// Gets the position of the centre of a circular curve. 
        /// </summary>
        private Position Center
        {
            get
            {
                return new Position(m_Header.XToGround(m_Data[Pointers.PQCCEN]),
                                    m_Header.YToGround(m_Data[Pointers.PQCCEN+1]));
            }
        }

        /// <summary>
        /// Gets the position of the start of a circular curve. 
        /// </summary>
        private Position BC
        {
            get
            {
                return new Position(m_Header.XToGround(m_Data[Pointers.PQCSTA]),
                                    m_Header.YToGround(m_Data[Pointers.PQCSTA+1]));
            }
        }

        /// <summary>
        /// Gets the position of the end of a circular curve. 
        /// </summary>
        private Position EC
        {
            get
            {
                return new Position(m_Header.XToGround(m_Data[Pointers.PQCEND]),
                                    m_Header.YToGround(m_Data[Pointers.PQCEND+1]));
            }
        }

        /// <summary>
        /// Loads positions for a DAD.
        /// </summary>
        /// <returns>TRUE if the DAD is linked to another. In that case, the caller
        ///	must repeat the call, only stopping when this function returns FALSE.</returns>
        private bool LoadPositions()
        {
            // If we have a DAD containing delta coordinates, unpack them.
            // Otherwise get the number of positions we have.

            int npt;			        	// Number of points in current DAD
	        int[] uData = new int[MAXYC1];	// Unpacked code 1 data (if needed)
            int rlen = m_Data[Pointers.PQRLEN];

            if (m_Desc[Pointers.PDCODE] == (int)Ntx.DataType.DeltaLine)
            {
		        npt = Unpack(m_Data, m_Desc[Pointers.PDLNST]-DESL, uData);
        		m_Desc[Pointers.PDCODE] = (int)Ntx.DataType.Line;
                LoadPositions(uData, 0, npt, rlen, true);
	        }
	        else
            {
		        npt = this.NumGroup;
                bool removeDups = (m_Desc[Pointers.PDCODE] == (int)Ntx.DataType.Line);
                LoadPositions(m_Data, m_Data[Pointers.PQHLEN], npt, rlen, removeDups);
	        }

            // If this DAD is linked to another, read the next DAD and
            // load that one too.
            if (IsSet(Flag.IsLinked))
            {
                ReadArray(m_Desc);
                Read(m_Desc[Pointers.PDLNST]-DESL, m_Data);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads positions from a data array
        /// </summary>
        /// <param name="data">The array containing the data</param>
        /// <param name="dataIndex">The index where the first XY can be found</param>
        /// <param name="npt">The number of positions to load</param>
        /// <param name="rlen">The size of a data group (in 32-bit words)</param>
        /// <param name="removeDups">Should any duplicate points be removed (this should be
        /// done for line data types)</param>
        private void LoadPositions(int[] data, int dataIndex, int npt, int rlen, bool removeDups)
        {
            // Ensure there are no duplicate coordinates in lines
            if (removeDups)
            {
                int ndup = NoDuplicates(data, dataIndex, npt, rlen);
                npt -= ndup;
            }

            // Check whether we are dealing with elevations or not
            bool is3d = IsSet(Flag.Is3D);

            // How many do we have to copy? We copy everything only if
            // this is the first DAD in a chain. (Note that names cannot
            // be linked).
            if (m_NumPosition > 0)
            {
                dataIndex += rlen;
                npt--;
            }

            // If position array is not big enough to hold the positions
            // for this DAD, reallocate & copy what we have.
            if (m_NumPosition + npt > m_Positions.Length)
            {
                Position[] newarray = new Position[Math.Max(m_Positions.Length*2, m_NumPosition+npt)];
                Array.Copy(m_Positions, newarray, m_NumPosition);
                m_Positions = newarray;
            }

            // Point to the initial position we will define
            int pRes = m_NumPosition;

            // Remember how many we will have in the array once done
            m_NumPosition += npt;

            // Copy over the positions
            for (; npt>0; dataIndex+=rlen, npt--, pRes++)
            {
                if (is3d && data[dataIndex+3]==0)
                    m_Positions[pRes] = new Position(m_Header.XToGround(data[dataIndex]),
                                                     m_Header.YToGround(data[dataIndex+1]),
                                                     m_Header.ZToGround(data[dataIndex+2]));
                else
                    m_Positions[pRes] = new Position(m_Header.XToGround(data[dataIndex]),
                                                     m_Header.YToGround(data[dataIndex+1]));
            }
        }

        /// <summary>
        /// Checks an array of 2D or 3D line data to remove any consecutive duplicate points. If there
        /// are any, the rest of the array is scrunched up to overwrite them.
        /// </summary>
        /// <param name="data">The array containing the XY or XYZ data</param>
        /// <param name="dataIndex">The index where the first position can be found</param>
        /// <param name="npt">The number of XY or XYZ points in the array.</param>
        /// <param name="rlen">The number of words in successive data groups.</param>
        /// <returns>The number of duplicates detected (& removed).</returns>
        private int NoDuplicates(int[] data, int dataIndex, int npt, int rlen)
        {
	        int px=dataIndex;		// Initial position to check
	        int ndup=0;				// Number of duplicates found so far
	        int i;					// Loop counter

            // Scan the array looking for duplicates. If we find any, just
            // stick an ECC in the X-slot of the initial vertex. This assumes
            // that there are no ECCs to start with (i.e. no bad data).
	        if ( rlen==2 )
            {
                // Check XY

		        for (i=1; i<npt; i++, px+=2)
                {
			        if (data[px] == data[px+2] && data[px+1] == data[px+3])
                    {
				        ndup++;
                        unchecked { data[px] = (int)ECC; }
			        }
		        }
	        }
	        else
            {
                // Check XYZ

		        for (i=1; i<npt; i++, px+=rlen)
                {
			        if (data[px] == data[px+rlen] && data[px+1] == data[px+rlen+1] && data[px+2] == data[px+rlen+2])
                    {
				        ndup++;
                        unchecked { data[px] = (int)ECC; }
			        }
		        }
	        }

            // If we counted any duplicates, squash up the array.

	        if ( ndup>0 )
            {
		        int dst = dataIndex;
		        int src = dataIndex;
		        for (i=0; i<npt; i++, src+=rlen)
                {
                    int srcval = data[src];
			        if ((uint)srcval!=ECC)
                    {
                        if (src!=dst)
                        {
                            for (int r=0; r<rlen; r++)
                                data[dst+r] = data[src+r];
                        }
				        dst += rlen;
			        }
		        }
	        }

	        return ndup;
        }

        /// <summary>
        /// Unpack a code 1 data array into an XY list. 
        /// </summary>
        /// <param name="dta">The code 1 data to unpack.</param>
        /// <param name="dtalen"></param>
        /// <param name="xy">The resultant array of XYs.</param>
        /// <returns>The number of points</returns>
        private int Unpack(int[] dta, int dtalen, int[] xy)
        {
            // Each byte holds signed values in the range +/-127, with -128
            // marking a padder used to align data on a 32-bit word.
            const int PADDER = -128;

	        int     word;		// Word sequence# in data
	        int 	curpt;		// Current output point
	        bool    hasz;		// True if data has Zs
            int b0, b1, b2, b3; // Bytes in a word

            // Confirm the data header length is ok
	        if (dta[Pointers.PQHLEN] <= Pointers.PQXINC )
                return 0;

            // Unpack from either a 2D or 3D array.

	        if (dta[Pointers.PQRLEN]==2)
	            hasz = false;
	        else
            {
	            hasz = true;
	            if (dta[Pointers.PQRLEN]!=4)
                    return 0;
	        }

            // Pick up XYZ increment factors.
	        int xinc = dta[Pointers.PQXINC];
	        int yinc = dta[Pointers.PQYINC];

            // Get the point from which deltas get evaluated.
	        xy[0] = Round(dta[Pointers.PQSTRT], xinc);
	        xy[1] = Round(dta[Pointers.PQSTRT+1], yinc);
	        int npt = 1;

            // Loop through the packed deltas (each word contains one
            // (dX,dY) pair if the data has Zs, and two pairs if no Zs).

	        if (hasz)
            {
                int zinc = dta[Pointers.PQZINC];
                xy[2] = Round(dta[Pointers.PQSTAZ], zinc);
                xy[3] = 1;

                for ( word=dta[Pointers.PQHLEN], curpt=0;
                      word<dtalen; //&& (uint)dta[word]!=ECC;
                      word++, npt++, curpt+=4)
                {
                    UnpackWord(dta[word], out b0, out b1, out b2, out b3);

                    xy[curpt+4] = xy[curpt  ] + (xinc * b0);
                    xy[curpt+5] = xy[curpt+1] + (yinc * b1);

                    // If the delta-Z is a padder, we must have a bad Z.

                    if (b2==PADDER)
                    {
                        xy[curpt+6] = xy[curpt+2];
                        xy[curpt+7] = 0;
                    }
                    else
                    {
                        xy[curpt+6] = xy[curpt+2] + (zinc * b2);
                        xy[curpt+7] = 1;
                    }
                }
	        }
            else
            {
                // 2D data

                for ( word=dta[Pointers.PQHLEN], curpt=0;
                      word<(dtalen-2);
                      word++, npt+=2, curpt+=4 )
                {
                    UnpackWord(dta[word], out b0, out b1, out b2, out b3);

                    xy[curpt+2] = xy[curpt+0] + (xinc * b0);
                    xy[curpt+3] = xy[curpt+1] + (yinc * b1);
                    xy[curpt+4] = xy[curpt+2] + (xinc * b2);
                    xy[curpt+5] = xy[curpt+3] + (yinc * b3);
                }

                // Do the very last word, checking to see if the last delta
                // is a padder.

                UnpackWord(dta[word], out b0, out b1, out b2, out b3);

                if (b0 != PADDER)
                {
                    xy[curpt+2] = xy[curpt+0] + (xinc * b0);
                    xy[curpt+3] = xy[curpt+1] + (yinc * b1);
                    curpt+=2;
                    npt++;
                }

                if (b2 != PADDER)
                {
                    xy[curpt+2] = xy[curpt+0] + (xinc * b2);
                    xy[curpt+3] = xy[curpt+1] + (yinc * b3);
                    curpt+=2;
                    npt++;
                }
            }

            // Replace the first & last coordinates with the true end points.

	        xy[curpt] = dta[Pointers.PQSTOP  ];
	        xy[curpt+1] = dta[Pointers.PQSTOP+1];

	        xy[0] = dta[Pointers.PQSTRT];
	        xy[1] = dta[Pointers.PQSTRT+1];

            if (hasz)
            {
		        xy[curpt+2] = dta[Pointers.PQSTPZ];
		        xy[curpt+3] = 1;
		        xy[2] = dta[Pointers.PQSTAZ];
		        xy[3] = 1;
	        }

	        return npt;
        }
    
        unsafe private void UnpackWord(int val, out int b0, out int b1, out int b2, out int b3)
        {
            sbyte* b = (sbyte*)&val;
            b0 = b[0];
            b1 = b[1];
            b2 = b[2];
            b3 = b[3];

            /*
             * This isn't quite right, since it doesn't handle the sign bit
             * 
            b0 = val & 0x000000FF;
            b1 = (val & 0x0000FF00) >> 8;
            b2 = (val & 0x00FF0000) >> 16;
            b3 = (int)((uint)val & 0xFF000000) >> 24;
             */
        }

        /// <summary>
        /// Round an integer to the closest value on a grid. 
        /// </summary>
        /// <param name="val">The value to round off.</param>
        /// <param name="incr">The size of the grid (in disk units).</param>
        /// <returns>The rounded value.</returns>
        int Round(int val, int incr)
        {
	        if (incr<=1)
                return val;

	        int excess = val % incr;
	        if ( Math.Abs(excess) <= (int)incr/2 )
	            return (val-excess);

            if (excess<0)
                return (val-incr-excess);

            return (val+incr-excess);
        }

        /// <summary>
        /// Decodes the current feature.
        /// </summary>
        private void SetFeature()
        {
            m_Feature.Theme = this.Theme;
            m_Feature.UserNum = this.UserNum;
            m_Feature.DataType = this.DataType;

            m_Feature.NorthWest = this.NorthWest;
            m_Feature.SouthEast = this.SouthEast;
            m_Feature.FeatureCode = this.FeatureCode;
            m_Feature.SourceId = this.SourceId;
            m_Feature.Key = this.Key;
        }

        /// <summary>
        /// The north-west corner of the current feature's cover.
        /// </summary>
        private Position NorthWest
        {
            get
            {
                return new Position(m_Header.XToGround(m_Desc[Pointers.PDMINX]),
                                    m_Header.YToGround(m_Desc[Pointers.PDMAXY]));
            }
        }

        /// <summary>
        /// The south-east corner of the current feature's cover.
        /// </summary>
        private Position SouthEast
        {
            get
            {
                return new Position(m_Header.XToGround(m_Desc[Pointers.PDMAXX]),
                                    m_Header.YToGround(m_Desc[Pointers.PDMINY]));
            }
        }

        /// <summary>
        /// Decodes the current line DAD.
        /// </summary>
        private void SetLine()
        {
            // Define the line as the active feature
            m_Feature = (Ntx.Feature)m_Line;

            // Define characteristics common to all features
            this.SetFeature();

            // Initialize the simple stuff
            m_Line.IsTopologicalArc = this.IsArc;
            m_Line.IsCurve = this.IsCurve;
            if (m_Line.IsCurve)
            {
                m_Line.Radius = this.Radius;
                m_Line.Center = this.Center;
                m_Line.BC = this.BC;
                m_Line.EC = this.EC;
            }
            else
            {
                m_Line.Radius = 0.0;
                m_Line.Center = null;
                m_Line.BC = null;
                m_Line.EC = null;
            }

            //	Load data for the line, chaining through the DADs if required.

            m_NumPosition = 0;
            bool islinked = this.LoadPositions();
            while (islinked)
            {
                islinked = this.LoadPositions();
            }
            m_Line.NumPosition = m_NumPosition;
            m_Line.Positions = m_Positions;
        }

        /// <summary>
        /// Decodes the current name DAD.
        /// </summary>
        private void SetName()
        {
            //	Define the name as the active feature
            m_Feature = (Feature)m_Name;

            //	Define characteristics common to all features
            this.SetFeature();

            m_Name.RefPosition = this.RefPosition;
            m_Name.Text = this.NameText;
            m_Name.IsLabel = this.IsLabel;
            m_Name.Height = this.Height;
            //m_Name.Rotation = this.FirstRotation;

            // Hold the position of each character in the file's position list.
            int nchar = m_Name.Count;
            m_NumPosition = 0;

            // Names SHOULDN'T be linked, but let's see what happens if they are ...
            bool islinked = this.LoadPositions();
            while (islinked)
            {
                islinked = this.LoadPositions();
            }

            m_Name.Positions = m_Positions;
        }

        /// <summary>
        /// Decodes the current symbol DAD.
        /// </summary>
        private void SetSymbol()
        {
            // Define the symbol as the active feature
            m_Feature = (Feature)m_Symbol;

            // Define characteristics common to all features
            this.SetFeature();

            // The location always refers to the first data group
            m_Symbol.Position = this.FirstPosition;

            // Check if this symbol is a topological node
            m_Symbol.IsNode = this.IsNode;

            // If there are actually more symbols in the DAD, shift them back
            this.RemoveFirstGroup();
        }

        /// <summary>
        /// Eliminates the first data group from the current DAD. At the
        /// moment, this just shifts the rest of the data back and adjusts
        ///	the length of the DAD. It does not bother reworking the window.
        /// Intended for symbol processing.
        /// </summary>
        private void RemoveFirstGroup()
        {
            // Get pointers to the location just after the data header, and
            // the data group (if any) that follows it.
            int dstIndex = m_Data[Pointers.PQHLEN];
            int srcIndex = dstIndex + m_Data[Pointers.PQRLEN];

            // Copy back stuff.
            int ncopy = m_Desc[Pointers.PDLNST] - DESL - m_Data[Pointers.PQHLEN] - m_Data[Pointers.PQRLEN];
            for (; ncopy>0; ncopy--, dstIndex++, srcIndex++)
            {
                m_Data[dstIndex] = m_Data[srcIndex];
            }

            // Adjust total length of the DAD
            m_Desc[Pointers.PDLNST] -= m_Data[Pointers.PQRLEN];
        }

        public Ntx.Line Line
        {
            get { return m_Line; }
        }

        public Ntx.Name Name
        {
            get { return m_Name; }
        }

        public Ntx.Symbol Symbol
        {
            get { return m_Symbol; }
        }
    }
}
