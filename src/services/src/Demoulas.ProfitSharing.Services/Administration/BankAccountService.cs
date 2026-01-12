using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Administration;

public sealed class BankAccountService : IBankAccountService
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly ILogger<BankAccountService> _logger;

    public BankAccountService(
        IProfitSharingDataContextFactory contextFactory,
        ILogger<BankAccountService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public Task<Result<IReadOnlyList<BankAccountDto>>> GetByBankIdAsync(
        int bankId,
        bool includeDisabled = false,
        CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var query = ctx.BankAccounts
                .TagWith($"Administration-GetBankAccountsByBankId-{bankId}-IncludeDisabled:{includeDisabled}")
                .Include(ba => ba.Bank)
                .Where(ba => ba.BankId == bankId);

            if (!includeDisabled)
            {
                query = query.Where(ba => !ba.IsDisabled);
            }

            var accounts = await query
                .OrderByDescending(ba => ba.IsPrimary)
                .ThenBy(ba => ba.RoutingNumber)
                .Select(ba => new BankAccountDto
                {
                    Id = ba.Id,
                    BankId = ba.BankId,
                    BankName = ba.Bank!.Name,
                    RoutingNumber = ba.RoutingNumber,
                    AccountNumber = ba.AccountNumber.MaskAccountNumber(),
                    IsPrimary = ba.IsPrimary,
                    IsDisabled = ba.IsDisabled,
                    ServicingFedRoutingNumber = ba.ServicingFedRoutingNumber,
                    ServicingFedAddress = ba.ServicingFedAddress,
                    FedwireTelegraphicName = ba.FedwireTelegraphicName,
                    FedwireLocation = ba.FedwireLocation,
                    FedAchChangeDate = ba.FedAchChangeDate,
                    FedwireRevisionDate = ba.FedwireRevisionDate,
                    Notes = ba.Notes,
                    EffectiveDate = ba.EffectiveDate,
                    DiscontinuedDate = ba.DiscontinuedDate,
                    CreatedAtUtc = ba.CreatedAtUtc,
                    CreatedBy = ba.CreatedBy ?? string.Empty,
                    ModifiedAtUtc = ba.ModifiedAtUtc,
                    ModifiedBy = ba.ModifiedBy
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} bank accounts for bank {BankId} (IncludeDisabled: {IncludeDisabled})",
                accounts.Count, bankId, includeDisabled);

            return Result<IReadOnlyList<BankAccountDto>>.Success(accounts);
        }, cancellationToken);
    }

    public Task<Result<BankAccountDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var account = await ctx.BankAccounts
                .TagWith($"Administration-GetBankAccountById-{id}")
                .Include(ba => ba.Bank)
                .Select(ba => new BankAccountDto
                {
                    Id = ba.Id,
                    BankId = ba.BankId,
                    BankName = ba.Bank!.Name,
                    RoutingNumber = ba.RoutingNumber,
                    AccountNumber = ba.AccountNumber.MaskAccountNumber(),
                    IsPrimary = ba.IsPrimary,
                    IsDisabled = ba.IsDisabled,
                    ServicingFedRoutingNumber = ba.ServicingFedRoutingNumber,
                    ServicingFedAddress = ba.ServicingFedAddress,
                    FedwireTelegraphicName = ba.FedwireTelegraphicName,
                    FedwireLocation = ba.FedwireLocation,
                    FedAchChangeDate = ba.FedAchChangeDate,
                    FedwireRevisionDate = ba.FedwireRevisionDate,
                    Notes = ba.Notes,
                    EffectiveDate = ba.EffectiveDate,
                    DiscontinuedDate = ba.DiscontinuedDate,
                    CreatedAtUtc = ba.CreatedAtUtc,
                    CreatedBy = ba.CreatedBy ?? string.Empty,
                    ModifiedAtUtc = ba.ModifiedAtUtc,
                    ModifiedBy = ba.ModifiedBy
                })
                .FirstOrDefaultAsync(ba => ba.Id == id, cancellationToken);

            if (account is null)
            {
                return Result<BankAccountDto>.Failure(Error.BankAccountNotFound);
            }

            return Result<BankAccountDto>.Success(account);
        }, cancellationToken);
    }

    public Task<Result<BankAccountDto>> GetPrimaryAccountAsync(int bankId, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var account = await ctx.BankAccounts
                .TagWith($"Administration-GetPrimaryBankAccount-{bankId}")
                .Include(ba => ba.Bank)
                .Where(ba => ba.BankId == bankId && ba.IsPrimary && !ba.IsDisabled)
                .Select(ba => new BankAccountDto
                {
                    Id = ba.Id,
                    BankId = ba.BankId,
                    BankName = ba.Bank!.Name,
                    RoutingNumber = ba.RoutingNumber,
                    AccountNumber = ba.AccountNumber.MaskAccountNumber(),
                    IsPrimary = ba.IsPrimary,
                    IsDisabled = ba.IsDisabled,
                    ServicingFedRoutingNumber = ba.ServicingFedRoutingNumber,
                    ServicingFedAddress = ba.ServicingFedAddress,
                    FedwireTelegraphicName = ba.FedwireTelegraphicName,
                    FedwireLocation = ba.FedwireLocation,
                    FedAchChangeDate = ba.FedAchChangeDate,
                    FedwireRevisionDate = ba.FedwireRevisionDate,
                    Notes = ba.Notes,
                    EffectiveDate = ba.EffectiveDate,
                    DiscontinuedDate = ba.DiscontinuedDate,
                    CreatedAtUtc = ba.CreatedAtUtc,
                    CreatedBy = ba.CreatedBy ?? string.Empty,
                    ModifiedAtUtc = ba.ModifiedAtUtc,
                    ModifiedBy = ba.ModifiedBy
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (account is null)
            {
                return Result<BankAccountDto>.Failure(Error.NoPrimaryAccountExists);
            }

            return Result<BankAccountDto>.Success(account);
        }, cancellationToken);
    }

    public Task<Result<BankAccountDto>> GetByRoutingNumberAsync(string routingNumber, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var account = await ctx.BankAccounts
                .TagWith($"Administration-GetBankAccountByRoutingNumber")
                .Include(ba => ba.Bank)
                .Where(ba => ba.RoutingNumber == routingNumber && !ba.IsDisabled)
                .Select(ba => new BankAccountDto
                {
                    Id = ba.Id,
                    BankId = ba.BankId,
                    BankName = ba.Bank!.Name,
                    RoutingNumber = ba.RoutingNumber,
                    AccountNumber = ba.AccountNumber.MaskAccountNumber(),
                    IsPrimary = ba.IsPrimary,
                    IsDisabled = ba.IsDisabled,
                    ServicingFedRoutingNumber = ba.ServicingFedRoutingNumber,
                    ServicingFedAddress = ba.ServicingFedAddress,
                    FedwireTelegraphicName = ba.FedwireTelegraphicName,
                    FedwireLocation = ba.FedwireLocation,
                    FedAchChangeDate = ba.FedAchChangeDate,
                    FedwireRevisionDate = ba.FedwireRevisionDate,
                    Notes = ba.Notes,
                    EffectiveDate = ba.EffectiveDate,
                    DiscontinuedDate = ba.DiscontinuedDate,
                    CreatedAtUtc = ba.CreatedAtUtc,
                    CreatedBy = ba.CreatedBy ?? string.Empty,
                    ModifiedAtUtc = ba.ModifiedAtUtc,
                    ModifiedBy = ba.ModifiedBy
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (account is null)
            {
                return Result<BankAccountDto>.Failure(Error.BankAccountNotFound);
            }

            return Result<BankAccountDto>.Success(account);
        }, cancellationToken);
    }

    public Task<Result<BankAccountDto>> CreateAsync(
        int bankId,
        string routingNumber,
        string accountNumber,
        bool isPrimary,
        string? servicingFedRoutingNumber,
        string? servicingFedAddress,
        string? fedwireTelegraphicName,
        string? fedwireLocation,
        DateOnly? fedAchChangeDate,
        DateOnly? fedwireRevisionDate,
        string? notes,
        DateOnly? effectiveDate,
        IAuditService auditService,
        IAppUser appUser,
        CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseWritableContext(async ctx =>
        {
            // Verify bank exists
            var bankExists = await ctx.Banks
                .TagWith($"Administration-VerifyBankExists-{bankId}")
                .AnyAsync(b => b.Id == bankId, cancellationToken);

            if (!bankExists)
            {
                return Result<BankAccountDto>.Failure(Error.BankNotFound);
            }

            // If setting as primary, check no other primary exists
            if (isPrimary)
            {
                var existingPrimary = await ctx.BankAccounts
                    .TagWith($"Administration-CheckExistingPrimaryAccount-{bankId}")
                    .AnyAsync(ba => ba.BankId == bankId && ba.IsPrimary && !ba.IsDisabled, cancellationToken);

                if (existingPrimary)
                {
                    return Result<BankAccountDto>.Failure(Error.MultiplePrimaryAccountsNotAllowed);
                }
            }

            var now = DateTimeOffset.UtcNow;
            var userName = appUser.UserName ?? "SYSTEM";
            
            var bankAccount = new BankAccount
            {
                BankId = bankId,
                RoutingNumber = routingNumber,
                AccountNumber = accountNumber,
                IsPrimary = isPrimary,
                IsDisabled = false,
                ServicingFedRoutingNumber = servicingFedRoutingNumber,
                ServicingFedAddress = servicingFedAddress,
                FedwireTelegraphicName = fedwireTelegraphicName,
                FedwireLocation = fedwireLocation,
                FedAchChangeDate = fedAchChangeDate,
                FedwireRevisionDate = fedwireRevisionDate,
                Notes = notes,
                EffectiveDate = effectiveDate,
                CreatedAtUtc = now,
                CreatedBy = userName,
                ModifiedAtUtc = now,
                ModifiedBy = userName
            };

            ctx.BankAccounts.Add(bankAccount);
            await ctx.SaveChangesAsync(cancellationToken);

            // Reload with bank for DTO
            var bank = await ctx.Banks
                .TagWith($"Administration-GetBankForNewAccount-{bankId}")
                .FirstAsync(b => b.Id == bankId, cancellationToken);

            // Audit log creation
            await auditService.LogDataChangeAsync(
                operationName: "Create Bank Account",
                tableName: "BANK_ACCOUNT",
                auditOperation: AuditEvent.AuditOperations.Create,
                primaryKey: $"Id:{bankAccount.Id}",
                changes:
                [
                    new AuditChangeEntryInput
                    {
                        ColumnName = "BANK_ID",
                        OriginalValue = null,
                        NewValue = bankId.ToString(),
                    },
                    new AuditChangeEntryInput
                    {
                        ColumnName = "ROUTING_NUMBER",
                        OriginalValue = null,
                        NewValue = routingNumber,
                    },
                    new AuditChangeEntryInput
                    {
                        ColumnName = "ACCOUNT_NUMBER",
                        OriginalValue = null,
                        NewValue = accountNumber.MaskAccountNumber(),
                    },
                    new AuditChangeEntryInput
                    {
                        ColumnName = "IS_PRIMARY",
                        OriginalValue = null,
                        NewValue = isPrimary.ToString(),
                    }
                ],
                cancellationToken);

            _logger.LogInformation("Created bank account {AccountId} for bank {BankId}, routing {Routing}, IsPrimary: {IsPrimary} by {User}",
                bankAccount.Id, bankId, routingNumber, isPrimary, userName);

            return Result<BankAccountDto>.Success(new BankAccountDto
            {
                Id = bankAccount.Id,
                BankId = bankAccount.BankId,
                BankName = bank.Name,
                RoutingNumber = bankAccount.RoutingNumber,
                AccountNumber = bankAccount.AccountNumber.MaskAccountNumber(),
                IsPrimary = bankAccount.IsPrimary,
                IsDisabled = bankAccount.IsDisabled,
                ServicingFedRoutingNumber = bankAccount.ServicingFedRoutingNumber,
                ServicingFedAddress = bankAccount.ServicingFedAddress,
                FedwireTelegraphicName = bankAccount.FedwireTelegraphicName,
                FedwireLocation = bankAccount.FedwireLocation,
                FedAchChangeDate = bankAccount.FedAchChangeDate,
                FedwireRevisionDate = bankAccount.FedwireRevisionDate,
                Notes = bankAccount.Notes,
                EffectiveDate = bankAccount.EffectiveDate,
                DiscontinuedDate = bankAccount.DiscontinuedDate,
                CreatedAtUtc = bankAccount.CreatedAtUtc,
                CreatedBy = bankAccount.CreatedBy,
                ModifiedAtUtc = bankAccount.ModifiedAtUtc,
                ModifiedBy = bankAccount.ModifiedBy
            });
        }, cancellationToken);
    }

    public Task<Result<BankAccountDto>> UpdateAsync(
        int id,
        int bankId,
        string routingNumber,
        string accountNumber,
        bool isPrimary,
        bool isDisabled,
        string? servicingFedRoutingNumber,
        string? servicingFedAddress,
        string? fedwireTelegraphicName,
        string? fedwireLocation,
        DateOnly? fedAchChangeDate,
        DateOnly? fedwireRevisionDate,
        string? notes,
        DateOnly? effectiveDate,
        DateOnly? discontinuedDate,
        IAuditService auditService,
        IAppUser appUser,
        CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseWritableContext(async ctx =>
        {
            var userName = appUser.UserName ?? "SYSTEM";
            
            var bankAccount = await ctx.BankAccounts
                .TagWith($"Administration-UpdateBankAccount-{id}")
                .Include(ba => ba.Bank)
                .FirstOrDefaultAsync(ba => ba.Id == id, cancellationToken);

            if (bankAccount is null)
            {
                return Result<BankAccountDto>.Failure(Error.BankAccountNotFound);
            }

            // Cannot disable primary account
            if (isDisabled && bankAccount.IsPrimary)
            {
                return Result<BankAccountDto>.Failure(Error.CannotDisablePrimaryAccount);
            }

            // If setting as primary, unset other primary accounts for this bank
            if (isPrimary && !bankAccount.IsPrimary)
            {
                await ctx.BankAccounts
                    .TagWith($"Administration-UnsetOtherPrimaryAccounts-{bankId}")
                    .Where(ba => ba.BankId == bankId && ba.IsPrimary && ba.Id != id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(ba => ba.IsPrimary, false)
                        .SetProperty(ba => ba.ModifiedAtUtc, DateTimeOffset.UtcNow)
                        .SetProperty(ba => ba.ModifiedBy, userName),
                        cancellationToken);
            }

            bankAccount.BankId = bankId;
            bankAccount.RoutingNumber = routingNumber;
            bankAccount.AccountNumber = accountNumber;
            bankAccount.IsPrimary = isPrimary;
            bankAccount.IsDisabled = isDisabled;
            bankAccount.ServicingFedRoutingNumber = servicingFedRoutingNumber;
            bankAccount.ServicingFedAddress = servicingFedAddress;
            bankAccount.FedwireTelegraphicName = fedwireTelegraphicName;
            bankAccount.FedwireLocation = fedwireLocation;
            bankAccount.FedAchChangeDate = fedAchChangeDate;
            bankAccount.FedwireRevisionDate = fedwireRevisionDate;
            bankAccount.Notes = notes;
            bankAccount.EffectiveDate = effectiveDate;
            bankAccount.DiscontinuedDate = discontinuedDate;
            bankAccount.ModifiedAtUtc = DateTimeOffset.UtcNow;
            bankAccount.ModifiedBy = userName;

            await ctx.SaveChangesAsync(cancellationToken);

            // Audit log update
            await auditService.LogDataChangeAsync(
                operationName: "Update Bank Account",
                tableName: "BANK_ACCOUNT",
                auditOperation: AuditEvent.AuditOperations.Update,
                primaryKey: $"Id:{bankAccount.Id}",
                changes:
                [
                    new AuditChangeEntryInput
                    {
                        ColumnName = "ROUTING_NUMBER",
                        OriginalValue = null, // Would need original values
                        NewValue = routingNumber,
                    },
                    new AuditChangeEntryInput
                    {
                        ColumnName = "IS_PRIMARY",
                        OriginalValue = null,
                        NewValue = isPrimary.ToString(),
                    },
                    new AuditChangeEntryInput
                    {
                        ColumnName = "IS_DISABLED",
                        OriginalValue = null,
                        NewValue = isDisabled.ToString(),
                    }
                ],
                cancellationToken);

            _logger.LogInformation("Updated bank account {AccountId}, routing {Routing}, IsPrimary: {IsPrimary}, IsDisabled: {IsDisabled} by {User}",
                bankAccount.Id, routingNumber, isPrimary, isDisabled, userName);

            return Result<BankAccountDto>.Success(new BankAccountDto
            {
                Id = bankAccount.Id,
                BankId = bankAccount.BankId,
                BankName = bankAccount.Bank!.Name,
                RoutingNumber = bankAccount.RoutingNumber,
                AccountNumber = bankAccount.AccountNumber.MaskAccountNumber(),
                IsPrimary = bankAccount.IsPrimary,
                IsDisabled = bankAccount.IsDisabled,
                ServicingFedRoutingNumber = bankAccount.ServicingFedRoutingNumber,
                ServicingFedAddress = bankAccount.ServicingFedAddress,
                FedwireTelegraphicName = bankAccount.FedwireTelegraphicName,
                FedwireLocation = bankAccount.FedwireLocation,
                FedAchChangeDate = bankAccount.FedAchChangeDate,
                FedwireRevisionDate = bankAccount.FedwireRevisionDate,
                Notes = bankAccount.Notes,
                EffectiveDate = bankAccount.EffectiveDate,
                DiscontinuedDate = bankAccount.DiscontinuedDate,
                CreatedAtUtc = bankAccount.CreatedAtUtc,
                CreatedBy = bankAccount.CreatedBy ?? string.Empty,
                ModifiedAtUtc = bankAccount.ModifiedAtUtc,
                ModifiedBy = bankAccount.ModifiedBy
            });
        }, cancellationToken);
    }

    public Task<Result<bool>> SetPrimaryAsync(int id, IAuditService auditService, IAppUser appUser, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseWritableContext(async ctx =>
        {
            var userName = appUser.UserName ?? "SYSTEM";
            
            var bankAccount = await ctx.BankAccounts
                .TagWith($"Administration-SetPrimaryAccount-{id}")
                .FirstOrDefaultAsync(ba => ba.Id == id, cancellationToken);

            if (bankAccount is null)
            {
                return Result<bool>.Failure(Error.BankAccountNotFound);
            }

            // Unset other primary accounts for this bank
            await ctx.BankAccounts
                .TagWith($"Administration-UnsetOtherPrimaryAccounts-{bankAccount.BankId}")
                .Where(ba => ba.BankId == bankAccount.BankId && ba.IsPrimary && ba.Id != id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(ba => ba.IsPrimary, false)
                    .SetProperty(ba => ba.ModifiedAtUtc, DateTimeOffset.UtcNow)
                    .SetProperty(ba => ba.ModifiedBy, userName),
                    cancellationToken);

            bankAccount.IsPrimary = true;
            bankAccount.ModifiedAtUtc = DateTimeOffset.UtcNow;
            bankAccount.ModifiedBy = userName;

            await ctx.SaveChangesAsync(cancellationToken);

            // Audit log set primary
            await auditService.LogDataChangeAsync(
                operationName: "Set Primary Bank Account",
                tableName: "BANK_ACCOUNT",
                auditOperation: AuditEvent.AuditOperations.Update,
                primaryKey: $"Id:{bankAccount.Id}",
                changes:
                [
                    new AuditChangeEntryInput
                    {
                        ColumnName = "IS_PRIMARY",
                        OriginalValue = "False",
                        NewValue = "True",
                    }
                ],
                cancellationToken);

            _logger.LogInformation("Set bank account {AccountId} as primary for bank {BankId} by {User}",
                bankAccount.Id, bankAccount.BankId, userName);

            return Result<bool>.Success(true);
        }, cancellationToken);
    }

    public Task<Result<bool>> DisableAsync(int id, IAuditService auditService, IAppUser appUser, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseWritableContext(async ctx =>
        {
            var userName = appUser.UserName ?? "SYSTEM";
            
            var bankAccount = await ctx.BankAccounts
                .TagWith($"Administration-DisableBankAccount-{id}")
                .FirstOrDefaultAsync(ba => ba.Id == id, cancellationToken);

            if (bankAccount is null)
            {
                return Result<bool>.Failure(Error.BankAccountNotFound);
            }

            // Cannot disable primary account
            if (bankAccount.IsPrimary)
            {
                return Result<bool>.Failure(Error.CannotDisablePrimaryAccount);
            }

            bankAccount.IsDisabled = true;
            bankAccount.ModifiedAtUtc = DateTimeOffset.UtcNow;
            bankAccount.ModifiedBy = userName;

            await ctx.SaveChangesAsync(cancellationToken);

            // Audit log disable
            await auditService.LogDataChangeAsync(
                operationName: "Disable Bank Account",
                tableName: "BANK_ACCOUNT",
                auditOperation: AuditEvent.AuditOperations.Update,
                primaryKey: $"Id:{bankAccount.Id}",
                changes:
                [
                    new AuditChangeEntryInput
                    {
                        ColumnName = "IS_DISABLED",
                        OriginalValue = "False",
                        NewValue = "True",
                    }
                ],
                cancellationToken);

            _logger.LogInformation("Disabled bank account {AccountId} for bank {BankId} by {User}",
                bankAccount.Id, bankAccount.BankId, userName);

            return Result<bool>.Success(true);
        }, cancellationToken);
    }

    // Interface implementation methods (delegate to existing methods)
    Task<Result<BankAccountDto>> IBankAccountService.CreateAsync(CreateBankAccountRequest request, IAuditService auditService, IAppUser appUser, CancellationToken cancellationToken)
        => CreateAsync(request.BankId, request.RoutingNumber, request.AccountNumber, request.IsPrimary,
            request.ServicingFedRoutingNumber, request.ServicingFedAddress, request.FedwireTelegraphicName,
            request.FedwireLocation, request.FedAchChangeDate, request.FedwireRevisionDate,
            request.Notes, request.EffectiveDate, auditService, appUser, cancellationToken);

    Task<Result<BankAccountDto>> IBankAccountService.UpdateAsync(UpdateBankAccountRequest request, IAuditService auditService, IAppUser appUser, CancellationToken cancellationToken)
        => UpdateAsync(request.Id, request.BankId, request.RoutingNumber, request.AccountNumber, request.IsPrimary, false,
            request.ServicingFedRoutingNumber, request.ServicingFedAddress, request.FedwireTelegraphicName,
            request.FedwireLocation, request.FedAchChangeDate, request.FedwireRevisionDate,
            request.Notes, request.EffectiveDate, request.DiscontinuedDate, auditService, appUser, cancellationToken);

    Task<Result<bool>> IBankAccountService.DisableAsync(int id, IAuditService auditService, IAppUser appUser, CancellationToken cancellationToken)
        => DisableAsync(id, auditService, appUser, cancellationToken);
}
