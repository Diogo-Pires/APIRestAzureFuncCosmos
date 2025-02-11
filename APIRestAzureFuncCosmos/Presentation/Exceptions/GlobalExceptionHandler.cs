using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Interfaces;
using System;
using System.Threading.Tasks;

namespace Presentation.Exceptions;
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public Task<IActionResult> HandleExceptionAsync(Exception ex)
    {
        _logger.LogError(ex, "An unhandled exception occurred.");

        var result = new ObjectResult(new { error = ex.Message })
        {
            StatusCode = 500
        };

        return Task.FromResult<IActionResult>(result);
    }
}