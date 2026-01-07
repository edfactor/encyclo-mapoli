using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.Extensions;

public static class ProfitDetailExtensions
{
    /// <summary>
    /// Gets the profit code IDs that represent payments/distributions (used to differentiate forfeitures from payments).
    /// These codes are used to distinguish actual forfeiture amounts from payment distribution amounts.
    /// </summary>
    public static byte[] GetProfitCodesForBalanceCalc()
    {
        return
        [
            /*1*/ ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
            /*2*/ ProfitCode.Constants.OutgoingForfeitures.Id,
            /*3*/ ProfitCode.Constants.OutgoingDirectPayments.Id,
            /*5*/ ProfitCode.Constants.OutgoingXferBeneficiary.Id,
            /*9*/ ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
        ];
    }

    /// <summary>
    /// Aggregates contributions from profit details.
    /// Contributions are identified by profit code 0 (IncomingContributions).
    /// </summary>
    public static decimal AggregateContributions(IEnumerable<ProfitDetail> profitDetails)
    {
        return profitDetails
            .Where(pd => pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id)
            .Sum(pd => pd.Contribution);
    }

    /// <summary>
    /// Aggregates earnings from profit details.
    /// Earnings are identified by profit code 0 (IncomingContributions).
    /// </summary>
    public static decimal AggregateEarnings(IEnumerable<ProfitDetail> profitDetails)
    {
        return profitDetails
            .Where(pd => pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
                       || pd.ProfitCodeId == ProfitCode.Constants.Incoming100PercentVestedEarnings.Id)
            .Sum(pd => pd.Earnings);
    }

    /// <summary>
    /// Aggregates forfeitures from profit details.
    /// Forfeitures are identified as any forfeiture amount that is NOT a payment code.
    /// This distinguishes actual forfeitures from payment distribution amounts.
    /// </summary>
    public static decimal AggregateForfeitures(IEnumerable<ProfitDetail> profitDetails)
    {
        var paymentCodes = GetProfitCodesForBalanceCalc();
        return profitDetails
            .Where(pd => !paymentCodes.Contains(pd.ProfitCodeId))
            .Sum(pd => pd.Forfeiture);
    }

    /// <summary>
    /// Aggregates all profit detail values (contributions, earnings, forfeitures) for a collection of profit details.
    /// This is the primary aggregation method used across all services for consistency.
    /// </summary>
    public static (decimal Contributions, decimal Earnings, decimal Forfeitures) AggregateAllProfitValues(IEnumerable<ProfitDetail> profitDetails)
    {
        return (
            Contributions: AggregateContributions(profitDetails),
            Earnings: AggregateEarnings(profitDetails),
            Forfeitures: AggregateForfeitures(profitDetails)
        );
    }
}
