using API.Enums;
using Microsoft.Azure.Cosmos;

namespace API.Services
{
    /// <summary>
    /// Provides data manipulation functions for the Cosmos DB.
    /// </summary>
    public class CosmosService
    {
        // Services
        private readonly CosmosClient _cosmosClient;

        // Variables
        private readonly string _databaseName;

        public CosmosService(CosmosClient client, string dbName)
        {
            _cosmosClient = client;
            _databaseName = dbName;
        }

        /// <summary>
        /// Returns the required container (Table) name.
        /// </summary>
        /// <param name="containerName">The name of the container.</param>
        /// <returns>
        /// A class object of <see cref="Container"/>.
        /// </returns>
        private Container GetContainer(string containerName)
        {
            return _cosmosClient.GetContainer(_databaseName, containerName);
        }

        /// <summary>
        /// Creates a new item to the db.
        /// </summary>
        /// <param name="data">The data object to be added.</param>
        /// <param name="containerName">The name of the container (Table).</param>
        /// <param name="partitionKey">The partition key for the dataset.</param>
        public async Task AddTaskAsync<T>(T data, string containerName, string partitionKey)
        {
            // gets the container
            var container = GetContainer(containerName);

            // creates the item
            await container.CreateItemAsync(
                data, 
                new PartitionKey(partitionKey),
                new ItemRequestOptions
                {
                    PreTriggers  = new List<string> { "checkSameName" }, // pre-trigger
                    PostTriggers = new List<string> { "insertLog" }      // post-trigger
                }
            );
        }

        /// <summary>
        /// Retrieves the dataset for the provide ID.
        /// </summary>
        /// <param name="id">The unique identifier of the item.</param>
        /// <param name="containerName">The name of the container (Table).</param>
        /// <returns>
        /// The result of type <typeparamref name="T"/>.
        /// </returns>
        public async Task<T> GetTaskAsync<T>(string id, string containerName)
        {
            // gets the container
            var container = GetContainer(containerName);

            // returns the data object
            return await container.ReadItemAsync<T>(id, new PartitionKey(nameof(Keys.TaskPartitionKey)));
        }

        /// <summary>
        /// Retrieves the dataset from the given query.
        /// </summary>
        /// <param name="query">The query to retrieves the data.</param>
        /// <param name="containerName">The name of the container (Table).</param>
        /// <returns>
        /// A collection of type <see cref="IEnumerable{T}"/>.
        /// </returns>
        public async Task<IEnumerable<T>> GetTasksAsync<T>(string query, string containerName)
        {
            // holds the temp data
            var results = new List<T>();

            // gets the container
            var container = GetContainer(containerName);

            // crates the query definition
            var queryDef = new QueryDefinition(query);

            // gets the dataset
            var iterator = container.GetItemQueryIterator<T>(queryDef);

            while (iterator.HasMoreResults)
            {
                // reads the response
                var response = await iterator.ReadNextAsync();

                // adds data to the list
                results.AddRange(response);
            }

            // returns the result
            return results;
        }

        /// <summary>
        /// Retrieves the dataset from the given query.
        /// </summary>
        /// <param name="query">The query to retrieves the data.</param>
        /// <param name="containerName">The name of the container (Table).</param>
        /// <returns>
        /// A collection of type <see cref="IEnumerable{dynamic}"/>.
        /// </returns>
        public async Task<IEnumerable<dynamic>> GetTasksAsync(string query, string containerName)
        {
            // holds the temp data
            var results   = new List<dynamic>();

            // gets the container
            var container = GetContainer(containerName);

            // creates the query definition
            var queryDef  = new QueryDefinition(query);

            // gets the dataset
            var iterator  = container.GetItemQueryIterator<dynamic>(queryDef);

            while (iterator.HasMoreResults)
            {
                // reads the response
                var response = await iterator.ReadNextAsync();

                // adds the raw response items to the list
                results.AddRange(response);
            }

            // returns the result
            return results;
        }

        /// <summary>
        /// Updates an existing item in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the item.</param>
        /// <param name="containerName">The name of the container (table).</param>
        /// <param name="data">The updated data object.</param>
        public async Task UpdateItemAsync<T>(string id, string containerName, T data)
        {
            // gets the container
            var container = GetContainer(containerName);

            // replaces the record set
            await container.ReplaceItemAsync(data, id, new PartitionKey(nameof(Keys.TaskPartitionKey)));
        }

        /// <summary>
        /// Deletes an existing item in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the item.</param>
        /// <param name="containerName">The name of the container (table).</param>
        public async Task DeleteItemAsync(string id, string containerName)
        {
            // gets the container
            var container = GetContainer(containerName);

            // deletes the record set from the db
            await container.DeleteItemAsync<object>(id, new PartitionKey(nameof(Keys.TaskPartitionKey)));
        }

        /// <summary>
        /// Inserts batch data to the database.
        /// </summary>
        /// <param name="containerName">The name of the container (table).</param>
        /// <param name="data">The batch data which is to be inserted.</param>
        /// <returns>
        /// The number of rows that inserted.
        /// </returns>
        public async Task<int> ExecuteBulkInsertAsync<T>(string containerName, List<T> data)
        {
            // gets the container
            var container = GetContainer(containerName);

            // gets the result of the execution
            var result    = await container.Scripts.ExecuteStoredProcedureAsync<int>(
                "bulkInsert",
                new PartitionKey(nameof(Keys.TaskPartitionKey)),
                new dynamic[] { data }
            );

            // returns the response
            return result.Resource;
        }
    }
}
