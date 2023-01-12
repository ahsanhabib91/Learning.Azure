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

var myKey = Guid.NewGuid().ToString();
Console.WriteLine($"KeyName: {myKey}");

// var myKey = "eca118f1-6f35-42d8-9ee7-b000620e71b6";

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

// var encryptedText = await manageKey.Encrypt(myKey, "A single block of plaintext");
var encryptedText = await manageKey.Encrypt(myKey, null, tenantId, clientId, clientSecret);

Console.WriteLine($"EncryptedText Text: {encryptedText}");

// var testEncryptedText = "tIaO2qi9EIUe5nt0kdHqQOg72TDqnhOeYXCC9v5Pstsd4cFz/Hc6BOk/98cJ7X/EDhDSLmAYnquTrk0oZxbwG6gw3Dp4gjP07ajRx8Fai5/Qis6JJAVtroHFLAlVmFIepuknCXsrrx18gPDNYEXnh0eQgePWH/h4y7Q2FN4Nf9arG8VcB3WK8uqleXcIAkDw7MgwJG41pGqRbvvnJvc7yHh+HELdhnbQg+Wu7o6+tbtDCaH1WH2muDECSvHIALmbjI1wPNrV5gLHAXTqdxSHh+uGrAvkez0fzzEgPmJeBlu4Chw3Wr7B0ZuMzkpsCL8Lztdyc0Xy+6lLER1ICcHxUg==";

// var decryptedText = await manageKey.Decrypt(myKey, encryptedText);
var decryptedText = await manageKey.Decrypt(myKey, encryptedText, tenantId, clientId, clientSecret);

Console.WriteLine($"Decrypted Text: {decryptedText}");






