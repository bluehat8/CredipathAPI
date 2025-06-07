using System;
using System.Text.Json.Serialization;

namespace CredipathAPI.DTOs
{
    public class RouteResponseDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }


        [JsonPropertyName("description")]
        public string? Description { get; set; }


        [JsonPropertyName("status")]
        public string? Status { get; set; }


        [JsonPropertyName("district")]
        public string? District { get; set; }


        [JsonPropertyName("location")]
        public string? Location { get; set; }


        [JsonPropertyName("clientsCount")]
        public int ClientsCount { get; set; }


        [JsonPropertyName("collaboratorsCount")]
        public int CollaboratorsCount { get; set; }


        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }


        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }


        [JsonPropertyName("lastVisit")]
        public DateTime? LastVisit { get; set; }
    }
}
