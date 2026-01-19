using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Common.Time;

/// <summary>
/// A time provider that can switch between real system time and fake time at runtime.
/// This enables IT Operations to activate/deactivate fake time without restarting the application.
/// </summary>
/// <remarks>
/// SECURITY: This provider should only be registered in non-production environments.
/// In production, the standard <see cref="TimeProvider.System"/> should be used directly.
/// All switch operations are logged for auditing purposes.
/// </remarks>
public sealed class SwitchableTimeProvider : TimeProvider
{
    private readonly ILogger<SwitchableTimeProvider>? _logger;
    private readonly object _lock = new();
    private volatile TimeProvider _currentProvider;
    private volatile FakeTimeConfiguration? _currentConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchableTimeProvider"/> class.
    /// Starts with real system time.
    /// </summary>
    /// <param name="logger">Optional logger for diagnostics and audit logging.</param>
    public SwitchableTimeProvider(ILogger<SwitchableTimeProvider>? logger = null)
    {
        _logger = logger;
        _currentProvider = System;
        _currentConfig = null;

        _logger?.LogInformation(
            "SwitchableTimeProvider initialized with real system time. Runtime switching is enabled.");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchableTimeProvider"/> class with initial fake time.
    /// </summary>
    /// <param name="initialConfig">The initial fake time configuration to apply.</param>
    /// <param name="logger">Optional logger for diagnostics and audit logging.</param>
    public SwitchableTimeProvider(FakeTimeConfiguration initialConfig, ILogger<SwitchableTimeProvider>? logger = null)
    {
        _logger = logger;

        if (initialConfig.Enabled)
        {
            var fakeProvider = new FakeTimeProvider(initialConfig);
            _currentProvider = fakeProvider;
            _currentConfig = initialConfig;

            _logger?.LogWarning(
                "SwitchableTimeProvider initialized with FAKE time: {FakeDateTime}. " +
                "SECURITY WARNING: Fake time should only be used in non-production environments.",
                fakeProvider.GetUtcNow().ToString("O"));
        }
        else
        {
            _currentProvider = System;
            _currentConfig = null;

            _logger?.LogInformation(
                "SwitchableTimeProvider initialized with real system time. Runtime switching is enabled.");
        }
    }

    /// <inheritdoc />
    public override DateTimeOffset GetUtcNow() => _currentProvider.GetUtcNow();

    /// <inheritdoc />
    public override TimeZoneInfo LocalTimeZone => _currentProvider.LocalTimeZone;

    /// <summary>
    /// Gets whether fake time is currently active.
    /// </summary>
    public bool IsFakeTimeActive => _currentProvider is FakeTimeProvider;

    /// <summary>
    /// Gets the current fake time configuration, if fake time is active.
    /// Returns null if real time is active.
    /// </summary>
    public FakeTimeConfiguration? CurrentConfiguration => _currentConfig;

    /// <summary>
    /// Activates fake time with the specified configuration.
    /// </summary>
    /// <param name="config">The fake time configuration to apply.</param>
    /// <returns>True if fake time was activated; false if the configuration was invalid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when config is null.</exception>
    public bool ActivateFakeTime(FakeTimeConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var validationErrors = config.Validate();
        if (validationErrors.Count > 0)
        {
            _logger?.LogWarning(
                "Failed to activate fake time due to validation errors: {Errors}",
                string.Join("; ", validationErrors));
            return false;
        }

        lock (_lock)
        {
            var previousWasFake = IsFakeTimeActive;
            var previousTime = _currentProvider.GetUtcNow();

            var fakeProvider = new FakeTimeProvider(config);
            _currentProvider = fakeProvider;
            _currentConfig = config;

            _logger?.LogWarning(
                "FAKE TIME ACTIVATED. Previous: {PreviousMode} at {PreviousTime:O}. " +
                "New fake time: {FakeTime:O}, AdvanceTime: {AdvanceTime}, TimeZone: {TimeZone}. " +
                "SECURITY WARNING: Ensure this is not a production environment.",
                previousWasFake ? "Fake" : "Real",
                previousTime,
                fakeProvider.GetUtcNow(),
                config.AdvanceTime,
                config.TimeZone ?? "System Default");
        }

        return true;
    }

    /// <summary>
    /// Deactivates fake time and returns to real system time.
    /// </summary>
    public void DeactivateFakeTime()
    {
        lock (_lock)
        {
            if (!IsFakeTimeActive)
            {
                _logger?.LogDebug("DeactivateFakeTime called but fake time was not active.");
                return;
            }

            var previousFakeTime = _currentProvider.GetUtcNow();
            _currentProvider = System;
            _currentConfig = null;

            _logger?.LogInformation(
                "Fake time DEACTIVATED. Previous fake time: {PreviousFakeTime:O}. " +
                "Now using real system time: {RealTime:O}.",
                previousFakeTime,
                System.GetUtcNow());
        }
    }

    /// <summary>
    /// Gets the underlying time provider (for diagnostic purposes only).
    /// </summary>
    internal TimeProvider UnderlyingProvider => _currentProvider;
}
