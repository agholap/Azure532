#
# AzureRMTables.ps1
#
## Global
$ResourceGroupName = "ToBeDeletedfeb22"
$Location = "westus"

## Storage
$StorageName = "generalvmstoragefeb22"
$StorageType = "Standard_GRS"

# Resource Group
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location


# Storage
$StorageAccount = New-AzureRmStorageAccount -ResourceGroupName $ResourceGroupName -Name $StorageName -Type $StorageType -Location $Location

#container

$ContainerName = "quickstartcontainer"
New-AzureStorageContainer -Name $ContainerName -Context $StorageAccount.Context -Permission Blob
 
Set-AzureStorageBlobContent -File "C:\ToUpload\Pic1.jpeg"    `
-Container $ContainerName  `
-Blob "Pic1.jpeg"  `
-Context $StorageAccount.Context

Set-AzureStorageBlobContent -File "C:\ToUpload\Pic2.jpeg"   `
-Blob "Pic2.jpeg"  `
-Container $ContainerName  `
-Context $StorageAccount.Context

#get all blobs
Get-AzureStorageBlob -Container $ContainerName -Context $StorageAccount.Context | select Name

# download the blob
Get-AzureStorageBlobContent -Blob "Pic1.jpeg"   `
-Container $ContainerName  `
-Destination "C:\FromAzure\"  `
-Context $StorageAccount.Context

Get-AzureStorageBlobContent -Blob "Pic2.jpeg"    `
-Container $ContainerName  `
-Destination "C:\FromAzure\"  `
-Context $StorageAccount.Context