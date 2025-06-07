using System.Text.Json.Serialization;

namespace CredipathAPI.DTOs
{
    public class PagedResponse<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = new List<T>();

        [JsonPropertyName("pagination")]
        public PaginationMetadata Pagination { get; set; } = new PaginationMetadata();
    }

    public class PaginationMetadata
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }


        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }


        [JsonPropertyName("total")]
        public int Total { get; set; }


        [JsonPropertyName("totalPages")]
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(Total / (double)PageSize) : 0;
    }
}
