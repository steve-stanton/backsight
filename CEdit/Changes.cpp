#include "StdAfx.h"
#include "DataField.h"
#include "EditSerializer.h"
#include "Features.h"
#include "Changes.h"
#include <assert.h>

#ifdef _CEDIT
#include "CeArcExtension.h"
#include "CeArcParallel.h"
#include "CeArcTrim.h"
#include "CeArcSubdivision.h"
#include "CeArcSubdivisionFace.h"
#include "CeAreaSubdivision.h"
#include "CeAttachPoint.h"
#include "CeDeletion.h"
#include "CeGetControl.h"
#include "CeImport.h"
#include "CeIntersectDir.h"
#include "CeIntersectDirDist.h"
#include "CeIntersectDirLine.h"
#include "CeIntersectDist.h"
#include "CeIntersectLine.h"
#include "CeLocation.h"
#include "CeMoveLabel.h"
#include "CeNewArc.h"
#include "CeNewCircle.h"
#include "CeNewLabel.h"
#include "CeNewPoint.h"
#include "CePath.h"
#include "CePointOnLine.h"
#include "CeRadial.h"
#include "CeSetLabelRotation.h"
#include "CeSetTopology.h"
#include "CeListIter.h"
#include "CeGetBackground.h"
#include "CeLabel.h"
#include "CeArc.h"
#include "CePoint.h"
#include "CeLeg.h"
#include "CeOffsetPoint.h"
#else
#include "CEditStubs.h"
#endif

//////////////////////////////////////////////////////////////////////////////////////////////////


IdFactory::IdFactory()
{
	m_MaxId = 0;

	// Load translations from a specific location
	LoadMappings("C:\\Backsight\\CEdit\\Entities.txt", m_EntityMap);
	LoadMappings("C:\\Backsight\\CEdit\\Templates.txt", m_TemplateMap);
	LoadMappings("C:\\Backsight\\CEdit\\IdGroups.txt", m_IdGroupMap);
}

void IdFactory::LoadMappings(LPCTSTR fileName, CMapStringToPtr& index)
{
	FILE* fp = fopen(fileName,"r");
	char buf[1024];

	while ( fgets(buf,sizeof(buf),fp) )
	{
		// Grab the buffer into a CString, trim off any leading and trailing whitespace.
		CString str(buf);
		str.TrimLeft();
		str.TrimRight();

		// Skip blank records.
		int nc = str.GetLength();
		if ( nc==0 ) continue;

		int eqpos = str.Find('=');
		if (eqpos > 0)
		{
			CString ids = str.Left(eqpos);
			ids.TrimLeft();
			ids.TrimRight();

			unsigned int id;
			sscanf((LPCTSTR)ids, "%d", &id);
			CString entName(str.Mid(eqpos+1));
			entName.TrimLeft();
			entName.TrimRight();

			// I THINK that SetAt creates a new copy
			index.SetAt(entName, (void*)id);
		}
	}

	fclose(fp);
}

int IdFactory::LookupId(CMapStringToPtr& index, LPCTSTR name)
{
	void* result;
	if (index.Lookup(name, result))
		return (int)result;
	else
		return 0;
}

unsigned int IdFactory::GetNextId(void* p)
{
	m_MaxId++;

	if (p != 0)
		m_ObjectIds.SetAt(p, (void*)m_MaxId);

	CePoint* pt = dynamic_cast<CePoint*>((CeClass*)p);
	if (pt != 0)
	{
		const CeLocation* loc = pt->GetpVertex();
		PointFeature_c::IndexAllLocations(*this, loc, m_MaxId);
	}

	return m_MaxId;
}

// Adds an additional index entry (this makes it possible to relate more than one CED object
// to the same Backsight ID).
void IdFactory::AddIndexEntry(void* p, unsigned int id)
{
	m_ObjectIds.SetAt(p, (void*)id);
}

unsigned int IdFactory::FindId(void* p) const
{
	void* result;
	if (m_ObjectIds.Lookup(p, result))
		return (unsigned int)result;

	return 0;
}

int IdFactory::GetEntityId(LPCTSTR entName)
{
	return LookupId(m_EntityMap, entName);
}

int IdFactory::GetFontId(LPCTSTR fontTitle)
{
	return 0;
}

int IdFactory::GetTableId(LPCTSTR tableName)
{
	return 123;
}

int IdFactory::GetTemplateId(LPCTSTR templateName)
{
	return LookupId(m_TemplateMap, templateName);
}

