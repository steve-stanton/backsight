#pragma once

#include "DataField.h"

class TextEditWriter;
class Persistent_c;
class PointGeometry_c;
class IdFactory;

class EditSerializer
{
public:
	EditSerializer(const IdFactory& idFactory, TextEditWriter& writer);
	~EditSerializer(void) {}

	void WriteByte(DataField field, byte value);
	void WriteInt32(DataField field, int value);
	void WriteUInt32(DataField field, unsigned int value);
	void WriteInt64(DataField field, __int64 value);
	void WriteDouble(DataField field, double value);
	void WriteSingle(DataField field, float value);
	void WriteBool(DataField field, bool value);
	void WriteString(DataField field, LPCTSTR value);
	void WriteDateTime(DataField field, const CTime& when);
	void WriteInternalId(DataField field, unsigned int id);
	void WriteFeatureRef(DataField field, void* feature);
	void WriteRadians(DataField field, double value);
	void WritePointGeometry(DataField xField, DataField yField, const PointGeometry_c& value);
	void WritePersistent(DataField field, const Persistent_c& p);
	void WritePersistentArray(DataField field, const CPtrArray& a);
	void WriteSimpleArray(DataField field, const CUIntArray& a);
	void WriteByteArray(DataField field, __int8* data, unsigned int length);

private:
	void WriteBegin(DataField field, LPCTSTR exportedTypeName);
	void WriteEnd();
	void RadiansAsShortString(char* buf, double value);
	void WritePersistent(LPCTSTR field, const Persistent_c& p);


private:
	TextEditWriter& m_Writer;
	const IdFactory& m_IdFactory;
};

