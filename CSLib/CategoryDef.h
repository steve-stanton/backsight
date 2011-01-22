#pragma once

#include "CoordinateSystemDef.h"

namespace CSLib
{
	public ref class CategoryDef
	{
	public:

		property String^ Name;
		property array<CoordinateSystemDef^>^ Systems;

		virtual String^ ToString() override
		{
			return this->Name;
		}
	};
}
