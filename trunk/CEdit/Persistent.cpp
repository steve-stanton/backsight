#include "StdAfx.h"
#include "EditSerializer.h"
#include "Persistent.h"


//////////////////////////////////////////////////////////////////////////////////////////////////

IdMapping_c::IdMapping_c(unsigned int internalId, unsigned int rawId)
{
	InternalId = internalId;
	RawId = rawId;
}

LPCTSTR IdMapping_c::GetTypeName() const
{
	static LPCTSTR typeName = "IdMapping";
	return typeName;
}

void IdMapping_c::WriteData(EditSerializer& s) const
{
    s.WriteInternalId(DataField_Id, InternalId);
    s.WriteUInt32(DataField_Key, RawId);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

LPCTSTR LegFace_c::GetTypeName() const
{
	static LPCTSTR typeName = "LegFace";
	return typeName;
}

void LegFace_c::WriteData(EditSerializer& s) const
{
    s.WriteInternalId(DataField_Id, Id);
    s.WriteInternalId(DataField_PrimaryFaceId, PrimaryFaceId);
	s.WriteString(DataField_EntryString, EntryString);
}
