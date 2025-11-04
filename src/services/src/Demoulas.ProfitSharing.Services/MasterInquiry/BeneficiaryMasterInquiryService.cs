using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.MasterInquiry;

/// <summary>
/// Service for beneficiary-specific master inquiry operations.
/// Handles lookup, filtering, and retrieval of beneficiary profit sharing data.
/// Extracted from MasterInquiryService for better separation of concerns.
/// </summary>
public sealed class BeneficiaryMasterInquiryService : IBeneficiaryMasterInquiryService
{
    private readonly IProfitSharingDataContextFactory _factory;

    public BeneficiaryMasterInquiryService(IProfitSharingDataContextFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Builds the query for beneficiary inquiry with optional filtering.
    /// </summary>
    public async Task<IQueryable<MasterInquiryItem>> GetBeneficiaryInquiryQueryAsync(
        MasterInquiryRequest? req = null,
        CancellationToken cancellationToken = default)
    {
        return await _factory.UseReadOnlyContext(async ctx =>
        {
            return await GetBeneficiaryInquiryQueryAsync(ctx, req, cancellationToken);
        }, cancellationToken);
    }

    /// <summary>
    /// Builds the query for beneficiary inquiry with optional filtering using an existing context.
    /// </summary>
    public async Task<IQueryable<MasterInquiryItem>> GetBeneficiaryInquiryQueryAsync(
        ProfitSharingReadOnlyDbContext ctx,
        MasterInquiryRequest? req = null,
        CancellationToken cancellationToken = default)
    {
        // CRITICAL FIX (PS-1998): Start from Beneficiaries and LEFT JOIN to ProfitDetails
        // to include beneficiaries even if they have no ProfitDetails records.
        // Previously, the query started from ProfitDetails with INNER JOIN,
        // which excluded beneficiaries that never had distributions.

        IQueryable<Beneficiary> beneficiariesQuery = ctx.Beneficiaries
            .Include(b => b.Contact)
            .ThenInclude(bc => bc!.ContactInfo)
            .TagWith("MasterInquiry: Get beneficiary");

        // Build the profit details filter if needed
        IQueryable<ProfitDetail> profitDetailsQuery = ctx.ProfitDetails;

        if (req?.EndProfitYear.HasValue == true)
        {
            profitDetailsQuery = profitDetailsQuery.Where(pd => pd.ProfitYear <= req.EndProfitYear.Value);
        }

        if (req?.PaymentType.HasValue == true)
        {
            var commentTypeIds = req.PaymentType switch
            {
                1 => new byte?[] { CommentType.Constants.Hardship.Id, CommentType.Constants.Distribution.Id },
                2 => new byte?[] { CommentType.Constants.Payoff.Id, CommentType.Constants.Forfeit.Id },
                3 => new byte?[] { CommentType.Constants.Rollover.Id, CommentType.Constants.RothIra.Id },
                _ => Array.Empty<byte?>()
            };

            if (commentTypeIds.Length > 0)
            {
                profitDetailsQuery = profitDetailsQuery.Where(pd => commentTypeIds.Contains(pd.CommentTypeId));
            }
        }

        profitDetailsQuery = profitDetailsQuery
            .Include(pd => pd.ProfitCode)
            .Include(pd => pd.ZeroContributionReason)
            .Include(pd => pd.TaxCode)
            .Include(pd => pd.CommentType)
            .TagWith("MasterInquiry: Get profit details for join");

        // LEFT JOIN: Beneficiaries to ProfitDetails
        var query = beneficiariesQuery
            .GroupJoin(
                profitDetailsQuery,
                b => b.Contact!.Ssn,
                pd => pd.Ssn,
                (b, profitDetails) => new { b, profitDetails })
            .SelectMany(x =>
                x.profitDetails.DefaultIfEmpty(),
                (x, pd) => new MasterInquiryItem
                {
                    ProfitDetail = pd,
                    ProfitCode = pd != null ? pd.ProfitCode : null,
                    ZeroContributionReason = pd != null ? pd.ZeroContributionReason : null,
                    TaxCode = pd != null ? pd.TaxCode : null,
                    CommentType = pd != null ? pd.CommentType : null,
                    TransactionDate = pd != null ? pd.CreatedAtUtc : DateTimeOffset.MinValue,
                    Member = new InquiryDemographics
                    {
                        Id = x.b.Id,
                        BadgeNumber = x.b.BadgeNumber,
                        FullName = x.b.Contact!.ContactInfo!.FullName != null ? x.b.Contact.ContactInfo.FullName : x.b.Contact.ContactInfo.LastName,
                        FirstName = x.b.Contact.ContactInfo.FirstName,
                        LastName = x.b.Contact.ContactInfo.LastName,
                        PayFrequencyId = 0,
                        Ssn = x.b.Contact.Ssn,
                        PsnSuffix = x.b.PsnSuffix,
                        CurrentIncomeYear = 0,
                        CurrentHoursYear = 0,
                        IsExecutive = false,
                        EmploymentStatusId = null,  // Beneficiaries don't have employment status
                    }
                });

        return await Task.FromResult(query);
    }

    /// <summary>
    /// Gets detailed beneficiary information by ID.
    /// </summary>
    public async Task<(int ssn, MemberDetails? memberDetails)> GetBeneficiaryDetailsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _factory.UseReadOnlyContext(async ctx =>
        {
            var memberData = await ctx.Beneficiaries
                .Include(b => b.Contact)
                .Where(b => b.Id == id)
                .Select(b => new
                {
                    b.Id,
                    b.Contact!.ContactInfo.FirstName,
                    b.Contact.ContactInfo.LastName,
                    b.Contact.ContactInfo.PhoneNumber,
                    b.Contact.Address.City,
                    b.Contact.Address.State,
                    Address = b.Contact.Address.Street,
                    b.Contact.Address.PostalCode,
                    b.Contact.DateOfBirth,
                    b.Contact.Ssn,
                    b.BadgeNumber,
                    b.PsnSuffix,
                    b.DemographicId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (memberData == null)
            {
                return (0, new MemberDetails { Id = 0 });
            }

            return (memberData.Ssn, new MemberDetails
            {
                IsEmployee = false,
                Id = memberData.Id,
                FirstName = memberData.FirstName,
                LastName = memberData.LastName,
                PhoneNumber = memberData.PhoneNumber,
                AddressCity = memberData.City!,
                AddressState = memberData.State!,
                Address = memberData.Address,
                AddressZipCode = memberData.PostalCode!,
                DateOfBirth = memberData.DateOfBirth,
                Age = memberData.DateOfBirth.Age(),
                Ssn = memberData.Ssn.MaskSsn(),
                BadgeNumber = memberData.BadgeNumber,
                PsnSuffix = memberData.PsnSuffix,
                PayFrequencyId = 0,
                IsExecutive = false,
            });
        }, cancellationToken);
    }

    /// <summary>
    /// Gets paginated beneficiary details for a set of SSNs with sorting support.
    /// </summary>
    public async Task<PaginatedResponseDto<MemberDetails>> GetBeneficiaryDetailsForSsnsAsync(
        SortedPaginationRequestDto req,
        ISet<int> ssns,
        CancellationToken cancellationToken = default)
    {
        return await _factory.UseReadOnlyContext(async ctx =>
        {
            // EF Core 9: Optimize beneficiary query with better projection
            var membersQuery = ctx.Beneficiaries
                .Where(b => b.Contact != null && ssns.Contains(b.Contact.Ssn))
                .TagWith("MasterInquiry: Get beneficiary details for SSNs");

            // Only filter by BadgeNumber if provided and not 0
            var badgeNumber = ((MasterInquiryRequest)req).BadgeNumber;
            if (badgeNumber.HasValue && badgeNumber != 0)
            {
                membersQuery = membersQuery.Where(b => b.BadgeNumber == badgeNumber);
            }

            // Optimize projection: select only what we need
            var members = membersQuery
                .Select(b => new
                {
                    b.Id,
                    b.Contact!.ContactInfo.FullName,
                    b.Contact!.ContactInfo.FirstName,
                    b.Contact.ContactInfo.LastName,
                    b.Contact.ContactInfo.PhoneNumber,
                    b.Contact.Address.City,
                    b.Contact.Address.State,
                    Address = b.Contact.Address.Street,
                    b.Contact.Address.PostalCode,
                    b.Contact.DateOfBirth,
                    b.Contact.Ssn,
                    b.BadgeNumber,
                    b.PsnSuffix,
                    DemographicId = b.Id
                });

            return await members.Select(memberData => new MemberDetails
            {
                Id = memberData.Id,
                IsEmployee = false,
                FirstName = memberData.FirstName,
                LastName = memberData.LastName,
                PhoneNumber = memberData.PhoneNumber,
                AddressCity = memberData.City!,
                AddressState = memberData.State!,
                Address = memberData.Address,
                AddressZipCode = memberData.PostalCode!,
                DateOfBirth = memberData.DateOfBirth,
                Ssn = memberData.Ssn.MaskSsn(),
                BadgeNumber = memberData.BadgeNumber,
                PsnSuffix = memberData.PsnSuffix,
                PayFrequencyId = 0,
                IsExecutive = false,
            })
                .ToPaginationResultsAsync(req, cancellationToken);
        }, cancellationToken);
    }

    /// <summary>
    /// Gets all (non-paginated) beneficiary details for a set of SSNs.
    /// </summary>
    public async Task<List<MemberDetails>> GetAllBeneficiaryDetailsForSsnsAsync(
        ISet<int> ssns,
        CancellationToken cancellationToken = default)
    {
        return await _factory.UseReadOnlyContext(async ctx =>
        {
            // EF Core 9: Optimize projection to fetch only needed data
            var members = await ctx.Beneficiaries
                .Where(b => b.Contact != null && ssns.Contains(b.Contact.Ssn))
                .TagWith("MasterInquiry: Get all beneficiary details for SSNs")
                .Select(b => new
                {
                    b.Id,
                    b.Contact!.ContactInfo.FirstName,
                    b.Contact.ContactInfo.LastName,
                    b.Contact.ContactInfo.PhoneNumber,
                    b.Contact.Address.City,
                    b.Contact.Address.State,
                    Address = b.Contact.Address.Street,
                    b.Contact.Address.PostalCode,
                    b.Contact.DateOfBirth,
                    b.Contact.Ssn,
                    b.BadgeNumber,
                    b.PsnSuffix,
                    DemographicId = b.Id
                })
                .ToListAsync(cancellationToken);

            var detailsList = new List<MemberDetails>();
            foreach (var memberData in members)
            {
                detailsList.Add(new MemberDetails
                {
                    Id = memberData.Id,
                    IsEmployee = false,
                    FirstName = memberData.FirstName,
                    LastName = memberData.LastName,
                    PhoneNumber = memberData.PhoneNumber,
                    AddressCity = memberData.City!,
                    AddressState = memberData.State!,
                    Address = memberData.Address,
                    AddressZipCode = memberData.PostalCode!,
                    DateOfBirth = memberData.DateOfBirth,
                    Ssn = memberData.Ssn.MaskSsn(),
                    BadgeNumber = memberData.BadgeNumber,
                    PsnSuffix = memberData.PsnSuffix,
                    PayFrequencyId = 0,
                    IsExecutive = false,
                });
            }

            return detailsList;
        }, cancellationToken);
    }

    /// <summary>
    /// Find beneficiary SSN by badge number and PSN suffix.
    /// </summary>
    public async Task<int> FindBeneficiarySsnByBadgeAsync(
        int badgeNumber,
        short psnSuffix,
        CancellationToken cancellationToken = default)
    {
        return await _factory.UseReadOnlyContext(async ctx =>
        {
            // ReadOnlyDbContext automatically handles AsNoTracking
            int ssnBene = await ctx.Beneficiaries
                .Where(b => b.BadgeNumber == badgeNumber && b.PsnSuffix == psnSuffix)
                .Join(ctx.BeneficiaryContacts, b => b.BeneficiaryContactId, bc => bc.Id, (b, bc) => bc.Ssn)
                .FirstOrDefaultAsync(cancellationToken);

            return ssnBene;
        }, cancellationToken);
    }
}
