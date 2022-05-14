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

        // number of events to be sent to the event hub
        private const int numOfEvents = 10;

        public Sender(string connectionString, string eventHubName)
        {
            // Create a producer client that you can use to send events to an event hub
            producerClient = new EventHubProducerClient(connectionString, eventHubName);
        }

        public async Task SendEventAsync() 
        {
            // Create a batch of events 
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            for (int i = 1; i <= numOfEvents; i++)
            {
                // if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Event {i}"))))
                // {
                //     // if it is too large for the batch
                //     throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                // }
                var data = new Employee() {Id = Guid.NewGuid(), Message = $"Event {i}"};
                //var jsonData = JsonConvert.SerializeObject(data);
                var jsonData = JsonSerializer.Serialize(data);
                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(jsonData))))
                {
                    // if it is too large for the batch
                    throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                }
            }

            try
            {
                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine($"A batch of {numOfEvents} events has been published.");
            }
            finally
            {
                await producerClient.DisposeAsync();
            }

        }
    }

    class Employee
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
    }
}
