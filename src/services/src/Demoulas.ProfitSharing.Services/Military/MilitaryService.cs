using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Military;

public class MilitaryService : IMilitaryService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;

    public MilitaryService(IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
    }

    public Task<Result<MilitaryContributionResponse>> CreateMilitaryServiceRecordAsync(
        CreateMilitaryContributionRequest req, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseWritableContext(async c =>
        {
#pragma warning disable DSMPS001
            var d = await c.Demographics.FirstOrDefaultAsync(d => d.BadgeNumber == req.BadgeNumber,
#pragma warning restore DSMPS001
                cancellationToken);

            if (d == null)
            {
                return Result<MilitaryContributionResponse>.Failure(Error.EmployeeNotFound);
            }

            var pd = new ProfitDetail
            {
                ProfitCodeId = /* 0 */ProfitCode.Constants.IncomingContributions,
                ProfitYear = req.ProfitYear,
                ProfitYearIteration = 1,
                CommentTypeId = /* 19 */CommentType.Constants.Military.Id,
                Remark = /* 19 */CommentType.Constants.Military.Name,
                Contribution = req.ContributionAmount,
                Ssn = d.Ssn,
                YearsOfServiceCredit = (byte)(req.IsSupplementalContribution ? 0 : 1),
                MonthToDate = 12,
                YearToDate = (short)req.ContributionDate.Year
            };
            c.ProfitDetails.Add(pd);

            await c.SaveChangesAsync(cancellationToken);

            return Result<MilitaryContributionResponse>.Success(new MilitaryContributionResponse
            {
                BadgeNumber = req.BadgeNumber,
                CommentTypeId = /* 19 */CommentType.Constants.Military,
                ProfitYear = req.ProfitYear,
                ContributionDate = new DateOnly(pd.YearToDate, pd.MonthToDate, 31),
                Amount = pd.Contribution,
                IsSupplementalContribution = pd.YearsOfServiceCredit == 0
            });
        }, cancellationToken);
    }

    public async Task<Result<PaginatedResponseDto<MilitaryContributionResponse>>> GetMilitaryServiceRecordAsync(GetMilitaryContributionRequest req, bool isArchiveRequest,
        CancellationToken cancellationToken = default)
    {
        #region Validation

        var validator = new InlineValidator<GetMilitaryContributionRequest>();

        if (!isArchiveRequest)
        {
            validator.RuleFor(r => r.BadgeNumber)
                .GreaterThan(0)
                .WithMessage($"{nameof(GetMilitaryContributionRequest.BadgeNumber)} must be greater than zero.");
        }

    
        var validationResult = await validator.ValidateAsync(req, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result<PaginatedResponseDto<MilitaryContributionResponse>>.ValidationFailure(errors);
        }

        #endregion

        var result = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
            // We grab all Military Contributions for all time
            var query = ctx.ProfitDetails
                .Include(pd => pd.CommentType)
                .Join(demographics,
                    c => c.Ssn,
                    cm => cm.Ssn,
                    (pd, d) => new { pd, d })
                .Where(x => x.pd.CommentTypeId == CommentType.Constants.Military.Id)
                .OrderByDescending(x => x.pd.ProfitYear)
                .ThenByDescending(x => x.pd.CreatedAtUtc)
                .Select(x => new MilitaryContributionResponse
                {
                    BadgeNumber = x.d.BadgeNumber,
                    ProfitYear = x.pd.ProfitYear,
                    CommentTypeId = x.pd.CommentTypeId,
                    ContributionDate = new DateOnly(x.pd.YearToDate == 0 ? x.pd.ProfitYear : x.pd.YearToDate, x.pd.MonthToDate == 0 ? 12 : x.pd.MonthToDate, 31),
                    Amount = x.pd.Contribution,
                    IsSupplementalContribution = x.pd.YearsOfServiceCredit == 0,
                    IsExecutive = x.d.PayFrequencyId == Demoulas.ProfitSharing.Data.Entities.PayFrequency.Constants.Monthly
                });

            if (!isArchiveRequest)
            {
                query = query.Where(x => x.BadgeNumber == req.BadgeNumber);
            }

            return await query.ToPaginationResultsAsync(req, cancellationToken);
        });

        return Result<PaginatedResponseDto<MilitaryContributionResponse>>.Success(result);
    }
}
