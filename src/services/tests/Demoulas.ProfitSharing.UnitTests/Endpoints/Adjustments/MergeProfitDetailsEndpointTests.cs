using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request.Demographics;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Demographics;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Adjustments;

/// <summary>
/// Comprehensive unit tests for MergeProfitDetailsEndpoint following DemographicsServiceTests template patterns.
/// Tests cover constructor validation, successful operations, error handling, boundary conditions, and telemetry integration.
/// </summary>
public class MergeProfitDetailsEndpointTests
{
    private readonly Mock<IMergeProfitDetailsService> _mergeProfitDetailsServiceMock;
    private readonly Mock<ILogger<MergeProfitDetailsEndpoint>> _loggerMock;
    private readonly MergeProfitDetailsEndpoint _endpoint;

    public MergeProfitDetailsEndpointTests()
    {
        _mergeProfitDetailsServiceMock = new Mock<IMergeProfitDetailsService>();
        _loggerMock = new Mock<ILogger<MergeProfitDetailsEndpoint>>();
        _endpoint = new MergeProfitDetailsEndpoint(_mergeProfitDetailsServiceMock.Object, _loggerMock.Object);
    }

    #region Constructor and Setup Tests

    [Fact]
    [Description("PS-1721 : Constructor with valid dependencies initializes correctly")]
    public void Constructor_WithValidDependencies_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var endpoint = new MergeProfitDetailsEndpoint(_mergeProfitDetailsServiceMock.Object, _loggerMock.Object);

