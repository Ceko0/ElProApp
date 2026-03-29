namespace ElProApp.Web.Models.JobDone
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ElProApp.Data.Models;
    using ElProApp.Services.Mapping;

    using static ElProApp.Common.EntityValidationConstants.JobDone;
    using static ElProApp.Common.EntityValidationErrorMessage.JobDobe;
    using static ElProApp.Common.EntityValidationErrorMessage.Master;

    /// <summary>
    /// Input model for creating a job-done record.
    /// </summary>
    public class JobDoneInputModel : IMapTo<JobDone>, IMapFrom<JobDone>
    {
        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Display(Name = "Име")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets start date.
        /// </summary>
        [Required(ErrorMessage = "Началната дата е задължителна.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets end date.
        /// </summary>
        [Required(ErrorMessage = "Крайната дата е задължителна.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets working days count.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(1, 30, ErrorMessage = ErrorMassageDaysForJob)]
        public int DaysForJob { get; set; }

        /// <summary>
        /// Gets or sets team identifier.
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// Gets or sets building identifier.
        /// </summary>
        public Guid BuildingId { get; set; }

        /// <summary>
        /// Gets or sets selected materials with quantities.
        /// </summary>
        public List<MaterialInputPair> Materials { get; set; } = new();

        /// <summary>
        /// Gets or sets available materials for selection.
        /// </summary>
        public ICollection<Material> MaterialsList { get; set; } = new List<Material>();

        /// <summary>
        /// Gets or sets available teams.
        /// </summary>
        public ICollection<Team> Teams { get; set; } = new List<Team>();

        /// <summary>
        /// Gets or sets available buildings.
        /// </summary>
        public ICollection<Building> Buildings { get; set; } = new List<Building>();
    }

    /// <summary>
    /// Represents material selection with quantity.
    /// </summary>
    public class MaterialInputPair
    {
        public string MaterialName { get; set; } = null!;
        /// <summary>
        /// Gets or sets material identifier.
        /// </summary>
        public Guid MaterialId { get; set; }

        /// <summary>
        /// Gets or sets quantity.
        /// </summary>
        public decimal Quantity { get; set; }
    }
}