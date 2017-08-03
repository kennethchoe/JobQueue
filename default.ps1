# required parameters :
# 	$databaseName

properties {
	$projectName = "JobQueue"
    $unitTestAssembly = "UnitTest.dll"
    $integrationTestAssembly = "IntegrationTest.dll"

	$base_dir = resolve-path .\
	$source_dir = "$base_dir\src"
	$solutionPath = "$source_dir\$projectName.sln"

    $nunitPath = "$source_dir\packages\NUnit.ConsoleRunner.3.7.0\tools"

    $version        = GetEnvOrDefault "PackageVersion" "1.0.0.0"   
	$databaseName   = GetEnvOrDefault "databaseName"   "JobQueue_demo"
	$databaseServer = GetEnvOrDefault "databaseServer" ".\sqlexpress"
    $projectConfig  = GetEnvOrDefault "ProjectConfig"  "Release"
	
	$build_dir = "$base_dir\build"
	$test_dir = "$build_dir\test"
	$testCopyIgnorePath = "_ReSharper"
	
	$RoundhouseExe = "$base_dir\tools\roundhouse.0.8.5.0\rh.exe"
	$zipCommand = "$base_dir\tools\7zip\7za.exe"	
	
	$databaseScripts = "$source_dir\Database"
	
	$connection_string = "server=$databaseserver;database=$databasename;Integrated Security=true;"
	
}

Framework '4.5.2'
$ErrorActionPreference = 'Stop'

dir psake-modules\*.psm1 | Import-Module

task default -depends Init, CommonAssemblyInfo, Compile, DropDatabase, CreateOrUpgradeDatabase, Test

task Init {
    delete_file $package_file
    delete_directory $build_dir
    create_directory $test_dir
	create_directory $build_dir
}

task Compile -depends Init {
	exec { & "$base_dir\tools\nuget.exe" restore $solutionPath }
    exec { msbuild /t:clean /v:q /nologo /p:Configuration=$projectConfig $solutionPath }
    exec { msbuild /t:build /v:q /nologo /p:Configuration=$projectConfig $solutionPath }
}

task Test {
	copy_all_assemblies_for_test $test_dir
	exec {
		& $nunitPath\nunit3-console $test_dir\$unitTestAssembly $test_dir\$integrationTestAssembly --work=$build_dir
	}
}

task DropDatabase {
    exec { & $RoundhouseExe /d="$databaseName" /s="$databaseServer" --drop --silent }
}

task CreateOrUpgradeDatabase {
    exec { & $RoundhouseExe /d="$databaseName" /s="$databaseServer" /f="$databaseScripts" --silent }
}

task CommonAssemblyInfo {
    Update-AllAssemblyInfoFiles $version src
}

function global:Copy_and_flatten ($source,$filter,$dest) {
  ls $source -filter $filter  -r | Where-Object{!$_.FullName.Contains("$testCopyIgnorePath") -and !$_.FullName.Contains("packages") }| cp -dest $dest -force
}

function global:copy_all_assemblies_for_test($destination){
  create_directory $destination
  Copy_and_flatten $source_dir *.exe $destination
  Copy_and_flatten $source_dir *.dll $destination
  Copy_and_flatten $source_dir *.config $destination
  Copy_and_flatten $source_dir *.xml $destination
  Copy_and_flatten $source_dir *.pdb $destination
  Copy_and_flatten $source_dir *.sql $destination
  Copy_and_flatten $source_dir *.xlsx $destination
}

function global:delete_file($file) {
    if($file) { remove-item $file -force -ErrorAction SilentlyContinue | out-null } 
}

function global:delete_directory($directory_name)
{
  rd $directory_name -recurse -force  -ErrorAction SilentlyContinue | out-null
}

function global:create_directory($directory_name)
{
  mkdir $directory_name  -ErrorAction SilentlyContinue  | out-null
}

function GetEnvOrDefault($name, $defaultValue)
{
	$value = (get-childitem -path Env: | where-object { $_.Name -eq $name }).Value
	if($value -ne $null) {
		return $value
	} else {
		return $defaultValue
	}
}