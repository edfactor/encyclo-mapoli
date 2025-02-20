using System.Linq.Expressions;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
            var mockSet = new Mock<DbSet<FakeSsn>>();
            var queryable = fakeSsnList.AsQueryable();

            // Set up sync operations
            mockSet.As<IQueryable<FakeSsn>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<FakeSsn>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<FakeSsn>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<FakeSsn>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            // Set up async operations
            // Set up async operations
            var asyncProvider = new Mock<IAsyncQueryProvider>();
            asyncProvider
                .Setup(m => m.ExecuteAsync<bool>(It.IsAny<Expression>(), It.IsAny<CancellationToken>()))
                .Returns(false);  // Simulate no existing SSN found

            mockSet.As<IQueryable<FakeSsn>>()
                .Setup(m => m.Provider)
                .Returns(asyncProvider.Object);


            mockSet.Setup(d => d.Add(It.IsAny<FakeSsn>()))
                .Callback<FakeSsn>(ssn => fakeSsnList.Add(ssn));

            var contextMock = new Mock<ProfitSharingDbContext>();
            contextMock.Setup(c => c.FakeSsns).Returns(mockSet.Object);
            contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dataContextFactoryMock = new Mock<IProfitSharingDataContextFactory>();
            dataContextFactoryMock
                .Setup(f => f.UseWritableContext(It.IsAny<Func<ProfitSharingDbContext, Task<int>>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<ProfitSharingDbContext, Task<int>> func, CancellationToken token) =>
                    func(contextMock.Object));

            var service = new FakeSsnService(dataContextFactoryMock.Object);

            // Act
            int resultSsn = await service.GenerateFakeSsnAsync(CancellationToken.None);

            // Assert
            Assert.StartsWith("666", resultSsn.ToString());
            contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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

            // Assert
            fakeContextMock.Verify(c => c.Entry(It.Is<TestSsnChangeHistory>(h => h.OldSsn == oldSsn && h.NewSsn == newSsn)), Times.Once);
            fakeContextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        public class TestSsnChangeHistory : SsnChangeHistory
        {
            public TestSsnChangeHistory()
            {
                ChangeDateUtc = DateTimeOffset.UtcNow;
            }
        }
    }
}
