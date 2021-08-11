# AspNetCoreWithAppRolesAndFineGrained

This demo use AAD App Roles & also provides fine-grained access control to data using Resource-based authorization.

## Disclaimer

**THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.**

## Rules

In this example, we want to provide fine-grained access control using Azure Active Directory App Roles & Groups. This way, we don't have to maintain the list of users and their roles in our application. Instead, we can keep our application focused on "role-based access".

![rules](.img/rules.png)

In this example, there are 2 branches of the company & a corporate office. The following rules should apply:

- Each person should be to read their own salary data
- No person should be able to modify their own salary data
- The regional manager of a branch should be able to read & write all salary data for their branch only
- The CFO should be able to read & write all salary data for all employees (except themselves, of course)

## Azure Active Directory configuration

To use this example, you will need to configure Azure Active Directory

### Create App Registration & configure

1.  Navigate to the Azure Active Directory blade in the [Azure portal](https://portal.azure.com)

1.  Click on the **App Registrations** blade

1.  Click **New registration**

1.  Assign a **Name**, **Accounts in this organizational directory only** & the **Redirect URI**

    > Note: You can assign this later if you don't know the redirect URI. If running locally, the default is **https://localhost:5001/signin-oidc**

1.  On the **Overview** blade, copy the following information to Notepad so you can use it later:

    - **Application (client) ID**
    - **Directory (tenant) ID**

1.  On the **Authentication** blade, select the **Implicit grant and hybrid flows->ID tokens** check box to enable ID tokens to be retrieved using OAuth2 **[hybrid flow](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow#request-an-id-token-as-well-hybrid-flow)**

1.  On the **Certificates & secrets** blade, click on **New client secret** and create a new secret. Store this secret in Notepad so you can use it later.

1.  On the **Token configuration** blade, add 2 new claims:

    **groups**
    1.  Click **Add groups claim**
    1.  Select **Groups assigned to the application**
    1.  Under **ID**, select **Group ID**
    1.  Click **Save**

    **upn**
    1.  Click **Add optional claim**
    1.  Select **Token type->ID**
    1.  Check the box for **upn**
    1.  Click **Add**

    ![tokenConfiguration](.img/tokenConfiguration.png))

1.  On the **API permissions** blade, make sure you have the following API permissions (click on **Add a permission** if not)

    **Microsoft Graph**
    - profile
    - User.Read

1.  On the **App roles** blade, create 4 roles (click on **Create app role**).

    Display Name | Description | Allowed member types | Value
    ------------ | ----------- | -------------------- | -----
    General.ReadWrite | General users can read & write their own data | Users/Groups | General.ReadWrite
    Salesperson.ReadWrite | Salespeople can read & write their own data | Users/Groups | Salesperson.ReadWrite
    RegionalManager.ReadWrite | Regional Managers can read & write their own branch data | Users/Groups | RegionalManager.ReadWrite
    CFO.ReadWrite | CFO can read and write all data | Users/Groups | CFO.ReadWrite

1.  On the **Manifest** blade, make sure the **groupMembershipClaims** is set to **ApplicationGroup**

    ![groupMembershipClaims](.img/groupMembershipClaims.png)

### Create AAD Groups

1.  On the **Groups** blade, create groups for each branch & role. Assign users as needed.

    Name | Group Type | Membership Type
    ---- | ---------- | ---------------
    WebAppName_General | Security | Assigned
    WebAppName_Scranton_Salespersion | Security | Assigned
    WebAppName_Scranton_RegionalManager | Security | Assigned
    WebAppName_Stamford_Salesperson | Security | Assigned
    WebAppName_Stamford_RegionalManager | Security | Assigned
    WebAppName_Corporate_CFO | Security | Assigned

### Configure Service Principal (Enterprise Application) to use AAD groups

1.  Under the **Enterprise applciations** blade, search for your new AAD service principal (the same name as the app registration)

1.  Under the **Users and roles** blade, click on **Add user/group** and map the new AAD groups to their respective roles.

    Display Name | Object Type | Role assigned
    ------------ | ----------- | -------------
    WebAppName_General | Group | General.ReadWrite
    WebAppName_Scranton_Salespersion | Group | Salesperson.ReadWrite
    WebAppName_Scranton_RegionalManager | Group | RegionalManager.ReadWrite
    WebAppName_Stamford_Salesperson | Group | Salesperson.ReadWrite
    WebAppName_Stamford_RegionalManager | Group | RegionalManager.ReadWrite
    WebAppName_Corporate_CFO | Group | CFO.ReadWrite

    ![groupRoleAssignment](.img/groupRoleAssignment.png)

## Web app configuration

1.  In the `src/AspNetCoreWithAppRolesAndFineGrained/appsettings.json` file, update the **AzureAD** section with the AAD app registration values you copied to Notepad before.

1.  In the `src/Data/DbInitializer.cs` file, update the values to match your **app role names**, **group IDs** and **users**.

## Run locally

1.  Initialize the local Sqlite database

    ```shell
    dotnet ef database update
    ```

1.  Run the application

    ```shell
    dotnet watch run
    ```

As you sign in with different users, you will see that they have different permissions.

**Salesperson**

**Regional manager - Scranton**

**Regional manager - Stamford**

**CFO**

## Deployment to Azure

Run the following command in an existing resource group.

```shell
cd inf/bicep
az deployment group create --resource-group rg-webAppWithAppRolesAndFineGrained-ussc-demo --template-file ./main.bicep --parameters ./main.parameters.json
```

## References

- https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-add-app-roles-in-azure-ad-apps
- https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-5.0
- https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-5.0
