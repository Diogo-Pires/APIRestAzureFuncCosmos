using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Application.DTOs;
using Application.Interfaces;
using Presentation.Enums;

namespace Presentation;

public class TaskFunction(ITaskService taskService)
{
    const string ROUTE_NAME = "task";
    private readonly ITaskService _taskService = taskService;

    [FunctionName("GetAllTasks")]
    public async Task<IActionResult> GetAllTasks(
        [HttpTrigger(AuthorizationLevel.Anonymous, RestVerbs.GET, Route = $"{ROUTE_NAME}s")] HttpRequest req)
    {
        var taskList = await _taskService.GetAllAsync();
        return new OkObjectResult(JsonConvert.SerializeObject(taskList));
    }

    [FunctionName("GetTaskById")]
    public async Task<IActionResult> GetTaskById(
        [HttpTrigger(AuthorizationLevel.Anonymous, RestVerbs.GET, Route = $"{ROUTE_NAME}/{{id}}")] HttpRequest req, string id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(task);
    }

    [FunctionName("CreateTask")]
    public async Task<IActionResult> CreateTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, RestVerbs.POST, Route = $"{ROUTE_NAME}")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var createTaskDto = JsonConvert.DeserializeObject<TaskDTO>(requestBody);

        var createdTask = await _taskService.CreateAsync(createTaskDto);

        return new CreatedAtActionResult(
            nameof(GetTaskById),
            "Task",          
            new { id = createdTask.Id },
            createdTask
        );
    }

    [FunctionName("UpdateTask")]
    public async Task<IActionResult> UpdateTask(
        [HttpTrigger(AuthorizationLevel.Function, RestVerbs.PUT, Route = $"{ROUTE_NAME}")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var taskToUpdate = JsonConvert.DeserializeObject<TaskDTO>(requestBody);

        if (taskToUpdate == null || string.IsNullOrWhiteSpace(taskToUpdate.Id))
            return new BadRequestObjectResult("Invalid task data");

        var updatedTask = await _taskService.UpdateAsync(taskToUpdate);
        if (updatedTask == null)
        { 
            return new NotFoundResult();
        }

        return new OkObjectResult(updatedTask);
    }

    [FunctionName("DeleteTask")]
    public async Task<IActionResult> DeleteTask(
        [HttpTrigger(AuthorizationLevel.Function, RestVerbs.DELETE, Route = $"{ROUTE_NAME}/{{id}}")] HttpRequest req,
        string id)
    {   
        var deleted = await _taskService.DeleteAsync(id);
        if (!deleted)
        {
            return new NotFoundResult();
        }

        return new NoContentResult();
    }

}