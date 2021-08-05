param longName string

resource webAppManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'mi-${longName}'
  location: resourceGroup().location  
}

output userAssignedManagedIdentityName string = webAppManagedIdentity.name
