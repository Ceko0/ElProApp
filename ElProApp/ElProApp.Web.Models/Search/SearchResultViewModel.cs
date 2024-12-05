namespace ElProApp.Web.Models.Search
{
    public class SearchResultViewModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? LastName { get; set; }

        public decimal? Money { get; set; }

        public string? Email { get; set; }

        public int DaysForJob { get; set; }

        public decimal Quantity { get; set; }
        public int CurrentPage { get; set; } 
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}
