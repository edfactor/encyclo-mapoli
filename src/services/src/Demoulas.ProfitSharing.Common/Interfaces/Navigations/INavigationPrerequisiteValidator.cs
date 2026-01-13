namespace Demoulas.ProfitSharing.Common.Interfaces.Navigations;

/// <summary>
/// Validates that all prerequisite navigations for a given navigation id are complete.
/// </summary>
public interface INavigationPrerequisiteValidator
{
    /// <summary>
    /// Throws a validation exception when any prerequisite is not complete.
    /// </summary>
    /// <param name="navigationId">Navigation.Constants value for the dependent navigation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ValidateAllCompleteAsync(short navigationId, CancellationToken cancellationToken);
}
