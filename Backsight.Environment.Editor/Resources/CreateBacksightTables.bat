echo off
set DBSERVER=%1
set DBNAME=%2
set WORKDIR=%3
rem sqlcmd -S "%DBSERVER%" -d %DBNAME% -i %WORKDIR%\CreateTables.sql
rem sqlcmd -S "%DBSERVER%" -d %DBNAME% -i %WORKDIR%\AddForeignKeys.sql
sqlcmd -S "%DBSERVER%" -d Test4 -i %WORKDIR%\CreateTables.sql
sqlcmd -S "%DBSERVER%" -d Test4 -i %WORKDIR%\AddForeignKeys.sql