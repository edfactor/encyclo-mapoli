namespace Demoulas.ProfitSharing.Services.Internal.Interfaces;

public interface IPayrollDuplicateSsnReportServiceInternal
{
    Task<bool> DuplicateSsnExistsAsync(CancellationToken ct);
}
