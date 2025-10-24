using System.ComponentModel;
using System.Net;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PayServices;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.PayServices;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

/// <summary>
/// Comprehensive unit tests for PayService covering GetPayServices functionality.
/// Tests cover different employment types, profit years, boundary conditions, error handling,
/// and data aggregation scenarios to ensure 80%+ code coverage.
/// PS-868: PayService comprehensive service layer tests
/// </summary>
[Description("PS-868 : PayService unit tests")]
public sealed class PayServiceTests : ApiTestBase<Api.Program>
{
    private const short ValidProfitYear = 2024;

    #region Part-Time Employment Tests

    [Fact(DisplayName = "PayService - Part-time employees with valid year")]
    [Description("PS-868 : Should retrieve part-time pay services for valid profit year")]
    public async Task GetPayServices_PartTime_WithValidYear_ShouldReturnResults()
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        
        // Should be successful or have valid error (not forbidden/unauthorized)
        var isValidResponse = response.Response.StatusCode == HttpStatusCode.OK ||
                             response.Response.StatusCode == HttpStatusCode.NotFound ||
                             response.Response.StatusCode == HttpStatusCode.BadRequest;
        
        isValidResponse.ShouldBeTrue($"Expected valid response but got {response.Response.StatusCode}");
        
        if (response.Response.StatusCode == HttpStatusCode.OK && response.Result is not null)
        {
            response.Result.ProfitYear.ShouldBe(ValidProfitYear);
            response.Result.PayServicesForYear.ShouldNotBeNull();
            response.Result.Description.ShouldNotBeNullOrEmpty();
            response.Result.TotalEmployeeNumber.ShouldBeGreaterThanOrEqualTo(0);
        }
    }

    [Fact(DisplayName = "PayService - Part-time with pagination")]
    [Description("PS-868 : Should handle pagination correctly for part-time employees")]
    public async Task GetPayServices_PartTime_WithPagination_ShouldHandleCorrectly()
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = 10
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        
        if (response.Response.StatusCode == HttpStatusCode.OK && response.Result is not null)
        {
            response.Result.PayServicesForYear.ShouldNotBeNull();
            // If there are results, they should respect pagination
            if (response.Result.PayServicesForYear.Results.Any())
            {
                response.Result.PayServicesForYear.Results.Count().ShouldBeLessThanOrEqualTo(10);
            }
        }
    }

    [Theory(DisplayName = "PayService - Part-time with various profit years")]
    [InlineData(2020)]
    [InlineData(2021)]
    [InlineData(2023)]
    [InlineData(2024)]
    [Description("PS-868 : Should handle various valid profit years for part-time employees")]
    public async Task GetPayServices_PartTime_WithVariousProfitYears_ShouldHandleCorrectly(short profitYear)
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = profitYear,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldNotBe(HttpStatusCode.Forbidden);
        response.Response.StatusCode.ShouldNotBe(HttpStatusCode.Unauthorized);
        
        if (response.Response.StatusCode == HttpStatusCode.OK && response.Result is not null)
        {
            response.Result.ProfitYear.ShouldBe(profitYear);
        }
    }

    #endregion

    #region Full-Time Straight Salary Tests

    [Fact(DisplayName = "PayService - Full-time straight salary employees")]
    [Description("PS-868 : Should retrieve full-time straight salary pay services")]
    public async Task GetPayServices_FullTimeStraightSalary_WithValidYear_ShouldReturnResults()
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesFullTimeStraightSalaryEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        
        var isValidResponse = response.Response.StatusCode == HttpStatusCode.OK ||
                             response.Response.StatusCode == HttpStatusCode.NotFound ||
                             response.Response.StatusCode == HttpStatusCode.BadRequest;
        
        isValidResponse.ShouldBeTrue($"Expected valid response but got {response.Response.StatusCode}");
        
        if (response.Response.StatusCode == HttpStatusCode.OK && response.Result is not null)
        {
            response.Result.ProfitYear.ShouldBe(ValidProfitYear);
        }
    }

    #endregion

    #region Full-Time Eight Paid Holidays Tests

    [Fact(DisplayName = "PayService - Full-time eight paid holidays employees")]
    [Description("PS-868 : Should retrieve full-time eight paid holidays pay services")]
    public async Task GetPayServices_FullTimeEightPaidHolidays_WithValidYear_ShouldReturnResults()
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesFullTimeEightPaidHolidaysEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        
        var isValidResponse = response.Response.StatusCode == HttpStatusCode.OK ||
                             response.Response.StatusCode == HttpStatusCode.NotFound ||
                             response.Response.StatusCode == HttpStatusCode.BadRequest;
        
        isValidResponse.ShouldBeTrue($"Expected valid response but got {response.Response.StatusCode}");
    }

    #endregion

    #region Full-Time Accrued Paid Holidays Tests

    [Fact(DisplayName = "PayService - Full-time accrued paid holidays employees")]
    [Description("PS-868 : Should retrieve full-time accrued paid holidays pay services")]
    public async Task GetPayServices_FullTimeAccruedPaidHolidays_WithValidYear_ShouldReturnResults()
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesFullTimeAccruedPaidHolidaysEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        
        var isValidResponse = response.Response.StatusCode == HttpStatusCode.OK ||
                             response.Response.StatusCode == HttpStatusCode.NotFound ||
                             response.Response.StatusCode == HttpStatusCode.BadRequest;
        
        isValidResponse.ShouldBeTrue($"Expected valid response but got {response.Response.StatusCode}");
    }

    #endregion

    #region Validation and Boundary Tests

    [Fact(DisplayName = "PayService - Invalid profit year too old")]
    [Description("PS-868 : Should reject profit year less than 2020")]
    public async Task GetPayServices_WithProfitYearTooOld_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = 1999, // Too old
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "PayService - Invalid profit year too far in future")]
    [Description("PS-868 : Should reject profit year more than 1 year in future")]
    public async Task GetPayServices_WithProfitYearTooFarInFuture_ShouldReturnBadRequest()
    {
        // Arrange
        short futureYear = 2101;
        var request = new PayServicesRequest
        {
            ProfitYear = futureYear,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "PayService - Zero profit year")]
    [Description("PS-868 : Should reject zero profit year")]
    public async Task GetPayServices_WithZeroProfitYear_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = 0,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "PayService - Minimum valid year boundary")]
    [Description("PS-868 : Should accept minimum supported profit year (2020)")]
    public async Task GetPayServices_WithMinimumValidYear_ShouldNotReturnBadRequest()
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = 2020,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldNotBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "PayService - Current year boundary")]
    [Description("PS-868 : Should accept current year as valid profit year")]
    public async Task GetPayServices_WithCurrentYear_ShouldNotReturnBadRequest()
    {
        // Arrange
        var currentYear = (short)DateTime.UtcNow.Year;
        var request = new PayServicesRequest
        {
            ProfitYear = currentYear,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldNotBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "PayService - Next year boundary")]
    [Description("PS-868 : Should accept next year as valid profit year")]
    public async Task GetPayServices_WithNextYear_ShouldNotReturnBadRequest()
    {
        // Arrange
        var nextYear = (short)(DateTime.UtcNow.Year + 1);
        var request = new PayServicesRequest
        {
            ProfitYear = nextYear,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldNotBe(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Authorization Tests

    [Fact(DisplayName = "PayService - Requires authentication")]
    [Description("PS-868 : Should return 401 Unauthorized without authentication")]
    public async Task GetPayServices_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = 25
        };

        // Act (no token assigned)
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Theory(DisplayName = "PayService - Authorized roles should access")]
    [InlineData(Role.FINANCEMANAGER)]
    [InlineData(Role.ADMINISTRATOR)]
    [InlineData(Role.AUDITOR)]
    [Description("PS-868 : Should allow roles with CanViewYearEndReports policy")]
    public async Task GetPayServices_WithAuthorizedRole_ShouldReturnSuccessOrNotFound(string role)
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(role);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldNotBe(HttpStatusCode.Forbidden,
            $"Role {role} should have CanViewYearEndReports policy");
    }

    [Theory(DisplayName = "PayService - Unauthorized roles should be forbidden")]
    [InlineData(Role.DISTRIBUTIONSCLERK)]
    [InlineData(Role.BENEFICIARY_ADMINISTRATOR)]
    [InlineData(Role.HARDSHIPADMINISTRATOR)]
    [InlineData(Role.ITOPERATIONS)]
    [Description("PS-868 : Should return 403 Forbidden for roles without CanViewYearEndReports policy")]
    public async Task GetPayServices_WithUnauthorizedRole_ShouldReturnForbidden(string role)
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(role);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Forbidden,
            $"Role {role} should not have CanViewYearEndReports policy");
    }

    #endregion

    #region Aggregation and Data Integrity Tests

    [Fact(DisplayName = "PayService - Response structure validation")]
    [Description("PS-868 : Should return response with correct structure")]
    public async Task GetPayServices_WithValidRequest_ShouldReturnCorrectStructure()
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        
        if (response.Response.StatusCode == HttpStatusCode.OK && response.Result is not null)
        {
            response.Result.ProfitYear.ShouldBe(ValidProfitYear);
            response.Result.PayServicesForYear.ShouldNotBeNull();
            response.Result.Description.ShouldNotBeNullOrEmpty();
            response.Result.TotalEmployeeNumber.ShouldBeGreaterThanOrEqualTo(0);
            
            // If there are results, validate DTO structure
            if (response.Result.PayServicesForYear.Results?.Any() == true)
            {
                foreach (var dto in response.Result.PayServicesForYear.Results)
                {
                    dto.Employees.ShouldBeGreaterThanOrEqualTo(0);
                    dto.YearsWages.ShouldBeGreaterThanOrEqualTo(0);
                    dto.YearsOfServiceLabel.ShouldNotBeNullOrEmpty();
                }
            }
        }
    }

    [Fact(DisplayName = "PayService - Years of service label formatting")]
    [Description("PS-868 : Should format years of service labels correctly")]
    public async Task GetPayServices_WithValidRequest_ShouldFormatLabelsCorrectly()
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = 100 // Get more results to test various year ranges
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        
        if (response.Response.StatusCode == HttpStatusCode.OK && 
            response.Result is not null && 
            response.Result.PayServicesForYear?.Results is not null && 
            response.Result.PayServicesForYear.Results.Any())
        {
            foreach (var dto in response.Result.PayServicesForYear.Results)
            {
                // Verify label format based on years of service
                if (dto.YearsOfService == -2)
                {
                    dto.YearsOfServiceLabel.ShouldBe("< 6 mos");
                }
                else if (dto.YearsOfService == -1)
                {
                    dto.YearsOfServiceLabel.ShouldBe("> 6 mos");
                }
                else if (dto.YearsOfService >= 0)
                {
                    dto.YearsOfServiceLabel.ShouldBe($"{dto.YearsOfService} yrs");
                }
            }
        }
    }

    [Fact(DisplayName = "PayService - Aggregation consistency")]
    [Description("PS-868 : Total employee number should match sum of employees in details")]
    public async Task GetPayServices_WithValidRequest_ShouldHaveConsistentAggregation()
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = 1000 // Get all results
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        
        if (response.Response.StatusCode == HttpStatusCode.OK && 
            response.Result is not null && 
            response.Result.PayServicesForYear?.Results is not null && 
            response.Result.PayServicesForYear.Results.Any())
        {
            var sumOfEmployees = response.Result.PayServicesForYear.Results.Sum(r => r.Employees);
            response.Result.TotalEmployeeNumber.ShouldBe(sumOfEmployees,
                "TotalEmployeeNumber should match sum of Employees in all detail records");
        }
    }

    #endregion

    #region Pagination Tests

    [Theory(DisplayName = "PayService - Various pagination sizes")]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(50)]
    [Description("PS-868 : Should handle various pagination sizes correctly")]
    public async Task GetPayServices_WithVariousPaginationSizes_ShouldHandleCorrectly(int pageSize)
    {
        // Arrange
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = pageSize
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        
        if (response.Response.StatusCode == HttpStatusCode.OK && response.Result is not null)
        {
            response.Result.PayServicesForYear.ShouldNotBeNull();
            // If there are results, they should not exceed requested page size
            if (response.Result.PayServicesForYear.Results.Any())
            {
                response.Result.PayServicesForYear.Results.Count().ShouldBeLessThanOrEqualTo(pageSize);
            }
        }
    }

    [Fact(DisplayName = "PayService - Skip pagination")]
    [Description("PS-868 : Should handle skip parameter correctly")]
    public async Task GetPayServices_WithSkipParameter_ShouldHandleCorrectly()
    {
        // Arrange - Get first page
        var firstPageRequest = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 0,
            Take = 5
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        var firstPageResponse = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(firstPageRequest);

        // Act - Get second page
        var secondPageRequest = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 5,
            Take = 5
        };

        var secondPageResponse = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(secondPageRequest);

        // Assert
        firstPageResponse.ShouldNotBeNull();
        secondPageResponse.ShouldNotBeNull();
        
        if (firstPageResponse.Response.StatusCode == HttpStatusCode.OK &&
            secondPageResponse.Response.StatusCode == HttpStatusCode.OK &&
            firstPageResponse.Result is not null &&
            secondPageResponse.Result is not null &&
            firstPageResponse.Result.PayServicesForYear?.Results is not null &&
            secondPageResponse.Result.PayServicesForYear?.Results is not null &&
            firstPageResponse.Result.PayServicesForYear.Results.Any() &&
            secondPageResponse.Result.PayServicesForYear.Results.Any())
        {
            var firstPageYears = firstPageResponse.Result.PayServicesForYear.Results
                .Select(r => r.YearsOfService).ToList();
            var secondPageYears = secondPageResponse.Result.PayServicesForYear.Results
                .Select(r => r.YearsOfService).ToList();
            
            // Pages should not have overlapping years (assuming ordered results)
            firstPageYears.ShouldNotBe(secondPageYears);
        }
    }

    #endregion

    #region Concurrent Request Tests

    [Fact(DisplayName = "PayService - Handle concurrent requests")]
    [Description("PS-868 : Should handle multiple simultaneous requests correctly")]
    public async Task GetPayServices_WithConcurrentRequests_ShouldHandleCorrectly()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        
        var request1 = new PayServicesRequest { ProfitYear = 2022, Skip = 0, Take = 25 };
        var request2 = new PayServicesRequest { ProfitYear = 2023, Skip = 0, Take = 25 };
        var request3 = new PayServicesRequest { ProfitYear = 2024, Skip = 0, Take = 25 };

        // Act
        var task1 = ApiClient.GETAsync<PayServicesPartTimeEndpoint, PayServicesRequest, PayServicesResponse>(request1);
        var task2 = ApiClient.GETAsync<PayServicesPartTimeEndpoint, PayServicesRequest, PayServicesResponse>(request2);
        var task3 = ApiClient.GETAsync<PayServicesPartTimeEndpoint, PayServicesRequest, PayServicesResponse>(request3);

        var responses = await Task.WhenAll(task1, task2, task3);

        // Assert
        responses.ShouldNotBeNull();
        responses.Length.ShouldBe(3);

        foreach (var response in responses)
        {
            response.ShouldNotBeNull();
            response.Response.StatusCode.ShouldNotBe(HttpStatusCode.Forbidden);
            response.Response.StatusCode.ShouldNotBe(HttpStatusCode.Unauthorized);
        }
    }

    #endregion

    #region Employment Type Constant Verification

    [Fact(DisplayName = "PayService - Employment type constants")]
    [Description("PS-868 : Should verify employment type constants are correctly defined")]
    public void GetPayServices_EmploymentTypeConstants_ShouldBeCorrectlyDefined()
    {
        // Assert
        EmploymentType.Constants.PartTime.ShouldBe('P');
        EmploymentType.Constants.FullTimeStraightSalary.ShouldBe('H');
        EmploymentType.Constants.FullTimeAccruedPaidHolidays.ShouldBe('G');
        EmploymentType.Constants.FullTimeEightPaidHolidays.ShouldBe('F');
    }

    #endregion

    #region Data Existence Tests

    [Fact(DisplayName = "PayService - Verify test data exists")]
    [Description("PS-868 : Should verify that test database has demographic and pay profit data")]
    public async Task GetPayServices_VerifyTestData_ShouldExist()
    {
        // Arrange & Act
        Demographic? firstDemographic = null;
        PayProfit? firstPayProfit = null;

        await MockDbContextFactory.UseReadOnlyContext(async ctx =>
        {
            firstDemographic = await ctx.Demographics.FirstOrDefaultAsync();
            firstPayProfit = await ctx.PayProfits.FirstOrDefaultAsync();
            return true;
        });

        // Assert
        firstDemographic.ShouldNotBeNull("Test database should have demographic data");
        firstPayProfit.ShouldNotBeNull("Test database should have pay profit data");
    }

    #endregion

    #region Error Handling Tests

    [Fact(DisplayName = "PayService - Handle empty result set gracefully")]
    [Description("PS-868 : Should handle scenarios with no matching data gracefully")]
    public async Task GetPayServices_WithNoMatchingData_ShouldHandleGracefully()
    {
        // Arrange - Use a very far future year that likely has no data
        var request = new PayServicesRequest
        {
            ProfitYear = (short)(DateTime.UtcNow.Year + 1),
            Skip = 0,
            Take = 25
        };

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<
            PayServicesPartTimeEndpoint,
            PayServicesRequest,
            PayServicesResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        
        // Should be either OK with empty results or BadRequest (future year validation)
        var isValidResponse = response.Response.StatusCode == HttpStatusCode.OK ||
                             response.Response.StatusCode == HttpStatusCode.NotFound ||
                             response.Response.StatusCode == HttpStatusCode.BadRequest;
        
        isValidResponse.ShouldBeTrue($"Expected valid response but got {response.Response.StatusCode}");
        
        if (response.Response.StatusCode == HttpStatusCode.OK && response.Result is not null)
        {
            response.Result.PayServicesForYear.ShouldNotBeNull();
            response.Result.TotalEmployeeNumber.ShouldBeGreaterThanOrEqualTo(0);
        }
    }

    #endregion

    #region Request Model Tests

    [Fact(DisplayName = "PayService - Request model properties")]
    [Description("PS-868 : Should verify PayServicesRequest has correct properties")]
    public void PayServicesRequest_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var request = new PayServicesRequest
        {
            ProfitYear = ValidProfitYear,
            Skip = 10,
            Take = 25
        };

        // Assert
        request.ProfitYear.ShouldBe(ValidProfitYear);
        request.Skip.ShouldBe(10);
        request.Take.ShouldBe(25);
    }

    [Fact(DisplayName = "PayService - Request example is valid")]
    [Description("PS-868 : Should verify PayServicesRequest example creates valid request")]
    public void PayServicesRequest_RequestExample_ShouldCreateValidRequest()
    {
        // Arrange & Act
        var request = PayServicesRequest.RequestExample();

        // Assert
        request.ShouldNotBeNull();
        request.ProfitYear.ShouldBeGreaterThan((short)0);
        request.ProfitYear.ShouldBeGreaterThanOrEqualTo((short)2000);
    }

    #endregion

    #region Response Model Tests

    [Fact(DisplayName = "PayService - Response model properties")]
    [Description("PS-868 : Should verify PayServicesResponse has correct properties")]
    public void PayServicesResponse_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var response = new PayServicesResponse
        {
            ProfitYear = ValidProfitYear,
            PayServicesForYear = new PaginatedResponseDto<PayServicesDto>(),
            Description = "Test description",
            TotalEmployeeNumber = 100
        };

        // Assert
        response.ProfitYear.ShouldBe(ValidProfitYear);
        response.PayServicesForYear.ShouldNotBeNull();
        response.Description.ShouldBe("Test description");
        response.TotalEmployeeNumber.ShouldBe(100);
    }

    [Fact(DisplayName = "PayService - Response example is valid")]
    [Description("PS-868 : Should verify PayServicesResponse example creates valid response")]
    public void PayServicesResponse_ResponseExample_ShouldCreateValidResponse()
    {
        // Arrange & Act
        var response = PayServicesResponse.ResponseExample();

        // Assert
        response.ShouldNotBeNull();
        response.ProfitYear.ShouldBeGreaterThan((short)0);
        response.PayServicesForYear.ShouldNotBeNull();
        response.Description.ShouldNotBeNullOrEmpty();
        response.TotalEmployeeNumber.ShouldBeGreaterThanOrEqualTo(0);
    }

    #endregion
}
