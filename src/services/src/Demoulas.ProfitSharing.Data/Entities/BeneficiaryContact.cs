using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class BeneficiaryContact : ModifiedBase
{
    private int _ssn;
    public required int Id { get; set; }

    public required int Ssn
    {
        get
        {
            return _ssn;
        }
        set
        {
            if (_ssn != value)
            {
                BeneficiarySsnChangeHistories.Add(new BeneficiarySsnChangeHistory()
                {
                    OldSsn = _ssn,
                    NewSsn = value,
                    ModifiedAtUtc = DateTimeOffset.UtcNow
                });
                _ssn = value;
            }
        }
    }

    public required DateOnly DateOfBirth { get; set; }

    public required Address Address { get; set; }
    public required ContactInfo ContactInfo { get; set; }

    public required DateOnly CreatedDate { get; set; }

    public List<Beneficiary>? Beneficiaries { get; set; }

    public List<BeneficiarySsnChangeHistory> BeneficiarySsnChangeHistories { get; set; } = new();
}
