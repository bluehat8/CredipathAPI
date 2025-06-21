using System.Text.Json.Serialization;

namespace CredipathAPI.DTOs
{
    public class RouteSimpleDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
