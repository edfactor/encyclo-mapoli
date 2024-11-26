using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;
public class MasterInquiryService : IMasterInquiryService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger _logger;
    public MasterInquiryService(
        IProfitSharingDataContextFactory dataContextFactory,
        ILoggerFactory loggerFactory
    )
    {
        _dataContextFactory = dataContextFactory;
        _logger = loggerFactory.CreateLogger<MasterInquiryService>();
    }

    public async Task<PaginatedResponseDto<MasterInquiryResponseDto>> GetMasterInquiry(MasterInquiryRequest req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("REQUEST MASTER INQUIRY"))
        {
            var rslt = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var query = ctx.ProfitDetails
                            .Join(ctx.Demographics,
                                pd => pd.Ssn,
                                d => d.Ssn,
                                (pd, d) => new { ProfitDetail = pd, Demographics = d })
                            .Where(x => x.Demographics.PayFrequency!.Id == 1);

                if (req.StartProfitYear.HasValue)
                {
                    query = query.Where(x => x.ProfitDetail.ProfitYear >= req.StartProfitYear);
                }

                if (req.EndProfitYear.HasValue)
                {
                    query = query.Where(x => x.ProfitDetail.ProfitYear <= req.EndProfitYear);
                }

                if (req.StartProfitMonth.HasValue)
                {
                    query = query.Where(x => x.ProfitDetail.MonthToDate >= req.StartProfitMonth);
                }

                if (req.EndProfitMonth.HasValue)
                {
                    query = query.Where(x => x.ProfitDetail.MonthToDate <= req.EndProfitMonth);
                }

                if (req.ProfitCode.HasValue)
                {
                    query = query.Where(x => x.ProfitDetail.ProfitCode.Id == req.ProfitCode);
                }

                if (req.ContributionAmount.HasValue)
                {
                    query = query.Where(x => x.ProfitDetail.Contribution == req.ContributionAmount);
                }

                if (req.EarningsAmount.HasValue)
                {
                    query = query.Where(x => x.ProfitDetail.Earnings == req.EarningsAmount);
                }

                if (req.SocialSecurity != null)
                {
                    query = query.Where(x => x.ProfitDetail.Ssn == req.SocialSecurity);
                }

                if (req.ForfeitureAmount.HasValue)
                {
                    query = query.Where(x =>
                        x.ProfitDetail.ProfitCode.Id == ProfitCode.Constants.IncomingContributions.Id &&
                        x.ProfitDetail.Forfeiture == req.ForfeitureAmount);
                }

                if (req.PaymentAmount.HasValue)
                {
                    query = query.Where(x =>
                        x.ProfitDetail.ProfitCode.Id != ProfitCode.Constants.IncomingContributions.Id &&
                        x.ProfitDetail.Forfeiture == req.PaymentAmount);
                }
                var results = await query
                .Select(x => new MasterInquiryResponseDto
                {
                    Id = x.ProfitDetail.Id,
                    Ssn = x.ProfitDetail.Ssn,
                    ProfitYear = x.ProfitDetail.ProfitYear,
                    ProfitYearIteration = x.ProfitDetail.ProfitYearIteration,
                    DistributionSequence = x.ProfitDetail.DistributionSequence,
                    ProfitCodeId = x.ProfitDetail.ProfitCodeId,
                    Contribution = x.ProfitDetail.Contribution,
                    Earnings = x.ProfitDetail.Earnings,
                    Forfeiture = x.ProfitDetail.Forfeiture,
                    MonthToDate = x.ProfitDetail.MonthToDate,
                    YearToDate = x.ProfitDetail.YearToDate,
                    Remark = x.ProfitDetail.Remark,
                    ZeroContributionReasonId = x.ProfitDetail.ZeroContributionReasonId,
                    FederalTaxes = x.ProfitDetail.FederalTaxes,
                    StateTaxes = x.ProfitDetail.StateTaxes,
                    TaxCodeId = x.ProfitDetail.TaxCodeId,
                    CommentTypeId = x.ProfitDetail.CommentTypeId,
                    CommentRelatedCheckNumber = x.ProfitDetail.CommentRelatedCheckNumber,
                    CommentRelatedState = x.ProfitDetail.CommentRelatedState,
                    CommentRelatedOracleHcmId = x.ProfitDetail.CommentRelatedOracleHcmId,
                    CommentRelatedPsnSuffix = x.ProfitDetail.CommentRelatedPsnSuffix,
                    CommentIsPartialTransaction = x.ProfitDetail.CommentIsPartialTransaction
                })
                .OrderByDescending(x => x.ProfitYear)
                .ToPaginationResultsAsync(req, cancellationToken);

                _logger.LogInformation("Returned {Results} records", results.Results.Count());
                return results;
            });

            return rslt;
        }
    }
}
