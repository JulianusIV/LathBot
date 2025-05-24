using System.Text.Json.Serialization;

namespace LathBotFront.EventHandlers
{
    public class PKMessageModel
    {
        [JsonPropertyName("original")]
        public string Original { get; set; }
        [JsonPropertyName("sender")]
        public string Sender { get; set; }
        [JsonPropertyName("system")]
        public object System { get; set; }

    }
}
