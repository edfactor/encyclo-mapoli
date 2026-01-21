using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

/// <summary>
/// Service for managing RMD (Required Minimum Distribution) factor data.
/// </summary>
public sealed class RmdsFactorService : IRmdsFactorService
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly ILogger<RmdsFactorService> _logger;

    public RmdsFactorService(
        IProfitSharingDataContextFactory contextFactory,
        ILogger<RmdsFactorService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public Task<List<RmdsFactorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var rmdData = await ctx.RmdsFactorsByAge
                .TagWith("GetAllRmdsFactors")
                .OrderBy(r => r.Age)
                .ToListAsync(cancellationToken);

            return rmdData.Select(r => new RmdsFactorDto
            {
                Age = r.Age,
                Factor = r.Factor
            }).ToList();
        }, cancellationToken);
    }

    public Task<RmdsFactorDto?> GetByAgeAsync(byte age, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var rmdData = await ctx.RmdsFactorsByAge
                .TagWith($"GetRmdsFactorByAge-{age}")
                .FirstOrDefaultAsync(r => r.Age == age, cancellationToken);

            if (rmdData is null)
            {
                return null;
            }

            return new RmdsFactorDto
            {
                Age = rmdData.Age,
                Factor = rmdData.Factor
            };
        }, cancellationToken);
    }

    public Task<RmdsFactorDto> UpsertAsync(RmdsFactorRequest request, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseWritableContext(async ctx =>
        {
            // Check if record exists
            var existing = await ctx.RmdsFactorsByAge
                .FirstOrDefaultAsync(r => r.Age == request.Age, cancellationToken);

            if (existing is not null)
            {
                // Update existing
                _logger.LogInformation(
                    "Updating RMD factor for age {Age}: Factor changing from {OldFactor} to {NewFactor}",
                    request.Age, existing.Factor, request.Factor);

                existing.Factor = request.Factor;
            }
            else
            {
                // Add new
                _logger.LogInformation(
                    "Adding new RMD factor for age {Age} with Factor {Factor}",
                    request.Age, request.Factor);

                var newEntity = new RmdsFactorByAge
                {
                    Age = request.Age,
                    Factor = request.Factor
                };

                ctx.RmdsFactorsByAge.Add(newEntity);
            }

            await ctx.SaveChangesAsync(cancellationToken);

            return new RmdsFactorDto
            {
                Age = request.Age,
                Factor = request.Factor
            };
        }, cancellationToken);
    }

    public Task<bool> DeleteAsync(byte age, CancellationToken cancellationToken = default)
    {
        return _contextFactory.UseWritableContext(async ctx =>
        {
            var entity = await ctx.RmdsFactorsByAge
                .FirstOrDefaultAsync(r => r.Age == age, cancellationToken);

            if (entity is null)
            {
                return false;
            }

            _logger.LogWarning(
                "Deleting RMD factor for age {Age} with Factor {Factor}",
                entity.Age, entity.Factor);

            ctx.RmdsFactorsByAge.Remove(entity);
            await ctx.SaveChangesAsync(cancellationToken);

            return true;
        }, cancellationToken);
    }
}
