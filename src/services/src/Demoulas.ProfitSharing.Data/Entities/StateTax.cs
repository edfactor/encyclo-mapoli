namespace Demoulas.ProfitSharing.Data.Entities;

public class StateTax
{
    public required string Abbreviation { get; set; }
    public required decimal Rate { get; set; }
    public required string UserModified { get; set; }
    public required DateOnly DateModified { get; set; }

}
