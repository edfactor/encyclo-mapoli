using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class TerminationCode : ILookupTable<char>
{
    public static class Constants
    {
        public const char LeftOnOwn = 'A';
        public const char PersonalOrFamilyReason = 'B';
        public const char CouldNotWorkAvailableHours = 'C';
        public const char Stealing = 'D';
        public const char NotFollowingCompanyPolicy = 'E';
        public const char FmlaExpired = 'F';
        public const char TerminatedPrivate = 'G';
        public const char JobAbandonment = 'H';
        public const char HealthReasonsNonFmla = 'I';
        public const char LayoffNoWork = 'J';
        public const char SchoolOrSports = 'K';
        public const char MoveOutOfArea = 'L';
        public const char PoorPerformance = 'M';
        public const char OffForSummer = 'N';
        public const char WorkmansCompensation = 'O';
        public const char Injured = 'P';
        public const char Transferred = 'Q';
        public const char Retired = 'R';
        public const char Competition = 'S';
        public const char AnotherJob = 'T';
        public const char WouldNotRehire = 'U';
        public const char NeverReported = 'V';
        public const char RetiredReceivingPension = 'W';
        public const char Military = 'X';
        public const char FmlaApproved = 'Y';
        public const char Deceased = 'Z';
    }

    public char Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Demographic>? Demographics { get; set; }
}