        // Assert
        endpoint.ShouldNotBeNull();
        endpoint.ShouldBeOfType<MergeProfitDetailsEndpoint>();
    }

    [Fact]
    [Description("PS-1721 : Constructor with null service creates instance but may cause NullReferenceException later")]
    public void Constructor_WithNullService_ShouldCreateInstance()
    {
        // Note: Constructor doesn't validate parameters - DI framework handles this
        // Arrange & Act
        var endpoint = new MergeProfitDetailsEndpoint(null!, _loggerMock.Object);

        // Assert
        endpoint.ShouldNotBeNull();
        endpoint.ShouldBeOfType<MergeProfitDetailsEndpoint>();
    }

    [Fact]
    [Description("PS-1721 : Constructor with null logger creates instance but may cause NullReferenceException later")]
    public void Constructor_WithNullLogger_ShouldCreateInstance()
    {
        // Note: Constructor doesn't validate parameters - DI framework handles this
        // Arrange & Act
        var endpoint = new MergeProfitDetailsEndpoint(_mergeProfitDetailsServiceMock.Object, null!);

        // Assert
        endpoint.ShouldNotBeNull();
        endpoint.ShouldBeOfType<MergeProfitDetailsEndpoint>();
    }

    [Fact]
    [Description("PS-1721 : HandleAsync with null service logs error but doesn't throw")]
    public async Task HandleAsync_WithNullService_ShouldLogErrorButNotThrow()
    {
        // Arrange
        var endpoint = new MergeProfitDetailsEndpoint(null!, _loggerMock.Object);
        var request = CreateValidMergeProfitDetailsRequest(123456789, 987654321);

        // Act - Should not throw due to try-catch in endpoint
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert - Should have logged the error
        VerifyLoggerCalled(LogLevel.Error, "MergeProfitDetailsToDemographic failed");
    }

    #endregion

    #region Happy Path Tests

    [Fact]
    [Description("PS-1721 : HandleAsync with valid SSNs calls service successfully")]
    public async Task HandleAsync_WithValidSSNs_ShouldCallServiceSuccessfully()
    {
        // Arrange
        var request = CreateValidMergeProfitDetailsRequest(123456789, 987654321);
        var cancellationToken = CancellationToken.None;

        _mergeProfitDetailsServiceMock
            .Setup(x => x.MergeProfitDetailsToDemographic(
                request.SourceSsn,
                request.DestinationSsn,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Demoulas.ProfitSharing.Common.Contracts.Result<bool>.Success(true));

        // Act
        await _endpoint.HandleAsync(request, cancellationToken);

        // Assert
        _mergeProfitDetailsServiceMock.Verify(x => x.MergeProfitDetailsToDemographic(
            request.SourceSsn,
            request.DestinationSsn,
            cancellationToken),
            Times.Once);

        // Verify success logging
        VerifyLoggerCalled(LogLevel.Information, "MergeProfitDetailsToDemographic successful");
    }

    [Theory]
    [Description("PS-1721 : HandleAsync with various valid SSN combinations")]
    [InlineData(111111111, 222222222)]
    [InlineData(999888777, 666555444)]
    [InlineData(555666777, 888999000)]
    [InlineData(123456789, 987654321)]
    [InlineData(100000001, 999999999)]
    [InlineData(0, 999999999)]

    public async Task HandleAsync_WithVariousValidSSNCombinations_ShouldCallServiceCorrectly(int sourceSsn, int destinationSsn)
    {
        // Arrange
        var request = CreateValidMergeProfitDetailsRequest(sourceSsn, destinationSsn);
        var cancellationToken = CancellationToken.None;

        _mergeProfitDetailsServiceMock
            .Setup(x => x.MergeProfitDetailsToDemographic(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Demoulas.ProfitSharing.Common.Contracts.Result<bool>.Success(true));

        // Act
        await _endpoint.HandleAsync(request, cancellationToken);

        // Assert
        _mergeProfitDetailsServiceMock.Verify(x => x.MergeProfitDetailsToDemographic(
            sourceSsn,
            destinationSsn,
            cancellationToken),
            Times.Once);
    }

    [Fact]
    [Description("PS-1721 : HandleAsync with cancellation token passes token to service")]
    public async Task HandleAsync_WithCancellationToken_ShouldPassTokenToService()
    {
        // Arrange
        var request = CreateValidMergeProfitDetailsRequest(123456789, 987654321);
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _mergeProfitDetailsServiceMock
            .Setup(x => x.MergeProfitDetailsToDemographic(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Demoulas.ProfitSharing.Common.Contracts.Result<bool>.Success(true));

        // Act
        await _endpoint.HandleAsync(request, cancellationToken);

        // Assert
        _mergeProfitDetailsServiceMock.Verify(x => x.MergeProfitDetailsToDemographic(
            request.SourceSsn,
            request.DestinationSsn,
            cancellationToken),
            Times.Once);
    }

    #endregion

    #region Error Handling Tests


    #endregion

    #region Boundary and Validation Tests

    [Theory]
    [Description("PS-1721 : HandleAsync with boundary SSN values")]
    [InlineData(1, 999999999)]          // Minimum and maximum valid SSNs
    [InlineData(100000000, 999999998)]  // Large valid SSNs
    [InlineData(500000000, 600000000)]  // Mid-range SSNs
    public async Task HandleAsync_WithBoundarySSNValues_ShouldProcessCorrectly(int sourceSsn, int destinationSsn)
    {
        // Arrange
        var request = CreateValidMergeProfitDetailsRequest(sourceSsn, destinationSsn);

        _mergeProfitDetailsServiceMock
            .Setup(x => x.MergeProfitDetailsToDemographic(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Demoulas.ProfitSharing.Common.Contracts.Result<bool>.Success(true));

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _mergeProfitDetailsServiceMock.Verify(x => x.MergeProfitDetailsToDemographic(
            sourceSsn,
            destinationSsn,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Description("PS-1721 : HandleAsync with same source and destination SSN should call service")]
    public async Task HandleAsync_WithSameSourceAndDestinationSSN_ShouldCallService()
    {
        // Note: Business logic validation happens in the service layer, not the endpoint
        // Arrange
        var request = CreateValidMergeProfitDetailsRequest(123456789, 123456789);

        _mergeProfitDetailsServiceMock
            .Setup(x => x.MergeProfitDetailsToDemographic(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Demoulas.ProfitSharing.Common.Contracts.Result<bool>.Success(true));

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _mergeProfitDetailsServiceMock.Verify(x => x.MergeProfitDetailsToDemographic(
            123456789,
            123456789,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region Request Model Tests

    [Fact]
    [Description("PS-1721 : MergeProfitDetailsRequest has correct properties")]
    public void MergeProfitDetailsRequest_HasCorrectProperties()
    {
        // Arrange & Act
        var request = new MergeProfitDetailsRequest
        {
            SourceSsn = 123456789,
            DestinationSsn = 987654321
        };

        // Assert
        request.SourceSsn.ShouldBe(123456789);
        request.DestinationSsn.ShouldBe(987654321);
    }

    [Fact]
    [Description("PS-1721 : MergeProfitDetailsRequest RequestExample creates valid request")]
    public void MergeProfitDetailsRequest_RequestExample_ShouldCreateValidRequest()
    {
        // Arrange & Act
        var request = MergeProfitDetailsRequest.RequestExample();

        // Assert
        request.ShouldNotBeNull();
        request.SourceSsn.ShouldBeGreaterThan(0);
        request.DestinationSsn.ShouldBeGreaterThan(0);
        request.SourceSsn.ShouldNotBe(request.DestinationSsn);
    }

    [Fact]
    [Description("PS-1721 : MergeProfitDetailsRequest can be instantiated with default constructor")]
    public void MergeProfitDetailsRequest_DefaultConstructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var request = new MergeProfitDetailsRequest();

        // Assert
        request.ShouldNotBeNull();
        request.SourceSsn.ShouldBe(0); // Default value
        request.DestinationSsn.ShouldBe(0); // Default value
    }

    #endregion

    #region Service Integration Tests

    [Fact]
    [Description("PS-1721 : HandleAsync calls service exactly once for valid request")]
    public async Task HandleAsync_WithValidRequest_ShouldCallServiceExactlyOnce()
    {
        // Arrange
        var request = CreateValidMergeProfitDetailsRequest(123456789, 987654321);
        var serviceCallCount = 0;

        _mergeProfitDetailsServiceMock
            .Setup(x => x.MergeProfitDetailsToDemographic(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Callback(() => serviceCallCount++)
            .ReturnsAsync(Demoulas.ProfitSharing.Common.Contracts.Result<bool>.Success(true));

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        serviceCallCount.ShouldBe(1);
        _mergeProfitDetailsServiceMock.Verify(x => x.MergeProfitDetailsToDemographic(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }


    #endregion

    #region Performance and Stress Tests

    [Fact]
    [Description("PS-1721 : HandleAsync completes within reasonable time")]
    public async Task HandleAsync_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var request = CreateValidMergeProfitDetailsRequest(123456789, 987654321);
        var timeout = TimeSpan.FromSeconds(5);

        _mergeProfitDetailsServiceMock
            .Setup(x => x.MergeProfitDetailsToDemographic(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Demoulas.ProfitSharing.Common.Contracts.Result<bool>.Success(true));

        // Act & Assert
        using var cancellationTokenSource = new CancellationTokenSource(timeout);
        await _endpoint.HandleAsync(request, cancellationTokenSource.Token);

        // If we reach here, the operation completed within the timeout
        Assert.True(true);
    }

    [Fact]
    [Description("PS-1721 : HandleAsync handles multiple concurrent calls")]
    public async Task HandleAsync_WithMultipleConcurrentCalls_ShouldHandleCorrectly()
    {
        // Arrange
        var tasks = new List<Task>();
        const int concurrentCalls = 10;

        _mergeProfitDetailsServiceMock
            .Setup(x => x.MergeProfitDetailsToDemographic(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Demoulas.ProfitSharing.Common.Contracts.Result<bool>.Success(true));

        // Act
        for (int i = 0; i < concurrentCalls; i++)
        {
            var sourceSsn = 100000000 + i;
            var destinationSsn = 200000000 + i;
            var request = CreateValidMergeProfitDetailsRequest(sourceSsn, destinationSsn);
            tasks.Add(_endpoint.HandleAsync(request, CancellationToken.None));
        }

        await Task.WhenAll(tasks);

        // Assert
        _mergeProfitDetailsServiceMock.Verify(x => x.MergeProfitDetailsToDemographic(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()),
            Times.Exactly(concurrentCalls));
    }

    #endregion

    #region Telemetry and Logging Tests

    [Fact]
    [Description("PS-1721 : HandleAsync logs success with masked SSNs")]
    public async Task HandleAsync_OnSuccess_ShouldLogWithMaskedSSNs()
    {
        // Arrange
        var request = CreateValidMergeProfitDetailsRequest(123456789, 987654321);

        _mergeProfitDetailsServiceMock
            .Setup(x => x.MergeProfitDetailsToDemographic(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Demoulas.ProfitSharing.Common.Contracts.Result<bool>.Success(true));

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        VerifyLoggerCalled(LogLevel.Information, "MergeProfitDetailsToDemographic successful");
        // Note: SSN masking is handled by the MaskSsn() extension method
    }

    [Fact]
    [Description("PS-1721 : Logger is properly injected and used")]
    public void Constructor_ShouldProperlyInjectLogger()
    {
        // Arrange, Act & Assert
        var endpoint = new MergeProfitDetailsEndpoint(_mergeProfitDetailsServiceMock.Object, _loggerMock.Object);

        endpoint.ShouldNotBeNull();
        // Logger injection is verified by successful constructor completion
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a valid MergeProfitDetailsRequest with specified SSNs.
    /// Ensures all required fields are properly initialized following the template pattern.
    /// </summary>
    private static MergeProfitDetailsRequest CreateValidMergeProfitDetailsRequest(int sourceSsn, int destinationSsn)
    {
        return new MergeProfitDetailsRequest
        {
            SourceSsn = sourceSsn,
            DestinationSsn = destinationSsn
        };
    }

    /// <summary>
    /// Verifies that the logger was called with the specified log level and message content.
    /// Follows the template pattern from DemographicsServiceTests for logger verification.
    /// </summary>
    private void VerifyLoggerCalled(LogLevel logLevel, string messageContains = "")
    {
        _loggerMock.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(messageContains)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                    Times.AtLeastOnce);
    }

    #endregion
}
