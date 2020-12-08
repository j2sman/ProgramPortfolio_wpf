FOR /F %%I IN ('DIR /B *.sln') DO SET PROGRAMNAME=%%I
@echo %PROGRAMNAME%
"C:\Library\Nuget.exe" restore "%PROGRAMNAME%"

pause
