#
# AzureRMTables.ps1
#
## Global
$ResourceGroupName = "ToBeDeletedfeb191"
$Location = "westus"

## Storage
$StorageName = "generalvmstoragefeb191"
$StorageType = "Standard_GRS"

# Resource Group
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location

# Storage
$StorageAccount = New-AzureRmStorageAccount -ResourceGroupName $ResourceGroupName -Name $StorageName -Type $StorageType -Location $Location

#Manage the access keys
$storageAccountKey = `
    (Get-AzureRmStorageAccountKey `
    -ResourceGroupName $ResourceGroupName `
    -Name $StorageName).Value[0]

#regenerate the key
New-AzureRmStorageAccountKey -ResourceGroupName $ResourceGroupName `
  -Name $StorageName `
  -KeyName key1 

  $tableName = "Customers"

  #create azure table - if error System.Spatial, Version=5.8.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35- 
  #Install-Module AzureRmStorageTable -  Install-Module AzureRm.Storage -AllowClobber -Force and open the window again
  $customer = New-AzureStorageTable -Name $tableName -Context $StorageAccount.Context 

  #create partition and rows
  $partition1 = "partition1"
  $partition2 = "partition2"

  Add-StorageTableRow -table $customer -partitionKey $partition1 -rowKey ("CA") -property @{"username"="Jason";"userid" = 1}

  Add-StorageTableRow -table $customer -partitionKey $partition2 -rowKey("MA") -property @{"username"="Mark";"userid"=2}

  Add-StorageTableRow -table $customer -partitionKey $partition1 -rowKey("WA") -property @{"username"="Justin";"userid"=3}

  #view all data
  Get-AzureStorageTableRowAll -table $customer | ft



