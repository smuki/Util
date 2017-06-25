mkdir %~dp0temp
nuget pack %~dp0src\Utils.csproj -build -Symbols -Prop Configuration=Release -OutputDirectory %~dp0temp

nuget.exe push %~dp0temp\Utils*.nupkg 123456 -Source http://localhost/nuget/api/v2/package
del /q %~dp0temp\*
rmdir %~dp0temp
