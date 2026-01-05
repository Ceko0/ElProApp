namespace ElProApp.Web.Models.Material
{
    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class MaterialViewModel : IMapFrom<Material>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Quantity { get; set; }

        public Guid BuildingId { get; set; }

        public string BuildingName { get; set; } = null!;

        public IEnumerable<SelectListItem> Buildings { get; set; }
        = new List<SelectListItem>();

    }
}
