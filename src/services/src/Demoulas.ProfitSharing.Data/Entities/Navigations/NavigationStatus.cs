using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities.Navigations;

public class NavigationStatus : ILookupTable<byte>
{

    public static class Constants
    {
        public const byte NotStarted = 1;
        public const byte InProgress = 2;
        public const byte OnHold = 3;
        public const byte Complete = 4;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }

    public List<Navigation>? Navigations { get; set; }
    public List<NavigationTracking>? NavigationTrackings { get; set; }

}
