[CmdletBinding(SupportsShouldProcess=$true, ConfirmImpact='Medium')]
[Alias()]
[OutputType([int])]
Param
(
	# The path to the source directory. Default $Env:BUILD_SOURCESDIRECTORY is set by TFS.
	[Parameter(Mandatory=$false)]
	[ValidateNotNullOrEmpty()]
	[string]$SourceDirectory = $Env:BUILD_SOURCESDIRECTORY,

	# Major Part of Version Number
	[Parameter(Mandatory=$false)]
	[string]$Major,

	# Minor Part of Version Number
	[Parameter(Mandatory=$false)]
	[string]$Minor,

	# Build Part of Version Number
	[Parameter(Mandatory=$false)]
	[string]$Build,

	# Minor Part of Version Number
	[Parameter(Mandatory=$false)]
	[string]$Revision,

	# Set the version number also in all AppManifest.xml files.
	[Parameter(Mandatory=$false)]
	[switch]$SetVersion,

	[Parameter(Mandatory=$false)]
	[string]$VersionFormat = "\d+\.\d+\.\d+\.\d+"
)

function Set-AssemblyVersion
{
	[CmdletBinding(SupportsShouldProcess=$true, ConfirmImpact='Medium')]
	[Alias()]
	[OutputType([int])]
	Param
	(
		# The path to the source directory.
		[Parameter(Mandatory=$false)]
		[ValidateNotNullOrEmpty()]
		[string]$SourceDirectory ,

		# Major Part of Version Number
		[Parameter(Mandatory=$false)]
		[string]$Major,

		# Minor Part of Version Number
		[Parameter(Mandatory=$false)]
		[string]$Minor,

		# Build Part of Version Number
		[Parameter(Mandatory=$false)]
		[string]$Build,

		# Minor Part of Version Number
		[Parameter(Mandatory=$false)]
		[string]$Revision,

		# Set the version number also in all AppManifest.xml files.
		[Parameter(Mandatory=$false)]
		[switch]$SetVersion,

		[Parameter(Mandatory=$false)]
		[string]$VersionFormat
	)

	if (-not (Test-Path $SourceDirectory)) {
		throw "The directory '$SourceDirectory' does not exist."
	}

	$files = Get-Files -SourceDirectory $SourceDirectory
	$appFiles = Get-AppManifest -SourceDirectory $SourceDirectory


	if (-not $Env:BUILD_BUILDNUMBER)
	{
		[System.Version] $Version = Get-VersionFromAppManifest ($appFiles | Select-Object -First 1)
	}
	else
	{
		[System.Version] $Version = [System.Version]$Env:BUILD_BUILDNUMBER
	}

	[System.Version] $UpdatedVersion = Create-NewVersionNumber -AppManifestVersion $Version -Major $Major -Minor $Minor -Build $Build -Revision $Revision

	if(-not $UpdatedVersion){
		 throw "Can not created an updated version object"
	}

	Write-Host "Update BUILD_BUILDNUMBER to $($UpdatedVersion.ToString())"
	Write-Host "BUILD_NUMBER=$($UpdatedVersion.ToString())" >> $GITHUB_ENV

	if($SetVersion.IsPresent){
		Set-FileContent -Files $files -Version $UpdatedVersion -VersionFormat $VersionFormat
		Set-AppManifest -Files $appFiles -Version $UpdatedVersion
	}  
}

