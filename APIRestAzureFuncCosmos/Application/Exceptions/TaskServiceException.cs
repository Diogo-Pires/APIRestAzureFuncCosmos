namespace Application.Exceptions;

[Serializable]
internal class TaskServiceException(string? message, Exception? innerException) : Exception(message, innerException)
{
}