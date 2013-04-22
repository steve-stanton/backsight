#include "StdAfx.h"
#include "DataField.h"
#include "EditSerializer.h"
#include "Features.h"
#include "Changes.h"
#include <assert.h>

#ifdef _CEDIT
#include "CeFeature.h"
#include "CeFeatureId.h"
#include "CeArc.h"
#include "CeLabel.h"
#include "CePoint.h"
#include "CeLocation.h"
#include "CeCircle.h"
#include "CeLine.h"
#include "CeCurve.h"
#include "CeMultiSegment.h"
#include "CePosition.h"
#include "CeSection.h"
#include "CeSegment.h"
#include "CeFont.h"
#include "CeText.h"
#include "CeKeyText.h"
#include "CeMiscText.h"
#include "CeRowText.h"
#include "CeVertex.h"
#include "CeIdManager.h"
#include "CeIdHandle.h"
#include "CeIdGroup.H"
#include "CeListIter.h"
#include "CePointOnLine.h"
#include "CeRow.h"
#include "CeSchema.h"
#endif

void EditFeatures::Add(const CeFeature* pFeat)
{
#ifdef _CEDIT
	objectstore::touch(pFeat, false);
#endif
	const CePoint* pPoint = dynamic_cast<const CePoint*>(pFeat);
	if (pPoint)
	{
		Points.Add((void*)pPoint);
		return;
	}

	const CeArc* pArc = dynamic_cast<const CeArc*>(pFeat);
	if (pArc)
	{
		Lines.Add((void*)pArc);
		return;
	}

	const CeLabel* pLabel = dynamic_cast<const CeLabel*>(pFeat);
	if (pLabel)
	{
		Labels.Add((void*)pLabel);
		return;
	}

	assert(1==0);
}

#ifdef _CEDIT
#include "CeObjectList.h"
#endif

unsigned int EditFeatures::PutFeatures(CeObjectList& result) const
{
	unsigned int numFeat = 0;

	numFeat += AppendFeatures(result, Points);
	numFeat += AppendFeatures(result, Lines);
	numFeat += AppendFeatures(result, Labels);

	return numFeat;
}

unsigned int EditFeatures::AppendFeatures(CeObjectList& result, const CPtrArray& features) const
{
	for (int i=0; i<features.GetSize(); i++)
		result.Append((CeClass*)features.GetAt(i));
//		result.Append((CeFeature*)features.GetAt(i));

	return (unsigned int)features.GetSize();
}

//////////////////////////////////////////////////////////////////////////////////////////////////

FeatureId_c::FeatureId_c(unsigned int nativeRawId)
{
	NativeId = nativeRawId;
	ForeignId = 0;
}

FeatureId_c::FeatureId_c(LPCTSTR foreignId)
{
	NativeId = 0;
	ForeignId = new CString(foreignId);
}

FeatureId_c::~FeatureId_c()
{
	delete ForeignId;
}

LPCTSTR FeatureId_c::GetTypeName() const
{
	static LPCTSTR typeName = "FeatureId";
	return typeName;
}

void FeatureId_c::WriteData(EditSerializer& s) const
{
	if (NativeId > 0)
		s.WriteUInt32(DataField_Key, NativeId);
	else
		s.WriteString(DataField_ForeignKey, (LPCTSTR)(*ForeignId));
}

//////////////////////////////////////////////////////////////////////////////////////////////////

FeatureStub_c::FeatureStub_c(IdFactory& idf, const CeFeature& f)
{
	InternalId = idf.GetNextId((void*)&f);
	EntityId = idf.GetEntityId(f.GetpWhat());

	CeFeatureId* pFid = f.GetpId();
	if (pFid == 0)
		Id = 0;
	else
	{
		unsigned int rawId = Feature_c::GetRawId(f);

		if (rawId > 0)
			Id = new FeatureId_c(rawId);
		else
			Id = new FeatureId_c(f.FormatKey());
	}
}

FeatureStub_c::FeatureStub_c(IdFactory& idf, unsigned int entityId, void* cedObject)
{
	InternalId = idf.GetNextId(cedObject);
	EntityId = entityId;
	Id = 0;
}

