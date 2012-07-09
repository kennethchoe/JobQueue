Import-Module WebAdministration

if (Test-Path "IIS:\Sites\Default Web Site\BAMPlus\SecurityManager")
{
	"Remove IIS Web Application"
	Remove-WebApplication -Name "BAMPlus\SecurityManager" -Site "Default Web Site"
}

if (Test-Path "c:\inetpub\wwwroot\BAMPlus\SecurityManager")
{
	"Remove Web Site Files"
	Remove-Item c:\inetpub\wwwroot\BAMPlus\SecurityManager -recurse
}

if (Test-Path "IIS:\Sites\Default Web Site\BAMPlus\SecurityManagerService")
{
	"Remove IIS Web Application"
	Remove-WebApplication -Name "BAMPlus\SecurityManagerService" -Site "Default Web Site"
}

if (Test-Path "c:\inetpub\wwwroot\BAMPlus\SecurityManagerService")
{
	"Remove Web Site Files"
	Remove-Item c:\inetpub\wwwroot\BAMPlus\SecurityManagerService -recurse
}

$Server=".\SQLEXPRESS" 
$dbName="Bktb4_SecurityMgr_Db"         
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | out-null
$SMOserver = New-Object ('Microsoft.SqlServer.Management.Smo.Server') -argumentlist $Server
#$SMOserver.Databases | select Name, Size,DataSpaceUsage, IndexSpaceUsage, SpaceAva
if ($SMOserver.Databases[$dbName] -ne $null)  
{  
	"Remove Database"
	$SMOserver.databases[$dbname].drop()
} 

Remove-Item c:\shared\* -recurse
