REM Copy in sample domains (based on the data that is provided as part of the standard MB distribution)

SET db=Backsight.dbo

bcp %db%.DOMAIN_DLS_PARCEL_TYPES          in Domains\DLS_PARCEL_TYPES.txt          -c -S localhost\sqlexpress -T
bcp %db%.DOMAIN_HALVES_of_DLS_QS_Polygons in Domains\HALVES_of_DLS_QS_Polygons.txt -c -S localhost\sqlexpress -T
bcp %db%.DOMAIN_Issuing_LTOs              in Domains\Issuing_LTOs.txt              -c -S localhost\sqlexpress -T
bcp %db%.DOMAIN_Meridian_Values           in Domains\Meridian_Values.txt           -c -S localhost\sqlexpress -T
bcp %db%.DOMAIN_Parcel_Types              in Domains\Parcel_Types.txt              -c -S localhost\sqlexpress -T
bcp %db%.DOMAIN_Parish_Lot_Types          in Domains\Parish_Lot_Types.txt          -c -S localhost\sqlexpress -T
bcp %db%.DOMAIN_Parish_Values             in Domains\Parish_Values.txt             -c -S localhost\sqlexpress -T
bcp %db%.DOMAIN_Part_of_Cadastral_Polygon in Domains\Part_of_Cadastral_Polygon.txt -c -S localhost\sqlexpress -T
bcp %db%.DOMAIN_Quarter_Sections          in Domains\Quarter_Sections.txt          -c -S localhost\sqlexpress -T
bcp %db%.DOMAIN_WaterBodyTypes            in Domains\WaterBodyTypes.txt            -c -S localhost\sqlexpress -T
bcp %db%.DOMAIN_Plan_Types                in Domains\Plan_Types.txt                -c -S localhost\sqlexpress -T
bcp %db%.DOMAIN_Range_Values              in Domains\Range_Values.txt              -c -S localhost\sqlexpress -T
