using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.Extensions;

public static class ProfitDetailExtensions
{
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

    public static (decimal contribution, decimal earnings, decimal forfeiture, decimal payment, decimal distribution, decimal beneficiaryAllocation, decimal rowBalance) CalculateAmounts(this ProfitDetail detail)
    {
        var contribution = CalculateContribution(detail);
        var earnings = CalculateEarnings(detail);
        var forfeiture = CalculateForfeiture(detail);
        var payment = CalculatePayment(detail);
        var distribution = CalculateDistribution(detail);
        var beneficiaryAllocation = CalculateBeneficiaryAllocation(detail);
        var rowBalance = CalculateRowBalance(detail);

        return (contribution, earnings, forfeiture, payment, distribution, beneficiaryAllocation, rowBalance);
    }

    internal static decimal CalculateContribution(this ProfitDetail detail)
    {
        return detail.Contribution;
    }

    internal static decimal CalculateEarnings(this ProfitDetail detail)
    {
        return detail.Earnings;
    }

    internal static decimal CalculateForfeiture(this ProfitDetail detail)
    {
        if (GetProfitCodesForBalanceCalc().Contains(detail.ProfitCodeId))
        {
            return 0;
        }
        return detail.Forfeiture;
    }

    internal static decimal CalculatePayment(this ProfitDetail detail)
    {
        if (GetProfitCodesForBalanceCalc().Contains(detail.ProfitCodeId))
        {
            return detail.Forfeiture;
        }

        return 0;
    }

    internal static decimal CalculateDistribution(this ProfitDetail detail)
    {
        return (detail.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id ||
                detail.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments.Id ||
                detail.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id)
            ? -detail.Forfeiture
            : 0;
    }

    internal static decimal CalculateBeneficiaryAllocation(this ProfitDetail detail)
    {
        return detail.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary.Id
            ? -detail.Forfeiture
            : detail.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id
                ? detail.Contribution
                : 0;
    }

    internal static decimal CalculateRowBalance(this ProfitDetail detail)
    {
        byte[] sumAllFieldProfitCodeTypes = GetProfitCodesForBalanceCalc();
        return sumAllFieldProfitCodeTypes.Contains(detail.ProfitCodeId)
            ? (-detail.Forfeiture + detail.Contribution + detail.Earnings)
            : (detail.Contribution + detail.Earnings + detail.Forfeiture);
    }
}
