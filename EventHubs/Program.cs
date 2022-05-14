using Learning.EventHubs;
using Microsoft.Extensions.Configuration;

/*
 * https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-dotnet-standard-getstarted-send
 * https://docs.microsoft.com/en-us/azure/event-hubs/sdks
 * https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/eventhub/Azure.Messaging.EventHubs/samples
 * https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/eventhub/Azure.Messaging.EventHubs.Processor/samples
 */

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

// connection string to the Event Hubs namespace
var connectionString = configuration["EventHubConnectionString"];
// name of the event hub
var eventHubName = configuration["EventHub"];

var blobStorageConnectionString = configuration["StorageConnectionString"];
var blobContainerName = configuration["BlobContainer"];

//Sender sender = new Sender(connectionString, eventHubName);
//await sender.SendEventAsync();

//await Task.Delay(TimeSpan.FromSeconds(5));

Console.WriteLine("=======================================================================================");

Receiver receiver = new Receiver(connectionString, eventHubName, blobStorageConnectionString, blobContainerName);
receiver.ReceiveEventAsync();


await Task.Delay(TimeSpan.FromSeconds(3600));