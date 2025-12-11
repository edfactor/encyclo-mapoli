using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Common.Common;
using Moq; // For ProfitDetail

namespace Demoulas.ProfitSharing.UnitTests.Common.Mocks;

public static class MockEmbeddedSqlService
{
    internal static IEmbeddedSqlService Initialize()
    {
        var mock = new Mock<IEmbeddedSqlService>();
        mock.Setup(m => m.GetTotalBalanceAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>()))
            .Returns((IProfitSharingDbContext x, short y) => Constants.FakeParticipantTotals.Object);

        mock.Setup(m => m.TotalVestingBalanceAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>(), It.IsAny<short>(), It.IsAny<DateOnly>()))
            .Returns((IProfitSharingDbContext x, short y, short z, DateOnly w) => Constants.FakeParticipantTotalVestingBalances.Object);

        mock.Setup(m => m.TotalVestingBalanceAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>(), It.IsAny<short>(), It.IsAny<DateOnly>()))
            .Returns((IProfitSharingDbContext x, short y, short z, DateOnly w) => Constants.FakeParticipantTotalVestingBalances.Object);

        mock.Setup(m => m.GetTotalComputedEtvaAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>()))
            .Returns((IProfitSharingDbContext x, short y) => Constants.FakeEtvaTotals.Object);

        mock.Setup(m => m.GetProfitShareTotals(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>(), It.IsAny<DateOnly>(), It.IsAny<short>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .Returns((IProfitSharingDbContext x, short y, DateOnly z, short a, DateOnly b, CancellationToken c) => Constants.ProfitShareTotals.Object);

        // Mock GetYearsOfServiceAlt to return the correct LINQ result using fake ProfitDetails
        mock.Setup(m => m.GetYearsOfServiceAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>(), It.IsAny<DateOnly>()))
            .Returns((IProfitSharingDbContext ctx, short profitYear, DateOnly asOfDate) =>
            {
                // Use the fake data from Constants or ctx.ProfitDetails if available
                var profitDetails = (ctx?.ProfitDetails?.AsQueryable() ?? new List<ProfitDetail>().AsQueryable());

                var query =
                    from pdx in
                        (from pd in profitDetails
                         where pd.ProfitYear <= profitYear
                         group pd by new { pd.Ssn, pd.ProfitYear } into pdGrp
                         select new { pdGrp.Key.Ssn, pdGrp.Key.ProfitYear, YearsOfServiceCredit = pdGrp.Max(x => x.YearsOfServiceCredit) }
                        )
                    group pdx by pdx.Ssn into pdxGrp
                    select new ParticipantTotalYear { Ssn = pdxGrp.Key, Years = (byte)pdxGrp.Sum(x => x.YearsOfServiceCredit) };

                return query;
            });

        return mock.Object;
    }
}
