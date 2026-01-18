using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.ItDevOps;

public sealed class StateTaxRatesService : IStateTaxRatesService
{
    private static readonly Error s_stateTaxNotFound = Error.EntityNotFound("State tax");

    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly IProfitSharingAuditService _profitSharingAuditService;
    private readonly ICommitGuardOverride _commitGuardOverride;
    private readonly IAppUser _appUser;
    private readonly StateTaxCache _stateTaxCache;
    private readonly ILogger<StateTaxRatesService> _logger;

    public StateTaxRatesService(
        IProfitSharingDataContextFactory contextFactory,
        IProfitSharingAuditService profitSharingAuditService,
        ICommitGuardOverride commitGuardOverride,
        IAppUser appUser,
        StateTaxCache stateTaxCache,
        ILogger<StateTaxRatesService> logger)
    {
        _contextFactory = contextFactory;
        _profitSharingAuditService = profitSharingAuditService;
        _commitGuardOverride = commitGuardOverride;
        _appUser = appUser;
        _stateTaxCache = stateTaxCache;
        _logger = logger;
    }

    public Task<Result<IReadOnlyList<StateTaxRateDto>>> GetStateTaxRatesAsync(CancellationToken cancellationToken)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var results = await ctx.StateTaxes
                .TagWith("ItDevOps-GetStateTaxRates")
                .OrderBy(x => x.Abbreviation)
                .Select(x => new StateTaxRateDto
                {
                    Abbreviation = x.Abbreviation,
                    Rate = x.Rate,
                    DateModified = x.ModifiedAtUtc != null ? DateOnly.FromDateTime(x.ModifiedAtUtc.Value.DateTime) : null,
                    UserModified = x.UserName,
                })
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<StateTaxRateDto>>.Success(results);
        }, cancellationToken);
    }

    public async Task<Result<StateTaxRateDto>> UpdateStateTaxRateAsync(UpdateStateTaxRateRequest request, CancellationToken cancellationToken)
    {
        using (_commitGuardOverride.AllowFor(roles: Role.ITDEVOPS))
        {
            return await _contextFactory.UseWritableContext(async ctx =>
            {
                var abbreviation = (request.Abbreviation ?? string.Empty).Trim().ToUpperInvariant();
                if (!IsValidAbbreviation(abbreviation))
                {
                    return Result<StateTaxRateDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Abbreviation)] = ["Abbreviation must be two letters (A-Z)."],
                    }));
                }

                var roundedRate = Math.Round(request.Rate, 2, MidpointRounding.AwayFromZero);
                if (roundedRate != request.Rate || roundedRate < 0m || roundedRate > 100m)
                {
                    return Result<StateTaxRateDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Rate)] = ["Rate must be between 0 and 100 with up to 2 decimal places."],
                    }));
                }

                var stateTax = await ctx.StateTaxes
                    .TagWith($"ItDevOps-UpdateStateTaxRate-{abbreviation}")
                    .FirstOrDefaultAsync(x => x.Abbreviation == abbreviation, cancellationToken);

                if (stateTax is null)
                {
                    return Result<StateTaxRateDto>.Failure(s_stateTaxNotFound);
                }

                var originalRate = stateTax.Rate;
                if (originalRate == roundedRate)
                {
                    return Result<StateTaxRateDto>.Success(new StateTaxRateDto
                    {
                        Abbreviation = stateTax.Abbreviation,
                        Rate = stateTax.Rate,
                        DateModified = stateTax.ModifiedAtUtc != null ? DateOnly.FromDateTime(stateTax.ModifiedAtUtc.Value.DateTime) : null,
                        UserModified = stateTax.UserName,
                    });
                }

                stateTax.Rate = roundedRate;
                stateTax.UserName = _appUser.UserName ?? "";
                stateTax.ModifiedAtUtc = DateTimeOffset.UtcNow;

                await ctx.SaveChangesAsync(cancellationToken);

                try
                {
                    await _stateTaxCache.InvalidateAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to invalidate StateTaxCache after updating {Abbreviation}", abbreviation);
                }

                await _profitSharingAuditService.LogDataChangeAsync(
                    operationName: "Update State Tax Rate",
                    tableName: "STATE_TAX",
                    auditOperation: AuditEvent.AuditOperations.Update,
                    primaryKey: $"Abbreviation:{abbreviation}",
                    changes:
                    [
                        new AuditChangeEntryInput
                        {
                            ColumnName = "RATE",
                            OriginalValue = originalRate.ToString("0.00"),
                            NewValue = roundedRate.ToString("0.00"),
                        },
                    ],
                    cancellationToken);

                return Result<StateTaxRateDto>.Success(new StateTaxRateDto
                {
                    Abbreviation = stateTax.Abbreviation,
                    Rate = stateTax.Rate,
                    DateModified = stateTax.ModifiedAtUtc != null ? DateOnly.FromDateTime(stateTax.ModifiedAtUtc.Value.DateTime) : null,
                    UserModified = stateTax.UserName,
                });
            }, cancellationToken);
        }
    }

    private static bool IsValidAbbreviation(string abbreviation)
    {
        return abbreviation.Length == 2 && abbreviation.All(c => c is >= 'A' and <= 'Z');
    }
}
