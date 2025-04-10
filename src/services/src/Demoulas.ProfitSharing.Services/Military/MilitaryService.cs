using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace Demoulas.ProfitSharing.Services.Military;

public class MilitaryService : IMilitaryService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public MilitaryService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    public Task<Result<MasterInquiryResponseDto>> CreateMilitaryServiceRecordAsync(
        CreateMilitaryContributionRequest req, CancellationToken cancellationToken = default)
    {
        #region Validation
        var validator = new InlineValidator<CreateMilitaryContributionRequest>();

        validator.RuleFor(r => r.ContributionAmount)
            .GreaterThan(0)
            .WithMessage($"The {nameof(CreateMilitaryContributionRequest.ContributionAmount)} must be greater than zero.");

        validator.RuleFor(r => r.ProfitYear)
            .GreaterThanOrEqualTo((short)2000)
            .WithMessage($"{nameof(MilitaryContributionRequest.ProfitYear)} must not less than 2000.")
            .LessThanOrEqualTo((short)DateTime.Today.Year)
            .WithMessage($"{nameof(MilitaryContributionRequest.ProfitYear)} must not be greater than this year.");

        validator.RuleFor(r => r.BadgeNumber)
            .GreaterThan(0)
            .WithMessage($"{nameof(CreateMilitaryContributionRequest.BadgeNumber)} must be greater than zero.");

        var validationResult = validator.Validate(req);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Task.FromResult(Result<MasterInquiryResponseDto>.ValidationFailure(errors));
        }
        #endregion
        return _dataContextFactory.UseWritableContext(async c =>
        {
            var d = await c.Demographics.FirstOrDefaultAsync(d => d.BadgeNumber == req.BadgeNumber,
                cancellationToken);

            if (d == null)
            {
                return Result<MasterInquiryResponseDto>.Failure(Error.EmployeeNotFound);
            }

            var pd = new ProfitDetail
            {
                ProfitCodeId = /* 0 */ProfitCode.Constants.IncomingContributions,
                ProfitYear = req.ProfitYear,
                ProfitYearIteration = 1,
                CommentTypeId = /* 19 */CommentType.Constants.Military.Id,
                Contribution = req.ContributionAmount,
                Ssn = d.Ssn,
                YearsOfServiceCredit = 1
            };
            c.ProfitDetails.Add(pd);

            await c.SaveChangesAsync(cancellationToken);

            return Result<MasterInquiryResponseDto>.Success(new MasterInquiryResponseDto
            {
                Id = pd.Id,
                DistributionSequence = pd.DistributionSequence,
                ProfitCodeId = pd.ProfitCodeId,
                BadgeNumber = req.BadgeNumber,
                Contribution = pd.Contribution,
                CommentTypeId = /* 19 */CommentType.Constants.Military,
                ProfitYear = req.ProfitYear,
                ProfitYearIteration = pd.ProfitYearIteration,
                Ssn = pd.Ssn.MaskSsn()
            });
        }, cancellationToken);
    }

    public async Task<Result<PaginatedResponseDto<MasterInquiryResponseDto>>> GetMilitaryServiceRecordAsync(MilitaryContributionRequest req, CancellationToken cancellationToken = default)
    {
        #region Validation

        var validator = new InlineValidator<MilitaryContributionRequest>();

        validator.RuleFor(r => r.BadgeNumber)
            .GreaterThan(0)
            .WithMessage($"{nameof(MilitaryContributionRequest.BadgeNumber)} must be greater than zero.");

        validator.RuleFor(r => r.ProfitYear)
            .GreaterThanOrEqualTo((short)2000)
            .WithMessage($"{nameof(MilitaryContributionRequest.ProfitYear)} must not less than 2000.")
            .LessThanOrEqualTo((short)DateTime.Today.Year)
            .WithMessage($"{nameof(MilitaryContributionRequest.ProfitYear)} must not be greater than this year.");

        var validationResult = await validator.ValidateAsync(req, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result<PaginatedResponseDto<MasterInquiryResponseDto>>.ValidationFailure(errors);
        }

        #endregion

        var result = await _dataContextFactory.UseReadOnlyContext(c =>
        {
            return c.ProfitDetails
                .Include(pd=> pd.CommentType)
                .Join(c.Demographics,   
                    c => c.Ssn,
                    cm => cm.Ssn,
                    (pd, d) => new { pd , d})
                .Where(x => x.d.BadgeNumber == req.BadgeNumber
                            && x.pd.ProfitYear == req.ProfitYear && x.pd.CommentTypeId == CommentType.Constants.Military.Id)
                .OrderByDescending(x => x.pd.ProfitYear)
                .ThenByDescending(x=> x.pd.CreatedUtc)
                .Select(x => new MasterInquiryResponseDto
                {
                    Id = x.pd.Id,
                    Ssn = x.pd.Ssn.MaskSsn(),
                    ProfitYear = x.pd.ProfitYear,
                    ProfitYearIteration = x.pd.ProfitYearIteration,
                    DistributionSequence = x.pd.DistributionSequence,
                    ProfitCodeId = x.pd.ProfitCodeId,
                    ProfitCodeName = x.pd.ProfitCode.Name,
                    Contribution = x.pd.Contribution,
                    Earnings = x.pd.Earnings,
                    Forfeiture = x.pd.Forfeiture,
                    Remark = x.pd.Remark,
                    CommentTypeId = x.pd.CommentTypeId,
                    CommentTypeName = x.pd.CommentType!.Name,
                    CommentRelatedCheckNumber = x.pd.CommentRelatedCheckNumber,
                    CommentRelatedState = x.pd.CommentRelatedState,
                    CommentRelatedOracleHcmId = x.pd.CommentRelatedOracleHcmId
                })
                .ToPaginationResultsAsync(req, cancellationToken);
        });

        return Result<PaginatedResponseDto<MasterInquiryResponseDto>>.Success(result);
    }
}