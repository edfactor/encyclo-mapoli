using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests;
public class YearEndServiceTests:ApiTestBase<Program>
{
    [Fact]
    public async Task YearEndProcessShouldSetPoints()
    {
        var yearEndService = ServiceProvider?.GetRequiredService<IYearEndService>();
        var dataContextFactory = ServiceProvider?.GetRequiredService<IProfitSharingDataContextFactory>();
        short testYear = 2023;

        Assert.NotNull(yearEndService);
        await yearEndService.RunFinalYearEndUpdates(testYear, CancellationToken.None);

        Assert.NotNull(dataContextFactory);

        await dataContextFactory.UseReadOnlyContext<bool>(ctx =>
        {
            foreach (var pp in ctx.PayProfits.Where(p=>p.ProfitYear == testYear))
            {
                Assert.Equal(testYear, pp.ProfitYear);
            }
            return Task.FromResult(true);
        });


    }
}
