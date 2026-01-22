using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Services.SystemInfo;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests;

public class FakeSsnServiceTests
{
    [Fact]
    public async Task TrackSsnChangeAsync_AddsHistoryAndSavesChanges()
    {
        // Arrange
        var fakeContextMock = new Mock<ProfitSharingDbContext>();
        fakeContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        fakeContextMock.Setup(c => c.Entry(It.IsAny<SsnChangeHistory>()))
            .Verifiable();

        var dataContextFactoryMock = new Mock<IProfitSharingDataContextFactory>();
        dataContextFactoryMock
            .Setup(f => f.UseWritableContext(It.IsAny<Func<ProfitSharingDbContext, Task>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<ProfitSharingDbContext, Task> func, CancellationToken token) =>
                func(fakeContextMock.Object));

        var service = new FakeSsnService(dataContextFactoryMock.Object);
        int oldSsn = 666010001;
        int newSsn = 666020002;

        // Act
        await service.TrackSsnChangeAsync<TestSsnChangeHistory>(oldSsn, newSsn, CancellationToken.None);

        // Assert
        fakeContextMock.Verify(c => c.Entry(It.Is<TestSsnChangeHistory>(h => h.OldSsn == oldSsn && h.NewSsn == newSsn)), Times.Once);
        fakeContextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    public class TestSsnChangeHistory : SsnChangeHistory
    {
        public TestSsnChangeHistory()
        {
            ModifiedAtUtc = DateTimeOffset.UtcNow;
        }
    }
}
