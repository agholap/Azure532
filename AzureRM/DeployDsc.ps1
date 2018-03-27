#
# DeployDsc.ps1
#
$resourceGroup = 'ToBeDeletedFeb1'
$location = 'westus'
$vmName = 'VM2'
$storageName = 'generalvmstoragefeb1'

Publish-AzureRmVMDscConfiguration -ConfigurationPath .\InstallWebServer.ps1     `
-ResourceGroupName $resourceGroup     `
-StorageAccountName $storageName

Set-AzureRmVMDscExtension -Version 2.21     `
-ResourceGroupName $resourceGroup    `
-VMName $vmName    `
-ArchiveStorageAccountName $storageName     `
-ArchiveBlobName InstallWebServer.ps1.zip     `
-AutoUpdate:$true      `
-ConfigurationName IIS



