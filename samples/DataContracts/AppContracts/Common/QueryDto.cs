using System.Collections.Generic;

namespace AppContracts.Common
{
    public class QueryDto
    {
        public List<FilterDto> Filters { get; init; } = new List<FilterDto>();
        public List<string> Sorts { get; init; } = new List<string>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public record FilterDto(string FieldName, string Comparision, string FieldValue);
}
