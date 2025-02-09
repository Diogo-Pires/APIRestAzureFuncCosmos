namespace Shared.Validations;

public class ErrorResponse
{
    public string Message { get; set; }
    public List<string> Errors { get; set; } = [];

    public ErrorResponse(string message, List<string>? errors = null)
    {
        Message = message;

        if (errors != null)
            Errors = errors;
    }
}
