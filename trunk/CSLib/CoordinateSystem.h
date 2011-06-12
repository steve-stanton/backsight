#pragma once

#include "cs_map.h"
#include "Chars.h"

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

		/* why virtual, __clrcall, sealed? - see 
		** http://stackoverflow.com/questions/880984/implementing-an-interface-declared-in-c-from-c-cli
		*/

		virtual IPosition^ __clrcall GetGeographic(IPosition^ p) sealed;
		virtual double __clrcall GetLineScaleFactor(IPosition^ a, IPosition^ b) sealed;
        virtual double __clrcall GetGroundArea(array<IPosition^>^ closedShape) sealed;
		virtual String^ __clrcall GetWellKnownText() sealed;

		virtual property ILength^ GeoidSeparation
		{
			ILength^ __clrcall get() sealed
			{
				return gcnew Length(m_CsData->csdef.geoid_sep);
			}

			void __clrcall set(ILength^ value) sealed
			{
				m_CsData->csdef.geoid_sep = value->Meters;
			}
		}

		// hgt_zz is supposedly an orthometric height (should find out what
		// that means one day). The mean elevation is used by GetGroundArea.
		// Does CSMap use it?
		virtual property ILength^ MeanElevation
		{
			ILength^ __clrcall get() sealed
			{
				return gcnew Length(m_CsData->csdef.hgt_zz);
			}

			void __clrcall set(ILength^ value) sealed
			{
				m_CsData->csdef.hgt_zz = value->Meters;
			}
		}

		/*
		** A brief name for the coordinate system.
		*/
		virtual property String^ Name
		{
			String^ __clrcall get() sealed
			{
				return Chars::Convert(m_CsData->csdef.key_nm);
			}
		}

		/*
		** The EPSG number for the system (0 if not known).
		*/
		virtual property int EpsgNumber
		{
			int __clrcall get() sealed
			{
				return m_CsData->csdef.epsgNbr;
			}
		}

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

		cs_Csprm_* m_CsData;

		static String^ s_CsFolder;
	};
}
