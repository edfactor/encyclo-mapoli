using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class DepartmentMap : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Department");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasData(
            new Department { Id = Department.Constants.Grocery, Name = "Grocery" },
            new Department { Id = Department.Constants.Meat, Name = "Meat" },
            new Department { Id = Department.Constants.Produce, Name = "Produce" },
            new Department { Id = Department.Constants.Deli, Name = "Deli" },
            new Department { Id = Department.Constants.Dairy, Name = "Dairy" },
            new Department { Id = Department.Constants.Beer_And_Wine, Name = "Beer and Wine" },
            new Department { Id = Department.Constants.Bakery, Name = "Bakery" }
        );
    }
}
