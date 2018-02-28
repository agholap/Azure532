#
# AzureRedisCache.ps1
#
## Global
$ResourceGroupName = "ToBeDeletedfeb"
$Location = "westus"

## Storage
$RedisCacheName = "ToBeDeletedCache"
# Resource Group
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location

New-AzureRmRedisCache -Name $RedisCacheName -Location $Location -ResourceGroupName $ResourceGroupName -Sku Basic -Size C0