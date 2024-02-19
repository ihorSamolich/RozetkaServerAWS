using Newtonsoft.Json;

namespace WebRozetka.Models.Address
{
    public class NPWarehouseProperties
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 200;
    }

    public class NPWarehouseRequestViewModel
    {
        [JsonProperty(PropertyName = "apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty(PropertyName = "modelName")]
        public string ModelName { get; set; }

        [JsonProperty(PropertyName = "calledMethod")]
        public string CalledMethod { get; set; }

        [JsonProperty(PropertyName = "methodProperties")]
        public NPWarehouseProperties MethodProperties { get; set; }
    }
}
