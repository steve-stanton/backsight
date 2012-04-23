#include "StdAfx.h"
#include <assert.h>

#ifdef _CEDIT
#include "CeSession.h"
#include "CeMap.h"
#include "CePerson.h"
#include "CeArcSubdivision.h"
#else
#include "CEditStubs.h"
#endif

#include "Changes.h"
#include "EditSerializer.h"
#include "CedExporter.h"


// Exports the current map to C:\Backsight

CedExporter::CedExporter(EditSerializer& s)
	: m_Serializer(s)
{
}

CedExporter::~CedExporter(void)
{
}

void CedExporter::ExportMap()
{
	// Generate an unique ID for the map (project)

	// Create index entry for the map
	CeMap* cedFile = CeMap::GetpMap();
	LPCTSTR mapName = cedFile->GetFileName();

	// Create output folder

	// Generate the output
	CPSEPtrList& sessions = cedFile->GetSessions();
	POSITION pos = sessions.GetHeadPosition();

	while (pos != 0)
	{
		CeSession* s = (CeSession*)sessions.GetNext(pos);
		ExportSession(*s);
	}

	// Rename the output file
}

#pragma comment(lib, "rpcrt4.lib")

void CedExporter::FillGuidString(CString& s) const
{
	s = "Testing";
	/*
	// See http://forums.codeguru.com/showthread.php?t=379736
	GUID Guid;
	CoCreateGuid(&Guid);
	//RPC_WSTR str;
	RPC_STR str;
	UuidToString((UUID*)&Guid, &str);

	s = (LPTSTR)str;

	RpcStringFree(&str);
	*/
}

void CedExporter::ExportSession(CeSession& session)
{
	// Ignore empty sessions
	const CPSEPtrList& ops = session.GetOperations();
	int nop = ops.GetCount();
	if (nop == 0)
		return;

	// Export the new session event
	CTime startTime(session.GetStart().GetTimeValue());
	CTime endTime(session.GetEnd().GetTimeValue());
	static LPCTSTR machineName = "";
	unsigned int sessionId = m_Serializer.GetInternalId();
	NewSessionEvent_c startSession(sessionId, startTime, (LPCTSTR)session.GetpWho()->GetpWho(), machineName);
	m_Serializer.WritePersistent(DataField_Edit, startSession);

	// Figure out the average time between successive edits (treat the end session event as an "edit")
	LONG sessionSecs = (endTime - startTime).GetTotalSeconds();
	LONG secsPerEdit = sessionSecs / (nop + 2);

	POSITION pos = ops.GetHeadPosition();
	for (int i=0; i<nop; i++)
	{
		CeOperation* op = (CeOperation*)ops.GetNext(pos);
		LONG secs = (i+1) * secsPerEdit;
		CTimeSpan delta(0,0,0, secs);
		CTime when = startTime + delta;
		ExportOperation(when, *op);
	}

	// Export the end session event
	unsigned int endId = m_Serializer.GetInternalId();
	EndSessionEvent_c endSession(endId, endTime);
	m_Serializer.WritePersistent(DataField_Edit, endSession);
}

unsigned int CedExporter::WriteOperation(Operation_c* op)
{
	m_Serializer.WritePersistent(DataField_Edit, *((const Operation_c*)op));
	unsigned int iid = op->Sequence;
	delete op;
	return iid;
}

