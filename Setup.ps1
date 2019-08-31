
Write-Output "Setting up solution workspace..."
$cwd = Split-Path -Path $MyInvocation.MyCommand.Path

Write-Debug $cwd
Set-Location -Path $cwd

New-Item -ItemType Directory -Path "Lib" -Force

$GameDataDir = Read-Host -Prompt "Path to OxygenNotIncluded_Data"
$GameDataName = Split-Path -Path $GameDataDir -Leaf -Resolve

Write-Debug $GameDataDir
Write-Debug $GameDataName

If ((Test-Path $GameDataDir -PathType Container) -AND $GameDataName -eq "OxygenNotIncluded_Data") {
	Write-Output "Linking Data..."

	New-Item -ItemType SymbolicLink -Path "Lib\Data" -Target $GameDataDir -Force

	Write-Output "Done."
} Else {
	Write-Output "Path does not appear to be valid. Check it exists and the name is correct."
	Exit 1
}

$GameDocsDir = Read-Host -Prompt "Path to Documents\OxygenNotIncluded"
$GameDocsName = Split-Path -Path $GameDocsDir -Leaf -Resolve

Write-Debug $GameDocsDir
Write-Debug $GameDocsName

If ((Test-Path $GameDocsDir -PathType Container) -AND $GameDocsName -eq "OxygenNotIncluded") {
	Write-Output "Linking mods..."

	New-Item -ItemType SymbolicLink -Path "Lib\Docs" -Target $GameDocsDir -Force

	Write-Output "Done."
} Else {
	Write-Output "Path does not appear to be valid. Check it exists and the name is correct."
	Exit 1
}

pause
