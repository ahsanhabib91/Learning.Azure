using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

namespace BlobStorage;

/*
 * Service SAS Token: https://docs.microsoft.com/en-us/azure/storage/blobs/sas-service-create?tabs=dotnet
 * Stored access policy: https://docs.microsoft.com/en-us/azure/storage/common/storage-stored-access-policy-define-dotnet?tabs=dotnet
 */
public class SASTokenService
{
    public static string CreateServiceSASforBlob(BlobClient blobClient, string storedPolicyName = null)
    {
        // Check whether this BlobClient object has been authorized with Shared Key.
        if (blobClient.CanGenerateSasUri)
        {
            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                BlobName = blobClient.Name,
                Resource = "b"
            };
            if (storedPolicyName == null)
            {
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }
            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
            Console.WriteLine("SAS URI for blob is: {0}", sasUri);
            Console.WriteLine();
            return sasUri.ToString();
        }
        else
        {
            Console.WriteLine(@"BlobClient must be authorized with Shared Key credentials to create a service SAS.");
            return null;
        }
    }

    public static Uri CreateServiceSasUriForContainer(BlobContainerClient containerClient, string storedPolicyName = null)
    {
        // Check whether this BlobContainerClient object has been authorized with Shared Key.
        if (containerClient.CanGenerateSasUri)
        {
            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerClient.Name,
                Resource = "c"
            };
            // storedPolicyName has to be an existing one
            if (storedPolicyName == null)
            {
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }

            Uri sasUri = containerClient.GenerateSasUri(sasBuilder);
            Console.WriteLine("SAS URI for blob container is: {0}", sasUri);
            Console.WriteLine();

            return sasUri;
        }
        else
        {
            Console.WriteLine(@"BlobContainerClient must be authorized with Shared Key 
                          credentials to create a service SAS.");
            return null;
        }
    }

    public static void CreateSharedAccessPolicy(BlobContainerClient container, string policyName)
    {
        try
        {
            List<BlobSignedIdentifier> signedIdentifiers = new List<BlobSignedIdentifier>();
            signedIdentifiers.Add(
            new BlobSignedIdentifier
            {
                Id = policyName,
                AccessPolicy = new BlobAccessPolicy
                {
                    StartsOn = DateTimeOffset.UtcNow.AddHours(-1),
                    ExpiresOn = DateTimeOffset.UtcNow.AddDays(1),
                    Permissions = "rw"
                }
            }
            );
            // Set the container's access policy.
            container.SetAccessPolicy(permissions: signedIdentifiers);
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine(e.ErrorCode);
            Console.WriteLine(e.Message);
        }
    }
}

