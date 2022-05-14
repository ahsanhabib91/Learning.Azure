using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

/*
 * ASP.NET Core ==> 
 *      https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0
 *      https://docs.microsoft.com/en-us/azure/key-vault/general/vs-key-vault-add-connected-service
 *      
 * Manage keys, certificates, and secrets ==> 
 *      https://docs.microsoft.com/en-us/azure/key-vault/general/developers-guide#manage-keys-certificates-and-secrets
 * 
 * Sample ==>
 *      https://github.com/Azure/azure-sdk-for-net/tree/Azure.Security.KeyVault.Keys_4.3.0/sdk/keyvault
 *      
 * Azure Identity client libraries ==> 
 *      https://docs.microsoft.com/en-us/azure/key-vault/general/developers-guide#azure-identity-client-libraries
 *      https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme
 * 
 * Use Key Vault references for App Service and Azure Functions ==>
 *      https://docs.microsoft.com/en-us/azure/app-service/app-service-key-vault-references
 *      https://docs.microsoft.com/en-us/azure/app-service/overview-managed-identity?toc=%2Fazure%2Fazure-functions%2Ftoc.json&tabs=portal%2Chttp
 *      
 * Azure.Extensions.AspNetCore ==>
 *      https://github.com/Azure/azure-sdk-for-net/tree/Azure.Extensions.AspNetCore.Configuration.Secrets_1.2.2/sdk/extensions
 * 
 */

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var tenantId = configuration["TENANT_ID"];
var clientId = configuration["CLIENT_ID"];
var clientSecret = configuration["CLIENT_SECRET"];
//var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

var keyVaultName = configuration["KEY_VAULT_NAME"];
var kvUri = $"https://{keyVaultName}.vault.azure.net";


/*
 * Managing Azure Key Vault Secret
 */
var secretClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
//var secretClient = new SecretClient(new Uri(kvUri), clientSecretCredential);

var secretName = "MySecret";
//await secretClient.SetSecretAsync(secretName, "Azure123");
//Console.WriteLine("Secret has been set !!!");
var secret = await secretClient.GetSecretAsync(secretName);
Console.WriteLine($"Secret Value => {secret.Value.Value}");

return;

/*
 * Managing Azure Key Vault Keys
 */
var keyClient = new KeyClient(new Uri(kvUri), new DefaultAzureCredential());
//var keyClient = new KeyClient(new Uri(kvUri), clientSecretCredential);

var keyName = "myKey";
var createdKey = await keyClient.CreateKeyAsync(keyName, KeyType.Rsa);
Console.WriteLine("Key has been set !!!");
var key = await keyClient.GetKeyAsync(keyName);
Console.WriteLine($"Key version is => {key.Value.Properties.Version}");