function Create-NewVersionNumber{
	[CmdletBinding(SupportsShouldProcess=$true, ConfirmImpact='Medium')]
	[Alias()]
	[OutputType([System.Version])]
	Param
	(
		# Version number from AppManifest.xml
		[Parameter(Mandatory=$false)]
		[ValidateNotNullOrEmpty()]
		[System.Version]$AppManifestVersion,

		# Major Part of Version Number
		[Parameter(Mandatory=$false)]
		[string]$Major,

		# Minor Part of Version Number
		[Parameter(Mandatory=$false)]
		[string]$Minor,

		# Build Part of Version Number
		[Parameter(Mandatory=$false)]
		[string]$Build,

		# Minor Part of Version Number
		[Parameter(Mandatory=$false)]
		[string]$Revision
	)
	
	$majorNumber = $AppManifestVersion.Major
	if(-not [string]::IsNullOrEmpty($Major)){
		$majorNumber = $Major
	}

	$minorNumber = $AppManifestVersion.Minor
	if(-not [string]::IsNullOrEmpty($Minor)){
		$minorNumber = $Minor
	}

	$buildNumber = $AppManifestVersion.Build
	if(-not [string]::IsNullOrEmpty($Build)){
		$buildNumber = $Build.Replace("C","")
	}

	$revisionNumber = $AppManifestVersion.Revision
	if(-not [string]::IsNullOrEmpty($Revision)){
		$revisionNumber = $Revision
	}

	$newVersion = New-Object -TypeName System.Version -ArgumentList $majorNumber,$minorNumber,$buildNumber,$revisionNumber
	return $newVersion
}

function Get-VersionFromAppManifest
{
	[CmdletBinding()]
	[Alias()]
	[OutputType([System.Version])]
	Param
	(
		[Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[System.IO.FileSystemInfo]$file
	)

	if(-not $file){
		throw "Could not find AppManifest.xml"
	}

	[xml]$xml = Get-Content $file.FullName
	$version = [System.Version]::Parse($xml.Package.Identity.Version)

	return $version
}

function Get-Files
{
	[CmdletBinding()]
	[Alias()]
	[OutputType([System.IO.FileSystemInfo[]])]
	Param
	(
	   [Parameter(Mandatory=$true)]
	   [ValidateNotNullOrEmpty()]
	   [string]$SourceDirectory
	)

	$folders = Get-ChildItem $SourceDirectory -Recurse -Include "*Properties*" | Where-Object { $_.PSIsContainer }

	return $folders | ForEach-Object { Get-ChildItem -Path $_.FullName -Recurse -include AssemblyInfo.* }
}

function Get-AppManifest
{
	[CmdletBinding()]
	[Alias()]
	[OutputType([System.IO.FileSystemInfo[]])]
	Param
	(
		[Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[string]$SourceDirectory
	)

	return Get-ChildItem -Path $SourceDirectory -Recurse -Filter "Package.appxmanifest"
}

function Set-FileContent
{
	[CmdletBinding(SupportsShouldProcess=$true, ConfirmImpact='Medium')]
	[OutputType([int])]
	Param
	(
		[Parameter(Mandatory=$true, Position=0)]
		[System.IO.FileSystemInfo[]]$Files,

		[Parameter(Mandatory=$true, Position=1)]
		[string]$Version,

		[Parameter(Mandatory=$true, Position=2)]
		[string]$VersionFormat
	)

	foreach ($file in $Files) 
	{
		$filecontent = Get-Content $file

		if ($PSCmdlet.ShouldProcess("$file", "Set-AssemblyVersion"))
		{
			attrib $file -r
			$filecontent -replace $VersionFormat, $Version | Out-File $file
			Write-Host "Applied Version '$Version' $($file.FullName) - version applied"
		}
	}
}

function Set-AppManifest()
{
	Param
	(
		[Parameter(Mandatory=$false)]
		[System.IO.FileSystemInfo[]]$Files,

		[Parameter(Mandatory=$true)]
		[System.Version]$Version
	)

	if(-not $Files)
	{
		Write-Host "No Mainfest files to update"
		return;
	}

	foreach ($file in $Files)
	{
		[xml]$xml = Get-Content $file.FullName

		$xml.Package.Identity.Version = $Version.ToString()

		if ($PSCmdlet.ShouldProcess("$file", "Set-AppManifest")){
			$xml.Save($file.FullName)
		}
	}
}

if (-not ($myinvocation.line.Contains("`$here\`$sut"))) {

	Write-Host "Source Directory: $SourceDirectory"
	Write-Host "Major: $Major Minor: $Minor Build: $Build Revision: $Revision"
	Write-Host "VersionFormat: $VersionFormat"
	Write-Host "SetVersion: $SetVersion"

	Set-AssemblyVersion -SourceDirectory $SourceDirectory -Major $Major -Minor $Minor -Build $Build -Revision $Revision -VersionFormat $VersionFormat -SetVersion:$SetVersion
}  