#include "StdAfx.h"
#include <assert.h>
#include "DataField.h"
#include "TextEditWriter.h"
#include "Features.h"
#include "Changes.h"
#include "EditSerializer.h"

EditSerializer::EditSerializer(const IdFactory& idFactory, TextEditWriter& writer)
	: m_IdFactory(idFactory), m_Writer(writer)
{
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
		sprintf(res, "%d-%d", ideg, imins);

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
	assert(id > 0);
    m_Writer.WriteInternalId(DataFields[field], id);
}

void EditSerializer::WriteFeatureRef(DataField field, void* feature)
{
	unsigned int iid = m_IdFactory.FindId(feature);
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
