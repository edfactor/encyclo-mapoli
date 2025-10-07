using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class CommentType : ILookupTable<byte>
{
    public required byte Id { get; set; }
    public required string Name { get; set; }

    public static class Constants
    {
        public static CommentType TransferOut => new() { Id = 1, Name = "Transfer Out" };
        public static CommentType TransferIn => new() { Id = 2, Name = "Transfer In" };
        public static CommentType QdroOut => new() { Id = 3, Name = "QDRO Out" };
        public static CommentType QdroIn => new() { Id = 4, Name = "QDRO In" };
        public static CommentType VOnly => new() { Id = 5, Name = "V-Only" };
        public static CommentType Forfeit => new() { Id = 6, Name = "Forfeit" };
        public static CommentType Unforfeit => new() { Id = 7, Name = "Un-Forfeit" };
        public static CommentType ClassAction => new() { Id = 8, Name = "Class Action" };
        public static CommentType Voided => new() { Id = 9, Name = "Voided" };
        public static CommentType Hardship => new() { Id = 10, Name = "Hardship" };
        public static CommentType Distribution => new() { Id = 11, Name = "Distribution" };
        public static CommentType Payoff => new() { Id = 12, Name = "Payoff" };
        public static CommentType Dirpay => new() { Id = 13, Name = "Dirpay" };
        public static CommentType Rollover => new() { Id = 14, Name = "Rollover" };
        public static CommentType RothIra => new() { Id = 15, Name = "Roth IRA" };
        public static CommentType Over64OneYearVested => new() { Id = 16, Name = "> 64 - 1 Year Vested" };
        public static CommentType Over64TwoYearsVested => new() { Id = 17, Name = "> 64 - 2 Year Vested" };
        public static CommentType Over64ThreeYearsVested => new() { Id = 18, Name = "> 64 - 3 Year Vested" };
        public static CommentType Military => new() { Id = 19, Name = "Military" };
        public static CommentType Other => new() { Id = 20, Name = "Other" };
        public static CommentType Reversal => new() { Id = 21, Name = "Rev" };
        public static CommentType UndoReversal => new() { Id = 22, Name = "Unrev" };
        public static CommentType OneHundredPercentEarnings => new() { Id = 23, Name = "100% Earnings" };
        // The variable name here purposely corresponds to the ZeroContrbutionReason.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested name
        public static CommentType SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested => new() { Id = 24, Name = ">64 & >5 100%" };
        public static CommentType ForfeitClassAction => new() { Id = 25, Name = "Forfeit Class Action" }; //https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/402817082/008-12+to+forfeit+Class+Action+-+Mockup
    }

    // Override Equals method for correct comparison between materialized values and constants
    public override bool Equals(object? obj)
    {
        if (obj is CommentType otherCommentType)
        {
            return Id == otherCommentType.Id;
        }
        return false;
    }

    // Override GetHashCode to be consistent with Equals
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static implicit operator byte(CommentType commentType)
    {
        return commentType.Id;
    }
}
