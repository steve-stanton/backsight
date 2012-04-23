#pragma once

#include "DataField.h"

/*
class AngleDirection_c;
class BearingDirecton_c;
class Distance_c;
class OffsetDistance_c;
class OffsetPoint_c;

#ifdef _CEDIT
class CeFont;
#endif
*/

class TextEditWriter;
class Persistent_c;
class PointGeometry_c;

class EditSerializer
{
public:
	EditSerializer(TextEditWriter& writer);
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
	void WriteIdAllocations(const CTime& when);

	unsigned int GetInternalId(void* p);
	unsigned int GetInternalId();
	void SetInternalId(void* p, unsigned int iid);
	void SetMaxInternalId(unsigned int maxId);

	int GetEntityId(LPCTSTR entName);
	int GetFontId(LPCTSTR fontTitle);
	int GetTableId(LPCTSTR tableName);
	int GetTemplateId(LPCTSTR templateName);
	int GetGroupId(LPCTSTR groupName);

private:
	void WriteBegin(DataField field, LPCTSTR exportedTypeName);
	void WriteEnd();
	void RadiansAsShortString(char* buf, double value);
	void WritePersistent(LPCTSTR field, const Persistent_c& p);
	void LoadMappings(LPCTSTR fileName, CMapStringToPtr& index);
	int LookupId(CMapStringToPtr& index, LPCTSTR name);


private:
	TextEditWriter& m_Writer;
	CMapPtrToPtr m_ObjectIds;
	unsigned int m_MaxId;
	CMapStringToPtr m_EntityMap;
	CMapStringToPtr m_TemplateMap;
};

