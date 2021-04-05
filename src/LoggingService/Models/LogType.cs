using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LoggingService.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogType
    {
        SystemLog,
        UserBehavior
    }
}
