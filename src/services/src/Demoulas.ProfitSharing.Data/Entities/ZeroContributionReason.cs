using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class ZeroContributionReason : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte Normal = 0;
        /// <summary>
        /// 18, 19, OR 20 WITH > 1000 HOURS
        /// </summary>
        public const byte Under21WithOver1Khours = 1;

        /// <summary>
        /// TERMINATED EMPLOYEE > 1000 HOURS WORKED GETS YEAR VESTED
        /// </summary>
        public const byte TerminatedEmployeeOver1000HoursWorkedGetsYearVested = 2;

        /// <summary>
        /// OVER 64 AND < 1000 HOURS GETS 1 YEAR VESTING (obsolete 11/20)
        /// </summary>
        [Obsolete]
        public const byte Over64WithLess1000Hours1YearVesting = 3;

        /// <summary>
        /// OVER 64 AND < 1000 HOURS GETS 2 YEARS VESTING (obsolete 11/20)
        /// </summary>
        [Obsolete]
        public const byte Over64WithLess1000Hours2YearsVesting = 4;

        /// <summary>
        /// OVER 64 AND > 1000 HOURS GETS 3 YEARS VESTING (obsolete 11/20)
        /// </summary>
        [Obsolete]
        public const byte Over64WithOver1000Hours3YearsVesting = 5;

        /// <summary>
        /// >=65 AND 1st CONTRIBUTION >= 5 YEARS AGO GETS 100% VESTED
        /// </summary>
        public const byte SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested = 6;

        /// <summary>
        /// =64 AND 1ST CONTRIBUTION >=5 YEARS AGO GETS 100% VESTED ON THEIR BIRTHDAY
        /// </summary>
        public const byte SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay = 7;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }

    public ICollection<PayProfit>? Profits { get; set; }
}
