
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Moq;
using Demoulas.ProfitSharing.UnitTests.Common.Common;
using Demoulas.ProfitSharing.UnitTests.Common.Helpers;

namespace Demoulas.ProfitSharing.UnitTests.Common.Mocks;
public static class MockEmbeddedSqlService
{
    private static Mock<IEmbeddedSqlService>? _embeddedSqlService = null;
    internal static IEmbeddedSqlService Initialize()
    {
        var mock = new Mock<IEmbeddedSqlService>();
        mock.Setup(m => m.GetTotalBalanceAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>()))
            .Returns((IProfitSharingDbContext x, short y) => Constants.FakeParticipantTotals.AsAsyncQueryable());

        mock.Setup(m => m.TotalVestingBalanceAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>(), It.IsAny<short>(), It.IsAny<DateOnly>()))
            .Returns((IProfitSharingDbContext x, short y, short z, DateOnly w) => Constants.FakeParticipantTotalVestingBalances.AsAsyncQueryable());

        mock.Setup(m => m.TotalVestingBalanceAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>(), It.IsAny<short>(), It.IsAny<DateOnly>()))
            .Returns((IProfitSharingDbContext x, short y, short z, DateOnly w) => Constants.FakeParticipantTotalVestingBalances.AsAsyncQueryable());

        _embeddedSqlService = mock;
        return mock.Object;
    }

    public static void RemockTotalVestingBalance()
    {
        _embeddedSqlService?.Setup(m => m.TotalVestingBalanceAlt(It.IsAny<IProfitSharingDbContext>(), It.IsAny<short>(), It.IsAny<short>(), It.IsAny<DateOnly>()))
            .Returns((IProfitSharingDbContext x, short y, short z, DateOnly w) => Constants.FakeParticipantTotalVestingBalances.AsAsyncQueryable());
    }
}
