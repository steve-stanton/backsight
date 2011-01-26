#pragma once

#include "cs_map.h"

using namespace System;
using namespace Backsight;

namespace CSLib
{
	public ref class CoordinateSystem
	{
	public:
		CoordinateSystem(String^ csKeyName);
		~CoordinateSystem();

		double GetScaleFactor(IPosition^ p);
		double GetLineScaleFactor(IPosition^ a, IPosition^ b);
		IPosition^ GetGeographic(IPosition^ p);
        double GetGroundArea(array<IPosition^>^ closedShape);

	private:

		/* The equatorial radius, in meters */
		property double EquitorialRadius
		{
			double get() { return m_CsData->datum.e_rad; }
		}

		/* The polar radius, in meters */
		property double PolarRadius
		{
			double get() { return m_CsData->datum.p_rad; }
		}

		property double GeoidSeparation
		{
			double get() { return m_CsData->csdef.geoid_sep; }
			void set(double value) { m_CsData->csdef.geoid_sep = value; }
		}

		// hgt_zz is supposedly an orthometric height (should find out what
		// that means one day). The mean elevation is used by GetGroundArea.
		// Does CSMap use it?
		property double MeanElevation
		{
			double get() { return m_CsData->csdef.hgt_zz; }
			void set(double value) { m_CsData->csdef.hgt_zz = value; }
		}

		cs_Csprm_* m_CsData;
	};
}
