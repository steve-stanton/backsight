#include "StdAfx.h"
#include <assert.h>
#include "DataField.h"
#include "TextEditWriter.h"
#include "Features.h"
#include "Changes.h"
#include "EditSerializer.h"

EditSerializer::EditSerializer(TextEditWriter& writer)
	: m_Writer(writer)
{
	m_MaxId = 0;

	// Load translations from a specific location
	LoadMappings("C:\\Backsight\\CEdit\\Entities.txt", m_EntityMap);
	LoadMappings("C:\\Backsight\\CEdit\\Templates.txt", m_TemplateMap);
}

void EditSerializer::LoadMappings(LPCTSTR fileName, CMapStringToPtr& index)
{
	FILE* fp = fopen(fileName,"r");
	char buf[1024];

	while ( fgets(buf,sizeof(buf),fp) )
	{
		// Grab the buffer into a CString, trim off any leading and trailing whitespace.
		CString str(buf);
		str.TrimLeft();
		str.TrimRight();

		// Skip blank records.
		int nc = str.GetLength();
		if ( nc==0 ) continue;

		int eqpos = str.Find('=');
		if (eqpos > 0)
		{
			CString ids = str.Left(eqpos);
			ids.TrimLeft();
			ids.TrimRight();

			unsigned int id;
			sscanf((LPCTSTR)ids, "%d", &id);
			CString entName(str.Mid(eqpos+1));
			entName.TrimLeft();
			entName.TrimRight();

			// I THINK that SetAt creates a new copy
			index.SetAt(entName, (void*)id);
		}
	}

	fclose(fp);
}

int EditSerializer::LookupId(CMapStringToPtr& index, LPCTSTR name)
{
	void* result;
	if (index.Lookup(name, result))
		return (int)result;
	else
		return 0;
}

void EditSerializer::WritePersistentArray(DataField field, const CPtrArray& a)
{
	WriteBegin(field, 0);
	WriteUInt32(DataField_Length, a.GetSize());

	char itemName[16];

    for (int i=0; i<a.GetSize(); i++)
    {
		sprintf(itemName, "[%d]", i);
		const Persistent_c* const p = (const Persistent_c* const)a.GetAt(i);
        WritePersistent(itemName, *p);
    }

    WriteEnd();
}

/*
/// <summary>
/// Writes an array of <see cref="FeatureStub"/>, creating them from the supplied
/// feature array.
/// </summary>
/// <param name="field">The tag that identifies the item.</param>
/// <param name="features">The features to convert into stubs before writing them out.</param>
void EditSerializer::WriteFeatureStubArray(DataField field, Feature[] features)
{
    var stubs = new FeatureStub[features.Length];

    for (int i = 0; i < stubs.Length; i++)
        stubs[i] = new FeatureStub(features[i]);

    WritePersistentArray<FeatureStub>(field, stubs);
}

/// <summary>
/// Writes an array of simple types to a storage medium.
/// </summary>
/// <typeparam name="T">The type of objects within the array (as it is known to the instance
/// that refers to it)</typeparam>
/// <param name="field">The tag that identifies the array.</param>
/// <param name="array">The array to write (may be null)</param>
void EditSerializer::WriteSimpleArray<T>(DataField field, T[] array) where T : IConvertible
{
    string name = DataFields[field];

    if (array == null)
    {
        m_Writer.WriteString(name, "null");
    }
    else
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < array.Length; i++)
        {
            if (i > 0)
                sb.Append(";");

            sb.Append(array[i].ToString());
        }

        m_Writer.WriteString(name, sb.ToString());
    }
}
*/

void EditSerializer::WriteSimpleArray(DataField field, const CUIntArray& a)
{
	CString result;
	char buf[16];

	for (int i=0; i<a.GetSize(); i++)
	{
		unsigned int val = a.GetAt(i);
		if (i > 0)
			result += ";";

		sprintf(buf, "%d", val);
		result += buf;
	}

	WriteString(field, (LPCTSTR)result);
}

void EditSerializer::WriteByteArray(DataField field, __int8* data, unsigned int length)
{
	CString result;
	char buf[8];

	for (unsigned int i=0; i<length; i++)
	{
		__int8 val = data[i];
		if (i > 0)
			result += ";";

		sprintf(buf, "%d", val);
		result += buf;
	}

	WriteString(field, (LPCTSTR)result);
}

