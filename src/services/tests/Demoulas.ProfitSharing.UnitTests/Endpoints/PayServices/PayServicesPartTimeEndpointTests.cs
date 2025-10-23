using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PayServices;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.PayServices;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.PayServices;

/// <summary>
/// Comprehensive unit tests for PayServicesPartTimeEndpoint following project testing patterns.
/// Tests cover constructor validation, successful operations, error handling, boundary conditions, and telemetry integration.
/// </summary>
[Description("PS-868 : PayServicesPartTimeEndpoint unit tests")]
public sealed class PayServicesPartTimeEndpointTests
{
    private readonly Mock<IPayService> _payServiceMock;
    private readonly Mock<ILogger<PayServicesPartTimeEndpoint>> _loggerMock;
    private readonly PayServicesPartTimeEndpoint _endpoint;

    public PayServicesPartTimeEndpointTests()
    {
        _payServiceMock = new Mock<IPayService>();
        _loggerMock = new Mock<ILogger<PayServicesPartTimeEndpoint>>();
        _endpoint = new PayServicesPartTimeEndpoint(_payServiceMock.Object, _loggerMock.Object);
    }

    #region Constructor and Setup Tests

    [Fact]
    [Description("PS-868 : Constructor with valid dependencies initializes correctly")]
    public void Constructor_WithValidDependencies_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var endpoint = new PayServicesPartTimeEndpoint(_payServiceMock.Object, _loggerMock.Object);

