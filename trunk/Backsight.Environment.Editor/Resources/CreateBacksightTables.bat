echo off
set DBSERVER=%1
set DBNAME=%2
sqlcmd -S "%DBSERVER%" -d %DBNAME% -i CreateTables.sql
sqlcmd -S "%DBSERVER%" -d %DBNAME% -i AddForeignKeys.sql