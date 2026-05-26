param location string = 'southeastasia'
param adminUsername string = 'Rookies2026Batch9Net1'

@secure()
param adminPassword string

param sqlServerName string = 'Rookies2026Batch9Net1'
param appServicePlanName string = 'Rookies2026Batch9Net1'

param webAppNames array = [
  'Rookies2026Batch9Net1-DEV'
  'Rookies2026Batch9Net1-DEV-API'
  'Rookies2026Batch9Net1-QC'
  'Rookies2026Batch9Net1-QC-API'
]

param dbNames array = [
  'Rookies2026Batch9Net1-DEV'
  'Rookies2026Batch9Net1-QC'
]

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2023-08-01' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: adminUsername
    administratorLoginPassword: adminPassword
    version: '12.0'
  }
}

// Firewall rule to allow Azure services
resource firewallRule 'Microsoft.Sql/servers/firewallRules@2023-08-01' = {
  name: 'AllowAllAzureIPs'
  parent: sqlServer
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// SQL Databases
resource sqlDatabases 'Microsoft.Sql/servers/databases@2023-08-01' = [for dbName in dbNames: {
  name: dbName
  parent: sqlServer
  location: location
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648
    requestedBackupStorageRedundancy: 'Local'
  }
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
}]

// Windows App Service Plan (D1)
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'D1'
    tier: 'Shared'
  }
  kind: 'app' // Windows
  properties: {
    reserved: false
  }
}

// Web Apps using .NET 8 on Windows
resource webApps 'Microsoft.Web/sites@2023-01-01' = [for name in webAppNames: {
  name: name
  location: location
  kind: 'app' // Windows
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      windowsFxVersion: 'DOTNET|10.0'
    }
  }
}]

// Output website URLs
output websiteUrls array = [for (name, i) in webAppNames: {
  name: name
  url: 'https://${webApps[i].properties.defaultHostName}'
}]

					  
					 
			 
