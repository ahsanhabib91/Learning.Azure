using Azure.Messaging.ServiceBus;

namespace ServiceBus;

public class MessageSender
{

    public ServiceBusSender sender;
    string queueOrTopicName;
    public MessageSender(ServiceBusClient client, string queueOrTopicName)
    {
        sender = client.CreateSender(queueOrTopicName);
        this.queueOrTopicName = queueOrTopicName;
    }

    public async Task SendAsync(int numOfMessages = 3)
    {
        // create a batch 
        using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
        for (int i = 1; i <= numOfMessages; i++)
        {
            var message = new ServiceBusMessage($"Message {i}");
            message.ApplicationProperties.Add("Author", "Ahsan"); // Adding metadata which could be using for Subscription filter
            // try adding a message to the batch
            if (!messageBatch.TryAddMessage(message))
            {
                // if it is too large for the batch
                throw new Exception($"The message {i} is too large to fit in the batch.");
            }
        }

        try
        {
            // Use the producer client to send the batch of messages to the Service Bus queue
            await sender.SendMessagesAsync(messageBatch);
            Console.WriteLine($"A batch of {numOfMessages} messages has been published to the {queueOrTopicName}");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
        }
    }

    public async Task SendSessionMessagesAsync(int numOfMessages = 3, string sessionId = "my-session")
    {
        // create a message batch that we can send
        ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

        for (int i = 1; i <= numOfMessages; i++)
        {
            // try adding a message to the batch
            if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}") { SessionId = sessionId }))
            {
                // if it is too large for the batch
                throw new Exception($"The message {i} is too large to fit in the batch.");
            }
        }

        try
        {
            // Use the producer client to send the batch of messages to the Service Bus queue
            await sender.SendMessagesAsync(messageBatch);
            Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue with SessionId: {sessionId}");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
        }
    }
}