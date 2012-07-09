function Create-Website{
	param(
		[string] $name,
		[string] $location,
		[string] $port
	)
	
	Import-Module WebAdministration

    new-item IIS:\AppPools\$name    
    $demoPool = Get-Item IIS:\AppPools\$name
<#  
	$demoPool.processModel.userName = $accountName
    $demoPool.processModel.password = $password
    $demoPool.processModel.identityType = 3
#>
    $demoPool | Set-Item

	New-Item IIS:\Sites\$name -physicalPath $location -bindings @{protocol="http";bindingInformation=":$port:"}

	Set-ItemProperty IIS:\Sites\$name -name applicationPool -value $name
}

Export-ModuleMember Create-Website