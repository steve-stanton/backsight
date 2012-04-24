#pragma once

class EditSerializer;

// Abstract base class for everything
class Persistent_c
{
public:
	virtual LPCTSTR GetTypeName() const = 0;
	virtual void WriteData(EditSerializer& s) const = 0;
};

//////////////////////////////////////////////////////////////////////////////////////////////////

class IdMapping_c : Persistent_c
{
public:
	unsigned int InternalId;
	unsigned int RawId;

	IdMapping_c(unsigned int internalId, unsigned int rawId);

	virtual LPCTSTR GetTypeName() const;
	virtual void WriteData(EditSerializer& s) const;
};


