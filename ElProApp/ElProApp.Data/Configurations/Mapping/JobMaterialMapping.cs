namespace ElProApp.Data.Configurations.Mapping
{
    using ElProApp.Data.Models;

    public class JobMaterialMapping
    {
        public Guid JobId { get; set; }
        public Job Job { get; set; }

        public Guid MaterialId { get; set; }
        public Material Material { get; set; }
    }
}
