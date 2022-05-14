using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Configuration;

using BlobStorage;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();
var connectionString = configuration["StorageConnectionString"];
var storageAccountURI = $@"{configuration["StorageAccountURI"]}";
//var blobContainerUri = $@"{configuration["BlobContainerUri"]}";

BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
BlobStorageService.blobServiceClient = blobServiceClient;

// Create a BlobContainerClient object which will be used to manipulate a container client
//BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

string file1 = "C:/Users/BS701/Documents/Temp/image1.jpg";
string file2 = "C:/Users/BS701/Documents/Temp/image2.jpg";
string file3 = "C:/Users/BS701/Documents/Temp/demo.txt";

BlobContainerClient container1 = await BlobStorageService.CreateOrGetContainer("con-1-container-level", PublicAccessType.BlobContainer);
BlobContainerClient container2 = await BlobStorageService.CreateOrGetContainer("con-2-blob-level", PublicAccessType.Blob);
BlobContainerClient container3 = await BlobStorageService.CreateOrGetContainer("con-3-private", PublicAccessType.None);

try
{
    //await BlobStorageService.UploadBlob(container1, file1);
    //await BlobStorageService.UploadBlob(container1, file2);
    //await BlobStorageService.UploadBlob(container1, file3);

    //await BlobStorageService.UploadBlob(container2, file1);
    //await BlobStorageService.UploadBlob(container2, file2);
    //await BlobStorageService.UploadBlob(container2, file3);

    //await BlobStorageService.UploadBlob(container3, file1);
    //await BlobStorageService.UploadBlob(container3, file2);
    //await BlobStorageService.UploadBlob(container3, file3);

    //await BlobStorageService.ListBlobsAsync(container1);
    //await BlobStorageService.ListBlobsAsync(container2);
    //await BlobStorageService.ListBlobsAsync(container3);

    //await BlobStorageService.ListBlobs(container1);
    //await BlobStorageService.ListBlobs(container2);

    //await BlobStorageService.ListBlobsAsAnonymousUser("con-1-container-level", storageAccountURI);
    //await BlobStorageService.ListBlobsAsAnonymousUser("con-2-blob-level", storageAccountURI);
    //await BlobStorageService.ListBlobsAsAnonymousUser("con-3-private", storageAccountURI);

    //await BlobStorageService.AcquireLease(container1, "demo.txt");
    //await BlobStorageService.DownloadBlob(container1);


    //SASTokenService.CreateServiceSASforBlob(container3.GetBlobClient("image1.jpg"));
    //SASTokenService.CreateServiceSasUriForContainer(container3);

    SASTokenService.CreateSharedAccessPolicy(container3, "demo-policy-1");
    SASTokenService.CreateServiceSASforBlob(container3.GetBlobClient("image1.jpg"), "demo-policy-1");
}
catch (Exception e)
{

    Console.Error.WriteLine(e);
}



