using Demoulas.Common.Contracts.Contracts.Request.Audit;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Entities.Entities.Audit;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Administration;

public sealed class TaxCodeService : ITaxCodeService
{
    private static readonly Error _taxCodeNotFound = Error.EntityNotFound("Tax code");

    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly IProfitSharingAuditService _profitSharingAuditService;
    private readonly ICommitGuardOverride _commitGuardOverride;
    private readonly ILogger<TaxCodeService> _logger;

    public TaxCodeService(
        IProfitSharingDataContextFactory contextFactory,
        IProfitSharingAuditService profitSharingAuditService,
        ICommitGuardOverride commitGuardOverride,
        ILogger<TaxCodeService> logger)
    {
        _contextFactory = contextFactory;
        _profitSharingAuditService = profitSharingAuditService;
        _commitGuardOverride = commitGuardOverride;
        _logger = logger;
    }

    public Task<Result<IReadOnlyList<TaxCodeAdminDto>>> GetTaxCodesAsync(CancellationToken cancellationToken)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var results = await ctx.TaxCodes
                .TagWith("Administration-GetTaxCodes")
                .OrderBy(x => x.Id)
                .Select(x => new TaxCodeAdminDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsAvailableForDistribution = x.IsAvailableForDistribution,
                    IsAvailableForForfeiture = x.IsAvailableForForfeiture,
                    IsProtected = x.IsProtected
                })
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<TaxCodeAdminDto>>.Success(results);
        }, cancellationToken);
    }

    public async Task<Result<TaxCodeAdminDto>> CreateTaxCodeAsync(CreateTaxCodeRequest request, CancellationToken cancellationToken)
    {
        using (_commitGuardOverride.AllowFor(roles: Role.ITDEVOPS))
        {
            return await _contextFactory.UseWritableContext(async ctx =>
            {
                var trimmedName = (request.Name ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(trimmedName))
                {
                    return Result<TaxCodeAdminDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Name)] = ["Name is required."],
                    }));
                }

                if (trimmedName.Length > 128)
                {
                    return Result<TaxCodeAdminDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Name)] = ["Name must be 128 characters or less."],
                    }));
                }

                var id = NormalizeTaxCodeId(request.Id);
                if (id is null)
                {
                    return Result<TaxCodeAdminDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Id)] = ["Id must be a single letter or number."],
                    }));
                }

                var exists = await ctx.TaxCodes
                    .TagWith($"Administration-CheckTaxCode-{id}")
                    .AnyAsync(x => x.Id == id.Value, cancellationToken);

                if (exists)
                {
                    return Result<TaxCodeAdminDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Id)] = ["A tax code with this ID already exists."],
                    }));
                }

                var nameExists = await ctx.TaxCodes
                    .TagWith("Administration-CheckDuplicateTaxCodeName")
                    .AnyAsync(x => x.Name == trimmedName, cancellationToken);

                if (nameExists)
                {
                    return Result<TaxCodeAdminDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Name)] = ["A tax code with this name already exists."],
                    }));
                }

                var taxCode = new Data.Entities.TaxCode
                {
                    Id = id.Value,
                    Name = trimmedName,
                    IsAvailableForDistribution = request.IsAvailableForDistribution,
                    IsAvailableForForfeiture = request.IsAvailableForForfeiture,
                    IsProtected = request.IsProtected
                };

                ctx.TaxCodes.Add(taxCode);
                await ctx.SaveChangesAsync(cancellationToken);

                await _profitSharingAuditService.LogDataChangeAsync(
                    operationName: "Create Tax Code",
                    tableName: "TAX_CODE",
                    auditOperation: AuditEvent.AuditOperations.Create,
                    primaryKey: $"Id:{taxCode.Id}",
                    changes:
                    [
                        new AuditChangeEntryInputRequest { ColumnName = "NAME", OriginalValue = null, NewValue = trimmedName },
                        new AuditChangeEntryInputRequest { ColumnName = "IS_AVAILABLE_DISTRIBUTION", OriginalValue = null, NewValue = request.IsAvailableForDistribution.ToString() },
                        new AuditChangeEntryInputRequest { ColumnName = "IS_AVAILABLE_FORFEITURE", OriginalValue = null, NewValue = request.IsAvailableForForfeiture.ToString() },
                        new AuditChangeEntryInputRequest { ColumnName = "ISPROTECTED", OriginalValue = null, NewValue = request.IsProtected.ToString() }
                    ],
                    cancellationToken);

                _logger.LogInformation(
                    "Created tax code {Id}: '{Name}', Distribution: {Distribution}, Forfeiture: {Forfeiture}, Protected: {Protected}",
                    taxCode.Id,
                    taxCode.Name,
                    taxCode.IsAvailableForDistribution,
                    taxCode.IsAvailableForForfeiture,
                    taxCode.IsProtected);

                return Result<TaxCodeAdminDto>.Success(new TaxCodeAdminDto
                {
                    Id = taxCode.Id,
                    Name = taxCode.Name,
                    IsAvailableForDistribution = taxCode.IsAvailableForDistribution,
                    IsAvailableForForfeiture = taxCode.IsAvailableForForfeiture,
                    IsProtected = taxCode.IsProtected
                });
            }, cancellationToken);
        }
    }

    public async Task<Result<TaxCodeAdminDto>> UpdateTaxCodeAsync(UpdateTaxCodeRequest request, CancellationToken cancellationToken)
    {
        using (_commitGuardOverride.AllowFor(roles: Role.ITDEVOPS))
        {
            return await _contextFactory.UseWritableContext(async ctx =>
            {
                var trimmedName = (request.Name ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(trimmedName))
                {
                    return Result<TaxCodeAdminDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Name)] = ["Name is required."],
                    }));
                }

                if (trimmedName.Length > 128)
                {
                    return Result<TaxCodeAdminDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Name)] = ["Name must be 128 characters or less."],
                    }));
                }

                var id = NormalizeTaxCodeId(request.Id);
                if (id is null)
                {
                    return Result<TaxCodeAdminDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Id)] = ["Id must be a single letter or number."],
                    }));
                }

                var taxCode = await ctx.TaxCodes
                    .TagWith($"Administration-UpdateTaxCode-{id}")
                    .FirstOrDefaultAsync(x => x.Id == id.Value, cancellationToken);

                if (taxCode is null)
                {
                    return Result<TaxCodeAdminDto>.Failure(_taxCodeNotFound);
                }

                if (taxCode.IsProtected)
                {
                    var isModified = taxCode.Name != trimmedName ||
                                     taxCode.IsAvailableForDistribution != request.IsAvailableForDistribution ||
                                     taxCode.IsAvailableForForfeiture != request.IsAvailableForForfeiture ||
                                     taxCode.IsProtected != request.IsProtected;

                    if (isModified)
                    {
                        return Result<TaxCodeAdminDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                        {
                            [nameof(request.IsProtected)] = ["Protected tax codes cannot be modified."],
                        }));
                    }
                }

                if (taxCode.IsProtected && !request.IsProtected)
                {
                    return Result<TaxCodeAdminDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.IsProtected)] = ["Cannot remove protected flag from TaxCode."],
                    }));
                }

                var nameExists = await ctx.TaxCodes
                    .TagWith("Administration-CheckDuplicateTaxCodeName")
                    .AnyAsync(x => x.Id != taxCode.Id && x.Name == trimmedName, cancellationToken);

                if (nameExists)
                {
                    return Result<TaxCodeAdminDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(request.Name)] = ["A tax code with this name already exists."],
                    }));
                }

                var originalName = taxCode.Name;
                var originalDistribution = taxCode.IsAvailableForDistribution;
                var originalForfeiture = taxCode.IsAvailableForForfeiture;
                var originalProtected = taxCode.IsProtected;

                if (originalName == trimmedName &&
                    originalDistribution == request.IsAvailableForDistribution &&
                    originalForfeiture == request.IsAvailableForForfeiture &&
                    originalProtected == request.IsProtected)
                {
                    return Result<TaxCodeAdminDto>.Success(new TaxCodeAdminDto
                    {
                        Id = taxCode.Id,
                        Name = taxCode.Name,
                        IsAvailableForDistribution = taxCode.IsAvailableForDistribution,
                        IsAvailableForForfeiture = taxCode.IsAvailableForForfeiture,
                        IsProtected = taxCode.IsProtected
                    });
                }

                taxCode.Name = trimmedName;
                taxCode.IsAvailableForDistribution = request.IsAvailableForDistribution;
                taxCode.IsAvailableForForfeiture = request.IsAvailableForForfeiture;
                taxCode.IsProtected = request.IsProtected;

                await ctx.SaveChangesAsync(cancellationToken);

                var changes = new List<AuditChangeEntryInputRequest>();
                if (originalName != trimmedName)
                {
                    changes.Add(new AuditChangeEntryInputRequest
                    {
                        ColumnName = "NAME",
                        OriginalValue = originalName,
                        NewValue = trimmedName
                    });
                }
                if (originalDistribution != request.IsAvailableForDistribution)
                {
                    changes.Add(new AuditChangeEntryInputRequest
                    {
                        ColumnName = "IS_AVAILABLE_DISTRIBUTION",
                        OriginalValue = originalDistribution.ToString(),
                        NewValue = request.IsAvailableForDistribution.ToString()
                    });
                }
                if (originalForfeiture != request.IsAvailableForForfeiture)
                {
                    changes.Add(new AuditChangeEntryInputRequest
                    {
                        ColumnName = "IS_AVAILABLE_FORFEITURE",
                        OriginalValue = originalForfeiture.ToString(),
                        NewValue = request.IsAvailableForForfeiture.ToString()
                    });
                }
                if (originalProtected != request.IsProtected)
                {
                    changes.Add(new AuditChangeEntryInputRequest
                    {
                        ColumnName = "ISPROTECTED",
                        OriginalValue = originalProtected.ToString(),
                        NewValue = request.IsProtected.ToString()
                    });
                }

                if (changes.Count > 0)
                {
                    await _profitSharingAuditService.LogDataChangeAsync(
                        operationName: "Update Tax Code",
                        tableName: "TAX_CODE",
                        auditOperation: AuditEvent.AuditOperations.Update,
                        primaryKey: $"Id:{taxCode.Id}",
                        changes: changes,
                        cancellationToken);
                }

                _logger.LogInformation(
                    "Updated tax code {Id}: '{OldName}' → '{NewName}', Distribution: {OldDistribution} → {NewDistribution}, Forfeiture: {OldForfeiture} → {NewForfeiture}, Protected: {OldProtected} → {NewProtected}",
                    taxCode.Id,
                    originalName,
                    trimmedName,
                    originalDistribution,
                    request.IsAvailableForDistribution,
                    originalForfeiture,
                    request.IsAvailableForForfeiture,
                    originalProtected,
                    request.IsProtected);

                return Result<TaxCodeAdminDto>.Success(new TaxCodeAdminDto
                {
                    Id = taxCode.Id,
                    Name = taxCode.Name,
                    IsAvailableForDistribution = taxCode.IsAvailableForDistribution,
                    IsAvailableForForfeiture = taxCode.IsAvailableForForfeiture,
                    IsProtected = taxCode.IsProtected
                });
            }, cancellationToken);
        }
    }

    public async Task<Result<bool>> DeleteTaxCodeAsync(char id, CancellationToken cancellationToken)
    {
        using (_commitGuardOverride.AllowFor(roles: Role.ITDEVOPS))
        {
            return await _contextFactory.UseWritableContext(async ctx =>
            {
                var taxCode = await ctx.TaxCodes
                    .TagWith($"Administration-DeleteTaxCode-{id}")
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (taxCode is null)
                {
                    return Result<bool>.Failure(_taxCodeNotFound);
                }

                if (taxCode.IsProtected)
                {
                    return Result<bool>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(taxCode.IsProtected)] = ["Protected tax codes cannot be deleted."],
                    }));
                }

                var inUse = await IsTaxCodeInUseAsync(ctx, id, cancellationToken);
                if (inUse)
                {
                    return Result<bool>.Failure(Error.Validation(new Dictionary<string, string[]>
                    {
                        [nameof(id)] = ["Tax code is currently in use and cannot be deleted."],
                    }));
                }

                var originalName = taxCode.Name;
                var originalDistribution = taxCode.IsAvailableForDistribution;
                var originalForfeiture = taxCode.IsAvailableForForfeiture;
                var originalProtected = taxCode.IsProtected;

                ctx.TaxCodes.Remove(taxCode);
                await ctx.SaveChangesAsync(cancellationToken);

                await _profitSharingAuditService.LogDataChangeAsync(
                    operationName: "Delete Tax Code",
                    tableName: "TAX_CODE",
                    auditOperation: AuditEvent.AuditOperations.Delete,
                    primaryKey: $"Id:{id}",
                    changes:
                    [
                        new AuditChangeEntryInputRequest { ColumnName = "NAME", OriginalValue = originalName, NewValue = null },
                        new AuditChangeEntryInputRequest { ColumnName = "IS_AVAILABLE_DISTRIBUTION", OriginalValue = originalDistribution.ToString(), NewValue = null },
                        new AuditChangeEntryInputRequest { ColumnName = "IS_AVAILABLE_FORFEITURE", OriginalValue = originalForfeiture.ToString(), NewValue = null },
                        new AuditChangeEntryInputRequest { ColumnName = "ISPROTECTED", OriginalValue = originalProtected.ToString(), NewValue = null }
                    ],
                    cancellationToken);

                _logger.LogInformation(
                    "Deleted tax code {Id}: '{Name}'",
                    id,
                    originalName);

                return Result<bool>.Success(true);
            }, cancellationToken);
        }
    }

    private static char? NormalizeTaxCodeId(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        var trimmed = id.Trim().ToUpperInvariant();
        if (trimmed.Length != 1)
        {
            return null;
        }

        var value = trimmed[0];
        return char.IsLetterOrDigit(value) ? value : null;
    }

    private static async Task<bool> IsTaxCodeInUseAsync(ProfitSharingDbContext ctx, char id, CancellationToken cancellationToken)
    {
        if (await ctx.Distributions.AnyAsync(d => d.TaxCodeId == id, cancellationToken))
        {
            return true;
        }

        if (await ctx.Distributions.AnyAsync(d => d.TaxCodeId == id, cancellationToken))
        {
            return true;
        }

        if (await ctx.ProfitDetails.AnyAsync(d => d.TaxCodeId == id, cancellationToken))
        {
            return true;
        }

        return await ctx.ProfitShareChecks.AnyAsync(d => d.TaxCodeId == id, cancellationToken);
    }
}
