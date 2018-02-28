# Variables    
## Global
$ResourceGroupName = "ToBeDeletedFeb1"
$Location = "westus"

## Storage
$StorageName = "generalvmstoragefeb1"
$StorageType = "Standard_GRS"

## Network
$InterfaceName = "ServerInterface01"
$InterfaceName1 = "ServerInterface02"
$InterfaceName2 = "ServerInterface03"

$SubnetVMName = "SubnetVM"
$SubnetLBName = "SubnetLB"

$VNetName = "VNetAvLB"
$VNetAddressPrefix = "10.0.0.0/16"
$VNetSubnetVMAddressPrefix = "10.0.0.0/24"
$VNetSubnetLBAddressPrefix = "10.0.2.0/24"

$PrivateIPInterface1 = "10.0.2.6"
$PrivateIPInterface2 = "10.0.2.7"
$PrivateIPInterface3 = "10.0.2.8"

$LBFrontEnd = "LBFrontend"
$LBBackEnd = "LBBackEnd"

$NATRule1 = "RDP1"
$NATRule2 = "RDP2"
$NATRule3 = "RDP3"
$HealthProbeName = "HealthProbe"
## Compute
$VM1Name = "VM1"
$VM2Name = "VM2"
$VM3Name = "VM3"

$VMSize = "Standard_A1"
$OSDisk1Name = $VM1Name + "OSDisk"
$OSDisk2Name = $VM2Name + "OSDisk"
$OSDisk3Name = $VM3Name + "OSDisk"
$AvailablitySetName = "AV1"
$LoadBalancerName = "AVLoadBalancer"

# Resource Group
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location

# Storage
$StorageAccount = New-AzureRmStorageAccount -ResourceGroupName $ResourceGroupName -Name $StorageName -Type $StorageType -Location $Location

# Network
$AV = New-AzureRmAvailabilitySet -Location $Location -Name $AvailablitySetName -ResourceGroupName $ResourceGroupName -PlatformFaultDomainCount 2 -PlatformUpdateDomainCount 5
$PIp = New-AzureRmPublicIpAddress -Name $InterfaceName -ResourceGroupName $ResourceGroupName -Location $Location -AllocationMethod Dynamic
	
	#Subnet for VM & LB
#$SubnetVMConfig = New-AzureRmVirtualNetworkSubnetConfig -Name $SubnetVMName -AddressPrefix $VNetSubnetVMAddressPrefix
$SubnetLBConfig = New-AzureRmVirtualNetworkSubnetConfig -Name $SubnetLBName -AddressPrefix $VNetSubnetLBAddressPrefix
	#Create Virtual Network with Subnet
$VNet = New-AzureRmVirtualNetwork -Name $VNetName -ResourceGroupName $ResourceGroupName -Location $Location -AddressPrefix $VNetAddressPrefix -Subnet $SubnetLBConfig
$frontendIP = New-AzureRmLoadBalancerFrontendIpConfig -Name LB-Frontend -PublicIpAddress $PIp
#$SubnetConfig = New-AzureRmVirtualNetworkSubnetConfig -Name $SubnetLB -AddressPrefix $VNetSubnetLBAddressPrefix
	#FrontEnd & Back End Address Pool - https://docs.microsoft.com/en-us/azure/load-balancer/load-balancer-get-started-internet-arm-ps
$frontendIP = New-AzureRmLoadBalancerFrontendIpConfig -Name $LBFrontEnd -PublicIpAddress $PIp
$backEndAddresspool = New-AzureRmLoadBalancerBackendAddressPoolConfig -Name $LBBackEnd
	#Create the NAT rules.
$inboundNATRule1= New-AzureRmLoadBalancerInboundNatRuleConfig -Name $NATRule1 -FrontendIpConfiguration $frontendIP -Protocol TCP -FrontendPort 3441 -BackendPort 3389
$inboundNATRule2= New-AzureRmLoadBalancerInboundNatRuleConfig -Name $NATRule2 -FrontendIpConfiguration $frontendIP -Protocol TCP -FrontendPort 3442 -BackendPort 3389
$inboundNATRule3= New-AzureRmLoadBalancerInboundNatRuleConfig -Name $NATRule3 -FrontendIpConfiguration $frontendIP -Protocol TCP -FrontendPort 3443 -BackendPort 3389

	#Create Health Rule
$healthProbe = New-AzureRmLoadBalancerProbeConfig -Name $HealthProbeName -RequestPath 'HealthProbe.aspx' -Protocol http -Port 80 -IntervalInSeconds 15 -ProbeCount 2
	#Create a load balancer rule.
	$lbrule = New-AzureRmLoadBalancerRuleConfig -Name HTTP -FrontendIpConfiguration $frontendIP -BackendAddressPool  $backEndAddresspool -Probe $healthProbe -Protocol Tcp -FrontendPort 80 -BackendPort 80
	#Create the load balancer 
	$NRPLB = New-AzureRmLoadBalancer -ResourceGroupName $ResourceGroupName -Name $LoadBalancerName -Location $Location -FrontendIpConfiguration $frontendIP -InboundNatRule $inboundNATRule1,$inboundNatRule2,$inboundNATRule3 -LoadBalancingRule $lbrule -BackendAddressPool $backEndAddresspool -Probe $healthProbe
	## Interface
#Get Subnet
$backendSubnet = Get-AzureRmVirtualNetworkSubnetConfig -Name $SubnetLBName -VirtualNetwork $VNet

$Interface =  New-AzureRmNetworkInterface -Name $InterfaceName -ResourceGroupName $ResourceGroupName -PrivateIpAddress $PrivateIPInterface1 -Location $Location -Subnet $backendSubnet -LoadBalancerBackendAddressPool $NRPLB.BackendAddressPools[0] -LoadBalancerInboundNatRule $NRPLB.InboundNatRules[0]
#$backendnic1= New-AzureRmNetworkInterface -Name lb-nic1-be     -ResourceGroupName NRP-RG  -Location 'West US' -PrivateIpAddress 10.0.2.6 -Subnet $backendSubnet -LoadBalancerBackendAddressPool $nrplb.BackendAddressPools[0] -LoadBalancerInboundNatRule $nrplb.InboundNatRules[0]
$Interface1 = New-AzureRmNetworkInterface -Name $InterfaceName1 -ResourceGroupName $ResourceGroupName -PrivateIpAddress $PrivateIPInterface2 -Location $Location -Subnet $backendSubnet -LoadBalancerBackendAddressPool $NRPLB.BackendAddressPools[0] -LoadBalancerInboundNatRule $NRPLB.InboundNatRules[1]
$Interface2 = New-AzureRmNetworkInterface -Name $InterfaceName2 -ResourceGroupName $ResourceGroupName -PrivateIpAddress $PrivateIPInterface3 -Location $Location -Subnet $backendSubnet -LoadBalancerBackendAddressPool $NRPLB.BackendAddressPools[0] -LoadBalancerInboundNatRule $NRPLB.InboundNatRules[2]
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
