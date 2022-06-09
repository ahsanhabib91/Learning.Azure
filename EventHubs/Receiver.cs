using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Learning.EventHubs.Model;

namespace Learning.EventHubs;

public class Receiver
{
    
    static BlobContainerClient storageClient;

    // The Event Hubs client types are safe to cache and use as a singleton for the lifetime
    // of the application, which is best practice when events are being published or read regularly.        
    EventProcessorClient processor;

    public Receiver(string ehubNamespaceConnectionString, string eventHubName, string blobStorageConnectionString, string blobContainerName)
    {
        // Read from the default consumer group: $Default
        // string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
        string consumerGroup = "$Default";

        // Create a blob container client that the event processor will use 
        storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

        // Create an event processor client to process events in the event hub
        processor = new EventProcessorClient(storageClient, consumerGroup, ehubNamespaceConnectionString, eventHubName);

        // Register handlers for processing events and handling errors
        processor.ProcessEventAsync += ProcessEventHandler;
        processor.ProcessErrorAsync += ProcessErrorHandler;
    }

    public async Task ReceiveEventAsync()
    {
        // Start the processing
        await processor.StartProcessingAsync();

        // Wait for 30 seconds for the events to be processed
        //await Task.Delay(TimeSpan.FromSeconds(300));

        // Stop the processing
        // await processor.StopProcessingAsync();
    }

    static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        try
        {
            string rawData = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());
            Console.WriteLine($"\t {DateTime.Now} --> Received event: {rawData}");
        }
        catch (Exception ex)
        {
            string rawData = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());
            Console.WriteLine($"Unable to read data. ex.Message: {ex.Message}, rawData --> {rawData}");
        }
        

        // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
        await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
    }

    static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        // Write details about the error to the console window
        Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
        Console.WriteLine(eventArgs.Exception.Message);
        return Task.CompletedTask;
    }
}
