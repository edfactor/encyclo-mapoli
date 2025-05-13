
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Moq;
using Demoulas.ProfitSharing.UnitTests.Common.Common;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;

namespace Demoulas.ProfitSharing.UnitTests.Common.Mocks;
public static class MockEmbeddedSqlService
{
    internal static IEmbeddedSqlService Initialize()
    {
        var mock = new Mock<IEmbeddedSqlService>();
        mock.Setup(m => m.GetTotalBalanceAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>()))
            .Returns((IProfitSharingDbContext x, short y) => Constants.FakeParticipantTotals.AsAsyncQueryable());

        mock.Setup(m => m.TotalVestingBalanceAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>(), It.IsAny<short>(), It.IsAny<DateOnly>()))
            .Returns((IProfitSharingDbContext x, short y, short z, DateOnly w) => Constants.FakeParticipantTotalVestingBalances.AsAsyncQueryable());

        mock.Setup(m => m.TotalVestingBalanceAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>(), It.IsAny<short>(), It.IsAny<DateOnly>()))
            .Returns((IProfitSharingDbContext x, short y, short z, DateOnly w) => Constants.FakeParticipantTotalVestingBalances.AsAsyncQueryable());

        mock.Setup(m => m.GetTotalComputedEtvaAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>()))
            .Returns((IProfitSharingDbContext x, short y) => Constants.FakeEtvaTotals.AsAsyncQueryable());


        mock.Setup(m => m.GetProfitShareTotals(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>(), It.IsAny<DateOnly>(), It.IsAny<short>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .Returns((IProfitSharingDbContext x, short y, DateOnly z, short a, DateOnly b, CancellationToken c) => Constants.ProfitShareTotals.AsAsyncQueryable()); 

        return mock.Object;

    }
}
