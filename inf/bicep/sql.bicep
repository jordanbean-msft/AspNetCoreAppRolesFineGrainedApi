param longName string
@secure()
param sqlAADAdminName string
@secure()
param sqlAADAdminObjectId string

resource sqlServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: 'sqls-${longName}'
  location: resourceGroup().location
  properties: {
    administrators: {
      login: sqlAADAdminName
      sid: sqlAADAdminObjectId
      tenantId: subscription().tenantId
      principalType: 'User'
      azureADOnlyAuthentication: true
    }
  } 
}

resource sql 'Microsoft.Sql/servers/databases@2021-02-01-preview' = {
  name: '${sqlServer.name}/sql-${longName}'
  location: resourceGroup().location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  properties: {    
  }
}

output sqlServerName string = sqlServer.name
output sqlDatabaseName string = sql.name
output sqlDatabaseConnectionString string = 'Server=tcp:${reference(sqlServer.id).fullyQualifiedDomainName};Initial Catalog=${sql.name};Authentication=Active Directory Managed Identity;'
