#include "CoordinateSystem.h"
#include "Chars.h"

namespace CSLib
{
	CoordinateSystem::CoordinateSystem(String^ csKeyName)
	{
		m_CsData = CS_csloc(Chars::Convert(csKeyName));
	}

	CoordinateSystem::~CoordinateSystem()
	{
		CS_free(m_CsData);
	}

	double CoordinateSystem::GetScaleFactor(IPosition^ p)
	{
		IPosition^ ll = GetGeographic(p);
		double latlon[3];
		latlon[0] = ll->X;
		latlon[1] = ll->Y;
		return CS_csscl(m_CsData, latlon);
	}

	IPosition^ CoordinateSystem::GetGeographic(IPosition^ p)
	{
		double xy[3];
		xy[0] = p->X;
		xy[1] = p->Y;
		double latlon[3];
		int res = CS_cs2ll(m_CsData, latlon, xy);
		return gcnew Position(latlon[0], latlon[1]);
	}

	double CoordinateSystem::GetLineScaleFactor(IPosition^ a, IPosition^ b)
	{
		double sfa = GetScaleFactor(a);
		double sfb = GetScaleFactor(b);
		return 0.5 * (sfa + sfb);
	}

	// The structure doesn't provide the expected values, but WKT does
	//double CoordinateSystem::GetFalseEasting()
	//{
	//	//cs_Trmer_* tm = (cs_Trmer_*)m_CsData;
	//	cs_Trmrs_* tm = (cs_Trmrs_*)m_CsData;
	//	//tm->x_off = 500000.0;
	//	return tm->x_off;
	//}

	// Takes a surprising time on first call
	//String^ CoordinateSystem::WKT()
	//{
	//	char buf [512];
	//	CS_cs2Wkt(buf, sizeof(buf), "UTM83-14", 0);
	//	return Chars::Convert(buf);
	//}

}
