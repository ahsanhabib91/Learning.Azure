using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace KeyVault;

public class ManageSecret
{
    private readonly SecretClient _secretClient;

    public ManageSecret(string keyVaultUrl)
    {
        _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
    }

    public async Task GetSecret(string secretName)
    {
        //await secretClient.SetSecretAsync(secretName, "Azure123");
        //Console.WriteLine("Secret has been set !!!");
        var secret = await _secretClient.GetSecretAsync(secretName);
        Console.WriteLine($"Secret Value => {secret.Value.Value}");
    }
}