[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $appName,
    [Parameter()]
    [string]
    $region,
    [Parameter()]
    [string]
    $environment
)

$ErrorActionPreference = "Stop"

Import-Module AzureAD

$appRegistration = Get-AzureADApplication -Filter "DisplayName eq '$appName'"

$replyUrls = New-Object System.Collections.Generic.List[string]

$replyUrls.Add("https://wa-$appName-$region-$environment/signin-oidc")

$managerAppRole = [Microsoft.Open.AzureAD.Model.AppRole]::new()
$managerAppRole.AllowedMemberTypes = New-Object System.Collections.Generic.List[string]
$managerAppRole.AllowedMemberTypes.Add("User")
$managerAppRole.DisplayName = "Regional Manager"
$managerAppRole.Description = "User Role"
$managerAppRole.Value = "Regional Manager"
$managerAppRole.Id = [Guid]::NewGuid().ToString()
$managerAppRole.IsEnabled = $true

$salespersonAppRole = [Microsoft.Open.AzureAD.Model.AppRole]::new()
$salespersonAppRole.AllowedMemberTypes = New-Object System.Collections.Generic.List[string]
$salespersonAppRole.AllowedMemberTypes.Add("User")
$salespersonAppRole.DisplayName = "Salesperson"
$salespersonAppRole.Description = "User Role"
$salespersonAppRole.Value = "Salesperson"
$salespersonAppRole.Id = [Guid]::NewGuid().ToString()
$salespersonAppRole.IsEnabled = $true

$newAppRoles = New-Object -TypeName System.Collections.Generic.List[Microsoft.Open.AzureAD.Model.AppRole]

$newAppRoles.Add($managerAppRole)
$newAppRoles.Add($salespersonAppRole)

if(!$appRegistration) {
  $appRegistration = New-AzureADApplication -DisplayName $appName `
                                            -IdentifierUris "https://wa-$appName-$region-$environment" `
                                            -ReplyUrls $replyUrls `
                                            -AppRoles $newAppRoles `
                                            -GroupMembershipClaims 'SecurityGroup' ` #get group membership in JWT token
                                            -OAuth2AllowIdTokenImplicitFlow: true `
                                            -Verbose
} else {
  $tempAppRoles = New-Object -TypeName System.Collections.Generic.List[Microsoft.Open.AzureAD.Model.AppRole]

  $existingAppRoles = $appRegistration.AppRoles
  
  $newAppRoles | Where-Object ${$_.DisplayName -notin $existingAppRoles.DisplayName } | ForEach-Object { $tempAppRoles.Add($_) }

  $appRegistration = Set-AzureADApplication -ObjectId $appRegistration.ObjectId `
                                            -DisplayName $appName `
                                            -IdentifierUris "https://wa-$appName-$region-$environment" `
                                            -ReplyUrls $replyUrls `
                                            -AppRoles $tempAppRoles `
                                            -GroupMembershipClaims 'SecurityGroup' ` #get group membership in JWT token
                                            -OAuth2AllowIdTokenImplicitFlow: true `
                                            -Verbose
}
