using Demoulas.ProfitSharing.Data.Contexts;

namespace Demoulas.ProfitSharing.Services.Beneficiaries.Validators;

/// <summary>
/// Model for database-dependent beneficiary validation.
/// Contains the context and data needed for database validation checks.
/// </summary>
public record BeneficiaryDatabaseValidationModel
{
    public required int BeneficiaryContactId { get; init; }
    public required int EmployeeBadgeNumber { get; init; }
    public required ProfitSharingDbContext Context { get; init; }
}
