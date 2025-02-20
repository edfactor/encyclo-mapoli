using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests
{
    public class FakeSsnServiceTests
    {
        [Fact]
        public async Task GenerateFakeSsnAsync_ReturnsValidFakeSsnAndSavesIt()
        {
            // Arrange
            var fakeSsnList = new List<FakeSsn>();
            var fakeSsnDbSetMock = new Mock<DbSet<FakeSsn>>();
            var fakeSsnQueryable = fakeSsnList.AsQueryable();
            fakeSsnDbSetMock.As<IQueryable<FakeSsn>>().Setup(m => m.Provider).Returns(fakeSsnQueryable.Provider);
            fakeSsnDbSetMock.As<IQueryable<FakeSsn>>().Setup(m => m.Expression).Returns(fakeSsnQueryable.Expression);
            fakeSsnDbSetMock.As<IQueryable<FakeSsn>>().Setup(m => m.ElementType).Returns(fakeSsnQueryable.ElementType);
            fakeSsnDbSetMock.As<IQueryable<FakeSsn>>().Setup(m => m.GetEnumerator()).Returns(fakeSsnQueryable.GetEnumerator());
            fakeSsnDbSetMock.Setup(d => d.Add(It.IsAny<FakeSsn>()))
                .Callback<FakeSsn>(ssn => fakeSsnList.Add(ssn));

            var fakeContextMock = new Mock<ProfitSharingDbContext>();
            fakeContextMock.Setup(c => c.FakeSsns).Returns(fakeSsnDbSetMock.Object);
            fakeContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dataContextFactoryMock = new Mock<IProfitSharingDataContextFactory>();
            dataContextFactoryMock
                .Setup(f => f.UseWritableContext(It.IsAny<Func<ProfitSharingDbContext, Task<int>>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<ProfitSharingDbContext, Task<int>> func, CancellationToken token) =>
                    func(fakeContextMock.Object));

            var service = new FakeSsnService(dataContextFactoryMock.Object);

            // Act
            int resultSsn = await service.GenerateFakeSsnAsync(CancellationToken.None);

            // Assert: Verify SSN is in fake format (area 666) and that SaveChangesAsync was invoked.
            Assert.StartsWith("666", resultSsn.ToString());
            fakeContextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Contains(fakeSsnList, f => f.Ssn == resultSsn);
        }

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

            // Assert: Verify that the history entry was created and SaveChangesAsync was called once.
            fakeContextMock.Verify(c => c.Entry(It.Is<TestSsnChangeHistory>(h => h.OldSsn == oldSsn && h.NewSsn == newSsn)), Times.Once);
            fakeContextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // Dummy implementation of a history record for testing purposes.
        public class TestSsnChangeHistory : SsnChangeHistory
        {
            public TestSsnChangeHistory()
            {
                ChangeDateUtc = DateTimeOffset.UtcNow;
            }
        }
    }
}
