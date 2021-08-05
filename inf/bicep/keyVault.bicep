param longName string
@secure()
param redisCacheConnectionString string
@secure()
param sqlDatabaseConnectionString string
param userAssignedManagedIdentityName string

resource userAssignedManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' existing = {
  name: userAssignedManagedIdentityName  
}

resource keyVault 'Microsoft.KeyVault/vaults@2021-04-01-preview' = {
  name: 'kv-${substring(longName, 0, 21)}'
  location: resourceGroup().location 
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: false
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: userAssignedManagedIdentity.properties.principalId
        permissions: {
          secrets: [
            'get'
          ]
        }
      }
    ]
  }
}

var redisCacheConnectionStringSecretName = 'redisCacheConnectionString'

resource keyVaultRedisCacheConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2021-04-01-preview' = {
  name: '${keyVault.name}/${redisCacheConnectionStringSecretName}'
  properties: {
    value: redisCacheConnectionString
  }  
}

var sqlDatabaseConnectionStringSecretName = 'sqlDatabaseConnectionString'

resource keyVaultSqlDatabaseConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2021-04-01-preview' = {
  name: '${keyVault.name}/${sqlDatabaseConnectionStringSecretName}'
  properties: {
    value: sqlDatabaseConnectionString
  }  
}

output keyVaultName string = keyVault.name
output keyVaultResourceId string = keyVault.id
output keyVaultRedisCacheConnectionStringSecretName string = redisCacheConnectionStringSecretName
output keyVaultSqlDatabaseConnectionStringSecretName string = sqlDatabaseConnectionStringSecretName
