using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.Common.Data.Contexts.ValueConverters;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DefinitionMap : IEntityTypeConfiguration<Definition>
{
    public void Configure(EntityTypeBuilder<Definition> builder)
    {
        _ = builder.HasKey(e => e.Key);
        _ = builder.ToTable("Definition");

        _ = builder.Property(e => e.Key)
            .HasMaxLength(24)
            .ValueGeneratedNever()
            .IsRequired();

        _ = builder.Property(e => e.Description)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasData(new List<Definition>
        {
            new Definition { Key = "P", Description = "Part Time" },
            new Definition { Key = "H", Description = "Full time(straight salary)" },
            new Definition { Key = "G", Description = "Full time accrued paid holidays" },
            new Definition { Key = "F", Description = "Full time 8 paid holidays " },
            new Definition { Key = "PF", Description = "Pay frequency (1=weekly, 2=monthly)" }
        });
    }
}
