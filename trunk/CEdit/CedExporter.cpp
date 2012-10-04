#include "StdAfx.h"
#include <assert.h>

#ifdef _CEDIT
#include "CeSession.h"
#include "CeMap.h"
#include "CePerson.h"
#include "CeIdManager.h"
#include "CeIdGroup.h"
#include "CeIdRange.h"
#include "CeIdHandle.h"
#include "CeOperation.h"
#else
#include "CEditStubs.h"
#endif

#include "Changes.h"
#include "TextEditWriter.h"
#include "EditSerializer.h"
#include "Features.h"
#include "CedExporter.h"


// Exports the current map to C:\Backsight

CedExporter::CedExporter()
{
}

CedExporter::~CedExporter(void)
{
}

#pragma comment(lib, "rpcrt4.lib")

void CedExporter::FillGuidString(CString& s) const
{
	// See http://forums.codeguru.com/showthread.php?t=379736
	GUID Guid;
	CoCreateGuid(&Guid);
	//RPC_WSTR str;
	unsigned char* str;
	UuidToString((UUID*)&Guid, &str);
	s = (LPTSTR)str;
	s.MakeUpper();

	RpcStringFree(&str);
}

void CedExporter::FillComputerName(CString& name) const
{
	DWORD buflen = 100;
	char cname[100];
	GetComputerName(cname, &buflen);

	name = cname;
}

#ifdef _CEDIT
#include "CeExportTypeUtil.h"
#include "CeTableEx.h"
#endif