int IdFactory::GetGroupId(LPCTSTR groupName)
{
	return LookupId(m_IdGroupMap, groupName);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

LPCTSTR Change_c::GetTypeName() const
{
	static LPCTSTR typeName = "Change";
	return typeName;
}

void Change_c::WriteData(EditSerializer& s) const
{
    s.WriteUInt32(DataField_Id, Sequence);
    s.WriteDateTime(DataField_When, When);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

NewProjectEvent_c::NewProjectEvent_c(IdFactory& idf, const CTime& when,
		LPCTSTR projectId, LPCTSTR projectName, int layerId, LPCTSTR defaultSystem,
		LPCTSTR userName, LPCTSTR machineName)
		: Change_c(idf, when)
{
	ProjectId = projectId;
    ProjectName = projectName;
    LayerId = layerId;
    DefaultSystem = defaultSystem;
    UserName = userName;
    MachineName = machineName;
}

LPCTSTR NewProjectEvent_c::GetTypeName() const
{
	static LPCTSTR typeName = "NewProjectEvent";
	return typeName;
}

void NewProjectEvent_c::WriteData(EditSerializer& s) const
{
	Change_c::WriteData(s);
    s.WriteString(DataField_ProjectId, (LPCTSTR)ProjectId);
    s.WriteString(DataField_ProjectName, (LPCTSTR)ProjectName);
    s.WriteInt32(DataField_LayerId, LayerId);
    s.WriteString(DataField_CoordinateSystem, (LPCTSTR)DefaultSystem);
    s.WriteString(DataField_UserName, (LPCTSTR)UserName);
    s.WriteString(DataField_MachineName, (LPCTSTR)MachineName);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

NewSessionEvent_c::NewSessionEvent_c(IdFactory& idf, const CTime& when, LPCTSTR userName, LPCTSTR machineName)
		: Change_c(idf, when)
{
	UserName = userName;
	MachineName = machineName;
}

LPCTSTR NewSessionEvent_c::GetTypeName() const
{
	static LPCTSTR typeName = "NewSessionEvent";
	return typeName;
}

void NewSessionEvent_c::WriteData(EditSerializer& s) const
{
	Change_c::WriteData(s);
    s.WriteString(DataField_UserName, (LPCTSTR)UserName);
    s.WriteString(DataField_MachineName, (LPCTSTR)MachineName);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

EndSessionEvent_c::EndSessionEvent_c(IdFactory& idf, const CTime& when)
		: Change_c(idf, when)
{
}

LPCTSTR EndSessionEvent_c::GetTypeName() const
{
	static LPCTSTR typeName = "EndSessionEvent";
	return typeName;
}

//////////////////////////////////////////////////////////////////////////////////////////////////

IdAllocation_c::IdAllocation_c(IdFactory& idf, const CTime& when, int groupId, int lowestId, int highestId)
		: Change_c(idf, when)
{
	GroupId = groupId;
	LowestId = lowestId;
	HighestId = highestId;
}

LPCTSTR IdAllocation_c::GetTypeName() const
{
	static LPCTSTR typeName = "IdAllocation";
	return typeName;
}

void IdAllocation_c::WriteData(EditSerializer& s) const
{
	Change_c::WriteData(s);
    s.WriteInt32(DataField_GroupId, GroupId);
    s.WriteInt32(DataField_LowestId, LowestId);
    s.WriteInt32(DataField_HighestId, HighestId);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

// static
void Operation_c::LoadExportFeatures(IdFactory& idf, const CeOperation& op, CPtrArray& exportFeatures, bool doPointsFirst)
{
	CeObjectList obList;
	op.GetFeatures(obList);

	CeListIter loop(&obList, TRUE);
	const CeFeature* f;

	if (doPointsFirst)
	{
		for ( f = (const CeFeature*)loop.GetHead();
			  f;
			  f = (const CeFeature*)loop.GetNext() )
		{
			const CePoint* p = dynamic_cast<const CePoint*>(f);
			if (p != 0)
			{
				Feature_c* xf = Feature_c::CreateExportFeature(idf, *f);
				exportFeatures.Add(xf);
			}
		}

		// Repeat for non-points

		for ( f = (const CeFeature*)loop.GetHead();
			  f;
			  f = (const CeFeature*)loop.GetNext() )
		{
			const CePoint* p = dynamic_cast<const CePoint*>(f);
			if (p == 0)
			{
				Feature_c* xf = Feature_c::CreateExportFeature(idf, *f);
				exportFeatures.Add(xf);
			}
		}
	}
	else
	{
		for ( f = (const CeFeature*)loop.GetHead();
			  f;
			  f = (const CeFeature*)loop.GetNext() )
		{
			Feature_c* xf = Feature_c::CreateExportFeature(idf, *f);
			exportFeatures.Add(xf);
		}
	}
}

// static 
void Operation_c::ReleaseExportFeatures(CPtrArray& exportFeatures)
{
	for (int i=0; i<exportFeatures.GetSize(); i++)
	{
		Feature_c* pf = (Feature_c*)exportFeatures.GetAt(i);
		delete pf;
	}
}

// static
void Operation_c::ReleaseIdMappingArray(CPtrArray* idMappings)
{
	if (idMappings != 0)
	{
		for (int i=0; i<idMappings->GetSize(); i++)
		{
			IdMapping_c* m = (IdMapping_c*)idMappings->GetAt(i);
			delete m;
		}

		delete idMappings;
	}
}

//////////////////////////////////////////////////////////////////////////////////////////////////

AttachPointOperation_c::AttachPointOperation_c(IdFactory& idf, const CTime& when, const CeAttachPoint& op)
	: Operation_c(idf, when)
{
	Line = op.GetpArc();
	PositionRatio = op.GetPositionRatio();
	Point = new FeatureStub_c(idf, *(op.GetpPoint()));
}

AttachPointOperation_c::~AttachPointOperation_c()
{
	delete Point;
}

LPCTSTR AttachPointOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "AttachPointOperation";
	return typeName;
}

void AttachPointOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

    s.WriteFeatureRef(DataField_Line, Line);
    s.WriteUInt32(DataField_PositionRatio, PositionRatio);
    s.WritePersistent(DataField_Point, *Point);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

DeletionOperation_c::DeletionOperation_c(IdFactory& idf, const CTime& when, const CeDeletion& op)
	: Operation_c(idf, when)
{
	// To be consistent, we would normally hold on to an array of void pointers, and convert to
	// internal IDs on the WriteData call. Just convert now, since a deletion can never make
	// forward references.

	CeObjectList* dels = op.GetDeletions();
	CeListIter loop(dels, TRUE);
	void* pThing;

	for ( pThing = loop.GetHead();
		  pThing;
		  pThing = loop.GetNext() )
	{
		unsigned int iid = idf.FindId(pThing);
		assert(iid < this->Sequence);
		Deletions.Add(iid);
	}
}

LPCTSTR DeletionOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "DeletionOperation";
	return typeName;
}

void DeletionOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);
    s.WriteSimpleArray(DataField_Delete, Deletions);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

GetControlOperation_c::GetControlOperation_c(IdFactory& idf, const CTime& when, const CeGetControl& op)
	: Operation_c(idf, when)
{
	Operation_c::LoadExportFeatures(idf, op, Points);
}

GetControlOperation_c::~GetControlOperation_c()
{
	Operation_c::ReleaseExportFeatures(Points);
}

LPCTSTR GetControlOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "GetControlOperation";
	return typeName;
}

void GetControlOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);
    s.WritePersistentArray(DataField_Points, Points);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

