namespace ElProApp.Data.Models.Mappings
{
    public class JobMaterialMapping
    {
        public Guid JobId { get; set; }
        public Job Job { get; set; } = null!;

        public Guid MaterialId { get; set; }
        public Material Material { get; set; } = null!;

    }
}
