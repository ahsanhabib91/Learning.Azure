using Microsoft.Azure.Cosmos;
using System.Net;

namespace CosmosDB.V3;

public class CosmosDB__V3
{
    // The Azure Cosmos DB endpoint for running this sample.
    private static readonly string EndpointUri = "<your endpoint here>";
    // The primary key for the Azure Cosmos account.
    private static readonly string PrimaryKey = "<your primary key>";

    // The Cosmos client instance
    private CosmosClient cosmosClient;

    // The database we will create
    private Database database;

    // The container we will create.
    private Container container;

    // The name of the database and container we will create
    private string databaseId = "FamilyDatabase";
    private string containerId = "FamilyContainer";

    public CosmosDB__V3(string EndpointUri, string PrimaryKey)
    {
        cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
    }

    public async Task CreateDatabaseContainerAsync(string databaseId, string containerId)
    {
        this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        Console.WriteLine("Created Database: {0}\n", this.database.Id);

        this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/LastName");
        Console.WriteLine("Created Container: {0}\n", this.container.Id);
    }

    public async Task AddItemsToContainerAsync(Family family)
    {
        try
        {
            //Read the item to see if it exists.
            ItemResponse<Family> familyResponse = await this.container.ReadItemAsync<Family>(family.Id, new PartitionKey(family.LastName));
            Console.WriteLine("Item in database with id: {0} already exists\n", familyResponse.Resource.Id);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
            ItemResponse<Family> familyResponse = await this.container.CreateItemAsync<Family>(family, new PartitionKey(family.LastName));

            // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", familyResponse.Resource.Id, familyResponse.RequestCharge);
        }
    }

    public async Task QueryItemsAsync()
    {
        var sqlQueryText = "SELECT * FROM c WHERE c.LastName = 'Andersen'";

        Console.WriteLine("Running query: {0}\n", sqlQueryText);

        QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
        using FeedIterator<Family> queryResultSetIterator = this.container.GetItemQueryIterator<Family>(queryDefinition);

        List<Family> families = new List<Family>();

        while (queryResultSetIterator.HasMoreResults)
        {
            FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
            foreach (Family family in currentResultSet)
            {
                families.Add(family);
                Console.WriteLine("\tRead {0}\n", family);
            }
        }
    }

    private async Task ReplaceFamilyItemAsync(string id, string partitionKeyValue)
    {
        ItemResponse<Family> familyResponse = await this.container.ReadItemAsync<Family>(id, new PartitionKey(partitionKeyValue));
        var itemBody = familyResponse.Resource;

        // update registration status from false to true
        itemBody.IsRegistered = true;
        // update grade of child
        itemBody.Children[0].Grade = 6;

        // replace the item with the updated content
        familyResponse = await this.container.ReplaceItemAsync<Family>(itemBody, itemBody.Id, new PartitionKey(itemBody.LastName));
        Console.WriteLine("Updated Family [{0},{1}].\n \tBody is now: {2}\n", itemBody.LastName, itemBody.Id, familyResponse.Resource);
    }

    private async Task DeleteFamilyItemAsync(string id, string partitionKeyValue)
    {
        // Delete an item. Note we must provide the partition key value and id of the item to delete
        ItemResponse<Family> familyResponse = await this.container.DeleteItemAsync<Family>(id, new PartitionKey(partitionKeyValue));
        Console.WriteLine("Deleted Family [{0},{1}]\n", partitionKeyValue, id);
    }

    private async Task DeleteDatabaseAndCleanupAsync()
    {
        DatabaseResponse databaseResourceResponse = await this.database.DeleteAsync();
        // Also valid: await this.cosmosClient.Databases["FamilyDatabase"].DeleteAsync();

        Console.WriteLine("Deleted Database: {0}\n", this.databaseId);

        //Dispose of CosmosClient
        this.cosmosClient.Dispose();
    }

}
