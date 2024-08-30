using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;
public sealed class MilitaryAndRehireService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger<YearEndService> _logger;

    public MilitaryAndRehireService(IProfitSharingDataContextFactory dataContextFactory,
        ILoggerFactory factory)
    {
        _dataContextFactory = dataContextFactory;
        _logger = factory.CreateLogger<YearEndService>();
    }

    public async Task GetAllInactiveMilitaryMembers(CancellationToken cancellationToken)
    {
        try
        {
            await _dataContextFactory.UseReadOnlyContext(async context =>
            {
                var inactiveMilitaryMembers = await context.Demographics.Where(d => d.TerminationCodeId == TerminationCode.Constants.Military 
                                                                                    && d.EmploymentStatusId == EmploymentStatus.Constants.Inactive).ToListAsync(cancellationToken: cancellationToken);

                return inactiveMilitaryMembers;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving inactive military members.");
            throw;
        }
    }
}
