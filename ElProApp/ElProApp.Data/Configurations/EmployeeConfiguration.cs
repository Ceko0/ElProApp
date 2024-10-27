namespace ElProApp.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;
    using static Common.EntityValidationConstants.Employee;

    /// <summary>
    /// Configuration class for the Employee entity, defining the schema for the Employee table.
    /// </summary>
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        /// <summary>
        /// Configures the Employee entity properties and relationships.
        /// </summary>
        /// <param name="builder">EntityTypeBuilder used to configure the Employee entity.</param>
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            // Sets the primary key for the Employee entity.
            builder.HasKey(e => e.Id);

            // Configures the FirstName property as required with a maximum length.
            builder.Property(e => e.FirstName)
                .IsRequired() // First name must not be null
                .HasMaxLength(NameMaxLength); // Maximum length defined by NameMaxLength constant

            // Configures the LastName property as required with a maximum length.
            builder.Property(e => e.LastName)
                .IsRequired() // Last name must not be null
                .HasMaxLength(NameMaxLength); // Maximum length defined by NameMaxLength constant

            // Configures the Wages property as required with a specific decimal type.
            builder.Property(e => e.Wages)
                .IsRequired() // Wages must not be null
                .HasColumnType("decimal(6, 2)"); // 6 digits total, 2 after the decimal point

            // Configures the MoneyToTake property with a specific decimal type.
            builder.Property(e => e.MoneyToTake)
                .HasColumnType("decimal(18, 2)"); // 18 digits total, 2 after the decimal point

            // Configures the one-to-one relationship between Employee and IdentityUser.
            builder.HasOne(e => e.User) // Specifies the navigation property
                .WithOne() // There is one IdentityUser per Employee
                .HasForeignKey<Employee>(e => e.UserId) // Specifies the foreign key
                .OnDelete(DeleteBehavior.Cascade); // Cascading delete on User deletion
        }
    }
}
