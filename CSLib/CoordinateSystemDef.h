#pragma once

using namespace System;

namespace CSLib
{
	public ref class CoordinateSystemDef
	{
	public:

		/* The name used to identify the coordinate system. */
		property String^ KeyName; // key_nm [24]

		/* The key name of the datum upon which the coordinate system is based. */
		property String^ DatumKeyName; // dat_knm [24]
		
		/* The key name of the ellipsoid upon which the coordinate system is based. */
		property String^ EllipsoidKeyName; // elp_knm [24]
								   
		/* The key name of the projection upon which the coordinate system is based,
		eight characters max. */
		property String^ ProjectionKeyName; // prj_knm [24]

		/* The classification into which this coordinate system falls.  I.e.
		State Plane 27, State Plane 83, UTM 27, etc. */
		property String^ Group; // group [24]

		/* Field by which coordinate systems can be classified by location, for example,
		world, North America, Central America, Europe, etc.  To be used by selector
		programs. */
		property String^ Location; // locatn [24]

		/* Up to 24 two character codes which define the countries (or US states) in which
		this coordinate system is designed to be used.  We use the US postal code
		abbreviations for states in lower case, and POSC country abbreviations in upper
		case. This also, is intended for use by a coordinate system selector program. */
		property String^ CountriesOrStates; // cntry_st [48]

		/* The name of the units of the coordinate system, i.e. the units of the resulting
		coordinate system. */
		property String^ Units; // unit [16]

		/* Twenty four projection parameters. The actual contents depend upon the
		projection.  For example, for Transverse Mercator only prj_prm1
		is used and it contains the Central Meridian.  Values in degrees as
		opposed to radians. */

		property double Param01; // prj_prm1
		property double Param02; // prj_prm2
		property double Param03; // prj_prm3
		property double Param04; // prj_prm4
		property double Param05; // prj_prm5
		property double Param06; // prj_prm6
		property double Param07; // prj_prm7
		property double Param08; // prj_prm8
		property double Param09; // prj_prm9
		property double Param10; // prj_prm10
		property double Param11; // prj_prm11
		property double Param12; // prj_prm12
		property double Param13; // prj_prm13
		property double Param14; // prj_prm14
		property double Param15; // prj_prm15
		property double Param16; // prj_prm16
		property double Param17; // prj_prm17
		property double Param18; // prj_prm18
		property double Param19; // prj_prm19
		property double Param20; // prj_prm20
		property double Param21; // prj_prm21
		property double Param22; // prj_prm22
		property double Param23; // prj_prm23
		property double Param24; // prj_prm24

		/* The origin of the projection.  Values are in degrees.  For several
		projections, parm1 carries the origin longitude (i.e. central meridian). */
		property double LongitudeOrigin; // org_lng
		property double LatitudeOrigin;  // org_lat

		/* The false easting to be applied to keep X coordinates positive.  Values are in the
		units of the resulting coordinates. */
		property double FalseEasting; // x_off

		/* The false northing to be applied to keep the Y coordinates positive.  Values are in
		the units of the resulting coordinates. */
		property double FalseNorthing; // y_off

		/* The scale reduction which is used on some projections to distribute
		the distortion uniformily across the map, else 1.0. */
		property double ScaleReduction; // scl_red

		/* The scale factor required to get from coordinate system units to meters
		by multiplication.  This factor is used to convert scalars (i.e. text height,
		elevations, etc.) in the system unit to meters by multiplication.  It is also used
		to convert scalars from meters to the system unit by division. */
		property double UnitsToMetersFactor; // unit_scl

		/* The scale factor to get to the desired map scale by division (e.g. 24000 for a
		USGS topo quad map).  This feature of CS_MAP is only used when one is trying
		to produce inches, millimeters, etc. on an existing map.  In this case, one sets
		the unit to inches, millimeters, whatever, and sets this value appropriately.  Usually,
		this value is set to 1.0. */
		property double MapScaleFactor; // map_scl

		/* A single scale factor which includes all the unit scale and the map scale
		factors defined above.  This factor must convert meters to coordinate system
		units by multiplication.  Therefore, it is essentially:
								   
			1.0 / (unit_scl * map_scl).

		This value is used to convert the ellipsoid equatorial radius to system units before
		all other calculations are made.  This variable exists primarily for historical
		reasons. */
		property double OldScaleFactor; // scale

		/* Absolute values of X & Y which are smaller than this are to be converted
		to a hard zero.  Set by the compiler to the system unit equivalent of .01
		millimeters by the setup function. This feature is included to prevent
		output such as 2.345E-05 which is usually undesirable. */
		property double ZeroX; // zero [2]
		property double ZeroY;

		/* The following values are set to zero by the compiler and are an attempt to
		prepare for future changes. */

		/* Longitude of the elevation point. */
		property double ElevationPointLongitude; // hgt_lng

		/* Latitude of the elevation point. */
		property double ElevationPointLatitude; // hgt_lat

		/* Elevation of the coordinate system; typically the actual elevation at
		elevation average point.  This is an orthometric height, i.e. height
		above the geoid. */
		property double Elevation; // hgt_zz

		/* If defined by the user, the height of the geoid above the ellipsoid, also
		known as the geoid separation, at the elevation point. */
		property double GeoidSeparation; // geoid_sep

		/* Lat/Longs outside the rectangle established by the following
		cause a warning message when converted???  If the min is greater than the max, the
		setup function generates these automatcially. */
		property double MinLatitude;	// ll_min [2]
		property double MinLongitude;
		property double MaxLatitude;	// ll_max [2]
		property double MaxLongitude;

		/* XY's outside the rectangle established by the following
		cause a warning message when converted???  If the min is greater than the max, the
		setup function generates these automatcially. */
		property double MinX; // xy_min [2]
		property double MinY;
		property double MaxX; // xy_max [2]
		property double MaxY;

		/* The complete name of the coordinate system. */
		property String^ Description; // desc_nm [64]

		/* Description of where the data for this coordinate system came from. */
		property String^ Source; // source [64]

		/* Quadrant of the cartesian coordinates. Used to handle coordinate systems in
		which X increases to the left, etc. */
		property short Quadrant; // quad

		/* Order of the complex series, if any used for this coordinate system.
		Order is currently computed automatically, so this field is currently ignored. */
		property short ComplexSeriesOrder; // order;

		/* Number of zones in an interrupted coordinate system definition, such
		as sinusoidal and Goode homolosine. Currently, the number of zones is
		automatically calculated and this field is ignored. This, of course,
		could change. */
		property short NumberOfZones; // zones

		/* Set to TRUE is this definition is to be protected from being changed by users. */
		property short Protect; // protect

		/* In the same form as the quad member of this structure, this elelment carries
		quad as specified by the EPSG database, originally populated with values from
		EPSG 7.05 */
		property short EPSGQuad; // epsg_qd

		/* The Oracle SRID number if known, else 0. */
		property short OracleSRID; // srid

		/* EPSG number, if known */
		property short EPSGNumber; // epsgNbr

		/* WKT flavor, if dervied from WKT, else zero */
		property short WKTFlavor; // wktFlvr
		
		virtual String^ ToString() override
		{
			return this->Description;
		}
	};
}
