#pragma once

// This exists only to check for compilation errors on machines that don't
// have the old CEdit codebase. It pinpoints the CEdit methods that get utilized
// during export to the Backsight format.

enum CEOP {	CEOP_NULL				= 0,
			CEOP_DATA_IMPORT		= 1,
			CEOP_ARC_SUBDIVISION	= 2,
			CEOP_ANNOTATION			= 3,
			CEOP_LINE_INTERSECTION	= 4,
			CEOP_NETWORK			= 5,
			CEOP_RADIAL_STAKEOUT	= 6,
			CEOP_EXTENSION			= 7,
			CEOP_BANK_TRAVERSE		= 8,
			CEOP_CLOSED_TRAVERSE	= 9,
			CEOP_OPEN_TRAVERSE		= 10,
			CEOP_PARALLEL_TRAVERSE	= 11,
			CEOP_DIR_INTERSECT		= 12,
			CEOP_DIST_INTERSECT		= 13,
			CEOP_DIRDIST_INTERSECT	= 14,
			CEOP_NEW_POINT			= 15,
			CEOP_NEW_LABEL			= 16,
			CEOP_MOVE_LABEL			= 17,
			CEOP_DELETION			= 18,
			CEOP_UPDATE				= 19,
			CEOP_NEW_ARC			= 20,
			CEOP_PATH				= 21,
			CEOP_SPLIT				= 22,
			CEOP_AREA_SUBDIVISION	= 23,
			CEOP_SET_LABEL_ROTATION	= 24,
			CEOP_GET_BACKGROUND		= 25,
			CEOP_TRUNCATE			= 26,
			CEOP_GET_CONTROL		= 27,
			CEOP_ARC_CLIP			= 28,
			CEOP_ARC_SPLIT			= 29,
			CEOP_NEW_CIRCLE			= 30,
			CEOP_LINE_INTERSECT		= 31,
			CEOP_DIRLINE_INTERSECT	= 32,
			CEOP_ARC_EXTEND			= 33,
			CEOP_RADIAL				= 34,
			CEOP_SET_THEME			= 35,
			CEOP_SET_TOPOLOGY		= 36,
			CEOP_POINT_ON_LINE		= 37,
			CEOP_PARALLEL			= 38,
			CEOP_TRIM				= 39,
			CEOP_ATTACH_POINT		= 40 };

class CeClass
{
public:
	virtual ~CeClass() = 0;
};

class CeEntity
{
public:
};

class CeIdGroup
{
public:
	bool HasCheckDigit ( void ) const { return FALSE; }
	const CPtrList&	GetIdRanges	( void ) const { return m_IdRanges; }
	const CString& GetGroupName ( void ) const { return Name; }

private:
	CPtrList m_IdRanges;
	CString Name;
};

class CeIdRange
{
public:
	unsigned int GetMin	( void ) const { return 0; }
	unsigned int GetMax	( void ) const { return 0; }
};

class CeIdManager
{
public:
	CeIdGroup* GetpGroup ( const CeEntity* const pEnt ) const { return 0; }
	CeIdGroup* GetpGroup ( const unsigned int index ) const { return 0; }
	unsigned int GetNumGroup ( void ) const { return 0; }
};

class CeIdHandle
{
public:
	static CeIdManager* GetIdManager ( void ) { return 0; }
};

class CeKey
{
public:
	bool IsNumeric ( void ) const { return TRUE; }
};

//////////////////////////////////////////////////////////////////////////////////////////////////
// Features

class CeFeatureId
{
public:
	const CeKey& CeFeatureId::GetKey ( void ) const { return Key; }
	LPCTSTR FormatKey ( void ) const { return 0; }

private:
	CeKey Key;
};

class CeOperation;

class CeFeature
{
public:
	virtual ~CeFeature() = 0;

	LPCTSTR GetpWhat ( void ) const { return 0; }
	CeEntity* GetpEntity ( void ) const { return 0; }
	bool IsForeignId ( void ) const { return FALSE; }
	CeFeatureId* GetpId ( void ) const { return 0; }
	LPCTSTR FormatKey ( void ) const { return 0; }
	bool IsTopological ( void ) const { return TRUE; }
	CeOperation* GetpCreator ( void ) const { return 0; }
};

