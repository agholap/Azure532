# Variables    
## Global
$ResourceGroupName = "ToBeDeletedFeb19"
$Location = "westus"

## Storage
$StorageName = "generalvmstorageq"
$StorageType = "Standard_GRS"

## Network
$InterfaceName = "ServerInterface07"
$InterfaceName1 = "ServerInterface08"
$InterfaceName2 = "ServerInterface09"
$Subnet1Name = "Subnet2"
$VNetName = "VNet10"
$VNetAddressPrefix = "10.0.0.0/16"
$VNetSubnetAddressPrefix = "10.0.0.0/24"
## Compute
$VM1Name = "VM1"
$VM2Name = "VM2"
$VM3Name = "VM3"

$VMSize = "Basic_A0"
$OSDisk1Name = $VM1Name + "OSDisk"
$OSDisk2Name = $VM2Name + "OSDisk"
$OSDisk3Name = $VM3Name + "OSDisk"
$AvailablitySetName = "AV1"

# Resource Group
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location

# Storage
$StorageAccount = New-AzureRmStorageAccount -ResourceGroupName $ResourceGroupName -Name $StorageName -Type $StorageType -Location $Location

# Network
$AV = New-AzureRmAvailabilitySet -Location $Location -Name $AvailablitySetName -ResourceGroupName $ResourceGroupName -PlatformFaultDomainCount 2 -PlatformUpdateDomainCount 5
$PIp = New-AzureRmPublicIpAddress -Name $InterfaceName -ResourceGroupName $ResourceGroupName -Location $Location -AllocationMethod Dynamic
$SubnetConfig = New-AzureRmVirtualNetworkSubnetConfig -Name $Subnet1Name -AddressPrefix $VNetSubnetAddressPrefix
$VNet = New-AzureRmVirtualNetwork -Name $VNetName -ResourceGroupName $ResourceGroupName -Location $Location -AddressPrefix $VNetAddressPrefix -Subnet $SubnetConfig
## Interface
$Interface = New-AzureRmNetworkInterface -Name $InterfaceName -ResourceGroupName $ResourceGroupName -Location $Location -SubnetId $VNet.Subnets[0].Id -PublicIpAddressId $PIp.Id
$Interface1 = New-AzureRmNetworkInterface -Name $InterfaceName1 -ResourceGroupName $ResourceGroupName -Location $Location -SubnetId $VNet.Subnets[0].Id 
$Interface2 = New-AzureRmNetworkInterface -Name $InterfaceName2 -ResourceGroupName $ResourceGroupName -Location $Location -SubnetId $VNet.Subnets[0].Id 

# Compute
## Setup local VM object
$Credential = Get-Credential


## Create the VM1 in Azure
$VirtualMachine1 = New-AzureRmVMConfig -VMName $VM1Name -VMSize $VMSize -AvailabilitySetId $AV.Id
$VirtualMachine1 = Set-AzureRmVMOperatingSystem -VM $VirtualMachine1 -Windows -ComputerName $VM1Name -Credential $Credential -ProvisionVMAgent -EnableAutoUpdate
$VirtualMachine1 = Set-AzureRmVMSourceImage -VM $VirtualMachine1 -PublisherName MicrosoftWindowsServer -Offer WindowsServer -Skus 2012-R2-Datacenter -Version "latest"
$VirtualMachine1 = Add-AzureRmVMNetworkInterface -VM $VirtualMachine1 -Id $Interface.Id
$OSDiskUri = $StorageAccount.PrimaryEndpoints.Blob.ToString() + "vhds/" + $OSDisk1Name+$VM1Name + ".vhd"
$VirtualMachine1 = Set-AzureRmVMOSDisk -VM $VirtualMachine1 -Name $OSDisk1Name -VhdUri $OSDiskUri -CreateOption FromImage
New-AzureRmVM -ResourceGroupName $ResourceGroupName -Location $Location -VM $VirtualMachine1 

## Create VM2 in Azure
$VirtualMachine2 = New-AzureRmVMConfig -VMName $VM2Name -VMSize $VMSize -AvailabilitySetId $AV.Id
$VirtualMachine2 = Set-AzureRmVMOperatingSystem -VM $VirtualMachine2 -Windows -ComputerName $VM2Name -Credential $Credential -ProvisionVMAgent -EnableAutoUpdate
$VirtualMachine2 = Set-AzureRmVMSourceImage -VM $VirtualMachine2 -PublisherName MicrosoftWindowsServer -Offer WindowsServer -Skus 2012-R2-Datacenter -Version "latest"
$VirtualMachine2 = Add-AzureRmVMNetworkInterface -VM $VirtualMachine2 -Id $Interface1.Id
$OSDiskUri = $StorageAccount.PrimaryEndpoints.Blob.ToString() + "vhds/" + $OSDisk2Name+ $VM2Name + ".vhd"
$VirtualMachine2 = Set-AzureRmVMOSDisk -VM $VirtualMachine2 -Name $OSDisk2Name -VhdUri $OSDiskUri -CreateOption FromImage
New-AzureRmVM -ResourceGroupName $ResourceGroupName -Location $Location -VM $VirtualMachine2

## Create VM3 in Azure
$VirtualMachine3 = New-AzureRmVMConfig -VMName $VM3Name -VMSize $VMSize -AvailabilitySetId $AV.Id
$VirtualMachine3 = Set-AzureRmVMOperatingSystem -VM $VirtualMachine3 -Windows -ComputerName $VM3Name -Credential $Credential -ProvisionVMAgent -EnableAutoUpdate
$VirtualMachine3 = Set-AzureRmVMSourceImage -VM $VirtualMachine3 -PublisherName MicrosoftWindowsServer -Offer WindowsServer -Skus 2012-R2-Datacenter -Version "latest"
$VirtualMachine3 = Add-AzureRmVMNetworkInterface -VM $VirtualMachine3 -Id $Interface2.Id
$OSDiskUri = $StorageAccount.PrimaryEndpoints.Blob.ToString() + "vhds/" + $OSDisk3Name+ $VM3Name + ".vhd"
$VirtualMachine2 = Set-AzureRmVMOSDisk -VM $VirtualMachine3 -Name $OSDisk3Name -VhdUri $OSDiskUri -CreateOption FromImage
New-AzureRmVM -ResourceGroupName $ResourceGroupName -Location $Location -VM $VirtualMachine3

#

