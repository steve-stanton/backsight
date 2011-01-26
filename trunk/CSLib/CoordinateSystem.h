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

	private:
		cs_Csprm_* m_CsData;

	};
}
