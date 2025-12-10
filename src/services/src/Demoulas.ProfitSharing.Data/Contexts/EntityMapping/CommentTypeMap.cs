using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class CommentTypeMap : IEntityTypeConfiguration<CommentType>
{
    public void Configure(EntityTypeBuilder<CommentType> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("COMMENT_TYPE");

        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever().HasColumnName("ID");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128).HasColumnName("NAME");

        builder.HasData(GetPredefinedCommentTypes());
    }

    private static List<CommentType> GetPredefinedCommentTypes()
    {
        return [
            CommentType.Constants.TransferOut,
            CommentType.Constants.TransferIn,
            CommentType.Constants.QdroOut,
            CommentType.Constants.QdroIn,
            CommentType.Constants.VOnly,
            CommentType.Constants.Forfeit,
            CommentType.Constants.Unforfeit,
            CommentType.Constants.ClassAction,
            CommentType.Constants.Voided,
            CommentType.Constants.Hardship,
            CommentType.Constants.Distribution,
            CommentType.Constants.Payoff,
            CommentType.Constants.Dirpay,
            CommentType.Constants.Rollover,
            CommentType.Constants.RothIra,
            CommentType.Constants.Over64OneYearVested,
            CommentType.Constants.Over64TwoYearsVested,
            CommentType.Constants.Over64ThreeYearsVested,
            CommentType.Constants.Military,
            CommentType.Constants.Other,
            CommentType.Constants.Reversal,
            CommentType.Constants.UndoReversal,
            CommentType.Constants.OneHundredPercentEarnings,
            CommentType.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
            CommentType.Constants.ForfeitClassAction,
            CommentType.Constants.ForfeitAdministrative
        ];
    }
}
