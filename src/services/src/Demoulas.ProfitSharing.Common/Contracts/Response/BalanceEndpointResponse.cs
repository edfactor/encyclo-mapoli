using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed class BalanceEndpointResponse
{
    public required string Id { get; set; }
    public required string Ssn { get; set; }
    public Decimal VestedBalance { get; set; }
    public Decimal TotalDistributions { get; set; }
    public Decimal Etva { get; set; }
    public Decimal VestingPercent { get; set; }
    public Decimal CurrentBalance { get; set; }

    public static BalanceEndpointResponse ResponseExample() {
        return new BalanceEndpointResponse()
        {
            Id = "123456789",
            Ssn = "xxx-xx-6789",
            VestedBalance = 2030,
            TotalDistributions = 200,
            Etva = 250,
            VestingPercent = .4m,
            CurrentBalance = 5000
        };
    }
}
