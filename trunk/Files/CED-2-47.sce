//*********************************************************************
//*                                                                   *
//*  SCHEMA.SCH - File defining the Attribute Structure for the       *
//*               Cadastral Editor Program                            *
//*                                                                   *
//*  Created: 1-Dec-1997 - Bob Bruce                                  *
//*                                                                   *
//*  Modified:20-Jun-1999 - Bob Bruce                                 *
//*           - changed Survey Plan theme to Registered Plan theme to *
//*             include Special Plot Plans in order to allow them to  *
//*             be subdivided by ordinary survey plans                *
//*                                                                   *
//*  Modified:11-Oct-2000 - Bob Bruce                                 *
//*           - added road widening annotation for annotating these   *
//*             types of non-topological polygons                     *
//*                                                                   *
//*  Modified:12-Oct-2000 - Bob Bruce - version 2.4                   *
//*           - added template to SCHEMA: "DLS Parcel Data to allow   *
//*             the labelling of legal subdivision parcels            *
//*                                                                   *
//*  Modified:30-Nov-2000 - Bob Bruce - version 2.41                  *
//*           - added Entity Parcel of Certificate of Title Parcel    *
//*             added Schema Parcel of Certificate of Title Parcel    *
//*             Data and added FieldSchema Parcel ID and Template     *
//*             Parcel & Title Annotation in order to accommodate     *
//*             Parcels in Certificates of Titles                     *
//*                                                                   *
//*  Modified:06-Feb-2001 - Bob Bruce - version 2.42                  *
//*           - added Entity Parcel of Certificate of Title Parcel    *
//*             added Schema Parcel of Certificate of Title Parcel    *
//*                                                                   *
//*  Modified:17-Jul-2002 - Bob Bruce - version 2.43                  *
//*           - added Entity Property Map Polygon                     *
//*                                                                   *
//*  Modified:26-Sep-2002 - Bob Bruce - version 2.44                  *
//*           - added Entities InProgress and Future Property Map     *
//*             Polygons                                              *
//*                                                                   *
//*  Modified:18-Feb-2004 - Bob Bruce (my birthday - 50!)             *
//*           - the conversion to Win XP produced a WIERD ERROR:      *
//*             it crashes when CEdit starts up, the FieldSchema      *
//*             Location had to be enclosed in " to make it work      *
//*                                                                   *
//*  Modified:27-Jul-2004 - Bob Bruce - version 2.45                  *
//*           - added Entity Mining Claim Line-NonTopological for     *
//*             displaying the edge of Mining Claims                  *
//*                                                                   *
//*  Modified:30-Nov-2006 - Bob Bruce - version 2.46                  *
//*           - added Contour Lines to Registered Plan layer          *
//*                                                                   *
//*  Modified:16-Jan-2008 - Bob Bruce - version 2.47                  *
//*           - the double quote problem reared it's ugly head again: *
//*             it crashes when CEdit starts up, the FieldSchema      *
//*             Datum had to be enclosed in " to make it work         *
//*                                                                   *
//*********************************************************************
//
// The following list of names is for the purpose of ensuring that all
// IDs of Entities, Schemas, FieldSchemas, and Domains are unique - a
// requirement of the Attribute Structure
//

// The Header for the Attribute Structure must appear before any other non-comment data
<ATTRIBUTESTRUCTURE: "Manitoba LIC Cadastral Schema"
  DESCRIPTION: "Specification of all entities & their attributes for the Manitoba LIC Cadastre"
  VERSION: 2.4700
>

//
// The start of the Attribute Definitions
//

//*********************************************************************
//* Theme Features                                                    *
//*********************************************************************
<THEME: "Registered Plan"
  DESCRIPTION: "Registered Survey Plan Layer"
  PARENT: NONE
  DEFAULTLINE: "Property Line"
  DEFAULTPOINT: "Computed Point"
  DEFAULTPOLYGON: "Plan Parcel or Lot"
>

<THEME: "Ownership"
  DESCRIPTION: "Property Ownership Layer"
  PARENT: "Registered Plan"
  DEFAULTLINE: "Ownership Property Line"
  DEFAULTPOINT: "Computed Ownership Point"
  DEFAULTPOLYGON: "Certificate of Title Parcel"
>

<THEME: "Assessment"
  DESCRIPTION: "Property Assessment Layer"
  PARENT: "Ownership"
  DEFAULTLINE: "Assessment Property Line"
  DEFAULTPOINT: "Computed Assessment Point"
  DEFAULTPOLYGON: "Assessment Parcel"
>

//*********************************************************************
//* Entity Features                                                   *
//*********************************************************************
//*********************************************************************
//*      Point Entity Features                                        *
//*********************************************************************

<ENTITY: "Computed Assessment Point"
  DESCRIPTION: "Assessment Point computed as part of cadastral mapping"
  THEME: "Assessment"
  GRAPHICSTYPE:  POINT
  TOPOLOGICAL: YES
>

<ENTITY: "Computed Point"
  DESCRIPTION: "Point computed as part of cadastral mapping"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  POINT
  TOPOLOGICAL: YES
>

<ENTITY: "Computed Ownership Point"
  DESCRIPTION: "Ownership Point computed as part of cadastral mapping"
  THEME: "Ownership"
  GRAPHICSTYPE:  POINT
  TOPOLOGICAL: YES
>

<ENTITY: "Control Point"
  DESCRIPTION: "Point tied to control survey"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  POINT
  TOPOLOGICAL: YES
  SCHEMA: "Control Data"
>

<ENTITY: "Dummy Point"
  DESCRIPTION: "Temporary entity used for interim point designation"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  POINT
  TOPOLOGICAL: YES
>

<ENTITY: "Dummy Point - Ownership"
  DESCRIPTION: "Temporary entity on Ownership Layer used for interim point designation"
  THEME: "Ownership"
  GRAPHICSTYPE:  POINT
  TOPOLOGICAL: YES
>

<ENTITY: "Integrated Point"
  DESCRIPTION: "Monumented Point tied to control survey - used as control for Cadastral Mapping"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  POINT
  TOPOLOGICAL: YES
  SCHEMA: "Control Data"
  SCHEMA: "Monument Data"
>

<ENTITY: "Monumented Point"
  DESCRIPTION: "Point which has a survey monument placed at it (as shown on a legal survey plan)"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  POINT
  TOPOLOGICAL: YES
  SCHEMA: "Monument Data"
>

//*********************************************************************
//*      Line Entity Features                                         *
//*********************************************************************
<ENTITY: "Construction Line"
  DESCRIPTION: "Line created during cadastral mapping which does not form a topological entity"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: NO
>

<ENTITY: "Construction Line - Ownership"
  DESCRIPTION: "Line created during cadastral mapping which does not form a topological entity"
  THEME: "Ownership"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: NO
>

<ENTITY: "Contour Line"
  DESCRIPTION: "Contour Line with Elevation"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: NO
  SCHEMA: "Contour Elevation"
>

