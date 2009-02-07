rem The Selkirk database is a sample provided by MB as part of a standard distribution

mkdir Domains
bcp Selkirk.dbo.DOMAIN_DLS_PARCEL_TYPES          out Domains\DLS_PARCEL_TYPES.txt          -c -S localhost\sqlexpress -T
bcp Selkirk.dbo.DOMAIN_HALVES_of_DLS_QS_Polygons out Domains\HALVES_of_DLS_QS_Polygons.txt -c -S localhost\sqlexpress -T
bcp Selkirk.dbo.DOMAIN_Issuing_LTOs              out Domains\Issuing_LTOs.txt              -c -S localhost\sqlexpress -T
bcp Selkirk.dbo.DOMAIN_Meridian_Values           out Domains\Meridian_Values.txt           -c -S localhost\sqlexpress -T
bcp Selkirk.dbo.DOMAIN_Parcel_Types              out Domains\Parcel_Types.txt              -c -S localhost\sqlexpress -T
bcp Selkirk.dbo.DOMAIN_Parish_Lot_Types          out Domains\Parish_Lot_Types.txt          -c -S localhost\sqlexpress -T
bcp Selkirk.dbo.DOMAIN_Parish_Values             out Domains\Parish_Values.txt             -c -S localhost\sqlexpress -T
bcp Selkirk.dbo.DOMAIN_Part_of_Cadastral_Polygon out Domains\Part_of_Cadastral_Polygon.txt -c -S localhost\sqlexpress -T
bcp Selkirk.dbo.DOMAIN_Quarter_Sections          out Domains\Quarter_Sections.txt          -c -S localhost\sqlexpress -T
bcp Selkirk.dbo.DOMAIN_WaterBodyTypes            out Domains\WaterBodyTypes.txt            -c -S localhost\sqlexpress -T
bcp Selkirk.dbo.DOMAIN_Plan_Types                out Domains\Plan_Types.txt                -c -S localhost\sqlexpress -T
bcp Selkirk.dbo.DOMAIN_Range_Values              out Domains\Range_Values.txt              -c -S localhost\sqlexpress -T
