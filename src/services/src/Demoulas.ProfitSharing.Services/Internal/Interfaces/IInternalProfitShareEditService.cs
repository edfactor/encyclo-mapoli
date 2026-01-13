using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;

namespace Demoulas.ProfitSharing.Services.Internal.Interfaces;

public interface IInternalProfitShareEditService : IProfitShareEditService
{
    // internal method used by ProfitMaster service to access profit detail record infomration.  Includes ssn, not masked.
    internal Task<(List<ProfitShareEditMemberRecord>, decimal BeginningBalanceTotal, decimal ContributionGrandTotal, decimal IncomingForfeitureGrandTotal, decimal EarningsGrandTotal)> ProfitShareEditRecords(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken);

}