<ENTITY: "Public Lane Line"
  DESCRIPTION: "Line forming the limit of a Public Lane on the survey layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Public Lane Line - Ownership"
  DESCRIPTION: "Line forming the limit of a Public Lane on the ownership layer"
  THEME: "Ownership"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Public Lane Line and Street"
  DESCRIPTION: "Line forming the limit of a Public Lane on a street face on the survey layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Public Lane Line and Street - Ownership"
  DESCRIPTION: "Line forming the limit of a Public Lane on a street face on the ownership layer"
  THEME: "Ownership"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Public Walk Line"
  DESCRIPTION: "Limit of a Public Walk on the survey layer other than at street face"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Public Walk Line - Ownership"
  DESCRIPTION: "Limit of a Public Walk on the ownership layer other than at street face"
  THEME: "Ownership"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Public Walk and Public Lane Line"
  DESCRIPTION: "Limit of a Public Walk and Public Lane on the survey layer at a street face"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Public Walk and Public Lane Line - Ownership"
  DESCRIPTION: "Limit of a Public Walk and Public Lane on the ownership layer at a street face"
  THEME: "Ownership"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Public Walk and Street Line"
  DESCRIPTION: "Limit of a Public Walk on the survey layer at a street face"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Public Walk and Street Line - Ownership"
  DESCRIPTION: "Limit of a Public Walk on the ownership layer at a street face"
  THEME: "Ownership"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Property Line"
  DESCRIPTION: "Line created in calculations which forms a topological entity"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Ownership Property Line"
  DESCRIPTION: "Line created in calculations on the ownership layer which forms a topological entity"
  THEME: "Ownership"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Assessment Property Line"
  DESCRIPTION: "Line created in calculations on the assessment layer which forms a topological entity"
  THEME: "Assessment"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Street Closing Line"
  DESCRIPTION: "Line used to form a topological boundary between adjacent street segments"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Street Closing Line - Ownership"
  DESCRIPTION: "Line used to form a topological boundary between adjacent street segments on the Ownership Layer"
  THEME: "Ownership"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Street Line"
  DESCRIPTION: "Line used to form a topological boundary of street polygons on the Survey Layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Street Line - Ownership"
  DESCRIPTION: "Line used to form a topological boundary of street polygons on the Ownership Layer"
  THEME: "Ownership"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Border of Dataset"
  DESCRIPTION: "Line depicting the edge of a property mapping dataset"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Historic Water Boundary"
  DESCRIPTION: "Line depicting a parcel boundary based upon a previous location of a water body (typically digitized from an older survey plan or map)"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Historic Water Boundary - Ownership"
  DESCRIPTION: "Line depicting a parcel boundary based upon a previous location of a water body (typically digitized from an older survey plan or map)"
  THEME: "Ownership"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Estimated C/L of O H W M"
  DESCRIPTION: "Line depicting a parcel boundary based upon a current location of the center line of a water body (typically digitized from aerial photography or survey plan)"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Estimated C/L of O H W M - Ownership"
  DESCRIPTION: "Line depicting a parcel boundary based upon a current location of the center line of a water body (typically digitized from aerial photography or survey plan)"
  THEME: "Ownership"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Current Water Boundary"
  DESCRIPTION: "Line depicting a parcel boundary based upon a current location of a water body (typically digitized from aerial photography or satellite imagery)"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Current Water Boundary - Ownership"
  DESCRIPTION: "Line depicting a parcel boundary based upon a current location of a water body (typically digitized from aerial photography or satellite imagery)"
  THEME: "Ownership"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Road Widening Line"
  DESCRIPTION: "Street Edge of a polygon used to widen a road"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>
<ENTITY: "Quarter Section Line"
  DESCRIPTION: "Edge of a DLS Quarter Section Polygon"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Section Line"
  DESCRIPTION: "Edge of a DLS Section Polygon"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Township Line"
  DESCRIPTION: "Edge of a DLS Township polygon"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Town Limit"
  DESCRIPTION: "Limits of a Town"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "City Limit"
  DESCRIPTION: "Limits of a City"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Park Boundary"
  DESCRIPTION: "Boundary of a Park"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Provincial Boundary"
  DESCRIPTION: "Boundary of a Province"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Indian Reserve Boundary"
  DESCRIPTION: "Boundary of an Indian Reserve"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Mining Claim Line-NonTopological"
  DESCRIPTION: "Edge of a Mining Claim - non-topological"
  THEME: "Registered Plan"
  GRAPHICSTYPE: LINE
  TOPOLOGICAL: NO
>

//*********************************************************************
//*      Polygon Entity Features                                      *
//*********************************************************************
<ENTITY: "Certificate of Title Parcel"
  DESCRIPTION: "Cadastral polygon created on the ownership layer identified by a certificate of title number attributes"
  THEME: "Ownership"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Certificate of Title Parcel Data"
  FONT=(NAME="Arial" SIZE=6 BOLD )
>

<ENTITY: "Parcel of Certificate of Title"
  DESCRIPTION: "Part of Certificate of Title Parcel"
  THEME: "Ownership"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Parcel of Certificate of Title Parcel Data"
  FONT=(NAME="Arial" SIZE=6 BOLD )
>

<ENTITY: "DLS Parcel"
  DESCRIPTION: "Cadastral polygon identified by D.L.S. attributes"
  THEME: "Registered Plan"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "DLS Parcel Data"
  FONT=(NAME="ARIAL" SIZE=9 BOLD )
>

<ENTITY: "DLS Parcel - Ownership"
  DESCRIPTION: "Cadastral polygon identified by D.L.S. attributes"
  THEME: "Ownership"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "DLS Parcel Data"
  FONT=(NAME="ARIAL" SIZE=9 BOLD )
>

<ENTITY: "Future Property Map Polygon"
  DESCRIPTION: "Polygon that delineates a FUTURE property mapping job"
  THEME: "Registered Plan"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Mapping Area Polygon"
  FONT=(NAME="Arial" SIZE=1000 ITALIC BOLD )
>

<ENTITY: "InProgress Property Map Polygon"
  DESCRIPTION: "Polygon that delineates a property mapping job that is currently being done"
  THEME: "Registered Plan"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Mapping Area Polygon"
  FONT=(NAME="Arial" SIZE=1000 ITALIC BOLD )
>

<ENTITY: "Judge's Order Parcel"
  DESCRIPTION: "Cadastral polygon identified by a Judge's Order"
  THEME: "Registered Plan"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Judge's Order Parcel Data"
  FONT=(NAME="Arial"   SIZE=6 BOLD )
>

<ENTITY: "Judge's Order Parcel - Owner"
  DESCRIPTION: "Cadastral polygon created on the ownership layer identified by a Judge's Order"
  THEME: "Ownership"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Judge's Order Parcel Data"
  FONT=(NAME="Arial"   SIZE=6 BOLD )
>

<ENTITY: "Legal Instrument Parcel"
  DESCRIPTION: "Cadastral polygon identified by a Legal Instrument #"
  THEME: "Registered Plan"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Legal Instrument Parcel Data"
  FONT=(NAME="Arial" SIZE=6 BOLD )
>

<ENTITY: "Legal Instrument Parcel - Owner"
  DESCRIPTION: "Cadastral polygon created on the survey layer identified by a Legal Instrument #"
  THEME: "Ownership"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Legal Instrument Parcel Data"
  FONT=(NAME="Arial" SIZE=6 BOLD )
>

<ENTITY: "Parish Lot Parcel"
  DESCRIPTION: "Cadastral polygon created on the survey layer identified by parish lot attributes"
  THEME: "Registered Plan"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Parish Lot Parcel Data"
  FONT=(NAME="ARIAL" SIZE=9 BOLD )
>

<ENTITY: "Parish Lot Parcel - Ownership"
  DESCRIPTION: "Cadastral polygon created on the survey layer identified by parish lot attributes"
  THEME: "Ownership"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Parish Lot Parcel Data"
  FONT=(NAME="ARIAL" SIZE=9 BOLD )
>

<ENTITY: "Public Walk"
  DESCRIPTION: "Cadastral polygon created on the survey layer designated as a public walk and identified by survey plan attributes"
  THEME: "Registered Plan"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Public Walk Data"
  FONT=(NAME="ARIAL" SIZE=2 BOLD )
>

<ENTITY: "Public Walk - Ownership"
  DESCRIPTION: "Cadastral polygon created on the survey layer designated as a public walk and identified by survey plan attributes"
  THEME: "Ownership"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Public Walk Data"
  FONT=(NAME="ARIAL" SIZE=2 BOLD )
