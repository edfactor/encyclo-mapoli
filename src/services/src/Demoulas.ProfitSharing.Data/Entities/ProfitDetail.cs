using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities;

public class ProfitDetail
{
    public short ProfitYear { get; set; }
    public byte ProfitYearIteration { get; set; }
    public short ProfitClient { get; set; }
    public required ProfitCode ProfitCode { get; set; }
    public short ProfitCodeId { get; set; }
    public decimal Contribution { get; set; }
    public decimal Earnings { get; set; }
    public decimal Forfeiture { get; set; }
    public byte Month { get; set; }
    public short Year { get; set; }
    public required string Comment { get; set; }
    public char ZeroCont { get; set; }
    public decimal FederalTaxes {get; set;}
    public decimal StateTaxes { get; set;}
    public required TaxCode TaxCode { get; set; }
    public char TaxCodeId { get; set; }
    public long SSN { get; set; }
    public int ProfDistId { get; set; }
}
