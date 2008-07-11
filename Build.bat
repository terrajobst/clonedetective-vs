@ECHO OFF

CD Build
"%SystemRoot%\Microsoft.NET\Framework\v3.5\MSBuild.exe" Build.proj

PAUSE