using System.Text.Json;

namespace API.Helpers
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header) {
            // serialize this into json so that it can go back with the header in JSON formats
            // by default, return from JsonSerializerOptions is Pascal case
            var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            // allow cors-policy otherwise clients will not able to access the info (Pagination) inside this header
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}