class CeLocation;

class CeTileData
{
public:
	const CeTileData* GetpTail ( void ) const { return 0; } // to add
	const CeTileData* GetpPrev ( void ) const { return 0; } // to add
	unsigned int GetNumLoc ( void ) const { return 0; } // to add
	const CeLocation** GetpLocations ( void ) const { return (const CeLocation**)Data; } // to add

private:
	CeLocation** Data;
};

class CeTile
{
public:
	const CeTileData* const GetpTileData ( void ) const { return 0; }
};

class CeTileId
{
public:
	CeTile* GetpTile ( void ) const { return 0; }
};

class CeOperation;
class CePoint;

class CeLocation
{
public:
	double GetEasting ( void ) const { return 0.0; }
	double GetNorthing ( void ) const { return 0.0; }
	const CeTileId&	GetTileID ( void ) const { return TileId; }
	bool operator== ( const CeLocation& rhs ) const { return FALSE; }
	CePoint* GetpPoint(const CeOperation& op, const bool onlyActive) const { return 0; }

private:
	CeTileId TileId;
};

class CeVertex
{
public:
	double GetEasting ( void ) const { return 0.0; }
	double GetNorthing ( void ) const { return 0.0; }
};

class CePoint : public CeFeature
{
public:
	const CeLocation* GetpVertex ( void ) const { return 0; }
};

class CeLine
{
public:
	virtual ~CeLine() = 0;
	CeLocation* const GetpStart ( void ) const { return 0; }
	CeLocation* const GetpEnd ( void ) const { return 0; }
};

class CeSegment : public CeLine
{
public:
};

class CeMultiSegment : public CeLine
{
public:
	unsigned int GetNumVertex ( void ) const { return 0; }
	CeLocation* const operator[] ( const unsigned int index ) const { return 0; }
};

class CeCircle
{
public:
	CePoint* GetpCentre ( const CeOperation* const pop, const bool onlyActive ) const { return 0; }
	CeClass* GetpObjects ( void ) const { return 0; }
};

class CeCurve : public CeLine
{
public:
	CeCircle* const CeCurve::GetpCircle ( void ) const { return 0; }
	bool IsClockwise ( void ) const { return FALSE; }
};

class CeSection : public CeLine
{
public:
	CeLine* const GetpBase ( void ) const { return 0; }
};

class CeArc : public CeFeature
{
public:
	//CePoint* GetStartPoint ( void ) const { return 0; }
	//CePoint* GetEndPoint ( void ) const { return 0; }
	CeLine*	const GetpLine ( void ) const { return 0; }
	CeLocation* const GetpStart ( void ) const { return 0; }
	CeLocation* const GetpEnd ( void ) const { return 0; }
};

class CeObjectList //: public CeClass
{
public:
	CeClass* const GetpFirst ( void ) const { return 0; }
	void Remove ( void ) {}
};

class CeOperation
{
public:
	virtual ~CeOperation() = 0;
	unsigned int GetFeatures ( CeObjectList& flist ) const { return 0; }
	CEOP GetType ( void ) const { return CEOP_NULL; }
};

class CeArcSubdivision : public CeOperation
{
public:
	CeArc* const GetpParent ( void ) const { return 0; }
	bool IsMultiFace ( void ) const { return FALSE; }
	CeObjectList* GetDistanceList ( const int faceIndex ) const { return 0; }
	CeObjectList* GetSectionList ( const int faceIndex ) const { return 0; }

};

class CeListIter
{
public:
	CeListIter (const CeClass* const pThing) {}
	CeListIter (const CeObjectList* const pList, bool wantDels = FALSE) {}
	void* GetHead ( void ) { return 0; }
	void* GetNext ( void ) { return 0; }
};

class CeFont
{
public:
	void GetFontTitle ( CString& fontTitle ) const {}
};

class CeText
{
public:
	virtual ~CeText() = 0;