>

<ENTITY: "Public Lane"
  DESCRIPTION: "Cadastral polygon created on the survey layer designated as a public lane and identified by survey plan attributes"
  THEME: "Registered Plan"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Public Lane Data"
  FONT=(NAME="ARIAL" SIZE=7 BOLD )
>

<ENTITY: "Public Lane - Ownership"
  DESCRIPTION: "Cadastral polygon created on the survey layer designated as a public lane and identified by survey plan attributes"
  THEME: "Ownership"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Public Lane Data"
  FONT=(NAME="ARIAL" SIZE=7 BOLD )
>

<ENTITY: "Special Plot Plan Parcel"
  DESCRIPTION: "Cadastral polygon identified by special plot plan attributes"
  THEME: "Registered Plan"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Survey, Director of Survey & Condominum Plan Parcel Data"
  FONT=(NAME="ARIAL" SIZE=6 BOLD)
>

<ENTITY: "Street Polygon - LARGE"
  DESCRIPTION: "Cadastral polygon forming a street and identified by street name attribute"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Street Data"
  FONT=(NAME="Arial Black" SIZE=25 BOLD)
>

<ENTITY: "Street Polygon - Regular"
  DESCRIPTION: "Cadastral polygon forming a street and identified by street name attribute"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Street Data"
  FONT=(NAME="Arial" SIZE=17 BOLD)
>

<ENTITY: "Street Polygon - small"
  DESCRIPTION: "Cadastral polygon forming a street and identified by street name attribute"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Street Data"
  FONT=(NAME="Arial" SIZE=10 BOLD)
>

<ENTITY: "Street Polygon - Owner - LARGE"
  DESCRIPTION: "Cadastral polygon forming a street and identified by street name attribute"
  THEME: "Ownership"
  GRAPHICSTYPE:  POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Street Data"
  FONT=(NAME="Arial Black" SIZE=25)
>

<ENTITY: "Street Polygon - Owner - Regular"
  DESCRIPTION: "Cadastral polygon forming a street and identified by street name attribute"
  THEME: "Ownership"
  GRAPHICSTYPE:  POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Street Data"
  FONT=(NAME="Arial" SIZE=17 BOLD)
>

<ENTITY: "Street Polygon - Owner - small"
  DESCRIPTION: "Cadastral polygon forming a street and identified by street name attribute"
  THEME: "Ownership"
  GRAPHICSTYPE:  POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Street Data"
  FONT=(NAME="Arial Narrow" SIZE=10 BOLD)
>

<ENTITY: "Plan Parcel or Lot"
  DESCRIPTION: "Cadastral polygon created on the survey layer identified by (Director of Surveys, Standard Survey or Condominium Plan attributes"
  THEME: "Registered Plan"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Survey, Director of Survey & Condominum Plan Parcel Data"
  FONT=(NAME="Arial" SIZE=6 BOLD )
>

<ENTITY: "Property Map Polygon"
  DESCRIPTION: "Polygon that delineates a property mapping job"
  THEME: "Registered Plan"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Mapping Area Polygon"
  FONT=(NAME="Arial" SIZE=1000 ITALIC BOLD )
>

<ENTITY: "Plan Parcel or Lot - Ownership"
  DESCRIPTION: "Cadastral polygon created on the ownership layer identified by (Director of Surveys, Standard Survey or Condominium Plan attributes"
  THEME: "Ownership"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Survey, Director of Survey & Condominum Plan Parcel Data"
  FONT=(NAME="Arial" SIZE=6 BOLD )
>

<ENTITY: "Assessment Parcel"
  DESCRIPTION: "Parcel created on assessment layer which forms a topological entity"
  THEME: "Assessment"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Assessment Roll Number"
  FONT=(NAME="ARIAL" SIZE=6 BOLD ITALIC UNDERLINE)
>

<ENTITY: "Water Body"
  DESCRIPTION: "Water polygon which forms a cadastral polygon"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Water Body Data"
  FONT=(NAME="ARIAL" SIZE=14 BOLD ITALIC)
>

<ENTITY: "Water Body - Ownership"
  DESCRIPTION: "Water polygon which forms a cadastral polygon"
  THEME: "Ownership"
  GRAPHICSTYPE:  POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Water Body Data"
  FONT=(NAME="ARIAL" SIZE=14 BOLD ITALIC)
>

<ENTITY: "Road Widening Parcel"
  DESCRIPTION: "Cadastral polygon used to enlarge Rights-of-Way"
  THEME: "Registered Plan"
  GRAPHICSTYPE: POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Survey, Director of Survey & Condominum Plan Parcel Data"
  FONT=(NAME="Arial" SIZE=6 BOLD )
>

//*********************************************************************
//*      Annotation Entity Features                                   *
//*********************************************************************
<ENTITY: "Block Annotation"
  DESCRIPTION: "Block number annotation - survey layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="Arial"  SIZE=17 BOLD )
>

<ENTITY: "Legal Instrument Annotation"
  DESCRIPTION: "Annotation for a legal instrument parcel on the survey layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE: ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="Arial"   SIZE=6 BOLD )
>

<ENTITY: "Legal Instrument Annotation - Ownership"
  DESCRIPTION: "Annotation for a legal isntrument parcel on the ownership layer"
  THEME: "Ownership"
  GRAPHICSTYPE: ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="Arial" SIZE=6 BOLD )
>

<ENTITY: "Lot/Parcel Number Annotation"
  DESCRIPTION: "Lot/Parcel number annotation - Survey Layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="ARIAL"  SIZE=6 BOLD )
>

<ENTITY: "Public Lane Annotation Entity"
  DESCRIPTION: "Public Lane Annotation -survey layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="ARIAL" SIZE=7 BOLD )
>

<ENTITY: "Public Lane Annotation Entity - Ownership"
  DESCRIPTION: "Public Lane Annotation - ownership layer"
  THEME: "Ownership"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="ARIAL" SIZE=7 BOLD )
>

<ENTITY: "Public Walk Annotation Entity"
  DESCRIPTION: "Public Walk Annotation - survey layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="ARIAL" SIZE=2 BOLD)
>

<ENTITY: "Public Walk Annotation Entity - Ownership"
  DESCRIPTION: "Public Walk Annotation - ownership layer"
  THEME: "Ownership"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="ARIAL" SIZE=2 BOLD)
>

<ENTITY: "Quarter Section Annotation"
  DESCRIPTION: "DLS Quarter Section Annotation - Survey Layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="Times New Roman" SIZE=9  BOLD )
>

<ENTITY: "Street Name Annotation Entity"
  DESCRIPTION: "Street Name Annotation Entity"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="ARIAL" SIZE=14  BOLD )
>

<ENTITY: "Street Name Annotation Entity - Ownership"
  DESCRIPTION: "Street Name Annotation Entity"
  THEME: "Ownership"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="ARIAL" SIZE=14  BOLD )
>

<ENTITY: "Survey Plan Number Annotation"
  DESCRIPTION: "LTO plan number annotation - survey layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="Arial"  SIZE=9 BOLD ITALIC )
>

<ENTITY: "Parish Lot Annotation"
  DESCRIPTION: "Annotation for parish lot polygon"
  THEME: "Registered Plan"
  GRAPHICSTYPE: ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="ARIAL" SIZE=9 BOLD )
>

<ENTITY: "Parish Lot Annotation - Ownership"
  DESCRIPTION: "Annotation for parish lot polygon on the Ownership layer"
  THEME: "Ownership"
  GRAPHICSTYPE: ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="ARIAL" SIZE=9 BOLD )
