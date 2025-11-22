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
}
