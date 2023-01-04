using Azure.Messaging.ServiceBus;

namespace ServiceBus;

public class SessionMessagesReceiver
{

    public async Task ReceiveSessionMessagesAsync(ServiceBusClient client, string queueName)
    {
        // create the options to use for configuring the processor
        var options = new ServiceBusSessionProcessorOptions
        {
            // By default after the message handler returns, the processor will complete the message
            // If I want more fine-grained control over settlement, I can set this to false.
            //AutoCompleteMessages = false,

            // I can also allow for processing multiple sessions
            MaxConcurrentSessions = 5,

            // By default or when AutoCompleteMessages is set to true, the processor will complete the message after executing the message handler
            // Set AutoCompleteMessages to false to [settle messages](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-transfers-locks-settlement#peeklock) on your own.
            // In both cases, if the message handler throws an exception without settling the message, the processor will abandon the message.
            MaxConcurrentCallsPerSession = 5,

            // Processing can be optionally limited to a subset of session Ids.
            SessionIds = { "Session1", "Session2" },
        };
        // create a session processor that we can use to process the messages
        await using ServiceBusSessionProcessor processor = client.CreateSessionProcessor(queueName, options);

        // configure the message and error handler to use
        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;

        // start processing
        await processor.StartProcessingAsync();

        Console.WriteLine("Wait for a minute and then press any key to end the processing");
        Console.ReadKey();

        // stop processing 
        Console.WriteLine("\nStopping the receiver...");
        await processor.StopProcessingAsync();
        Console.WriteLine("Stopped receiving messages");
    }

    async Task MessageHandler(ProcessSessionMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        Console.WriteLine($"{DateTime.Now} --> Received: {args.SessionId}: {body}");

        // we can evaluate application logic and use that to determine how to settle the message.
        await args.CompleteMessageAsync(args.Message);

        // we can also set arbitrary session state using this receiver
        // the state is specific to the session, and not any particular message
        await args.SetSessionStateAsync(new BinaryData("some state"));
    }

    Task ErrorHandler(ProcessErrorEventArgs args)
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

}
