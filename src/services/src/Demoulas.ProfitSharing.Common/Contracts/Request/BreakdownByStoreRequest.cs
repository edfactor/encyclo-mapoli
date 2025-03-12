namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record BreakdownByStoreRequest : ProfitYearRequest
{
    public bool Under21Only { get; set; }

    public short? StoreNumber { get; set; }
}
