param longName string

resource webAppManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'mi-${longName}'
  location: resourceGroup().location  
}

resource webApp 'Microsoft.Web/sites@2021-01-15' = {
  name: 'wa-${longName}'
  location: resourceGroup().location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${webAppManagedIdentity.id}': {}
    }
  }
  properties: {    
  }
}

output webAppName string = webApp.name
output webAppManagedIdentityName string = webAppManagedIdentity.name
