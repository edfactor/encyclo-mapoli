using System.Text;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Services.Certificates;

public sealed class CertificateService : ICertificateService
{
    private readonly IBreakdownService _breakdownService;
    private readonly ICalendarService _calendarService;
    private readonly IAnnuityRatesService _annuityRatesService;
    private readonly IAnnuityRateValidator _annuityRateValidator;

    public CertificateService(
        IBreakdownService breakdownService,
        ICalendarService calendarService,
        IAnnuityRatesService annuityRatesService,
        IAnnuityRateValidator annuityRateValidator)
    {
        _breakdownService = breakdownService;
        _calendarService = calendarService;
        _annuityRatesService = annuityRatesService;
        _annuityRateValidator = annuityRateValidator;
    }

    public async Task<Result<string>> GetCertificateFile(CerficatePrintRequest request, CancellationToken token)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, token);
        var annuityRatesResult = await GetAnnuityRatesByAge(request, token);
        if (!annuityRatesResult.IsSuccess)
        {
            return Result<string>.Failure(annuityRatesResult.Error!);
        }
        var annuityRates = annuityRatesResult.Value!;

        var members = await GetCertificateData(request, token);

        var sb = new StringBuilder();
        var spaces_2 = new string(' ', 2);
        var spaces_3 = new string(' ', 3);
        var spaces_4 = new string(' ', 4);
        var spaces_11 = new string(' ', 11);
        var spaces_28 = new string(' ', 28);
        var spaces_32 = new string(' ', 32);
        var spaces_38 = new string(' ', 38);
        var spaces_45 = new string(' ', 45);
        var spaces_60 = new string(' ', 60);
        var linefeeds_2 = new string('\n', 2);
        var linefeeds_4 = new string('\n', 4);
        _ = new string('\n', 5);
        var linefeeds_6 = new string('\n', 6);

        //Add xerox header
        sb.Append("\fDJDE JDE=PROFNEW,JDL=DFLT5,END,;\r");

        foreach (var member in members.Response.Results)
        {
            var estimatedPaymentAtAge = member.DateOfBirth.Age(calInfo.FiscalEndDate.ToDateTime(TimeOnly.MaxValue));
            if (estimatedPaymentAtAge < 67)
            {
                estimatedPaymentAtAge = 67;
            }
            if (!annuityRates.TryGetValue((byte)estimatedPaymentAtAge, out var annuityRate))
            {
                return Result<string>.Failure(Error.Validation(new Dictionary<string, string[]>
                {
                    ["AnnuityRate"] = [$"Annuity rate for age {estimatedPaymentAtAge} not found in year {request.ProfitYear}. This indicates incomplete rate data."]
                }));
            }
            var pmtSingle = (member.BeginningBalance + member.Contributions + member.Forfeitures) / annuityRate.SingleRate / 12;
            var pmtJoint = (member.BeginningBalance + member.Contributions + member.Forfeitures) / annuityRate.JointRate / 12;

            #region Formfeed
            sb.Append("\f");
            #endregion

            WriteMemberInfo(sb, member, true);

            #region Spacing
            sb.Append(linefeeds_2);
            sb.Append("\r\n");
            sb.Append(linefeeds_6);
            #endregion

            #region Spacing
            sb.Append("{\r\n");
            #endregion

            #region Balances
            var begBal = member.BeginningBalance.ToString("$#,###,###.00 ;$#,###,###.00-").PadLeft(14);
            sb.AppendFormat(member.BeginningBalance.ToString("$#,###,###.00 ;$#,###,###.00-").PadLeft(14));
            sb.Append(spaces_3);
            sb.AppendFormat(member.Contributions.ToString("$#,###,###.00 ;$#,###,###.00-").PadLeft(14));
            sb.Append(spaces_3);
            sb.AppendFormat(member.Distributions.ToString("$#,###,###.00 ;$#,###,###.00-").PadLeft(14));
            sb.Append(spaces_4);
            sb.AppendFormat(member.EndingBalance.ToString("$#,###,###.00 ;$#,###,###.00-").PadLeft(14));
            sb.Append(spaces_3);
            sb.AppendFormat(member.VestedAmount.ToString("$#,###,###.00 ;$#,###,###.00-").PadLeft(14).TrimEnd());
            sb.Append("\r\n");
            #endregion

            #region Spacing
            sb.Append(linefeeds_6);
            #endregion

            #region Estimated Monthly Payment at <age>
            sb.Append(spaces_32);
            sb.Append(estimatedPaymentAtAge.ToString("###").PadRight(3));
            sb.Append(spaces_38);
            sb.Append(estimatedPaymentAtAge.ToString("###").PadRight(3).Trim());
            sb.Append("\r\n");
            #endregion

            #region Spacing
            sb.Append(linefeeds_4);
            #endregion

            #region Payment
            sb.Append(spaces_11);
            sb.AppendFormat(pmtSingle.ToString("$#,###,###.00 ;$#,###,###.00-").PadLeft(14));
            sb.Append(spaces_28);
            sb.AppendFormat(pmtJoint.ToString("$#,###,###.00 ;$#,###,###.00-").PadLeft(14).TrimEnd());
            sb.Append("\r\n");
            #endregion

            #region Spacing
            sb.Append(linefeeds_6);
            #endregion

            #region Age in body         "and that you are <age> on this date"
            sb.Append(spaces_60);
            sb.Append(estimatedPaymentAtAge.ToString("###").PadRight(3).TrimEnd());
            sb.Append('\r');
            #endregion

            #region Formfeed
            sb.Append("\f");
            sb.Append("{\r\n");
            sb.Append(linefeeds_4);
            #endregion

            WriteMemberInfo(sb, member, false);

        }

        return Result<string>.Success(sb.ToString());

        void WriteMemberInfo(StringBuilder sb, MemberYearSummaryDto member, bool addLf)
        {
            #region Store and Badge Line
            sb.Append(spaces_45);
            sb.Append(member.StoreNumber.ToString("000"));
            sb.Append("-");
            sb.Append(member.BadgeNumber.ToString("0000000"));
            sb.Append("\r\n");
            #endregion

            #region Name line
            sb.Append(spaces_45);
            sb.Append(member.FullName);
            sb.Append("\r\n");
            #endregion

            #region Address line
            sb.Append(spaces_45);
            sb.Append(member.Street1);
            sb.Append("\r\n");
            #endregion

            #region City, State, Zip line
            sb.Append(spaces_45);
            sb.Append(member.City ?? "");
            sb.Append(", ");
            sb.Append(member.State ?? "");
            sb.Append(spaces_2);
            sb.Append((Convert.ToInt32(member.PostalCode ?? "0")).ToString("00000"));
            sb.Append("\r");
            if (addLf)
            {
                sb.Append("\n");
            }
            #endregion
        }
    }

    public async Task<Result<ReportResponseBase<CertificateReprintResponse>>> GetMembersWithBalanceActivityByStore(CerficatePrintRequest request, CancellationToken token)
    {
        var rawData = await GetCertificateData(request, token);
        var rslt = new List<CertificateReprintResponse>();
        var annuityRatesResult = await GetAnnuityRatesByAge(request, token);
        if (!annuityRatesResult.IsSuccess)
        {
            return Result<ReportResponseBase<CertificateReprintResponse>>.Failure(annuityRatesResult.Error!);
        }
        var annuityRates = annuityRatesResult.Value!;
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, token);

        foreach (var cert in rawData.Response.Results)
        {
            decimal? jointRate = null;
            decimal? singleRate = null;
            decimal? pmtSingle = null;
            decimal? pmtJoint = null;
            var age = (byte)cert.DateOfBirth.Age(calInfo.FiscalEndDate.ToDateTime(TimeOnly.MaxValue));
            if (age < 67)
            {
                age = 67;
            }

            annuityRates.TryGetValue(age, out var annuityRate);
            if (annuityRate != null)
            {
                jointRate = annuityRate.JointRate;
                singleRate = annuityRate.SingleRate;
                pmtSingle = (cert.BeginningBalance + cert.Contributions + cert.Forfeitures) / annuityRate.SingleRate / 12;
                pmtJoint = (cert.BeginningBalance + cert.Contributions + cert.Forfeitures) / annuityRate.JointRate / 12;
            }

            rslt.Add(new CertificateReprintResponse()
            {
                BadgeNumber = cert.BadgeNumber,
                FullName = cert.FullName,
                StoreNumber = cert.StoreNumber,
                DateOfBirth = cert.DateOfBirth,
                BeginningBalance = cert.BeginningBalance,
                Contributions = cert.Contributions,
                Forfeitures = cert.Forfeitures,
                VestedAmount = cert.VestedAmount,
                EndingBalance = cert.EndingBalance,
                Distributions = cert.Distributions,
                AnnuityJointRate = jointRate,
                AnnuitySingleRate = singleRate,
                MonthlyPaymentJoint = pmtJoint,
                MonthlyPaymentSingle = pmtSingle,
                PayClassificationId = cert.PayClassificationId,
                PayClassificationName = cert.PayClassificationName,
                CertificateSort = cert.CertificateSort,
                HireDate = cert.HireDate,
                TerminationDate = cert.TerminationDate,
                EnrollmentId = cert.EnrollmentId,
                ProfitShareHours = cert.ProfitShareHours,
                Earnings = cert.Earnings,
                VestedPercent = cert.VestedPercent,
                Street1 = cert.Street1,
                City = cert.City,
                State = cert.State,
                PostalCode = cert.PostalCode,
                IsExecutive = cert.IsExecutive,
            });
        }

        return Result<ReportResponseBase<CertificateReprintResponse>>.Success(new ReportResponseBase<CertificateReprintResponse>()
        {
            ReportDate = rawData.ReportDate,
            ReportName = rawData.ReportName,
            Response = new PaginatedResponseDto<CertificateReprintResponse>
            {
                Results = rslt,
                Total = rawData.Response.Total
            },
            StartDate = rawData.StartDate,
            EndDate = rawData.EndDate,

        });
    }

    private Task<ReportResponseBase<MemberYearSummaryDto>> GetCertificateData(CerficatePrintRequest request, CancellationToken token)
    {
        var breakdownRequest = new BreakdownByStoreRequest
        {
            ProfitYear = request.ProfitYear,
            Skip = request.Skip,
            Take = request.Take,
            SortBy = ReferenceData.CertificateSort,
        };

        return _breakdownService.GetMembersWithBalanceActivityByStore(breakdownRequest, request.Ssns, request.BadgeNumbers ?? Array.Empty<int>(), token);
    }

    private async Task<Result<Dictionary<byte, AnnuityRateDto>>> GetAnnuityRatesByAge(CerficatePrintRequest request, CancellationToken token)
    {
        // Validate year completeness first
        var validationResult = await _annuityRateValidator.ValidateYearCompletenessAsync(request.ProfitYear, token);
        if (!validationResult.IsSuccess)
        {
            return Result<Dictionary<byte, AnnuityRateDto>>.Failure(validationResult.Error!);
        }

        // Get annuity rates for the year
        var result = await _annuityRatesService.GetAnnuityRatesByYearAsync(request.ProfitYear, token);
        if (!result.IsSuccess)
        {
            return Result<Dictionary<byte, AnnuityRateDto>>.Failure(
                result.Error ?? Error.Unexpected("Failed to retrieve annuity rates."));
        }

        var dictionary = result.Value!.ToDictionary(x => x.Age, x => x);
        return Result<Dictionary<byte, AnnuityRateDto>>.Success(dictionary);
    }
}