/// <summary>
/// Write a value in radians to a storage medium.
/// </summary>
/// <param name="field">The tag that identifies the item.</param>
/// <param name="value">The radian value to write</param>
void EditSerializer::WriteRadians(DataField field, double value)
{
	char buf[16];
	RadiansAsShortString(buf, value);
    m_Writer.WriteString(DataFields[field], buf);
}

#include <math.h>

void EditSerializer::RadiansAsShortString(char* res, double value)
{
    // Convert to decimal degrees (possibly signed).
    const double RADTODEG = 360.0 / 3.14159265358979323846;
	double sdeg = value * RADTODEG;

    // Get the degrees, minutes, and seconds, all unsigned.
	double deg, mins, secs, rem;
    rem = modf(abs(sdeg), &deg);
    rem = modf(rem*60.0, &mins);
    secs = rem*60.0;

    // Make sure we don't have max-values (i.e. 60's)
	unsigned int ideg  = (unsigned int)deg;
	unsigned int imins = (unsigned int)mins;

	if (abs(secs-60.0) < 0.001) // 3 decimals formatted below
    {
		secs -= 60.0;
		secs = 0.0;
		imins++;
	}

	if ( imins>=60 )
    {
		imins = 0;
		ideg++;
	}

	// Create the return string, making sure that the sign is there.
	if ( sdeg<0.0 )
		sprintf(res, "-%d-%d", ideg, imins);
	else
		sprintf(res, "&d-%d", ideg, imins);

	// Append seconds if they'll show.
    if (secs>=0.001)
	{
		char extra[10];
		sprintf(extra, "%-.3f", secs);
		strcat(res, extra);
	}
}

/// <summary>
/// Writes a 2D position to a storage medium.
/// </summary>
/// <param name="xField">The tag that identifies the easting value.</param>
/// <param name="yField">The tag that identifies the northing value.</param>
/// <param name="value">The position to write</param>
void EditSerializer::WritePointGeometry(DataField xField, DataField yField, const PointGeometry_c& value)
{
    m_Writer.WriteInt64(DataFields[xField], value.X);
    m_Writer.WriteInt64(DataFields[yField], value.Y);
}

/// <summary>
/// Writes an unsigned byte to a storage medium.
/// </summary>
/// <param name="field">The tag that identifies the item.</param>
/// <param name="value">The unsigned byte to write.</param>
void EditSerializer::WriteByte(DataField field, byte value)
{
    m_Writer.WriteByte(DataFields[field], value);
}

/// <summary>
/// Writes a four-byte signed integer to a storage medium.
/// </summary>
/// <param name="field">The tag that identifies the item.</param>
/// <param name="value">The four-byte signed integer to write.</param>
void EditSerializer::WriteInt32(DataField field, int value)
{
    m_Writer.WriteInt32(DataFields[field], value);
}

/// <summary>
/// Writes a four-byte unsigned integer to a storage medium.
/// </summary>
/// <param name="field">The tag that identifies the item.</param>
/// <param name="value">The four-byte unsigned integer to write.</param>
void EditSerializer::WriteUInt32(DataField field, unsigned int value)
{
    m_Writer.WriteUInt32(DataFields[field], value);
}

/// <summary>
/// Writes an eight-byte signed integer to a storage medium.
/// </summary>
/// <param name="field">The tag that identifies the item.</param>
/// <param name="value">The eight-byte signed integer to write.</param>
void EditSerializer::WriteInt64(DataField field, __int64 value)
{
    m_Writer.WriteInt64(DataFields[field], value);
}

/// <summary>
/// Writes an eight-byte floating-point value to a storage medium.
/// </summary>
/// <param name="field">The tag that identifies the item.</param>
/// <param name="value">The eight-byte floating-point value to write.</param>
void EditSerializer::WriteDouble(DataField field, double value)
{
    m_Writer.WriteDouble(DataFields[field], value);
}

/// <summary>
/// Writes an four-byte floating-point value to a storage medium.
/// </summary>
/// <param name="field">The tag that identifies the item.</param>
/// <param name="value">The four-byte floating-point value to write.</param>
void EditSerializer::WriteSingle(DataField field, float value)
{
    m_Writer.WriteSingle(DataFields[field], value);
}

/// <summary>
/// Writes a one-byte boolean value to a storage medium.
/// </summary>
/// <param name="field">The tag that identifies the item.</param>
/// <param name="value">The boolean value to write (0 or 1).</param>
void EditSerializer::WriteBool(DataField field, bool value)
{
    m_Writer.WriteBool(DataFields[field], value);
}

