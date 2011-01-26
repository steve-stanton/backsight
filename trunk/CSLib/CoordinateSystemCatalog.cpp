#include "CoordinateSystemCatalog.h"
#include "cs_map.h"
#include "Chars.h"

using namespace System;
using namespace System::IO;
using namespace System::Collections::Generic;

namespace CSLib
{
	CoordinateSystemCatalog::CoordinateSystemCatalog(String^ csFolder)
	{
		m_CSFolder = csFolder;
	}

	CoordinateSystemCatalog::~CoordinateSystemCatalog()
	{
	}

	void CoordinateSystemCatalog::Load()
	{
		int res = CS_altdr(Chars::Convert(m_CSFolder));
		if (res!=0)
			throw gcnew Exception("Cannot locate coordinate system data folder");

		this->Datums = ReadDatums();
		this->Ellipsoids = ReadEllipsoids();
		this->Systems = ReadSystems();
		this->Categories = ReadCategories();
	}

	CoordinateSystemDef^ CoordinateSystemCatalog::FindByEPSGNumber(short epsgNumber)
	{
		for each (CoordinateSystemDef^ cs in this->Systems)
		{
			if (cs->EPSGNumber == epsgNumber)
				return cs;
		}

		return nullptr;
	}

	DatumDef^ CoordinateSystemCatalog::FindDatumByKeyName(String^ keyName)
	{
		for each (DatumDef^ datum in this->Datums)
		{
			if (datum->KeyName == keyName)
				return datum;
		}

		return nullptr;
	}

	EllipsoidDef^ CoordinateSystemCatalog::FindEllipsoidByKeyName(String^ keyName)
	{
		for each (EllipsoidDef^ e in this->Ellipsoids)
		{
			if (e->KeyName == keyName)
				return e;
		}

		return nullptr;
	}

	array<CoordinateSystemDef^>^ CoordinateSystemCatalog::ReadSystems()
	{
		int res, crypt;
		List<CoordinateSystemDef^>^ csDefs = gcnew List<CoordinateSystemDef^>();
		cs_Csdef_ cs;
		FILE* csFile = CS_csopn("rb");

		while ((res = CS_csrd (csFile,&cs,&crypt)) > 0)
		{
			CoordinateSystemDef^ csDef = gcnew CoordinateSystemDef();

			csDef->KeyName = Chars::Convert(cs.key_nm);
			csDef->DatumKeyName = Chars::Convert(cs.dat_knm);
			csDef->EllipsoidKeyName = Chars::Convert(cs.elp_knm);
			csDef->ProjectionKeyName = Chars::Convert(cs.prj_knm);
			csDef->Group = Chars::Convert(cs.group);
			csDef->Location = Chars::Convert(cs.locatn);
			csDef->CountriesOrStates = Chars::Convert(cs.cntry_st);
			csDef->Units = Chars::Convert(cs.unit);
			csDef->Param01 = cs.prj_prm1;
			csDef->Param02 = cs.prj_prm2;
			csDef->Param03 = cs.prj_prm3;
			csDef->Param04 = cs.prj_prm4;
			csDef->Param05 = cs.prj_prm5;
			csDef->Param06 = cs.prj_prm6;
			csDef->Param07 = cs.prj_prm7;
			csDef->Param08 = cs.prj_prm8;
			csDef->Param09 = cs.prj_prm9;
			csDef->Param10 = cs.prj_prm10;
			csDef->Param11 = cs.prj_prm11;
			csDef->Param12 = cs.prj_prm12;
			csDef->Param13 = cs.prj_prm13;
			csDef->Param14 = cs.prj_prm14;
			csDef->Param15 = cs.prj_prm15;
			csDef->Param16 = cs.prj_prm16;
			csDef->Param17 = cs.prj_prm17;
			csDef->Param18 = cs.prj_prm18;
			csDef->Param19 = cs.prj_prm19;
			csDef->Param20 = cs.prj_prm20;
			csDef->Param21 = cs.prj_prm21;
			csDef->Param22 = cs.prj_prm22;
			csDef->Param23 = cs.prj_prm23;
			csDef->Param24 = cs.prj_prm24;
			csDef->LongitudeOrigin = cs.org_lng;
			csDef->LatitudeOrigin = cs.org_lat;
			csDef->FalseEasting = cs.x_off;
			csDef->FalseNorthing = cs.y_off;
			csDef->ScaleReduction = cs.scl_red;
			csDef->UnitsToMetersFactor = cs.unit_scl;
			csDef->MapScaleFactor = cs.map_scl;
			csDef->OldScaleFactor = cs.scale;
			csDef->ZeroX = cs.zero[0];
			csDef->ZeroY = cs.zero[1];
			csDef->ElevationPointLongitude = cs.hgt_lng;
			csDef->ElevationPointLatitude = cs.hgt_lat;
			csDef->Elevation = cs.hgt_zz;
			csDef->GeoidSeparation = cs.geoid_sep;
			csDef->MinLatitude = cs.ll_min[1];
			csDef->MinLongitude = cs.ll_min[0];
			csDef->MaxLatitude = cs.ll_max[1];
			csDef->MaxLongitude = cs.ll_max[0];
			csDef->MinX = cs.xy_min[0];
			csDef->MinY = cs.xy_min[1];
			csDef->MaxX = cs.xy_max[0];
			csDef->MaxY = cs.xy_max[1];
			csDef->Description = Chars::Convert(cs.desc_nm);
			csDef->Source = Chars::Convert(cs.source);
			csDef->Quadrant = cs.quad;
			csDef->ComplexSeriesOrder = cs.order;
			csDef->NumberOfZones = cs.zones;
			csDef->Protect = cs.protect;
			csDef->EPSGQuad = cs.epsg_qd;
			csDef->OracleSRID = cs.srid;
			csDef->EPSGNumber = cs.epsgNbr;
			csDef->WKTFlavor = cs.wktFlvr;

			// Provide expanded versions of important fields
			//csDef->Datum = FindDatumByKeyName(csDef->DatumKeyName);
			//csDef->Ellipsoid = FindEllipsoidByKeyName(csDef->EllipsoidKeyName);

			csDefs->Add(csDef);
		}

		CS_csDictCls (csFile);
		csFile = NULL;
		return csDefs->ToArray();
	}