ImportOperation_c::ImportOperation_c(IdFactory& idf, const CTime& when)
	: Operation_c(idf, when)
{
	Source = "";
}

ImportOperation_c::ImportOperation_c(IdFactory& idf, const CTime& when, const CeImport& op)
	: Operation_c(idf, when)
{
	Source = op.GetFile();
	Operation_c::LoadExportFeatures(idf, op, Features, TRUE);
}

ImportOperation_c::ImportOperation_c(IdFactory& idf, const CTime& when, const CeGetBackground& op)
	: Operation_c(idf, when)
{
	Source = op.GetFile();
	Operation_c::LoadExportFeatures(idf, op, Features, TRUE);
}

ImportOperation_c::~ImportOperation_c()
{
	Operation_c::ReleaseExportFeatures(Features);
}

LPCTSTR ImportOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "ImportOperation";
	return typeName;
}

void ImportOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);
	s.WriteString(DataField_Source, Source);
    s.WritePersistentArray(DataField_Features, Features);
}

void ImportOperation_c::Add(Feature_c* f)
{
	Features.Add(f);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

IntersectDirectionAndDistanceOperation_c::IntersectDirectionAndDistanceOperation_c(
	IdFactory& idf, const CTime& when, const CeIntersectDirDist& op)
	: Operation_c(idf, when)
{
	Direction = Direction_c::CreateExportDirection(op.GetpDir());
	Distance = Observation_c::CreateExportLength(op.GetpDist());
	From = op.GetpDistFrom();
	IsDefault = op.IsDefault();
	To = new FeatureStub_c(idf, *(op.GetpIntersect()));

	CeArc* dirLine = op.GetpDirArc();
	if (dirLine == 0)
		DirLine = 0;
	else
		DirLine = new FeatureStub_c(idf, *dirLine);

	CeArc* distLine = op.GetpDistArc();
	if (distLine == 0)
		DistLine = 0;
	else
		DistLine = new FeatureStub_c(idf, *distLine);
}

IntersectDirectionAndDistanceOperation_c::~IntersectDirectionAndDistanceOperation_c()
{
	delete Direction;
	delete Distance;
	delete To;
	delete DirLine;
	delete DistLine;
}

LPCTSTR IntersectDirectionAndDistanceOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "IntersectDirectionAndDistanceOperation";
	return typeName;
}

void IntersectDirectionAndDistanceOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

    s.WritePersistent(DataField_Direction, *Direction);
    s.WritePersistent(DataField_Distance, *Distance);
    s.WriteFeatureRef(DataField_From, From);
    s.WriteBool(DataField_Default, IsDefault);
    s.WritePersistent(DataField_To, *To);

    if (DirLine != 0)
        s.WritePersistent(DataField_DirLine, *DirLine);

    if (DistLine != 0)
        s.WritePersistent(DataField_DistLine, *DistLine);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

IntersectDirectionAndLineOperation_c::IntersectDirectionAndLineOperation_c(IdFactory& idf, const CTime& when, const CeIntersectDirLine& op)
	: Operation_c(idf, when)
{
	Direction = Direction_c::CreateExportDirection(op.GetpDir());
	Line = op.GetpArc();
	IsSplit = op.IsSplit();
	CloseTo = op.GetpCloseTo();
	Intersection = new FeatureStub_c(idf, *(op.GetpIntersect()));

	CeArc* dirLine = op.GetpDirArc();
	if (dirLine == 0)
		DirLine = 0;
	else
		DirLine = new FeatureStub_c(idf, *dirLine);

	CeArc* beforeSplit = op.GetpArcBeforeSplit();
	if (beforeSplit == 0)
		LineA = 0;
	else
		LineA = idf.GetNextId(beforeSplit);


	CeArc* afterSplit = op.GetpArcAfterSplit();
	if (afterSplit == 0)
		LineB = 0;
	else
		LineB = idf.GetNextId(afterSplit);
}

IntersectDirectionAndLineOperation_c::~IntersectDirectionAndLineOperation_c()
{
	delete Direction;
    delete Intersection;
    delete DirLine;
}

LPCTSTR IntersectDirectionAndLineOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "IntersectDirectionAndLineOperation";
	return typeName;
}

void IntersectDirectionAndLineOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

	s.WritePersistent(DataField_Direction, *Direction);
    s.WriteFeatureRef(DataField_Line, Line);
    s.WriteFeatureRef(DataField_CloseTo, CloseTo);
    s.WritePersistent(DataField_To, *Intersection);

    if (DirLine != 0)
        s.WritePersistent(DataField_DirLine, *DirLine);

    if (LineA != 0)
        s.WriteInternalId(DataField_SplitBefore, LineA);

    if (LineB != 0)
        s.WriteInternalId(DataField_SplitAfter, LineB);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

IntersectTwoDirectionsOperation_c::IntersectTwoDirectionsOperation_c(IdFactory& idf, const CTime& when, const CeIntersectDir& op)
	: Operation_c(idf, when)
{
	Direction1 = Direction_c::CreateExportDirection(op.GetpDir1());
	Direction2 = Direction_c::CreateExportDirection(op.GetpDir2());
	To = new FeatureStub_c(idf, *(op.GetpIntersect()));

	CeArc* line1 = op.GetpArc1();
	if (line1 == 0)
		Line1 = 0;
	else
		Line1 = new FeatureStub_c(idf, *line1);

	CeArc* line2 = op.GetpArc2();
	if (line2 == 0)
		Line2 = 0;
	else
		Line2 = new FeatureStub_c(idf, *line2);
}

IntersectTwoDirectionsOperation_c::~IntersectTwoDirectionsOperation_c()
{
	delete Direction1;
	delete Direction2;
	delete To;
	delete Line1;
	delete Line2;
}

LPCTSTR IntersectTwoDirectionsOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "IntersectTwoDirectionsOperation";
	return typeName;
}

void IntersectTwoDirectionsOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

    s.WritePersistent(DataField_Direction1, *Direction1);
    s.WritePersistent(DataField_Direction2, *Direction2);
    s.WritePersistent(DataField_To, *To);

    if (Line1 != 0)
        s.WritePersistent(DataField_Line1, *Line1);

    if (Line2 != 0)
        s.WritePersistent(DataField_Line2, *Line2);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

IntersectTwoDistancesOperation_c::IntersectTwoDistancesOperation_c(IdFactory& idf, const CTime& when, const CeIntersectDist& op)
	: Operation_c(idf, when)
{
	Distance1 = Observation_c::CreateExportLength(op.GetpDist1());
	From1 = op.GetpFrom1();
	Distance2 = Observation_c::CreateExportLength(op.GetpDist2());
	From2 = op.GetpFrom2();
	IsDefault = op.IsDefault();
	To = new FeatureStub_c(idf, *(op.GetpIntersect()));

	CeArc* line1 = op.GetpArc1();
	if (line1 == 0)
		Line1 = 0;
	else
		Line1 = new FeatureStub_c(idf, *line1);

	CeArc* line2 = op.GetpArc2();
	if (line2 == 0)
		Line2 = 0;
	else
		Line2 = new FeatureStub_c(idf, *line2);
}

IntersectTwoDistancesOperation_c::~IntersectTwoDistancesOperation_c()
{
	delete Distance1;
	delete Distance2;
	delete To;
	delete Line1;
	delete Line2;
}

LPCTSTR IntersectTwoDistancesOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "IntersectTwoDistancesOperation";
	return typeName;
}

void IntersectTwoDistancesOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

    s.WritePersistent(DataField_Distance1, *Distance1);
    s.WriteFeatureRef(DataField_From1, From1);
    s.WritePersistent(DataField_Distance2, *Distance2);
    s.WriteFeatureRef(DataField_From2, From2);
    s.WriteBool(DataField_Default, IsDefault);
    s.WritePersistent(DataField_To, *To);

    if (Line1 != 0)
        s.WritePersistent(DataField_Line1, *Line1);

    if (Line2 != 0)
        s.WritePersistent(DataField_Line2, *Line2);
}


//////////////////////////////////////////////////////////////////////////////////////////////////

IntersectTwoLinesOperation_c::IntersectTwoLinesOperation_c(IdFactory& idf, const CTime& when, const CeIntersectLine& op)
	: Operation_c(idf, when)
{
	Line1 = op.GetpArc1();
	IsSplit1 = op.IsSplit1();
	Line2 = op.GetpArc2();
	IsSplit2 = op.IsSplit2();
	CloseTo = op.GetpCloseTo();
	Intersection = new FeatureStub_c(idf, *(op.GetpIntersect()));

	CeArc* line1a = op.GetpArc1a();
	if (line1a == 0)
		Line1a = 0;
	else
		Line1a = idf.GetNextId(line1a);

	CeArc* line1b = op.GetpArc1b();
	if (line1b == 0)
		Line1b = 0;
	else
		Line1b = idf.GetNextId(line1b);

	CeArc* line2a = op.GetpArc2a();
	if (line2a == 0)
		Line2a = 0;
	else
		Line2a = idf.GetNextId(line2a);

	CeArc* line2b = op.GetpArc2b();
	if (line2b == 0)
		Line2b = 0;
	else
		Line2b = idf.GetNextId(line2b);
}

IntersectTwoLinesOperation_c::~IntersectTwoLinesOperation_c()
{
	delete Intersection;
}

