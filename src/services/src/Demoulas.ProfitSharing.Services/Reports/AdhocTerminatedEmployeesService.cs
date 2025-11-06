using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

public class AdhocTerminatedEmployeesService : IAdhocTerminatedEmployeesService
{
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;

    public AdhocTerminatedEmployeesService(
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        IDemographicReaderService demographicReaderService
    )
    {
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _demographicReaderService = demographicReaderService;
    }

    public async Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetTerminatedEmployees(StartAndEndDateRequest req, CancellationToken cancellationToken)
    {
        var startDate = req.BeginningDate;
        var endDate = req.EndingDate;

        var rslt = await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographic = await _demographicReaderService.BuildDemographicQuery(ctx, false);
            var query = (from d in demographic.Include(d => d.TerminationCode)
                         where d.TerminationDate != null
                            && d.TerminationDate.Value >= startDate
                            && d.TerminationDate.Value <= endDate
                            && d.TerminationCodeId != TerminationCode.Constants.Retired
                         select new AdhocTerminatedEmployeeResponse
                         {
                             BadgeNumber = d.BadgeNumber,
                             FullName = d.ContactInfo.FullName != null ? d.ContactInfo.FullName : string.Empty,
                             FirstName = d.ContactInfo.FirstName,
                             LastName = d.ContactInfo.LastName,
                             MiddleInitial = !string.IsNullOrEmpty(d.ContactInfo.MiddleName) ? d.ContactInfo.MiddleName[0].ToString() : string.Empty,
                             Ssn = d.Ssn.MaskSsn(),
                             TerminationDate = d.TerminationDate!.Value,
                             TerminationCodeId = d.TerminationCodeId,
                             TerminationCode = d.TerminationCode != null ? d.TerminationCode.Name : string.Empty,
                             IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
                             Address = d.Address.Street,
                             Address2 = !string.IsNullOrEmpty(d.Address.Street2) ? d.Address.Street2 : string.Empty,
                             City = !string.IsNullOrEmpty(d.Address.City) ? d.Address.City : string.Empty,
                             State = !string.IsNullOrEmpty(d.Address.State) ? d.Address.State : string.Empty,
                             PostalCode = !string.IsNullOrEmpty(d.Address.PostalCode) ? d.Address.PostalCode : string.Empty
                         }).ToPaginationResultsAsync(req, cancellationToken: cancellationToken);

            return await query;
        }, cancellationToken);

        return new ReportResponseBase<AdhocTerminatedEmployeeResponse>()
        {
            ReportName = "Adhoc Terminated Employee Report",
            ReportDate = DateTimeOffset.Now,
            StartDate = startDate,
            EndDate = endDate,
            Response = rslt
        };
    }

    public async Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetTerminatedEmployeesNeedingFormLetter(FilterableStartAndEndDateRequest req, CancellationToken cancellationToken)
    {
        // Convert to TerminatedLettersRequest and delegate to the more flexible overload
        var terminatedLettersRequest = new TerminatedLettersRequest
        {
            BeginningDate = req.BeginningDate,
            EndingDate = req.EndingDate,
            ExcludeZeroBalance = req.ExcludeZeroBalance,
            Skip = req.Skip,
            Take = req.Take,
            SortBy = req.SortBy,
            IsSortDescending = req.IsSortDescending,
            BadgeNumbers = null // No badge filtering for the original method
        };

        return await GetTerminatedEmployeesNeedingFormLetter(terminatedLettersRequest, cancellationToken);
    }

    public async Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetTerminatedEmployeesNeedingFormLetter(TerminatedLettersRequest req, CancellationToken cancellationToken)
    {
        var beginningDate = req.BeginningDate ?? DateOnly.MinValue;
        var endingDate = req.EndingDate ?? DateOnly.MaxValue;

        var rslt = await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographic = await _demographicReaderService.BuildDemographicQuery(ctx, false /*Want letter to be sent to the most current address*/);
            var query = (from d in demographic
                    .Include(x => x.Address)
                    .Include(x => x.TerminationCode)
                         where d.TerminationDate != null
                            && d.TerminationDate.Value >= beginningDate && d.TerminationDate.Value <= endingDate
                            && d.EmploymentStatusId == EmploymentStatus.Constants.Terminated
                            && d.TerminationCodeId != TerminationCode.Constants.Retired
                            && (req.BadgeNumbers == null || !req.BadgeNumbers.Any() || req.BadgeNumbers.Contains(d.BadgeNumber))
                         /*TODO : Exclude employees who have already been sent a letter?*/
                         /*Filter for employees who are not fully vested, and probably have a balance */
                         select new AdhocTerminatedEmployeeResponse
                         {
                             BadgeNumber = d.BadgeNumber,
                             FullName = d.ContactInfo.FullName != null ? d.ContactInfo.FullName : string.Empty,
                             Ssn = d.Ssn.MaskSsn(),
                             Address = d.Address.Street,
                             Address2 = !string.IsNullOrEmpty(d.Address.Street2) ? d.Address.Street2 : string.Empty,
                             City = !string.IsNullOrEmpty(d.Address.City) ? d.Address.City : string.Empty,
                             State = !string.IsNullOrEmpty(d.Address.State) ? d.Address.State : string.Empty,
                             PostalCode = !string.IsNullOrEmpty(d.Address.PostalCode) ? d.Address.PostalCode : string.Empty,
                             TerminationDate = d.TerminationDate!.Value,
                             TerminationCodeId = d.TerminationCodeId,
                             TerminationCode = d.TerminationCode != null ? d.TerminationCode.Name : string.Empty,
                             FirstName = d.ContactInfo.FirstName,
                             LastName = d.ContactInfo.LastName,
                             MiddleInitial = !string.IsNullOrEmpty(d.ContactInfo.MiddleName) ? d.ContactInfo.MiddleName[0].ToString() : string.Empty
                         }).ToPaginationResultsAsync(req, cancellationToken: cancellationToken);

            return await query;
        }, cancellationToken);

        return new ReportResponseBase<AdhocTerminatedEmployeeResponse>()
        {
            ReportName = "Adhoc Terminated Employee Report",
            ReportDate = DateTimeOffset.Now,
            StartDate = beginningDate,
            EndDate = endingDate,
            Response = rslt
        };
    }

    public async Task<string> GetFormLetterForTerminatedEmployees(StartAndEndDateRequest startAndEndDateRequest, CancellationToken cancellationToken)
    {
        var filterableRequest = new FilterableStartAndEndDateRequest
        {
            BeginningDate = startAndEndDateRequest.BeginningDate,
            EndingDate = startAndEndDateRequest.EndingDate,
            Skip = startAndEndDateRequest.Skip,
            Take = startAndEndDateRequest.Take,
            SortBy = startAndEndDateRequest.SortBy,
            IsSortDescending = startAndEndDateRequest.IsSortDescending
        };
        var report = await GetTerminatedEmployeesNeedingFormLetter(filterableRequest, cancellationToken);
        return GenerateFormLetterFromReport(report);
    }

    public async Task<string> GetFormLetterForTerminatedEmployees(TerminatedLettersRequest terminatedLettersRequest, CancellationToken cancellationToken)
    {
        var report = await GetTerminatedEmployeesNeedingFormLetter(terminatedLettersRequest, cancellationToken);
        return GenerateFormLetterFromReport(report);
    }

    private static string GenerateFormLetterFromReport(ReportResponseBase<AdhocTerminatedEmployeeResponse> report)
    {
        if (!report.Response.Results.Any())
        {
            return "No terminated employees found needing a form letter.";
        }

        var letter = new System.Text.StringBuilder();
        var space_7 = new string(' ', 7);
        _ = new string(' ', 11);
        _ = new string(' ', 24);
        var space_25 = new string(' ', 25);
        _ = new string(' ', 27);

        foreach (var emp in report.Response.Results)
        {
            #region Beginning of letter
            letter.AppendLine();
            letter.AppendLine("DJDE JDE=QPS003,JDL=PAYROL,END,;");
            #endregion

            #region Return address
            letter.AppendLine(" DEMOULAS PROFIT SHARING PLAN AND TRUST");
            letter.AppendLine(" 875 EAST STREET");
            letter.AppendLine(" ANDOVER, MA 01810");
            #endregion

            #region Spacing
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine();
            #endregion

            #region Member information
            letter.AppendLine($"{space_7}{emp.FirstName}{(emp.MiddleInitial != string.Empty ? " " : "")}{emp.MiddleInitial} {emp.LastName}");
            letter.AppendLine($"{space_7}{emp.Address}");
            letter.AppendLine($"{space_7}{emp.City}, {emp.State} {emp.PostalCode}");
            #endregion

            #region Spacing
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine();
            #endregion

            #region Salutation
            letter.AppendLine($"{space_7}Dear {emp.FirstName}:");
            #endregion

            #region Spacing
            letter.AppendLine();
            letter.AppendLine();
            #endregion

            #region Body of letter
            letter.AppendLine($"{space_7}Attached are the necessary forms needed to be completed by you in order for you to");
            letter.AppendLine($"{space_7}begin receiving your vested interest in the Demoulas Profit Sharing Plan and Trust.");
            #endregion

            #region Spacing
            letter.AppendLine();
            letter.AppendLine();
            #endregion

            #region Instructions
            letter.AppendLine($"{space_7}Please return the completed forms to the attention of:");
            #endregion

            #region Spacing
            letter.AppendLine();
            letter.AppendLine();
            #endregion

            #region Remittance address
            letter.AppendLine($"{space_25}Demoulas Profit Sharing Plan and Trust");
            letter.AppendLine($"{space_25}875 East Street");
            letter.AppendLine($"{space_25}Tewksbury, MA  01876");
            #endregion

            #region Spacing
            letter.AppendLine();
            letter.AppendLine();
            #endregion

            #region Questions
            letter.AppendLine($"{space_7}If you have any questions please do not hesitate to call (978) 851-8000.");
            #endregion

            #region Spacing
            letter.AppendLine();
            letter.AppendLine();
            #endregion

            #region Closing
            letter.AppendLine($"{space_7}Sincerely,");
            #endregion

            #region Spacing
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine();
            #endregion

            #region Signature
            letter.AppendLine($"{space_7}DEMOULAS PROFIT SHARING PLAN & TRUST");
            #endregion

            #region Printer Control
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine("DJDE JDE=DISNO1,JDL=PAYROL,END,;");
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine("DJDE JDE=DISNO2,JDL=PAYROL,END,;");
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine("DJDE JDE=DISNO3,JDL=PAYROL,END,;");
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine("DJDE JDE=DISNO4,JDL=PAYROL,END,;");
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine("DJDE JDE=DISNO5,JDL=PAYROL,END,;");
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine("DJDE JDE=BENDS1,JDL=PAYROL,END,;");
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine("DJDE JDE=ACKNRC,JDL=PAYROL,END,;");
            letter.Append("\f"); // Form feed to end the letter
            #endregion
        }

        return letter.ToString();
    }
}
