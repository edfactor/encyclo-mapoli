using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Services.Administration;

public sealed class BankService : IBankService
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly ILogger<BankService> _logger;

    public BankService(
        IProfitSharingDataContextFactory contextFactory,
        ILogger<BankService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public Task<Result<IReadOnlyList<BankDto>>> GetAllAsync(bool includeDisabled = false, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var query = ctx.Banks
                .TagWith($"Administration-GetAllBanks-IncludeDisabled:{includeDisabled}")
                .AsQueryable();

            if (!includeDisabled)
            {
                query = query.Where(b => !b.IsDisabled);
            }

            var banks = await query
                .OrderBy(b => b.Name)
                .Select(b => new BankDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    OfficeType = b.OfficeType,
                    City = b.City,
                    State = b.State,
                    Phone = b.Phone,
                    Status = b.Status,
                    IsDisabled = b.IsDisabled,
                    AccountCount = b.Accounts.Count(a => !a.IsDisabled),
                    CreatedAtUtc = b.CreatedAtUtc,
                    CreatedBy = b.CreatedBy ?? string.Empty,
                    ModifiedAtUtc = b.ModifiedAtUtc,
                    ModifiedBy = b.ModifiedBy
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} banks (IncludeDisabled: {IncludeDisabled})",
                banks.Count, includeDisabled);

            return Result<IReadOnlyList<BankDto>>.Success(banks);
        }, cancellationToken);
    }

    public Task<Result<BankDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var bank = await ctx.Banks
                .TagWith($"Administration-GetBankById-{id}")
                .Select(b => new BankDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    OfficeType = b.OfficeType,
                    City = b.City,
                    State = b.State,
                    Phone = b.Phone,
                    Status = b.Status,
                    IsDisabled = b.IsDisabled,
                    AccountCount = b.Accounts.Count(a => !a.IsDisabled),
                    CreatedAtUtc = b.CreatedAtUtc,
                    CreatedBy = b.CreatedBy ?? string.Empty,
                    ModifiedAtUtc = b.ModifiedAtUtc,
                    ModifiedBy = b.ModifiedBy
                })
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

            if (bank is null)
            {
                return Result<BankDto>.Failure(Error.BankNotFound);
            }

            return Result<BankDto>.Success(bank);
        }, cancellationToken);
    }

    public Task<Result<BankDto>> CreateAsync(
        string name,
        string? officeType,
        string? city,
        string? state,
        string? phone,
        string? status,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseWritableContext(async ctx =>
        {
            // Check for duplicate name
            var exists = await ctx.Banks
                .TagWith($"Administration-CheckDuplicateBankName-{name}")
                .AnyAsync(b => b.Name == name, cancellationToken);

            if (exists)
            {
                return Result<BankDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                {
                    [nameof(name)] = ["A bank with this name already exists."]
                }));
            }

            var now = DateTimeOffset.UtcNow;
            var bank = new Bank
            {
                Name = name,
                OfficeType = officeType,
                City = city,
                State = state,
                Phone = phone,
                Status = status,
                IsDisabled = false,
                CreatedAtUtc = now,
                CreatedBy = createdBy,
                ModifiedAtUtc = now,
                ModifiedBy = createdBy
            };

            ctx.Banks.Add(bank);
            await ctx.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created bank {BankId}: '{Name}' by {User}",
                bank.Id, bank.Name, createdBy);

            return Result<BankDto>.Success(new BankDto
            {
                Id = bank.Id,
                Name = bank.Name,
                OfficeType = bank.OfficeType,
                City = bank.City,
                State = bank.State,
                Phone = bank.Phone,
                Status = bank.Status,
                IsDisabled = bank.IsDisabled,
                AccountCount = 0,
                CreatedAtUtc = bank.CreatedAtUtc,
                CreatedBy = bank.CreatedBy,
                ModifiedAtUtc = bank.ModifiedAtUtc,
                ModifiedBy = bank.ModifiedBy
            });
        }, cancellationToken);
    }

    public Task<Result<BankDto>> UpdateAsync(
        int id,
        string name,
        string? officeType,
        string? city,
        string? state,
        string? phone,
        string? status,
        bool isDisabled,
        string modifiedBy,
        CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseWritableContext(async ctx =>
        {
            var bank = await ctx.Banks
                .TagWith($"Administration-UpdateBank-{id}")
                .Include(b => b.Accounts)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

            if (bank is null)
            {
                return Result<BankDto>.Failure(Error.BankNotFound);
            }

            // Check for duplicate name (excluding current bank)
            var duplicateExists = await ctx.Banks
                .TagWith($"Administration-CheckDuplicateBankName-Update-{name}")
                .AnyAsync(b => b.Name == name && b.Id != id, cancellationToken);

            if (duplicateExists)
            {
                return Result<BankDto>.Failure(Error.Validation(new Dictionary<string, string[]>
                {
                    [nameof(name)] = ["A bank with this name already exists."]
                }));
            }

            bank.Name = name;
            bank.OfficeType = officeType;
            bank.City = city;
            bank.State = state;
            bank.Phone = phone;
            bank.Status = status;
            bank.IsDisabled = isDisabled;
            bank.ModifiedAtUtc = DateTimeOffset.UtcNow;
            bank.ModifiedBy = modifiedBy;

            await ctx.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated bank {BankId}: '{Name}', IsDisabled: {IsDisabled} by {User}",
                bank.Id, bank.Name, bank.IsDisabled, modifiedBy);

            return Result<BankDto>.Success(new BankDto
            {
                Id = bank.Id,
                Name = bank.Name,
                OfficeType = bank.OfficeType,
                City = bank.City,
                State = bank.State,
                Phone = bank.Phone,
                Status = bank.Status,
                IsDisabled = bank.IsDisabled,
                AccountCount = bank.Accounts.Count(a => !a.IsDisabled),
                CreatedAtUtc = bank.CreatedAtUtc,
                CreatedBy = bank.CreatedBy ?? string.Empty,
                ModifiedAtUtc = bank.ModifiedAtUtc,
                ModifiedBy = bank.ModifiedBy
            });
        }, cancellationToken);
    }

    public Task<Result<bool>> DisableAsync(int id, string modifiedBy, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseWritableContext(async ctx =>
        {
            var bank = await ctx.Banks
                .TagWith($"Administration-DisableBank-{id}")
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

            if (bank is null)
            {
                return Result<bool>.Failure(Error.BankNotFound);
            }

            bank.IsDisabled = true;
            bank.ModifiedAtUtc = DateTimeOffset.UtcNow;
            bank.ModifiedBy = modifiedBy;

            await ctx.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Disabled bank {BankId}: '{Name}' by {User}",
                bank.Id, bank.Name, modifiedBy);

            return Result<bool>.Success(true);
        }, cancellationToken);
    }

    // Interface implementation methods (delegate to existing methods)
    Task<Result<BankDto>> IBankService.CreateAsync(CreateBankRequest request, CancellationToken cancellationToken)
        => CreateAsync(request.Name, request.OfficeType, request.City, request.State, request.Phone, request.Status, "System", cancellationToken);

    Task<Result<BankDto>> IBankService.UpdateAsync(UpdateBankRequest request, CancellationToken cancellationToken)
        => UpdateAsync(request.Id, request.Name, request.OfficeType, request.City, request.State, request.Phone, request.Status, false, "System", cancellationToken);

    Task<Result<bool>> IBankService.DisableAsync(int id, CancellationToken cancellationToken)
        => DisableAsync(id, "System", cancellationToken);
}
