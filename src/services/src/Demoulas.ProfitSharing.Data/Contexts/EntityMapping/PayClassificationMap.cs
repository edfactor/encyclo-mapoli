using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class PayClassificationMap : IEntityTypeConfiguration<PayClassification>
{
    public void Configure(EntityTypeBuilder<PayClassification> builder)
    {
        builder.ToTable("PAY_CLASSIFICATION");
        builder.HasKey(e => e.Id);

        _ = builder.Property(e => e.Id)
            .HasMaxLength(4)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .IsRequired();

        _ = builder.Property(e => e.Name)
            .HasMaxLength(64)
            .IsRequired()
            .HasColumnName("NAME")
            .HasComment("Pay Classification");

        // Seed minimal set for now; full list should be migrated via data script/migration to avoid duplicate key issues during PK type change.
        builder.HasData(
            new PayClassification { Id = PayClassification.Constants.Manager, Name = "1-MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AssistantManager, Name = "2-ASSISTANT MANAGER" },
            new PayClassification { Id = PayClassification.Constants.SpiritsManager, Name = "4-SPIRITS MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AsstSpiritsManager, Name = "5-ASST SPIRITS MANAGER" },
            new PayClassification { Id = PayClassification.Constants.SpiritsClerkFt, Name = "6-SPIRITS CLERK - FT" },
            new PayClassification { Id = PayClassification.Constants.SpiritsClerkPt, Name = "7-SPIRITS CLERK - PT" },
            new PayClassification { Id = PayClassification.Constants.FrontEndManager, Name = "10-FRONT END MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AssistantHeadCashier, Name = "11-ASST HEAD CASHIER" },
            new PayClassification { Id = PayClassification.Constants.CashiersAm, Name = "13-CASHIERS - AM" }
        );
    }
}
