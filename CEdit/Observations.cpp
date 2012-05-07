#include "StdAfx.h"
#include "DataField.h"
#include "EditSerializer.h"
#include "Observations.h"
#include <assert.h>

#ifdef _CEDIT
#include "CeDistance.h"
#include "CeDistanceUnit.h"
#include "CeOffsetPoint.h"
#include "CeOffsetDistance.h"
#include "CeDirection.h"
#include "CeAngle.h"
#include "CeDeflection.h"
#include "CeBearing.h"
#include "CeParallel.h"
#else
#include "CEditStubs.h"
#endif

//////////////////////////////////////////////////////////////////////////////////////////////////

// static
Observation_c* Observation_c::CreateExportLength(const CeObservation* o)
{
	if (o == 0)
		return 0;

#ifdef _CEDIT
	objectstore::touch(o, false);
#endif

	const CeDistance* d = dynamic_cast<const CeDistance*>(o);
	if (d != 0)
		return new Distance_c(*d);

	const CeOffsetPoint* p = dynamic_cast<const CeOffsetPoint*>(o);
	if (p != 0)
		return new OffsetPoint_c(*p);

	assert(1==0);
	return 0;
}

//////////////////////////////////////////////////////////////////////////////////////////////////

Distance_c::Distance_c(const CeDistance& d)
{
	ObservedDistance = d.GetDistance();
	UnitType = GetUnitType(d);

	if (d.IsFixed())
		IsFixed = 1;
	else
		IsFixed = 0;

	IsAnnotationFlipped = 0;	
}

unsigned __int8 Distance_c::GetUnitType(const CeDistance& d)
{
	CeDistanceUnit*	units = d.GetpUnit();
	UNIT unit = units->GetUnit();
	if (unit == UNIT_ENTRY)
		return 0;
	else
		return (unsigned __int8)unit;
}

LPCTSTR Distance_c::GetTypeName() const
{
	static LPCTSTR typeName = "Distance";
	return typeName;
}

void Distance_c::WriteData(EditSerializer& s) const
{
	s.WriteDouble(DataField_Value, ObservedDistance);
	s.WriteByte(DataField_Unit, UnitType);

	if (IsFixed)
		s.WriteBool(DataField_Fixed, true);

	if (IsAnnotationFlipped)
		s.WriteBool(DataField_Flipped, true);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

OffsetPoint_c::OffsetPoint_c(const CeOffsetPoint& ofp)
{
	Point = (void*)ofp.GetpPoint();
}

LPCTSTR OffsetPoint_c::GetTypeName() const
{
	static LPCTSTR typeName = "OffsetPoint";
	return typeName;
}

void OffsetPoint_c::WriteData(EditSerializer& s) const
{
	s.WriteFeatureRef(DataField_Point, this->Point);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

OffsetDistance_c::OffsetDistance_c(const CeOffsetDistance& ofd)
{
	Distance = new Distance_c(ofd.GetOffset());
	IsLeft = !ofd.IsRight();
}

OffsetDistance_c::~OffsetDistance_c()
{
	delete Distance;
}

LPCTSTR OffsetDistance_c::GetTypeName() const
{
	static LPCTSTR typeName = "OffsetDistance";
	return typeName;
}

void OffsetDistance_c::WriteData(EditSerializer& s) const
{
	s.WritePersistent(DataField_Offset, *Distance);
	s.WriteBool(DataField_Left, IsLeft);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

// static
Direction_c* Direction_c::CreateExportDirection(const CeDirection* d)
{
	if (d == 0)
		return 0;

#ifdef _CEDIT
	objectstore::touch(d, false);
#endif

	const CeAngle* a = dynamic_cast<const CeAngle*>(d);
	if (a != 0)
		return new AngleDirection_c(*a);

	const CeBearing* b = dynamic_cast<const CeBearing*>(d);
	if (b != 0)
		return new BearingDirection_c(*b);

	const CeParallel* p = dynamic_cast<const CeParallel*>(d);
	if (p != 0)
		return new ParallelDirection_c(*p);

	const CeDeflection* df = dynamic_cast<const CeDeflection*>(d);
	if (df != 0)
		return new DeflectionDirection_c(*df);

	assert(1==0);
	return 0;
}

Direction_c::Direction_c(const CeDirection& d)
{
	Offset = 0;
	CeOffset* offset = d.GetpOffset();
	if (offset != 0)
	{
		CeOffsetDistance* ofd = dynamic_cast<CeOffsetDistance*>(offset);
		if (ofd != 0)
			Offset = new OffsetDistance_c(*ofd);

		CeOffsetPoint* ofp = dynamic_cast<CeOffsetPoint*>(offset);
		if (ofp != 0)
			Offset = new OffsetPoint_c(*ofp);

		assert(Offset != 0);
	}
}

Direction_c::~Direction_c()
{
	delete Offset;
}

LPCTSTR Direction_c::GetTypeName() const
{
	static LPCTSTR typeName = "Direction";
	return typeName;
}

void Direction_c::WriteData(EditSerializer& s) const
{
	if (Offset != 0)
        s.WritePersistent(DataField_Offset, *Offset);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

AngleDirection_c::AngleDirection_c(const CeAngle& a)
	: Direction_c(a)
{
	Observation = a.GetObservation();
	Backsight = (void*)a.GetpBacksight();
	From = (void*)a.GetpFrom();
}

LPCTSTR AngleDirection_c::GetTypeName() const
{
	static LPCTSTR typeName = "AngleDirection";
	return typeName;
}

void AngleDirection_c::WriteData(EditSerializer& s) const
{
	Direction_c::WriteData(s);

    s.WriteFeatureRef(DataField_Backsight, Backsight);
    s.WriteFeatureRef(DataField_From, From);
    s.WriteRadians(DataField_Value, Observation);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

DeflectionDirection_c::DeflectionDirection_c(const CeDeflection& d)
	: AngleDirection_c(d)
{
}

LPCTSTR DeflectionDirection_c::GetTypeName() const
{
	static LPCTSTR typeName = "DeflectionDirection";
	return typeName;
}

//////////////////////////////////////////////////////////////////////////////////////////////////

BearingDirection_c::BearingDirection_c(const CeBearing& b)
	: Direction_c(b)
{
	Observation = b.GetObservation();
	From = (void*)b.GetpFrom();
}

LPCTSTR BearingDirection_c::GetTypeName() const
{
	static LPCTSTR typeName = "BearingDirection";
	return typeName;
}

void BearingDirection_c::WriteData(EditSerializer& s) const
{
	Direction_c::WriteData(s);

    s.WriteFeatureRef(DataField_From, From);
    s.WriteRadians(DataField_Value, Observation);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

ParallelDirection_c::ParallelDirection_c(const CeParallel& p)
	: Direction_c(p)
{
	From = (void*)p.GetpFrom();
	Par1 = p.GetpStart();
	Par2 = p.GetpEnd();
}

LPCTSTR ParallelDirection_c::GetTypeName() const
{
	static LPCTSTR typeName = "ParallelDirection";
	return typeName;
}

void ParallelDirection_c::WriteData(EditSerializer& s) const
{
	Direction_c::WriteData(s);

    s.WriteFeatureRef(DataField_From, From);
    s.WriteFeatureRef(DataField_Start, Par1);
    s.WriteFeatureRef(DataField_End, Par2);
}

//////////////////////////////////////////////////////////////////////////////////////////////////