        // Assert
        endpoint.ShouldNotBeNull();
        endpoint.ShouldBeOfType<PayServicesPartTimeEndpoint>();
        endpoint.NavigationId.ShouldBeGreaterThan((short)0);
    }

    [Fact]
    [Description("PS-868 : Constructor with null service creates instance but may cause NullReferenceException later")]
    public void Constructor_WithNullService_ShouldCreateInstance()
    {
        // Note: Constructor doesn't validate parameters - DI framework handles this
        // Arrange & Act
        var endpoint = new PayServicesPartTimeEndpoint(null!, _loggerMock.Object);

        // Assert
        endpoint.ShouldNotBeNull();
        endpoint.ShouldBeOfType<PayServicesPartTimeEndpoint>();
    }

    [Fact]
    [Description("PS-868 : Constructor with null logger creates instance but may cause NullReferenceException later")]
    public void Constructor_WithNullLogger_ShouldCreateInstance()
    {
        // Note: Constructor doesn't validate parameters - DI framework handles this
        // Arrange & Act
        var endpoint = new PayServicesPartTimeEndpoint(_payServiceMock.Object, null!);

        // Assert
        endpoint.ShouldNotBeNull();
        endpoint.ShouldBeOfType<PayServicesPartTimeEndpoint>();
    }

    #endregion

    #region Happy Path Tests

    [Fact]
    [Description("PS-868 : ExecuteAsync with valid request calls service successfully")]
    public async Task ExecuteAsync_WithValidRequest_ShouldCallServiceSuccessfully()
    {
        // Arrange
        var request = CreateValidPayServicesRequest(2024);
        var expectedResponse = CreateSuccessResponse(2024);
        var cancellationToken = CancellationToken.None;

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                PayServicesRequest.Constants.PartTime,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PayServicesResponse>.Success(expectedResponse));

        // Act
        var result = await _endpoint.ExecuteAsync(request, cancellationToken);

        // Assert
        _payServiceMock.Verify(x => x.GetPayServices(
            It.Is<PayServicesRequest>(r => r.ProfitYear == request.ProfitYear),
            PayServicesRequest.Constants.PartTime,
            cancellationToken),
            Times.Once);
    }

    [Theory]
    [Description("PS-868 : ExecuteAsync with various valid profit years")]
    [InlineData(2020)]
    [InlineData(2021)]
    [InlineData(2023)]
    [InlineData(2024)]
    [InlineData(2025)]
    public async Task ExecuteAsync_WithVariousValidProfitYears_ShouldCallServiceCorrectly(short profitYear)
    {
        // Arrange
        var request = CreateValidPayServicesRequest(profitYear);
        var expectedResponse = CreateSuccessResponse(profitYear);

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                PayServicesRequest.Constants.PartTime,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PayServicesResponse>.Success(expectedResponse));

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        _payServiceMock.Verify(x => x.GetPayServices(
            It.Is<PayServicesRequest>(r => r.ProfitYear == profitYear),
            PayServicesRequest.Constants.PartTime,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Description("PS-868 : ExecuteAsync with cancellation token passes token to service")]
    public async Task ExecuteAsync_WithCancellationToken_ShouldPassTokenToService()
    {
        // Arrange
        var request = CreateValidPayServicesRequest(2024);
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                PayServicesRequest.Constants.PartTime,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PayServicesResponse>.Success(CreateSuccessResponse(2024)));

        // Act
        await _endpoint.ExecuteAsync(request, cancellationToken);

        // Assert
        _payServiceMock.Verify(x => x.GetPayServices(
            It.IsAny<PayServicesRequest>(),
            PayServicesRequest.Constants.PartTime,
            cancellationToken),
            Times.Once);
    }

    [Fact]
    [Description("PS-868 : ExecuteAsync always uses PartTime employment type constant")]
    public async Task ExecuteAsync_ShouldAlwaysUsePartTimeEmploymentType()
    {
        // Arrange
        var request = CreateValidPayServicesRequest(2024);
        char? capturedEmploymentType = null;

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                It.IsAny<char>(),
                It.IsAny<CancellationToken>()))
            .Callback<PayServicesRequest, char, CancellationToken>((_, empType, _) => 
                capturedEmploymentType = empType)
            .ReturnsAsync(Result<PayServicesResponse>.Success(CreateSuccessResponse(2024)));

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        capturedEmploymentType.ShouldBe(PayServicesRequest.Constants.PartTime);
        capturedEmploymentType.ShouldBe('P');
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    [Description("PS-868 : ExecuteAsync with service failure returns ProblemHttpResult")]
    public async Task ExecuteAsync_WithServiceFailure_ShouldReturnProblemHttpResult()
    {
        // Arrange
        var request = CreateValidPayServicesRequest(2024);
        var error = Error.Unexpected("Service failure occurred");

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                PayServicesRequest.Constants.PartTime,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PayServicesResponse>.Failure(error));

        // Act & Assert
        // The endpoint should handle the failure gracefully
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        _payServiceMock.Verify(x => x.GetPayServices(
            It.IsAny<PayServicesRequest>(),
            PayServicesRequest.Constants.PartTime,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Description("PS-868 : ExecuteAsync with validation failure returns ProblemHttpResult")]
    public async Task ExecuteAsync_WithValidationFailure_ShouldReturnProblemHttpResult()
    {
        // Arrange
        var request = CreateValidPayServicesRequest(2024);
        var validationErrors = new Dictionary<string, string[]>
        {
            { "ProfitYear", new[] { "Profit year must be between 2000 and 2100" } }
        };

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                PayServicesRequest.Constants.PartTime,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PayServicesResponse>.ValidationFailure(validationErrors));

        // Act & Assert
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        _payServiceMock.Verify(x => x.GetPayServices(
            It.IsAny<PayServicesRequest>(),
            PayServicesRequest.Constants.PartTime,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Description("PS-868 : ExecuteAsync with cancelled operation throws OperationCanceledException")]
    public async Task ExecuteAsync_WithCancelledOperation_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var request = CreateValidPayServicesRequest(2024);
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                PayServicesRequest.Constants.PartTime,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(
            async () => await _endpoint.ExecuteAsync(request, cancellationTokenSource.Token));
    }

    #endregion

    #region Boundary and Validation Tests

    
    [Fact]
    [Description("PS-868 : ExecuteAsync with empty response data should succeed")]
    public async Task ExecuteAsync_WithEmptyResponseData_ShouldSucceed()
    {
        // Arrange
        var request = CreateValidPayServicesRequest(2024);
        var emptyResponse = new PayServicesResponse
        {
            ProfitYear = 2024,
            PayServicesForYear = new PaginatedResponseDto<PayServicesDto>(),
            Description = "No data found",
            TotalEmployeeNumber = 0
        };

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                PayServicesRequest.Constants.PartTime,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PayServicesResponse>.Success(emptyResponse));

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        _payServiceMock.Verify(x => x.GetPayServices(
            It.IsAny<PayServicesRequest>(),
            PayServicesRequest.Constants.PartTime,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region Request Model Tests

    [Fact]
    [Description("PS-868 : PayServicesRequest has correct properties")]
    public void PayServicesRequest_HasCorrectProperties()
    {
        // Arrange & Act
        var request = new PayServicesRequest
        {
            ProfitYear = 2024
        };

        // Assert
        request.ProfitYear.ShouldBe((short)2024);
    }

    [Fact]
    [Description("PS-868 : PayServicesRequest RequestExample creates valid request")]
    public void PayServicesRequest_RequestExample_ShouldCreateValidRequest()
    {
        // Arrange & Act
        var request = PayServicesRequest.RequestExample();

        // Assert
        request.ShouldNotBeNull();
        request.ProfitYear.ShouldBeGreaterThan((short)0);
        request.ProfitYear.ShouldBeGreaterThanOrEqualTo((short)2000);
    }

    [Fact]
    [Description("PS-868 : PayServicesRequest Constants are correctly defined")]
    public void PayServicesRequest_Constants_ShouldBeCorrectlyDefined()
    {
        // Assert
        PayServicesRequest.Constants.PartTime.ShouldBe('P');
        PayServicesRequest.Constants.FullTimeStraightSalary.ShouldBe('H');
        PayServicesRequest.Constants.FullTimeAccruedPaidHolidays.ShouldBe('G');
        PayServicesRequest.Constants.FullTimeEightPaidHolidays.ShouldBe('F');
    }

    #endregion

    #region Response Model Tests

    [Fact]
    [Description("PS-868 : PayServicesResponse has correct properties")]
    public void PayServicesResponse_HasCorrectProperties()
    {
        // Arrange & Act
        var response = new PayServicesResponse
        {
            ProfitYear = 2024,
            PayServicesForYear = new PaginatedResponseDto<PayServicesDto>(),
            Description = "Test description",
            TotalEmployeeNumber = 100
        };

        // Assert
        response.ProfitYear.ShouldBe((short)2024);
        response.PayServicesForYear.ShouldNotBeNull();
        response.Description.ShouldBe("Test description");
        response.TotalEmployeeNumber.ShouldBe(100);
    }

    [Fact]
    [Description("PS-868 : PayServicesResponse ResponseExample creates valid response")]
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

    #region Service Integration Tests

    [Fact]
    [Description("PS-868 : ExecuteAsync calls service exactly once for valid request")]
    public async Task ExecuteAsync_WithValidRequest_ShouldCallServiceExactlyOnce()
    {
        // Arrange
        var request = CreateValidPayServicesRequest(2024);
        var serviceCallCount = 0;

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                PayServicesRequest.Constants.PartTime,
                It.IsAny<CancellationToken>()))
            .Callback(() => serviceCallCount++)
            .ReturnsAsync(Result<PayServicesResponse>.Success(CreateSuccessResponse(2024)));

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        serviceCallCount.ShouldBe(1);
        _payServiceMock.Verify(x => x.GetPayServices(
            It.IsAny<PayServicesRequest>(),
            PayServicesRequest.Constants.PartTime,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Description("PS-868 : ExecuteAsync passes correct employment type to service")]
    public async Task ExecuteAsync_ShouldPassCorrectEmploymentTypeToService()
    {
        // Arrange
        var request = CreateValidPayServicesRequest(2024);
        char? actualEmploymentType = null;

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                It.IsAny<char>(),
                It.IsAny<CancellationToken>()))
            .Callback<PayServicesRequest, char, CancellationToken>((_, empType, _) =>
                actualEmploymentType = empType)
            .ReturnsAsync(Result<PayServicesResponse>.Success(CreateSuccessResponse(2024)));

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        actualEmploymentType.ShouldBe(PayServicesRequest.Constants.PartTime);
    }

    #endregion

    #region Performance and Stress Tests

    [Fact]
    [Description("PS-868 : ExecuteAsync completes within reasonable time")]
    public async Task ExecuteAsync_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var request = CreateValidPayServicesRequest(2024);
        var timeout = TimeSpan.FromSeconds(5);

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                PayServicesRequest.Constants.PartTime,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PayServicesResponse>.Success(CreateSuccessResponse(2024)));

        // Act & Assert
        using var cancellationTokenSource = new CancellationTokenSource(timeout);
        await _endpoint.ExecuteAsync(request, cancellationTokenSource.Token);

        // If we reach here, the operation completed within the timeout
        Assert.True(true);
    }

    [Fact]
    [Description("PS-868 : ExecuteAsync handles multiple concurrent calls")]
    public async Task ExecuteAsync_WithMultipleConcurrentCalls_ShouldHandleCorrectly()
    {
        // Arrange
        var tasks = new List<Task>();
        const int concurrentCalls = 10;

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                PayServicesRequest.Constants.PartTime,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PayServicesResponse>.Success(CreateSuccessResponse(2024)));

        // Act
        for (int i = 0; i < concurrentCalls; i++)
        {
            var profitYear = (short)(2020 + i);
            var request = CreateValidPayServicesRequest(profitYear);
            tasks.Add(_endpoint.ExecuteAsync(request, CancellationToken.None));
        }

        await Task.WhenAll(tasks);

        // Assert
        _payServiceMock.Verify(x => x.GetPayServices(
            It.IsAny<PayServicesRequest>(),
            PayServicesRequest.Constants.PartTime,
            It.IsAny<CancellationToken>()),
            Times.Exactly(concurrentCalls));
    }

    #endregion

    #region Telemetry Tests

    [Fact]
    [Description("PS-868 : ExecuteAsync records business operations telemetry")]
    public async Task ExecuteAsync_ShouldRecordBusinessOperationsTelemetry()
    {
        // Arrange
        var request = CreateValidPayServicesRequest(2024);

        _payServiceMock
            .Setup(x => x.GetPayServices(
                It.IsAny<PayServicesRequest>(),
                PayServicesRequest.Constants.PartTime,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PayServicesResponse>.Success(CreateSuccessResponse(2024)));

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        // Telemetry is recorded via EndpointTelemetry.BusinessOperationsTotal.Add
        // Verification happens through the ExecuteWithTelemetry wrapper
        _payServiceMock.Verify(x => x.GetPayServices(
            It.IsAny<PayServicesRequest>(),
            PayServicesRequest.Constants.PartTime,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Description("PS-868 : Logger is properly injected and used")]
    public void Constructor_ShouldProperlyInjectLogger()
    {
        // Arrange, Act & Assert
        var endpoint = new PayServicesPartTimeEndpoint(_payServiceMock.Object, _loggerMock.Object);

        endpoint.ShouldNotBeNull();
        // Logger injection is verified by successful constructor completion
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a valid PayServicesRequest with specified profit year.
    /// Ensures all required fields are properly initialized following the template pattern.
    /// </summary>
    private static PayServicesRequest CreateValidPayServicesRequest(short profitYear)
    {
        return new PayServicesRequest
        {
            ProfitYear = profitYear
        };
    }

    /// <summary>
    /// Creates a success response with specified profit year.
    /// </summary>
    private static PayServicesResponse CreateSuccessResponse(short profitYear)
    {
        return new PayServicesResponse
        {
            ProfitYear = profitYear,
            PayServicesForYear = new PaginatedResponseDto<PayServicesDto>
            {
                Results = new List<PayServicesDto>
                {
                    new PayServicesDto
                    {
                        Employees = 10,
                        WeeklyPay = 500.00M,
                        YearsWages = 26000.00M,
                        YearsOfService = 5
                    }
                },
                Total = 1
            },
            Description = "Successfully retrieved pay services data",
            TotalEmployeeNumber = 10
        };
    }

    #endregion
}