	array<DatumDef^>^ CoordinateSystemCatalog::ReadDatums()
	{
		int res, crypt;
		cs_Dtdef_ datum;
		FILE* datumFile = CS_dtopn("rb");
		List<DatumDef^>^ datums = gcnew List<DatumDef^>();

		while ((res = CS_dtrd (datumFile,&datum,&crypt)) > 0)
		{
			DatumDef^ dd = gcnew DatumDef();

			dd->KeyName = Chars::Convert(datum.key_nm);
			dd->EllipsoidKeyName = Chars::Convert(datum.ell_knm);
			dd->Group = Chars::Convert(datum.group);
			dd->Location = Chars::Convert(datum.locatn);
			dd->CountriesOrStates = Chars::Convert(datum.cntry_st);
			dd->DeltaX = datum.delta_X;
			dd->DeltaY = datum.delta_Y;
			dd->DeltaZ = datum.delta_Z;
			dd->RotationX = datum.rot_X;
			dd->RotationY = datum.rot_Y;
			dd->RotationZ = datum.rot_Z;
			dd->BursaWolfeScale = datum.bwscale;
			dd->Name = Chars::Convert(datum.name);
			dd->Source = Chars::Convert(datum.source);
			dd->Protect = datum.protect;
			dd->ToWGS84Via = datum.to84_via;
			dd->EPSGNumber = datum.epsgNbr;
			dd->WKTFlavor = datum.wktFlvr;

			datums->Add(dd);
		}

		CS_dtDictCls (datumFile);
		datumFile = NULL;
		return datums->ToArray();
	}

	array<EllipsoidDef^>^ CoordinateSystemCatalog::ReadEllipsoids()
	{
		int res, crypt;
		cs_Eldef_ ellipsoid;
		FILE* elFile = CS_elopn("rb");
		List<EllipsoidDef^>^ elps = gcnew List<EllipsoidDef^>();

		while ((res = CS_elrd (elFile,&ellipsoid,&crypt)) > 0)
		{
			EllipsoidDef^ elp = gcnew EllipsoidDef();

			elp->KeyName = Chars::Convert(ellipsoid.key_nm);
			elp->Group = Chars::Convert(ellipsoid.group);
			elp->EquatorialRadius = ellipsoid.e_rad;
			elp->PolarRadius = ellipsoid.p_rad;
			elp->Flattening = ellipsoid.flat;
			elp->Eccentricity = ellipsoid.ecent;
			elp->Name = Chars::Convert(ellipsoid.name);
			elp->Source = Chars::Convert(ellipsoid.source);
			elp->Protect = ellipsoid.protect;
			elp->EPSGNumber = ellipsoid.epsgNbr;
			elp->WKTFlavor = ellipsoid.wktFlvr;

			elps->Add(elp);
		}

		CS_elDictCls (elFile);
		elFile = NULL;
		return elps->ToArray();
	}

	array<CategoryDef^>^ CoordinateSystemCatalog::ReadCategories()
	{
		// Index previously loaded systems
		Dictionary<String^, CoordinateSystemDef^>^ csDic = gcnew Dictionary<String^, CoordinateSystemDef^>();
		for each (CoordinateSystemDef^ cs in this->Systems)
			csDic->Add(cs->KeyName, cs);

		String^ catFile = Path::Combine(m_CSFolder, "category.asc");
		StreamReader^ sr = File::OpenText(catFile);
		String^ s;
		List<CoordinateSystemDef^>^ catSystems = gcnew List<CoordinateSystemDef^>();
		List<CategoryDef^>^ result = gcnew List<CategoryDef^>();
		CategoryDef^ cat = nullptr;

		while ((s = sr->ReadLine()) != nullptr)
		{
			if (s->StartsWith("["))
			{
				// Remember the current category (if there is one)
				if (catSystems->Count > 0)
				{
					cat->Systems = catSystems->ToArray();
					result->Add(cat);
					catSystems->Clear();
				}

				cat = gcnew CategoryDef();
				cat->Name = s->Substring(1, s->Length-2);
			}
			else
			{
				int eqPos = s->IndexOf("=");

				if (eqPos > 0)
				{
					s = s->Substring(0, eqPos)->Trim();
					CoordinateSystemDef^ cs = nullptr;

					// Some systems are included in the category list, but may not
					// be in the coordinate system list (possibly for legal reasons).
					if (csDic->TryGetValue(s, cs))
						catSystems->Add(cs);
				}
			}
		}

		if (catSystems->Count > 0)
		{
			cat->Systems = catSystems->ToArray();
			result->Add(cat);
		}

		sr->Close();
		return result->ToArray();
	}
}