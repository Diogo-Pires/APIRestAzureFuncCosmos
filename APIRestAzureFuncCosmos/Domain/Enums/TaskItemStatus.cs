using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domain.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TaskItemStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}