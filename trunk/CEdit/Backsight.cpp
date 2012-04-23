#include "StdAfx.h"
#include "Backsight.h"
#include <typeinfo>
#include "DataField.h"
#include "Observations.h"
#include "Changes.h"
#include "EditSerializer.h"

/*
LPCTSTR Persistent_c::GetTypeName() const
{
	//const char* typeName(typeid(*this).name());
	//return &typeName[6]; // ignore leading "class "

	static LPCTSTR typeName = "unknown";
	return typeName;
}
*/

LPCTSTR MovePolygonPositionOperation_c::GetTypeName() const
{
	static LPCTSTR typeName = "MovePolygonPositionOperation";
	return typeName;
}

void MovePolygonPositionOperation_c::WriteData(EditSerializer& s) const
{
	Operation_c::WriteData(s);

    s.WriteFeatureRef(DataField_Label, Label);
    s.WritePointGeometry(DataField_NewX, DataField_NewY, NewPosition);

    if (OldPosition != 0)
        s.WritePointGeometry(DataField_OldX, DataField_OldY, *OldPosition);
}

#ifdef _CEDIT

void BacksightExporter::ExportSession(CeSession& session)
{
	// Export the new session event
	CTime startTime(session.GetStart().GetTimeValue());
	CTime endTime(session.GetEnd().GetTimeValue());
	static LPCTSTR machineName = "";
	NewSessionEvent_c startSession(AllocateNextId(), startTime, (LPCTSTR)session.GetpWho()->GetpWho(), machineName);
	startSession.WriteData(m_Serializer);

	// Figure out the average time between successive edits
	CPSEPtrList& ops = session.GetOperations();
	int nop = ops.GetCount();
	LONG sessionSecs = (endTime - startTime).GetTotalSeconds();
	LONG secsPerEdit = sessionSecs / (nop + 1);

	POSITION pos = ops.GetHeadPosition();
	for (int i=0; i<nop; i++)
	{
		//CeOperation& op = op
		LONG secs = (i+1) * secsPerEdit;
		CTimeSpan delta(0,0,0, secs);
		CTime when = startTime + delta;
		op.ExportEdit(
	}

	// Export the end session event
	EndSessionEvent_c endSession(AllocateNextId(), endTime);
	endSession.WriteData(m_Serializer);
}

#endif

#ifdef _CEDIT

void BacksightExporter::ExportOperation(EditSerializer& s, const CeOperation& op)
{
	switch (op.GetType())
	{
	case CEOP_DATA_IMPORT:
		return new ImportOperation_c(s, (const CeImport&)op);

	case CEOP_ARC_SUBDIVISION:
	{
		const CeArcSubdivision& op = (const CeArcSubdivision&)op;
		LineSubdivisionOperation_c* res1 = new LineSubdivisionOperation_c(s, op, 0);

		if (op.IsMultiFace())
			LineSubdivisionOperation_c* res2 = new LineSubdivisionOperation_c(s, op, res1->Sequence);
	}

	case CEOP_DIR_INTERSECT:
		return new IntersectTwoDirectionsOperation_c(s, (const CeIntersectDir&)op);

	case CEOP_DIST_INTERSECT:
		return new IntersectTwoDistancesOperation_c(s, (const CeIntersectDist&)op);

	case CEOP_DIRDIST_INTERSECT:
		return new IntersectDirectionAndDistanceOperation_c(s, (const CeIntersectDirDist&)op);

	case CEOP_LINE_INTERSECT:
		return new IntersectTwoLinesOperation_c(s, (const CeIntersectLine&)op);

	case CEOP_NEW_POINT:
		return new NewPointOperation_c(s, (const CeNewPoint&)op);

	case CEOP_NEW_LABEL:
		return new NewTextOperation_c(s, (const CeNewLabel&)op);

	case CEOP_MOVE_LABEL:
		return new MoveTextOperation_c(s, (const CeMoveLabel&)op);

	case CEOP_DELETION:
		return new DeletionOperation_c(s, (const CeDeletion&)op);

	case CEOP_UPDATE:
		return 0; // TODO

	case CEOP_NEW_ARC:
		return new NewLineOperation_c(s, (const CeNewArc&)op);

	case CEOP_PATH:
		return new PathOperation_c(s, (const CePath&)op);

	case CEOP_AREA_SUBDIVISION:
		return new PolygonSubdivisionOperation_c(s, (const CeAreaSubdivision&)op);

	case CEOP_SET_LABEL_ROTATION:
		return new TextRotationOperation_c(s, (const CeSetLabelRotation&)op);

	case CEOP_GET_BACKGROUND:
		return new ImportOperation_c(s, (const CeGetBackground&)op);

	case CEOP_GET_CONTROL:
		return new GetControlOperation_c(s, (const CeGetControl&)op);

	case CEOP_NEW_CIRCLE:
		return new NewCircleOperation_c(s, (const CeNewCircle&)op);

	case CEOP_DIRLINE_INTERSECT:
		return new IntersectDirLineOperation_c(s, (const CeIntersectDirLine&)op);

	case CEOP_ARC_EXTEND:
		return new LineExtensionOperation_c(s, (const CeArcExtension&)op);

	case CEOP_RADIAL:
		return new RadialOperation_c(s, (const CeRadial&)op);

	case CEOP_SET_THEME:
		assert(1==0);

	case CEOP_SET_TOPOLOGY:
		return new SetTopologyOperation_c(s, (const CeSetTopology&)op);

	case CEOP_POINT_ON_LINE:
		return new SimpleLineSubdivisionOperation_c(s, (const CePointOnLine&)op);

	case CEOP_PARALLEL:
		return new ParallelLineOperation_c(s, (const CeArcParallel&)op);

	case CEOP_TRIM:
		return new TrimLineOperation_c(s, (const CeArcTrim&)op);

	case CEOP_ATTACH_POINT:
		return new AttachPointOperation_c(s, (const CeAttachPoint&)op);
	}
}

#endif
