using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using ServiceBus;

/*
 * https://github.com/azure/azure-sdk-for-net/tree/main/sdk/servicebus/Azure.Messaging.ServiceBus/samples
 */

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var connectionString = configuration["ServiceBusConnectionString"];

// the client that owns the connection and can be used to create senders and receivers
ServiceBusClient client = new ServiceBusClient(connectionString);
ServiceBusClient client2 = new ServiceBusClient(connectionString);

try
{
    MessageSender messageSender = new MessageSender(client, "bs23-topic");
    await messageSender.SendAsync(5);
    //await messageSender.SendSessionMessagesAsync(50, "Session1");
    //await messageSender.SendSessionMessagesAsync(50, "Session2");

    //MessageReceiver messageReceiver = new MessageReceiver(client, "bs23-queue");
    MessageReceiver messageReceiver1 = new MessageReceiver(client, "bs23-topic", "bs23-sub-1");
    MessageReceiver messageReceiver2 = new MessageReceiver(client, "bs23-topic", "bs23-sub-2");
    await messageReceiver1.ReceiveMessagesAsync();
    await messageReceiver2.ReceiveMessagesAsync();


    //SessionMessagesReceiver sessionMessagesReceiver = new SessionMessagesReceiver();
    //await sessionMessagesReceiver.ReceiveSessionMessagesAsync(client, "bs23-session-queue");


    //Console.WriteLine("Wait for a minute and then press any key to end the processing");
    //Console.ReadKey();
    //await client.DisposeAsync();
    //Console.WriteLine("Stopped receiving messages");
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
}