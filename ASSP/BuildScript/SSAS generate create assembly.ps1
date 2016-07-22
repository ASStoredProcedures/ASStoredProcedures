param (
	[string] $dllFile = $(throw "-dllFile parameter required"),
	[string] $xmlaFile 
)

if (-not ( test-path $dllFile)) { 
	write-error "The dll '$dllFile' does not exist."
	exit
	}
	
if ($xmlaFile -eq "") { $xmlaFile = $dllFile.Replace(".dll", ".xmla") }
write-host "Started Creating Xmla script: $xmlaFile"
## $file = "D:\Codeplex\ASStoredProcedures\ASSP\bin\Debug\ASSP.dll"
## $outfile = "D:\Codeplex\ASStoredProcedures\ASSP\bin\Debug\ASSP.xmla"

$arr = [System.IO.File]::ReadAllBytes($dllFile)
$blockCnt = $arr.Length / 1024
$header = @"
<Create AllowOverwrite="true" xmlns="http://schemas.microsoft.com/analysisservices/2003/engine">
  <ObjectDefinition>
    <Assembly xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:ddl2="http://schemas.microsoft.com/analysisservices/2003/engine/2" xmlns:ddl2_2="http://schemas.microsoft.com/analysisservices/2003/engine/2/2" xmlns:ddl100_100="http://schemas.microsoft.com/analysisservices/2008/engine/100/100" xmlns:ddl200="http://schemas.microsoft.com/analysisservices/2010/engine/200" xmlns:ddl200_200="http://schemas.microsoft.com/analysisservices/2010/engine/200/200" xmlns:ddl300="http://schemas.microsoft.com/analysisservices/2011/engine/300" xmlns:ddl300_300="http://schemas.microsoft.com/analysisservices/2011/engine/300/300" xmlns:ddl400="http://schemas.microsoft.com/analysisservices/2012/engine/400" xmlns:ddl400_400="http://schemas.microsoft.com/analysisservices/2012/engine/400/400" xsi:type="ClrAssembly">
      <ID>ASSP</ID>
      <Name>ASSP</Name>
      <Description />
      <ImpersonationInfo>
        <ImpersonationMode>Default</ImpersonationMode>
      </ImpersonationInfo>
      <Files>
        <File>
          <Name>ASSP.dll</Name>
          <Type>Main</Type>
          <Data>
"@

$footer = @"
 </Data>
        </File>
      </Files>
      <PermissionSet>Unrestricted</PermissionSet>
    </Assembly>
  </ObjectDefinition>
</Create>
"@

set-content -path $xmlaFile -value  $header

for ($i = 0; $i -lt $blockCnt; $i++)
{
	$str = [System.Convert]::ToBase64String( $arr[($i * 1024) ..((($i+1) * 1024) -1)] ) 
	$blk = "                <Block>$str</Block>"
	add-content -path $xmlaFile -value  $blk
}
add-content -path $xmlaFile -value $footer
write-host "Finised Creating Xmla script: $xmlaFile"

