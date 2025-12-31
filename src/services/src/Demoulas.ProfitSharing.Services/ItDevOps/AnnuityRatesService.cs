using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.ItDevOps;

public sealed class AnnuityRatesService : IAnnuityRatesService
{
    private const decimal MaxRate = 99.9999m;
    private static readonly Error s_annuityRateNotFound = Error.EntityNotFound("Annuity rate");

    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly IAuditService _auditService;
    private readonly ICommitGuardOverride _commitGuardOverride;
    private readonly IAppUser _appUser;
    private readonly ILogger<AnnuityRatesService> _logger;

    public AnnuityRatesService(
        IProfitSharingDataContextFactory contextFactory,
        IAuditService auditService,
        ICommitGuardOverride commitGuardOverride,
        IAppUser appUser,
        ILogger<AnnuityRatesService> logger)
    {
        _contextFactory = contextFactory;
        _auditService = auditService;
        _commitGuardOverride = commitGuardOverride;
        _appUser = appUser;
        _logger = logger;
    }

    public Task<Result<IReadOnlyList<AnnuityRateDto>>> GetAnnuityRatesAsync(GetAnnuityRatesRequest request, CancellationToken cancellationToken)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var sortBy = request.SortBy;
            var isSortDescending = request.IsSortDescending is true;

            var query = ctx.AnnuityRates
                .TagWith("ItDevOps-GetAnnuityRates");

            query = sortBy switch
            {
                "Age" => isSortDescending
                    ? query.OrderByDescending(x => x.Age).ThenByDescending(x => x.Year)
                    : query.OrderBy(x => x.Age).ThenBy(x => x.Year),
                _ => isSortDescending
                    ? query.OrderByDescending(x => x.Year).ThenBy(x => x.Age)
                    : query.OrderBy(x => x.Year).ThenBy(x => x.Age),
            };

