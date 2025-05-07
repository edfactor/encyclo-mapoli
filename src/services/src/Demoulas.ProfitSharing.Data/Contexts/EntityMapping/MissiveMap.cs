using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
public class MissiveMap : IEntityTypeConfiguration<Missive>
{
    public void Configure(EntityTypeBuilder<Missive> builder)
    {
        builder.ToTable("MISSIVES");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id).HasColumnName("ID");

        builder.Property(f => f.Message)
            .HasColumnName("MESSAGE");

        builder.HasData(
            new Missive() { Id = Missive.Constants.VestingIncreasedOnCurrentBalance, Message= "** VESTING INCREASED ON   CURRENT BALANCE ( > 1000 HRS) **" },
            new Missive() { Id = Missive.Constants.VestingIsNow100Percent, Message = "VEST IS NOW 100%, 65+/5 YRS" }
        );
    }
}
