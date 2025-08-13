using System.Security.Cryptography;
using System.Text;

namespace Demoulas.ProfitSharing.Data.Entities.Virtual;
public class ParticipantTotalVestingBalance
{
    public int Ssn { get; set; }
    public int Id { get; set; }
    public decimal? VestedBalance { get; set; }
    public decimal? VestingPercent { get; set; }
    public decimal? CurrentBalance { get; set; }
    public byte? YearsInPlan { get; set; }
    public decimal? AllocationsToBeneficiary { get; set; }
    public decimal? AllocationsFromBeneficiary { get; set; }

    public byte[] CheckSum
    {
        get
        {
            byte[] bytes = Encoding.UTF8.GetBytes(
                $"{Ssn}{Id}{Ssn}{CurrentBalance}{YearsInPlan}{AllocationsToBeneficiary}{AllocationsFromBeneficiary}{VestedBalance}{VestingPercent}");
            return SHA256.HashData(bytes);
        }
    }
}
