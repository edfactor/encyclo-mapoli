namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IYearEndService
{
    Task RunFinalYearEndUpdates(short profitYear, bool rebuild, CancellationToken ct);
    Task UpdateEnrollmentId(short profitYear, CancellationToken ct);
    Task<short> GetCompletedYearEnd(CancellationToken ct);
}