FeatureStub_c::~FeatureStub_c()
{
	delete Id;
}

LPCTSTR FeatureStub_c::GetTypeName() const
{
	static LPCTSTR typeName = "FeatureStub";
	return typeName;
}

void FeatureStub_c::WriteData(EditSerializer& s) const
{
    s.WriteInternalId(DataField_Id, InternalId);
    s.WriteUInt32(DataField_Entity, EntityId);

	if (Id != 0)
		Id->WriteData(s);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

// static
Feature_c* Feature_c::CreateExportFeature(IdFactory& idf, const CeFeature& f)
{
#ifdef _CEDIT
	objectstore::touch(&f, false);
#endif
	const CePoint* point = dynamic_cast<const CePoint*>(&f);
	if (point != 0)
		return new PointFeature_c(idf, *point);

	const CeArc* line = dynamic_cast<const CeArc*>(&f);
	if (line != 0)
		return new LineFeature_c(idf, *line);

	const CeLabel* label = dynamic_cast<const CeLabel*>(&f);
	if (label != 0)
		return new TextFeature_c(idf, *label);

	assert(1==0);
	return 0;
}

// static
unsigned int Feature_c::GetRawId(const CeFeature& f)
{
	if (f.IsForeignId())
		return 0;

	const CeFeatureId* const fid = f.GetpId();
	if (fid == 0)
		return 0;

	if (!fid->GetKey().IsNumeric())
		return 0;

	CeIdManager* idMan = CeIdHandle::GetIdManager();
	CeEntity* ent = f.GetpEntity();
	CeIdGroup* group = idMan->GetpGroup(ent);

	unsigned int val;
	sscanf(fid->FormatKey(), "%d", &val);

	if (group->HasCheckDigit())
		return val/10;
	else
		return val;
}

// static
CeArc* Feature_c::GetFirstArc(CeObjectList& features)
{
	CeListIter loop(&features, TRUE);
	CeFeature* f;

	for ( f = (CeFeature*)loop.GetHead();
		  f;
		  f = (CeFeature*)loop.GetNext() )
	{
#ifdef _CEDIT
		objectstore::touch(f, false);
#endif
		CeArc* a = dynamic_cast<CeArc*>(f);
		if (a != 0)
			return a;
	}

	return 0;
}

// static
CePoint* Feature_c::GetFirstPoint(CeObjectList& features)
{
	CeListIter loop(&features);
	CeFeature* f;

	for ( f = (CeFeature*)loop.GetHead();
		  f;
		  f = (CeFeature*)loop.GetNext() )
	{
#ifdef _CEDIT
		objectstore::touch(f, false);
#endif
		CePoint* p = dynamic_cast<CePoint*>(f);
		if (p != 0)
			return p;
	}

	return 0;
}


Feature_c::Feature_c(IdFactory& idf, const CeFeature& f)
{
	Stub = new FeatureStub_c(idf, f);
}

Feature_c::Feature_c(IdFactory& idf, unsigned int entityId, void* cedObject)
{
	Stub = new FeatureStub_c(idf, entityId, cedObject);
}

Feature_c::~Feature_c()
{
	delete Stub;
}

LPCTSTR Feature_c::GetTypeName() const
{
	static LPCTSTR typeName = "Feature";
	return typeName;
}

void Feature_c::WriteData(EditSerializer& s) const
{
	Stub->WriteData(s);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

PointGeometry_c::PointGeometry_c(void)
{
	X = 0;
	Y = 0;
}

PointGeometry_c::PointGeometry_c(const CeLocation& p)
{
	Init(p.GetEasting(), p.GetNorthing());
}

PointGeometry_c::PointGeometry_c(const CeVertex& p)
{
	Init(p.GetEasting(), p.GetNorthing());
}

PointGeometry_c::PointGeometry_c(const CePosition& p)
{
	Init(p.GetEasting(), p.GetNorthing());
}

PointGeometry_c::PointGeometry_c(double x, double y)
{
	Init(x, y);
}

void PointGeometry_c::Init(double x, double y)
{
	X = (__int64)(x * 1000000.0);
	Y = (__int64)(y * 1000000.0);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

#include "CedExporter.h"

PointFeature_c::PointFeature_c(IdFactory& idf, const CePoint& p)
	: Feature_c(idf, p)
{
	const CeLocation* loc = p.GetpVertex();
	Geom = new PointGeometry_c(*loc);

	// Additionally index the location for the point AND any coincident locations (that haven't
	// been recorded already). This tackles a problem in obtaining internal IDs for line terminals
	// (see comments in LineFeature_c constructor).
	//IndexAllLocations(idf, loc, this->Stub->InternalId);
}

PointFeature_c::PointFeature_c(IdFactory& idf, unsigned int entityId, const CeLocation& loc)
	: Feature_c(idf, entityId, (void*)&loc)
{
	Geom = new PointGeometry_c(loc);
	IndexAllLocations(idf, &loc, this->Stub->InternalId);
}

// static
void PointFeature_c::IndexAllLocations(IdFactory& idf, const CeLocation* loc, unsigned int id)
{
	CPtrArray locs;
	CedExporter::GetAllCoincidentLocations(loc, locs);

	for (int i=0; i<locs.GetSize(); i++)
	{
		void* pLoc = locs.GetAt(i);
		if (idf.FindId(pLoc) == 0)
			idf.AddIndexEntry(pLoc, id);
	}
}

PointFeature_c::~PointFeature_c()
{
	delete Geom;
}

LPCTSTR PointFeature_c::GetTypeName() const
{
	static LPCTSTR typeName = "PointFeature";
	return typeName;
}

void PointFeature_c::WriteData(EditSerializer& s) const
{
	Feature_c::WriteData(s);
	s.WritePointGeometry(DataField_X, DataField_Y, *Geom);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

MultiSegmentGeometry_c::MultiSegmentGeometry_c(const CeMultiSegment& ms)
{
	for (int i=0; i<(int)ms.GetNumVertex(); i++)
	{
		const CeLocation* const loc = ms[i];
		CString xy;
		if (i == 0)
			xy.Format("%lf %lf", loc->GetEasting(), loc->GetNorthing());
		else
			xy.Format(",%lf %lf", loc->GetEasting(), loc->GetNorthing());

		LineString += xy;
	}

	/*
	unsigned int np = ms.GetNumVertex();
	__int64* x = new __int64[np];
	__int64* y = new __int64[np];

	unsigned int i;
	for (i=0; i<np; i++)
	{
		const CeLocation* const loc = ms[i];
		x[i] = (__int64)(loc->GetEasting() * 1000000.0);
		y[i] = (__int64)(loc->GetNorthing() * 1000000.0);
	}

    // Get the number of common high-order bytes for X and Y
	int nx = CountCommonHighOrderBytes(x, np);
	int ny = CountCommonHighOrderBytes(y, np);

	// Figure out required array size (in bytes)
    int sizeHeader = 2 + nx + ny;
    int sizeDataX = np * (8-nx); 
    int sizeDataY = np * (8-ny);

	Length = sizeHeader + sizeDataX + sizeDataY;
	Data = new __int8[Length];

    // Define the header
    Data[0] = (byte)nx;
    Data[1] = (byte)ny;

	// Copy over common high-order bytes for X
	int from;
    int to=2;
	__int8 data[8];
	FillByteArray(x[0], data);
    for (from=0; from<nx; from++, to++)
		Data[to] = data[from];

	// Copy over common high-order bytes for Y
	FillByteArray(y[0], data);
	for (from=0; from<ny; from++, to++)
		Data[to] = data[from];

	// Copy over the low-order bytes for each (X,Y)

    //to is currently = sizeHeader;
    for (i=0; i<np; i++)
    {
		int j;
        FillByteArray(x[i], data);
        for (j=nx; j<8; j++)
        {
            Data[to] = data[j];
            to++;
        }

        FillByteArray(y[i], data);
        for (j=ny; j<8; j++)
        {
            Data[to] = data[j];
            to++;
        }
    }

	delete[] x;
	delete[] y;
	*/
}

MultiSegmentGeometry_c::~MultiSegmentGeometry_c()
{
	//delete[] Data;
}

// Counts the number of common high order bytes in an int64 array
int MultiSegmentGeometry_c::CountCommonHighOrderBytes(__int64* data, int nData)
{
	int nCommon = 8;
	__int8 common[8];
	__int8 current[8];

	FillByteArray(data[0], common);
	
    for (int i=1; i<nData && nCommon>0; i++)
    {
		FillByteArray(data[i], current);

		for (int j=0; j<nCommon; j++)
        {
            if (common[j] != current[j])
            {
                nCommon = j;
                break;
            }
        }
    }

	return nCommon;
}

// Copies bytes from an __int64 to a byte array (starting with the high order bytes)
void MultiSegmentGeometry_c::FillByteArray(__int64 value, __int8* bytes)
{
	__int8* vs = (__int8*)&value;

	for (int i=0; i<8; i++)
		bytes[7-i] = vs[i];
}

// Fills an array with the high-order bytes from an __int64 (array element 0 is the highest order byte)
void MultiSegmentGeometry_c::GetHighBytes(__int64 value, __int8* bytes, int nByte)
{
	__int8 all[8];
	FillByteArray(value, all);

	for (int i=0; i<nByte; i++)
		bytes[i] = all[i];
}

LPCTSTR MultiSegmentGeometry_c::GetTypeName() const
{
	static LPCTSTR typeName = "MultiSegmentGeometry";
	return typeName;
}

void MultiSegmentGeometry_c::WriteData(EditSerializer& s) const
{
	//s.WriteByteArray(DataField_Data, Data, Length);
	s.WriteString(DataField_LineString, (LPCTSTR)LineString);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

LPCTSTR ArcGeometry_c::GetTypeName() const
{
	static LPCTSTR typeName = "ArcGeometry";
	return typeName;
}

void ArcGeometry_c::WriteData(EditSerializer& s) const
{
    s.WriteBool(DataField_Clockwise, IsClockwise);

    if (CenterPoint != 0)
        s.WriteFeatureRef(DataField_Center, CenterPoint);
    else
        s.WriteFeatureRef(DataField_FirstArc, FirstArc);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

SectionGeometry_c::SectionGeometry_c(void* baseLine)
{
	Base = baseLine;
}

LPCTSTR SectionGeometry_c::GetTypeName() const
{
	static LPCTSTR typeName = "SectionGeometry";
	return typeName;
}

void SectionGeometry_c::WriteData(EditSerializer& s) const
{
	s.WriteFeatureRef(DataField_Base, Base);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

LineFeature_c::LineFeature_c(IdFactory& idf, const CeArc& line)
	: Feature_c(idf, line)
{
	// The From and To fields are held as Backsight internal IDs rather than pointers to
	// CEdit objects. This helps to re-inforce the idea that the terminal points must
	// exist BEFORE the line can be created (they must NEVER be objects created by a
	// subsequent editing operation).
	
	// The lookup is done using the CeLocation pointers (not CePoint) because CEdit would let
	// you have lines without full-fledged terminal points. During export to Backsight, extra
	// points may need to be fabricated up front. However, rather than creating extra CePoint
	// objects (which usually relate to persistent data in CED files), the fabricated points
	// are instances of PointFeature_c that are added to an extra import operation, which appears
	// at the very start of the export file. The fact that these fabricated points exist is
	// made known by indexing their ID via the underlying CeLocation.

	// Meanwhile, when PointFeature_c objects are created for real CePoint features, I
	// cross-reference BOTH the CePoint and the CeLocation to the assigned Backsight ID.

	// This is what makes it possible to do a lookup by CeLocation at this stage.

	From = idf.FindId(line.GetpStart());
	To = idf.FindId(line.GetpEnd());

	assert(From > 0);
	assert(To > 0);

	IsTopological = line.IsTopological();

	// Start by assuming that we've got a CeSegment (which doesn't need any geometry object for serialization)
	Geom = 0;

	const CeLine* const geom = line.GetpLine();
#ifdef _CEDIT
	objectstore::touch(geom, false);
#endif
	const CeMultiSegment* mseg = dynamic_cast<const CeMultiSegment*>(geom);
	if (mseg != 0)
		Geom = new MultiSegmentGeometry_c(*mseg);

	const CeCurve* arc = dynamic_cast<const CeCurve*>(geom);
	if (arc != 0)
	{
		// Locate the first line attached to the circle
		const CeCircle* const circle = arc->GetpCircle();
		CeArc* pFirst = GetFirstArc(*circle);
		assert(pFirst != 0);

		ArcGeometry_c* arcGeom = new ArcGeometry_c(arc->IsClockwise());
		if (pFirst == &line)
			arcGeom->CenterPoint = circle->GetpCentre(0, FALSE);
		else
			arcGeom->FirstArc = pFirst;

		Geom = arcGeom;
	}

	const CeSection* section = dynamic_cast<const CeSection*>(geom);
	if (section != 0)
	{
		// Only deal with sections produced via the two types of line subdivision edits (we should
		// not be attempting to serialize sections produced via line intersections).

		const CeArc* baseLine = 0;
		CeOperation* pop = line.GetpCreator();
#ifdef _CEDIT
		objectstore::touch(pop, false);
#endif
		const CeArcSubdivision* lineSub = dynamic_cast<const CeArcSubdivision*>(pop);
		if (lineSub != 0)
			baseLine = lineSub->GetpParent();

		const CePointOnLine* ptOnLine = dynamic_cast<const CePointOnLine*>(pop);
		if (ptOnLine != 0)
			baseLine = ptOnLine->GetpArc();

		assert(baseLine != 0);
		Geom = new SectionGeometry_c((void *)baseLine);
	}

	// That should leave just CeSegment
	if (Geom == 0)
	{
		const CeSegment* seg = dynamic_cast<const CeSegment*>(geom);
		assert(seg != 0);
	}
}

CeArc* LineFeature_c::GetFirstArc(const CeCircle& circle) const
{
	const CeClass* const pObjects = circle.GetpObjects();
	CeListIter loop(pObjects, TRUE);
	CeClass* pThing;
	CeArc* result = 0;

	for ( pThing=(CeClass*)loop.GetHead(); pThing; pThing=(CeClass*)loop.GetNext() )
	{
#ifdef _CEDIT
		objectstore::touch(pThing, false);
#endif
		CeCurve* pCurve = dynamic_cast<CeCurve*>(pThing);
		if (pCurve != 0)
		{
			CeArc* pArc = GetFirstArc(*pCurve);
			if (result == 0 || result->GetpCreator()->GetSequence() < result->GetpCreator()->GetSequence())
				result = pArc;
		}
	}

	return result;
}

CeArc* LineFeature_c::GetFirstArc(const CeCurve& curve) const
{
	const CeClass* const pObjects = curve.GetpObjects();
	CeListIter loop(pObjects, TRUE);
	CeClass* pThing;
	CeArc* result = 0;

	for ( pThing=(CeClass*)loop.GetHead(); pThing; pThing=(CeClass*)loop.GetNext() )
	{
#ifdef _CEDIT
		objectstore::touch(pThing, false);
#endif
		CeArc* pArc = dynamic_cast<CeArc*>(pThing);
		if (pArc != 0)
		{
			if (result == 0 || pArc->GetpCreator()->GetSequence() < result->GetpCreator()->GetSequence())
				result = pArc;
		}
	}

	return result;
}

LineFeature_c::~LineFeature_c()
{
	delete Geom;
}

LPCTSTR LineFeature_c::GetTypeName() const
{
	static LPCTSTR typeName = "LineFeature";
	return typeName;
}

void LineFeature_c::WriteData(EditSerializer& s) const
{
	Feature_c::WriteData(s);

	s.WriteInternalId(DataField_From, From);
    s.WriteInternalId(DataField_To, To);
    s.WriteBool(DataField_Topological, IsTopological);

    if (Geom != 0)
        s.WritePersistent(DataField_Type, *Geom);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

TextGeometry_c::TextGeometry_c(IdFactory& idf, const CeText& text)
{
	CeFont* font = text.GetpFont();
	if (font == 0)
		Font = 0;
	else
	{
		CString ft;
		font->GetFontTitle(ft);
		Font = idf.GetFontId((LPCTSTR)ft);
	}

	Position.X = (__int64)(text.GetEasting() * 1000000.0);
	Position.Y = (__int64)(text.GetNorthing() * 1000000.0);
	CString str;
	text.GetText(str);
	Width = text.GetWidth() * str.GetLength(); // may need a bit more than this
	Height = text.GetHeight();
	Rotation = text.GetRotation();
}

TextGeometry_c::~TextGeometry_c()
{
}

LPCTSTR TextGeometry_c::GetTypeName() const
{
	static LPCTSTR typeName = "TextGeometry";
	return typeName;
}

void TextGeometry_c::WriteData(EditSerializer& s) const
{
    if (Font != 0)
        s.WriteInt32(DataField_Font, Font);

    s.WritePointGeometry(DataField_X, DataField_Y, Position);
    s.WriteDouble(DataField_Width, Width);
    s.WriteDouble(DataField_Height, Height);
    s.WriteRadians(DataField_Rotation, Rotation);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

KeyTextGeometry_c::KeyTextGeometry_c(IdFactory& idf, const CeKeyText& text)
	: TextGeometry_c(idf, text)
{
	// Nothing to do
}

LPCTSTR KeyTextGeometry_c::GetTypeName() const
{
	static LPCTSTR typeName = "KeyTextGeometry";
	return typeName;
}

//////////////////////////////////////////////////////////////////////////////////////////////////

RowTextGeometry_c::RowTextGeometry_c(IdFactory& idf, const CeRowText& text)
	: TextGeometry_c(idf, text)
{
	TableId = idf.GetTableId(text.GetRow()->GetpSchema()->GetName());
	TemplateId = idf.GetTemplateId(text.GetTemplate()->GetName());
}

LPCTSTR RowTextGeometry_c::GetTypeName() const
{
	static LPCTSTR typeName = "RowTextContent";
	return typeName;
}

void RowTextGeometry_c::WriteData(EditSerializer& s) const
{
	TextGeometry_c::WriteData(s);
    s.WriteInt32(DataField_Table, TableId);
    s.WriteInt32(DataField_Template, TemplateId);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

MiscTextGeometry_c::MiscTextGeometry_c(IdFactory& idf, const CeMiscText& text)
	: TextGeometry_c(idf, text)
{
	text.GetText(Text);
}

LPCTSTR MiscTextGeometry_c::GetTypeName() const
{
	static LPCTSTR typeName = "MiscTextGeometry";
	return typeName;
}

void MiscTextGeometry_c::WriteData(EditSerializer& s) const
{
	TextGeometry_c::WriteData(s);
	s.WriteString(DataField_Text, (LPCTSTR)Text);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

TextFeature_c::TextFeature_c(IdFactory& idf, const CeLabel& label)
	: Feature_c(idf, label)
{
	CeText* text = label.GetpText();
#ifdef _CEDIT
	objectstore::touch(text, false);
#endif

	CeKeyText* keyText = dynamic_cast<CeKeyText*>(text);
	if (keyText != 0)
		Geom = new KeyTextGeometry_c(idf, *keyText);

	CeMiscText* miscText = dynamic_cast<CeMiscText*>(text);
	if (miscText != 0)
		Geom = new MiscTextGeometry_c(idf, *miscText);

	CeRowText* rowText = dynamic_cast<CeRowText*>(text);
	if (rowText != 0)
		Geom = new RowTextGeometry_c(idf, *rowText);

	assert(Geom != 0);

	IsTopological = label.IsTopological();

	if (IsTopological)
	{
		CeVertex posn;
		label.GetPolPosition(posn);
		PolygonPosition = new PointGeometry_c(posn);
	}
	else
	{
		PolygonPosition = 0;
	}
}

TextFeature_c::~TextFeature_c()
{
	delete Geom;
	delete PolygonPosition;
}

LPCTSTR TextFeature_c::GetTypeName() const
{
	static LPCTSTR typeName = "TextFeature";
	return typeName;
}

void TextFeature_c::WriteData(EditSerializer& s) const
{
	Feature_c::WriteData(s);

    s.WriteBool(DataField_Topological, IsTopological);

	if (PolygonPosition != 0)
		s.WritePointGeometry(DataField_PolygonX, DataField_PolygonY, *PolygonPosition);

    s.WritePersistent(DataField_Type, *Geom);
}

//////////////////////////////////////////////////////////////////////////////////////////////////
