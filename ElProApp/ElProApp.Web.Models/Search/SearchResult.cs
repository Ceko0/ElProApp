namespace ElProApp.Web.Models.Search
{
    public class SearchResult<T> where T : class
    {
        public string? TableName { get; set; } 
        public List<T>? Results { get; set; }
    }
}
