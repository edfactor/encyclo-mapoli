using Demoulas.ProfitSharing.IntegrationTests.Fixtures;

namespace Demoulas.ProfitSharing.IntegrationTests
{
    public class OracleDbContextTests : IClassFixture<OracleContainerFixture>
    {
        private readonly OracleContainerFixture _fixture;
        public OracleDbContextTests(OracleContainerFixture fixture)
        {
            _fixture = fixture;
        }


        [Fact]
        public async Task TestInsertAndRetrieveEntity()
        {
            _fixture.OracleContainer.GetConnectionString();
            await Task.CompletedTask;
        }
    }
}