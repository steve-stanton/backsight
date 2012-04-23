#pragma once

#ifdef _CEDIT
class CeSession;
class CeMap;
#else
#include "CEditStubs.h"
#endif

class CedExporter
{
public:
	CedExporter(EditSerializer& s);
	virtual ~CedExporter(void);
	void ExportMap();
	unsigned int GenerateIds(CeMap* cedFile, CMapPtrToPtr& objectIds);

private:
	void ExportSession(CeSession& session);
	unsigned int WriteOperation(Operation_c* op);
	unsigned int ExportOperation(const CTime& when, const CeOperation& op);
	void FillGuidString(CString& s) const;

private:
	EditSerializer& m_Serializer;
};

