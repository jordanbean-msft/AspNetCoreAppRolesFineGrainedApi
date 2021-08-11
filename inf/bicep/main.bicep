param appName string
param region string
param environment string
@secure()
param sqlAADAdminName string
@secure()
param sqlAADAdminObjectId string

var longName = '${appName}-${region}-${environment}'

module userAssignedManagedIdentityModule 'userAssignedManagedIdentity.bicep' = {
  name: 'userAssignedManagedIdentityDeploy'
  params: {
    longName: longName
  }  
}

module sqlModule 'sql.bicep' = {
  name: 'sqlDeploy'
  params: {
    sqlAADAdminName: sqlAADAdminName
    sqlAADAdminObjectId: sqlAADAdminObjectId
    longName: longName
  }
}

module webModule 'web.bicep' = {
  name: 'webDeploy'
  params: {
    longName: longName
    userAssignedManagedIdentityName: userAssignedManagedIdentityModule.outputs.userAssignedManagedIdentityName
    keyVaultName: keyVaultModule.outputs.keyVaultName
    redisCacheConnectionStringSecretName: redisModule.outputs.redisCacheConnectionString
    sqlDatabaseConnectionStringSecretName: sqlModule.outputs.sqlDatabaseConnectionString
  }
}

module redisModule 'redisCache.bicep' = {
  name: 'redisDeploy'
  params: {
    longName: longName
  }
}

module keyVaultModule 'keyVault.bicep' = {
  name: 'keyVaultDeploy'
  params: {
    longName: longName
    redisCacheConnectionString: redisModule.outputs.redisCacheConnectionString
    sqlDatabaseConnectionString: sqlModule.outputs.sqlDatabaseConnectionString
    userAssignedManagedIdentityName: userAssignedManagedIdentityModule.outputs.userAssignedManagedIdentityName
  }  
}

output webAppName string = webModule.outputs.webAppName
output userAssignedManagedIdentityName string = userAssignedManagedIdentityModule.outputs.userAssignedManagedIdentityName
output sqlServerName string = sqlModule.outputs.sqlServerName
output sqlDatabaseName string = sqlModule.outputs.sqlDatabaseName
output redisCacheName string = redisModule.outputs.redisCacheName
output keyVaultName string = keyVaultModule.outputs.keyVaultName