	CeFont* GetpFont ( void ) const { return 0; }
	double GetEasting ( void ) const { return 0.0; }
	double GetNorthing ( void ) const { return 0.0; }
	double GetWidth ( void ) const { return 0.0; }
	double GetHeight ( void ) const { return 0.0; }
	double GetRotation ( void ) const { return 0.0; }
	unsigned int GetText ( CString& text ) const { return 0; }
};

class CeMiscText : public CeText
{
public:
};

class CeSchema
{
public:
	LPCTSTR GetName ( void ) const { return 0; }
};

class CeRow
{
public:
	CeSchema* GetpSchema ( void ) const { return 0; }
};

class CeTemplate
{
public:
	LPCTSTR GetName ( void ) const { return 0; }
};

class CeRowText : public CeText
{
public:
	const CeRow* GetRow ( void ) const { return 0; }
	const CeTemplate* GetTemplate ( void ) const { return 0; }
};

class CeKeyText : public CeText
{
public:
};

class CeLabel : public CeFeature
{
public:
	CeText* GetpText ( void ) const { return 0; }
	bool GetPolPosition	( CeVertex& posn ) const { return FALSE; }
	double GetEasting ( void ) const { return 0.0; }
	double GetNorthing ( void ) const { return 0.0; }
};

//////////////////////////////////////////////////////////////////////////////////////////////////
// Observations

class CeObservation
{
public:
	virtual ~CeObservation() = 0;
};

enum UNIT { UNIT_METRES=1,
			UNIT_FEET=2,
			UNIT_CHAINS=3,
			UNIT_ENTRY=4 };

class CeDistanceUnit
{
public:
	UNIT GetUnit ( void ) const { return UNIT_ENTRY; }
};

class CeDistance : public CeObservation
{
public:
	double GetDistance ( void ) const { return 0.0; }
	bool IsFixed ( void ) const { return FALSE; }
	CeDistanceUnit* GetpUnit ( void ) const { return 0; }
};

class CeOffset : public CeObservation
{
public:
	virtual ~CeOffset() = 0;
};

class CeOffsetPoint : public CeOffset
{
public:
	CePoint* GetpPoint ( void ) const { return 0; }
};

class CeOffsetDistance : public CeOffset
{
public:
	bool IsRight ( void ) const { return FALSE; }
	const CeDistance& GetOffset ( void ) const { return Distance; }

	CeDistance Distance;
};

class CeDirection : public CeObservation
{
public:
	CeOffset* GetpOffset ( void ) const { return 0; }
	double GetObservation ( void ) const { return 0.0; }
};

class CeAngle : public CeDirection
{
public:
	const CePoint* GetpBacksight ( void ) const { return 0; }
	const CePoint* GetpFrom ( void ) const { return 0; }
};

class CeDeflection : public CeAngle
{
public:
};

class CeBearing : public CeDirection
{
public:
	const CePoint* GetpFrom ( void ) const { return 0; }
};

class CeParallel : public CeDirection
{
public:
	const CePoint* GetpFrom ( void ) const { return 0; }
	CePoint* GetpStart ( void ) const { return 0; }
	CePoint* GetpEnd ( void ) const { return 0; }
};

class CeGetControl : public CeOperation
{
public:
};

class CeImport : public CeOperation
{
public:
	LPCTSTR GetFile ( void ) const { return 0; }
};

class CeGetBackground : public CeOperation
{
public:
	LPCTSTR GetFile ( void ) const { return 0; }
};

class CeRadial : public CeOperation
{
public:
	CeDirection* GetpDirection ( void ) const { return 0; }
	CeObservation* GetpLength ( void ) const { return 0; }
	CePoint* GetpPoint ( void ) const { return 0; }
	CeArc* GetpArc ( void ) const { return 0; }
};

class CeArcExtension : public CeOperation
{
public:
	CeArc* GetpExtendArc ( void ) const { return 0; }
	bool IsExtendFromEnd ( void ) const { return FALSE; }
	const CeDistance& GetLength	( void ) const { return Length; }
	CeArc* GetpNewArc ( void ) const { return 0; }
	CePoint* GetpNewPoint ( void ) const { return 0; }

private:
	CeDistance Length;
};


class CeIntersect : public CeOperation
{
public:
};

