ECHO OFF

IF {%1}=={} GOTO Usage
IF {%2}=={} GOTO Usage

SET job=%1
SET db=%2

IF EXIST %job%-A01.txt bcp %db%.CertificateofTitleParcelData in %job%-A01.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A03.txt bcp %db%.DLSParcelData                in %job%-A03.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A05.txt bcp %db%.JudgesOrderParcelData        in %job%-A05.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A06.txt bcp %db%.PublicLaneData               in %job%-A06.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A07.txt bcp %db%.LegalInstrumentParcelData    in %job%-A07.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A09.txt bcp %db%.ParishLotParcelData          in %job%-A09.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A10.txt bcp %db%.PlanParcelData               in %job%-A10.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A12.txt bcp %db%.StreetData                   in %job%-A12.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A13.txt bcp %db%.WaterBodyData                in %job%-A13.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A14.txt bcp %db%.PublicWalkData               in %job%-A14.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A15.txt bcp %db%.CertificateofTitleParcelData in %job%-A15.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A16.txt bcp %db%.PropertyMapPolygonData       in %job%-A16.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A17.txt bcp %db%.PropertyMapPolygonData       in %job%-A17.txt -c -S localhost\sqlexpress -T
IF EXIST %job%-A18.txt bcp %db%.PropertyMapPolygonData       in %job%-A18.txt -c -S localhost\sqlexpress -T

GOTO Exit

:Usage
ECHO Usage: CopyInJob [job-name] [database-name.schema]

:Exit
