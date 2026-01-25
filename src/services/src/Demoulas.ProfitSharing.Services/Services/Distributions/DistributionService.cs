using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Services.Reports;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Services.Distributions;

public sealed class DistributionService : IDistributionService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly IAppUser? _appUser;
    private readonly TotalService _totalService;
    private readonly ICalendarService _calendarService;
    private readonly TimeProvider _timeProvider;

    public DistributionService(IProfitSharingDataContextFactory dataContextFactory, IDemographicReaderService demographicReaderService, IAppUser? appUser, TotalService totalService, ICalendarService calendarService, TimeProvider timeProvider)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _appUser = appUser;
        _totalService = totalService;
        _calendarService = calendarService;
        _timeProvider = timeProvider;
    }

    public Task<Result<PaginatedResponseDto<DistributionSearchResponse>>> SearchAsync(DistributionSearchRequest request, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographic = await _demographicReaderService.BuildDemographicQueryAsync(ctx, false);
            var query = from dist in ctx.Distributions
                        join freq in ctx.DistributionFrequencies on dist.FrequencyId equals freq.Id
                        join status in ctx.DistributionStatuses on dist.StatusId equals status.Id
                        join tax in ctx.TaxCodes on dist.TaxCodeId equals tax.Id
                        join demLj in demographic on dist.Ssn equals demLj.Ssn into demGroup
                        from dem in demGroup.DefaultIfEmpty()
                        join benLj in ctx.Beneficiaries.Where(b => !b.IsDeleted) on dist.Ssn equals benLj.Contact!.Ssn into benGroup
                        from ben in benGroup.DefaultIfEmpty()
                        select new DistributionSearchQueryItem
                        {
                            Id = dist.Id,
                            PaymentSequence = dist.PaymentSequence,
                            Ssn = dist.Ssn,
                            BadgeNumber = dem != null ? dem.BadgeNumber : null,
                            // FullName picks employee name if available, otherwise beneficiary name
                            FullName = dem != null && dem.ContactInfo.FullName != null
                                ? dem.ContactInfo.FullName
                                : (ben != null && ben.Contact!.ContactInfo.FullName != null
                                    ? ben.Contact!.ContactInfo.FullName
                                    : string.Empty),
                            FrequencyId = dist.FrequencyId,
                            FrequencyName = freq.Name,
                            StatusId = dist.StatusId,
                            StatusName = status.Name,
                            TaxCodeId = dist.TaxCodeId,
                            TaxCodeName = tax.Name,
                            GrossAmount = dist.GrossAmount,
                            FederalTax = dist.FederalTaxAmount,
                            StateTax = dist.StateTaxAmount,
                            CheckAmount = dist.CheckAmount,
                            IsExecutive = dem != null && dem.PayFrequencyId == PayFrequency.Constants.Monthly,
                            DemographicId = dem != null ? dem.Id : null,
                            BeneficiaryId = ben != null ? ben.Id : null
                        };

            int searchSsn;
            if (!string.IsNullOrWhiteSpace(request.Ssn) && int.TryParse(request.Ssn, out searchSsn))
            {
                // Validate that SSN exists in either demographics or beneficiaries
                var ssnExists = await demographic.AnyAsync(d => d.Ssn == searchSsn, cancellationToken) ||
                               await ctx.Beneficiaries.Where(b => !b.IsDeleted).AnyAsync(b => b.Contact!.Ssn == searchSsn, cancellationToken);

                if (!ssnExists)
                {
                    return Result<PaginatedResponseDto<DistributionSearchResponse>>.Failure(Error.SsnNotFound);
                }

                query = query.Where(d => d.Ssn == searchSsn);
            }

            if (request.BadgeNumber.HasValue)
            {
                if (request.PsnSuffix.HasValue)
                {
                    var ssn = await ctx.Beneficiaries.Where(x => !x.IsDeleted && x.BadgeNumber == request.BadgeNumber.Value && x.PsnSuffix == request.PsnSuffix.Value).Select(x => x.Contact!.Ssn).FirstOrDefaultAsync(cancellationToken);
                    if (ssn == default)
                    {
                        return Result<PaginatedResponseDto<DistributionSearchResponse>>.Failure(Error.BadgeNumberNotFound);
                    }
                    query = query.Where(d => d.Ssn == ssn);
                }
                else
                {
                    var ssn = await demographic.Where(x => x.BadgeNumber == request.BadgeNumber.Value).Select(x => x.Ssn).FirstOrDefaultAsync(cancellationToken);
                    if (ssn == default)
                    {
                        return Result<PaginatedResponseDto<DistributionSearchResponse>>.Failure(Error.BadgeNumberNotFound);
                    }
                    query = query.Where(d => d.Ssn == ssn);
                }

            }

            if (request.DistributionFrequencyId.HasValue)
            {
                query = query.Where(d => d.FrequencyId == request.DistributionFrequencyId.Value);
            }

            // Support both single and multiple status IDs
            // Prioritize DistributionStatusIds (array) if provided, otherwise use single DistributionStatusId
            if (request.DistributionStatusIds != null && request.DistributionStatusIds.Count > 0)
            {
                // Convert string list to char list for comparison
                var statusChars = request.DistributionStatusIds
                    .Where(s => !string.IsNullOrEmpty(s) && s.Length == 1)
                    .Select(s => s[0])
                    .ToList();

                if (statusChars.Count > 0)
                {
                    query = query.Where(d => statusChars.Contains(d.StatusId));
                }
            }
            else if (request.DistributionStatusId.HasValue)
            {
                query = query.Where(d => d.StatusId == request.DistributionStatusId.Value);
            }

            if (request.TaxCodeId.HasValue)
            {
                query = query.Where(d => d.TaxCodeId == request.TaxCodeId.Value);
            }

            if (request.MinGrossAmount.HasValue)
            {
                query = query.Where(d => d.GrossAmount >= request.MinGrossAmount.Value);
            }

            if (request.MaxGrossAmount.HasValue)
            {
                query = query.Where(d => d.GrossAmount <= request.MaxGrossAmount.Value);
            }

            if (request.MinCheckAmount.HasValue)
            {
                query = query.Where(d => d.CheckAmount >= request.MinCheckAmount.Value);
            }

            if (request.MaxCheckAmount.HasValue)
            {
                query = query.Where(d => d.CheckAmount <= request.MaxCheckAmount.Value);
            }

            // Filter by MemberType similar to MasterInquiry pattern
            if (request.MemberType.HasValue)
            {
                if (request.MemberType.Value == 1) // Employees only
                {
                    query = query.Where(d => d.BadgeNumber != null);
                }
                else if (request.MemberType.Value == 2) // Beneficiaries only
                {
                    query = query.Where(d => d.BadgeNumber == null);
                }
                // If null or any other value, don't filter (show all)
            }

            // Add query tagging for production traceability
            var searchContext = $"DistributionSearch-Badge{request.BadgeNumber}-SSN{(string.IsNullOrWhiteSpace(request.Ssn?.MaskSsn()) ? "None" : "Provided")}-{_timeProvider.GetUtcNow():yyyyMMddHHmm}";
            query = query.TagWith(searchContext);

            // Normalize SortBy to PascalCase for OrderByProperty compatibility
            // Frontend sends camelCase (e.g., "fullName") but OrderByProperty uses case-sensitive reflection
            var paginationRequest = new SortedPaginationRequestDto
            {
                Skip = request.Skip,
                Take = request.Take,
                SortBy = !string.IsNullOrWhiteSpace(request.SortBy)
                    ? request.SortBy.FirstCharToUpper()
                    : null,
                IsSortDescending = request.IsSortDescending
            };

            var data = await query.ToPaginationResultsAsync(paginationRequest, cancellationToken);

            var result = new PaginatedResponseDto<DistributionSearchResponse>(request)
            {
                Total = data.Total,
                Results = data.Results.Select(d => new DistributionSearchResponse
                {
                    Id = d.Id,
                    PaymentSequence = d.PaymentSequence,
                    Ssn = d.Ssn.MaskSsn(),
                    BadgeNumber = d.BadgeNumber,
                    FullName = d.FullName,
                    FrequencyId = d.FrequencyId,
                    FrequencyName = d.FrequencyName,
                    StatusId = d.StatusId,
                    StatusName = d.StatusName,
                    TaxCodeId = d.TaxCodeId,
                    TaxCodeName = d.TaxCodeName,
                    GrossAmount = d.GrossAmount,
                    FederalTax = d.FederalTax,
                    StateTax = d.StateTax,
                    CheckAmount = d.CheckAmount,
                    IsExecutive = d.IsExecutive,
                    IsEmployee = d.BadgeNumber.HasValue,
                    DemographicId = d.DemographicId,
                    BeneficiaryId = d.BeneficiaryId
                }).ToList()
            };

            return Result<PaginatedResponseDto<DistributionSearchResponse>>.Success(result);
        }, cancellationToken);
    }

    public Task<Result<CreateOrUpdateDistributionResponse>> CreateDistributionAsync(CreateDistributionRequest request, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseWritableContext(async ctx =>
        {
            var demographic = await _demographicReaderService.BuildDemographicQueryAsync(ctx, false);
            var dem = await demographic.Where(d => d.BadgeNumber == request.BadgeNumber).FirstOrDefaultAsync(cancellationToken);
            if (dem == null)
            {
                return Result<CreateOrUpdateDistributionResponse>.Failure(Error.BadgeNumberNotFound);
            }

            var balance = await _totalService.GetVestingBalanceForSingleMemberAsync(Common.Contracts.Request.SearchBy.Ssn, dem.Ssn, (short)DateTime.Today.Year, cancellationToken);
            if (request.TaxCodeId == TaxCode.Constants.NormalDistribution.Id && dem.DateOfBirth.Age() > 64 && balance != default && balance.VestedBalance != request.GrossAmount && string.IsNullOrEmpty(request.Memo))
            {
                request.Memo = "AGE>64 - OVERRIDE";
            }

            if (request.GrossAmount > (balance?.VestedBalance ?? 0))
            {
                var validationErrors = new Dictionary<string, string[]>
                {
                    [nameof(request.GrossAmount)] = [$"Gross amount {request.GrossAmount:C} exceeds vested balance {(balance?.VestedBalance ?? 0):C}."]
                };
                return Result<CreateOrUpdateDistributionResponse>.ValidationFailure(validationErrors);
            }

            if (request.FrequencyId == DistributionFrequency.Constants.RolloverDirect)
            {
                request.FederalTaxAmount = 0;
                request.FederalTaxPercentage = 0;
                request.StateTaxAmount = 0;
                request.StateTaxPercentage = 0;
                request.CheckAmount = request.GrossAmount;
            }

            var maxSequence = await ctx.Distributions.Where(d => d.Ssn == dem.Ssn).MaxAsync(d => (byte?)d.PaymentSequence, cancellationToken: cancellationToken) ?? 0;
            var distribution = new Distribution
            {
                Ssn = dem.Ssn,
                EmployeeName = dem.ContactInfo!.FullName ?? "",
                PaymentSequence = (byte)(maxSequence + 1),
                StatusId = request.StatusId,
                FrequencyId = request.FrequencyId,
                PayeeId = request.PayeeId,
                ForTheBenefitOfPayee = request.ForTheBenefitOfPayee,
                ForTheBenefitOfAccountType = request.ForTheBenefitOfAccountType,
                Tax1099ForEmployee = request.Tax1099ForEmployee,
                Tax1099ForBeneficiary = request.Tax1099ForBeneficiary,
                GrossAmount = request.GrossAmount,
                FederalTaxAmount = request.FederalTaxAmount,
                StateTaxAmount = request.StateTaxAmount,
                TaxCodeId = request.TaxCodeId,
                IsDeceased = request.IsDeceased,
                GenderId = request.GenderId,
                QualifiedDomesticRelationsOrder = request.IsQdro,
                Memo = request.Memo,
                RothIra = request.IsRothIra,
                CreatedAtUtc = _timeProvider.GetUtcNow().DateTime,
                UserName = _appUser != null ? _appUser.UserName : "unknown",
                ThirdPartyPayeeAccount = request.ThirdPartyPayee?.Account
            };

            if (request.ThirdPartyPayee != null)
            {
                distribution.ThirdPartyPayee = new DistributionThirdPartyPayee
                {
                    Payee = request.ThirdPartyPayee.Payee,
                    Name = request.ThirdPartyPayee.Name,
                    Address = new Data.Entities.Address()
                    {
                        Street = request.ThirdPartyPayee.Address.Street,
                        Street2 = request.ThirdPartyPayee.Address.Street2,
                        Street3 = request.ThirdPartyPayee.Address.Street3,
                        Street4 = request.ThirdPartyPayee.Address.Street4,
                        City = request.ThirdPartyPayee.Address.City,
                        State = request.ThirdPartyPayee.Address.State,
                        PostalCode = request.ThirdPartyPayee.Address.PostalCode,
                        CountryIso = request.ThirdPartyPayee.Address.CountryIso ?? "US",
                    },
                    Memo = request.ThirdPartyPayee.Memo
                };
            }
            await ctx.Distributions.AddAsync(distribution, cancellationToken);
            await ctx.SaveChangesAsync(cancellationToken);

            return Result<CreateOrUpdateDistributionResponse>.Success(new CreateOrUpdateDistributionResponse
            {
                Id = distribution.Id,
                BadgeNumber = request.BadgeNumber, // Include BadgeNumber from the original request
                MaskSsn = distribution.Ssn.MaskSsn(),
                PaymentSequence = distribution.PaymentSequence,
                StatusId = distribution.StatusId,
                FrequencyId = distribution.FrequencyId,
                PayeeId = distribution.PayeeId,
                ForTheBenefitOfPayee = distribution.ForTheBenefitOfPayee,
                ForTheBenefitOfAccountType = distribution.ForTheBenefitOfAccountType,
                Tax1099ForEmployee = distribution.Tax1099ForEmployee,
                Tax1099ForBeneficiary = distribution.Tax1099ForBeneficiary,
                FederalTaxPercentage = distribution.FederalTaxPercentage,
                StateTaxPercentage = distribution.StateTaxPercentage,
                GrossAmount = distribution.GrossAmount,
                FederalTaxAmount = distribution.FederalTaxAmount,
                StateTaxAmount = distribution.StateTaxAmount,
                CheckAmount = distribution.CheckAmount,
                TaxCodeId = distribution.TaxCodeId,
                IsDeceased = distribution.IsDeceased,
                GenderId = distribution.GenderId,
                IsQdro = distribution.QualifiedDomesticRelationsOrder,
                Memo = distribution.Memo,
                IsRothIra = distribution.RothIra,
                ThirdPartyPayee = new ThirdPartyPayee()
                {
                    Account = distribution.ThirdPartyPayeeAccount,
                    Name = distribution.ThirdPartyPayee?.Name,
                    Payee = distribution.ThirdPartyPayee?.Payee,
                    Memo = distribution.ThirdPartyPayee?.Memo,
                    Address = new Common.Contracts.Request.Distributions.Address()
                    {
                        Street = distribution.ThirdPartyPayee?.Address?.Street ?? string.Empty,
                        Street2 = distribution.ThirdPartyPayee?.Address?.Street2,
                        Street3 = distribution.ThirdPartyPayee?.Address?.Street3,
                        Street4 = distribution.ThirdPartyPayee?.Address?.Street4,
                        City = distribution.ThirdPartyPayee?.Address?.City,
                        State = distribution.ThirdPartyPayee?.Address?.State,
                        PostalCode = distribution.ThirdPartyPayee?.Address?.PostalCode,
                        CountryIso = distribution.ThirdPartyPayee?.Address?.CountryIso ?? "US",
                    }
                }
            });
        }, cancellationToken);
    }

    public Task<Result<CreateOrUpdateDistributionResponse>> UpdateDistributionAsync(UpdateDistributionRequest request, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseWritableContext(async ctx =>
        {
            var distribution = await ctx.Distributions
                .Include(d => d.ThirdPartyPayee)
                .ThenInclude(tp => tp!.Address)
                .Where(d => d.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (distribution == null)
            {
                return Result<CreateOrUpdateDistributionResponse>.Failure(Error.DistributionNotFound);
            }
            var demographic = await _demographicReaderService.BuildDemographicQueryAsync(ctx, false);
            var dem = await demographic.Where(d => d.BadgeNumber == request.BadgeNumber).FirstOrDefaultAsync(cancellationToken);
            if (dem == null)
            {
                return Result<CreateOrUpdateDistributionResponse>.Failure(Error.BadgeNumberNotFound);
            }
            var balance = await _totalService.GetVestingBalanceForSingleMemberAsync(Common.Contracts.Request.SearchBy.Ssn, dem.Ssn, (short)DateTime.Today.Year, cancellationToken);
            if (request.TaxCodeId == TaxCode.Constants.NormalDistribution.Id && dem.DateOfBirth.Age() > 64 && balance != default && balance.VestedBalance != request.GrossAmount && string.IsNullOrEmpty(request.Memo))
            {
                request.Memo = "AGE>64 - OVERRIDE";
            }
            var originalGrossAmount = distribution.GrossAmount;
            if (request.GrossAmount > (balance != default ? balance.VestedBalance - originalGrossAmount : originalGrossAmount))
            {
                var validationErrors = new Dictionary<string, string[]>
                {
                    [nameof(request.GrossAmount)] = [$"Gross amount {request.GrossAmount:C} exceeds vested balance {(balance?.VestedBalance ?? 0):C}."]
                };
                return Result<CreateOrUpdateDistributionResponse>.ValidationFailure(validationErrors);
            }

            if (request.FrequencyId == DistributionFrequency.Constants.RolloverDirect)
            {
                request.FederalTaxAmount = 0;
                request.FederalTaxPercentage = 0;
                request.StateTaxAmount = 0;
                request.StateTaxPercentage = 0;
                request.CheckAmount = request.GrossAmount;
            }

            distribution.StatusId = request.StatusId;
            distribution.FrequencyId = request.FrequencyId;
            if (request.ThirdPartyPayee != default)
            {
                distribution.ThirdPartyPayee = distribution.ThirdPartyPayee ?? new DistributionThirdPartyPayee()
                {
                    Address = new Data.Entities.Address() { Street = string.Empty }
                };
                distribution.ThirdPartyPayee!.Name = request.ThirdPartyPayee.Name;
                distribution.ThirdPartyPayee!.Payee = request.ThirdPartyPayee.Payee;
                distribution.ThirdPartyPayee!.Memo = request.ThirdPartyPayee.Memo;
                distribution.ThirdPartyPayee!.Address.Street = request.ThirdPartyPayee.Address.Street;
                distribution.ThirdPartyPayee!.Address.Street2 = request.ThirdPartyPayee.Address.Street2;
                distribution.ThirdPartyPayee!.Address.Street3 = request.ThirdPartyPayee.Address.Street3;
                distribution.ThirdPartyPayee!.Address.Street4 = request.ThirdPartyPayee.Address.Street4;
                distribution.ThirdPartyPayee!.Address.City = request.ThirdPartyPayee.Address.City;
                distribution.ThirdPartyPayee!.Address.State = request.ThirdPartyPayee.Address.State;
                distribution.ThirdPartyPayee!.Address.PostalCode = request.ThirdPartyPayee.Address.PostalCode;
                distribution.ThirdPartyPayee!.Address.CountryIso = request.ThirdPartyPayee.Address.CountryIso ?? "US";
                distribution.ThirdPartyPayeeAccount = request.ThirdPartyPayee?.Account;
            }
            else
            {
                distribution.ThirdPartyPayeeId = null;
            }
            distribution.ForTheBenefitOfAccountType = request.ForTheBenefitOfAccountType;
            distribution.ForTheBenefitOfPayee = request.ForTheBenefitOfPayee;
            distribution.Tax1099ForBeneficiary = request.Tax1099ForBeneficiary;
            distribution.Tax1099ForEmployee = request.Tax1099ForEmployee;
            distribution.GrossAmount = request.GrossAmount;
            distribution.FederalTaxAmount = request.FederalTaxAmount;
            distribution.StateTaxAmount = request.StateTaxAmount;
            distribution.TaxCodeId = request.TaxCodeId;
            distribution.IsDeceased = request.IsDeceased;
            distribution.GenderId = request.GenderId;
            distribution.QualifiedDomesticRelationsOrder = request.IsQdro;
            distribution.Memo = request.Memo;
            distribution.RothIra = request.IsRothIra;
            distribution.ModifiedAtUtc = _timeProvider.GetUtcNow().DateTime;

            await ctx.SaveChangesAsync(cancellationToken);
            var response = new CreateOrUpdateDistributionResponse
            {
                Id = distribution.Id,
                BadgeNumber = request.BadgeNumber,
                MaskSsn = distribution.Ssn.MaskSsn(),
                PaymentSequence = distribution.PaymentSequence,
                StatusId = distribution.StatusId,
                FrequencyId = distribution.FrequencyId,
                PayeeId = distribution.PayeeId,
                ForTheBenefitOfPayee = distribution.ForTheBenefitOfPayee,
                ForTheBenefitOfAccountType = distribution.ForTheBenefitOfAccountType,
                Tax1099ForEmployee = distribution.Tax1099ForEmployee,
                Tax1099ForBeneficiary = distribution.Tax1099ForBeneficiary,
                FederalTaxPercentage = distribution.FederalTaxPercentage,
                StateTaxPercentage = distribution.StateTaxPercentage,
                GrossAmount = distribution.GrossAmount,
                FederalTaxAmount = distribution.FederalTaxAmount,
                StateTaxAmount = distribution.StateTaxAmount,
                CheckAmount = distribution.CheckAmount,
                TaxCodeId = distribution.TaxCodeId,
                IsDeceased = distribution.IsDeceased,
                GenderId = distribution.GenderId,
                IsQdro = distribution.QualifiedDomesticRelationsOrder,
                Memo = distribution.Memo,
                IsRothIra = distribution.RothIra,
                ThirdPartyPayee = new ThirdPartyPayee()
                {
                    Account = distribution.ThirdPartyPayeeAccount,
                    Name = distribution.ThirdPartyPayee?.Name,
                    Payee = distribution.ThirdPartyPayee?.Payee,
                    Memo = distribution.ThirdPartyPayee?.Memo,
                    Address = new Common.Contracts.Request.Distributions.Address()
                    {
                        Street = distribution.ThirdPartyPayee?.Address?.Street ?? string.Empty,
                        Street2 = distribution.ThirdPartyPayee?.Address?.Street2,
                        Street3 = distribution.ThirdPartyPayee?.Address?.Street3,
                        Street4 = distribution.ThirdPartyPayee?.Address?.Street4,
                        City = distribution.ThirdPartyPayee?.Address?.City,
                        State = distribution.ThirdPartyPayee?.Address?.State,
                        PostalCode = distribution.ThirdPartyPayee?.Address?.PostalCode,
                        CountryIso = distribution.ThirdPartyPayee?.Address?.CountryIso ?? "US",
                    }
                }
            };
            return Result<CreateOrUpdateDistributionResponse>.Success(response);
        }, cancellationToken);
    }

    public async Task<Result<bool>> DeleteDistributionAsync(int distributionId, CancellationToken cancellationToken)
    {
        // Validate input parameters
        if (distributionId <= 0)
        {
            var validationErrors = new Dictionary<string, string[]>
            {
                [nameof(distributionId)] = ["Distribution ID must be a positive integer."]
            };
            return Result<bool>.ValidationFailure(validationErrors);
        }

        return await _dataContextFactory.UseWritableContext(async ctx =>
        {
            var distribution = await ctx.Distributions.Where(d => d.Id == distributionId).FirstOrDefaultAsync(cancellationToken);
            if (distribution == null)
            {
                return Result<bool>.Failure(Error.DistributionNotFound);
            }

            distribution.StatusId = DistributionStatus.Constants.PurgeRecord;
            distribution.ModifiedAtUtc = DateTimeOffset.UtcNow;

            await ctx.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }, cancellationToken);
    }

    public Task<Result<DistributionRunReportSummaryResponse[]>> GetDistributionRunReportSummaryAsync(CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var distributionQuery = GetDistributionExtract(ctx, Array.Empty<char>());

            var groupedResults = await distributionQuery
                .TagWith($"DistributionSummaryReport-{_timeProvider.GetUtcNow():yyyyMMddHHmm}")
                .GroupBy(d => d.FrequencyId)
                .Select(g => new DistributionRunReportSummaryResponse
                {
                    DistributionFrequencyId = g.Key,
                    DistributionTypeName = g.First().Frequency!.Name,
                    TotalDistributions = g.Count(),
                    TotalGrossAmount = g.Sum(d => d.GrossAmount),
                    TotalFederalTaxAmount = g.Sum(d => d.FederalTaxAmount),
                    TotalStateTaxAmount = g.Sum(d => d.StateTaxAmount),
                    TotalCheckAmount = g.Sum(d => d.GrossAmount) - g.Sum(d => d.FederalTaxAmount) - g.Sum(d => d.StateTaxAmount)
                })
                .ToListAsync(cancellationToken);

            var foundFrequencyIds = groupedResults.Select(gr => gr.DistributionFrequencyId).ToHashSet();
            var missingFrequencies = (await ctx.DistributionFrequencies.ToListAsync(cancellationToken))
                .Where(df => !foundFrequencyIds.Any(gr => gr == df.Id))
                .Select(df => new DistributionRunReportSummaryResponse
                {
                    DistributionFrequencyId = df.Id,
                    DistributionTypeName = df.Name,
                    TotalDistributions = 0,
                    TotalGrossAmount = 0,
                    TotalFederalTaxAmount = 0,
                    TotalStateTaxAmount = 0,
                    TotalCheckAmount = 0
                })
                .ToList();

            var manualAndOnHoldDistributions = await (
                from d in ctx.Distributions.Include(x => x.Frequency)
                where d.StatusId == DistributionStatus.Constants.RequestOnHold || d.StatusId == DistributionStatus.Constants.ManualCheck
                group d by d.StatusId into g
                select new DistributionRunReportSummaryResponse
                {
                    DistributionFrequencyId = null,
                    DistributionTypeName = g.First().Status!.Name,
                    TotalDistributions = g.Count(),
                    TotalGrossAmount = g.Sum(d => d.GrossAmount),
                    TotalFederalTaxAmount = g.Sum(d => d.FederalTaxAmount),
                    TotalStateTaxAmount = g.Sum(d => d.StateTaxAmount),
                    TotalCheckAmount = g.Sum(d => d.GrossAmount) - g.Sum(d => d.FederalTaxAmount) - g.Sum(d => d.StateTaxAmount)
                })
                .TagWith($"DistributionSummaryManualOnHold-{_timeProvider.GetUtcNow():yyyyMMddHHmm}")
                .ToListAsync(cancellationToken);

            var foundStatusNames = manualAndOnHoldDistributions.Select(gr => gr.DistributionTypeName).ToHashSet();
            var missingStatuses = (await ctx.DistributionStatuses.Where(d => d.Id == DistributionStatus.Constants.RequestOnHold || d.Id == DistributionStatus.Constants.ManualCheck).ToListAsync(cancellationToken))
                .Where(ds => !foundStatusNames.Any(gr => gr == ds.Name))
                .Select(ds => new DistributionRunReportSummaryResponse
                {
                    DistributionFrequencyId = null,
                    DistributionTypeName = ds.Name,
                    TotalDistributions = 0,
                    TotalGrossAmount = 0,
                    TotalFederalTaxAmount = 0,
                    TotalStateTaxAmount = 0,
                    TotalCheckAmount = 0
                })
                .ToList();

            groupedResults.AddRange(missingFrequencies);
            groupedResults.AddRange(manualAndOnHoldDistributions);
            groupedResults.AddRange(missingStatuses);

            groupedResults.Sort((a, b) => a.DistributionTypeName.CompareTo(b.DistributionTypeName, StringComparison.OrdinalIgnoreCase));

            return Result<DistributionRunReportSummaryResponse[]>.Success(groupedResults.ToArray());
        }, cancellationToken);
    }

    public Task<Result<PaginatedResponseDto<DistributionsOnHoldResponse>>> GetDistributionsOnHoldAsync(SortedPaginationRequestDto request, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var query = from dist in ctx.Distributions.Include(x => x.Payee)
                        where dist.StatusId == DistributionStatus.Constants.RequestOnHold
                        select new
                        {
                            dist.Ssn,
                            PayTo = dist.Payee!.Name,
                            dist.CheckAmount,
                        };

            // Add query tagging for on-hold distributions
            query = query.TagWith($"DistributionsOnHold-{_timeProvider.GetUtcNow():yyyyMMddHHmm}");

            var paginatedResults = await query.ToPaginationResultsAsync(request, cancellationToken);

            var maskedResults = new PaginatedResponseDto<DistributionsOnHoldResponse>(request)
            {
                Total = paginatedResults.Total,
                Results = paginatedResults.Results.Select(d => new DistributionsOnHoldResponse
                {
                    Ssn = d.Ssn.MaskSsn(),
                    PayTo = d.PayTo,
                    CheckAmount = d.CheckAmount,
                }).ToList()
            };

            return Result<PaginatedResponseDto<DistributionsOnHoldResponse>>.Success(maskedResults);
        }, cancellationToken);
    }

    public Task<Result<PaginatedResponseDto<ManualChecksWrittenResponse>>> GetManualCheckDistributionsAsync(SortedPaginationRequestDto request, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var query = from dist in ctx.Distributions.Include(x => x.Payee)
                        where dist.StatusId == DistributionStatus.Constants.ManualCheck
                        select new
                        {
                            dist.Ssn,
                            PayTo = dist.Payee!.Name,
                            dist.CheckAmount,
                            dist.ManualCheckNumber,
                            dist.GrossAmount,
                            dist.FederalTaxAmount,
                            dist.StateTaxAmount,
                        };

            // Add query tagging for manual check distributions
            query = query.TagWith($"ManualCheckDistributions-{_timeProvider.GetUtcNow():yyyyMMddHHmm}");

            var paginatedResults = await query.ToPaginationResultsAsync(request, cancellationToken);

            var maskedResults = new PaginatedResponseDto<ManualChecksWrittenResponse>(request)
            {
                Total = paginatedResults.Total,
                Results = paginatedResults.Results.Select(d => new ManualChecksWrittenResponse
                {
                    Ssn = d.Ssn.MaskSsn(),
                    PayTo = d.PayTo,
                    CheckAmount = d.CheckAmount,
                    CheckNumber = d.ManualCheckNumber,
                    GrossAmount = d.GrossAmount,
                    FederalTaxAmount = d.FederalTaxAmount,
                    StateTaxAmount = d.StateTaxAmount,
                }).ToList()
            };

            return Result<PaginatedResponseDto<ManualChecksWrittenResponse>>.Success(maskedResults);
        }, cancellationToken);
    }

    public Task<Result<PaginatedResponseDto<DistributionRunReportDetail>>> GetDistributionRunReportAsync(DistributionRunReportRequest request, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographicQuery = await _demographicReaderService.BuildDemographicQueryAsync(ctx, false);
            var distributionQuery = GetDistributionExtract(ctx, request.DistributionFrequencies ?? Array.Empty<char>());
            var query = from dist in distributionQuery
                        join dem in demographicQuery.Include(x => x.PayClassification)
                                                    .Include(x => x.Address)
                                                    .Include(x => x.Department)
                                                    .Include(x => x.EmploymentType)
                                                    .Include(x => x.ContactInfo) on dist.Ssn equals dem.Ssn
                        select new DistributionRunReportDetail
                        {
                            BadgeNumber = dem.BadgeNumber,
                            DepartmentId = dem.DepartmentId,
                            DepartmentName = dem.Department!.Name,
                            PayClassificationId = dem.PayClassificationId,
                            PayClassificationName = dem.PayClassification!.Name,
                            StoreNumber = dem.StoreNumber,
                            TaxCodeId = dist.TaxCodeId,
                            TaxCodeName = dist.TaxCode!.Name,
                            EmployeeName = dem.ContactInfo!.FullName ?? "",
                            EmploymentTypeId = dem.EmploymentTypeId,
                            EmploymentTypeName = dem.EmploymentType!.Name,
                            HireDate = dem.HireDate,
                            FullTimeDate = dem.FullTimeDate,
                            DateOfBirth = dem.DateOfBirth,
                            Age = dem.DateOfBirth.Age(),
                            GrossAmount = dist.GrossAmount,
                            StateTaxAmount = dist.StateTaxAmount,
                            FederalTaxAmount = dist.FederalTaxAmount,
                            CheckAmount = dist.CheckAmount,
                            PayeeName = dist.Payee != null ? dist.Payee.Name : null,
                            PayeeAddress = dist.Payee != null ? dist.Payee.Address!.Street : null,
                            PayeeCity = dist.Payee != null ? dist.Payee.Address!.City : null,
                            PayeeState = dist.Payee != null ? dist.Payee.Address!.State : null,
                            PayeePostalCode = dist.Payee != null ? dist.Payee.Address!.PostalCode : null,
                            ThirdPartyPayeeName = dist.ThirdPartyPayee != null ? dist.ThirdPartyPayee.Name : null,
                            ThirdPartyPayeeAddress = dist.ThirdPartyPayee != null ? dist.ThirdPartyPayee.Address!.Street : null,
                            ThirdPartyPayeeCity = dist.ThirdPartyPayee != null ? dist.ThirdPartyPayee.Address!.City : null,
                            ThirdPartyPayeeState = dist.ThirdPartyPayee != null ? dist.ThirdPartyPayee.Address!.State : null,
                            ThirdPartyPayeePostalCode = dist.ThirdPartyPayee != null ? dist.ThirdPartyPayee.Address!.PostalCode : null,
                            IsDesceased = dist.IsDeceased,
                            ForTheBenefitOfAccountType = dist.ForTheBenefitOfAccountType,
                            ForTheBenefitOfPayee = dist.ForTheBenefitOfPayee,
                            Tax1099ForEmployee = dist.Tax1099ForEmployee,
                            Tax1099ForBeneficiary = dist.Tax1099ForBeneficiary
                        };

            // Add query tagging for distribution run report
            var frequencies = request.DistributionFrequencies?.Length > 0 ? string.Join(",", request.DistributionFrequencies) : "All";
            query = query.TagWith($"DistributionRunReport-Freq{frequencies}-{_timeProvider.GetUtcNow():yyyyMMddHHmm}");

            var paginatedResults = await query.ToPaginationResultsAsync(request, cancellationToken);
            return Result<PaginatedResponseDto<DistributionRunReportDetail>>.Success(paginatedResults);
        }, cancellationToken);
    }

    public Task<Result<PaginatedResponseDto<DisbursementReportDetailResponse>>> GetDisbursementReportAsync(ProfitYearRequest request, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
            var demographicQuery = await _demographicReaderService.BuildDemographicQueryAsync(ctx, false);
            var distributionQuery = GetDistributionExtract(ctx, new[] { DistributionFrequency.Constants.Annually, DistributionFrequency.Constants.Monthly, DistributionFrequency.Constants.Quarterly });
            var query = from dist in distributionQuery
                        join dem in demographicQuery on dist.Ssn equals dem.Ssn into demJoin
                        from dem in demJoin.DefaultIfEmpty()
                        join bc in ctx.BeneficiaryContacts on dist.Ssn equals bc.Ssn into benJoin
                        from ben in benJoin.DefaultIfEmpty()
                        join vb in _totalService.TotalVestingBalance(ctx, request.ProfitYear, calInfo.FiscalEndDate) on dist.Ssn equals vb.Ssn into vbJoin
                        from vest in vbJoin.DefaultIfEmpty()
                        select new DisbursementReportDetailResponse
                        {
                            DistributionFrequencyId = dist.FrequencyId,
                            DistributionFrequencyName = dist.Frequency!.Name,
                            Ssn = dist.Ssn.MaskSsn(),
                            BadgeNumber = dem != null ? dem.BadgeNumber : 0,
                            EmployeeName = dem.ContactInfo!.FullName ?? ben.ContactInfo.FullName ?? "?",
                            VestedBalance = vest != null ? vest.VestedBalance ?? 0 : 0,
                            OriginalAmount = dist.GrossAmount,
                            RemainingBalance = (vest != null ? vest.VestedBalance ?? 0 : 0) - dist.GrossAmount
                        };
            // Add query tagging for disbursement report
            query = query.TagWith($"DisbursementReport-{_timeProvider.GetUtcNow():yyyyMMddHHmm}");
            var paginatedResults = await query.ToPaginationResultsAsync(request, cancellationToken);
            return Result<PaginatedResponseDto<DisbursementReportDetailResponse>>.Success(paginatedResults);
        }, cancellationToken);
    }

    private IQueryable<Distribution> GetDistributionExtract(IProfitSharingDbContext ctx, char[] distributionFrequencies)
    {
        var distributionQuery = ctx.Distributions
            .Include(d => d.Frequency)
            .Include(d => d.Status)
            .Include(d => d.TaxCode)
            .Include(d => d.Payee)
            .Include(d => d.ThirdPartyPayee)
            .ThenInclude(tp => tp!.Address)
            .Where(x => x.StatusId != DistributionStatus.Constants.RequestOnHold && x.StatusId != DistributionStatus.Constants.ManualCheck)
            .Select(d => d);

        if (distributionFrequencies.Any())
        {
            distributionQuery = distributionQuery.Where(d => distributionFrequencies.Contains(d.FrequencyId));
        }

        // Add query tagging for distribution extract operations
        var frequencies = distributionFrequencies.Any() ? string.Join(",", distributionFrequencies) : "All";
        distributionQuery = distributionQuery.TagWith($"DistributionExtract-Freq{frequencies}-{_timeProvider.GetUtcNow():yyyyMMddHHmm}");

        return distributionQuery;
    }
}

/// <summary>
/// Internal DTO for distribution search query results.
/// Property names match the response DTO (PascalCase) for consistent sorting via ToPaginationResultsAsync.
/// </summary>
internal sealed class DistributionSearchQueryItem
{
    public long Id { get; init; }
    public byte PaymentSequence { get; init; }
    public int Ssn { get; init; }
    public long? BadgeNumber { get; init; }
    public string FullName { get; init; } = string.Empty;
    public char FrequencyId { get; init; }
    public string FrequencyName { get; init; } = string.Empty;
    public char StatusId { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public char TaxCodeId { get; init; }
    public string TaxCodeName { get; init; } = string.Empty;
    public decimal GrossAmount { get; init; }
    public decimal FederalTax { get; init; }
    public decimal StateTax { get; init; }
    public decimal CheckAmount { get; init; }
    public bool IsExecutive { get; init; }
    public int? DemographicId { get; init; }
    public int? BeneficiaryId { get; init; }
}