void CedExporter::CreateExport(CeMap* cedFile)
{
	// Ensure root folders exist (methods will quietly fail if folders are already there)
	CreateDirectory("C:\\Backsight", 0);
	CreateDirectory("C:\\Backsight\\index", 0);

	// Ensure the export has not been done already by looking for an existing index entry
	LPCTSTR mapName = cedFile->GetFileName();
	CString indexFileName;
	indexFileName.Format("C:\\Backsight\\index\\%s.txt", mapName);
	CFileStatus fileStatus;
	if (CFile::GetStatus((LPCTSTR)indexFileName, fileStatus))
	{
		AfxMessageBox("Map has been exported previously");
		return;
	}

	IdFactory idFactory;
	CPtrArray items;

	// Generate a GUID for the project
	CString guid;
	FillGuidString(guid);

	// Record the current computer name
	CString machineName;
	FillComputerName(machineName);

	// Create the new project event (assuming UTM zone 14 on NAD83)
	CTime now = CTime::GetCurrentTime();
	int layerId = 10; // Survey layer
	items.Add(new NewProjectEvent_c(idFactory, now, (LPCTSTR)guid, mapName, layerId, "UTM83-14", "CEdit", (LPCTSTR)machineName));

	// Invent a pseudo-session to enclose all ID allocations (and any other stuff)
	items.Add(new NewSessionEvent_c(idFactory, now, "CEdit", ""));

	CeIdManager* idMan = CeIdHandle::GetIdManager();
	unsigned int nGroup = idMan->GetNumGroup();

	for (unsigned int i=0; i<nGroup; i++)
	{
		const CeIdGroup* group = idMan->GetpGroup(i);
		int groupId = idFactory.GetGroupId(group->GetGroupName());
		const CPtrList& ranges = group->GetIdRanges();

		POSITION pos = ranges.GetHeadPosition();
		while ( pos )
		{
			CeIdRange* range = (CeIdRange*)ranges.GetNext(pos);
			items.Add(new IdAllocation_c(idFactory, now, groupId, range->GetMin(), range->GetMax()));
		}
	}
	
	// Generate any points that will be needed for line ends (whereas CEdit would let you have lines without
	// an end point, Backsight requires them)
	ImportOperation_c* extra = new ImportOperation_c(idFactory, now);
	GenerateExtraPoints(cedFile, idFactory, extra->Features);
	//AfxMessageBox("done extra points");

	// Represent the points as an import operation
	if (extra->Features.GetSize() == 0)
		delete extra;
	else
		items.Add(extra);

	items.Add(new EndSessionEvent_c(idFactory, now));

	// Now loop through each session (but ignore empty sessions).
	CPSEPtrList& sessions = cedFile->GetSessions();
	POSITION spos = sessions.GetHeadPosition();

	while (spos != 0)
	{
		CeSession* session = (CeSession*)sessions.GetNext(spos);
		const CPSEPtrList& ops = session->GetOperations();
		int nop = ops.GetCount();

		if (nop > 0)
		{
			// Append the NewSessionEvent
			CTime startTime(session->GetStart().GetTimeValue());
			CTime endTime(session->GetEnd().GetTimeValue());
			items.Add(new NewSessionEvent_c(idFactory, startTime, (LPCTSTR)session->GetpWho()->GetpWho(), ""));

			// Figure out the average time between successive edits (treat the end session event as an "edit")
			LONG sessionSecs = (endTime - startTime).GetTotalSeconds();
			LONG secsPerEdit = sessionSecs / (nop + 2);

			POSITION opos = ops.GetHeadPosition();
			for (int i=0; i<nop; i++)
			{
				CeOperation* op = (CeOperation*)ops.GetNext(opos);
				LONG secs = (i+1) * secsPerEdit;
				CTimeSpan delta(0,0,0, secs);
				CTime when = startTime + delta;
				AppendExportItems(when, *op, idFactory, items);
			}

			// Append the end session event
			items.Add(new EndSessionEvent_c(idFactory, endTime));
		}
	}

	// Create the project folder
	CString projectFolder;
	projectFolder.Format("C:\\Backsight\\%s", (LPCTSTR)guid);
	CreateDirectory((LPCTSTR)projectFolder, 0);

	// Produce the output file
	unsigned int maxId = idFactory.GetNextId();
	CString fileName;
	fileName.Format("%s\\%08X.txt", (LPCTSTR)projectFolder, maxId);

	FILE* fp = fopen((LPCTSTR)fileName, "w");
	TextEditWriter* tw = new TextEditWriter(fp);
	EditSerializer* es = new EditSerializer(idFactory, *tw);

	for (int ix=0; ix<items.GetSize(); ix++)
	{
		Persistent_c* p = (Persistent_c*)items.GetAt(ix);
		es->WritePersistent(DataField_Edit, *p);
	}

	delete es;
	delete tw;
	fclose(fp);

	// Write the index entry file
	fp = fopen((LPCTSTR)indexFileName, "w");
	fprintf(fp, "%s", (LPCTSTR)guid);
	fclose(fp);

	// Remove the export objects
	for (int ip=0; ip<items.GetSize(); ip++)
	{
		Persistent_c* p = (Persistent_c*)items.GetAt(ip);
		delete p;
	}

	// Dump out attributes...

	// Obtain the mapping from schema to output file extension (for consistency with
	// current data distributions done by GeoManitoba).
	CeExportTypeUtil xt;
	int rcode = xt.Load();
	if ( rcode < 0 )
	{
		CString msg;
		xt.GetLoadMessage(msg,rcode);
		AfxMessageBox(msg);
		return;
	}

	// Collect the IDs
	CPtrList ids;
	cedFile->GetIds(ids);

	// Collect the rows attached to the IDs
	CPtrList rows;
	CeTableEx::CollectRows(rows, ids);

	// Group by table. Then dispense with the list of pointers to rows.
	CPtrList tables;
	CeTableEx::BinRows(tables, rows);
	rows.RemoveAll();

	// Go through each bin, exporting the info to an output text file.	
	POSITION pos = tables.GetHeadPosition();
	CString tableFileName;

	while ( pos )
	{
		CeTableEx* pTable = (CeTableEx*)tables.GetNext(pos);

		// Determine the name of the output file (based on the name of the schema)
		const CeSchema& schema = pTable->GetSchema();
		tableFileName.Format("%s\\%s-%s.txt", (LPCTSTR)projectFolder, mapName, xt.GetFileType(schema));

		// Write out the attributes
		pTable->Export((LPCTSTR)tableFileName);
		delete pTable;
	}

	// Remove pointers to the tables (now deleted).
	tables.RemoveAll();
}

