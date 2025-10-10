using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Lookups;

public class CalendarRecordEndpointTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "CalendarRecord - Should return success for valid profit year")]
    [Description("PS-#### : Returns calendar dates for valid profit year")]
    public async Task Get_ReturnsSuccess_ForValidProfitYear()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new YearRequest { ProfitYear = 2024 };

        // Act
        var response = await ApiClient.GETAsync<CalendarRecordEndpoint, YearRequest, CalendarResponseDto>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "CalendarRecord - Should return valid date range")]
    [Description("PS-#### : Fiscal begin date should be before fiscal end date")]
    public async Task Get_ReturnsValidDateRange_ForProfitYear()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new YearRequest { ProfitYear = 2024 };

        // Act
        var response = await ApiClient.GETAsync<CalendarRecordEndpoint, YearRequest, CalendarResponseDto>(request);

        // Assert
        response.Result!.FiscalBeginDate.ShouldBeLessThan(response.Result.FiscalEndDate);
    }

    [Fact(DisplayName = "CalendarRecord - Should work with FINANCEMANAGER role")]
    [Description("PS-#### : Allows access with finance manager role")]
    public async Task Get_ReturnsSuccess_WithFinanceManagerRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new YearRequest { ProfitYear = 2024 };

        // Act
        var response = await ApiClient.GETAsync<CalendarRecordEndpoint, YearRequest, CalendarResponseDto>(request);

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "CalendarRecord - Should work with AUDITOR role")]
    [Description("PS-#### : Allows access with auditor role")]
    public async Task Get_ReturnsSuccess_WithAuditorRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.AUDITOR);
        var request = new YearRequest { ProfitYear = 2024 };

        // Act
        var response = await ApiClient.GETAsync<CalendarRecordEndpoint, YearRequest, CalendarResponseDto>(request);

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "CalendarRecord - Should return dates for different years")]
    [Description("PS-#### : Returns different dates for different profit years")]
    public async Task Get_ReturnsDifferentDates_ForDifferentYears()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request2024 = new YearRequest { ProfitYear = 2024 };
        var request2025 = new YearRequest { ProfitYear = 2025 };

        // Act
        var response2024 = await ApiClient.GETAsync<CalendarRecordEndpoint, YearRequest, CalendarResponseDto>(request2024);
        var response2025 = await ApiClient.GETAsync<CalendarRecordEndpoint, YearRequest, CalendarResponseDto>(request2025);

        // Assert
        response2024.Result!.FiscalBeginDate.ShouldNotBe(response2025.Result!.FiscalBeginDate);
        response2024.Result.FiscalEndDate.ShouldNotBe(response2025.Result.FiscalEndDate);
    }

    [Fact(DisplayName = "CalendarRecord - Should return non-default dates")]
    [Description("PS-#### : Fiscal dates should not be default values")]
    public async Task Get_ReturnsNonDefaultDates_ForValidYear()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new YearRequest { ProfitYear = 2024 };

        // Act
        var response = await ApiClient.GETAsync<CalendarRecordEndpoint, YearRequest, CalendarResponseDto>(request);

        // Assert
        response.Result!.FiscalBeginDate.ShouldNotBe(default(DateOnly));
        response.Result.FiscalEndDate.ShouldNotBe(default(DateOnly));
    }
}
