#include "StdAfx.h"
#include "TextEditWriter.h"

/// <summary>
/// Writes an unsigned byte to a storage medium.
/// </summary>
/// <param name="name">A name tag for the item</param>
/// <param name="value">The unsigned byte to write.</param>
void TextEditWriter::WriteByte(LPCTSTR name, byte value)
{
	char buf[8];
	sprintf(buf, "%d", value);
    WriteValue(name, buf);
}

/// <summary>
/// Writes a four-byte signed integer to a storage medium.
/// </summary>
/// <param name="name">A name tag for the item</param>
/// <param name="value">The four-byte signed integer to write.</param>
void TextEditWriter::WriteInt32(LPCTSTR name, int value)
{
	char buf[16];
	sprintf(buf, "%d", value);
    WriteValue(name, buf);
}

/// <summary>
/// Writes a four-byte unsigned integer to a storage medium.
/// </summary>
/// <param name="name">A name tag for the item</param>
/// <param name="value">The four-byte unsigned integer to write.</param>
void TextEditWriter::WriteUInt32(LPCTSTR name, unsigned int value)
{
	char buf[16];
	sprintf(buf, "%u", value);
    WriteValue(name, buf);
}

/// <summary>
/// Writes an eight-byte signed integer to a storage medium.
/// </summary>
/// <param name="name">A name tag for the item</param>
/// <param name="value">The eight-byte signed integer to write.</param>
void TextEditWriter::WriteInt64(LPCTSTR name, __int64 value)
{
	char buf[32];
	sprintf(buf, "%I64d", value);
    WriteValue(name, buf);
}

/// <summary>
/// Writes an eight-byte floating-point value to a storage medium.
/// </summary>
/// <param name="name">A name tag for the item</param>
/// <param name="value">The eight-byte floating-point value to write.</param>
void TextEditWriter::WriteDouble(LPCTSTR name, double value)
{
	char buf[32];
	sprintf(buf, "%f", value);
    WriteValue(name, buf);
}

/// <summary>
/// Writes an four-byte floating-point value to a storage medium.
/// </summary>
/// <param name="name">A name tag for the item</param>
/// <param name="value">The four-byte floating-point value to write.</param>
void TextEditWriter::WriteSingle(LPCTSTR name, float value)
{
	char buf[32];
	sprintf(buf, "%f", value);
    WriteValue(name, buf);
}

/// <summary>
/// Writes a one-byte boolean value to a storage medium.
/// </summary>
/// <param name="name">A name tag for the item</param>
/// <param name="value">The boolean value to write (0 or 1).</param>
void TextEditWriter::WriteBool(LPCTSTR name, bool value)
{
	if (value)
		WriteValue(name, "1");
	else
		WriteValue(name, "0");
}

/// <summary>
/// Writes a string to a storage medium.
/// </summary>
/// <param name="name">A name tag for the item</param>
/// <param name="value">The string to write (if a null is supplied, just the name tag will be written).</param>
void TextEditWriter::WriteString(LPCTSTR name, LPCTSTR value)
{
    WriteValue(name, value);
}

/// <summary>
/// Writes a timestamp to a storage medium.
/// </summary>
/// <param name="name">A name tag for the item</param>
/// <param name="when">The timestamp to write</param>
void TextEditWriter::WriteDateTime(LPCTSTR name, const CTime& value)
{
	WriteValue(name, (LPCTSTR)value.Format("%Y-%m-%dT%H:%M:%S"));
}

/// <summary>
/// Writes an internal ID to a storage medium.
/// </summary>
/// <param name="name">A name tag for the item</param>
/// <param name="id">The internal ID to write</param>
void TextEditWriter::WriteInternalId(LPCTSTR name, unsigned int id)
{
    WriteUInt32(name, id);
}

/// <summary>
/// Writes an object to text by calling its implementation of <see cref="System.Object.ToString"/>.
/// </summary>
/// <param name="name">A name tag for the item</param>
/// <param name="value">The object to write (if a null is supplied, just the name tag will be written).</param>
void TextEditWriter::WriteValue(LPCTSTR name, LPCTSTR value)
{
    if (value == 0)
		WriteLine(name);
    else
	{
		WriteIndent();
		fprintf(m_File, "%s=%s\n", name, value);
	}
}

/// <summary>
/// Writes the text that precedes the data values for an object.
/// </summary>
void TextEditWriter::WriteBeginObject()
{
	WriteLiteral("{");
	m_NumIndent++;
}

/// <summary>
/// Writes the text that follows the data values for an object.
/// </summary>
void TextEditWriter::WriteEndObject()
{
	m_NumIndent--;
	WriteLiteral("}");
}

/// <summary>
/// Writes a text string (preceded by the current indent).
/// </summary>
/// <param name="value">The string to write.</param>
void TextEditWriter::WriteLiteral(LPCTSTR val)
{
	WriteLine(val);
}

// Writes text to the output, preceded by any indent, and followed by a newline
void TextEditWriter::WriteLine(LPCTSTR line)
{
	WriteIndent();
	fputs(line, m_File);
	fputs("\n", m_File);
}

void TextEditWriter::WriteIndent()
{
	for (int i=0; i<m_NumIndent; i++)
		fputc('\t', m_File);
}