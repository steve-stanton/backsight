#pragma once
class TextEditWriter
{
public:
	TextEditWriter(FILE*& fp) : m_File(fp), m_NumIndent(0) {}
	virtual ~TextEditWriter(void) {}

	void WriteBeginObject();
	void WriteEndObject();
	void WriteLiteral(LPCTSTR value);
	void WriteByte(LPCTSTR name, byte value);
    void WriteInt32(LPCTSTR name, int value);
    void WriteUInt32(LPCTSTR name, unsigned int value);
    void WriteInt64(LPCTSTR name, __int64 value);
    void WriteDouble(LPCTSTR name, double value);
    void WriteSingle(LPCTSTR name, float value);
    void WriteBool(LPCTSTR name, bool value);
    void WriteString(LPCTSTR name, LPCTSTR value);
    void WriteDateTime(LPCTSTR name, const CTime& value);
    void WriteInternalId(LPCTSTR name, unsigned int id);

private:
	void WriteValue(LPCTSTR name, LPCTSTR value);
	void WriteIndent();
	void WriteLine(LPCTSTR line);

private:
	FILE*& m_File;
	int m_NumIndent;
};

