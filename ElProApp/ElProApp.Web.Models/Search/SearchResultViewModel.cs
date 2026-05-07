namespace ElProApp.Web.Models.Search
{
    public class SearchResultViewModel
    {
        public string Id { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? AdditionalInfo { get; set; }
    }
}