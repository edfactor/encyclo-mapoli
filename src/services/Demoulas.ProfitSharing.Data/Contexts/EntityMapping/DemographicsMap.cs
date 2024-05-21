using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping
{
    internal sealed class DemographicsMap : IEntityTypeConfiguration<Demographics>
    {
        public void Configure(EntityTypeBuilder<Demographics> builder)
        {

            //_ = builder.HasKey(e => e.ArId);

            //_ = builder.ToTable("AR_ACTIVE");

            //_ = builder.HasIndex(e => new { e.ArInvoiceDate, e.ArInvoice }, "IDX_AR_ACTIVE_INVOICE");

            //_ = builder.Property(e => e.ArId)
            //    .HasPrecision(15)
            //    .ValueGeneratedNever()
            //    .HasColumnName("AR_ID");
            //_ = builder.Property(e => e.ArAccount)
            //    .HasPrecision(10)
            //    .HasColumnName("AR_ACCOUNT");


        }
    }
}
