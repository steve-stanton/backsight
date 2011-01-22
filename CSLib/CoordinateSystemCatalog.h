#pragma once

#include "CoordinateSystemDef.h"
#include "DatumDef.h"
#include "EllipsoidDef.h"
#include "CategoryDef.h"

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

	private:
		array<DatumDef^>^ ReadDatums();
		array<EllipsoidDef^>^ ReadEllipsoids();
		array<CoordinateSystemDef^>^ ReadSystems();
		array<CategoryDef^>^ ReadCategories();

		String^ m_CSFolder;
	};
}
