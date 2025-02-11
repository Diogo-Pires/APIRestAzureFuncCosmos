using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Presentation.Interfaces;

public interface IExceptionHandler
{
    Task<IActionResult> HandleExceptionAsync(Exception ex);
}
