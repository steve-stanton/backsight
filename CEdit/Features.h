#pragma once

class EditSerializer;
class FeatureId_c;

#include "Persistent.h"

#ifdef _CEDIT
class CeFeature;
class CeFeatureId;
class CeArc;
class CeLabel;
class CePoint;
class CeLocation;
class CeCircle;
class CeLine;
class CeCurve;
class CeMultiSegment;
class CePosition;
class CeSection;
class CeSegment;
class CeText;
class CeKeyText;
class CeMiscText;
class CeRowText;
class CeVertex;
class CeObjectList;
#else
#include "CEditStubs.h"
#endif

//////////////////////////////////////////////////////////////////////////////////////////////////

class FeatureId_c : public Persistent_c
{
public:
	// One or the other (if NativeId==0, it's a foreign ID)
	unsigned int NativeId;
	CString* ForeignId;

	FeatureId_c(unsigned int nativeRawId);
	FeatureId_c(LPCTSTR foreignId);

	virtual ~FeatureId_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class FeatureStub_c : public Persistent_c
{
public:
	unsigned int InternalId;
	unsigned int EntityId;
	const FeatureId_c* Id;

	FeatureStub_c(EditSerializer& s, const CeFeature& f);

	virtual ~FeatureStub_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;

//private:
//	unsigned int GetRawId(const CeFeature& f);
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class Feature_c : public Persistent_c
{
public:
	FeatureStub_c* Stub;

	virtual ~Feature_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;

	static Feature_c* CreateExportFeature(EditSerializer& s, const CeFeature& f);
	static unsigned int GetRawId(const CeFeature& f);
	static CeArc* GetFirstArc(CeObjectList& features);
	static CePoint* GetFirstPoint(CeObjectList& features);


protected:
	Feature_c(EditSerializer& s, const CeFeature& f);
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class PointGeometry_c
{
public:
	__int64 X;
	__int64 Y;

	PointGeometry_c(void);
	PointGeometry_c(const CeLocation& loc);
	PointGeometry_c(const CeVertex& p);
	PointGeometry_c(const CePosition& p);
	PointGeometry_c(double x, double y);

private:

	void Init(double x, double y);
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class PointFeature_c : public Feature_c
{
public:
	PointGeometry_c* Geom;

	PointFeature_c(EditSerializer& s, const CePoint& p);

	virtual ~PointFeature_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};


//////////////////////////////////////////////////////////////////////////////////////////////////

class LineGeometry_c : public Persistent_c
{
public:
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class MultiSegmentGeometry_c : public LineGeometry_c
{
public:
	unsigned int Length;
	__int8* Data;

	MultiSegmentGeometry_c(const CeMultiSegment& mseg);

	virtual ~MultiSegmentGeometry_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;

private:
	int CountCommonHighOrderBytes(__int64* data, int nData);
	void FillByteArray(__int64 value, __int8* bytes);
	void GetHighBytes(__int64 value, __int8* bytes, int nByte);
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class ArcGeometry_c : public LineGeometry_c
{
public:
	bool IsClockwise;
	void* CenterPoint;
	void* FirstArc;

	// Specify either center point OR first arc
	ArcGeometry_c(bool isClockwise)
		: IsClockwise(isClockwise), CenterPoint(0), FirstArc(0) {}

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class SectionGeometry_c : public LineGeometry_c
{
public:
	void* Base;

	SectionGeometry_c(void* baseLine);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class LineFeature_c : public Feature_c
{
public:
	void* From;
	void* To;
	LineGeometry_c* Geom;
	bool IsTopological;

	LineFeature_c(EditSerializer& s, const CeArc& line);

	virtual ~LineFeature_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;

private:
	CeArc* GetFirstArc(const CeCircle& circle) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class TextGeometry_c : public Persistent_c
{
public:
	int Font;
	PointGeometry_c Position;
	double Width;
	double Height;
	double Rotation;

	TextGeometry_c(EditSerializer& s, const CeText& text);

	virtual ~TextGeometry_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class KeyTextGeometry_c : public TextGeometry_c
{
public:
	KeyTextGeometry_c(EditSerializer& s, const CeKeyText& text);

	virtual LPCTSTR GetTypeName() const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class RowTextGeometry_c : public TextGeometry_c
{
public:
	int TableId;
	int TemplateId;

	RowTextGeometry_c(EditSerializer& s, const CeRowText& text);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class MiscTextGeometry_c : public TextGeometry_c
{
public:
	CString Text;

	MiscTextGeometry_c(EditSerializer& s, const CeMiscText& text);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class TextFeature_c : public Feature_c
{
public:
	TextGeometry_c* Geom;
	bool IsTopological;
	PointGeometry_c* PolygonPosition;

	TextFeature_c(EditSerializer& s, const CeLabel& label);

	virtual ~TextFeature_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};
