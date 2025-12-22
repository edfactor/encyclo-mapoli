using System;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class CommentTypeMap : ModifiedBaseMap<CommentType>
{
    public override void Configure(EntityTypeBuilder<CommentType> builder)
    {
        base.Configure(builder);

        builder.HasKey(x => x.Id);
        builder.ToTable("COMMENT_TYPE");

        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever().HasColumnName("ID");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128).HasColumnName("NAME");

        builder.HasData(GetPredefinedCommentTypes());
    }

    private static List<CommentType> GetPredefinedCommentTypes() =>
    [
        new CommentType { Id = 1, Name = "Transfer Out" },
        new CommentType { Id = 2, Name = "Transfer In" },
        new CommentType { Id = 3, Name = "QDRO Out" },
        new CommentType { Id = 4, Name = "QDRO In" },
        new CommentType { Id = 5, Name = "V-Only" },
        new CommentType { Id = 6, Name = "Forfeit" },
        new CommentType { Id = 7, Name = "Un-Forfeit" },
        new CommentType { Id = 8, Name = "Class Action" },
        new CommentType { Id = 9, Name = "Voided" },
        new CommentType { Id = 10, Name = "Hardship" },
        new CommentType { Id = 11, Name = "Distribution" },
        new CommentType { Id = 12, Name = "Payoff" },
        new CommentType { Id = 13, Name = "Dirpay" },
        new CommentType { Id = 14, Name = "Rollover" },
        new CommentType { Id = 15, Name = "Roth IRA" },
        new CommentType { Id = 16, Name = "> 64 - 1 Year Vested" },
        new CommentType { Id = 17, Name = "> 64 - 2 Year Vested" },
        new CommentType { Id = 18, Name = "> 64 - 3 Year Vested" },
        new CommentType { Id = 19, Name = "Military" },
        new CommentType { Id = 20, Name = "Other" },
        new CommentType { Id = 21, Name = "Rev" },
        new CommentType { Id = 22, Name = "Unrev" },
        new CommentType { Id = 23, Name = "100% Earnings" },
        new CommentType { Id = 24, Name = ">64 & >5 100%" },
        new CommentType { Id = 25, Name = "Forfeit Class Action" },
        new CommentType { Id = 26, Name = "Forfeit Administrative" },
        new CommentType { Id = 27, Name = "Administrative - taking money from under 21" },
        new CommentType { Id = 28, Name = "Forfeiture adjustment for Class Action" }
    ];
}
