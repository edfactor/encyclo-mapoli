using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public class ProfitCode : ILookupTable<byte>
{
    public static class Constants
    {
        public static ProfitCode IncomingContributions => new ProfitCode
        {
            Id = 0, Name = "Incoming contributions, forfeitures, earnings", Frequency = "Year-end only"
        };

        public static ProfitCode OutgoingPaymentsPartialWithdrawal => new ProfitCode
        {
            Id = 1, Name = "Outgoing payments (not rollovers or direct payments) - Partial withdrawal", Frequency = "Multiple Times"
        };

        public static ProfitCode OutgoingForfeitures => new ProfitCode { Id = 2, Name = "Outgoing forfeitures", Frequency = "Multiple Times" };

        public static ProfitCode OutgoingDirectPayments => new ProfitCode
        {
            Id = 3, Name = "Outgoing direct payments / rollover payments", Frequency = "Multiple Times"
        };

        public static ProfitCode OutgoingXferBeneficiary => new ProfitCode
        {
            Id = 5, Name = "Outgoing XFER beneficiary / QDRO allocation (beneficiary payment)", Frequency = "Once"
        };

        public static ProfitCode IncomingQdroBeneficiary => new ProfitCode
        {
            Id = 6, Name = "Incoming QDRO beneficiary allocation (beneficiary receipt)", Frequency = "Once"
        };

        public static ProfitCode Incoming100PercentVestedEarnings => new ProfitCode
        {
            Id = 8,
            Name = "Incoming \"100% vested\" earnings",
            Frequency = "Usually year-end, unless there is special processing – i.e. Class Action settlement. Earnings are 100% vested."
        };

        public static ProfitCode Outgoing100PercentVestedPayment => new ProfitCode
        {
            Id = 9, Name = "Outgoing payment from 100% vesting amount (payment of ETVA funds)", Frequency = "Multiple Times"
        };
    }

    public required byte Id { get; set; }
    public required string Name { get; set; }
    public required string Frequency { get; init; }

    
    // Override Equals method for correct comparison
    public override bool Equals(object? obj)
    {
        if (obj is ProfitCode otherProfitCode)
        {
            return Id == otherProfitCode.Id;
        }
        return false;
    }

    // Override GetHashCode to be consistent with Equals
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
#pragma warning disable S3875
    public static bool operator ==(byte? left, ProfitCode? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right?.Id);
    }

    public static bool operator !=(byte? left, ProfitCode right)
    {
        return !(left == right);
    }
#pragma warning restore S3875
}
