using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
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
    /// <summary>
    /// Builds the query for beneficiary inquiry with optional filtering.
    /// </summary>
    public IQueryable<MasterInquiryItem> GetBeneficiaryInquiryQuery(
        ProfitSharingReadOnlyDbContext ctx,
        MasterInquiryRequest? req = null)
    {
        // ReadOnlyDbContext automatically handles AsSplitQuery and AsNoTracking
        IQueryable<ProfitDetail> profitDetailsQuery = ctx.ProfitDetails;

        // OPTIMIZATION: Pre-filter ProfitDetails before expensive join if we have selective criteria
        if (req?.EndProfitYear.HasValue == true)
        {
            profitDetailsQuery = profitDetailsQuery.Where(pd => pd.ProfitYear <= req.EndProfitYear.Value);
        }

        if (req?.PaymentType.HasValue == true)
        {
            // Apply payment type filter early for massive performance gain
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

        var query = profitDetailsQuery
            .Include(pd => pd.ProfitCode)
            .Include(pd => pd.ZeroContributionReason)
            .Include(pd => pd.TaxCode)
            .Include(pd => pd.CommentType)
            .TagWith("MasterInquiry: Get beneficiary with profit details")
            .Join(ctx.Beneficiaries.Join(ctx.BeneficiaryContacts, b => b.BeneficiaryContactId, bc => bc.Id, (b, bc) => new { b, bc }),
                pd => pd.Ssn, bene => bene.bc.Ssn,
                (pd, d) => new MasterInquiryItem
                {
                    ProfitDetail = pd,
                    ProfitCode = pd.ProfitCode,
                    ZeroContributionReason = pd.ZeroContributionReason,
                    TaxCode = pd.TaxCode,
                    CommentType = pd.CommentType,
                    TransactionDate = pd.CreatedAtUtc,
                    Member = new InquiryDemographics
                    {
                        Id = d.bc.Id,
                        BadgeNumber = d.b.BadgeNumber,
                        FullName = d.bc.ContactInfo.FullName != null ? d.bc.ContactInfo.FullName : d.bc.ContactInfo.LastName,
                        FirstName = d.bc.ContactInfo.FirstName,
                        LastName = d.bc.ContactInfo.LastName,
                        PayFrequencyId = 0,
                        Ssn = d.bc.Ssn,
                        PsnSuffix = d.b.PsnSuffix,
                        CurrentIncomeYear = 0,
                        CurrentHoursYear = 0,
                        IsExecutive = false,
                    }
                });

        return query;
    }

    /// <summary>
    /// Gets detailed beneficiary information by ID.
    /// </summary>
    public async Task<(int ssn, MemberDetails? memberDetails)> GetBeneficiaryDetailsAsync(
        ProfitSharingReadOnlyDbContext ctx,
        int id,
        CancellationToken cancellationToken)
    {
        var memberData = await ctx.Beneficiaries
            .Include(b => b.Contact)
            .Where(b => b.Id == id)
            .Select(b => new
            {
                b.Id,
                b.Contact!.ContactInfo.FirstName,
                b.Contact.ContactInfo.LastName,
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
    }

    /// <summary>
    /// Gets paginated beneficiary details for a set of SSNs with sorting support.
    /// </summary>
    public Task<PaginatedResponseDto<MemberDetails>> GetBeneficiaryDetailsForSsnsAsync(
        ProfitSharingReadOnlyDbContext ctx,
        SortedPaginationRequestDto req,
        ISet<int> ssns,
        CancellationToken cancellationToken)
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

        return members.Select(memberData => new MemberDetails
        {
            Id = memberData.Id,
            IsEmployee = false,
            FirstName = memberData.FirstName,
            LastName = memberData.LastName,
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
    }

    /// <summary>
    /// Gets all (non-paginated) beneficiary details for a set of SSNs.
    /// </summary>
    public async Task<List<MemberDetails>> GetAllBeneficiaryDetailsForSsnsAsync(
        ProfitSharingReadOnlyDbContext ctx,
        ISet<int> ssns,
        CancellationToken cancellationToken)
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
    }

    /// <summary>
    /// Find beneficiary SSN by badge number and PSN suffix.
    /// </summary>
    public async Task<int> FindBeneficiarySsnByBadgeAsync(
        ProfitSharingReadOnlyDbContext ctx,
        int badgeNumber,
        short psnSuffix,
        CancellationToken cancellationToken)
    {
        // ReadOnlyDbContext automatically handles AsNoTracking
        int ssnBene = await ctx.Beneficiaries
            .Where(b => b.BadgeNumber == badgeNumber && b.PsnSuffix == psnSuffix)
            .Join(ctx.BeneficiaryContacts, b => b.BeneficiaryContactId, bc => bc.Id, (b, bc) => bc.Ssn)
            .FirstOrDefaultAsync(cancellationToken);

        return ssnBene;
    }
}
