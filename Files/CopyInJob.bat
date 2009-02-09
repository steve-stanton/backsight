ECHO OFF

IF {%1}=={} GOTO Usage
IF {%2}=={} GOTO Usage

SET job=%1
SET db=%2

IF EXIST %job%-n-A01.txt bcp %db%.CertificateofTitleParcelData in %job%-n-A01.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A03.txt bcp %db%.DLSParcelData                in %job%-n-A03.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A05.txt bcp %db%.JudgesOrderParcelData        in %job%-n-A05.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A06.txt bcp %db%.PublicLaneData               in %job%-n-A06.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A07.txt bcp %db%.LegalInstrumentParcelData    in %job%-n-A07.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A09.txt bcp %db%.ParishLotParcelData          in %job%-n-A09.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A10.txt bcp %db%.PlanParcelData               in %job%-n-A10.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A12.txt bcp %db%.StreetData                   in %job%-n-A12.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A13.txt bcp %db%.WaterBodyData                in %job%-n-A13.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A14.txt bcp %db%.PublicWalkData               in %job%-n-A14.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A15.txt bcp %db%.CertificateofTitleParcelData in %job%-n-A15.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A16.txt bcp %db%.PropertyMapPolygonData       in %job%-n-A16.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A17.txt bcp %db%.PropertyMapPolygonData       in %job%-n-A17.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-n-A18.txt bcp %db%.PropertyMapPolygonData       in %job%-n-A18.txt -c -S localhost\sqlexpress -T

GOTO Exit

:Usage
ECHO Usage: Import [job-name] [database-name.schema]

:Exit
