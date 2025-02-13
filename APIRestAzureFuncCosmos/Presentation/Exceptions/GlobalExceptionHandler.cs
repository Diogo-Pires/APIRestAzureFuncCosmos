using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Interfaces;
using System;

namespace Presentation.Exceptions;
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public IActionResult HandleException(Exception ex)
    {
        var msg = "An unhandled exception occurred.";
        _logger.LogError(ex, msg);

        return new ObjectResult(new { error = msg })
        {
            StatusCode = 500
        };
    }
}