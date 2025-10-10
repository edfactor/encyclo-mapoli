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

public class CalendarRecordRangeEndpointTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "CalendarRecordRange - Should return success for valid year range")]
    [Description("PS-#### : Returns calendar dates for valid profit year range")]
    public async Task Get_ReturnsSuccess_ForValidYearRange()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new YearRangeRequest { BeginProfitYear = 2024, EndProfitYear = 2025 };

        // Act
        var response = await ApiClient.GETAsync<CalendarRecordRangeEndpoint, YearRangeRequest, CalendarResponseDto>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "CalendarRecordRange - Should return valid date range")]
    [Description("PS-#### : Fiscal begin date should be before fiscal end date")]
    public async Task Get_ReturnsValidDateRange_ForYearRange()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new YearRangeRequest { BeginProfitYear = 2024, EndProfitYear = 2025 };

        // Act
        var response = await ApiClient.GETAsync<CalendarRecordRangeEndpoint, YearRangeRequest, CalendarResponseDto>(request);

        // Assert
        response.Result!.FiscalBeginDate.ShouldBeLessThan(response.Result.FiscalEndDate);
    }

    [Fact(DisplayName = "CalendarRecordRange - Should work with FINANCEMANAGER role")]
    [Description("PS-#### : Allows access with finance manager role")]
    public async Task Get_ReturnsSuccess_WithFinanceManagerRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new YearRangeRequest { BeginProfitYear = 2024, EndProfitYear = 2025 };

        // Act
        var response = await ApiClient.GETAsync<CalendarRecordRangeEndpoint, YearRangeRequest, CalendarResponseDto>(request);

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "CalendarRecordRange - Should work with AUDITOR role")]
    [Description("PS-#### : Allows access with auditor role")]
    public async Task Get_ReturnsSuccess_WithAuditorRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.AUDITOR);
        var request = new YearRangeRequest { BeginProfitYear = 2024, EndProfitYear = 2025 };

        // Act
        var response = await ApiClient.GETAsync<CalendarRecordRangeEndpoint, YearRangeRequest, CalendarResponseDto>(request);

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "CalendarRecordRange - Should handle same begin and end year")]
    [Description("PS-#### : Returns valid dates when begin and end year are the same")]
    public async Task Get_ReturnsValidDates_ForSameBeginAndEndYear()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new YearRangeRequest { BeginProfitYear = 2024, EndProfitYear = 2024 };

        // Act
        var response = await ApiClient.GETAsync<CalendarRecordRangeEndpoint, YearRangeRequest, CalendarResponseDto>(request);

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
        response.Result!.FiscalBeginDate.ShouldBeLessThanOrEqualTo(response.Result.FiscalEndDate);
    }

    [Fact(DisplayName = "CalendarRecordRange - Should handle multi-year range")]
    [Description("PS-#### : Returns dates spanning multiple years")]
    public async Task Get_ReturnsValidDates_ForMultiYearRange()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new YearRangeRequest { BeginProfitYear = 2023, EndProfitYear = 2025 };

        // Act
        var response = await ApiClient.GETAsync<CalendarRecordRangeEndpoint, YearRangeRequest, CalendarResponseDto>(request);

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
        response.Result!.FiscalBeginDate.ShouldBeLessThan(response.Result.FiscalEndDate);
    }

    [Fact(DisplayName = "CalendarRecordRange - Should return non-default dates")]
    [Description("PS-#### : Fiscal dates should not be default values")]
    public async Task Get_ReturnsNonDefaultDates_ForValidYearRange()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new YearRangeRequest { BeginProfitYear = 2024, EndProfitYear = 2025 };

        // Act
        var response = await ApiClient.GETAsync<CalendarRecordRangeEndpoint, YearRangeRequest, CalendarResponseDto>(request);

        // Assert
        response.Result!.FiscalBeginDate.ShouldNotBe(default(DateOnly));
        response.Result.FiscalEndDate.ShouldNotBe(default(DateOnly));
    }

    [Fact(DisplayName = "CalendarRecordRange - Should return different ranges for different inputs")]
    [Description("PS-#### : Different year ranges produce different date ranges")]
    public async Task Get_ReturnsDifferentRanges_ForDifferentYearRanges()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request1 = new YearRangeRequest { BeginProfitYear = 2024, EndProfitYear = 2024 };
        var request2 = new YearRangeRequest { BeginProfitYear = 2024, EndProfitYear = 2025 };

        // Act
        var response1 = await ApiClient.GETAsync<CalendarRecordRangeEndpoint, YearRangeRequest, CalendarResponseDto>(request1);
        var response2 = await ApiClient.GETAsync<CalendarRecordRangeEndpoint, YearRangeRequest, CalendarResponseDto>(request2);

        // Assert
        response1.Result!.FiscalEndDate.ShouldNotBe(response2.Result!.FiscalEndDate);
    }
}
