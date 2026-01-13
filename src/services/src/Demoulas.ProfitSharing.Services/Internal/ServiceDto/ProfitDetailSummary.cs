namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

/// <summary>
/// The summary of PROFIT_DETAIL rows for a given individual identified by SSN.
/// </summary>
internal sealed record ProfitDetailSummary
(
    decimal Distribution,
    decimal Forfeiture,
    decimal BeneficiaryAllocation
);
