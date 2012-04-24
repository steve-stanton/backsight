#pragma once

#include "Persistent.h"
#include "Observations.h"

#ifdef _CEDIT
class CeOperation;
class CeArcExtension;
class CeArcParallel;
class CeArcTrim;
class CeArcSubdivision;
class CeArcSubdivisionFace;
class CeAreaSubdivision;
class CeAttachPoint;
class CeDeletion;
class CeGetControl;
class CeImport;
class CeIntersectDir;
class CeIntersectDirDist;
class CeIntersectDirLine;
class CeIntersectDist;
class CeIntersectLine;
class CeMoveLabel;
class CeNewArc;
class CeNewCircle;
class CeNewLabel;
class CeNewPoint;
class CePath;
class CePointOnLine;
class CeRadial;
class CeSetLabelRotation;
class CeSetTopology;
class CeGetBackground;
class CeArc;
class CePoint;
class CeObjectList;
#else
#include "CEditStubs.h"
#endif

class FeatureStub_c;
class LineFeature_c;
class PointGeometry_c;
class PointFeature_c;
class TextFeature_c;

//////////////////////////////////////////////////////////////////////////////////////////////////

class IdFactory
{
public:
	IdFactory(void);

	// Obtain an ID for something that isn't represented within a CED file
	unsigned int GetNextId()
	{
		m_MaxId++;
		return m_MaxId;
	}

	unsigned int GetNextId(void* o);
	unsigned int FindId(void* o) const;

