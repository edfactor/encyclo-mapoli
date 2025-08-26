using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;
public sealed class CertificatesReportEndpoint: EndpointWithCsvBase<CerficatePrintRequest, CertificateReprintResponse, CertificatesReportEndpoint.CertificateReprintResponseMap>
{
    private readonly ICertificateService _certificateService;

    public CertificatesReportEndpoint(ICertificateService certificateService) : base(Navigation.Constants.PrintProfitCerts)
    {
        _certificateService = certificateService;
    }

    public override void Configure()
    {
        Get("post-frozen/certificates");
        Summary(s =>
        {
            s.Summary = "Returns base data for the certificates to be printed";
            s.Description = "Returns a list of employees who are to receive certificates.  This is typically used to reprint lost certificates.";
            s.ExampleRequest = new CerficatePrintRequest
            {
                ProfitYear = 2025,
                BadgeNumbers = new int[] { 12345, 23456 }
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<CertificateReprintResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<CertificateReprintResponse>
                        {
                            Results = new List<CertificateReprintResponse> { CertificateReprintResponse.ResponseExample() }
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "Certificate Reprint";
    
    public override Task<ReportResponseBase<CertificateReprintResponse>> GetResponse(CerficatePrintRequest req, CancellationToken ct)
    {
        return _certificateService.GetMembersWithBalanceActivityByStore(req, ct);
    }

    public sealed class CertificateReprintResponseMap : CsvHelper.Configuration.ClassMap<CertificateReprintResponse>
    {
        public CertificateReprintResponseMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("Badge Number");
            Map(m => m.FullName).Index(1).Name("Full Name");
            Map(m => m.Street1).Index(2).Name("Address");
            Map(m => m.City).Index(3).Name("City");
            Map(m => m.State).Index(4).Name("State");
            Map(m => m.PostalCode).Index(5).Name("Zip");
            Map(m => m.PayClassificationName).Index(6).Name("Pay Classification");
            Map(m => m.BeginningBalance).Index(7).Name("Beginning Balance").TypeConverterOption.Format("C2");
            Map(m => m.Earnings).Index(8).Name("Earnings").TypeConverterOption.Format("C2");
            Map(m => m.Contributions).Index(9).Name("Contributions").TypeConverterOption.Format("C2");
            Map(m => m.Forfeitures).Index(10).Name("Forfeitures").TypeConverterOption.Format("C2");
            Map(m => m.Distributions).Index(11).Name("Distributions").TypeConverterOption.Format("C2");
            Map(m => m.EndingBalance).Index(12).Name("Ending Balance").TypeConverterOption.Format("C2");
            Map(m => m.VestedAmount).Index(13).Name("Vested Amount").TypeConverterOption.Format("C2");
            Map(m => m.VestedPercent).Index(14).Name("Vested %").TypeConverterOption.Format("P0");
            Map(m => m.DateOfBirth).Index(15).Name("Date of Birth").TypeConverterOption.Format("MM/dd/yyyy");
            Map(m => m.HireDate).Index(16).Name("Hire Date").TypeConverterOption.Format("MM/dd/yyyy");
            Map(m => m.TerminationDate).Index(17).Name("Termination Date").TypeConverterOption.Format("MM/dd/yyyy");
            Map(m => m.EnrollmentId).Index(18).Name("Enrollment ID");
            Map(m => m.ProfitShareHours).Index(19).Name("Profit Share Hours").TypeConverterOption.Format("N2");
            Map(m => m.CertificateSort).Index(20).Name("Certificate Sort");
            Map(m => m.AnnuitySingleRate).Index(21).Name("Annuity Single Rate").TypeConverterOption.Format("C2");
            Map(m => m.AnnuityJointRate).Index(22).Name("Annuity Joint Rate").TypeConverterOption.Format("C2");
            Map(m => m.MonthlyPaymentSingle).Index(23).Name("Monthly Payment Single").TypeConverterOption.Format("C2");
            Map(m => m.MonthlyPaymentJoint).Index(24).Name("Monthly Payment Joint").TypeConverterOption.Format("C2");
        }
    }
}
