using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using KeyVault;
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
// var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

var keyVaultName = configuration["KEY_VAULT_NAME"];
var kvUri = $"https://{keyVaultName}.vault.azure.net";

// var myKey = Guid.NewGuid().ToString();
// Console.WriteLine($"KeyName: {myKey}");

var myKey = "<my-key>";

// Console.WriteLine(myKey);


// var manageSecret = new ManageSecret(kvUri);
// await manageSecret.GetSecret("MySecret");


// var manageKey = new ManageKey(kvUri);
var manageKey = new ManageKey(kvUri, tenantId, clientId, clientSecret);

try 
{
    await manageKey.GetKey(myKey);
} 
catch (RequestFailedException ex) when (ex.Status == 404)
{
    await manageKey.CreateKey(myKey);
    Console.WriteLine("Error occured:");
    // throw ex;
}

// await manageKey.GetKey(myKey);

// return;

// var plainText = "A single block of plaintext";
var plainText = "I am talking from Mac Machine";
// var encryptedText = await manageKey.Encrypt(myKey, plainText);
var encryptedText = await manageKey.Encrypt(myKey, plainText, tenantId, clientId, clientSecret);

Console.WriteLine($"EncryptedText Text: {encryptedText}");

// var testEncryptedText = "p6lW7AjtAq3220RewD+fJPOvnB3EpaJFNegY8iVMYjbVs6RDyANaEdi+HqJq+Vkiisq3Kk8Nyfpx+gdEIFhQAYBwtjSo11zHWiz1odW7/btdUKiYOJ3DVFLDTiLigJD1BcXIv9wfPF8S9S3Lt31tpdNis9gaFSDXWKTnLPWa0ynKGhoD7InTLnNm36zTAlwq2thcExwj8lWuqF0ygb/AYrifl4uE2tu4Fl+eX8zU/1O7ZycI2daHiga7j4YDWmemWx+jMY793gImtuQc+FkzEb1npG1icbO6bd+Vu4Ew0FmKhKozccB3ec6Q4CwoyKcXeFrx3Qs78/fMsMjNJCIudQ== ";

// var decryptedText = await manageKey.Decrypt(myKey, encryptedText);
var decryptedText = await manageKey.Decrypt(myKey, encryptedText, tenantId, clientId, clientSecret);

Console.WriteLine($"Decrypted Text: {decryptedText}");






