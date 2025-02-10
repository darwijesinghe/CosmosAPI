using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CosmosController : ControllerBase
    {
        // Services
        private readonly CosmosService        _cosmosService;
        private readonly IReadOnlySet<string> _allowedContainers;

        public CosmosController(CosmosService cosmosService, IReadOnlySet<string> allowedContainers)
        {
            _cosmosService     = cosmosService;
            _allowedContainers = allowedContainers;
        }

        /// <summary>
        /// Creates a new item to the db.
        /// </summary>
        /// <param name="containerName">The name of the container (Table).</param>
        /// <param name="task">The data to be added to the database.</param>
        /// <returns>
        /// A <see cref="OkResult"/> if success; otherwise error message.
        /// </returns>
        [HttpPost("CreateRecord")]
        public async Task<IActionResult> AddTaskAsync(string containerName, [FromBody] TaskItem task)
        {
            try
            {
                // checks the container name
                if (!_allowedContainers.Contains(containerName))
                    return BadRequest("Invalid container name.");

                // adds data to the db
                await _cosmosService.AddTaskAsync(task, containerName, task.Id.ToString());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            // returns the result
            return Ok();
        }

        /// <summary>
        /// Retrieves the data by given ID.
        /// </summary>
        /// <param name="id">The unique key of the record set.</param>
        /// <param name="containerName">The name of the container (Table).</param>
        /// <returns>
        /// A <see cref="OkResult"/> if success; otherwise error message.
        /// </returns>
        [HttpGet("GetTask")]
        public async Task<IActionResult> GetTaskAsync(string id, string containerName)
        {
            try
            {
                // checks the container name
                if (!_allowedContainers.Contains(containerName))
                    return BadRequest("Invalid container name.");

                // retrieves the data
                var task = await _cosmosService.GetTaskAsync<TaskItem>(id, containerName);
                if (task is null)
                    return Problem("Required data not found.");

                // returns the result
                return Ok(task);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the all the data.
        /// </summary>
        /// <param name="containerName">The name of the container (Table).</param>
        /// <returns>
        /// A <see cref="OkResult"/> if success; otherwise error message.
        /// </returns>
        [HttpGet("GetTasks")]
        public async Task<IActionResult> GetTasksAsync(string containerName)
        {
            try
            {
                // checks the container name
                if (!_allowedContainers.Contains(containerName))
                    return BadRequest("Invalid container name.");

                // retrieves the data
                var tasks = await _cosmosService.GetTasksAsync<TaskItem>("select * from c", containerName);
                if (tasks is null || !tasks.Any())
                    return Problem("Required data not found.");

                // returns the result
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing record in the database.
        /// </summary>
        /// <param name="id">The unique key of the record set.</param>
        /// <param name="containerName">The name of the container (Table).</param>
        /// <param name="task">The updated data object.</param>
        /// <returns>
        /// A <see cref="OkResult"/> if success; otherwise error message.
        /// </returns>
        [HttpPut("UpdateTask")]
        public async Task<IActionResult> UpdateItemAsync(string id, string containerName, [FromBody] TaskItem task)
        {
            try
            {
                // checks the container name
                if (!_allowedContainers.Contains(containerName))
                    return BadRequest("Invalid container name.");

                // checks the record existence
                var found = await _cosmosService.GetTaskAsync<TaskItem>(id, containerName);
                if(found is null)
                    return BadRequest("No data found to update.");

                // updates the record set
                await _cosmosService.UpdateItemAsync<TaskItem>(id, containerName, task);

                // returns the result
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an existing record in the database.
        /// </summary>
        /// <param name="id">The unique key of the record set.</param>
        /// <param name="containerName">The name of the container (Table).</param>
        /// <returns>
        /// A <see cref="OkResult"/> if success; otherwise error message.
        /// </returns>
        [HttpDelete("DeleteTask")]
        public async Task<IActionResult> DeleteItemAsync(string id, string containerName)
        {
            try
            {
                // checks the container name
                if (!_allowedContainers.Contains(containerName))
                    return BadRequest("Invalid container name.");

                // checks the record existence
                var found = await _cosmosService.GetTaskAsync<TaskItem>(id, containerName);
                if(found is null)
                    return BadRequest("No data found to delete.");

                // updates the record set
                await _cosmosService.DeleteItemAsync(id, containerName);

                // returns the result
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
