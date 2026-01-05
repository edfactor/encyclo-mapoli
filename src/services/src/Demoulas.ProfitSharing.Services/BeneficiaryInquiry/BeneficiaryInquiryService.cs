using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.BeneficiaryInquiry;

public class BeneficiaryInquiryService : IBeneficiaryInquiryService
{
    // Magic number constants
    private const int EmployeeMemberType = 1;
    private const int PsnSuffixRoot = 1000;
    private const int PsnSuffixRootMax = 10000;

    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IFrozenService _frozenService;
    private readonly ITotalService _totalService;
    private readonly IMasterInquiryService _masterInquiryService;

    private sealed record BeneficiarySearchFilterRow
    {
        public int BadgeNumber { get; init; }
        public short PsnSuffix { get; init; }
        public string? FullName { get; init; }
        public int Ssn { get; init; }
        public string? Street { get; init; }
        public string? City { get; init; }
        public string? State { get; init; }
        public string? Zip { get; init; }
        public short? Age { get; init; }
    }

    private sealed record BeneficiaryRow
    {
        public int Id { get; init; }
        public int BadgeNumber { get; init; }
        public short PsnSuffix { get; init; }
        public int DemographicId { get; init; }
        public decimal Percent { get; init; }
        public DateOnly CreatedDate { get; init; }
        public DateOnly DateOfBirth { get; init; }
        public int Ssn { get; init; }
        public string? City { get; init; }
        public string CountryIso { get; init; } = string.Empty;
        public string? PostalCode { get; init; }
        public string? State { get; init; }
        public string Street { get; init; } = string.Empty;
        public string? Street2 { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string EmailAddress { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string? MiddleName { get; init; }
        public string MobileNumber { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public bool IsExecutive { get; init; }
        public string? Relationship { get; init; }
    }

    public BeneficiaryInquiryService(IProfitSharingDataContextFactory dataContextFactory, IFrozenService frozenService, ITotalService totalService, IMasterInquiryService masterInquiryService)
    {
        _dataContextFactory = dataContextFactory;
        _frozenService = frozenService;
        _totalService = totalService;
        _masterInquiryService = masterInquiryService;
    }

    private async Task<PaginatedResponseDto<BeneficiarySearchFilterResponse>> GetEmployeeQueryPaginated(BeneficiarySearchFilterRequest request, CancellationToken cancellationToken)
    {
        var member = await _masterInquiryService.GetMembersAsync(new Common.Contracts.Request.MasterInquiry.MasterInquiryRequest()
        {
            BadgeNumber = request.BadgeNumber,
            PsnSuffix = request.PsnSuffix,
            Name = request.Name,
            Ssn = request.Ssn != null ? Convert.ToInt32(request.Ssn) : 0,
            MemberType = request.MemberType,
            EndProfitYear = (short)DateTime.Now.Year,
            ProfitYear = (short)DateTime.Now.Year

        }, cancellationToken);

        var mapped = member.Results.Select(x => new BeneficiarySearchFilterResponse()
        {
            Ssn = x.Ssn.ToString().MaskSsn(),
            Age = x.DateOfBirth.Age(),
            BadgeNumber = x.BadgeNumber,
            PsnSuffix = 0, // Employees don't have PsnSuffix
            City = x.AddressCity,
            FullName = x.FullName,
            State = x.AddressState,
            Street = x.Address,
            Zip = x.AddressZipCode
        }).AsQueryable();

        // Use ToPaginationResultsAsync with in-memory queryable
        return await mapped.ToPaginationResultsAsync(request, cancellationToken);
    }

    private IQueryable<BeneficiarySearchFilterRow> GetBeneficiaryQuery(BeneficiarySearchFilterRequest request, ProfitSharingReadOnlyDbContext context)
    {
        var query = context.Beneficiaries.Include(x => x.Contact).ThenInclude(x => x!.Address).Include(x => x.Contact!.ContactInfo)
            .Where(x =>
            (request.BadgeNumber == null || x.BadgeNumber == request.BadgeNumber) &&
            (request.PsnSuffix == null || x.PsnSuffix == request.PsnSuffix) &&
            (string.IsNullOrEmpty(request.Name) || EF.Functions.Like(x.Contact!.ContactInfo!.FullName!.ToUpper(), $"%{request.Name.ToUpper()}%")) &&
            (request.Ssn == null || x.Contact!.Ssn == request.Ssn)
            ).Select(x => new BeneficiarySearchFilterRow
            {
                Ssn = x.Contact!.Ssn,
                Age = x.Contact!.DateOfBirth.Age(),
                BadgeNumber = x.BadgeNumber,
                PsnSuffix = x.PsnSuffix,
                City = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.City : null,
                FullName = x.Contact!.ContactInfo!.FullName,
                State = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.State : null,
                Street = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.Street : null,
                Zip = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.PostalCode : null
            });

        return query;

    }

    /// <summary>
    /// Retrieves beneficiary information filtered by request criteria.
    /// </summary>
    /// <param name="request">The beneficiary search filter request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated beneficiary results.</returns>
    public async Task<PaginatedResponseDto<BeneficiarySearchFilterResponse>> BeneficiarySearchFilter(BeneficiarySearchFilterRequest request, CancellationToken cancellationToken)
    {

        /*
         Note: Just call Master Inquiry Service and transform according to your response.
         */
        PaginatedResponseDto<BeneficiarySearchFilterResponse> result;

        if (request.MemberType == 0) // All - search both employees and beneficiaries
        {
            // Create a request without pagination to get all results first
            var unpaginatedRequest = request with { Skip = 0, Take = int.MaxValue };
            
            // Get all employee results
            var employeeResults = await GetEmployeeQueryPaginated(unpaginatedRequest, cancellationToken);
            
            // Get all beneficiary results
            var beneficiaryRows = await _dataContextFactory.UseReadOnlyContext(async context =>
            {
                var query = GetBeneficiaryQuery(request, context);
                return await query.ToPaginationResultsAsync(unpaginatedRequest, cancellationToken);
            }, cancellationToken);
            
            var beneficiaryResults = beneficiaryRows.Results?.Select(r => new BeneficiarySearchFilterResponse
            {
                BadgeNumber = r.BadgeNumber,
                PsnSuffix = r.PsnSuffix,
                FullName = r.FullName,
                Ssn = r.Ssn != 0 ? r.Ssn.ToString().MaskSsn() : string.Empty.MaskSsn(),
                Street = r.Street,
                City = r.City,
                State = r.State,
                Zip = r.Zip,
                Age = r.Age,
            }).ToList() ?? [];
            
            // Combine results from both sources
            var combinedResults = (employeeResults.Results ?? []).Concat(beneficiaryResults).ToList();
            
            // Apply pagination manually to the combined results
            var pagedResults = combinedResults
                .Skip(request.Skip ?? 0)
                .Take(request.Take ?? 50)
                .ToList();
            
            result = new PaginatedResponseDto<BeneficiarySearchFilterResponse>
            {
                IsPartialResult = employeeResults.IsPartialResult || beneficiaryRows.IsPartialResult,
                ResultHash = string.Empty,
                TimeoutOccurred = employeeResults.TimeoutOccurred || beneficiaryRows.TimeoutOccurred,
                Total = combinedResults.Count,
                Results = pagedResults
            };
        }
        else if (request.MemberType == EmployeeMemberType)
        {
            // Employee query is already in-memory, handle pagination separately
            result = await GetEmployeeQueryPaginated(request, cancellationToken);
        }
        else
        {
            // Beneficiary query uses database, use proper async pagination
            var rows = await _dataContextFactory.UseReadOnlyContext(async context =>
            {
                var query = GetBeneficiaryQuery(request, context);
                // IMPORTANT: await the pagination while the DbContext (and its connection) is still alive.
                return await query.ToPaginationResultsAsync(request, cancellationToken);
            }, cancellationToken);

            result = new PaginatedResponseDto<BeneficiarySearchFilterResponse>
            {
                IsPartialResult = rows.IsPartialResult,
                ResultHash = rows.ResultHash,
                TimeoutOccurred = rows.TimeoutOccurred,
                Total = rows.Total,
                Results = rows.Results is null
                    ? []
                    : rows.Results.Select(r => new BeneficiarySearchFilterResponse
                    {
                        BadgeNumber = r.BadgeNumber,
                        PsnSuffix = r.PsnSuffix,
                        FullName = r.FullName,
                        Ssn = r.Ssn != 0 ? r.Ssn.ToString().MaskSsn() : string.Empty.MaskSsn(),
                        Street = r.Street,
                        City = r.City,
                        State = r.State,
                        Zip = r.Zip,
                        Age = r.Age,
                    }).ToList(),
            };
        }

        return result;
    }

    private static int StepBackNumber(int num)
    {
        if (num <= PsnSuffixRoot)
        {
            return 0;
        }

        // Get the hundreds and tens/ones parts
        int lastThree = num % PsnSuffixRoot;

        if (lastThree == 0)
        {
            // e.g., 1200 -> 1000
            return (num / PsnSuffixRoot) * PsnSuffixRoot;
        }
        else if (lastThree % 100 == 0)
        {
            // e.g., 1100 -> 1000
            return (num / PsnSuffixRoot) * PsnSuffixRoot;
        }
        else if (lastThree % 10 == 0)
        {
            // e.g., 1120 -> 1100, 1210 -> 1200
            return (num / 100) * 100;
        }
        else
        {
            // e.g., if we ever had 1125 -> 1120 (not in your examples but just in case)
            return (num / 10) * 10;
        }
    }

    private Task<PaginatedResponseDto<BeneficiaryRow>> GetPreviousBeneficiaries(BeneficiaryRequestDto request, IQueryable<Beneficiary> query, CancellationToken cancellationToken)
    {
        int psnSuffix = request.PsnSuffix ?? 0;
        List<int> psns = new();
        if (psnSuffix > PsnSuffixRoot)
        {
            int stepBack = StepBackNumber(psnSuffix);
            while (stepBack >= PsnSuffixRoot)
            {
                psns.Add(stepBack);

                int next = StepBackNumber(stepBack);
                if (next == stepBack)
                {
                    break; // safety to prevent infinite loop
                }

                stepBack = StepBackNumber(stepBack);
            }
            query = query.Where(x => psns.Contains(x.PsnSuffix)).OrderByDescending(x => x.PsnSuffix);

            var result = query.Select(x => new BeneficiaryRow
            {
                Id = x.Id,
                BadgeNumber = x.BadgeNumber,
                PsnSuffix = x.PsnSuffix,
                DemographicId = x.DemographicId,
                Percent = x.Percent,
                CreatedDate = x.Contact != null ? x.Contact.CreatedDate : DateOnly.MaxValue,
                DateOfBirth = x.Contact != null ? x.Contact.DateOfBirth : DateOnly.MaxValue,
                Ssn = x.Contact != null ? x.Contact.Ssn : 0,
                City = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.City : null,
                CountryIso = x.Contact != null && x.Contact.Address != null && x.Contact.Address.CountryIso != null ? x.Contact.Address.CountryIso : "",
                PostalCode = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.PostalCode : null,
                State = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.State : null,
                Street = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.Street ?? "" : "",
                Street2 = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.Street2 : null,
                FirstName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.FirstName ?? "" : "",
                LastName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.LastName ?? "" : "",
                EmailAddress = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.EmailAddress ?? "" : "",
                FullName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.FullName ?? "" : "",
                MiddleName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.MiddleName : null,
                MobileNumber = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.MobileNumber ?? "" : "",
                PhoneNumber = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.PhoneNumber ?? "" : "",
                IsExecutive = false,
                Relationship = x.Relationship
            });

            return result.ToPaginationResultsAsync(request, cancellationToken);
        }
        else
        {
            query = query.Where(x => x.PsnSuffix == psnSuffix);

            var result = query.Select(x => new BeneficiaryRow
            {
                Id = x.Id,
                BadgeNumber = x.BadgeNumber,
                PsnSuffix = x.PsnSuffix,
                DemographicId = x.DemographicId,
                Percent = x.Percent,
                CreatedDate = x.Contact != null ? x.Contact.CreatedDate : DateOnly.MaxValue,
                DateOfBirth = x.Demographic != null ? x.Demographic.DateOfBirth : DateOnly.MaxValue,
                Ssn = x.Demographic != null ? x.Demographic.Ssn : 0,
                City = x.Demographic != null && x.Demographic.Address != null ? x.Demographic.Address.City : null,
                CountryIso = x.Demographic != null && x.Demographic.Address != null && x.Demographic.Address.CountryIso != null ? x.Demographic.Address.CountryIso : "",
                PostalCode = x.Demographic != null && x.Demographic.Address != null ? x.Demographic.Address.PostalCode : null,
                State = x.Demographic != null && x.Demographic.Address != null ? x.Demographic.Address.State : null,
                Street = x.Demographic != null && x.Demographic.Address != null ? x.Demographic.Address.Street ?? "" : "",
                Street2 = x.Demographic != null && x.Demographic.Address != null ? x.Demographic.Address.Street2 : null,
                FirstName = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.FirstName ?? "" : "",
                LastName = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.LastName ?? "" : "",
                EmailAddress = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.EmailAddress ?? "" : "",
                FullName = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.FullName ?? "" : "",
                MiddleName = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.MiddleName : null,
                MobileNumber = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.MobileNumber ?? "" : "",
                PhoneNumber = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.PhoneNumber ?? "" : "",
                Relationship = x.Relationship,
                IsExecutive = x.Demographic != null && x.Demographic.PayFrequencyId == PayFrequency.Constants.Monthly,
            });

            return result.ToPaginationResultsAsync(request, cancellationToken);
        }
    }

    /// <summary>
    /// Retrieves beneficiary information with hierarchical structure and balance information.
    /// </summary>
    /// <param name="request">The beneficiary request containing badge number and PSN suffix.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Beneficiary response with current beneficiaries and beneficiary-of relationships.</returns>
    public async Task<BeneficiaryResponse> GetBeneficiary(BeneficiaryRequestDto request, CancellationToken cancellationToken)
    {
        var frozenStateResponse = await _frozenService.GetActiveFrozenDemographic(cancellationToken);
        short yearEnd = frozenStateResponse.ProfitYear;

        var beneficiaryRows = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var query = context.Beneficiaries.Include(x => x.Contact).ThenInclude(x => x!.ContactInfo).Include(x => x.Demographic).ThenInclude(x => x!.ContactInfo).Include(x => x.Demographic).ThenInclude(x => x!.Address)
                .Where(x => request.BadgeNumber == null || request.BadgeNumber == 0 || x.BadgeNumber == request.BadgeNumber);

            var prevBeneficiaries = await GetPreviousBeneficiaries(request, query, cancellationToken);

            // Check if user is querying from root (psnSuffix null or 0)
            if (request.PsnSuffix == null || request.PsnSuffix == 0)
            {
                // Top-level beneficiaries: 1000, 2000, etc.
                query = query.Where(x =>
                    x.PsnSuffix >= PsnSuffixRoot && x.PsnSuffix < PsnSuffixRootMax && x.PsnSuffix % PsnSuffixRoot == 0);
            }
            else
            {

                // Determine range for children
                int lower = request.PsnSuffix.Value;
                int upper;

                // Determine level based on digit positions (works with base-10)
                if (lower % PsnSuffixRoot == 0)
                {
                    // Level 1: 1000, 2000, etc.
                    upper = lower + PsnSuffixRoot;
                }
                else if (lower % 100 == 0)
                {
                    // Level 2: 1100, 1200, etc.
                    upper = lower + 100;
                }
                else if (lower % 10 == 0)
                {
                    // Level 3: 1210, 1220, etc.
                    upper = lower + 10;
                }
                else
                {
                    // Level 4 (leaf): 1211, etc. — no children
                    upper = lower; // no children expected
                }

                // Fetch direct children or deeper if desired
                query = query.Where(x =>
                    x.PsnSuffix > lower && x.PsnSuffix < upper);
            }

            var result = query.Select(x => new BeneficiaryRow
            {
                Id = x.Id,
                BadgeNumber = x.BadgeNumber,
                PsnSuffix = x.PsnSuffix,
                DemographicId = x.DemographicId,
                Percent = x.Percent,
                CreatedDate = x.Contact != null ? x.Contact.CreatedDate : DateOnly.MaxValue,
                DateOfBirth = x.Contact != null ? x.Contact.DateOfBirth : DateOnly.MaxValue,
                Ssn = x.Contact != null ? x.Contact.Ssn : 0,
                City = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.City : null,
                CountryIso = x.Contact != null && x.Contact.Address != null && x.Contact.Address.CountryIso != null ? x.Contact.Address.CountryIso : "",
                PostalCode = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.PostalCode : null,
                State = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.State : null,
                Street = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.Street ?? "" : "",
                Street2 = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.Street2 : null,
                FirstName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.FirstName ?? "" : "",
                LastName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.LastName ?? "" : "",
                EmailAddress = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.EmailAddress ?? "" : "",
                FullName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.FullName ?? "" : "",
                MiddleName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.MiddleName : null,
                MobileNumber = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.MobileNumber ?? "" : "",
                PhoneNumber = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.PhoneNumber ?? "" : "",
                IsExecutive = x.Demographic != null && x.Demographic.PayFrequencyId == PayFrequency.Constants.Monthly,
                Relationship = x.Relationship
            });

            PaginatedResponseDto<BeneficiaryRow> final = await result.ToPaginationResultsAsync(request, cancellationToken);
            return new BeneficiaryResponseRows { Beneficiaries = final, BeneficiaryOf = prevBeneficiaries };
        }
, cancellationToken);

        PaginatedResponseDto<BeneficiaryDto>? beneficiaries = null;
        if (beneficiaryRows.Beneficiaries is not null)
        {
            var ssnList = new HashSet<int>(beneficiaryRows.Beneficiaries.Results?.Select(r => r.Ssn).Where(ssn => ssn != 0) ?? []);
            var balanceList = ssnList.Count > 0
                ? await _totalService.GetVestingBalanceForMembersAsync(SearchBy.Ssn, ssnList, yearEnd, cancellationToken)
                : [];
            var balanceLookup = balanceList.ToDictionary(x => x.Id);

            beneficiaries = new PaginatedResponseDto<BeneficiaryDto>
            {
                IsPartialResult = beneficiaryRows.Beneficiaries.IsPartialResult,
                ResultHash = beneficiaryRows.Beneficiaries.ResultHash,
                TimeoutOccurred = beneficiaryRows.Beneficiaries.TimeoutOccurred,
                Total = beneficiaryRows.Beneficiaries.Total,
                Results = beneficiaryRows.Beneficiaries.Results is null
                    ? []
                    : beneficiaryRows.Beneficiaries.Results.Select(r =>
                    {
                        balanceLookup.TryGetValue(r.Ssn, out var balance);
                        return new BeneficiaryDto
                        {
                            Id = r.Id,
                            BadgeNumber = r.BadgeNumber,
                            PsnSuffix = r.PsnSuffix,
                            DemographicId = r.DemographicId,
                            Percent = r.Percent,
                            CreatedDate = r.CreatedDate,
                            DateOfBirth = r.DateOfBirth,
                            Ssn = r.Ssn != 0 ? r.Ssn.ToString().MaskSsn() : string.Empty.MaskSsn(),
                            City = r.City,
                            CountryIso = r.CountryIso,
                            PostalCode = r.PostalCode,
                            State = r.State,
                            Street = r.Street,
                            Street2 = r.Street2,
                            FirstName = r.FirstName,
                            LastName = r.LastName,
                            EmailAddress = r.EmailAddress,
                            FullName = r.FullName,
                            MiddleName = r.MiddleName,
                            MobileNumber = r.MobileNumber,
                            PhoneNumber = r.PhoneNumber,
                            IsExecutive = r.IsExecutive,
                            Relationship = r.Relationship,
                            CurrentBalance = balance?.CurrentBalance,
                        };
                    }).ToList(),
            };
        }

        PaginatedResponseDto<BeneficiaryDto>? beneficiaryOf = null;
        if (beneficiaryRows.BeneficiaryOf is not null)
        {
            var ssnListBO = new HashSet<int>(beneficiaryRows.BeneficiaryOf.Results?.Select(r => r.Ssn).Where(ssn => ssn != 0) ?? []);
            var balanceListBO = ssnListBO.Count > 0
                ? await _totalService.GetVestingBalanceForMembersAsync(SearchBy.Ssn, ssnListBO, yearEnd, cancellationToken)
                : [];
            var balanceLookupBO = balanceListBO.ToDictionary(x => x.Id);

            beneficiaryOf = new PaginatedResponseDto<BeneficiaryDto>
            {
                IsPartialResult = beneficiaryRows.BeneficiaryOf.IsPartialResult,
                ResultHash = beneficiaryRows.BeneficiaryOf.ResultHash,
                TimeoutOccurred = beneficiaryRows.BeneficiaryOf.TimeoutOccurred,
                Total = beneficiaryRows.BeneficiaryOf.Total,
                Results = beneficiaryRows.BeneficiaryOf.Results is null
                    ? []
                    : beneficiaryRows.BeneficiaryOf.Results.Select(r =>
                    {
                        balanceLookupBO.TryGetValue(r.Ssn, out var balance);
                        return new BeneficiaryDto
                        {
                            Id = r.Id,
                            BadgeNumber = r.BadgeNumber,
                            PsnSuffix = r.PsnSuffix,
                            DemographicId = r.DemographicId,
                            Percent = r.Percent,
                            CreatedDate = r.CreatedDate,
                            DateOfBirth = r.DateOfBirth,
                            Ssn = r.Ssn != 0 ? r.Ssn.ToString().MaskSsn() : string.Empty.MaskSsn(),
                            City = r.City,
                            CountryIso = r.CountryIso,
                            PostalCode = r.PostalCode,
                            State = r.State,
                            Street = r.Street,
                            Street2 = r.Street2,
                            FirstName = r.FirstName,
                            LastName = r.LastName,
                            EmailAddress = r.EmailAddress,
                            FullName = r.FullName,
                            MiddleName = r.MiddleName,
                            MobileNumber = r.MobileNumber,
                            PhoneNumber = r.PhoneNumber,
                            IsExecutive = r.IsExecutive,
                            Relationship = r.Relationship,
                            CurrentBalance = balance?.CurrentBalance,
                        };
                    }).ToList(),
            };
        }

        return new BeneficiaryResponse { Beneficiaries = beneficiaries, BeneficiaryOf = beneficiaryOf };
    }

