using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Common.Extensions;

public static class ProfitDetailExtensions
{
    public static (
        decimal contribution,
        decimal earnings,
        decimal forfeiture,
        decimal payment,
        decimal distribution,
        decimal beneficiaryAllocation,
        decimal currentBalance) CalculateAmounts(this ProfitDetail detail)
    {
        var contribution = detail.Contribution;
        var earnings = detail.Earnings;
        
        // Calculate forfeiture amount
        var forfeiture = detail.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
            ? detail.Forfeiture
            : (detail.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id ? -detail.Forfeiture : 0);

        // Calculate payment amount
        var payment = detail.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id 
            ? detail.Forfeiture 
            : 0;

        // Calculate distribution amount
        var distribution = (detail.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id ||
                          detail.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments.Id ||
                          detail.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id)
            ? -detail.Forfeiture
            : 0;

        // Calculate beneficiary allocation
        var beneficiaryAllocation = detail.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary.Id 
            ? -detail.Forfeiture
            : detail.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id 
                ? detail.Contribution 
                : 0;

        // Calculate current balance
        var currentBalance = detail.Contribution + detail.Earnings +
                           (detail.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id 
                               ? detail.Forfeiture 
                               : 0) -
                           (detail.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id 
                               ? detail.Forfeiture 
                               : 0);

        return (contribution, earnings, forfeiture, payment, distribution, beneficiaryAllocation, currentBalance);
    }
}
