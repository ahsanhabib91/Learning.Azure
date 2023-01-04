using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace Learning.EventHubs
{
    public class Sender
    {
        // The Event Hubs client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when events are being published or read regularly.
        static EventHubProducerClient producerClient;

        private const int numOfEvents = 10;

        public Sender(string connectionString, string eventHubName)
        {
            // Create a producer client that you can use to send events to an event hub
            producerClient = new EventHubProducerClient(connectionString, eventHubName);
        }

        public async Task SendEventAsync() 
        {
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            for (int i = 1; i <= numOfEvents; i++)
            {
                var data = new { Id = Guid.NewGuid(), Message = $"Event {i}" };
                //var jsonData = JsonConvert.SerializeObject(data);
                var jsonData = JsonSerializer.Serialize(data);
                //if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(jsonData))))
                if (!eventBatch.TryAdd(new EventData(jsonData)))
                {
                    throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                }
            }

            try
            {
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine($"A batch of {numOfEvents} events has been published.");
            }
            finally
            {
                await producerClient.DisposeAsync();
            }

        }
    }
}