LPCTSTR IntersectTwoLinesOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "IntersectTwoLinesOperation";
	return typeName;
}

void IntersectTwoLinesOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

    s.WriteFeatureRef(DataField_Line1, Line1);
    s.WriteFeatureRef(DataField_Line2, Line2);
    s.WriteFeatureRef(DataField_CloseTo, CloseTo);
    s.WritePersistent(DataField_To, *Intersection);

    if (Line1a != 0)
        s.WriteInternalId(DataField_SplitBefore1, Line1a);

    if (Line1b != 0)
        s.WriteInternalId(DataField_SplitAfter1, Line1b);

    if (Line2a != 0)
        s.WriteInternalId(DataField_SplitBefore2, Line2a);

    if (Line2b != 0)
        s.WriteInternalId(DataField_SplitAfter2, Line2b);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

LineExtensionOperation_c::LineExtensionOperation_c(IdFactory& idf, const CTime& when, const CeArcExtension& op)
	: Operation_c(idf, when)
{
	ExtendLine = (void*)op.GetpExtendArc();
	IsExtendFromEnd = op.IsExtendFromEnd();
	Length = new Distance_c(op.GetLength());
	NewPoint = new FeatureStub_c(idf, *(op.GetpNewPoint()));

	const CeArc* newLine = op.GetpNewArc();
	if (newLine == 0)
		NewLine = 0;
	else
		NewLine = new FeatureStub_c(idf, *newLine);
}

LineExtensionOperation_c::~LineExtensionOperation_c()
{
    delete Length;
    delete NewLine;
    delete NewPoint;
}

LPCTSTR LineExtensionOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "LineExtensionOperation";
	return typeName;
}

void LineExtensionOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

    s.WriteFeatureRef(DataField_Line, ExtendLine);
    s.WriteBool(DataField_ExtendFromEnd, IsExtendFromEnd);
    s.WritePersistent(DataField_Distance, *Length);
    s.WritePersistent(DataField_NewPoint, *NewPoint);

    if (NewLine != 0)
        s.WritePersistent(DataField_NewLine, *NewLine);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

LineSubdivisionFace_c::LineSubdivisionFace_c(const CeObjectList& distances)
{
    IsEntryFromEnd = false;

	CeListIter dLoop(&distances);
	const CeDistance* d;

	for ( d = (const CeDistance*)dLoop.GetHead();
		  d;
		  d = (const CeDistance*)dLoop.GetNext() )
	{
		Distance_c* xd = new Distance_c(*d);
		Distances.Add(xd);
	}
}

LineSubdivisionFace_c::~LineSubdivisionFace_c()
{
	for (int i=0; i<Distances.GetSize(); i++)
	{
		Distance_c* xd = (Distance_c*)Distances.GetAt(i);
		delete xd;
	}
}

LPCTSTR LineSubdivisionFace_c::GetTypeName() const
{
	static LPCTSTR typeName = "LineSubdivisionFace";
	return typeName;
}

void LineSubdivisionFace_c::WriteData(EditSerializer& s) const
{
    s.WriteBool(DataField_EntryFromEnd, IsEntryFromEnd);
    s.WritePersistentArray(DataField_Sections, Distances);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

LineSubdivisionOperation_c::LineSubdivisionOperation_c(IdFactory& idf, const CTime& when, const CeArcSubdivision& op, unsigned int otherSide)
	: Operation_c(idf, when)
{
	Line = op.GetpParent();
	OtherSide = otherSide;
	Ids = 0;

	int faceIndex;

	if (otherSide == 0)
		faceIndex = 0;
	else
		faceIndex = 1;

	CeObjectList* distances = op.GetDistanceList(faceIndex);
	Face = new LineSubdivisionFace_c(*distances);

	CeObjectList* sections = op.GetSectionList(faceIndex);
	CeArc* firstArc = (CeArc*)sections->GetpFirst();
	PointType = idf.GetEntityId(firstArc->GetpWhat());

	CeListIter loop(sections, TRUE);
	CeArc* a;

	for ( a = (CeArc*)loop.GetHead();
		  a;
		  a = (CeArc*)loop.GetNext() )
	{
		CePoint* p = a->GetpEnd()->GetpPoint(op, FALSE);
		if (p != 0 && p->GetpCreator() == (CeOperation*)&op)
		{
			unsigned int iid = idf.GetNextId(p);

			// If the point has a user-perceived ID, remember the mapping
			unsigned int rawId = Feature_c::GetRawId(*p);
			if (rawId != 0)
			{
				IdMapping_c* m = new IdMapping_c(iid, rawId);

				if (Ids == 0)
					Ids = new CPtrArray();

				Ids->Add(m);
			}
		}

		idf.GetNextId(a);
	}
}

LineSubdivisionOperation_c::~LineSubdivisionOperation_c()
{
	delete Face;
	Operation_c::ReleaseIdMappingArray(Ids);
}

LPCTSTR LineSubdivisionOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "LineSubdivisionOperation";
	return typeName;
}

void LineSubdivisionOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

    s.WriteFeatureRef(DataField_Line, Line);
    s.WritePersistent(DataField_Face, *Face);

    if (OtherSide != 0)
        s.WriteInternalId(DataField_OtherSide, OtherSide);

	s.WriteInt32(DataField_PointType, PointType);

	if (Ids != 0)
		s.WritePersistentArray(DataField_Ids, *Ids);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

