using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class CommentType : ModifiedBase, ILookupTable<byte>, IEquatable<CommentType>
{
    public required byte Id { get; set; }
    public required string Name { get; set; }

    /// <summary>
    /// Indicates whether this comment type is used in business logic and must not be changed.
    /// Once set to true, can only be unset via direct database update outside this application.
    /// This flag helps protect critical comment types from accidental name changes that would break business logic.
    /// </summary>
    public bool IsProtected { get; set; }

    public static class Constants
    {
        public static CommentType TransferOut => new() { Id = 1, Name = "Transfer Out", IsProtected = true };
        public static CommentType TransferIn => new() { Id = 2, Name = "Transfer In", IsProtected = true };
        public static CommentType QdroOut => new() { Id = 3, Name = "QDRO Out", IsProtected = true };
        public static CommentType QdroIn => new() { Id = 4, Name = "QDRO In", IsProtected = true };
        public static CommentType VOnly => new() { Id = 5, Name = "V-Only", IsProtected = true };
        public static CommentType Forfeit => new() { Id = 6, Name = "Forfeit", IsProtected = true };
        public static CommentType Unforfeit => new() { Id = 7, Name = "Un-Forfeit", IsProtected = true };
        public static CommentType ClassAction => new() { Id = 8, Name = "Class Action", IsProtected = true };
        public static CommentType Voided => new() { Id = 9, Name = "Voided", IsProtected = true };
        public static CommentType Hardship => new() { Id = 10, Name = "Hardship", IsProtected = true };
        public static CommentType Distribution => new() { Id = 11, Name = "Distribution", IsProtected = true };
        public static CommentType Payoff => new() { Id = 12, Name = "Payoff", IsProtected = true };
        public static CommentType Dirpay => new() { Id = 13, Name = "Dirpay" };
        public static CommentType Rollover => new() { Id = 14, Name = "Rollover", IsProtected = true };
        public static CommentType RothIra => new() { Id = 15, Name = "Roth IRA", IsProtected = true };
        public static CommentType Over64OneYearVested => new() { Id = 16, Name = "> 64 - 1 Year Vested" };
        public static CommentType Over64TwoYearsVested => new() { Id = 17, Name = "> 64 - 2 Year Vested" };
        public static CommentType Over64ThreeYearsVested => new() { Id = 18, Name = "> 64 - 3 Year Vested" };
        public static CommentType Military => new() { Id = 19, Name = "Military", IsProtected = true };
        public static CommentType Other => new() { Id = 20, Name = "Other" };
        public static CommentType Reversal => new() { Id = 21, Name = "Rev", IsProtected = true };
        public static CommentType UndoReversal => new() { Id = 22, Name = "Unrev", IsProtected = true };
        public static CommentType OneHundredPercentEarnings => new() { Id = 23, Name = "100% Earnings", IsProtected = true };
        // The variable name here purposely corresponds to the ZeroContrbutionReason.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested name
        public static CommentType SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested => new() { Id = 24, Name = ">64 & >5 100%", IsProtected = true };
        public static CommentType ForfeitClassAction => new() { Id = 25, Name = "Forfeit Class Action", IsProtected = true }; //https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/402817082/008-12+to+forfeit+Class+Action+-+Mockup
        public static CommentType ForfeitAdministrative => new() { Id = 26, Name = "Forfeit Administrative", IsProtected = true }; // MAIN-2170 Administrative forfeitures
        public static CommentType UnforfeitUnder21 => new() { Id = 27, Name = "Administrative - taking money from under 21" }; // PS-2152
        public static CommentType ForfeitClassActionAdjustment => new() { Id = 28, Name = "Forfeiture adjustment for Class Action" }; // PS-2152
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

    public bool Equals(CommentType? other)
    {
        if (other != null)
        {
            return other.Id == this.Id;
        }
        return false;
    }

    public static implicit operator byte(CommentType commentType)
    {
        return commentType.Id;
    }

    public static bool operator ==(CommentType? left, CommentType? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }
        if (ReferenceEquals(left, null))
        {
            return false;
        }
        if (ReferenceEquals(right, null))
        {
            return false;
        }
        return left.Equals(right);
    }

    public static bool operator !=(CommentType? left, CommentType? right)
    {
        return !(left == right);
    }

    // Override GetHashCode to be consistent with Equals
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
