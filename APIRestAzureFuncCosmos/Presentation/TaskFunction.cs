using Application.DTOs;
using Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Shared.Enums;
using Shared.Validations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Presentation;

public class TaskFunction(ITaskService taskService, IValidator<TaskDTO> createValidator)
{
    const string ROUTE_NAME = "task";
    private readonly ITaskService _taskService = taskService;
    private readonly IValidator<TaskDTO> _createValidator = createValidator;

    /// <summary>
    /// Get all tasks.
    /// </summary>
    /// <returns><see cref="List<TaskDTO>"/></returns>
    /// <remarks>
    /// Usage Example:
    /// GET tasks
    ///
    /// Headers
    /// Accept: application/json
    /// </remarks>
    /// <response code="200">Ok</response>
    [OpenApiOperation(operationId: nameof(GetAllTasks), tags: [ROUTE_NAME])]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: RestUtilityConsts.APPJSON, bodyType: typeof(List<TaskDTO>), Description = "Get all the task")]
    [FunctionName(nameof(GetAllTasks))]
    public async Task<IActionResult> GetAllTasks(
        [HttpTrigger(AuthorizationLevel.Anonymous, RestUtilityConsts.GET, Route = $"{ROUTE_NAME}s")] HttpRequest req)
    {
        var taskList = await _taskService.GetAllAsync();
        return new OkObjectResult(JsonConvert.SerializeObject(taskList));
    }

    /// <summary>
    /// Get a task by id.
    /// </summary>
    /// <param name="nameof(id)"></param>
    /// <returns><see cref="TaskDTO"/></returns>
    /// <remarks>
    /// Usage Example:
    /// GET task/id
    ///
    /// Headers
    /// Accept: application/json
    /// </remarks>
    /// <response code="200">Ok</response>
    /// <response code="404">Not Found</response>
    [OpenApiOperation(operationId: nameof(GetTaskById), tags: [ROUTE_NAME])]
    [OpenApiParameter(name: nameof(id), In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Task's id")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: RestUtilityConsts.APPJSON, bodyType: typeof(TaskDTO), Description = "Get a task by id")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: RestUtilityConsts.APPJSON, bodyType: typeof(ErrorResponse), Description = "Model for errors")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Task was not found")]
    [FunctionName(nameof(GetTaskById))]
    public async Task<IActionResult> GetTaskById(
        [HttpTrigger(AuthorizationLevel.Anonymous, RestUtilityConsts.GET, Route = $"{ROUTE_NAME}/{{id}}")] HttpRequest req, string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return new BadRequestObjectResult(RestUtilityConsts.VALIDATION_ID_NOT_EMPTY);
        }

        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(task);
    }

    /// <summary>
    /// Creates a task.
    /// </summary>
    /// <param name="nameof(req)"></param>
    /// <returns><see cref="TaskDTO"/></returns>
    /// <remarks>
    /// Usage Example:
    /// POST task/
    /// {
    /// "title": "Aprender Azure 41",
    /// "description": "Estudar Azure Functions 2",
    /// "isCompleted": false
    ///}
    ///
    /// Headers
    /// Accept: application/json
    /// </remarks>
    /// <response code="201">Created</response>
    /// <response code="400">Bad Request</response>
    [OpenApiOperation(operationId: nameof(CreateTask), tags: [ROUTE_NAME])]
    [OpenApiParameter(name: nameof(req), In = ParameterLocation.Path, Required = true, Type = typeof(TaskDTO), Description = "A new task")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: RestUtilityConsts.APPJSON, bodyType: typeof(TaskDTO), Description = "Creates a new task")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Provided task was wrongly formated")]
    [FunctionName(nameof(CreateTask))]
    public async Task<IActionResult> CreateTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, RestUtilityConsts.POST, Route = $"{ROUTE_NAME}")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var createTaskDto = JsonConvert.DeserializeObject<TaskDTO>(requestBody);

        if (createTaskDto == null)
        {
            return new BadRequestObjectResult(new { Error = RestUtilityConsts.VALIDATION_INVALID_JSON_REQUEST });
        }

        var validationResult = await _createValidator.ValidateAsync(createTaskDto);
        if (!validationResult.IsValid)
        {
            return new BadRequestObjectResult(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var createdTask = await _taskService.CreateAsync(createTaskDto);

        return new CreatedAtActionResult(
            nameof(GetTaskById),
            "Task",
            new { id = createdTask.Id },
            createdTask
        );
    }

    /// <summary>
    /// Updates a task.
    /// </summary>
    /// <param name="nameof(req)"></param>
    /// <returns><see cref="TaskDTO"/></returns>
    /// <remarks>
    /// Usage Example:
    /// PUT task/
    /// {
    /// "id": "fc7c69b1-27cb-4dd9-a633-45cce665a563",
    /// "title": "Aprender Azure 41",
    /// "description": "Estudar Azure Functions 2",
    /// "isCompleted": false
    ///}
    ///
    /// Headers
    /// Accept: application/json
    /// </remarks>
    /// <response code="200">Ok</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">Not Found</response>
    [OpenApiOperation(operationId: nameof(UpdateTask), tags: [ROUTE_NAME])]
    [OpenApiParameter(name: nameof(req), In = ParameterLocation.Path, Required = true, Type = typeof(TaskDTO), Description = "A task to be update")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: RestUtilityConsts.APPJSON, bodyType: typeof(TaskDTO), Description = "Updates a task")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: RestUtilityConsts.APPJSON, bodyType: typeof(string), Description = "Provided task was wrongly formated")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Task was not found")]
    [FunctionName(nameof(UpdateTask))]
    public async Task<IActionResult> UpdateTask(
        [HttpTrigger(AuthorizationLevel.Function, RestUtilityConsts.PUT, Route = $"{ROUTE_NAME}")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var updateTaskDto = JsonConvert.DeserializeObject<TaskDTO>(requestBody);

        if (updateTaskDto == null)
        {
            return new BadRequestObjectResult(new { Error = RestUtilityConsts.VALIDATION_INVALID_JSON_REQUEST });
        }

        if (string.IsNullOrWhiteSpace(updateTaskDto.Id))
        {
            return new BadRequestObjectResult(RestUtilityConsts.VALIDATION_ID_NOT_EMPTY);
        }

        var updatedTask = await _taskService.UpdateAsync(updateTaskDto);
        if (updatedTask == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(updatedTask);
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="nameof(id)"></param>
    /// <returns><see cref="NoContentResult"/></returns>
    /// <remarks>
    /// Usage Example:
    /// DELETE task/id
    ///
    /// Headers
    /// Accept: application/json
    /// </remarks>
    /// <response code="204">No Content</response>
    /// <response code="404">Not Found</response>
    [FunctionName(nameof(DeleteTask))]
    [OpenApiOperation(operationId: nameof(DeleteTask), tags: [ROUTE_NAME])]
    [OpenApiParameter(name: nameof(req), In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The id of the task to be delete")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "Deletes a task")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: RestUtilityConsts.APPJSON, bodyType: typeof(ErrorResponse), Description = "Model for errors")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Task was not found")]
    public async Task<IActionResult> DeleteTask(
        [HttpTrigger(AuthorizationLevel.Function, RestUtilityConsts.DELETE, Route = $"{ROUTE_NAME}/{{id}}")] HttpRequest req, string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return new BadRequestObjectResult(RestUtilityConsts.VALIDATION_ID_NOT_EMPTY);
        }

        var deleted = await _taskService.DeleteAsync(id);
        if (!deleted)
        {
            return new NotFoundResult();
        }

        return new NoContentResult();
    }
}