>

<ENTITY: "Range Annotation"
  DESCRIPTION: "DLS Range Annotation - Registered Plan Layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="Times New Roman" SIZE=9  BOLD )
>

<ENTITY: "Section Annotation"
  DESCRIPTION: "DLS Section Annotation - Registered Plan Layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="Times New Roman" SIZE=9  BOLD )
>

<ENTITY: "Township Annotation"
  DESCRIPTION: "DLS Township Annotation - Registered Plan Layer"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="Times New Roman" SIZE=9  BOLD )
>

<ENTITY: "Road Widening  Annotation"
  DESCRIPTION: "Annotation for non-topological polygon used to enlarge Rights-of-Way"
  THEME: "Registered Plan"
  GRAPHICSTYPE: ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="ARIAL" SIZE=9 BOLD )
>

<ENTITY: "Water Body Annotation"
  DESCRIPTION: "Annotation of a Water Body"
  THEME: "Registered Plan"
  GRAPHICSTYPE:  ANNOTATION
  TOPOLOGICAL: NO
  FONT=(NAME="ARIAL" SIZE=14 BOLD ITALIC)
>

//*********************************************************************
//* Schema,FieldSchema,Domain & Template Features                     *
//*********************************************************************
<SCHEMA: "Contour Elevation"
  DESCRIPTION: "Data that provides the elevation of the countour line"
  FIELDSCHEMA: "Elevation"
>

<FIELDSCHEMA: "Elevation"
  DESCRIPTION: "An elevation value"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dFLOAT4
  DEFAULTVALUE: 0.0
  MAXCHAR: 0
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<SCHEMA: "Monument Data"
  DESCRIPTION: "Data that describes the survey monument placed in the field"
  FIELDSCHEMA: "Monument Field"
>

<FIELDSCHEMA: "Monument Field"
  DESCRIPTION: "Type of Monument placed at this point"
  ISLOCKED: NO
  DOMAIN: "Monument Type"
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: " "
  MAXCHAR: 2
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "Monument Type"
  DESCRIPTION: "List of known monument types"
  {LIST:
" " "UNDEFINED" "Monument is currently not set" /
"A" "Manitoba Government Survey Post in Pipe" "Standard brass cap cemented in the top of a 1 inch diameter by four feet long iron pipe driven into the ground to a depth of at least 3 feet" /
"B" "Manitoba Government Survey Post in Concrete" "Standard brass cap set flush with the top of a concrete slab one foot square three feet to four feet in depth - to top of the monument being approximately four inches above ground level" /
"C" "Manitoba Government Survey Post in Concrete" "Standard brass cap cemented into a hole drilled into rock - the bottom of the cap being flush with the rock level" /
"D" "Solid iron post 1 1/8\"" "Solid iron post 1 1/8\" x 1 1/8\" - minimum 48\" long - marked M.L.S." /
"E" "Solid iron post 1\"" "Solid iron post 1\" x 1\" - minimum 36\" long - marked M.L.S." /
"F" "Solid iron post 3/4\" - sq. top" "Solid iron post 3/4\" x 3/4\" - minimum 30\" long - or a 30\" long hollow tube with 3/4\" x 3/4\" x 4\" square top" /
"G" "Solid iron post 3/4\" - tri. top" "Solid iron post 3/4\" x 3/4\" - minimum 30\" long - or a 30\" long hollow tube with 3/4\" x 3/4\" x 4\" triangular top" /
"A1" "I.P." "Iron Post" /
"A2" "Wo.P." "Wooden Post" /
"A3" "M.P." "Marker Post" /
"A4" "C.L.S.P." "Canada Land Survey Post (Brass Cap)" /
"A5" "D.L.S.P." "Dominion Land Survey Post (Old Standard Iron Post)" }
>

<SCHEMA: "Control Data"
  DESCRIPTION: "Points which have been used as control points for cadastral mapping"
  FIELDSCHEMA: "Location"
  FIELDSCHEMA: "Datum"
>

<FIELDSCHEMA: "Location"
  DESCRIPTION: "General location of point"
  ISLOCKED: YES
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 12
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<FIELDSCHEMA: "Datum"
  DESCRIPTION: "Datum/Adjustment of Point Computation"
  ISLOCKED: YES
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 9
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<FIELDSCHEMA: "PART"
  DESCRIPTION: "PART of Cadastral Polygon (1 of 2 allowed types)"
  ISLOCKED: NO
  DOMAIN: "PART of Cadastral Polygon"
  ISREQUIRED: NO
  DATATYPE: dCHARS
  DEFAULTVALUE: "  "
  MAXCHAR: 2
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "PART of Cadastral Polygon"
  DESCRIPTION: "Indicator of Part of Cadastral  Polygon"
  {LIST:
"PT"  "Part of Cadastral  Polygon"  "Is PART of original Cadastral  Polygon" /
"  "  "Whole Cadastral  Polygon"  "Is ENTIRE original Cadastral  Polygon" }
>

<FIELDSCHEMA: "Lot ID"
  DESCRIPTION: "Lot ID (Alphanumeric)"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: NO
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 5
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<FIELDSCHEMA: "Block ID"
  DESCRIPTION: "Block ID (Alphanumeric)"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: NO
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 4
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<FIELDSCHEMA: "Plan ID"
  DESCRIPTION: "Plan ID (Alphanumeric)"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 7
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<FIELDSCHEMA: "Original Issuing LTO"
  DESCRIPTION: "Land Title Office that originally issued the Plan"
  ISLOCKED: NO
  DOMAIN: "Issuing LTOs"
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: "  "
  MAXCHAR: 2
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "Issuing LTOs"
  DESCRIPTION: "List of Land Title Offices which issued non-unique plan numbers"
  {LIST:
"BO" "Boissevain" "Boissevain LTO" /
"B"  "Brandon" "Brandon LTO" /
"C"  "Carman" "Carman LTO" /
"D"  "Dauphin" "Dauphin LTO" /
"DS" "Director of Surveys" "Director of Surveys Plan" /
"DU"  "Dufferin" "Dufferin LTO" /
"DL"  "Dufferin-Lorne" "Dufferin-Lorne LTO" /
"L"  "Lisgar" "Lisgar LTO" /
"MN" "Manchester" "Manchester LTO" /
"ME" "Marquette East" "Marquette East LTO" /
"MW" "Marquette West" "Marquette West LTO" /
"M"  "Morden" "Morden LTO" /
"MO" "Morris" "Morris LTO" /
"N"  "Neepawa" "Neepawa LTO" /
"NO" "Norfolk" "Norfolk LTO" /
"PP" "Portage (now in Winnipeg)" "Portage (now in Winnipeg) LTO" /
"P"  "Portage" "Portage LTO" /
"PR" "Provenchier" "Provenchier LTO" /
"RL"  "Rock Lake" "Rock Lake LTO" /
"R"  "Rockwood" "Rockwood LTO" /
"S"  "Selkirk" "Selkirk LTO" /
"SL" "Shoal Lake" "Shoal Lake LTO" /
"SR" "Souris River" "Souris River LTO" /
"TM" "Turtle Mountain" "Turtle Mountain LTO" /
"V"  "Virden" "Virden LTO" /
"W"  "Winnipeg Division" "Winnipeg Division LTO" /
"  " "Unique Provincial Numbers" "Unique Provincial Numbers"  }
>

<SCHEMA: "Certificate of Title Parcel Data"
  DESCRIPTION: "Data that describes the Certificate of Title polygon"
  FIELDSCHEMA: "Certificate of Title Name"
  TEMPLATE: "Certificate of Title Annotation"
>

