@echo off
if "%1" == "" goto BuildDefault
goto BuildTarget

:BuildDefault
"%PROGRAMFILES(X86)%\MSBuild\14.0\Bin\MsBuild.exe" src\AK.CmdLine.msbuild /p:Configuration=Release;BuildType=Release /t:All
goto End

:BuildTarget
"%PROGRAMFILES(X86)%\MSBuild\14.0\Bin\MsBuild.exe" src\AK.CmdLine.msbuild /p:Configuration=Release;BuildType=Release /t:%*

:End