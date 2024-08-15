namespace Demoulas.ProfitSharing.Common.Exceptions;

public class DemographicException : Exception
{
    public DemographicException(string? message) : base(message)
    {
        
    }

    public DemographicException(string? message, Exception? innerException)
        : base(message, innerException)
    {
        
    }
}