/// <summary>
/// Writes a string to a storage medium.
/// </summary>
/// <param name="field">The tag that identifies the item.</param>
/// <param name="value">The string to write (if a null is supplied, just the name tag will be written).</param>
void EditSerializer::WriteString(DataField field, LPCTSTR value)
{
    m_Writer.WriteString(DataFields[field], value);
}

/// <summary>
/// Writes a timestamp to a storage medium.
/// </summary>
/// <param name="field">The tag that identifies the item.</param>
/// <param name="when">The timestamp to write</param>
void EditSerializer::WriteDateTime(DataField field, const CTime& when)
{
    m_Writer.WriteDateTime(DataFields[field], when);
}

/// <summary>
/// Writes an internal ID to a storage medium.
/// </summary>
/// <param name="field">The tag that identifies the item.</param>
/// <param name="id">The internal ID to write</param>
void EditSerializer::WriteInternalId(DataField field, unsigned int id)
{
    m_Writer.WriteInternalId(DataFields[field], id);
}

unsigned int EditSerializer::GetInternalId()
{
	m_MaxId++;
	return m_MaxId;
}

unsigned int EditSerializer::GetInternalId(void* p)
{
	void* result;
	if (m_ObjectIds.Lookup(p, result))
		return (unsigned int)result;

	m_MaxId++;
	m_ObjectIds.SetAt(p, (void*)m_MaxId);
	return m_MaxId;
}

void EditSerializer::SetInternalId(void* p, unsigned int iid)
{
	assert(iid > m_MaxId);
	m_MaxId = iid;
	m_ObjectIds.SetAt(p, (void*)m_MaxId);
}

void EditSerializer::SetMaxInternalId(unsigned int maxId)
{
	assert(maxId >= m_MaxId);
	m_MaxId = maxId;
}

void EditSerializer::WriteFeatureRef(DataField field, void* feature)
{
	unsigned int iid = GetInternalId(feature);
	m_Writer.WriteInternalId(DataFields[field], iid);
}

void EditSerializer::WritePersistent(DataField field, const Persistent_c& p)
{
	WriteBegin(field, p.GetTypeName());
	p.WriteData(*this);
	WriteEnd();
}

// Private version for use with WritePersistentArray
void EditSerializer::WritePersistent(LPCTSTR field, const Persistent_c& p)
{
	m_Writer.WriteString(field, p.GetTypeName());
	m_Writer.WriteBeginObject();
	p.WriteData(*this);
	WriteEnd();
}

void EditSerializer::WriteBegin(DataField field, LPCTSTR exportedTypeName)
{
	m_Writer.WriteString(DataFields[field], exportedTypeName);
	m_Writer.WriteBeginObject();
}

void EditSerializer::WriteEnd()
{
	m_Writer.WriteEndObject();
}

int EditSerializer::GetEntityId(LPCTSTR entName)
{
	return LookupId(m_EntityMap, entName);
}

int EditSerializer::GetTemplateId(LPCTSTR templateName)
{
	return LookupId(m_TemplateMap, templateName);
}

int EditSerializer::GetFontId(LPCTSTR fontTitle)
{
	return 0;
}

#ifdef _CEDIT
#include "CeIdManager.h"
#include "CeIdHandle.h"
#include "CeIdGroup.h"
#include "CeIdRange.h"
else
#include "CEditStubs.h"
#endif

void EditSerializer::WriteIdAllocations(const CTime& when)
{
	CeIdManager* idMan = CeIdHandle::GetIdManager();
	IdAllocation_c a(0, when);

	unsigned int nGroup = idMan->GetNumGroup();
	for (unsigned int i=0; i<nGroup; i++)
	{
		const CeIdGroup* group = idMan->GetpGroup(i);
		a.GroupId = GetGroupId((LPCTSTR)(group->GetGroupName()));
		const CPtrList& ranges = group->GetIdRanges();

		POSITION pos = ranges.GetHeadPosition();
		while ( pos )
		{
			CeIdRange* range = (CeIdRange*)ranges.GetNext(pos);
			a.LowestId = (int)range->GetMin();
			a.HighestId = (int)range->GetMax();
			m_MaxId++;
			a.Sequence = m_MaxId;
			WritePersistent(DataField_Edit, a);
		}
	}
}

int EditSerializer::GetGroupId(LPCTSTR groupName)
{
	return 0;
}

int EditSerializer::GetTableId(LPCTSTR tableName)
{
	return 123;
}