	int GetEntityId(LPCTSTR entName);
	int GetFontId(LPCTSTR fontTitle);
	int GetTableId(LPCTSTR tableName);
	int GetTemplateId(LPCTSTR templateName);
	int GetGroupId(LPCTSTR groupName);

private:
	void LoadMappings(LPCTSTR fileName, CMapStringToPtr& index);
	int LookupId(CMapStringToPtr& index, LPCTSTR name);

private:
	unsigned int m_MaxId;
	CMapPtrToPtr m_ObjectIds;
	CMapStringToPtr m_EntityMap;
	CMapStringToPtr m_TemplateMap;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class Change_c : public Persistent_c
{
public:
	unsigned int Sequence;
	CTime When;

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;

protected:
	Change_c(unsigned int sequence, const CTime& when) : Sequence(sequence)
	{
		When = when;
	}

	Change_c(IdFactory& idFactory, const CTime& when)
	{
		Sequence = idFactory.GetNextId();
		When = when;
	}
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class NewProjectEvent_c : Change_c
{
public:
	CString ProjectId;
    CString ProjectName;
    int LayerId;
    CString DefaultSystem;
    CString UserName;
    CString MachineName;

	NewProjectEvent_c(IdFactory& idf, const CTime& when,
		LPCTSTR projectId, LPCTSTR projectName, int layerId, LPCTSTR defaultSystem,
		LPCTSTR userName, LPCTSTR machineName);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class NewSessionEvent_c : public Change_c
{
public:
    CString UserName;
    CString MachineName;

	NewSessionEvent_c(IdFactory& idf, const CTime& when, LPCTSTR userName, LPCTSTR machineName);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class EndSessionEvent_c : public Change_c
{
public:
	EndSessionEvent_c(IdFactory& idf, const CTime& when);

	virtual LPCTSTR GetTypeName() const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class IdAllocation_c : public Change_c
{
public:
    int GroupId;
    int LowestId;
    int HighestId;

	IdAllocation_c(IdFactory& idf, const CTime& when, int groupId, int lowestId, int highestId);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class Operation_c : public Change_c
{
public:
	static void LoadExportFeatures(IdFactory& idf, const CeOperation& op, CPtrArray& exportFeatures);
	static void ReleaseExportFeatures(CPtrArray& exportFeatures);
	static void ReleaseIdMappingArray(CPtrArray* idMappings);

protected:
	Operation_c(unsigned int sequence, const CTime& when)
		: Change_c(sequence, when) {}
	Operation_c(IdFactory& idFactory, const CTime& when)
		: Change_c(idFactory, when) {}
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class AttachPointOperation_c : public Operation_c
{
public:
	void* Line;
	unsigned int PositionRatio;
	FeatureStub_c* Point;

	AttachPointOperation_c(IdFactory& idf, const CTime& when, const CeAttachPoint& op);

	virtual ~AttachPointOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class DeletionOperation_c : public Operation_c
{
public:
	CUIntArray Deletions;

	DeletionOperation_c(IdFactory& idf, const CTime& when, const CeDeletion& op);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class GetControlOperation_c : public Operation_c
{
public:
	CPtrArray Points;

	GetControlOperation_c(IdFactory& idf, const CTime& when, const CeGetControl& op);

	virtual ~GetControlOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class ImportOperation_c : public Operation_c
{
public:
	CString Source;
	CPtrArray Features;

	ImportOperation_c(IdFactory& idf, const CTime& when, const CeImport& op);
	ImportOperation_c(IdFactory& idf, const CTime& when, const CeGetBackground& op);

	virtual ~ImportOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class IntersectDirectionAndDistanceOperation_c : public Operation_c
{
public:
    Direction_c* Direction;
    Observation_c* Distance;
	void* From;
	bool IsDefault;
	FeatureStub_c* To;
	FeatureStub_c* DirLine;
    FeatureStub_c* DistLine;

	IntersectDirectionAndDistanceOperation_c(IdFactory& idf, const CTime& when, const CeIntersectDirDist& op);

	virtual ~IntersectDirectionAndDistanceOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class IntersectDirectionAndLineOperation_c : public Operation_c
{
public:
	Direction_c* Direction;
    void* Line;
	bool IsSplit;
    void* CloseTo;
    FeatureStub_c* Intersection;
    FeatureStub_c* DirLine;
    unsigned int LineA;
    unsigned int LineB;

	IntersectDirectionAndLineOperation_c(IdFactory& idf, const CTime& when, const CeIntersectDirLine& op);

	virtual ~IntersectDirectionAndLineOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class IntersectTwoDirectionsOperation_c : public Operation_c
{
public:
    Direction_c* Direction1;
    Direction_c* Direction2;
    FeatureStub_c* To;
    FeatureStub_c* Line1;
    FeatureStub_c* Line2;

	IntersectTwoDirectionsOperation_c(IdFactory& idf, const CTime& when, const CeIntersectDir& op);

	virtual ~IntersectTwoDirectionsOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class IntersectTwoDistancesOperation_c : public Operation_c
{
public:
    Observation_c* Distance1;
    void* From1;
    Observation_c* Distance2;
    void* From2;
    bool IsDefault;
    FeatureStub_c* To;
    FeatureStub_c* Line1;
    FeatureStub_c* Line2;

	IntersectTwoDistancesOperation_c(IdFactory& idf, const CTime& when, const CeIntersectDist& op);

	virtual ~IntersectTwoDistancesOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class IntersectTwoLinesOperation_c : public Operation_c
{
public:
    void* Line1;
    bool IsSplit1;
    void* Line2;
    bool IsSplit2;
    void* CloseTo;
    FeatureStub_c* Intersection;
    unsigned int Line1a;
    unsigned int Line1b;
    unsigned int Line2a;
    unsigned int Line2b;

	IntersectTwoLinesOperation_c(IdFactory& idf, const CTime& when, const CeIntersectLine& op);

	virtual ~IntersectTwoLinesOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class LineExtensionOperation_c : public Operation_c
{
public:
    void* ExtendLine;
    bool IsExtendFromEnd;
    Distance_c* Length;
    FeatureStub_c* NewLine;
    FeatureStub_c* NewPoint;

	LineExtensionOperation_c(IdFactory& idf, const CTime& when, const CeArcExtension& op);

	virtual ~LineExtensionOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class LineSubdivisionFace_c : public Persistent_c
{
public:
	CPtrArray Distances;
    bool IsEntryFromEnd;

	LineSubdivisionFace_c(const CeObjectList& distances);

	virtual ~LineSubdivisionFace_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class LineSubdivisionOperation_c : public Operation_c
{
public:
    void* Line;
    LineSubdivisionFace_c* Face;
    unsigned int OtherSide;
	int PointType;
	CPtrArray* Ids;

	LineSubdivisionOperation_c(IdFactory& idf, const CTime& when, const CeArcSubdivision& op, unsigned int otherSide);

	virtual ~LineSubdivisionOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;

private:
	void LoadFace(const CeObjectList& distances, const CeObjectList& sections);
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class MoveTextOperation_c : public Operation_c
{
public:
    void* Text;
    PointGeometry_c* OldPosition;
    PointGeometry_c* OldPolPosition;
    PointGeometry_c* NewPosition;

	MoveTextOperation_c(IdFactory& idf, const CTime& when, const CeMoveLabel& op);

	virtual ~MoveTextOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class NewCircleOperation_c : public Operation_c
{
public:
	void* Center;
	Observation_c* Radius;
	FeatureStub_c* ClosingPoint;
	FeatureStub_c* Arc;

	NewCircleOperation_c(IdFactory& idf, const CTime& when, const CeNewCircle& op);

	virtual ~NewCircleOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class NewLineOperation_c : public Operation_c
{
public:
	LineFeature_c* NewLine; 

	NewLineOperation_c(IdFactory& idf, const CTime& when, const CeNewArc& op);

	virtual ~NewLineOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class NewPointOperation_c : public Operation_c
{
public:
	PointFeature_c* NewPoint;

	NewPointOperation_c(IdFactory& idf, const CTime& when, const CeNewPoint& op);

	virtual ~NewPointOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class NewTextOperation_c : public Operation_c
{
public:
	TextFeature_c* Text;

	NewTextOperation_c(IdFactory& idf, const CTime& when, const CeNewLabel& op);

	virtual ~NewTextOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class ParallelLineOperation_c : public Operation_c
{
public:
	void* RefLine;
    Observation_c* Offset;        
	void* Term1;
	void* Term2;
	bool IsArcReversed;
	FeatureStub_c* StartPoint;
	FeatureStub_c* EndPoint;
	FeatureStub_c* ParLine;

	ParallelLineOperation_c(IdFactory& idf, const CTime& when, const CeArcParallel& op);

	virtual ~ParallelLineOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class PathOperation_c : public Operation_c
{
public:
	void* From;
	void* To;
	CString EntryString;
	int DefaultEntryUnit;
	int PointType;
	int LineType;
	CPtrArray* Ids;

	PathOperation_c(IdFactory& idf, const CTime& when, const CePath& op);

	virtual ~PathOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;

private:
	CeArc* GetFirstArc(CeObjectList& features);
	CePoint* GetFirstPoint(CeObjectList& features);
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class PolygonSubdivisionOperation_c : public Operation_c
{
public:
	void* Label;
	CPtrArray Lines;

	PolygonSubdivisionOperation_c(IdFactory& idf, const CTime& when, const CeAreaSubdivision& op);

	virtual ~PolygonSubdivisionOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class RadialOperation_c : public Operation_c
{
public:
	Direction_c* Direction;
	Observation_c* Length;
	FeatureStub_c* To;
	FeatureStub_c* Line;

	RadialOperation_c(IdFactory& idf, const CTime& when, const CeRadial& radial);

	virtual ~RadialOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class SetTopologyOperation_c : public Operation_c
{
public:
	void* Line;

	SetTopologyOperation_c(IdFactory& idf, const CTime& when, const CeSetTopology& op);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class SimpleLineSubdivisionOperation_c : public Operation_c
{
public:
	void* Line;
    Distance_c* Distance;
    bool IsFromEnd;
    unsigned int NewLine1;
	FeatureStub_c* NewPoint;
    unsigned int NewLine2;

	SimpleLineSubdivisionOperation_c(IdFactory& idf, const CTime& when, const CePointOnLine& op);

	virtual ~SimpleLineSubdivisionOperation_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class TextRotationOperation_c : public Operation_c
{
public:
    double Rotation;

	TextRotationOperation_c(IdFactory& idf, const CTime& when, const CeSetLabelRotation& op);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class TrimLineOperation_c : public Operation_c
{
public:
	CUIntArray Lines;
	CUIntArray Points;

	TrimLineOperation_c(IdFactory& idf, const CTime& when, const CeArcTrim& op);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;

private:
	void LoadIdArray(IdFactory& idf, CeObjectList* features, CUIntArray& ids);
};

//////////////////////////////////////////////////////////////////////////////////////////////////
