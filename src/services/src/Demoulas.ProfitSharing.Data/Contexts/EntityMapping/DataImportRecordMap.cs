using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demoulas.Common.Data.Contexts.ValueConverters;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

public sealed class DataImportRecordMap : IEntityTypeConfiguration<DataImportRecord>
{
    public void Configure(EntityTypeBuilder<DataImportRecord> builder)
    {
        _ = builder.ToTable("DATA_IMPORT_RECORD");
        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(d => d.Id).HasColumnName("ID").ValueGeneratedOnAdd();
    }
}
