using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Certificates;
public sealed class CertificateService : ICertificateService
{
    private readonly IBreakdownService _breakdownService;
    private readonly ICalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public CertificateService(IBreakdownService breakdownService, ICalendarService calendarService, IProfitSharingDataContextFactory dataContextFactory)
    {
        _breakdownService = breakdownService;
        _calendarService = calendarService;
        _dataContextFactory = dataContextFactory;
    }

    public async Task<string> GetCertificateFile(ProfitYearRequest request, CancellationToken token)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, token);

        Dictionary<byte, AnnuityRate> annuityRates = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            return await ctx.AnnuityRates
                .Where(x => x.Year == request.ProfitYear).ToDictionaryAsync(x => x.Age, x => x, token);
        });

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
        var linefeeds_5 = new string ('\n', 5);
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
            var annuityRate = annuityRates[(byte)estimatedPaymentAtAge];
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
            sb.AppendFormat("\r\n");
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

        return sb.ToString();

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

    public async Task<ReportResponseBase<MemberYearSummaryDto>> GetCertificateData(ProfitYearRequest request, CancellationToken token)
    {
        var breakdownRequest = new BreakdownByStoreRequest
        {
            ProfitYear = request.ProfitYear,
            Skip = request.Skip,
            Take = request.Take,
            SortBy = ReferenceData.CertificateSort,
        };

        return await _breakdownService.GetMembersWithBalanceByStore(breakdownRequest, token);

    }
}
