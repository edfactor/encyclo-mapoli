using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;

namespace Demoulas.ProfitSharing.Common.Interfaces.Administration;

public interface IBankService
{
    Task<Result<IReadOnlyList<BankDto>>> GetAllAsync(bool includeDisabled = false, CancellationToken cancellationToken = default);
    Task<Result<BankDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<BankDto>> CreateAsync(CreateBankRequest request, CancellationToken cancellationToken = default);
    Task<Result<BankDto>> UpdateAsync(UpdateBankRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DisableAsync(int id, CancellationToken cancellationToken = default);
}