unsigned int CedExporter::ExportOperation(const CTime& when, const CeOperation& op)
{
	switch (op.GetType())
	{
	case CEOP_DATA_IMPORT:
		return WriteOperation(new ImportOperation_c(m_Serializer, when, (const CeImport&)op));

	case CEOP_ARC_SUBDIVISION:
	{
		const CeArcSubdivision& sub = (const CeArcSubdivision&)op;
		unsigned int face1 = WriteOperation(new LineSubdivisionOperation_c(m_Serializer, when, sub, 0));

		if (sub.IsMultiFace())
			WriteOperation(new LineSubdivisionOperation_c(m_Serializer, when, sub, face1));

		return face1;
	}

	case CEOP_DIR_INTERSECT:
		return WriteOperation(new IntersectTwoDirectionsOperation_c(m_Serializer, when, (const CeIntersectDir&)op));

	case CEOP_DIST_INTERSECT:
		return WriteOperation(new IntersectTwoDistancesOperation_c(m_Serializer, when, (const CeIntersectDist&)op));

	case CEOP_DIRDIST_INTERSECT:
		return WriteOperation(new IntersectDirectionAndDistanceOperation_c(m_Serializer, when, (const CeIntersectDirDist&)op));

	case CEOP_LINE_INTERSECT:
		return WriteOperation(new IntersectTwoLinesOperation_c(m_Serializer, when, (const CeIntersectLine&)op));

	case CEOP_NEW_POINT:
		return WriteOperation(new NewPointOperation_c(m_Serializer, when, (const CeNewPoint&)op));

	case CEOP_NEW_LABEL:
		return WriteOperation(new NewTextOperation_c(m_Serializer, when, (const CeNewLabel&)op));

	case CEOP_MOVE_LABEL:
		return WriteOperation(new MoveTextOperation_c(m_Serializer, when, (const CeMoveLabel&)op));

	case CEOP_DELETION:
		return WriteOperation(new DeletionOperation_c(m_Serializer, when, (const CeDeletion&)op));

	case CEOP_NEW_ARC:
		return WriteOperation(new NewLineOperation_c(m_Serializer, when, (const CeNewArc&)op));

	case CEOP_PATH:
		return WriteOperation(new PathOperation_c(m_Serializer, when, (const CePath&)op));

	case CEOP_AREA_SUBDIVISION:
		return WriteOperation(new PolygonSubdivisionOperation_c(m_Serializer, when, (const CeAreaSubdivision&)op));

	case CEOP_SET_LABEL_ROTATION:
		return WriteOperation(new TextRotationOperation_c(m_Serializer, when, (const CeSetLabelRotation&)op));

	case CEOP_GET_BACKGROUND:
		return WriteOperation(new ImportOperation_c(m_Serializer, when, (const CeGetBackground&)op));

	case CEOP_GET_CONTROL:
		return WriteOperation(new GetControlOperation_c(m_Serializer, when, (const CeGetControl&)op));

	case CEOP_NEW_CIRCLE:
		return WriteOperation(new NewCircleOperation_c(m_Serializer, when, (const CeNewCircle&)op));

	case CEOP_DIRLINE_INTERSECT:
		return WriteOperation(new IntersectDirectionAndLineOperation_c(m_Serializer, when, (const CeIntersectDirLine&)op));

	case CEOP_ARC_EXTEND:
		return WriteOperation(new LineExtensionOperation_c(m_Serializer, when, (const CeArcExtension&)op));

	case CEOP_RADIAL:
		return WriteOperation(new RadialOperation_c(m_Serializer, when, (const CeRadial&)op));

	case CEOP_SET_THEME:
		assert(1==0);

	case CEOP_SET_TOPOLOGY:
		return WriteOperation(new SetTopologyOperation_c(m_Serializer, when, (const CeSetTopology&)op));

	case CEOP_POINT_ON_LINE:
		return WriteOperation(new SimpleLineSubdivisionOperation_c(m_Serializer, when, (const CePointOnLine&)op));

	case CEOP_PARALLEL:
		return WriteOperation(new ParallelLineOperation_c(m_Serializer, when, (const CeArcParallel&)op));

	case CEOP_TRIM:
		return WriteOperation(new TrimLineOperation_c(m_Serializer, when, (const CeArcTrim&)op));

	case CEOP_ATTACH_POINT:
		return WriteOperation(new AttachPointOperation_c(m_Serializer, when, (const CeAttachPoint&)op));
	}

	return 0;
}

// Generates IDs for all elements of a CED file that has a corresponding ID in a Backsight data file.
// This covers things like sessions, operations, features, and miscellaneous other things. This is done
// in case edits have been "corrected" to make use of features that were actually created by subsequent edits.
unsigned int CedExporter::GenerateIds(CeMap* cedFile, CMapPtrToPtr& objectIds)
{
	// Allocate an ID for the NewProjectEvent
	unsigned int maxId = 1;

	// ID allocations will be written as part of a special "export" session that immediately
	// follows the NewProjectEvent

	maxId++; // NewSessionEvent

	CeIdManager* idMan = CeIdHandle::GetIdManager();
	unsigned int nGroup = idMan->GetNumGroup();

	for (unsigned int i=0; i<nGroup; i++)
	{
		const CeIdGroup* group = idMan->GetpGroup(i);
		const CPtrList& ranges = group->GetIdRanges();

		POSITION pos = ranges.GetHeadPosition();
		while ( pos )
		{
			CeIdRange* range = (CeIdRange*)ranges.GetNext(pos);
			maxId++;
			objectIds.SetAt(range, (void*)maxId);
		}
	}

	maxId++; // EndSessionEvent

	// Now loop through each session (but ignore empty sessions).
	// Each session involves at least two IDs (NewSessionEvent and EndSessionEvent). Then we need
	// an ID for each edit, plus IDs for each feature that was created. In SOME cases, we need
	// additional IDs (e.g. each leg in a connection path gets it's own ID).

	CPSEPtrList& sessions = cedFile->GetSessions();
	POSITION spos = sessions.GetHeadPosition();

	while (spos != 0)
	{
		CeSession* s = (CeSession*)sessions.GetNext(spos);
		const CPSEPtrList& ops = s->GetOperations();
		int nop = ops.GetCount();

		if (nop > 0)
		{
			// Allocate ID for the NewSessionEvent
			maxId++;
			objectIds.SetAt(s, (void*)maxId);

			POSITION opos = ops.GetHeadPosition();
			for (int i=0; i<nop; i++)
			{
				// Allocate ID for the edit itself
				CeOperation* op = (CeOperation*)ops.GetNext(opos);
				maxId++;
				objectIds.SetAt(op, (void*)maxId);

				// For MOST edits, we just need an ID for each created feature. However, some edits
				// involve extra stuff (e.g. each leg on a connection path gets its own ID).
				//features.

				switch (op->GetType())
				{
					case CEOP_PATH:
						break;

					case CEOP_ARC_SUBDIVISION:
						break;

					default:
					{
						// Generate ID for each feature that was created.
						// ...do circles get an ID?

						CeObjectList features;
						op->GetFeatures(features);
						CeListIter iter(&features);
						void* pThing;

						for (pThing = iter.GetHead(); pThing; pThing = iter.GetNext())
						{
							maxId++;
							objectIds.SetAt(pThing, (void*)maxId);
						}
					}
				}
			}

			// Reserve one additional ID for the EndSessionEvent
			maxId++;
		}
	}

	return maxId;
}
