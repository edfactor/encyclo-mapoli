using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class MilitaryAndRehireService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    
    public MilitaryAndRehireService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    public async Task GetAllInactiveMilitaryMembers(CancellationToken cancellationToken)
    {

        await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var inactiveMilitaryMembers = await context.Demographics.Where(d => d.TerminationCodeId == TerminationCode.Constants.Military
                                                                                && d.EmploymentStatusId == EmploymentStatus.Constants.Inactive)
                .ToListAsync(cancellationToken: cancellationToken);

            return inactiveMilitaryMembers;
        });

    }
}
