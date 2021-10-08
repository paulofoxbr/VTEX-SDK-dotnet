using Newtonsoft.Json;

namespace VTEX.Transport
{
    public class Error
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("exception")]
        public object Exception { get; set; }
    }
}