class CeIntersectDirLine : public CeIntersect
{
public:
	CeDirection* GetpDir ( void ) const { return 0; }
	CeArc* GetpDirArc ( void ) const { return 0; }
	CeArc* GetpArc ( void ) const { return 0; }
	bool IsSplit ( void ) const { return FALSE; }
	CePoint* GetpCloseTo ( void ) const { return 0; }
	CePoint* GetpIntersect ( void ) const { return 0; }
	CeArc* GetpArcBeforeSplit ( void ) const { return 0; } // to add
	CeArc* GetpArcAfterSplit ( void ) const { return 0; } // to add
};

class CeIntersectDir : public CeIntersect
{
public:
	CeDirection* GetpDir1 ( void ) const { return 0; }
	CeDirection* GetpDir2 ( void ) const { return 0; }
	CePoint* GetpIntersect ( void ) const { return 0; }
	CeArc* GetpArc1 ( void ) const { return 0; }
	CeArc* GetpArc2 ( void ) const { return 0; }
};

class CeIntersectDist : public CeIntersect
{
public:
	CeObservation* GetpDist1 ( void ) const { return 0; }
	CeObservation* GetpDist2 ( void ) const { return 0; }
	CePoint* GetpFrom1 ( void ) const { return 0; }
	CePoint* GetpFrom2 ( void ) const { return 0; }
	bool IsDefault ( void ) const { return FALSE; }
	CePoint* GetpIntersect ( void ) const { return 0; }
	CeArc* GetpArc1 ( void ) const { return 0; }
	CeArc* GetpArc2 ( void ) const { return 0; }
};

class CeIntersectDirDist : public CeIntersect
{
public:
	CeDirection* GetpDir ( void ) const { return 0; }
	CeObservation* GetpDist ( void ) const { return 0; }
	CePoint* GetpDistFrom ( void ) const { return 0; }
	bool IsDefault ( void ) const { return FALSE; }
	CePoint* GetpIntersect ( void ) const { return 0; }
	CeArc* GetpDirArc ( void ) const { return 0; }
	CeArc* GetpDistArc ( void ) const { return 0; }
};

class CeIntersectLine : public CeIntersect
{
public:
	CeArc* GetpArc1 ( void ) const { return 0; }
	CeArc* GetpArc2 ( void ) const { return 0; }
	bool IsSplit1 ( void ) const { return FALSE; }
	bool IsSplit2 ( void ) const { return FALSE; }
	CePoint* GetpCloseTo ( void ) const { return 0; }
	CePoint* GetpIntersect ( void ) const { return 0; }
	CeArc* GetpArc1a ( void ) const { return 0; } // to add
	CeArc* GetpArc1b ( void ) const { return 0; } // to add
	CeArc* GetpArc2a ( void ) const { return 0; } // to add
	CeArc* GetpArc2b ( void ) const { return 0; } // to add
};

class CeNewPoint : public CeOperation
{
public:
	CePoint* GetpPoint ( void ) const { return 0; }
};

class CeNewLabel : public CeOperation
{
public:
	CeLabel* GetpLabel ( void ) const { return 0; }
};

class CeNewArc : public CeOperation
{
public:
	CeArc* GetpArc ( void ) const { return 0; }
};

class CeAreaSubdivision : public CeOperation
{
public:
	CeLabel* GetpLabel ( void ) const { return 0; } // to add
};

class CeSetLabelRotation : public CeOperation
{
public:
	double GetRotation() const { return 0.0; } // to add
};

class CeNewCircle : public CeNewArc
{
public:
	CePoint* GetCentre ( void ) const { return 0; }
	CeObservation* GetRadius ( void ) const { return 0; }
};

class CeDeletion : public CeOperation
{
public:
	CeObjectList* GetDeletions ( void ) const { return 0; }
};

class CePosition
{
public:
	double GetEasting ( void ) const { return 0.0; }
	double GetNorthing ( void ) const { return 0.0; }
};

class CeMoveLabel : public CeOperation
{
public:
	CeLabel* GetpLabel ( void ) const { return 0; }
	const CePosition& GetOldPosition ( void ) const { return OldPosition; }

private:
	CePosition OldPosition;
};

