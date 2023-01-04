using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Microsoft.Extensions.Configuration;

/*
 * https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/tables/Azure.Data.Tables
 * https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-overview#table-storage-concepts
 * URL format: Azure Table Storage accounts use this format: 
 * http://<storage account>.table.core.windows.net or http://<storage account>.table.core.windows.net/<table>
 */

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var connectionString = configuration["StorageConnectionString"];
var tableStorageAccountURITable = $@"{configuration["TableStorageAccountURITable"]}";
var accountName = $@"{configuration["AccountName"]}";
var storageAccountKey = $@"{configuration["StorageAccountKey"]}";

try
{
    string tableName = "MyTable1";

    var serviceClient = new TableServiceClient(
    new Uri(tableStorageAccountURITable),
    new TableSharedKeyCredential(accountName, storageAccountKey));

    var tableClient = serviceClient.GetTableClient(tableName);

    //var tableClient = new TableClient(
    //new Uri(tableStorageAccountURITable),
    //tableName,
    //new TableSharedKeyCredential(accountName, storageAccountKey));

    //await CreateTableWithServiceClient(serviceClient, "MyTable1");
    //await CreateTableWithTableClient(tableClient);

    //GetTableList(serviceClient, "MyTable1");

    //await DeleteTable(serviceClient, "MyTable1");

    //await AddItems(tableClient, "Cat", Guid.NewGuid().ToString());
    //await AddItems(tableClient, "Cat", Guid.NewGuid().ToString());
    //await AddItems(tableClient, "Cat", Guid.NewGuid().ToString());

}
catch (Exception e)
{
    Console.WriteLine(e);
}

static async Task CreateTableWithServiceClient(TableServiceClient tableServiceClient, string tableName)
{
    TableItem table = await tableServiceClient.CreateTableIfNotExistsAsync(tableName);
    Console.WriteLine($"The created table's name is {table.Name}.");
}

static async Task CreateTableWithTableClient(TableClient tableClient)
{
    TableItem table = await tableClient.CreateAsync();
    Console.WriteLine($"The created table's name is {table.Name}.");
}

static void GetTableList(TableServiceClient tableServiceClient, string tableName)
{
    Pageable<TableItem> queryTableResults = tableServiceClient.Query(filter: $"TableName eq '{tableName}'");

    Console.WriteLine("The following are the names of the tables in the query results:");

    // Iterate the <see cref="Pageable"> in order to access queried tables.

    foreach (TableItem table in queryTableResults)
    {
        Console.WriteLine(table.Name);
    }
}

static async Task DeleteTable(TableServiceClient tableServiceClient, string tableName)
{
    await tableServiceClient.DeleteTableAsync(tableName);
}

static async Task AddItems(TableClient tableClient, string partitionKey, string rowKey)
{
    var entity = new TableEntity(partitionKey, rowKey)
    {
        { "Product", "Marker Set" },
        { "Price", 5.00 },
        { "Quantity", 21 }
    };
    Console.WriteLine($"{entity.RowKey}: {entity["Product"]} costs ${entity.GetDouble("Price")}.");
    await tableClient.AddEntityAsync(entity);
}
