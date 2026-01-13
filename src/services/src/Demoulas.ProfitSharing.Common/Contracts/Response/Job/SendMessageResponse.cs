namespace Demoulas.ProfitSharing.Common.Contracts.Response.Job;

public sealed record SendMessageResponse
{
    public string Message { get; set; } = "Message sent successfully";

    public static SendMessageResponse ResponseExample()
    {
        return new SendMessageResponse
        {
            Message = "Job message sent successfully"
        };
    }
}