    private sealed record BeneficiaryResponseRows
    {
        public required PaginatedResponseDto<BeneficiaryRow> Beneficiaries { get; init; }
        public required PaginatedResponseDto<BeneficiaryRow> BeneficiaryOf { get; init; }
    }


    /// <summary>
    /// Retrieves detailed beneficiary information for a specific badge number and optional PSN suffix.
    /// </summary>
    /// <param name="request">The beneficiary detail request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Detailed beneficiary information including current balance.</returns>
    public async Task<BeneficiaryDetailResponse> GetBeneficiaryDetail(BeneficiaryDetailRequest request, CancellationToken cancellationToken)
    {
        var frozenStateResponse = await _frozenService.GetActiveFrozenDemographic(cancellationToken);
        short yearEnd = frozenStateResponse.ProfitYear;

        List<BeneficiaryDetailResponse> result;

        if (request.PsnSuffix.HasValue && request.PsnSuffix > 0)
        {
            // Query from database - use async
            var rows = await _dataContextFactory.UseReadOnlyContext(async context =>
            {
                return await context.Beneficiaries
                    .Where(x => x.BadgeNumber == request.BadgeNumber && x.PsnSuffix == request.PsnSuffix)
                    .Select(x => new
                    {
                        FullName = x.Contact!.ContactInfo!.FullName ?? string.Empty,
                        BadgeNumber = x.BadgeNumber,
                        City = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.City : null,
                        DateOfBirth = x.Contact!.DateOfBirth,
                        PsnSuffix = x.PsnSuffix,
                        Ssn = x.Contact!.Ssn,
                        State = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.State : null,
                        Street = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.Street : null,
                        Zip = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.PostalCode : null,
                        IsExecutive = false,
                    })
                    .ToListAsync(cancellationToken);
            }, cancellationToken);

            result = rows.Select(r => new BeneficiaryDetailResponse
            {
                FullName = r.FullName,
                BadgeNumber = r.BadgeNumber,
                City = r.City,
                DateOfBirth = r.DateOfBirth,
                PsnSuffix = r.PsnSuffix,
                Ssn = r.Ssn != 0 ? r.Ssn.ToString().MaskSsn() : string.Empty.MaskSsn(),
                State = r.State,
                Street = r.Street,
                Zip = r.Zip,
                IsExecutive = r.IsExecutive,
            }).ToList();
        }
        else
        {
            // Query from in-memory service result - use sync
            var memberDetail = await _masterInquiryService.GetMembersAsync(new Common.Contracts.Request.MasterInquiry.MasterInquiryRequest
            {
                BadgeNumber = request.BadgeNumber,
                EndProfitYear = (short)DateTime.Now.Year,
                ProfitYear = (short)DateTime.Now.Year,
                MemberType = EmployeeMemberType
            }, cancellationToken);

            result = memberDetail.Results.Select(x => new BeneficiaryDetailResponse
            {
                FullName = x.FullName,
                BadgeNumber = x.BadgeNumber,
                City = x.AddressCity,
                DateOfBirth = x.DateOfBirth,
                Ssn = x.Ssn.ToString().MaskSsn(),
                State = x.AddressState,
                Street = x.Address,
                Zip = x.AddressZipCode,
                IsExecutive = x.IsExecutive,
            }).ToList(); // Use .ToList() for in-memory collection
        }


        ISet<int> badgeNumbers = new HashSet<int>(result.Select(x => x.BadgeNumber).ToList());
        var balanceList = await _totalService.GetVestingBalanceForMembersAsync(SearchBy.BadgeNumber, badgeNumbers, yearEnd, cancellationToken);
        foreach (var item in result)
        {
            item.CurrentBalance = balanceList.Select(x => x.CurrentBalance).FirstOrDefault();
            item.Ssn = item.Ssn is not null ? item.Ssn.MaskSsn() : string.Empty.MaskSsn();
        }


        return result.FirstOrDefault()!;
    }

    /// <summary>
    /// Retrieves all available beneficiary types.
    /// </summary>
    /// <param name="beneficiaryTypesRequestDto">The beneficiary types request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Response containing list of beneficiary types.</returns>
    public async Task<BeneficiaryTypesResponseDto> GetBeneficiaryTypes(BeneficiaryTypesRequestDto beneficiaryTypesRequestDto, CancellationToken cancellationToken)
    {
        var result = await _dataContextFactory.UseReadOnlyContext(context =>
        {
            return context.BeneficiaryTypes.Select(x => new BeneficiaryTypeDto { Id = x.Id, Name = x.Name }).ToListAsync(cancellationToken);
        }, cancellationToken);

        return new BeneficiaryTypesResponseDto() { BeneficiaryTypeList = result };
    }


}
