namespace ElProApp.Services.Data.Interfaces
{
    public interface IDeletableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public  bool  IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