<TEMPLATE: "Certificate of Title Annotation"
  DESCRIPTION: "Annotation that applies to C of T Parcels"
  FORMAT: "C.T. %1"
>

<SCHEMA: "Parcel of Certificate of Title Parcel Data"
  DESCRIPTION: "Data that describes the Parcel of Certificate of Title polygon"
  FIELDSCHEMA: "Certificate of Title Name"
  FIELDSCHEMA: "Parcel ID"
  TEMPLATE: "Parcel & Title Annotation"
>

<TEMPLATE: "Parcel & Title Annotation"
  DESCRIPTION: "Parcel Annotation that applies to C of T Parcels"
  FORMAT: "Pcl. %2 C.T. %1"
>

<FIELDSCHEMA: "Certificate of Title Name"
  DESCRIPTION: "LTO Certificate of Title Value"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 12
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<FIELDSCHEMA: "Parcel ID"
  DESCRIPTION: "Parcel ID value for LTO Certificate of Title Value"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: NO
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 3
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<SCHEMA: "DLS Parcel Data"
  DESCRIPTION: "DLS Polygon Attributes"
  FIELDSCHEMA: "PART"
  FIELDSCHEMA: "HALF"
  FIELDSCHEMA: "DLS PARCEL TYPE"
  FIELDSCHEMA: "DLS LOT #"
  FIELDSCHEMA: "DLS LEGAL SUB-DIVISION #"
  FIELDSCHEMA: "QS VALUE"
  FIELDSCHEMA: "SECTION #"
  FIELDSCHEMA: "TOWNSHIP #"
  FIELDSCHEMA: "RANGE VALUE"
  FIELDSCHEMA: "MERIDIAN"
  TEMPLATE: "DLS Parcel Data - EPM or WPM - QS,Sec,Twp,Rge"
  TEMPLATE: "PART DLS Parcel Data - EPM or WPM - QS,Sec,Twp,Rge"
  TEMPLATE: "PART Sec,Twp,Rge - DLS Parcel Data - EPM or WPM"
  TEMPLATE: "Sec,Twp,Rge - DLS Parcel Data - EPM or WPM"
  TEMPLATE: "PART Lot,Sec,Twp,Rge - DLS Parcel Data - EPM or WPM"
  TEMPLATE: "Lot,Sec,Twp,Rge - DLS Parcel Data - EPM or WPM"
  TEMPLATE: "Part Lot,Twp,Rge - DLS Parcel Data - EPM or WPM"
  TEMPLATE: "Lot,Twp,Rge - DLS Parcel Data - EPM or WPM"
  TEMPLATE: "L.S.,Sec,Twp,Rge - DLS Parcel Data - EPM or WPM"
>

<TEMPLATE: "PART DLS Parcel Data - EPM or WPM - QS,Sec,Twp,Rge"
  DESCRIPTION: "Annotation that applies to EPM & WPM QS Parcels"
  FORMAT: "%1 %6 1/4 Sec. %7\nTwp. %8, Rge %9 %10.P.M."
>

<TEMPLATE: "DLS Parcel Data - EPM or WPM - QS,Sec,Twp,Rge"
  DESCRIPTION: "Annotation that applies to EPM & WPM QS Parcels"
  FORMAT: "%6 1/4 Sec. %7\nTwp. %8, Rge %9 %10.P.M."
>

<TEMPLATE: "PART Sec,Twp,Rge - DLS Parcel Data - EPM or WPM"
  DESCRIPTION: "Annotation that applies to Part EPM & WPM Section Parcels"
  FORMAT: "%1 Sec. %7\nTwp. %8, Rge %9 %10.P.M."
>

<TEMPLATE: "Sec,Twp,Rge - DLS Parcel Data - EPM or WPM"
  DESCRIPTION: "Annotation that applies to EPM & WPM Section Parcels"
  FORMAT: "Sec. %7\nTwp. %8, Rge %9 %10.P.M."
>

<TEMPLATE: "PART Lot,Sec,Twp,Rge - DLS Parcel Data - EPM or WPM"
  DESCRIPTION: "Annotation that applies to EPM & WPM Part Lot Section Parcels"
  FORMAT: "%1 Lot %4 Sec. %7\nTwp. %8, Rge %9 %10.P.M."
>

<TEMPLATE: "Lot,Sec,Twp,Rge - DLS Parcel Data - EPM or WPM"
  DESCRIPTION: "Annotation that applies to EPM & WPM Lot Section Parcels"
  FORMAT: "Lot %4 Sec. %7\nTwp. %8, Rge %9 %10.P.M."
>

<TEMPLATE: "Part Lot,Twp,Rge - DLS Parcel Data - EPM or WPM"
  DESCRIPTION: "Annotation that applies to Part EPM & WPM DLS Lot Parcels"
  FORMAT: "%1 Lot %4 Twp. %8, Rge %9 %10.P.M."
>

<TEMPLATE: "Lot,Twp,Rge - DLS Parcel Data - EPM or WPM"
  DESCRIPTION: "Annotation that applies to EPM & WPM DLS Lot Parcels"
  FORMAT: "Lot %4 Twp. %8, Rge %9 %10.P.M."
>

<TEMPLATE: "L.S.,Sec,Twp,Rge - DLS Parcel Data - EPM or WPM"
  DESCRIPTION: "Annotation that applies to EPM & WPM Legal Subdivision Parcels"
  FORMAT: "L.S. %5 Sec. %7 Twp. %8, Rge %9 %10.P.M."
>

<FIELDSCHEMA: "HALF"
  DESCRIPTION: "HALF of DLS QS Polygons (1 of 5 allowed types)"
  ISLOCKED: NO
  DOMAIN: "HALVES of DLS QS Polygons"
  ISREQUIRED: NO
  DATATYPE: dCHARS
  DEFAULTVALUE: " "
  MAXCHAR: 1
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "HALVES of DLS QS Polygons"
  DESCRIPTION: "List of allowed HALVES of DLS QS Polygons"
  {LIST:
" "  "No Half QS"  "Polygon is some other type of 1/4-Sec-Twp-Range polygon" /
"N"  "North Half QS"  "Polygon is North Half of a Quarter Section" /
"S"  "South Half QS"  "Polygon is South Half of a Quarter Section" /
"E"  "East Half QS"  "Polygon is East Half of a Quarter Section" /
"W"  "West Half QS"  "Polygon is West Half of a Quarter Section" }
>

<FIELDSCHEMA: "DLS PARCEL TYPE"
  DESCRIPTION: "Type of DLS Parcel (1 of 3 allowed types)"
  ISLOCKED: NO
  DOMAIN: "DLS PARCEL TYPES"
  ISREQUIRED: NO
  DATATYPE: dCHARS
  DEFAULTVALUE: " "
  MAXCHAR: 1
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "DLS PARCEL TYPES"
  DESCRIPTION: "List of allowed DLS PARCEL TYPES"
  {LIST:
" "  "No Parcel"  "Polygon is standard 1/4-Sec-Twp-Range" /
"L"  "DLS Lot"  "Polygon is Lot within 1/4-Sec-Twp-Range polygon" /
"S"  "Legal Subdivision"  "Polygon is  Subdivision of Sec-Twp-Range polygon" }
>

<FIELDSCHEMA: "DLS LOT #"
  DESCRIPTION: "DLS LOT #"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: NO
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 3
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<FIELDSCHEMA: "DLS LEGAL SUB-DIVISION #"
  DESCRIPTION: "DLS LEGAL SUB-DIVISION #"
  ISLOCKED: NO
  DOMAIN: "LEGAL SUB-DIVISION NUMBERS"
  ISREQUIRED: NO
  DATATYPE: dINT1
  DEFAULTVALUE: 0
  MAXCHAR: 0
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "LEGAL SUB-DIVISION NUMBERS"
  DESCRIPTION: "Allowed Values of LEGAL SUB-DIVISION NUMBERS"
   {INTRANGE: MINIMUM=0 MAXIMUM=16 INCREMENT=1}
