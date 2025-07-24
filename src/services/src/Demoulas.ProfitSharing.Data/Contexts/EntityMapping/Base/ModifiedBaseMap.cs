using Demoulas.ProfitSharing.Data.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;

internal abstract class ModifiedBaseMap<TType> : IEntityTypeConfiguration<TType> where TType : ModifiedBase
{
    public virtual void Configure(EntityTypeBuilder<TType> builder)
    {

        builder.Property(e => e.ModifiedAtUtc);

        _ = builder.Property(e => e.UserName)
            .HasMaxLength(96)
            .HasColumnName("USER_NAME")
            .HasDefaultValueSql("SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");


        _ = builder.Property(e => e.CreatedAtUtc)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasColumnName("CREATED_AT")
            .HasColumnType("TIMESTAMP WITH TIME ZONE")
            .HasDefaultValueSql("SYSTIMESTAMP");

        _ = builder.Property(e => e.ModifiedAtUtc)
            .IsRequired()
            .ValueGeneratedOnUpdate()
            .HasColumnName("MODIFIED_AT_UTC")
            .HasColumnType("TIMESTAMP WITH TIME ZONE")
            .HasDefaultValueSql("SYSTIMESTAMP");
    }
}
