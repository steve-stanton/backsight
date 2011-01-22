#pragma once
/*
	The following structure defines the form of the Ellipsoid
	Dictionary.  This file has a 4 byte magic number on the front
	which is followed by any number of the following structures.
	The list is maintained in order by key name and is accessed via
	binary search techniques.  CS_dtloc puts together a datum
	definition (cs_Dtdef_) and an ellipsoid definition (cs_Eldef_)
	and returns a complete datum definition structure (cs_Datum_).

	Therefore, take care not to get these three different
	structures confused.  In general, programs which manipulate
	the contents of the dicionaries involved will used the
	cs_Eldef_ and cs_Dtdef_ structures while programs which
	actually perform calculations will use CS_dtloc to obtain
	a pointer to a malloc'ed cs_Datum_ structure and use its
	contents to perform all calculations.

	In the following structure, the units of the radii are (now)
	required to be in meters.
*/

using namespace System;

public ref class EllipsoidDef
{
public:

	/* Key name, used to locate a specific entry in the table. */
	property String^ KeyName; // key_nm [24]

	/* Used to group ellipsoids for selection purposes. */
	property String^ Group; // group [6]

	/* Equatorial radius IN METERS. */
	property double EquatorialRadius; // e_rad

	/* Polar radius IN METERS. */
	property double PolarRadius; // p_rad

	/* Flattening ratio, e.g. 1.0/297.0 */
	property double Flattening; // flat

	/* Ecentricity. */
	property double Eccentricity; // ecent

	/* Full descriptive name of the datum. */
	property String^ Name; // name [64]

	/* Description of where the data for this coordinate system came from. */
	property String^ Source; // source [64]

	/* TRUE indicates that this is a distribution definition.  Typically
	used to require a confirmation before modification. */
	property short Protect; // protect

	/* EPSG Code number for this definition, if known.*/
	property short EPSGNumber; // epsgNbr

	/* WKT flavor, if dervied from WKT, else zero */
	property short WKTFlavor; // wktFlvr
};
