@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" build.msproj /p:Configuration="%config%" /p:Platform="x64" /m /v:M /fl /flp:LogFile=msbuild2008.log;Verbosity=Normal /nr:false /target:ZipFile /p:Edition="2008"
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" build.msproj /p:Configuration="%config%" /p:Platform="x64" /m /v:M /fl /flp:LogFile=msbuild2012.log;Verbosity=Normal /nr:false /target:ZipFile /p:Edition="2012"
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" build.msproj /p:Configuration="%config%" /p:Platform="x64" /m /v:M /fl /flp:LogFile=msbuild2014.log;Verbosity=Normal /nr:false /target:ZipFile /p:Edition="2014"
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" build.msproj /p:Configuration="%config%" /p:Platform="x64" /m /v:M /fl /flp:LogFile=msbuild2016.log;Verbosity=Normal /nr:false /target:ZipFile /p:Edition="2016"
pause