@echo off

SET MSBUILDDIR=%PROGRAMFILES(X86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\

IF NOT EXIST %MSBUILDDIR%nul goto MissingMSBuildToolsPath
IF NOT EXIST %MSBUILDDIR%msbuild.exe goto MissingMSBuildExe

FOR /F %%I IN ('DIR /B *.sln') DO SET PROGRAMNAME=%%I
@echo %PROGRAMNAME%
"%MSBUILDDIR%msbuild.exe" "%PROGRAMNAME%" /p:configuration=release /t:Clean;Rebuild /flp:logfile=%PROGRAMNAME%.log;errorsonly

pause

goto:eof
::ERRORS
::---------------------
:MissingMSBuildRegistry
echo Cannot obtain path to MSBuild tools from registry
goto:eof
:MissingMSBuildToolsPath
echo The MSBuild tools path from the registry '%MSBUILDDIR%' does not exist
goto:eof
:MissingMSBuildExe
echo The MSBuild executable could not be found at '%MSBUILDDIR%'
goto:eof
pause
