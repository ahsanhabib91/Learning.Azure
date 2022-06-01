using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

// https://docs.microsoft.com/en-us/azure/cosmos-db/sql/how-to-write-stored-procedures-triggers-udfs?tabs=javascript#triggers
// https://docs.microsoft.com/en-us/azure/cosmos-db/sql/how-to-use-stored-procedures-triggers-udfs?tabs=dotnet-sdk-v3#how-to-run-pre-triggers

namespace CosmosDb.ServerSide.Demos
{
    public static class TriggerDemo
    {
        public async static Task Run()
        {
            Debugger.Break();

            await CreateTriggers();

            await ViewTriggers();

            await ExecuteTriggers();

            await DeleteTriggers();
        }

        // Create triggers

        private async static Task CreateTriggers()
        {
            Console.WriteLine();
            Console.WriteLine(">>> Create Triggers <<<");
            Console.WriteLine();

            await CreatePreTrigger("trgPreValidateToDoItemTimestamp");
            await CreatePostTrigger("trgPostUpdateMetadata");
        }

        private async static Task CreatePreTrigger(string triggerId)
        {
            var preTrgBody = File.ReadAllText($@"Server\{triggerId}.js");

            var trgProps = new TriggerProperties
            {
                Id = "trgPreValidateToDoItemTimestamp",
                Body = preTrgBody,
                TriggerOperation = TriggerOperation.Create,
                TriggerType = TriggerType.Pre
            };

            var container = Shared.Client.GetContainer("adventure-works", "stores");
            var result = await container.Scripts.CreateTriggerAsync(trgProps);
            Console.WriteLine($"Created pre trigger {triggerId} ({result.RequestCharge} RUs);");
        }

        private async static Task CreatePostTrigger(string triggerId)
        {
            var postTrgBody = File.ReadAllText($@"Server\{triggerId}.js");

            var trgProps = new TriggerProperties
            {
                Id = "trgPostUpdateMetadata",
                Body = postTrgBody,
                TriggerOperation = TriggerOperation.Create,
                TriggerType = TriggerType.Post
            };

            var container = Shared.Client.GetContainer("adventure-works", "stores");
            var result = await container.Scripts.CreateTriggerAsync(trgProps);
            Console.WriteLine($"Created post trigger {triggerId} ({result.RequestCharge} RUs);");
        }

        // View triggers

        private static async Task ViewTriggers()
        {
            Console.WriteLine();
            Console.WriteLine(">>> View Triggers <<<");
            Console.WriteLine();

            var container = Shared.Client.GetContainer("adventure-works", "stores");

            var iterator = container.Scripts.GetTriggerQueryIterator<TriggerProperties>();
            var trgrocs = await iterator.ReadNextAsync();

            var count = 0;
            foreach (var trgroc in trgrocs)
            {
                count++;
                Console.WriteLine($" Trigger Id: {trgroc.Id}; TriggerType: {trgroc.TriggerType}");
            }

            Console.WriteLine();
            Console.WriteLine($"Total triggers: {count}");
        }

        // Execute triggers

        private async static Task ExecuteTriggers()
        {
            Console.Clear();
            await Execute_trgPreValidateToDoItemTimestamp();
            await Execute_trgPostUpdateMetadata();
        }

        private async static Task Execute_trgPreValidateToDoItemTimestamp()
        {
            Console.WriteLine();
            Console.WriteLine("Execute trgPreValidateToDoItemTimestamp trigger");

            dynamic newItem = new
            {
                category = "Personal",
                name = "Groceries",
                description = "Pick up strawberries",
                isComplete = false
            };

            var container = Shared.Client.GetContainer("adventure-works", "stores");
            var pk = new PartitionKey(string.Empty);
            var result = await container.CreateItemAsync(newItem, pk, 
                new ItemRequestOptions { PreTriggers = new List<string> { "trgPreValidateToDoItemTimestamp" } });
            var message = result.Resource;

            Console.WriteLine($"Result: {message}");
        }

        private async static Task Execute_trgPostUpdateMetadata()
        {
            Console.WriteLine();
            Console.WriteLine("Execute trgPostUpdateMetadata trigger");

            dynamic newItem = new
            {
                category = "Personal",
                name = "Groceries",
                description = "Pick up strawberries",
                isComplete = false
            };

            var container = Shared.Client.GetContainer("adventure-works", "stores");
            var pk = new PartitionKey(string.Empty);
            var result = await container.CreateItemAsync(newItem, pk,
                new ItemRequestOptions { PostTriggers = new List<string> { "trgPostUpdateMetadata" } });
            var message = result.Resource;

            Console.WriteLine($"Result: {message}");
        }

        // Delete triggers

        private async static Task DeleteTriggers()
        {
            Console.WriteLine();
            Console.WriteLine(">>> Delete Triggers <<<");
            Console.WriteLine();

            await DeleteTrigger("trgPreValidateToDoItemTimestamp");
            await DeleteTrigger("trgPostUpdateMetadata");
        }

        private async static Task DeleteTrigger(string sprocId)
        {
            var container = Shared.Client.GetContainer("adventure-works", "stores");
            await container.Scripts.DeleteTriggerAsync(sprocId);

            Console.WriteLine($"Deleted trigger: {sprocId}");
        }
    }
}
