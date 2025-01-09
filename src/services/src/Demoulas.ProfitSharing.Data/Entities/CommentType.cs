using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class CommentType : ILookupTable<byte>
{
    public required byte Id { get; set; }
    public required string Name { get; set; }

    public static class Constants
    {
        public static CommentType TransferOut => new CommentType() { Id = 1, Name = "Transfer Out" };
        public static CommentType TransferIn => new CommentType() { Id = 2, Name = "Transfer In" };
        public static CommentType QdroOut => new CommentType() { Id = 3, Name = "QDRO Out" };
        public static CommentType QdroIn => new CommentType() { Id = 4, Name = "QDRO In" };
        public static CommentType VOnly => new CommentType() { Id = 5, Name = "V-Only" };
        public static CommentType Forfeit => new CommentType() { Id = 6, Name = "Forfeit" };
        public static CommentType UnForfeit => new CommentType() { Id = 7, Name = "Un-Forfeit" };
        public static CommentType ClassAction => new CommentType() { Id = 8, Name = "Class Action" };
        public static CommentType Voided => new CommentType() { Id = 9, Name = "Voided" };
        public static CommentType Hardship => new CommentType() { Id = 10, Name = "Hardship" };
        public static CommentType Distribution => new CommentType() { Id = 11, Name = "Distribution" };
        public static CommentType Payoff  => new CommentType() { Id = 12, Name = "Payoff" };
        public static CommentType Dirpay => new CommentType() { Id = 13, Name = "Dirpay" };
        public static CommentType Rollover => new CommentType() { Id = 14, Name = "Rollover" };
        public static CommentType RothIra => new CommentType() { Id = 15, Name = "Roth IRA" };
        public static CommentType Over64OneYearVested => new CommentType() { Id = 16, Name = "> 64 - 1 Year Vested" };
        public static CommentType Over64TwoYearsVested => new CommentType() { Id = 17, Name = "> 64 - 2 Year Vested" };
        public static CommentType Over64ThreeYearsVested => new CommentType() { Id = 18, Name = "> 64 - 3 Year Vested" };
        public static CommentType Military => new CommentType() { Id = 19, Name = "Military" };
        public static CommentType Other => new CommentType() { Id = 20, Name = "Other" };
        public static CommentType Rev => new CommentType() { Id = 21, Name = "Rev" };
        public static CommentType Unrev => new CommentType() { Id = 22, Name = "Unrev" };
        public static CommentType OneHundredPercentEarnings => new CommentType() { Id = 23, Name = "100% Earnings" };
        // Uses name from ZeroContrbutionReason SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested
        public static CommentType SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested => new CommentType() { Id = 24, Name = ">64 & >5 Zero Records" }; 
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