class CeSetTopology : public CeOperation
{
public:
	CeArc* GetpArc ( void ) const { return 0; } // to add
};

class CePointOnLine : public CeOperation
{
public:
	CeArc* GetpArc ( void ) const { return 0; }
	CeDistance* GetpDistance ( void ) const { return 0; }
	CePoint* GetpNewPoint ( void ) const { return 0; }
	CeArc* GetpNewArc1 ( void ) const { return 0; } // to add
	CeArc* GetpNewArc2 ( void ) const { return 0; } // to add
};

class CeArcParallel : public CeOperation
{
public:
	CeArc* GetpRefArc ( void ) const { return 0; }
	CeObservation* GetpOffset ( void ) const { return 0; }
	CeArc* GetpTerm1 ( void ) const { return 0; }
	CeArc* GetpTerm2 ( void ) const { return 0; }
	bool IsArcReversed ( void ) const { return FALSE; }
	CePoint* GetStartPoint ( void ) const { return 0; }
	CePoint* GetEndPoint ( void ) const { return 0; }
	CeArc* GetpParArc ( void ) const { return 0; }
};

class CeArcTrim : public CeOperation
{
public:
	CeObjectList* GetArcs ( void ) const { return 0; } // to add
	CeObjectList* GetPoints ( void ) const { return 0; } // to add
};

class CeAttachPoint : public CeOperation
{
public:
	CeArc* GetpArc ( void ) const { return 0; } // to add
	CePoint* GetpPoint ( void ) const { return 0; } // to add
	unsigned int GetPositionRatio ( void ) const { return 0; } // to add
};

class CeArcSubdivisionFace
{
public:
	CeObjectList* GetDistanceList ( void ) const { return 0; }
	CeObjectList* GetSectionList ( void ) const { return 0; }
};

class CeLeg
{
public:
	virtual ~CeLeg() = 0;
	CePoint* GetpCentrePoint ( const CeOperation& op ) const { return 0; }
	CePoint* GetpEndPoint ( const CeOperation& op ) const { return 0; }
	unsigned short GetCount ( void ) const { return 0; }
	CeFeature* GetpFeature ( const unsigned short index ) const { return 0; }
	void AddToString ( CString& str ) const {}
};

class CeExtraLeg : CeLeg
{
public:
};

class CePath : public CeOperation
{
public:
	CePoint* GetpFrom ( void ) const { return 0; } // to add
	CePoint* GetpTo ( void ) const { return 0; } // to add
	void GetString ( CString& str ) const {}
	int GetNumLeg ( void ) const { return 0; }
	CeLeg* GetpLeg ( const int index ) const { return 0; }
};

//////////////////////////////////////////////////////////////////////////////

class CeTime
{
public:
	time_t GetTimeValue ( void ) const { return TimeValue; } // to add

private:
	time_t TimeValue;
};

class CePerson
{
public:
	LPCTSTR GetpWho ( void ) const { return 0; }
};

class CPSEPtrList : public CPtrList
{
public:
};

class CeSession
{
public:
	const CeTime& GetStart ( void ) const { return AnyTime; }
	const CeTime& GetEnd ( void ) const { return AnyTime; }
	CePerson* GetpWho ( void ) const { return 0; }
	const CPSEPtrList& CeSession::GetOperations ( void ) const { return Operations; }

private:
	CeTime AnyTime;
	CPSEPtrList Operations;
};

class CeMap
{
public:
	static CeMap* GetpMap ( void ) { return 0; }

	CPSEPtrList& GetSessions ( void ) { return m_Sessions; }
	LPCTSTR GetFileName ( void ) const { return 0; }
	unsigned int GetIds	( CPtrList& ids ) const { return 0; }

private:
	CPSEPtrList m_Sessions;
};

class CeTableEx
{
public:
	static unsigned int CollectRows ( CPtrList& rows, const CPtrList& ids ) { return 0; }
	static unsigned int BinRows ( CPtrList& tables, const CPtrList& rows ) { return 0; }
	const CeSchema&	GetSchema ( void ) const { return m_Schema; }
	int Export ( LPCTSTR outName ) const { return 0; }

private:
	CeSchema m_Schema;
};
