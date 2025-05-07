namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IYearEndService
{
    Task RunFinalYearEndUpdates(short profitYear, CancellationToken ct);
    Task UpdateEnrollmentId(short profitYear, CancellationToken ct);
}
