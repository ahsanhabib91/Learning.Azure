using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using System.Text;

/*
 * https://docs.microsoft.com/en-us/azure/storage/queues/storage-quickstart-queues-dotnet
 * https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/storage/Azure.Storage.Queues
 * https://docs.microsoft.com/en-us/azure/storage/queues/storage-tutorial-queues?toc=%2Fazure%2Fstorage%2Fqueues%2Ftoc.json&tabs=dotnet
 * https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-azure-and-service-bus-queues-compared-contrasted
 * first-in-first-out (FIFO) is NOT guaranteed in Storage Queue
 */

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var connectionString = configuration["StorageConnectionString"];

string queueName = "sample-queue-1";

QueueClientOptions queueClientOptions = new QueueClientOptions()
{
    MessageEncoding = QueueMessageEncoding.Base64
};
QueueClient queueClient = new QueueClient(connectionString, queueName, queueClientOptions);

//QueueClient queueClient = new QueueClient(connectionString, queueName);
QueueProperties queueProperties = await queueClient.GetPropertiesAsync();
Console.WriteLine($"ApproximateMessagesCount: {queueProperties.ApproximateMessagesCount}");

// Create the queue
//await queueClient.CreateAsync();
//Console.WriteLine("Queue Successfully Created");

// Sending messages
for (int i = 1; i <= 10; i++)
{
    SendMessage(queueClient, $"Message-{i}");
}

//await PeekMessages(queueClient);

//ReceiveMessages(queueClient);


static void SendMessage(QueueClient queueClient, dynamic message)
{
    queueClient.SendMessage(message);

    //queueClient.SendMessage(Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
}

static void ReceiveMessages(QueueClient queueClient)
{
    while (true)
    {
        // Get the next messages from the queue
        foreach (QueueMessage message in queueClient.ReceiveMessages().Value)
            //foreach (QueueMessage message in queueClient.ReceiveMessages(maxMessages: 2).Value)
        {
            // "Process" the message
            Console.WriteLine($"Message: {message.Body}, Dequeue Count: {message.DequeueCount}");

            // Let the service know we're finished with the message and it can be safely deleted.
            // if we do not delete the message, then it will be inserted back to the queue and the DequeueCount will get increased
            queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
            //Thread.Sleep(1000);
        }
    }
}

static async Task PeekMessages(QueueClient queueClient)
{
    // Peek at messages in the queue
    // Peeking the message does not increase the DequeueCount
    PeekedMessage[] peekedMessages = await queueClient.PeekMessagesAsync(maxMessages: 10);

    foreach (PeekedMessage peekedMessage in peekedMessages)
    {
        Console.WriteLine($"Message: {peekedMessage.MessageText}, Dequeue Count: {peekedMessage.DequeueCount}");
    }
}
