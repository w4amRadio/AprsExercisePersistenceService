Function TarDeploymentFolder ([string]$sevenZipPath, [string]$deploymentFolderPath, [string]$destination)
{
	#$zipResult = & "$sevenZipPath" a -t{tar} "${deploymentFolderPath}" "${destination}";
    $zipResult = & "$sevenZipPath" a -t tar "${deploymentFolderPath}" "${destination}";
	return $zipResult;
}

Set-Location ..\..
$projectDirectory = Get-Location

echo $projectDirectory

#-- Compress File or Folder Here --# 
#TarDeploymentFolder -$sevenZipPath "C:\Program Files\7-Zip\7z.exe" -$deploymentFolderPath $projectDirectory"\bin\Debug\net5.0\linux-arm" -$destination $projectDirectory"\bin\Debug\net5.0\aprs_persist.tar";
TarDeploymentFolder "C:\Program Files\7-Zip\7z.exe" $projectDirectory"\bin\Debug\net5.0\linux-arm" $projectDirectory"\bin\Debug\net5.0\aprs_persist.tar";
#----------------------------------