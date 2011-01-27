#include "CoordinateSystem.h"
#include "Chars.h"

namespace CSLib
{

	// static
	String^ CoordinateSystem::Home::get()
	{
		return s_CsFolder;
	}

	// static
	void CoordinateSystem::Home::set(String^ folder)
	{
		int res = CS_altdr(Chars::Convert(folder));
		if (res!=0)
			throw gcnew Exception("Cannot locate coordinate system data folder");

		s_CsFolder = folder;
	}

	CoordinateSystem::CoordinateSystem(String^ csKeyName)
	{
		if (String::IsNullOrEmpty(s_CsFolder))
		{
			String^ home = System::Environment::GetEnvironmentVariable("CS_MAP_DIR");
			if (String::IsNullOrEmpty(home))
				throw gcnew Exception("CoordinateSystem.Home property has not been defined");

			this->Home = home;
		}

		m_CsData = CS_csloc(Chars::Convert(csKeyName));

		if (m_CsData == nullptr)
			throw gcnew Exception("Cannot locate coordinate system: "+csKeyName);
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

	double CoordinateSystem::GetGroundArea(array<IPosition^>^ v)
	{
        if (v->Length <= 2)
            return 0.0;

		double a = this->EquitorialRadius;
		double b = this->PolarRadius;

        // Get the ellipsoid scale factor for the map (note that
        // geoid separation is expected to be a positive value in
        // places where the geoid is above the reference ellipsoid).
		double efac = a / (a + this->MeanElevation + this->GeoidSeparation);

        // Get the eccentricity
        //double asq = a*a;
        //double esq = (asq - b*b)/(asq);

        // Actually, the documentation says you should divide by b*b,
        // not that it should make much difference
		double bsq = b*b;
        double esq = (a*a - bsq)/(bsq);

        // Work with a local origin the corresponds to the first
        // position (this DOES make quite a perceptible difference,
        // especially when the area is fairly small).

        double xo = v[0]->X;
        double yo = v[0]->Y;

        // The initial position is at the local origin (the
        // multiplying factor won't be used)

        double xs = 0.0;
        double ys = 0.0;
        double f1 = 0.0;

        double f2;
        double xe;
        double ye;

        double area = 0.0;

        for ( int i=1; i<v->Length; i++, f1=f2, xs=xe, ys=ye )
        {
            // Get the scale factor at the end of the current segment
            f2 = 1.0 / (GetScaleFactor(v[i]) * efac);
            xe = (v[i]->X - xo) * f2;
            ye = (v[i]->Y - yo) * f2;

            // Use the mid-X of the segment to get the area left (signed).
            // If the line is directed up the way, it contributes a
            // positive area. If directed down, it contributes a
            // negative area. So, if flat, it contributes nothing (what
            // we are actually calculating here is double the area; we
            // will adjust this when we are done with the loop).

            double extra = (ys-ye) * (xe+xs);
            area += extra;
        }

        return (area * 0.5);
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
