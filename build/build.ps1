Framework "4.0"

$env:Path += ";${Env:ProgramFiles(x86)}\Microsoft Visual Studio 12.0\Common7\IDE;${Env:ProgramFiles(x86)}\Microsoft Visual Studio 12.0\Common7\Tools;${Env:ProgramFiles(x86)}\Microsoft Visual Studio 11.0\Common7\IDE;${Env:ProgramFiles(x86)}\Microsoft Visual Studio 11.0\Common7\Tools;$env:WINDIR\Microsoft.NET\Framework\v4.0.30319;$env:WINDIR\Microsoft.NET\Framework\v2.0.50727"

properties {
	$base_dir = resolve-path .\..
	$build_dir = "$base_dir\build"
	$source_dir = "$base_dir\src"
	$build_artifacts_dir = "$base_dir\build_artifacts"
	$tools_dir = "$base_dir\tools"
	$test_dir = "$build_artifacts_dir\tests"
    $nuget_dir = "$build_dir\nuget"
    $nuget_packaging_dir = "$build_artifacts_dir\nuget"

    $nuspec_file = "$nuget_packaging_dir\RestClient.nuspec"

    $global:build_configuration = "Debug"

	$json = Get-Content $base_dir\semver.json -Raw | ConvertFrom-Json
    $semver = $json.major.ToString() + "." + $json.minor + "." + $json.patch
    $revision = if($revision) { $revision } else { 0 }
    $build_number = "$semver+$revision"
    $assembly_version = $json.major.ToString() + "." + $json.minor + ".0.0"
    $assembly_file_version = ($semver -replace "\-.*$", "") + ".$revision"
}

task default -depends compile, test

task dist -depends mark_release, create_common_assembly_info, compile, undo_commom_assembly_info, test, create_package

task mark_release {
    $global:build_configuration = "Release"
}

task clean {
	rd $build_artifacts_dir -recurse -force  -ErrorAction SilentlyContinue | out-null
    rd $source_dir\packages -recurse -force  -ErrorAction SilentlyContinue | out-null
	mkdir $build_artifacts_dir  -ErrorAction SilentlyContinue  | out-null
}

task compile -depends clean {
    exec { & $tools_dir\nuget\nuget.exe restore $source_dir\RestClient.sln }
	exec { msbuild  $source_dir\RestClient.sln /t:Clean /t:Build /p:Configuration=$build_configuration /v:m /nologo }
}

task test {
    exec { & $tools_dir\xunit\xunit.console.clr4.exe $test_dir\$build_configuration\Rest.Client.Tests.dll /xml $test_dir\tests_results.xml }
}

task create_common_assembly_info {
	$date = Get-Date
	$asmInfo = "using System.Reflection;

[assembly: AssemblyVersionAttribute(""$assembly_version"")]
[assembly: AssemblyFileVersionAttribute(""$assembly_file_version"")]
[assembly: AssemblyCopyrightAttribute(""Copyright Gøran Sveia Kvarv 2011-" + $date.Year + """)]
[assembly: AssemblyProductAttribute(""RestClient"")]
[assembly: AssemblyTrademarkAttribute(""RestClient"")]
[assembly: AssemblyConfigurationAttribute(""$build_configuration"")]
[assembly: AssemblyInformationalVersionAttribute(""$assembly_version"")]"

	$asmInfo | out-file "$source_dir\CommonAssemblyInfo.cs" -encoding utf8
}

task undo_commom_assembly_info -precondition  { $is_local_build } {
    exec { & git checkout $source_dir\CommonAssemblyInfo.cs }
}

task create_package {
    cp $nuget_dir $nuget_packaging_dir -recurse
    create_nuspec $nuspec_file

    robocopy "$build_artifacts_dir\RestClient\$build_configuration\" "$nuget_packaging_dir\lib\portable-net4+netcore45+wpa81+MonoAndroid1+MonoTouch1" /S /XL

    exec { & $tools_dir\nuget\nuget.exe pack $nuspec_file -OutputDirectory $nuget_packaging_dir}
}

function create_nuspec($nuspec){
    $nuspec = get-item $nuspec

    $xml = New-Object XML
    $xml.Load($nuspec)

    $xml.package.metadata.version = "$semver"

    $xml.Save($nuspec)
}
