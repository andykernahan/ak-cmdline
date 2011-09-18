@echo off
if "%1" == "" goto BuildDefault
goto BuildTarget

:BuildDefault
%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe src\AK.CmdLine.msbuild /p:Configuration=Release;BuildType=Release /t:All
goto End

:BuildTarget
%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe src\AK.CmdLine.msbuild /p:Configuration=Release;BuildType=Release /t:%*

:End