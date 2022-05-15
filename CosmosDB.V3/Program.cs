using CosmosDB.V3;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

/*
 * https://docs.microsoft.com/en-us/azure/cosmos-db/sql/sql-api-get-started
 * https://docs.microsoft.com/en-us/azure/cosmos-db/sql/sql-api-sdk-dotnet-standard
 * https://docs.microsoft.com/en-us/azure/cosmos-db/sql/sql-api-dotnet-v3sdk-samples
 * https://github.com/Azure/azure-cosmos-dotnet-v3/tree/master/Microsoft.Azure.Cosmos.Samples/Usage
 */

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

// The Azure Cosmos DB endpoint for running this sample.
var EndpointUri = configuration["CosmosEndpoint"];
// The primary key for the Azure Cosmos account.
var PrimaryKey = $@"{configuration["CosmosMasterKey"]}";

// The Cosmos client instance
CosmosClient cosmosClient;

// The database we will create
Database database;

// The container we will create.
Container container;

// The name of the database and container we will create
var databaseId = "FamilyDatabase";
var containerId = "FamilyContainer";


try
{
    CosmosDB__V3 cosmosDB__V3 = new(EndpointUri, PrimaryKey);
    await cosmosDB__V3.CreateDatabaseContainerAsync(databaseId, containerId);

    await cosmosDB__V3.QueryItemsAsync();

    Family andersenFamily = new Family
    {
        Id = "Andersen.1",
        LastName = "Andersen",
        Parents = new Parent[]
    {
            new Parent { FirstName = "Thomas" },
            new Parent { FirstName = "Mary Kay" }
    },
        Children = new Child[]
    {
            new Child
            {
                FirstName = "Henriette Thaulow",
                Gender = "female",
                Grade = 5,
                Pets = new Pet[]
                {
                    new Pet { GivenName = "Fluffy" }
                }
            }
    },
        Address = new Address { State = "WA", County = "King", City = "Seattle" },
        IsRegistered = false
    };
    Family wakefieldFamily = new Family
    {
        Id = "Wakefield.7",
        LastName = "Wakefield",
        Parents = new Parent[]
    {
            new Parent { FamilyName = "Wakefield", FirstName = "Robin" },
            new Parent { FamilyName = "Miller", FirstName = "Ben" }
    },
        Children = new Child[]
    {
            new Child
            {
                FamilyName = "Merriam",
                FirstName = "Jesse",
                Gender = "female",
                Grade = 8,
                Pets = new Pet[]
                {
                    new Pet { GivenName = "Goofy" },
                    new Pet { GivenName = "Shadow" }
                }
            },
            new Child
            {
                FamilyName = "Miller",
                FirstName = "Lisa",
                Gender = "female",
                Grade = 1
            }
    },
        Address = new Address { State = "NY", County = "Manhattan", City = "NY" },
        IsRegistered = true
    };


    await cosmosDB__V3.AddItemsToContainerAsync(andersenFamily);
    await cosmosDB__V3.AddItemsToContainerAsync(wakefieldFamily);

}
catch (CosmosException cosmosException)
{
    Console.WriteLine("Cosmos Exception with Status {0} : {1}\n", cosmosException.StatusCode, cosmosException);
}
catch (Exception e)
{
    Console.WriteLine("Error: {0}", e);
}
