namespace Demoulas.ProfitSharing.Data.Entities.Base;

public abstract class Member : ModifiedBase
{
    /// <summary>
    /// Gets or sets the unique identifier for the member.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the badge number.
    /// PSN Number is a link to an employee where the money comes from.
    /// BadgeNumber number is only by an employee.
    /// </summary>
    public required int BadgeNumber { get; set; }
}
