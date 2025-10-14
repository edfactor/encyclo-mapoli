using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.MasterInquiry;

/// <summary>
/// Utility methods for master inquiry operations including filtering and sorting.
/// </summary>
public static class MasterInquiryHelpers
{
    /// <summary>
    /// Applies comprehensive filtering to a master inquiry query based on request parameters.
    /// Filters are applied in order of selectivity for optimal Oracle query performance.
    /// </summary>
    public static IQueryable<MasterInquiryItem> FilterMemberQuery(
        MasterInquiryRequest req,
        IQueryable<MasterInquiryItem> query)
    {
        // CRITICAL: Apply most selective filters first for Oracle query optimizer
        // PaymentType is highly selective, so apply it early
        if (req.PaymentType.HasValue)
        {
            switch (req.PaymentType)
            {
                case 1: // Hardship/Distribution
                    List<byte?> array = [CommentType.Constants.Hardship.Id, CommentType.Constants.Distribution.Id];
                    query = query.Where(x => x.ProfitDetail != null && array.Contains(x.ProfitDetail.CommentTypeId));
                    break;
                case 2: // payoffs
                    array = [CommentType.Constants.Payoff.Id, CommentType.Constants.Forfeit.Id];
                    query = query.Where(x => x.ProfitDetail != null && array.Contains(x.ProfitDetail.CommentTypeId));
                    break;
                case 3: // rollovers
                    array = [CommentType.Constants.Rollover.Id, CommentType.Constants.RothIra.Id];
                    query = query.Where(x => x.ProfitDetail != null && array.Contains(x.ProfitDetail.CommentTypeId));
                    break;
            }
        }

        // Apply EndProfitYear early - it's highly selective
        if (req.EndProfitYear.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.ProfitYear <= req.EndProfitYear));
        }

        // SSN is most selective - always apply early if present
        if (req.Ssn != 0)
        {
            query = query.Where(x => (x.Member.Ssn == req.Ssn));
        }

        // BadgeNumber is very selective
        if (req.BadgeNumber.HasValue && req.BadgeNumber > 0)
        {
            query = query.Where(x => x.Member.BadgeNumber == req.BadgeNumber);
        }

        // Name filter is selective
        if (!string.IsNullOrWhiteSpace(req.Name))
        {
            var pattern = $"%{req.Name.ToUpperInvariant()}%";
            query = query.Where(x => EF.Functions.Like(x.Member.FullName.ToUpper(), pattern));
        }

        // ProfitCode is moderately selective
        if (req.ProfitCode.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.ProfitCodeId == req.ProfitCode));
        }

        // Apply other filters
        if (req.MemberType != 1 /* Employee Only */ && req.PsnSuffix > 0)
        {
            query = query.Where(x => x.Member.PsnSuffix == req.PsnSuffix);
        }

        if (req.StartProfitMonth.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.MonthToDate >= req.StartProfitMonth));
        }

        if (req.EndProfitMonth.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.MonthToDate <= req.EndProfitMonth));
        }

        if (req.ContributionAmount.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.Contribution == req.ContributionAmount));
        }

        if (req.EarningsAmount.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.Earnings == req.EarningsAmount));
        }

        if (req.ForfeitureAmount.HasValue)
        {
            query = query.Where(x =>
                (x.ProfitDetail == null || x.ProfitDetail.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id) &&
                (x.ProfitDetail == null || x.ProfitDetail.Forfeiture == req.ForfeitureAmount));
        }

        if (req.PaymentAmount.HasValue)
        {
            query = query.Where(x =>
                (x.ProfitDetail == null || x.ProfitDetail.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id) &&
                (x.ProfitDetail == null || x.ProfitDetail.Forfeiture == req.PaymentAmount));
        }

        return query;
    }

    /// <summary>
    /// Applies sorting to member details query based on request parameters.
    /// </summary>
    public static IQueryable<MemberDetails> ApplySorting(
        IQueryable<MemberDetails> query,
        SortedPaginationRequestDto req)
    {
        if (string.IsNullOrEmpty(req.SortBy))
        {
            return query;
        }

        var isDescending = req.IsSortDescending ?? false;
        return req.SortBy.ToLower() switch
        {
            "fullname" => isDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName),
            "ssn" => isDescending ? query.OrderByDescending(x => x.Ssn) : query.OrderBy(x => x.Ssn),
            "badgenumber" => isDescending ? query.OrderByDescending(x => x.BadgeNumber) : query.OrderBy(x => x.BadgeNumber),
            "address" => isDescending ? query.OrderByDescending(x => x.Address) : query.OrderBy(x => x.Address),
            "addresscity" => isDescending ? query.OrderByDescending(x => x.AddressCity) : query.OrderBy(x => x.AddressCity),
            "addressstate" => isDescending ? query.OrderByDescending(x => x.AddressState) : query.OrderBy(x => x.AddressState),
            "addresszipCode" => isDescending ? query.OrderByDescending(x => x.AddressZipCode) : query.OrderBy(x => x.AddressZipCode),
            "age" => isDescending ? query.OrderByDescending(x => x.Age) : query.OrderBy(x => x.Age),
            "employmentStatus" => isDescending ? query.OrderByDescending(x => x.EmploymentStatus) : query.OrderBy(x => x.EmploymentStatus),
            _ => query
        };
    }

    /// <summary>
    /// Determines if we should use the optimized SSN query path.
    /// Use optimization when we have highly selective filters that can dramatically reduce the dataset.
    /// </summary>
    public static bool ShouldUseOptimizedSsnQuery(MasterInquiryRequest req)
    {
        // Use optimized path when we have filters that can reduce the dataset before joining
        // Complex filters (name search, specific amounts) require full join
        bool hasComplexFilters = !string.IsNullOrWhiteSpace(req.Name)
            || req.ContributionAmount.HasValue
            || req.EarningsAmount.HasValue
            || req.ForfeitureAmount.HasValue
            || req.PaymentAmount.HasValue;

        if (hasComplexFilters)
        {
            return false; // Must use standard path for complex filters
        }

        // Use fast path if we have PaymentType (highly selective)
        if (req.PaymentType.HasValue && req.PaymentType.Value > 0)
        {
            return true;
        }

        // Also use fast path if we have EndProfitYear with SSN or BadgeNumber (targeted lookup)
        if (req.EndProfitYear.HasValue && (req.Ssn != 0 || (req.BadgeNumber.HasValue && req.BadgeNumber.Value > 0)))
        {
            return true;
        }

        // For broad queries (all payment types, no specific member), use standard path with pre-filtering
        return false;
    }
}
