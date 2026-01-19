namespace Demoulas.ProfitSharing.Common.Time;

/// <summary>
/// Extension methods for <see cref="TimeProvider"/> to support common time operations.
/// </summary>
public static class TimeProviderExtensions
{
    /// <summary>
    /// Gets whether the time provider is returning fake/simulated time.
    /// </summary>
    /// <param name="timeProvider">The time provider to check.</param>
    /// <returns>True if the provider is a fake time provider; otherwise, false.</returns>
    public static bool IsFakeTime(this TimeProvider timeProvider)
    {
        return timeProvider is FakeTimeProvider ||
               (timeProvider is SwitchableTimeProvider switchable && switchable.IsFakeTimeActive);
    }

    /// <summary>
    /// Gets whether the time provider supports runtime switching between real and fake time.
    /// </summary>
    /// <param name="timeProvider">The time provider to check.</param>
    /// <returns>True if the provider supports runtime switching; otherwise, false.</returns>
    public static bool IsRuntimeSwitchingEnabled(this TimeProvider timeProvider)
    {
        return timeProvider is SwitchableTimeProvider;
    }

    /// <summary>
    /// Gets the switchable time provider if the provider supports runtime switching.
    /// </summary>
    /// <param name="timeProvider">The time provider to check.</param>
    /// <returns>The switchable time provider, or null if runtime switching is not supported.</returns>
    public static SwitchableTimeProvider? AsSwitchable(this TimeProvider timeProvider)
    {
        return timeProvider as SwitchableTimeProvider;
    }

    /// <summary>
    /// Gets the current local date as a <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    /// <returns>The current local date.</returns>
    public static DateOnly GetLocalDateOnly(this TimeProvider timeProvider)
    {
        return DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
    }

    /// <summary>
    /// Gets the current UTC date as a <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    /// <returns>The current UTC date.</returns>
    public static DateOnly GetUtcDateOnly(this TimeProvider timeProvider)
    {
        return DateOnly.FromDateTime(timeProvider.GetUtcNow().DateTime);
    }

    /// <summary>
    /// Gets the current local year.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    /// <returns>The current local year.</returns>
    public static int GetLocalYear(this TimeProvider timeProvider)
    {
        return timeProvider.GetLocalNow().Year;
    }

    /// <summary>
    /// Gets the current local year as a short (Int16).
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    /// <returns>The current local year as a short.</returns>
    public static short GetLocalYearAsShort(this TimeProvider timeProvider)
    {
        return (short)timeProvider.GetLocalNow().Year;
    }

    /// <summary>
    /// Gets the current local month.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    /// <returns>The current local month (1-12).</returns>
    public static int GetLocalMonth(this TimeProvider timeProvider)
    {
        return timeProvider.GetLocalNow().Month;
    }

    /// <summary>
    /// Gets the current local month as a byte.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    /// <returns>The current local month as a byte (1-12).</returns>
    public static byte GetLocalMonthAsByte(this TimeProvider timeProvider)
    {
        return (byte)timeProvider.GetLocalNow().Month;
    }
}