#ifdef _CEDIT

// Need to include h-files for all the edits?
#include "CeImport.h"
#include "CeArcSubdivision.h"
#include "CeIntersectDir.h"
#include "CeIntersectDist.h"
#include "CeIntersectDirDist.h"
#include "CeIntersectLine.h"
#include "CeNewPoint.h"
#include "CeNewLabel.h"
#include "CeMoveLabel.h"
#include "CeDeletion.h"
#include "CeNewArc.h"
#include "CePath.h"
#include "CeAreaSubdivision.h"
#include "CeSetLabelRotation.h"
#include "CeGetBackground.h"
#include "CeGetControl.h"
#include "CeNewCircle.h"
#include "CeIntersectDirLine.h"
#include "CeArcExtension.h"
#include "CeRadial.h"
#include "CeSetTopology.h"
#include "CePointOnLine.h"
#include "CeArcParallel.h"
#include "CeArcTrim.h"
#include "CeAttachPoint.h"

#endif

void CedExporter::AppendExportItems(const CTime& when, const CeOperation& op, IdFactory& idf, CPtrArray& exportItems)
{
	switch (op.GetType())
	{
	case CEOP_DATA_IMPORT:
		exportItems.Add(new ImportOperation_c(idf, when, (const CeImport&)op));
		return;

	case CEOP_ARC_SUBDIVISION:
	{
		const CeArcSubdivision& sub = (const CeArcSubdivision&)op;
		LineSubdivisionOperation_c* face1 = new LineSubdivisionOperation_c(idf, when, sub, 0);
		exportItems.Add(face1);

		if (sub.IsMultiFace())
			exportItems.Add(new LineSubdivisionOperation_c(idf, when, sub, face1->Sequence));

		return;
	}

	case CEOP_DIR_INTERSECT:
		exportItems.Add(new IntersectTwoDirectionsOperation_c(idf, when, (const CeIntersectDir&)op));
		return;

	case CEOP_DIST_INTERSECT:
		exportItems.Add(new IntersectTwoDistancesOperation_c(idf, when, (const CeIntersectDist&)op));
		return;

	case CEOP_DIRDIST_INTERSECT:
		exportItems.Add(new IntersectDirectionAndDistanceOperation_c(idf, when, (const CeIntersectDirDist&)op));
		return;

	case CEOP_LINE_INTERSECT:
		exportItems.Add(new IntersectTwoLinesOperation_c(idf, when, (const CeIntersectLine&)op));
		return;

	case CEOP_NEW_POINT:
		exportItems.Add(new NewPointOperation_c(idf, when, (const CeNewPoint&)op));
		return;

	case CEOP_NEW_LABEL:
		exportItems.Add(new NewTextOperation_c(idf, when, (const CeNewLabel&)op));
		return;

	case CEOP_MOVE_LABEL:
		exportItems.Add(new MoveTextOperation_c(idf, when, (const CeMoveLabel&)op));
		return;

	case CEOP_DELETION:
	{
		DeletionOperation_c* dop = new DeletionOperation_c(idf, when, (const CeDeletion&)op);
		if (dop->Deletions.GetSize() > 0)
			exportItems.Add(dop);
		return;
	}

	case CEOP_NEW_ARC:
		exportItems.Add(new NewLineOperation_c(idf, when, (const CeNewArc&)op));
		return;

	case CEOP_PATH:
		exportItems.Add(new PathOperation_c(idf, when, (const CePath&)op));
		return;

	case CEOP_AREA_SUBDIVISION:
		exportItems.Add(new PolygonSubdivisionOperation_c(idf, when, (const CeAreaSubdivision&)op));
		return;

	case CEOP_SET_LABEL_ROTATION:
		exportItems.Add(new TextRotationOperation_c(idf, when, (const CeSetLabelRotation&)op));
		return;

	case CEOP_GET_BACKGROUND:
		exportItems.Add(new ImportOperation_c(idf, when, (const CeGetBackground&)op));
		return;

	case CEOP_GET_CONTROL:
		exportItems.Add(new GetControlOperation_c(idf, when, (const CeGetControl&)op));
		return;

	case CEOP_NEW_CIRCLE:
		exportItems.Add(new NewCircleOperation_c(idf, when, (const CeNewCircle&)op));
		return;

	case CEOP_DIRLINE_INTERSECT:
		exportItems.Add(new IntersectDirectionAndLineOperation_c(idf, when, (const CeIntersectDirLine&)op));
		return;

	case CEOP_ARC_EXTEND:
		exportItems.Add(new LineExtensionOperation_c(idf, when, (const CeArcExtension&)op));
		return;

	case CEOP_RADIAL:
		exportItems.Add(new RadialOperation_c(idf, when, (const CeRadial&)op));
		return;

	case CEOP_SET_THEME:
		AfxMessageBox("Cannot process set theme command");
		assert(1==0);
		return;

	case CEOP_SET_TOPOLOGY:
		exportItems.Add(new SetTopologyOperation_c(idf, when, (const CeSetTopology&)op));
		return;

	case CEOP_POINT_ON_LINE:
		exportItems.Add(new SimpleLineSubdivisionOperation_c(idf, when, (const CePointOnLine&)op));
		return;

	case CEOP_PARALLEL:
		exportItems.Add(new ParallelLineOperation_c(idf, when, (const CeArcParallel&)op));
		return;

	case CEOP_TRIM:
		exportItems.Add(new TrimLineOperation_c(idf, when, (const CeArcTrim&)op));
		return;

	case CEOP_ATTACH_POINT:
		exportItems.Add(new AttachPointOperation_c(idf, when, (const CeAttachPoint&)op));
		return;
	}
}

