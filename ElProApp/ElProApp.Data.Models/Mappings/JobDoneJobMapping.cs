namespace ElProApp.Data.Models.Mappings
{
    using Microsoft.EntityFrameworkCore;

    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationErrorMessage.Master;
    using static Common.EntityValidationErrorMessage.Job;
    using System.ComponentModel.DataAnnotations.Schema;

    public class JobDoneJobMapping
    {
        public Guid JobId { get; set; }

        public Job Job { get; set; }

        public Guid JobDoneId { get; set; }

        public JobDone JobDone { get; set; }

        [Range(0.01, 9999.99, ErrorMessage = ErrorMassagePozitive)]
        [RegularExpression(@"^\d{1,6}(\.\d{1,2})?$", ErrorMessage = ErrorMassageQuantity)]
        [Comment("The quantity of the job with up to 6 digits before the decimal point and up to 2 digits after.")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The date when the record was created.
        /// </summary>
        [Comment("The date when the record was created.")]
        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now.Date;
    }
}
