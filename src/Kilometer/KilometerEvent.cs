using System.Dynamic;
using Newtonsoft.Json;

namespace Kilometer
{
    public class KilometerEvent
    {
        [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; }

        [JsonProperty("event_type")]
        public string Type { get; set; }

        [JsonProperty("event_name")]
        public string Name { get; set; }

        [JsonProperty("event_properties")]
        public object Properties { get; set; }

        public KilometerEvent()
        {
            if (Properties == null)
            {
                Properties = new ExpandoObject();
            }
        }
    }
}