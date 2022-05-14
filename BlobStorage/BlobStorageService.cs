using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace BlobStorage;

/*
 * https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
 */

public class BlobStorageService
{
    public static BlobServiceClient blobServiceClient;

    public static async Task<BlobContainerClient> CreateOrGetContainer(string containerName, PublicAccessType publicAccessType = PublicAccessType.None)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        //if (containerClient.Exists())
        if (await containerClient.ExistsAsync())
        {
            return containerClient;
        }

        containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
        //var isCreated = await container.CreateIfNotExistsAsync();
        Console.WriteLine($"Container created {containerClient.Name}: {containerClient.Uri}");

        containerClient.SetAccessPolicy(publicAccessType);
        Console.WriteLine($"Container {containerClient.Name} has been made: {containerClient.GetAccessPolicy().Value}");

        return containerClient;
    }

    public static async Task UploadBlob(BlobContainerClient containerClient, string localFilePath)
    {
        FileInfo file = new FileInfo(localFilePath);

        BlobClient blobClient = containerClient.GetBlobClient(file.Name);

        Console.WriteLine($"Uploading to Blob storage as blob: {blobClient.Uri}");

        await blobClient.UploadAsync(localFilePath, true);
    }

    public static async Task AcquireLease(BlobContainerClient containerClient, string blobName)
    {
        BlobClient blobClient = containerClient.GetBlobClient(blobName);
        BlobLeaseClient lease = blobClient.GetBlobLeaseClient();
        await lease.AcquireAsync(TimeSpan.FromSeconds(60));
        Console.WriteLine($"Blob lease acquired. Lease = {lease.LeaseId}");
        Thread.Sleep(TimeSpan.FromSeconds(15));
        await lease.ReleaseAsync();
        Console.WriteLine($"LeaseId: {lease.LeaseId} is broken");
    }

    public static async Task ListBlobsAsync(BlobContainerClient containerClient)
    {
        Console.WriteLine($"\n\tAsync List of Blobs in {containerClient.Name} and {containerClient.GetAccessPolicy().Value}");
        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
            Console.WriteLine($"{blobItem.Name} - {blobClient.Uri}");
        }
    }

    public static async Task ListBlobs(BlobContainerClient containerClient)
    {
        Console.WriteLine($"\n\tList of Blobs in {containerClient.Name} and {containerClient.GetAccessPolicy().Value}");
        foreach (var blob in containerClient.GetBlobs())
        {
            BlobClient blobClient = containerClient.GetBlobClient(blob.Name);
            Console.WriteLine($"{blob.Name} - {blobClient.Uri}");
        }
    }

    /*
     * When Access Level is Container and User is Anonymous, can list the Blobs inside that container.
     * When Access Level is Private and User is Anonymous, cannot List the Blobs inside that Container.
     * When Access Level is Blob and User is Anonymous, cannot List Blobs inside that Container. But Can access the Blob individually.
     */
    public static async Task ListBlobsAsAnonymousUser(string containerName, string storageAccountURI)
    {
        BlobServiceClient blobClientForAnonymous = new BlobServiceClient(new Uri(storageAccountURI));
        BlobContainerClient containerClient = blobClientForAnonymous.GetBlobContainerClient(containerName);

        Console.WriteLine($"\tAccessing {containerName} as Anonymous User");

        //await ListBlobs(containerClient);
        //await ListBlobsAsync(containerClient);

        //foreach (var blob in containerClient.GetBlobs())
        //{
        //    BlobClient blobClient = containerClient.GetBlobClient(blob.Name);
        //    Console.WriteLine($"{blob.Name} - {blobClient.Uri}");
        //}

        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
            Console.WriteLine($"{blobItem.Name} - {blobClient.Uri}");
        }
    }

    /*
     * blobContainerUri -> Container URL, not the storgae account URL
     */
    public static async Task ListBlobsAsAnonymousUserUsingContainerClient(string blobContainerUri)
    {
        BlobContainerClient containerClientForAnonymous = new BlobContainerClient(new Uri(blobContainerUri));
        await ListBlobs(containerClientForAnonymous);
    }

    public static async Task DownloadBlob(BlobContainerClient containerClient)
    {
        string downloadedFilePath = "C:/Users/BS701/Documents/Temp/downloaded_file.txt";
        BlobClient blobClient = containerClient.GetBlobClient("demo.txt");
        await blobClient.DownloadToAsync(downloadedFilePath);
        Console.WriteLine("File Downloaded ....");
    }

    public static void DeleteBlobs(BlobContainerClient container)
    {
        foreach (var blob in container.GetBlobs())
        {
            container.DeleteBlob(blob.Name);
        }
    }

    public static async Task DeleteBlobsAsync(BlobContainerClient container)
    {
        await foreach (var blob in container.GetBlobsAsync())
        {
            await container.DeleteBlobAsync(blob.Name);
        }
    }

    public static async Task DeleteContainerAsync(BlobContainerClient containerClient)
    {
        await containerClient.DeleteAsync();
    }
}

