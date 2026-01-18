using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;

namespace Demoulas.ProfitSharing.Common.Interfaces.Administration;

public interface IBankAccountService
{
    Task<Result<IReadOnlyList<BankAccountDto>>> GetByBankIdAsync(int bankId, bool includeDisabled = false, CancellationToken cancellationToken = default);
    Task<Result<BankAccountDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<BankAccountDto>> GetPrimaryAccountAsync(int bankId, CancellationToken cancellationToken = default);
    Task<Result<BankAccountDto>> GetByRoutingNumberAsync(string routingNumber, CancellationToken cancellationToken = default);
    Task<Result<BankAccountDto>> CreateAsync(CreateBankAccountRequest request, IProfitSharingAuditService profitSharingAuditService, IAppUser appUser, CancellationToken cancellationToken = default);
    Task<Result<BankAccountDto>> UpdateAsync(UpdateBankAccountRequest request, IProfitSharingAuditService profitSharingAuditService, IAppUser appUser, CancellationToken cancellationToken = default);
    Task<Result<bool>> SetPrimaryAsync(int id, IProfitSharingAuditService profitSharingAuditService, IAppUser appUser, CancellationToken cancellationToken = default);
    Task<Result<bool>> DisableAsync(int id, IProfitSharingAuditService profitSharingAuditService, IAppUser appUser, CancellationToken cancellationToken = default);
}
