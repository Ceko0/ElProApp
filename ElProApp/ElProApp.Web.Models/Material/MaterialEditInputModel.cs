namespace ElProApp.Web.Models.Material
{
    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;
    using static Common.EntityValidationConstants.Material;
    using static Common.EntityValidationErrorMessage.Material;
    using static Common.EntityValidationErrorMessage.Master;

    public class MaterialEditInputModel : IMapFrom<Material> , IMapTo<Material>
    {
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, 9999.99, ErrorMessage = ErrorMassagePozitive)]
        [RegularExpression(@"^\d{1,6}(\.\d{1,2})?$", ErrorMessage = ErrorMassageQuantity)]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        public Guid BuildingId { get; set; }
    }
}
