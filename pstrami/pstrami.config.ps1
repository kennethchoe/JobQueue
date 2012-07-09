#############################################	
#   Configuration Section
#############################################	

# projects:
$script:projectName = "SecurityManager";
$script:projectName2 = "SecurityManagerService";
$script:databaseServer=".\sqlexpress"
$script:BaseTargetDir="c:\inetpub\wwwroot\BAMPlus\" ; 
$script:databaseName = "Bktb4_SecurityMgr_Db"
$script:ConnectionString="server=$databaseServer;database=$databasename;Integrated Security=true;"

$script:iisFolderName = "IIS:\Sites\Default Web Site\BAMPlus\"

Environment "target" -servers @( 
    Server "localhost" @("SetEnvironments"; "Web"; "Db") ;) -installPath "c:\installs\$projectName"

#############################################	

Role "SetEnvironments" -Incremental {
	if($env:databaseServer -ne $null) {
		$script:databaseServer = $env:databaseServer
	}
} -FullInstall {
    #Set database server name from environment variable
}

Role "Web" -Incremental {
    write-host "Installing the websites"
	
	# project one
    $destination = $BaseTargetDir + $projectName
    Set-AppOffline $destination
    Delete-Dir $destination   
    Copy-Dir web\$projectName $destination 
    Update-WebConfigForProduction "$destination\web.config"    
    Set-AppOnline $destination
	Convert-Folder-To-IIS-Application ($iisFolderName + $projectName)
	
	#project 2
	$destination = $BaseTargetDir += $projectName2
    Set-AppOffline $destination
    Delete-Dir $destination   
    Copy-Dir web\$projectName2 $destination 
    Update-WebConfigForProduction "$destination\web.config"    
    Set-AppOnline $destination
	Convert-Folder-To-IIS-Application ($iisFolderName + $projectName2)
	
} -FullInstall {
    #Create Appliction User
    #Create IIS Site
    #Set Credentials on App Pool
}

Role "Db" -Incremental {
    write-host "Installing the Database"

    # backup database
    
    & Database\Tools\DatabaseDeployer.exe "Update" $databaseServer $databaseName "database"

} -FullInstall { 

    #create database
    #run migration
    & Database\Tools\DatabaseDeployer.exe "Create" $databaseServer $databaseName "database"

    #load initial data
    #Load-Data "$projectName.IntegrationTests.dll" $ConnectionString

	#### to do
    #create application login
    #Create-WindowsDatabaseLogin "LunchBoxApp" $databaseServer $databaseName
}

#Role "Application" -Incremental {
#    write-host "Installing the Batch Application"
#    $destination = $BaseTargetDir + "_" + $env + "_agents"
#    
#    # Disable Schedule Tasks
#    # Stop Tasks and Services
#    # Delete Existing Files
#    # Copy New Files
#    Copy-Dir Agents $destination 
#    # Enable Tasks
#    # Kick off tasks
#} -FullInstall { 
#    # create scheduled tasks
#    # set security for scheduled tasks
#    # Run Incremental 
#}



#----------------------------------------------------------------------------------

#function script:Update-NHibernateConfiguration ($filename,$connectionString) {
#        "Updating $filename"
#        $webConfigPath = Resolve-Path $filename
#        $config = New-Object XML 
#        $config.Load($webConfigPath) 
#        $ns = New-Object Xml.XmlNamespaceManager $config.NameTable
#        $ns.AddNamespace( "e", "urn:nhibernate-configuration-2.2" )
#        $config.SelectSingleNode("//e:property[@name = 'connection.connection_string'] ",$ns).innertext = $connectionString
#        $config.Save($webConfigPath) 
#} 


# By Dmitriy Yudin on 2/17/2012
# Converts folder into IIS Web Application
# For example: "C:\inetpub\wwwroot\BAMPlus\SecurityManager" will make SecurityManager web application 
function script:Convert-Folder-To-IIS-Application($iisFolderName){
	Import-Module WebAdministration
	ConvertTo-WebApplication $iisFolderName -ErrorAction SilentlyContinue
}

function script:Delete-Dir($dir){
    if(test-path $dir ){ remove-item $dir -Recurse -Force}
}

function script:Copy-Dir{
    param($source,$destination)
    & xcopy $source $destination /S /I /Y /Q
}

#function script:Load-Data{
#    param($loadDataTestAssembly,$connectionString)
# Update-NHibernateConfiguration "tests/hibernate.cfg.xml" $connectionString
# & "tests\tools\nunit\nunit-console.exe" /include=DataLoader tests\$loadDataTestAssembly
#
#}

function script:Update-WebConfigForProduction {
    param($filename)
    $xml = [xml](get-content $filename)
    $root = $xml.get_DocumentElement();
    $root."system.web".compilation.debug = "false"
    #$root."system.web".customErrors.mode = "RemoteOnly"
    $xml.Save($filename)
}

function script:Set-AppOffline {
    param($destination)
    if(test-path $destination\app_offline.htm.bak){
        rename-item "$destination\app_offline.htm.bak"  "$destination\app_offline.htm" | out-null}
}

function script:Set-AppOnline 
{
    param($destination)
    if(test-path $destination\app_offline.htm){
        rename-item "$destination\app_offline.htm" "$destination\app_offline.htm.bak" | out-null}

}