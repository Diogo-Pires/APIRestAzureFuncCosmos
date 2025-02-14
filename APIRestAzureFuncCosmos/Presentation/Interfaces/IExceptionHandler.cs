using Microsoft.AspNetCore.Mvc;
using System;

namespace Presentation.Interfaces;

public interface IExceptionHandler
{
    IActionResult HandleException(Exception ex);
}
