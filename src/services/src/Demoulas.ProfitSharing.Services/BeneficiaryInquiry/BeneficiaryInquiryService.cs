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

    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IFrozenService _frozenService;
    private readonly ITotalService _totalService;
    private readonly IMasterInquiryService _masterInquiryService;

    public BeneficiaryInquiryService(IProfitSharingDataContextFactory dataContextFactory, IFrozenService frozenService, ITotalService totalService, IMasterInquiryService masterInquiryService)
    {
        _dataContextFactory = dataContextFactory;
        _frozenService = frozenService;
        _totalService = totalService;
        _masterInquiryService = masterInquiryService;
    }

    private async Task<IQueryable<BeneficiarySearchFilterResponse>> GetEmployeeQuery(BeneficiarySearchFilterRequest request, ProfitSharingReadOnlyDbContext context)
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

        });
        return member.Results.Select(x => new BeneficiarySearchFilterResponse()
        {
            Ssn = x.Ssn.ToString(),
            Age = x.DateOfBirth.Age(),
            BadgeNumber = x.BadgeNumber,
            City = x.AddressCity,
            Name = x.FullName,
            State = x.AddressState,
            Street = x.Address,
            Zip = x.AddressZipCode
        }).AsQueryable();

    }

    private IQueryable<BeneficiarySearchFilterResponse> GetBeneficiaryQuery(BeneficiarySearchFilterRequest request, ProfitSharingReadOnlyDbContext context)
    {
        var query = context.Beneficiaries.Include(x => x.Contact).ThenInclude(x => x!.Address).Include(x => x.Contact!.ContactInfo)
            .Where(x =>
            (request.BadgeNumber == null || x.BadgeNumber == request.BadgeNumber) &&
            (request.PsnSuffix == null || x.PsnSuffix == request.PsnSuffix) &&
            (string.IsNullOrEmpty(request.Name) || EF.Functions.Like(x.Contact!.ContactInfo!.FullName!.ToUpper(), $"%{request.Name.ToUpper()}%")) &&
            (request.Ssn == null || x.Contact!.Ssn == request.Ssn)
            ).Select(x => new BeneficiarySearchFilterResponse()
            {
                Ssn = x.Contact!.Ssn.ToString(),
                Age = DateTime.Now.Year - x.Contact!.DateOfBirth.Year,
                BadgeNumber = x.BadgeNumber,
                PsnSuffix = x.PsnSuffix,
                City = x.Contact.Address.City,
                Name = x.Contact!.ContactInfo!.FullName,
                State = x.Contact.Address.State,
                Street = x.Contact.Address.Street,
                Zip = x.Contact.Address.PostalCode
            });

        return query;

    }

    public async Task<PaginatedResponseDto<BeneficiarySearchFilterResponse>> BeneficiarySearchFilter(BeneficiarySearchFilterRequest request, CancellationToken cancellation)
    {
        var result = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            IQueryable<BeneficiarySearchFilterResponse> query;
            switch (request.MemberType)
            {
                case 1: //employee
                    query = await GetEmployeeQuery(request, context);
                    break;
                case 2: //beneficiaries
                    query = GetBeneficiaryQuery(request, context);
                    break;
                default:
                    query = GetBeneficiaryQuery(request, context);
                    break;
            }

            return query.ToPaginationResultsAsync(request, cancellation);
        }, cancellation);

        foreach (var item in result.Result.Results)
        {
            item.Ssn = !item.Ssn!.Contains("X") ? item.Ssn.MaskSsn() : item.Ssn;
        }

        return result.Result;
    }



    private int StepBackNumber(int num)
    {
        if (num <= 1000)
        {
            return 0;
        }

        // Get the hundreds and tens/ones parts
        int lastThree = num % 1000;

        if (lastThree == 0)
        {
            // e.g., 1200 -> 1000
            return (num / 1000) * 1000;
        }
        else if (lastThree % 100 == 0)
        {
            // e.g., 1100 -> 1000
            return (num / 1000) * 1000;
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
    private async Task<PaginatedResponseDto<BeneficiaryDto>> GetPreviousBeneficiaries(BeneficiaryRequestDto request, IQueryable<Beneficiary> query, CancellationToken cancellationToken)
    {
        List<BeneficiaryDto> res = new();
        int psnSuffix = request.PsnSuffix ?? 0;
        List<int> psns = new();
        if (psnSuffix > 1000)
        {
            int stepBack = StepBackNumber(psnSuffix);
            while (stepBack >= 1000)
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
            var result = query.Select(x => new BeneficiaryDto()
            {
                Id = x.Id,
                BadgeNumber = x.BadgeNumber,
                PsnSuffix = x.PsnSuffix,
                DemographicId = x.DemographicId,
                Percent = x.Percent,
                KindId = x.KindId,
                CreatedDate = x.Contact != null ? x.Contact.CreatedDate : DateOnly.MaxValue,
                DateOfBirth = x.Contact != null ? x.Contact.DateOfBirth : DateOnly.MaxValue,
                Ssn = x.Contact != null ? x.Contact.Ssn.ToString() : string.Empty,
                City = x.Contact != null && x.Contact.Address != null && x.Contact.Address.City != null ? x.Contact.Address.City : null,
                CountryIso = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.CountryIso ?? "" : "",
                PostalCode = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.PostalCode : null,
                State = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.State : null,
                Street = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.Street : "",
                Street2 = x.Contact != null && x.Contact.Address != null ? x.Contact.Address.Street2 : null,
                FirstName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.FirstName : "",
                LastName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.LastName : "",
                EmailAddress = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.EmailAddress : "",
                FullName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.FullName : "",
                MiddleName = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.MiddleName : null,
                MobileNumber = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.MobileNumber : "",
                PhoneNumber = x.Contact != null && x.Contact.ContactInfo != null ? x.Contact.ContactInfo.PhoneNumber : "",
                IsExecutive = false,
                Kind = new BeneficiaryKindDto()
                {
                    Id = x.Kind != null ? x.Kind.Id : BeneficiaryKind.Constants.Primary,
                    Name = x.Kind != null ? x.Kind.Name : null
                },
                Relationship = x.Relationship
            });
            return await result.ToPaginationResultsAsync(request, cancellationToken);
        }
        else
        {
            query = query.Where(x => x.PsnSuffix == psnSuffix);
            var result = query.Select(x => new BeneficiaryDto()
            {
                Id = x.Id,
                BadgeNumber = x.BadgeNumber,
                PsnSuffix = x.PsnSuffix,
                DemographicId = x.DemographicId,
                Percent = x.Percent,
                KindId = x.KindId,
                CreatedDate = x.Contact != null ? x.Contact.CreatedDate : DateOnly.MaxValue,
                DateOfBirth = x.Demographic != null ? x.Demographic.DateOfBirth : DateOnly.MaxValue,
                Ssn = x.Demographic != null && x.Demographic.Ssn != 0 ? x.Demographic.Ssn.ToString() : string.Empty,
                City = x.Demographic != null && x.Demographic.Address != null ? x.Demographic.Address.City : null,
                CountryIso = x.Demographic != null && x.Demographic.Address != null ? x.Demographic.Address.CountryIso ?? "" : "",
                PostalCode = x.Demographic != null && x.Demographic.Address != null ? x.Demographic.Address.PostalCode : null,
                State = x.Demographic != null && x.Demographic.Address != null ? x.Demographic.Address.State : null,
                Street = x.Demographic != null && x.Demographic.Address != null ? x.Demographic.Address.Street : "",
                Street2 = x.Demographic != null && x.Demographic.Address != null ? x.Demographic.Address.Street2 : null,
                FirstName = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.FirstName : "",
                LastName = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.LastName : "",
                EmailAddress = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.EmailAddress : "",
                FullName = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.FullName : "",
                MiddleName = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.MiddleName : null,
                MobileNumber = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.MobileNumber : "",
                PhoneNumber = x.Demographic != null && x.Demographic.ContactInfo != null ? x.Demographic.ContactInfo.PhoneNumber : "",
                Kind = new BeneficiaryKindDto()
                {
                    Id = x.Kind != null ? x.Kind.Id : BeneficiaryKind.Constants.Primary,
                    Name = x.Kind != null ? x.Kind.Name : null
                },
                Relationship = x.Relationship,
                IsExecutive = x.Demographic != null ? x.Demographic.PayFrequencyId == PayFrequency.Constants.Monthly : false,
            });
            return await result.ToPaginationResultsAsync(request, cancellationToken);
        }
    }
    public async Task<BeneficiaryResponse> GetBeneficiary(BeneficiaryRequestDto request, CancellationToken cancellationToken)
    {
        BeneficiaryResponse response = new BeneficiaryResponse();
        var frozenStateResponse = await _frozenService.GetActiveFrozenDemographic(cancellationToken);
        short yearEnd = frozenStateResponse.ProfitYear;



        var beneficiary = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var query = context.Beneficiaries.Include(x => x.Contact).ThenInclude(x => x!.ContactInfo).Include(x => x.Demographic).ThenInclude(x => x!.ContactInfo).Include(x => x.Demographic).ThenInclude(x => x!.Address)
            .Where(x => request.BadgeNumber == null || request.BadgeNumber == 0 || x.BadgeNumber == request.BadgeNumber);

            var prevBeneficiaries = await GetPreviousBeneficiaries(request, query, cancellationToken);

            // Check if user is querying from root (psnSuffix null or 0)
            if (request.PsnSuffix == null || request.PsnSuffix == 0)
            {
                // Top-level beneficiaries: 1000, 2000, etc.
                query = query.Where(x =>
                    x.PsnSuffix >= 1000 && x.PsnSuffix < 10000 && x.PsnSuffix % 1000 == 0);
            }
            else
            {

                // Determine range for children
                int lower = request.PsnSuffix.Value;
                int upper;

                // Determine level based on digit positions (works with base-10)
                if (lower % 1000 == 0)
                {
                    // Level 1: 1000, 2000, etc.
                    upper = lower + 1000;
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

            var result = query.Select(x => new BeneficiaryDto()
            {
                Id = x.Id,
                BadgeNumber = x.BadgeNumber,
                PsnSuffix = x.PsnSuffix,
                DemographicId = x.DemographicId,
                Percent = x.Percent,
                KindId = x.KindId,
                CreatedDate = x.Contact != null ? x.Contact.CreatedDate : DateOnly.MaxValue,
                DateOfBirth = x.Contact != null ? x.Contact.DateOfBirth : DateOnly.MaxValue,
                Ssn = x.Contact != null ? x.Contact.Ssn.ToString() : string.Empty,
                City = x.Contact != null ? x.Contact.Address.City : null,
                CountryIso = x.Contact != null ? x.Contact.Address.CountryIso ?? "" : "",
                PostalCode = x.Contact != null ? x.Contact.Address.PostalCode : null,
                State = x.Contact != null ? x.Contact.Address.State : null,
                Street = x.Contact != null ? x.Contact.Address.Street : "",
                Street2 = x.Contact != null ? x.Contact.Address.Street2 : null,
                FirstName = x.Contact != null ? x.Contact.ContactInfo.FirstName : "",
                LastName = x.Contact != null ? x.Contact.ContactInfo.LastName : "",
                EmailAddress = x.Contact != null ? x.Contact.ContactInfo.EmailAddress : "",
                FullName = x.Contact != null ? x.Contact.ContactInfo.FullName : "",
                MiddleName = x.Contact != null ? x.Contact.ContactInfo.MiddleName : null,
                MobileNumber = x.Contact != null ? x.Contact.ContactInfo.MobileNumber : "",
                PhoneNumber = x.Contact != null ? x.Contact.ContactInfo.PhoneNumber : "",
                IsExecutive = x.Demographic != null ? x.Demographic!.PayFrequencyId == PayFrequency.Constants.Monthly : false,
                Kind = new BeneficiaryKindDto()
                {
                    Id = x.Kind != null ? x.Kind.Id : BeneficiaryKind.Constants.Primary,
                    Name = x.Kind != null ? x.Kind.Name : null
                },
                Relationship = x.Relationship
            });
            PaginatedResponseDto<BeneficiaryDto> final = await result.ToPaginationResultsAsync(request, cancellationToken);
            return new BeneficiaryResponse() { Beneficiaries = final, BeneficiaryOf = prevBeneficiaries };
        }
, cancellationToken);
        //setting Current balance
        if (beneficiary.Beneficiaries?.Results != null)
        {
            ISet<int> ssnList = new HashSet<int>(beneficiary.Beneficiaries.Results.Select(x => Convert.ToInt32(x.Ssn)).ToList());
            var balanceList = await _totalService.GetVestingBalanceForMembersAsync(SearchBy.Ssn, ssnList, yearEnd, cancellationToken);
            foreach (var item in beneficiary.Beneficiaries.Results)
            {
                item.CurrentBalance = balanceList.Where(x => x.Id.ToString() == item.Ssn).Select(x => x.CurrentBalance).FirstOrDefault();
                item.Ssn = item.Ssn!.MaskSsn();
            }
        }

        //setting Current balance
        if (beneficiary.BeneficiaryOf?.Results != null)
        {
            ISet<int> ssnListBO = new HashSet<int>(beneficiary.BeneficiaryOf.Results.Select(x => Convert.ToInt32(x.Ssn)).ToList());
            var balanceListBO = await _totalService.GetVestingBalanceForMembersAsync(SearchBy.Ssn, ssnListBO, yearEnd, cancellationToken);
            foreach (var item in beneficiary.BeneficiaryOf.Results)
            {
                item.CurrentBalance = balanceListBO.Where(x => x.Id.ToString() == item.Ssn).Select(x => x.CurrentBalance).FirstOrDefault();
                item.Ssn = item.Ssn!.MaskSsn();
            }
        }
        return beneficiary;
    }


    public async Task<BeneficiaryDetailResponse> GetBeneficiaryDetail(BeneficiaryDetailRequest request, CancellationToken cancellationToken)
    {
        var frozenStateResponse = await _frozenService.GetActiveFrozenDemographic(cancellationToken);
        short yearEnd = frozenStateResponse.ProfitYear;
        var result = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            IQueryable<BeneficiaryDetailResponse> query;
            if (request.PsnSuffix.HasValue && request.PsnSuffix > 0)
            {
                query = context.Beneficiaries.Include(x => x.Contact).ThenInclude(x => x!.Address).Include(x => x.Contact!.ContactInfo)
                .Where(x => x.BadgeNumber == request.BadgeNumber && x.PsnSuffix == request.PsnSuffix)
                .Select(x => new BeneficiaryDetailResponse
                {
                    Name = x.Contact!.ContactInfo!.FullName,
                    BadgeNumber = x.BadgeNumber,
                    City = x.Contact.Address.City,
                    DateOfBirth = x.Contact.DateOfBirth,
                    PsnSuffix = x.PsnSuffix,
                    Ssn = x.Contact!.Ssn.ToString(),
                    State = x.Contact.Address.State,
                    Street = x.Contact.Address.Street,
                    Zip = x.Contact.Address.PostalCode,
                    IsExecutive = false,
                });
            }
            else
            {
                var memberDetail = await _masterInquiryService.GetMembersAsync(new Common.Contracts.Request.MasterInquiry.MasterInquiryRequest
                {
                    BadgeNumber = request.BadgeNumber,
                    EndProfitYear = (short)DateTime.Now.Year,
                    ProfitYear = (short)DateTime.Now.Year,
                    MemberType = 1
                });
                query = memberDetail.Results.Select(x => new BeneficiaryDetailResponse
                {
                    Name = x.FullName,
                    BadgeNumber = x.BadgeNumber,
                    City = x.AddressCity,
                    DateOfBirth = x.DateOfBirth,
                    Ssn = x.Ssn.ToString(),
                    State = x.AddressState,
                    Street = x.Address,
                    Zip = x.AddressZipCode,
                    IsExecutive = x.IsExecutive,
                }).AsQueryable();
            }

            return query.ToList();
        }, cancellationToken);


        ISet<int> badgeNumbers = new HashSet<int>(result.Select(x => x.BadgeNumber).ToList());
        var balanceList = await _totalService.GetVestingBalanceForMembersAsync(SearchBy.BadgeNumber, badgeNumbers, yearEnd, cancellationToken);
        foreach (var item in result)
        {
            item.CurrentBalance = balanceList.Select(x => x.CurrentBalance).FirstOrDefault();
            if (request.PsnSuffix.HasValue && request.PsnSuffix > 0)
            {
                item.Ssn = item.Ssn!.MaskSsn();
            }
        }


        return result.FirstOrDefault()!;
    }

    public async Task<BeneficiaryTypesResponseDto> GetBeneficiaryTypes(BeneficiaryTypesRequestDto beneficiaryTypesRequestDto, CancellationToken cancellation)
    {
        var result = await _dataContextFactory.UseReadOnlyContext(context =>
        {
            return context.BeneficiaryTypes.Select(x => new BeneficiaryTypeDto { Id = x.Id, Name = x.Name }).ToListAsync(cancellation);
        }, cancellation);

        return new BeneficiaryTypesResponseDto() { BeneficiaryTypeList = result };
    }


    public async Task<BeneficiaryKindResponseDto> GetBeneficiaryKind(BeneficiaryKindRequestDto beneficiaryKindRequestDto, CancellationToken cancellation)
    {
        var result = await _dataContextFactory.UseReadOnlyContext(context =>
        {
            return context.BeneficiaryKinds.Select(x => new BeneficiaryKindDto { Id = x.Id, Name = x.Name }).ToListAsync(cancellation);
        }, cancellation);

        return new BeneficiaryKindResponseDto() { BeneficiaryKindList = result };
    }
}
