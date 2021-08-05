param appName string
param region string
param environment string
@secure()
param sqlAADAdminName string
@secure()
param sqlAADAdminObjectId string

var longName = '${appName}-${region}-${environment}'

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
  }
}

output webAppName string = webModule.outputs.webAppName
output webAppManagedIdentityName string = webModule.outputs.webAppManagedIdentityName
output sqlServerName string = sqlModule.outputs.sqlServerName
output sqlDatabaseName string = sqlModule.outputs.sqlDatabaseName
