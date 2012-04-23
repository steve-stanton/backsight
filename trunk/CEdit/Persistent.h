#pragma once

class EditSerializer;

// Abstract base class for everything
class Persistent_c
{
public:
	virtual LPCTSTR GetTypeName() const = 0;
	virtual void WriteData(EditSerializer& s) const = 0;
};
