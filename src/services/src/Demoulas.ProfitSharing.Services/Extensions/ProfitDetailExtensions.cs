using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.Extensions;

public static class ProfitDetailExtensions
{
    /// <summary>
    /// Gets the profit code IDs that represent payments/distributions (used to differentiate forfeitures from payments).
    /// These codes are used to distinguish actual forfeiture amounts from payment distribution amounts.
    /// This is the single source of truth for payment codes used in balance calculations.
    /// Referenced by EmbeddedSqlService.GetBalanceSubquery() and other aggregation queries.
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
}