            var results = await query
                .Select(x => new AnnuityRateDto
                {
                    Year = x.Year,
                    Age = x.Age,
                    SingleRate = x.SingleRate,
                    JointRate = x.JointRate,
                    DateModified = x.ModifiedAtUtc != null ? DateOnly.FromDateTime(x.ModifiedAtUtc.Value.DateTime) : null,
                    UserModified = x.UserName,
                })
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<AnnuityRateDto>>.Success(results);
        }, cancellationToken);
    }

    public Task<Result<IReadOnlyList<AnnuityRateDto>>> GetAnnuityRatesByYearAsync(short year, CancellationToken cancellationToken)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var results = await ctx.AnnuityRates
                .TagWith($"ItDevOps-GetAnnuityRatesByYear-{year}")
                .Where(x => x.Year == year)
                .OrderBy(x => x.Age)
                .Select(x => new AnnuityRateDto
                {
                    Year = x.Year,
                    Age = x.Age,
                    SingleRate = x.SingleRate,
                    JointRate = x.JointRate,
                    DateModified = x.ModifiedAtUtc != null ? DateOnly.FromDateTime(x.ModifiedAtUtc.Value.DateTime) : null,
                    UserModified = x.UserName,
                })
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<AnnuityRateDto>>.Success(results);
        }, cancellationToken);
    }

    public async Task<Result<AnnuityRateDto>> UpdateAnnuityRateAsync(UpdateAnnuityRateRequest request, CancellationToken cancellationToken)
    {
        using (_commitGuardOverride.AllowFor(roles: Role.ITDEVOPS))
        {
            return await _contextFactory.UseWritableContext(async ctx =>
            {
                if (!IsValidYear(request.Year))
                {
                    return Result<AnnuityRateDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Year)] = ["Year must be between 1900 and 2100."],
                    }));
                }

                if (!IsValidAge(request.Age))
                {
                    return Result<AnnuityRateDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Age)] = ["Age must be between 0 and 120."],
                    }));
                }

                var roundedSingle = Math.Round(request.SingleRate, 4, MidpointRounding.AwayFromZero);
                if (roundedSingle != request.SingleRate || roundedSingle < 0m || roundedSingle > MaxRate)
                {
                    return Result<AnnuityRateDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.SingleRate)] = ["SingleRate must be between 0 and 99.9999 with up to 4 decimal places."],
                    }));
                }

                var roundedJoint = Math.Round(request.JointRate, 4, MidpointRounding.AwayFromZero);
                if (roundedJoint != request.JointRate || roundedJoint < 0m || roundedJoint > MaxRate)
                {
                    return Result<AnnuityRateDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.JointRate)] = ["JointRate must be between 0 and 99.9999 with up to 4 decimal places."],
                    }));
                }

                var annuityRate = await ctx.AnnuityRates
                    .TagWith($"ItDevOps-UpdateAnnuityRate-{request.Year}-{request.Age}")
                    .FirstOrDefaultAsync(x => x.Year == request.Year && x.Age == request.Age, cancellationToken);

                if (annuityRate is null)
                {
                    return Result<AnnuityRateDto>.Failure(s_annuityRateNotFound);
                }

                var originalSingle = annuityRate.SingleRate;
                var originalJoint = annuityRate.JointRate;

                if (originalSingle == roundedSingle && originalJoint == roundedJoint)
                {
                    return Result<AnnuityRateDto>.Success(new AnnuityRateDto
                    {
                        Year = annuityRate.Year,
                        Age = annuityRate.Age,
                        SingleRate = annuityRate.SingleRate,
                        JointRate = annuityRate.JointRate,
                        DateModified = annuityRate.ModifiedAtUtc != null ? DateOnly.FromDateTime(annuityRate.ModifiedAtUtc.Value.DateTime) : null,
                        UserModified = annuityRate.UserName,
                    });
                }

                annuityRate.SingleRate = roundedSingle;
                annuityRate.JointRate = roundedJoint;
                annuityRate.UserName = _appUser.UserName ?? "";
                annuityRate.ModifiedAtUtc = DateTimeOffset.UtcNow;

                await ctx.SaveChangesAsync(cancellationToken);

                try
                {
                    var changes = new List<AuditChangeEntryInput>(capacity: 2);

                    if (originalSingle != roundedSingle)
                    {
                        changes.Add(new AuditChangeEntryInput
                        {
                            ColumnName = "SINGLE_RATE",
                            OriginalValue = originalSingle.ToString("0.0000"),
                            NewValue = roundedSingle.ToString("0.0000"),
                        });
                    }

                    if (originalJoint != roundedJoint)
                    {
                        changes.Add(new AuditChangeEntryInput
                        {
                            ColumnName = "JOINT_RATE",
                            OriginalValue = originalJoint.ToString("0.0000"),
                            NewValue = roundedJoint.ToString("0.0000"),
                        });
                    }

                    await _auditService.LogDataChangeAsync(
                        operationName: "Update Annuity Rate",
                        tableName: "ANNUITY_RATE",
                        auditOperation: AuditEvent.AuditOperations.Update,
                        primaryKey: $"Year:{request.Year},Age:{request.Age}",
                        changes: changes,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to write audit log for ANNUITY_RATE update (Year={Year}, Age={Age})", request.Year, request.Age);
                }

                return Result<AnnuityRateDto>.Success(new AnnuityRateDto
                {
                    Year = annuityRate.Year,
                    Age = annuityRate.Age,
                    SingleRate = annuityRate.SingleRate,
                    JointRate = annuityRate.JointRate,
                    DateModified = annuityRate.ModifiedAtUtc != null ? DateOnly.FromDateTime(annuityRate.ModifiedAtUtc.Value.DateTime) : null,
                    UserModified = annuityRate.UserName,
                });
            }, cancellationToken);
        }
    }

    private static bool IsValidYear(short year)
    {
        return year is >= 1900 and <= 2100;
    }

    private static bool IsValidAge(byte age)
    {
        return age <= 120;
    }
}
