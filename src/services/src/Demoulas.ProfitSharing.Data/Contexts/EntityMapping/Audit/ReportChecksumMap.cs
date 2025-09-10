using System.Text.Json;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Audit;

internal sealed class ReportChecksumMap : ModifiedBaseMap<ReportChecksum>
{
    public override void Configure(EntityTypeBuilder<ReportChecksum> builder)
    {
        _ = builder.ToTable("REPORT_CHECKSUM");
        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(d => d.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(d => d.ReportType)
            .HasColumnName("REPORT_TYPE")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(d => d.ProfitYear)
            .HasColumnName("PROFIT_YEAR")
            .HasPrecision(4)
            .IsRequired();

        builder.Property(d => d.RequestJson)
            .HasColumnName("REQUEST_JSON")
            .HasColumnType("CLOB")
            .IsRequired();

        builder.Property(d => d.ReportJson)
            .HasColumnName("REPORT_JSON")
            .HasColumnType("CLOB")
            .IsRequired();

       builder.Property(d => d.KeyFieldsChecksumJson)
            .HasColumnName("KEYFIELDS_CHECKSUM_JSON")
            .HasColumnType("CLOB")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Web),
                v => JsonSerializer.Deserialize<List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>>(v, JsonSerializerOptions.Web) ?? new List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>()
            )
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IEnumerable<KeyValuePair<string, KeyValuePair<decimal, byte[]>>>>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.Key.GetHashCode(), v.Value.Value.GetHashCode())),
                c => c.ToList()));
        
        base.Configure(builder);
    }
}
