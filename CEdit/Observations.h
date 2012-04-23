#pragma once

#include "Persistent.h"

#ifdef _CEDIT
class CeObservation;
class CeDistance;
class CeDistanceUnit;
class CeOffsetPoint;
class CeOffsetDistance;
class CeDirection;
class CeAngle;
class CeDeflection;
class CeBearing;
class CeParallel;
#else
#include "CEditStubs.h"
#endif

//////////////////////////////////////////////////////////////////////////////////////////////////

class Observation_c : public Persistent_c
{
public:

	static Observation_c* CreateExportLength(const CeObservation* o);
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class Distance_c : public Observation_c
{
public:
	double ObservedDistance;
	unsigned __int8 UnitType;
	bool IsFixed;
	bool IsAnnotationFlipped;

	Distance_c(const CeDistance& d);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;

private:
	unsigned __int8 GetUnitType(const CeDistance& d);
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class Offset_c : public Observation_c
{
public:
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class OffsetPoint_c : public Offset_c
{
public:
	void* Point;

	OffsetPoint_c(const CeOffsetPoint& ofp);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class OffsetDistance_c : public Offset_c
{
public:
	Distance_c* Distance;
	bool IsLeft;

	OffsetDistance_c(const CeOffsetDistance& ofd);

	virtual ~OffsetDistance_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class Direction_c : public Observation_c
{
public:

	static Direction_c* CreateExportDirection(const CeDirection* d);

	Offset_c* Offset;

	Direction_c(const CeDirection& d);

	virtual ~Direction_c();
	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class AngleDirection_c : public Direction_c
{
public:
	double Observation;
	void* Backsight;
	void* From;

	AngleDirection_c(const CeAngle& a);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class DeflectionDirection_c : public AngleDirection_c
{
public:
	DeflectionDirection_c(const CeDeflection& d);

	virtual LPCTSTR GetTypeName() const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class BearingDirection_c : public Direction_c
{
public:
	double Observation;
	void* From;

	BearingDirection_c(const CeBearing& b);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class ParallelDirection_c : public Direction_c
{
public:
	void* From;
	void* Par1;
	void* Par2;

	ParallelDirection_c(const CeParallel& p);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//////////////////////////////////////////////////////////////////////////////////////////////////
