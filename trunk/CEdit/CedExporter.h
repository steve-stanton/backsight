#pragma once

#ifdef _CEDIT
class CeOperation;
class CeMap;
#else
#include "CEditStubs.h"
#endif

class CedExporter
{
public:
	CedExporter();
	virtual ~CedExporter(void);
	void CreateExport(CeMap* cedFile);

private:
	void FillGuidString(CString& s) const;
	void FillComputerName(CString& name) const;
	void AppendExportItems(const CTime& when, const CeOperation& op, IdFactory& idf, CPtrArray& exportItems);
};

