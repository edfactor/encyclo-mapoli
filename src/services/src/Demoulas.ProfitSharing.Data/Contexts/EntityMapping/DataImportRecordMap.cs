using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

public sealed class DataImportRecordMap : IEntityTypeConfiguration<DataImportRecord>
{
    public void Configure(EntityTypeBuilder<DataImportRecord> builder)
    {
        _ = builder.ToTable("DATA_IMPORT_RECORD");
        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(d => d.Id).HasColumnName("ID").ValueGeneratedOnAdd();
        _ = builder.Property(d => d.SourceSchema).HasColumnName("SOURCE_SCHEMA").HasMaxLength(50).IsRequired();
        _ = builder.Property(d => d.ImportDateTimeUtc).HasColumnName("IMPORT_DATE_TIME_UTC").IsRequired();
    }
}
