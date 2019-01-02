@ECHO OFF
ECHO Upload Package for NuGet
ECHO Use as parameter the project name (*.nupkg)
nuget.exe push %1 oy2jaijucpfjs2bun7tqaem2fnasoxmuxndeeemyzt2bii -Source https://www.nuget.org/api/v2/package