MoveTextOperation_c::MoveTextOperation_c(IdFactory& idf, const CTime& when, const CeMoveLabel& op)
	: Operation_c(idf, when)
{
	CeLabel* label = op.GetpLabel();
	Text = label;

	// What if it was previously moved? (will presumably lose intervening positions)
	OldPosition = new PointGeometry_c(op.GetOldPosition());
	NewPosition = new PointGeometry_c(label->GetEasting(), label->GetNorthing());

	// I don't think the old edit dealt with polygon reference points
	OldPolPosition = 0;
}

MoveTextOperation_c::~MoveTextOperation_c()
{
	delete OldPosition;
	delete OldPolPosition;
	delete NewPosition;
}

LPCTSTR MoveTextOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "MoveTextOperation";
	return typeName;
}

void MoveTextOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

    s.WriteFeatureRef(DataField_Text, Text);
    s.WritePointGeometry(DataField_OldX, DataField_OldY, *OldPosition);
    s.WritePointGeometry(DataField_NewX, DataField_NewY, *NewPosition);

    if (OldPolPosition != 0)
        s.WritePointGeometry(DataField_OldPolygonX, DataField_OldPolygonY, *OldPolPosition);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

NewCircleOperation_c::NewCircleOperation_c(IdFactory& idf, const CTime& when, const CeNewCircle& op)
	: Operation_c(idf, when)
{
	Center = op.GetCentre();
	Radius = Observation_c::CreateExportLength(op.GetRadius());

	if (dynamic_cast<CeOffsetPoint*>(op.GetRadius()) == 0)
	{
		CeArc* arc = op.GetpArc();
		CePoint* cp = arc->GetpStart()->GetpPoint(op, FALSE);
		assert(cp->GetpCreator() == (CeOperation*)&op);
		ClosingPoint = new FeatureStub_c(idf, *cp);
	}
	else
	{
		ClosingPoint = 0;
	}

	Arc = new FeatureStub_c(idf, *(op.GetpArc()));
}

NewCircleOperation_c::~NewCircleOperation_c()
{
	delete Radius;
	delete ClosingPoint;
	delete Arc;
}

LPCTSTR NewCircleOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "NewCircleOperation";
	return typeName;
}

void NewCircleOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

    s.WriteFeatureRef(DataField_Center, Center);
    s.WritePersistent(DataField_Radius, *Radius);

	if (ClosingPoint != 0)
        s.WritePersistent(DataField_ClosingPoint, *ClosingPoint);

    s.WritePersistent(DataField_Arc, *Arc);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

NewLineOperation_c::NewLineOperation_c(IdFactory& idf, const CTime& when, const CeNewArc& op)
	: Operation_c(idf, when)
{
	NewLine = new LineFeature_c(idf, *(op.GetpArc()));
}

NewLineOperation_c::~NewLineOperation_c()
{
	delete NewLine;
}

LPCTSTR NewLineOperation_c::GetTypeName() const
{
	LineGeometry_c* geom = NewLine->Geom;

	if (geom == 0)
	{
		static LPCTSTR segmentEditName = "NewSegmentOperation";
		return segmentEditName;
	}

	// Only arcs or complete circles can be created via this NewLineOperation (multisegments come
	// in only via imports, sections come in via other edits).

	ArcGeometry_c* arc = dynamic_cast<ArcGeometry_c*>(geom);
	if (arc != 0)
	{
		if (NewLine->From == NewLine->To)
		{
			static LPCTSTR circleEditName = "NewCircleOperation";
			return circleEditName;
		}

		static LPCTSTR arcEditName = "NewArcOperation";
		return arcEditName;
	}

	assert (1==0);
	return 0;
}

void NewLineOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);
	s.WritePersistent(DataField_Line, *NewLine);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

NewPointOperation_c::NewPointOperation_c(IdFactory& idf, const CTime& when, const CeNewPoint& op)
	: Operation_c(idf, when)
{
	NewPoint = new PointFeature_c(idf, *(op.GetpPoint()));
}

NewPointOperation_c::~NewPointOperation_c()
{
	delete NewPoint;
}

LPCTSTR NewPointOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "NewPointOperation";
	return typeName;
}

void NewPointOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);
	s.WritePersistent(DataField_Point, *NewPoint);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

NewTextOperation_c::NewTextOperation_c(IdFactory& idf, const CTime& when, const CeNewLabel& op)
	: Operation_c(idf, when)
{
	Text = new TextFeature_c(idf, *(op.GetpLabel()));
}

NewTextOperation_c::~NewTextOperation_c()
{
	delete Text;
}

LPCTSTR NewTextOperation_c::GetTypeName() const
{
	TextGeometry_c* geom = Text->Geom;

	KeyTextGeometry_c* keyText = dynamic_cast<KeyTextGeometry_c*>(geom);
	if (keyText != 0)
	{
		static LPCTSTR keyTextEditName = "NewKeyTextOperation";
		return keyTextEditName;
	}

	MiscTextGeometry_c* miscText = dynamic_cast<MiscTextGeometry_c*>(geom);
	if (miscText != 0)
	{
		static LPCTSTR miscTextEditName = "NewMiscTextOperation";
		return miscTextEditName;
	}

	RowTextGeometry_c* rowText = dynamic_cast<RowTextGeometry_c*>(geom);
	if (rowText != 0)
	{
		static LPCTSTR rowTextEditName = "NewRowTextOperation";
		return rowTextEditName;
	}

	assert(1==0);
	return 0;
}

void NewTextOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);
	s.WritePersistent(DataField_Text, *Text);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

ParallelLineOperation_c::ParallelLineOperation_c(IdFactory& idf, const CTime& when, const CeArcParallel& op)
	: Operation_c(idf, when)
{
	RefLine = op.GetpRefArc();
	Offset = Observation_c::CreateExportLength(op.GetpOffset());
	Term1 = op.GetpTerm1();
	Term2 = op.GetpTerm2();
	IsArcReversed = op.IsArcReversed();

	CePoint* start = op.GetStartPoint();
	if (start == 0)
		StartPoint = 0;
	else
		StartPoint = new FeatureStub_c(idf, *start);

	CePoint* end = op.GetEndPoint();
	if (end == 0)
		EndPoint = 0;
	else
		EndPoint = new FeatureStub_c(idf, *end);

	ParLine = new FeatureStub_c(idf, *(op.GetpParArc()));
}

ParallelLineOperation_c::~ParallelLineOperation_c()
{
	delete Offset;
	delete StartPoint;
	delete EndPoint;
	delete ParLine;
}

LPCTSTR ParallelLineOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "ParallelLineOperation";
	return typeName;
}

void ParallelLineOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);
    s.WriteFeatureRef(DataField_RefLine, RefLine);

    if (Term1 != 0)
        s.WriteFeatureRef(DataField_Term1, Term1);

    if (Term2 != 0)
        s.WriteFeatureRef(DataField_Term2, Term2);

    if (IsArcReversed)
        s.WriteBool(DataField_ReverseArc, true);

    s.WritePersistent(DataField_Offset, *Offset);

    if (StartPoint != 0)
        s.WritePersistent(DataField_From, *StartPoint);

    if (EndPoint != 0)
        s.WritePersistent(DataField_To, *EndPoint);

    s.WritePersistent(DataField_NewLine, *ParLine);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

PathOperation_c::PathOperation_c(IdFactory& idf, const CTime& when, const CePath& op)
	: Operation_c(idf, when)
{
	From = op.GetpFrom();
	To = op.GetpTo();

	// Determine the default entity types by looking for the first point/line created
	// by the edit. In the unlikely event that no points were created (i.e. a single
	// span connection path), get by with an undefined entity type (if really necessary,
	// may want to consider representing the connection path as a NewLineOperation).
	CeObjectList features;
	op.GetFeatures(features);

	CePoint* fp = Feature_c::GetFirstPoint(features);
	if (fp == 0)
		PointType = 0;
	else
		PointType = idf.GetEntityId(fp->GetpWhat());

	CeArc* fa = Feature_c::GetFirstArc(features);
	if (fa == 0)
		LineType = 0;
	else
		LineType = idf.GetEntityId(fa->GetpWhat());

	// The data entry string will have units attached to every observed distance,
	// so it shouldn't matter what value we use for the default entry units.
	op.GetString(EntryString);
	DefaultEntryUnit = 0;

	// Allocate IDs for every leg plus 2 for every span (regardless of whether it
	// has a line feature).

	for (int i=0; i<op.GetNumLeg(); i++)
	{
		CeLeg* leg = op.GetpLeg(i);

		// Assign ID for center point (if it's there). If the leg doesn't have a center
		// point (e.g. it's a straight leg), this just reserves an ID
		CePoint* center = leg->GetpCentrePoint(op);
		idf.GetNextId(center);

		// Cul-de-sacs may have no observed distances, but they still generate a line
		unsigned short nSpan = leg->GetCount();
		if (nSpan == 0)
			nSpan = 1;

		for (unsigned short iSpan=0; iSpan<nSpan; iSpan++)
		{
			// Get the line and/or point for the span (if any)

			CeFeature* f = leg->GetpFeature(iSpan);
			CeArc* a = 0;
			CePoint* p = 0;

			if (f != 0)
			{
				a = dynamic_cast<CeArc*>(f);
				if (a == 0)
				{
					p = dynamic_cast<CePoint*>(f);
					assert(p != 0);
				}
				else
				{
					p = a->GetpEnd()->GetpPoint(op, FALSE);

					// Don't assign ID to the end point if it's the end of the path
					if (p == op.GetpTo())
						p = 0;
				}
			}

			// The point (if there is one) always gets the next ID number
			unsigned int iid = idf.GetNextId(p);
			if (p != 0)
			{
				// If the point has a user-perceived ID, remember the mapping
				unsigned int rawId = Feature_c::GetRawId(*p);
				if (rawId != 0)
				{
					IdMapping_c* m = new IdMapping_c(iid, rawId);
					Ids.Add(m);
				}
			}

			// Lines were never assigned IDs in CEdit.
			idf.GetNextId(a);
		}
	}
}

PathOperation_c::~PathOperation_c()
{
	Operation_c::ReleaseIdMappingArray(&Ids);
}

LPCTSTR PathOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "PathOperation";
	return typeName;
}

void PathOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

    s.WriteFeatureRef(DataField_From, From);
    s.WriteFeatureRef(DataField_To, To);
    s.WriteString(DataField_EntryString, EntryString);
    s.WriteInt32(DataField_DefaultEntryUnit, DefaultEntryUnit);
    s.WriteInt32(DataField_PointType, PointType);
    s.WriteInt32(DataField_LineType, LineType);

	if (Ids.GetSize() > 0)
		s.WritePersistentArray(DataField_Ids, Ids);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

