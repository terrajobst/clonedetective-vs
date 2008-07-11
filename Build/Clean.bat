@echo off

SET PATH=%PATH%;%SystemRoot%\Microsoft.NET\Framework\v3.5
MSBuild Build.proj /t:Clean

PAUSE