#ifdef _CEDIT
#include "CeListIter.h"
#include "CePoint.h"
#include "CeArc.h"
#include "CeOffsetPoint.h"
#endif

void CedExporter::GenerateExtraPoints(CeMap* cedFile, IdFactory& idf, CPtrArray& extraPoints)
{
	CPSEPtrList& sessions = cedFile->GetSessions();
	POSITION spos = sessions.GetHeadPosition();
	CeObjectList features;

	// Index of the locations that have been accounted for - the key is a CeLocation pointer,
	// the value is unused.
	CMapPtrToPtr locIndex;

	LogFile = fopen("C:\\Backsight\\Export.txt", "w");

	while (spos != 0)
	{
		CeSession* session = (CeSession*)sessions.GetNext(spos);
		const CPSEPtrList& ops = session->GetOperations();
		POSITION opos = ops.GetHeadPosition();

		while (opos != 0)
		{
			CeOperation* op = (CeOperation*)ops.GetNext(opos);
			op->GetFeatures(features);
			CeListIter* loop = new CeListIter(&features, TRUE);
			CeFeature* f;

			// Note the locations of points created by the edit
			for (f = (CeFeature*)loop->GetHead(); f; f = (CeFeature*)loop->GetNext())
			{
#ifdef _CEDIT
				objectstore::touch(f, false);
#endif
				const CePoint* point = dynamic_cast<const CePoint*>(f);
				if (point != 0)
				{
					// Note ALL coincident locations (since those may have been used as line
					// terminals rather than the original location).
					RecordLocations(*point, locIndex);
				}
			}

			//Log("start line loop");

			// Ignore new circle edits where the radius was specified as a distance. In that case, the
			// closing point must be generated by the edit itself (since Backsight always expects to
			// see a new point in that case).
			bool processLines = true;
			if (op->GetType() == CEOP_NEW_CIRCLE)
			{
				CeNewCircle* nc = (CeNewCircle*)op;
				CeObservation* o = nc->GetRadius();
#ifdef _CEDIT
				objectstore::touch(o, false);
#endif
				if (dynamic_cast<CeOffsetPoint*>(o) == 0)
					processLines = false;
			}

			if (processLines)
			{
				// Process the lines created by the edit. For each terminal, check whether the
				// location is already noted. If not, we need to fabricate a point.
				for (f = (CeFeature*)loop->GetHead(); f; f = (CeFeature*)loop->GetNext())
				{
#ifdef _CEDIT
					objectstore::touch(f, false);
#endif
					const CeArc* line = dynamic_cast<const CeArc*>(f);
					if (line != 0)
					{
						CheckForExtraPoint(line->GetpStart(), locIndex, idf, extraPoints);
						CheckForExtraPoint(line->GetpEnd(), locIndex, idf, extraPoints);
					}
				}
			}

			//Log("done line loop");

			features.Remove();
			delete loop;

			//Log("next");
		}
	}

	fclose(LogFile);
	LogFile = 0;
}