PolygonSubdivisionOperation_c::PolygonSubdivisionOperation_c(IdFactory& idf, const CTime& when, const CeAreaSubdivision& op)
	: Operation_c(idf, when)
{
	Label = op.GetpLabel();
	Operation_c::LoadExportFeatures(idf, op, Lines);
}

PolygonSubdivisionOperation_c::~PolygonSubdivisionOperation_c()
{
	Operation_c::ReleaseExportFeatures(Lines);
}

LPCTSTR PolygonSubdivisionOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "PolygonSubdivisionOperation";
	return typeName;
}

void PolygonSubdivisionOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

    if (Label != 0)
        s.WriteFeatureRef(DataField_DeactivatedLabel, Label);

    s.WritePersistentArray(DataField_Lines, Lines);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

RadialOperation_c::RadialOperation_c(IdFactory& idf, const CTime& when, const CeRadial& op)
	: Operation_c(idf, when)
{
	Direction = Direction_c::CreateExportDirection(op.GetpDirection());
	Length = Observation_c::CreateExportLength(op.GetpLength());
	To = new FeatureStub_c(idf, *(op.GetpPoint()));

	CeArc* line = op.GetpArc();
	if (line == 0)
		Line = 0;
	else
		Line = new FeatureStub_c(idf, *line);
}

RadialOperation_c::~RadialOperation_c()
{
	delete Direction;
	delete Length;
	delete To;
	delete Line;
}

LPCTSTR RadialOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "RadialOperation";
	return typeName;
}

void RadialOperation_c::WriteData(EditSerializer& s) const
{
    Operation_c::WriteData(s);

	s.WritePersistent(DataField_Direction, *Direction);
    s.WritePersistent(DataField_Length, *Length);
    s.WritePersistent(DataField_To, *To);

    if (Line != 0)
        s.WritePersistent(DataField_Line, *Line);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

SetTopologyOperation_c::SetTopologyOperation_c(IdFactory& idf, const CTime& when, const CeSetTopology& op)
	: Operation_c(idf, when)
{
	Line = op.GetpArc();
}

LPCTSTR SetTopologyOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "SetTopologyOperation";
	return typeName;
}

void SetTopologyOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);
    s.WriteFeatureRef(DataField_Line, Line);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

SimpleLineSubdivisionOperation_c::SimpleLineSubdivisionOperation_c(IdFactory& idf, const CTime& when, const CePointOnLine& op)
	: Operation_c(idf, when)
{
	Line = op.GetpArc();
	Distance = new Distance_c(*(op.GetpDistance()));

	if (Distance->ObservedDistance < 0.0)
	{
		IsFromEnd = TRUE;
		Distance->ObservedDistance = -Distance->ObservedDistance;
	}
	else
	{
		IsFromEnd = FALSE;
	}

	NewPoint = new FeatureStub_c(idf, *(op.GetpNewPoint()));
	NewLine1 = idf.GetNextId(op.GetpNewArc1());
	NewLine2 = idf.GetNextId(op.GetpNewArc2());
}

SimpleLineSubdivisionOperation_c::~SimpleLineSubdivisionOperation_c()
{
	delete Distance;
	delete NewPoint;
}

LPCTSTR SimpleLineSubdivisionOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "SimpleLineSubdivisionOperation";
	return typeName;
}

void SimpleLineSubdivisionOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);
    s.WriteFeatureRef(DataField_Line, Line);
    s.WritePersistent(DataField_Distance, *Distance);
    s.WriteBool(DataField_EntryFromEnd, IsFromEnd);
    s.WritePersistent(DataField_NewPoint, *NewPoint);
    s.WriteInternalId(DataField_NewLine1, NewLine1);
    s.WriteInternalId(DataField_NewLine2, NewLine2);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

TextRotationOperation_c::TextRotationOperation_c(IdFactory& idf, const CTime& when, const CeSetLabelRotation& op)
	: Operation_c(idf, when)
{
	Rotation = op.GetRotation();
}

LPCTSTR TextRotationOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "TextRotationOperation";
	return typeName;
}

void TextRotationOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);
	s.WriteRadians(DataField_Value, Rotation);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

TrimLineOperation_c::TrimLineOperation_c(IdFactory& idf, const CTime& when, const CeArcTrim& op)
	: Operation_c(idf, when)
{
	LoadIdArray(idf, op.GetArcs(), Lines);
	LoadIdArray(idf, op.GetPoints(), Points);	
}

void TrimLineOperation_c::LoadIdArray(IdFactory& idf, CeObjectList* features, CUIntArray& ids)
{
	CeListIter loop(features, TRUE);
	void* pThing;

	for ( pThing = loop.GetHead();
		  pThing;
		  pThing = loop.GetNext() )
	{
		unsigned int iid = idf.FindId(pThing);
		assert(iid < this->Sequence);
		ids.Add(iid);
	}
}

LPCTSTR TrimLineOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "TrimLineOperation";
	return typeName;
}

void TrimLineOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);
    s.WriteSimpleArray(DataField_Lines, Lines);
    s.WriteSimpleArray(DataField_Points, Points);
}

//////////////////////////////////////////////////////////////////////////////////////////////////
