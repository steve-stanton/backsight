#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace Backsight;

public ref class DatumDef : IExpandablePropertyItem
{
public:

	/* Key name, used to locate a specific entry in the dictionary. */
	property String^ KeyName; // key_nm

	/* Ellipsoid key name. Used to access the Ellipsoid Dictionary to obtain
	the ellipsoid upon which the datum is based. */
	property String^ EllipsoidKeyName; // ell_knm

	/* The classification into which this datum falls. */
	property String^ Group; // group

	/* Field by which coordinate systems can be classified by location, for
	example, world, North America, Central America, Europe, etc.  To be used
	by selector programs. */
	property String^ Location; // locatn

	/* Up to 24 two character codes which define the countries (or US states) in which
	this coordinate system is designed to be used.  We use the US postal code
	abbreviations for states in lower case, and POSC country abbreviations in upper
	case. This also, is intended for use by a coordinate system selector program. */
	property String^ CountriesOrStates; // cntry_st

	/* The following values are usually extracted from DMA Technical Report 8350.2-B */

	/* X component of the vector from the WGS-84 geocenter to the geocenter of
	this datum. */
	property double DeltaX; // delta_X

	/* Y component of the vector from the WGS-84 geocenter to the geocenter of
	this datum. */
	property double DeltaY; // delta_Y

	/* Z component of the vector from the WGS-84 geocenter to the geocenter of
	this datum. */
	property double DeltaZ; // delta_Z

	/* If a Bursa/Wolfe definition has been made, one of the following values will
	be non-zero.  If a pure Molodensky definition is provided, the values
	of the rot_X, rot_Y, rot_Z, and bwscale are all set to hard zero.
	This is the switch which is used to determine is a Bursa/Wolf conversion
	has been defined as opposed to a standard Molodensky. */

	/* Angle from WGS-84 X axis to local geodetic system X axis in arc
	seconds, east is positive. */
	property double RotationX; // rot_X

	/* Angle from WGS-84 Y axis to local geodetic system Y axis in arc
	seconds, north is positive. */
	property double RotationY; // rot_Y

	/* Angle from WGS-84 Z axis to local geodetic system Z axis in arc
	seconds, use right hand rule. */
	property double RotationZ; // rot_Z

	/* Scale factor in parts per million. Don't include the base 1.0; we
	add that at setup time. */
	property double BursaWolfeScale; // bwscale

	/* Full descriptive name of the datum. */
	property String^ Name; // name

	/* Description of where the data for this coordinate system came from. */
	property String^ Source; // source

	/* TRUE indicates that this is a  distribution definition.  Typically
	requires confirmation for change. */
	property short Protect; // protect

	/* Conversion technique, one of:
		cs_DTCTYP_MOLO
		cs_DTCTYP_BURS
		cs_DTCTYP_MREG
		cs_DTCTYP_NAD27
		cs_DTCTYP_NAD83
		cs_DTCTYP_WGS84
		cs_DTCTYP_WGS72
		cs_DTCTYP_HPGN
		cs_DTCTYP_AGD66
		cs_DTCTYP_GEOCTR
		cs_DTCTYP_6PARM
		cs_DTCTYP_4PARM
		cs_DTCTYP_AGD84
		cs_DTCTYP_NZGD49
		cs_DTCTYP_ATS77
		.
		.
		.
		*/
	property short ToWGS84Via;

	/* EPSG number, if known */
	property short EPSGNumber; // epsgNbr

	/* WKT flavor, if dervied from WKT, else zero */
	property short WKTFlavor; // wktFlvr
};
