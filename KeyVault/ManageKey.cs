using System.Diagnostics;
using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace KeyVault;

public class ManageKey
{
    private readonly KeyClient _keyClient;

    public ManageKey(string keyVaultUrl)
    {
        _keyClient = new KeyClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
    }
    
    public ManageKey(string keyVaultUrl, string tenantId, string clientId, string clientSecret)
    {
        _keyClient = new KeyClient(new Uri(keyVaultUrl), new ClientSecretCredential(tenantId, clientId, clientSecret));
    }

    public async Task CreateKey(string keyName)
    {
        var key = await _keyClient.CreateKeyAsync(keyName, KeyType.Rsa);
        Console.WriteLine($"Key version is => {key.Value.Properties.Version}");
    }
    
    public async Task GetKey(string keyName)
    {
        var key = await _keyClient.GetKeyAsync(keyName);
        Console.WriteLine($"Key version is => {key.Value.Properties.Version}");
    }
    
    public async Task<string> Encrypt(string keyName, string plainText)
    {
        var key = await _keyClient.GetKeyAsync(keyName);
        var cryptoClient = new CryptographyClient(key.Value.Id, new DefaultAzureCredential());
        byte[] plaintextAsByteArray = Encoding.UTF8.GetBytes(plainText);
        EncryptResult encryptResult = await cryptoClient.EncryptAsync(EncryptionAlgorithm.RsaOaep256, plaintextAsByteArray);
        // Console.WriteLine($"Encrypted data using the algorithm {encryptResult.Algorithm}, with key {encryptResult.KeyId}. The resulting encrypted data is {Convert.ToBase64String(encryptResult.Ciphertext)}");
        return Convert.ToBase64String(encryptResult.Ciphertext);
    }
    
    public async Task<string> Decrypt(string keyName, string encryptedText)
    {
        var key = await _keyClient.GetKeyAsync(keyName);
        var cryptoClient = new CryptographyClient(key.Value.Id, new DefaultAzureCredential());
        byte[] dataToDecryptAsByteArray = Convert.FromBase64String(encryptedText);
        DecryptResult decryptResult = await cryptoClient.DecryptAsync(EncryptionAlgorithm.RsaOaep256, dataToDecryptAsByteArray);
        // Console.WriteLine($"Decrypted data using the algorithm {decryptResult.Algorithm}, with key {decryptResult.KeyId}. The resulting decrypted data is {Encoding.UTF8.GetString(decryptResult.Plaintext)}");
        return Encoding.UTF8.GetString(decryptResult.Plaintext);
    }

    public async Task<string> Encrypt(string keyName, string plainText, string tenantId, string clientId, string clientSecret)
    {
        var key = await _keyClient.GetKeyAsync(keyName);
        var cryptoClient = new CryptographyClient(key.Value.Id, new ClientSecretCredential(tenantId, clientId, clientSecret));
        byte[] plaintextAsByteArray = Encoding.UTF8.GetBytes(plainText);
        EncryptResult encryptResult = await cryptoClient.EncryptAsync(EncryptionAlgorithm.RsaOaep256, plaintextAsByteArray);
        // Console.WriteLine($"Encrypted data using the algorithm {encryptResult.Algorithm}, with key {encryptResult.KeyId}. The resulting encrypted data is {Convert.ToBase64String(encryptResult.Ciphertext)}");
        return Convert.ToBase64String(encryptResult.Ciphertext);
    }

    public async Task<string> Decrypt(string keyName, string encryptedText, string tenantId, string clientId, string clientSecret)
    {
        var key = await _keyClient.GetKeyAsync(keyName);
        var cryptoClient = new CryptographyClient(key.Value.Id, new ClientSecretCredential(tenantId, clientId, clientSecret));
        byte[] dataToDecryptAsByteArray = Convert.FromBase64String(encryptedText);
        DecryptResult decryptResult = await cryptoClient.DecryptAsync(EncryptionAlgorithm.RsaOaep256, dataToDecryptAsByteArray);
        // Console.WriteLine($"Decrypted data using the algorithm {decryptResult.Algorithm}, with key {decryptResult.KeyId}. The resulting decrypted data is {Encoding.UTF8.GetString(decryptResult.Plaintext)}");
        return Encoding.UTF8.GetString(decryptResult.Plaintext);
    }
}