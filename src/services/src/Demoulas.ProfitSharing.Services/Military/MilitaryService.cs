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

namespace Demoulas.ProfitSharing.Services.Military
{
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

            var validator = new InlineValidator<CreateMilitaryContributionRequest>();

            validator.RuleFor(r => r.ContributionAmount)
                .GreaterThan(0)
                .WithMessage($"The {nameof(CreateMilitaryContributionRequest.ContributionAmount)} must be greater than zero.");

            validator.RuleFor(r => r.ContributionDate)
                .NotEmpty()
                .Must(date => date.ToDateTime(TimeOnly.MinValue) > DateTime.Today.AddYears(-3))
                .WithMessage($"The {nameof(CreateMilitaryContributionRequest.ContributionDate)} must be within the last three years.");


            var validationResult = validator.Validate(req);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return Task.FromResult(Result<MasterInquiryResponseDto>.ValidationFailure(errors));
            }

            return _dataContextFactory.UseWritableContext(async c =>
            {
                var d = await c.Demographics.FirstOrDefaultAsync(d => d.BadgeNumber == req.BadgeNumber,
                    cancellationToken);

                if (d == null)
                {
                    return Result<MasterInquiryResponseDto>.Failure(Error.EmployeeNotFound);
                }

                c.ProfitDetails.Add(new ProfitDetail
                {
                    ProfitCodeId = /* 0 */ProfitCode.Constants.IncomingContributions,
                    ProfitYear = req.ProfitYear,
                    ProfitYearIteration = 1,
                    CommentTypeId = /* 19 */CommentType.Constants.Military,
                    Contribution = req.ContributionAmount,
                    Ssn = d.Ssn
                });

                await c.SaveChangesAsync(cancellationToken);

                return Result<MasterInquiryResponseDto>.Success(new MasterInquiryResponseDto
                {
                    BadgeNumber = req.BadgeNumber,
                    Contribution = req.ContributionAmount,
                    CommentTypeId = /* 19 */CommentType.Constants.Military,
                    ProfitYear = req.ProfitYear
                });
            }, cancellationToken);
        }

        public Task<PaginatedResponseDto<MasterInquiryResponseDto>> GetMilitaryServiceRecordAsync(MilitaryContributionRequest req, CancellationToken cancellationToken = default)
        {
            return _dataContextFactory.UseReadOnlyContext(c =>
            {
                return c.ProfitDetails
                    .Include(pd=> pd.CommentType)
                    .Join(c.Demographics,   
                        c => c.Ssn,
                        cm => cm.Ssn,
                        (pd, d) => new { pd , d})
                    .Where(x => x.d.BadgeNumber == req.BadgeNumber
                    && x.pd.ProfitYear == req.ProfitYear)
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
        }
    }
}

