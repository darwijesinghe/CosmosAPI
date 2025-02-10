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
        /// <param name="item">The data object to be added.</param>
        /// <param name="containerName">The name of the container (Table).</param>
        /// <param name="partitionKey">The partition key for the dataset.</param>
        public async Task AddTaskAsync<T>(T item, string containerName, string partitionKey)
        {
            // gets the container
            var container = GetContainer(containerName);

            // creates the item
            await container.CreateItemAsync(item, new PartitionKey(partitionKey));
        }

        /// <summary>
        /// Retrieves the dataset for the provide ID.
        /// </summary>
        /// <param name="id">The unique identifier of the item.</param>
        /// <param name="containerName">The name of the container (Table).</param>
        /// <returns>
        /// The <typeparamref name="T"/> object.
        /// </returns>
        public async Task<T> GetTaskAsync<T>(string id, string containerName)
        {
            // gets the container
            var container = GetContainer(containerName);

            // returns the data object
            return await container.ReadItemAsync<T>(id, new PartitionKey(id.ToString()));
        }

        /// <summary>
        /// Retrieves the dataset from the given query.
        /// </summary>
        /// <param name="query">The query to retrieves the data.</param>
        /// <param name="containerName">The name of the container (Table).</param>
        /// <returns>
        /// The list of <see cref="List{T}"/> object.
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
        /// Updates an existing item in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the item.</param>
        /// <param name="containerName">The name of the container (table).</param>
        /// <param name="item">The updated data object.</param>
        public async Task UpdateItemAsync<T>(string id, string containerName, T item)
        {
            // gets the container
            var container = GetContainer(containerName);

            // replaces the record set
            await container.ReplaceItemAsync(item, id, new PartitionKey(id.ToString()));
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
            await container.DeleteItemAsync<object>(id, new PartitionKey(id.ToString()));
        }
    }
}
