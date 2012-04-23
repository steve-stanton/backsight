#pragma once

class EditSerializer;

#include "Persistent.h"
#include "Features.h"
#include "Changes.h"

class MovePolygonPositionOperation_c : public Operation_c
{
public:
    void* Label;
    PointGeometry_c* OldPosition;
    PointGeometry_c& NewPosition;

	MovePolygonPositionOperation_c(unsigned int sequence, CTime& when, void* label, PointGeometry_c* oldPosition, PointGeometry_c& newPosition)
		: Operation_c(sequence, when), Label(label), OldPosition(oldPosition), NewPosition(newPosition) {}

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};

//TODO - UpdateOperation




