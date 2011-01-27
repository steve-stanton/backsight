#pragma once

#include "cs_map.h"

using namespace System;
using namespace Backsight;

namespace CSLib
{
	public ref class CoordinateSystem : public ISpatialSystem
	{
	public:
		CoordinateSystem(String^ csKeyName);
		~CoordinateSystem();

		double GetScaleFactor(IPosition^ p);

		/* Implement ISpatialSystem */
		/* see http://stackoverflow.com/questions/880984/implementing-an-interface-declared-in-c-from-c-cli */

		virtual IPosition^ __clrcall GetGeographic(IPosition^ p) sealed;
		virtual double __clrcall GetLineScaleFactor(IPosition^ a, IPosition^ b) sealed;
        virtual double __clrcall GetGroundArea(array<IPosition^>^ closedShape) sealed;

		/*
		** The folder containing CSMap dictionary files. Must be defined before
		** attempting to instantiate any coordinate systems (if you fail to do so,
		** the CoordinateSystem constructor will attempt to define this by looking
		** at the CS_MAP_DIR environment variable).
		*/
		static property String^ Home
		{
			String^ get();
			void set(String^ folder);
		}

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

		static String^ s_CsFolder;
	};
}