>

<FIELDSCHEMA: "QS VALUE"
  DESCRIPTION: "Quarter Section"
  ISLOCKED: NO
  DOMAIN: "Quarter Sections"
  ISREQUIRED: NO
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 2
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "Quarter Sections"
  DESCRIPTION: "List of allowed Quarter Sections"
  {LIST:
"NE"  "NE 1/4"  "NE 1/4 Section" /
"NW"  "NW 1/4"  "NW 1/4 Section" /
"SE"  "SE 1/4"  "SE 1/4 Section" /
"SW"  "SW 1/4"  "SW 1/4 Section" }
>

<FIELDSCHEMA: "SECTION #"
  DESCRIPTION: "Defined Section Numbers"
  ISLOCKED: NO
  DOMAIN: "Section Numbers"
  ISREQUIRED: YES
  DATATYPE: dINT1
  DEFAULTVALUE: NONE
  MAXCHAR: 0
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "Section Numbers"
  DESCRIPTION: "Allowed Values of Section Numbers"
   {INTRANGE: MINIMUM=0 MAXIMUM=36 INCREMENT=1}
>

<FIELDSCHEMA: "TOWNSHIP #"
  DESCRIPTION: "DLS Township Number in Manitoba"
  ISLOCKED: NO
  DOMAIN: "Manitoba Township Numbers"
  ISREQUIRED: YES
  DATATYPE: dINT2
  DEFAULTVALUE: NONE
  MAXCHAR: 0
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "Manitoba Township Numbers"
  DESCRIPTION: "Allowed Values of Manitoba Township Numbers"
   {INTRANGE: MINIMUM=1 MAXIMUM=126 INCREMENT=1}
>

<FIELDSCHEMA: "RANGE VALUE"
  DESCRIPTION: "DLS Range in Manitoba"
  ISLOCKED: NO
  DOMAIN: "Range Values"
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 3
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "Range Values"
  DESCRIPTION: "List of allowed Range Values"
  {LIST:
"1"  "Range 1"  "DLS Range 1 in Manitoba" /
"2"  "Range 2"  "DLS Range 2 in Manitoba" /
"3"  "Range 3"  "DLS Range 3 in Manitoba" /
"4"  "Range 4"  "DLS Range 4 in Manitoba" /
"5"  "Range 5"  "DLS Range 5 in Manitoba" /
"6"  "Range 6"  "DLS Range 6 in Manitoba" /
"7"  "Range 7"  "DLS Range 7 in Manitoba" /
"8"  "Range 8"  "DLS Range 8 in Manitoba" /
"9"  "Range 9"  "DLS Range 9 in Manitoba" /
"10"  "Range 10"  "DLS Range 10 in Manitoba" /
"10A"  "Range 10A"  "DLS Range 10A (Lac du Bonnet) in Manitoba" /
"11"  "Range 11"  "DLS Range 11 in Manitoba" /
"11A"  "Range 11A"  "DLS Range 11A (Lac du Bonnet) in Manitoba" /
"12"  "Range 12"  "DLS Range 12 in Manitoba" /
"13"  "Range 13"  "DLS Range 13 in Manitoba" /
"14"  "Range 14"  "DLS Range 14 in Manitoba" /
"15"  "Range 15"  "DLS Range 15 in Manitoba" /
"16"  "Range 16"  "DLS Range 16 in Manitoba" /
"17"  "Range 17"  "DLS Range 17 in Manitoba" /
"18"  "Range 18"  "DLS Range 18 in Manitoba" /
"19"  "Range 19"  "DLS Range 19 in Manitoba" /
"20"  "Range 20"  "DLS Range 20 in Manitoba" /
"21"  "Range 21"  "DLS Range 21 in Manitoba" /
"22"  "Range 22"  "DLS Range 22 in Manitoba" /
"23"  "Range 23"  "DLS Range 23 in Manitoba" /
"24"  "Range 24"  "DLS Range 24 in Manitoba" /
"25"  "Range 25"  "DLS Range 25 in Manitoba" /
"26"  "Range 26"  "DLS Range 26 in Manitoba" /
"27"  "Range 27"  "DLS Range 27 in Manitoba" /
"28"  "Range 28"  "DLS Range 28 in Manitoba" /
"29"  "Range 29"  "DLS Range 29 in Manitoba" /
"29A"  "Range 29A"  "DLS Range 29A in Manitoba" /
"30"  "Range 30"  "DLS Range 30 in Manitoba" }
>

<FIELDSCHEMA: "MERIDIAN"
  DESCRIPTION: "DLS Meridian in Manitoba"
  ISLOCKED: NO
  DOMAIN: "Meridian Values"
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 1
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "Meridian Values"
  DESCRIPTION: "List of allowed Meridian Values"
  {LIST:
"E" "EPM" "East of Principal Meridian" /
"W" "WPM" "West of Principal Meridian" /
"Z" "2ME" "2nd Meridian East" }
>

<SCHEMA: "Judge's Order Parcel Data"
  DESCRIPTION: "Attribute data identifying the Judge's Order Parcel"
  FIELDSCHEMA: "Judge's Order ID"
  TEMPLATE: "Judge's Order Parcel Annotation"
>

<TEMPLATE: "Judge's Order Parcel Annotation"
  DESCRIPTION: "Annotation that applies to Judge's Order Parcels"
  FORMAT: "J.O. %1"
>

<FIELDSCHEMA: "Judge's Order ID"
  DESCRIPTION: "ID of Judge's Order establishing polygon"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 12
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<SCHEMA: "Legal Instrument Parcel Data"
  DESCRIPTION: "Attribute data identifying the Legal Instrument Parcel"
  FIELDSCHEMA: "Legal Instrument ID"
  TEMPLATE: "Legal Instrument Parcel Annotation"
>

<TEMPLATE: "Legal Instrument Parcel Annotation"
  DESCRIPTION: "Annotation that applies to Legal Instrument Parcels"
  FORMAT: "Inst. #%1"
>

<FIELDSCHEMA: "Legal Instrument ID"
  DESCRIPTION: "ID of Legal Instrument establishing polygon"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 11
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<SCHEMA: "Mapping Area Polygon"
  DESCRIPTION: "Label name for a property mapping job"
  FIELDSCHEMA: "Property Mapping Name"
  TEMPLATE: "Property Mapping Area Label"
>

<TEMPLATE: "Property Mapping Area Label"
  DESCRIPTION: "Label for a property mapping area polygon"
  FORMAT: "%1"
>

<FIELDSCHEMA: "Property Mapping Name"
  DESCRIPTION: "Name of a Property Mapping Area Polygon"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 25
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<SCHEMA: "Parish Lot Parcel Data"
  DESCRIPTION: "Attribute data for the Parish Lot"
  FIELDSCHEMA: "PART"
  FIELDSCHEMA: "Parish Lot ID"
  FIELDSCHEMA: "Parish Lot Type"
  FIELDSCHEMA: "Parish"
  TEMPLATE: "Parish Lot Parcel Annotation"
  TEMPLATE: "PART Parish Lot Parcel Annotation"
  TEMPLATE: "Complete Parish Lot Parcel Annotation"
  TEMPLATE: "Complete PART Parish Lot Parcel Annotation"
>

<TEMPLATE: "PART Parish Lot Parcel Annotation"
  DESCRIPTION: "Annotation that applies to Parish Lot Parcels"
  FORMAT: "%1 %v3 %2"
>