void CedExporter::CheckForExtraPoint(const CeLocation* loc, CMapPtrToPtr& locIndex, IdFactory& idf, CPtrArray& extraPoints)
{
	// Nothing to do if the location has already been noted
	void* x;
	if (locIndex.Lookup((void*)loc, x))
		return;

	CString msg;
	msg.Format("Recording extra point for %x", (int)loc);
	Log(msg);

	// Generate an extra point
	unsigned int entityId = 0;
	PointFeature_c* p = new PointFeature_c(idf, entityId, *loc);

	//msg.Format("Added point %d", p->Stub->InternalId);
	//Log(msg);

	extraPoints.Add(p);
	locIndex.SetAt((void*)loc, (void*)p->Stub->InternalId); // I don't think we really need the ID, but hold it just in case
}

void CedExporter::RecordLocations(const CePoint& p, CMapPtrToPtr& locIndex) 
{
	//CString s;
	//s.Format("Process point %s", p.FormatKey());
	//Log(s);

	//FILE* log = 0;
	//if (p.FormatKey() == "2632804")
	//	log = LogFile;

	CPtrArray locs;
	const CeLocation* loc = p.GetpVertex();
	GetAllCoincidentLocations(loc, locs, LogFile);

	//if (locs.GetSize() != 1)
	//{
	//	CString s;
	//	s.Format("%d locs", locs.GetSize());
	//	Log(s);
	//}

	for (int i=0; i<locs.GetSize(); i++)
	{
		void* pLoc = locs.GetAt(i);
		locIndex.SetAt(pLoc, 0);
	}
}

#ifdef _CEDIT
#include "CeTile.h"
#include "CeTileId.h"
#include "CeTileData.h"
#include "CeLocation.h"
#endif

// static
void CedExporter::GetAllCoincidentLocations(const CeLocation* loc, CPtrArray& locs, FILE* log)
{
	//if (log != 0)
	//{
	//	fprintf(log, "before loop\n");
	//	fflush(log);
	//}

	if (loc == 0)
		return;
	
	const CeTileId& tileId = loc->GetTileID();
	const CeTile* t = tileId.GetpTile();

	for (const CeTileData* td = t->GetpTileData()->GetpTail(); td; td = td->GetpPrev())
	{
		int nLoc = (int)td->GetNumLoc();

		//if (log != 0)
		//{
		//	fprintf(log, "nLoc=%d\n", nLoc);
		//	fflush(log);
		//}

		const CeLocation** tLocs = td->GetpLocations();

		for (int i=0; i<nLoc; i++)
		{
			const CeLocation* tLoc = tLocs[i];
			if ((*loc) == (*tLoc))
				locs.Add((void*)tLoc);
		}
	}

	//if (log != 0)
	//{
	//	fprintf(log, "done loop nDup=%d\n", locs.GetSize());
	//	fflush(log);
	//}
}

void CedExporter::Log(LPCTSTR msg)
{
	if (LogFile != 0)
	{
		fprintf(LogFile, "%s\n", msg);
		fflush(LogFile);
	}
}

void CedExporter::Log(const CString& msg)
{
	Log((LPCTSTR)msg);
}
