using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LoggingService.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Severity
    {
        [JsonProperty(PropertyName = "fail")]
        Fail,
        [JsonProperty(PropertyName = "exception")]
        Exception,
        [JsonProperty(PropertyName = "warn")]
        Warn,
        [JsonProperty(PropertyName = "info")]
        Info
    }
}