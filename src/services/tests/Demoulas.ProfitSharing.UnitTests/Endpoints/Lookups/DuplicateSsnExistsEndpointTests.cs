using System.Net.Http.Json;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Lookups;

public class DuplicateSsnExistsEndpointTests
{
    [Fact]
    public async Task Get_ReturnsTrue_WhenServiceReportsDuplicatesExist()
    {
        // Arrange
        var mockDataContextFactory = MockDataContextFactory.InitializeForTesting();
        var mockReportService = new Mock<IPayrollDuplicateSsnReportService>();
        mockReportService.Setup(s => s.DuplicateSsnExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        WebApplicationFactory<Program> webApplicationFactory = new WebApplicationFactory<Program>();

        var builder = webApplicationFactory.WithWebHostBuilder(hostBuilder =>
        {
            hostBuilder.UseEnvironment("Testing");
            hostBuilder.ConfigureServices(services =>
            {
                services.AddTransient((_) => mockDataContextFactory);
                services.AddTransient<ICalendarService, CalendarService>();
                services.AddTransient<IPayrollDuplicateSsnReportService>(_ => mockReportService.Object);
            });
        });

        var client = builder.CreateClient();

        // Act
        var result = await client.GetFromJsonAsync<bool>("/duplicate-ssns/exists");

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task Get_ReturnsFalse_WhenServiceReportsNoDuplicates()
    {
        // Arrange
        var mockDataContextFactory = MockDataContextFactory.InitializeForTesting();
        var mockReportService = new Mock<IPayrollDuplicateSsnReportService>();
        mockReportService.Setup(s => s.DuplicateSsnExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

        WebApplicationFactory<Program> webApplicationFactory = new WebApplicationFactory<Program>();

        var builder = webApplicationFactory.WithWebHostBuilder(hostBuilder =>
        {
            hostBuilder.UseEnvironment("Testing");
            hostBuilder.ConfigureServices(services =>
            {
                services.AddTransient((_) => mockDataContextFactory);
                services.AddTransient<ICalendarService, CalendarService>();
                services.AddTransient<IPayrollDuplicateSsnReportService>(_ => mockReportService.Object);
            });
        });

        var client = builder.CreateClient();

        // Act
        var result = await client.GetFromJsonAsync<bool>("/duplicate-ssns/exists");

        // Assert
        result.ShouldBeFalse();
    }
}
