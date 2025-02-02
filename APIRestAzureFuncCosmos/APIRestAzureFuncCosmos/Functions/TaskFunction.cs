using System.Net;
using Application.Services;
using Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;


namespace Presentation.Functions;

public class TaskFunction
{
    private readonly TaskService _taskService;

    public TaskFunction(TaskService taskService)
    {
        _taskService = taskService;
    }

    [Function("GetAllTasks")]
    public async Task<HttpResponseData> GetAllTasks([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        var tasks = await _taskService.GetAllTasksAsync();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(tasks);
        return response;
    }

    [Function("CreateTask")]
    public async Task<HttpResponseData> CreateTask([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var task = await req.ReadFromJsonAsync<TaskItem>();
        await _taskService.CreateTaskAsync(task.Title, task.Description);

        var response = req.CreateResponse(HttpStatusCode.Created);
        return response;
    }
}
