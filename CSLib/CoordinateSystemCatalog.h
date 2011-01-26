#pragma once

#include "CoordinateSystemDef.h"
#include "DatumDef.h"
#include "EllipsoidDef.h"
#include "CategoryDef.h"

using namespace System;

namespace CSLib
{
	public ref class CoordinateSystemCatalog
	{

	public:

		CoordinateSystemCatalog(String^ csFolder);
		~CoordinateSystemCatalog();

		property array<CoordinateSystemDef^>^ Systems;
		property array<DatumDef^>^ Datums;
		property array<EllipsoidDef^>^ Ellipsoids;
		property array<CategoryDef^>^ Categories;

		void Load();
		CoordinateSystemDef^ FindByEPSGNumber(short epsgNumber);

	private:
		array<DatumDef^>^ ReadDatums();
		array<EllipsoidDef^>^ ReadEllipsoids();
		array<CoordinateSystemDef^>^ ReadSystems();
		array<CategoryDef^>^ ReadCategories();
		DatumDef^ FindDatumByKeyName(String^ keyName);
		EllipsoidDef^ FindEllipsoidByKeyName(String^ keyName);

		String^ m_CSFolder;
	};
}