<TEMPLATE: "Parish Lot Parcel Annotation"
  DESCRIPTION: "Annotation that applies to Parish Lot Parcels"
  FORMAT: "%v3 %2"
>

<TEMPLATE: "Complete PART Parish Lot Parcel Annotation"
  DESCRIPTION: "Annotation that Displays Parish Lot Parcels"
  FORMAT: "%1 %v3 %2 Parish of %v4"
>

<TEMPLATE: "Complete Parish Lot Parcel Annotation"
  DESCRIPTION: "Annotation that Displays Parish Lot Parcels"
  FORMAT: "%v3 %2 Parish of %v4"
>

<FIELDSCHEMA: "Parish Lot ID"
  DESCRIPTION: "Parish Lot ID (Alphanumeric)"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 5
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<FIELDSCHEMA: "Parish Lot Type"
  DESCRIPTION: "Type of Parish Lot ID (one of 5 allowed types)"
  ISLOCKED: NO
  DOMAIN: "Parish Lot Types"
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: "RL"
  MAXCHAR: 2
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "Parish Lot Types"
  DESCRIPTION: "List of allowed Parish Lot Types"
  {LIST:
"GL" "GROUP LOT" "GROUP LOT Lot Type" /
"IR" "INDIAN RESERVE" "INDIAN RESERVE Lot Type" /
"LL" "LAKE LOT" "LAKE LOT Lot Type" /
"OT" "OUTER TWO MILE LOT" "OUTER TWO MILE LOT Lot Type" /
"PL" "PARK LOT" "PARK LOT Lot Type" /
"RL" "RIVER LOT" "RIVER LOT Lot Type" /
"SL" "SETTLEMENT LOT" "SETTLEMENT LOT Lot Type" /
"WL" "WOOD LOT" "WOOD LOT Lot Type" }
>

<FIELDSCHEMA: "Parish"
  DESCRIPTION: "Parish ID (one of 45 allowed values)"
  ISLOCKED: NO
  DOMAIN: "Parish Values"
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 2
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "Parish Values"
  DESCRIPTION: "List of allowed Parish Values"
  {LIST:
"BP" "BAIE ST PAUL" "Parish of BAIE ST PAUL" /
"BE" "BIG EDDY" "Parish of BIG EDDY" /
"BH" "BROKENHEAD" "Parish of BROKENHEAD" /
"CR" "CROSS LAKE" "Parish of CROSS LAKE" /
"DN" "DUCK BAY NORTH" "Parish of DUCK BAY NORTH" /
"DS" "DUCK BAY SOUTH" "Parish of DUCK BAY SOUTH" /
"FF" "FAIRFORD" "Parish of FAIRFORD" /
"FM" "FAIRFORD MISSION" "Parish of FAIRFORD MISSION" /
"FB" "FISHER BAY" "Parish of FISHER BAY" /
"FA" "FORT ALEXANDER" "Parish of FORT ALEXANDER" /
"GR" "GRAND RAPID" "Parish of GRAND RAPID" /
"GP" "GRANDE POINTE" "Parish of GRANDE POINTE" /
"HE" "HEADINGLY" "Parish of HEADINGLY" /
"HB" "HIGH BLUFF" "Parish of HIGH BLUFF" /
"KI" "KILDONAN" "Parish of KILDONAN" /
"LO" "LORETTE" "Parish of LORETTE" /
"MR" "MANIGOTAGAN RIVER" "Parish of MANIGOTAGAN RIVER" /
"MH" "MANITOBA HOUSE" "Parish of MANITOBA HOUSE" /
"NH" "NORWAY HOUSE" "NORWAY HOUSE Settlement" /
"OI" "OAK ISLAND" "Parish of OAK ISLAND" /
"OP" "OAK POINT" "Parish of OAK POINT" /
"PQ" "PASQUIA" "Parish of PASQUIA" /
"PC" "PINE CREEK" "Parish of PINE CREEK" /
"PO" "POPLAR POINT" "Parish of POPLAR POINT" /
"PP" "PORTAGE LA PRAIRIE" "Parish of PORTAGE LA PRAIRIE" /
"RR" "RAT RIVER" "Parish of RAT RIVER" /
"RM" "RIDING MTN NATIONAL PARK" "Parish of RIDING MTN NATIONAL PARK" /
"RC" "ROMAN CATH. MISSION PROP." "Parish of ROMAN CATH. MISSION PROP." /
"AD" "ST ANDREWS" "Parish of ST ANDREWS" /
"BO" "ST BONIFACE" "Parish of ST BONIFACE" /
"CH" "ST CHARLES" "Parish of ST CHARLES" /
"CL" "ST CLEMENTS" "Parish of ST CLEMENTS" /
"FX" "ST FRANCOIS XAVIER" "Parish of ST FRANCOIS XAVIER" /
"JA" "ST JAMES" "Parish of ST JAMES" /
"JO" "ST JOHN" "Parish of ST JOHN" /
"LA" "ST LAURENT" "Parish of ST LAURENT" /
"MA" "ST MALO" "Parish of ST MALO" /
"NO" "ST NORBERT" "Parish of ST NORBERT" /
"PA" "ST PAUL" "Parish of ST PAUL" /
"PE" "ST PETER" "Parish of ST PETER" /
"VI" "ST VITAL" "Parish of ST VITAL" /
"AG" "STE AGATHE" "Parish of STE AGATHE" /
"AN" "STE ANNE" "Parish of STE ANNE" /
"PS" "THE PAS" "Parish of THE PAS" /
"UV" "UMFREVILLE" "Parish of UMFREVILLE" /
"WE" "WESTBOURNE" "Parish of WESTBOURNE" }
>

<SCHEMA: "Public Lane Data"
  DESCRIPTION: "Attribute data describing a Public Lane"
  FIELDSCHEMA: "NAME"
  TEMPLATE: "Public Lane Annotation"
>

<TEMPLATE: "Public Lane Annotation"
  DESCRIPTION: "Annotation that applies to Public Lanes"
  FORMAT: "Public Lane"
>

<FIELDSCHEMA: "NAME"
  DESCRIPTION: "Name of Public Lane or Walk"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: NO
  DATATYPE: dCHARS
  DEFAULTVALUE: " "
  MAXCHAR: 8
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<SCHEMA: "Public Walk Data"
  DESCRIPTION: "Attribute data describing a Public Walk"
  FIELDSCHEMA: "NAME"
  TEMPLATE: "Public Walk Annotation"
>

<TEMPLATE: "Public Walk Annotation"
  DESCRIPTION: "Annotation that applies to Public Walks"
  FORMAT: "Public Walk"
>

<SCHEMA: "Survey, Director of Survey & Condominum Plan Parcel Data"
  DESCRIPTION: "Attribute data for the Survey, Director of Survey & Condominum Plan polygon"
  FIELDSCHEMA: "PART"
  FIELDSCHEMA: "Lot ID"
  FIELDSCHEMA: "Block ID"
  FIELDSCHEMA: "Plan ID"
  FIELDSCHEMA: "Parcel Type"
  FIELDSCHEMA: "Plan Type"
  FIELDSCHEMA: "Original Issuing LTO"
  TEMPLATE: "LOT - Survey Plan Annotation"
  TEMPLATE: "PART LOT - Survey Plan Annotation"
  TEMPLATE: "BLOCK - Survey Plan Annotation"
  TEMPLATE: "PART BLOCK - Survey Plan Annotation"
  TEMPLATE: "PLAN - Survey Plan Annotation"
  TEMPLATE: "PART PLAN - Survey Plan Annotation"
  TEMPLATE: "PLAN - Survey Plan Annotation with LTO Office"
  TEMPLATE: "PART PLAN - Survey Plan Annotation with LTO Office"
  TEMPLATE: "Public Reserve OR Public Park - Survey Plan Annotation"
