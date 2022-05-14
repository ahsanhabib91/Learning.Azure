using Azure.Messaging.ServiceBus;

namespace ServiceBus;

public class MessageReceiver
{
    private ServiceBusProcessor processor;
    public MessageReceiver(ServiceBusClient client, string queueName)
    {
        var options = new ServiceBusProcessorOptions()
        {
           MaxConcurrentCalls = 10,
        };
        processor = client.CreateProcessor(queueName, options);

        // add handler to process messages
        processor.ProcessMessageAsync += MessageHandler;

        // add handler to process any errors
        processor.ProcessErrorAsync += ErrorHandler;
    }

    public MessageReceiver(ServiceBusClient client, string topicName, string subscriptionName)
    {
        var options = new ServiceBusProcessorOptions()
        {
            MaxConcurrentCalls = 10,
        };
        processor = client.CreateProcessor(topicName, subscriptionName, options);

        // add handler to process messages
        processor.ProcessMessageAsync += MessageHandler;

        // add handler to process any errors
        processor.ProcessErrorAsync += ErrorHandler;
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"{DateTime.Now} --> Received: {body}");

        // complete the message. messages is deleted from the queue. 
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        // the error source tells me at what point in the processing an error occurred
        Console.WriteLine(args.ErrorSource);
        // the fully qualified namespace is available
        Console.WriteLine(args.FullyQualifiedNamespace);
        // as well as the entity path
        Console.WriteLine(args.EntityPath);
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

    public async Task ReceiveMessagesAsync()
    {
        // start processing 
        await processor.StartProcessingAsync();

        Console.WriteLine("Wait for a minute and then press any key to end the processing");
        Console.ReadKey();

        // stop processing 
        Console.WriteLine("\nStopping the receiver...");
        await processor.StopProcessingAsync();
        Console.WriteLine("Stopped receiving messages");
    }

}
