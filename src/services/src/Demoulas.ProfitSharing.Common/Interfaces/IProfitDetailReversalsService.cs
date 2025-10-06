namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IProfitDetailReversalsService
{
    Task ReverseProfitDetailsAsync(int[] profitDetailIds, CancellationToken cancellationToken);
}