>

<TEMPLATE: "LOT - Survey Plan Annotation"
  DESCRIPTION: "Annotation that places Lot IDs on Survey Plan Parcels"
  FORMAT: "%2"
>

<TEMPLATE: "PART LOT - Survey Plan Annotation"
  DESCRIPTION: "Annotation that places Lot IDs on Survey Plan Parcels"
  FORMAT: "%1 %2"
>

<TEMPLATE: "PART BLOCK - Survey Plan Annotation"
  DESCRIPTION: "Annotation that places Part Block IDs on Survey Plan Parcels"
  FORMAT: "%1 Block %3"
>

<TEMPLATE: "BLOCK - Survey Plan Annotation"
  DESCRIPTION: "Annotation that places Block IDs on Survey Plan Parcels"
  FORMAT: "Block %3"
>

<TEMPLATE: "PART PLAN - Survey Plan Annotation"
  DESCRIPTION: "Annotation that places Plan IDs on Survey Plan Parcels"
  FORMAT: "%1 Plan %4"
>

<TEMPLATE: "PLAN - Survey Plan Annotation"
  DESCRIPTION: "Annotation that places Plan IDs on Survey Plan Parcels"
  FORMAT: "Plan %4"
>

<TEMPLATE: "PART PLAN - Survey Plan Annotation with LTO Office"
  DESCRIPTION: "Annotation that places Part Plan IDs on Survey Plan Parcels"
  FORMAT: "%1 Plan %4 %v7 LTO"
>

<TEMPLATE: "PLAN - Survey Plan Annotation with LTO Office"
  DESCRIPTION: "Annotation that places Plan IDs on Survey Plan Parcels"
  FORMAT: "Plan %4 %v7 LTO"
>

<TEMPLATE: "Public Reserve OR Public Park - Survey Plan Annotation"
  DESCRIPTION: "Annotation that places Public Reserve or Public Park IDs on Survey Plan Parcels"
  FORMAT: "%v5 %2"
>

<FIELDSCHEMA: "Parcel Type"
  DESCRIPTION: "Type of Plan Parcel Type (used to ID Public Reserves and Public Parks)"
  ISLOCKED: NO
  DOMAIN: "Parcel Types"
  ISREQUIRED: NO
  DATATYPE: dCHARS
  DEFAULTVALUE: " "
  MAXCHAR: 1
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "Parcel Types"
  DESCRIPTION: "List of allowed Plan Parcel Types"
  {LIST:
" " "Standard" "Ordinary Plan Polygon" /
"P" "Public Park" "Public Park polygon" /
"R" "Public Reserve" "Public Reserve polygon" }
>

<FIELDSCHEMA: "Plan Type"
  DESCRIPTION: "Type of Plan Parcel Type (used to ID Director of Surveys, LTO Survey & Condominium Plans)"
  ISLOCKED: NO
  DOMAIN: "Plan Types"
  ISREQUIRED: NO
  DATATYPE: dCHARS
  DEFAULTVALUE: "S"
  MAXCHAR: 1
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: "Plan Types"
  DESCRIPTION: "List of allowed Plan Types"
  {LIST:
"D" "Director of Surveys" "Director of Surveys Plan" /
"S" "LTO Survey" "LTO Survey Plan" /
"C" "Condominium" "Condominium Plan" }
>

<SCHEMA: "Assessment Roll Number"
  DESCRIPTION: "Attribute data identifying the assessment layer polygon"
  FIELDSCHEMA: "Municipality Number"
  FIELDSCHEMA: "Assessment Roll"
>

<FIELDSCHEMA: "Municipality Number"
  DESCRIPTION: "Municipality ID value"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 3
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 1
>

<FIELDSCHEMA: "Assessment Roll"
  DESCRIPTION: "Assessment Roll ID value"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 7
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 2
>

<SCHEMA: "Street Data"
  DESCRIPTION: "Attribute data identifying the street polygon"
  FIELDSCHEMA: "Street Name"
  TEMPLATE: "Street Name Annotation"
>

<TEMPLATE: "Street Name Annotation"
  DESCRIPTION: "Annotation that places Street Names on Polygons"
  FORMAT: "%1"
>

<FIELDSCHEMA: "Street Name"
  DESCRIPTION: "Name of the Street"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 40
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<SCHEMA: "Water Body Data"
  DESCRIPTION: "Data that describes the water body polygon"
  FIELDSCHEMA: "Water Body Name"
  FIELDSCHEMA: "Water Body Type"
  TEMPLATE: "Water Body Annotation, Name followed by Water Body Type(River or Lake)"
  TEMPLATE: "Other Lake Annotation (word Lake followed by Name)"
>

<TEMPLATE: "Water Body Annotation, Name followed by Water Body Type(River or Lake)"
  DESCRIPTION: "Annotation that places the water body name followed by the name of the water body type"
  FORMAT: "%1 %v2"
>

<TEMPLATE: "Other Lake Annotation (word Lake followed by Name)"
  DESCRIPTION: "Annotation that places the word LAKE followed by the water body name"
  FORMAT: "Lake %1"
>

<FIELDSCHEMA: "Water Body Name"
  DESCRIPTION: "Name of the water body"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 40
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<FIELDSCHEMA: "Water Body Type"
  DESCRIPTION: "Type of water body polygon"
  ISLOCKED: NO
  DOMAIN: WaterBodyTypes
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 1
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: NO
  KEYSEQUENCE: 0
>

<DOMAIN: WaterBodyTypes
  DESCRIPTION: "List of allowed water body types"
  {LIST:
"R" "River" "River polygon" /
"C" "Creek" "Creek polygon" /
"L" "Lake" "Lake polygon" }
>

<THEME: "Hydro Easements"
  DESCRIPTION: "Hydro Easements Layer"
  PARENT: NONE
  DEFAULTLINE: "Hydro Easement Property Line"
  DEFAULTPOINT: "Computed Hydro Easement Point"
  DEFAULTPOLYGON: "Hydro Easement Polygon"
>

<ENTITY: "Computed Hydro Easement Point"
  DESCRIPTION: "Hydro Easement Point computed as part of cadastral mapping"
  THEME: "Hydro Easements"
  GRAPHICSTYPE:  POINT
  TOPOLOGICAL: YES
>

<ENTITY: "Hydro Easement Property Line"
  DESCRIPTION: "Line created in calculations on the Hydro Easement layer which forms a topological entity"
  THEME: "Hydro Easements"
  GRAPHICSTYPE:  LINE
  TOPOLOGICAL: YES
>

<ENTITY: "Hydro Easement Polygon"
  DESCRIPTION: "Parcel created on Hydro Easement layer which forms a topological entity"
  THEME: "Hydro Easements"
  GRAPHICSTYPE:  POLYGON
  TOPOLOGICAL: YES
  SCHEMA: "Hydro Easement Parcel"
  FONT=(NAME="ARIAL" SIZE=18 BOLD ITALIC UNDERLINE)
>

<SCHEMA: "Hydro Easement Parcel"
  DESCRIPTION: "Attribute data identifying the Hydro Easement  polygon"
  FIELDSCHEMA: "Hydro Easement Parcel ID"
>

<FIELDSCHEMA: "Hydro Easement Parcel ID"
  DESCRIPTION: "Hydro Easement Parcel ID value"
  ISLOCKED: NO
  DOMAIN: ""
  ISREQUIRED: YES
  DATATYPE: dCHARS
  DEFAULTVALUE: NONE
  MAXCHAR: 12
  ISCOLLECTION: NO
  AUTOSEQUENCED: NO
  ISUNIQUE: YES
  KEYSEQUENCE: 0
>
