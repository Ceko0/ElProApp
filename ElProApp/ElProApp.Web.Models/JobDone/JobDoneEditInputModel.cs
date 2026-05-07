namespace ElProApp.Web.Models.JobDone
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    using ElProApp.Data.Models;
    using ElProApp.Services.Mapping;

    using static ElProApp.Common.EntityValidationConstants.JobDone;
    using static ElProApp.Common.EntityValidationErrorMessage.JobDobe;
    using static ElProApp.Common.EntityValidationErrorMessage.Master;

    /// <summary>
    /// Input model for editing a job-done record.
    /// </summary>
    public class JobDoneEditInputModel : IMapTo<JobDone>, IMapFrom<JobDone>
    {
        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Display(Name = "Име")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets working days count.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(1, 30, ErrorMessage = ErrorMassageDaysForJob)]
        public int DaysForJob { get; set; }

        /// <summary>
        /// Gets or sets building identifier.
        /// </summary>
        public Guid BuildingId { get; set; }

        /// <summary>
        /// Gets or sets building.
        /// </summary>
        [ValidateNever]
        public Building Building { get; set; } = null!;

        /// <summary>
        /// Gets or sets team identifier.
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// Gets or sets team.
        /// </summary>
        [ValidateNever]
        public Team Team { get; set; } = null!;

        /// <summary>
        /// Gets or sets selected materials with quantities.
        /// </summary>
        public List<MaterialInputPair> Materials { get; set; } = new();

        /// <summary>
        /// Gets or sets available materials for selection.
        /// </summary>
        [ValidateNever]
        public ICollection<Material> MaterialsList { get; set; } = new List<Material>();
    }
}