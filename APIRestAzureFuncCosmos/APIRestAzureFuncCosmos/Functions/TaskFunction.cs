using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Application.Services;
using Presentation.DTOs;

namespace Presentation.Functions;

public class TaskFunction(ITaskService taskService)
{
    private readonly ITaskService _taskService = taskService;

    [FunctionName("GetAllTasks")]
    public async Task<IActionResult> GetAllTasks([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        var tasks = await _taskService.GetAllTasksAsync();
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        return new OkObjectResult(data);
    }

    [FunctionName("CreateTask")]
    public async Task<IActionResult> CreateTask(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var createTaskDto = JsonConvert.DeserializeObject<CreateTaskDto>(requestBody);

        return new OkObjectResult(createTaskDto);
    }
}