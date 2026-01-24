namespace ElProApp.Web.Models.Material
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationErrorMessage.Material;
    using static Common.EntityValidationErrorMessage.Master;

    public class BuildingMaterialViewModel
    {
        [Required]
        public Guid MaterialId { get; set; }

        [Required]
        public string MaterialName { get; set; } = null!;

        [Required(ErrorMessage = ErrorMassageQuantity)]
        [Range(0, 999999.99, ErrorMessage = ErrorMassagePozitive)]
        public decimal Quantity { get; set; }

        [Required]
        public Guid BuildingId { get; set; }

        [Required]
        public string BuildingName { get; set; } = null!;
    }
}
