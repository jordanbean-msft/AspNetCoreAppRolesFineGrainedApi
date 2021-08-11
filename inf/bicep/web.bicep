param longName string
param userAssignedManagedIdentityName string
param keyVaultName string
param redisCacheConnectionStringSecretName string
param sqlDatabaseConnectionStringSecretName string

resource userAssignedManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' existing = {
  name: userAssignedManagedIdentityName
}

resource webApp 'Microsoft.Web/sites@2021-01-15' = {
  name: 'wa-${longName}'
  location: resourceGroup().location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${userAssignedManagedIdentity.id}': {}
    }
  }
  properties: {
    siteConfig: {
      connectionStrings: [
        {
          name: 'redisCacheConnectionString'
          connectionString: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=${redisCacheConnectionStringSecretName})'
        }
        {
          name: 'sqlDatabaseConnectionString'
          connectionString: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=${sqlDatabaseConnectionStringSecretName})'
        }
      ]
    }
  }

}

output webAppName string = webApp.name
