namespace Shared.Exceptions;

[Serializable]
public class DomainException(string? message) : Exception(message)
{
}