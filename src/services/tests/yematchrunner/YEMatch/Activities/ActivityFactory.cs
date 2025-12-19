using System.Diagnostics;
using YEMatch.AssertActivities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;
using YEMatch.SmartIntegrationTests;

namespace YEMatch.Activities;

public sealed class ActivityFactory : IActivityFactory
{
    private readonly Dictionary<ActivityName, IActivity> _activitiesByName;

    private readonly List<IActivity> _integrationActivities;
    // Ready activities which modify the system
    //  private readonly HashSet<string> activitiesWithUpdates = ["A6", "A7", "A12", "A13A", "A13B", "A18", "A21", "A22", "A23", "A24"]

    private readonly List<IActivity> _parallelActivities = [];
    private readonly List<IActivity> _readyActivities;
    private readonly List<IActivity> _smartActivities;
    private readonly List<IActivity> _testActivities;

    public ActivityFactory(
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        IIntegrationTestFactory integrationTestFactory,
        string dataDirectory)
    {
        ArgumentNullException.ThrowIfNull(readySshClientFactory);
        ArgumentNullException.ThrowIfNull(smartApiClientFactory);
        ArgumentNullException.ThrowIfNull(integrationTestFactory);
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);

        Stopwatch wholeRunStopWatch = new();
        wholeRunStopWatch.Start();

        _smartActivities = smartApiClientFactory.CreateActivities(dataDirectory);
        _readyActivities = readySshClientFactory.CreateActivities(dataDirectory);

        // Pass all required dependencies to TestActivityFactory
        _testActivities = TestActivityFactory.CreateActivities(
            dataDirectory,
            readySshClientFactory.GetSshClient(),
            readySshClientFactory.GetSftpClient(),
            true, // Will be updated if SetNewScramble() is called
            smartApiClientFactory.CreateClient());

        if (_readyActivities.Count != _smartActivities.Count)
        {
            throw new InvalidOperationException("READY and SMART activities are different length");
        }

        for (int i = 0; i < _readyActivities.Count; i++)
        {
            if (_smartActivities[i].Name().Substring(1) != _readyActivities[i].Name().Substring(1))
            // We always expect Ready short name to be the same as Smart short name (ie.  A7 and A7 should match.)
            {
                throw new InvalidOperationException(
                    $"READY and SMART activities are different at index {i}  s={_smartActivities[i].Name()} r={_readyActivities[i].Name()}");
            }

            _parallelActivities.Add(new ParallelActivity("P" + _readyActivities[i].Name().Substring(1),
                _readyActivities[i], _smartActivities[i]));
        }

        _integrationActivities = integrationTestFactory.CreateActivities();

        // Pre-compute activities dictionary for fast lookup with enum keys
        _activitiesByName = _smartActivities
            .Concat(_readyActivities)
            .Concat(_testActivities)
            .Concat(_parallelActivities)
            .Concat(_integrationActivities)
            .ToDictionary(a => MapStringToEnum(a.Name()), a => a);
    }

    public Dictionary<ActivityName, IActivity> GetActivitiesByName()
    {
        return _activitiesByName;
    }

    public IActivity GetActivity(ActivityName name)
    {
        if (_activitiesByName.TryGetValue(name, out IActivity? activity))
        {
            return activity;
        }

        throw new KeyNotFoundException($"Activity '{name}' not found");
    }

    public void SetNewScramble()
    {
        // This is a hack to reach into a class and fiddle with it, but we are still thinking
        // about how to dynamicaly switch between "classic" and "new" scramble choices.
        ReadyActivity ra = (ReadyActivity)_activitiesByName[ActivityName.R00_BuildDatabase];
        ra.Args = "profitshare"; // lock this Runner to the latest schema.
    }

    public bool IsNewScramble()
    {
        ReadyActivity ra = (ReadyActivity)_activitiesByName[ActivityName.R00_BuildDatabase];
        return ra.Args == "" || ra.Args == "profitshare";
    }

    /// <summary>
    ///     Maps string activity names (from IActivity.Name()) to ActivityName enum values
    /// </summary>
    private static ActivityName MapStringToEnum(string name)
    {
        return name switch
        {
            // READY activities
            "R00" => ActivityName.R00_BuildDatabase,
            "R01" => ActivityName.R01_CleanUpReports,
            "R02" => ActivityName.R02_MilitaryAndRehire,
            "R03" => ActivityName.R03_ProfTermination,
            "R04" => ActivityName.R04_ProfShareLoanBalance,
            "R05" => ActivityName.R05_ExtractExecutiveHoursAndDollars,
            "R06" => ActivityName.R06_ClearExecutiveHoursAndDollars,
            "R07" => ActivityName.R07_ReadyScreen00809,
            "R08" => ActivityName.R08_ProfitShareReport,
            "R09" => ActivityName.R09_YEOraclePayrollProcessing,
            "R10" => ActivityName.R10_LoadOraclePayProfit,
            "R11" => ActivityName.R11_ProfitSharingYTDWagesExtract,
            "R12" => ActivityName.R12_ProfLoadYrEndDemoProfitShare,
            "R13A" => ActivityName.R13A_PayProfitShiftPartTime,
            "R13B" => ActivityName.R13B_PayProfitShiftWeekly,
            "R14" => ActivityName.R14_ZeroPyPdPayProfit,
            "R15" => ActivityName.R15_ProfitSharingYTDWagesExtract2,
            "R16" => ActivityName.R16_ReadyScreen00809Second,
            "R17" => ActivityName.R17_ProfitShareReportEditRun,
            "R18" => ActivityName.R18_ProfitShareReportFinalRun,
            "R19" => ActivityName.R19_GetEligibleEmployees,
            "R20" => ActivityName.R20_ProfitForfeit,
            "R21" => ActivityName.R21_ProfitShareUpdate,
            "R22" => ActivityName.R22_ProfitShareEdit,
            "R23" => ActivityName.R23_ProfitMasterUpdate,
            "R24" => ActivityName.R24_ProfPayMasterUpdate,
            "R24B" => ActivityName.R24B_ProfPayMasterUpdatePartTwo,
            "R25" => ActivityName.R25_ProfShareReportByAge,
            "R26" => ActivityName.R26_ProfShareGrossReport,
            "R27" => ActivityName.R27_ProfShareByStore,
            "R28" => ActivityName.R28_PrintProfitCerts,
            "R29" => ActivityName.R29_SaveProfPayMstr,

            // SMART activities
            "S00" => ActivityName.S00_InitializeDatabase,
            "S01" => ActivityName.S01_CleanUpReports,
            "S02" => ActivityName.S02_MilitaryAndRehire,
            "S03" => ActivityName.S03_ProfTermination,
            "S04" => ActivityName.S04_ProfShareLoanBalance,
            "S05" => ActivityName.S05_ExtractExecutiveHoursAndDollars,
            "S06" => ActivityName.S06_ClearExecutiveHoursAndDollars,
            "S07" => ActivityName.S07_ReadyScreen00809,
            "S08" => ActivityName.S08_ProfitShareReport,
            "S09" => ActivityName.S09_YEOraclePayrollProcessing,
            "S10" => ActivityName.S10_LoadOraclePayProfit,
            "S11" => ActivityName.S11_ProfitSharingYTDWagesExtract,
            "S12" => ActivityName.S12_ProfLoadYrEndDemoProfitShare,
            "S13A" => ActivityName.S13A_PayProfitShiftPartTime,
            "S13B" => ActivityName.S13B_PayProfitShiftWeekly,
            "S14" => ActivityName.S14_ZeroPyPdPayProfit,
            "S15" => ActivityName.S15_ProfitSharingYTDWagesExtract2,
            "S16" => ActivityName.S16_ReadyScreen00809Second,
            "S17" => ActivityName.S17_ProfitShareReportEditRun,
            "S18" => ActivityName.S18_ProfitShareReportFinalRun,
            "S19" => ActivityName.S19_GetEligibleEmployees,
            "S20" => ActivityName.S20_ProfitForfeit,
            "S21" => ActivityName.S21_ProfitShareUpdate,
            "S22" => ActivityName.S22_ProfitShareEdit,
            "S23" => ActivityName.S23_ProfitMasterUpdate,
            "S24" => ActivityName.S24_ProfPayMasterUpdate,
            "S24B" => ActivityName.S24B_ProfPayMasterUpdatePartTwo,
            "S25" => ActivityName.S25_ProfShareReportByAge,
            "S26" => ActivityName.S26_ProfShareGrossReport,
            "S27" => ActivityName.S27_ProfShareByStore,
            "S28" => ActivityName.S28_PrintProfitCerts,
            "S29" => ActivityName.S29_SaveProfPayMstr,

            // Parallel activities
            "P00" => ActivityName.P00_BuildDatabase,
            "P01" => ActivityName.P01_CleanUpReports,
            "P02" => ActivityName.P02_MilitaryAndRehire,
            "P03" => ActivityName.P03_ProfTermination,
            "P04" => ActivityName.P04_ProfShareLoanBalance,
            "P05" => ActivityName.P05_ExtractExecutiveHoursAndDollars,
            "P06" => ActivityName.P06_ClearExecutiveHoursAndDollars,
            "P07" => ActivityName.P07_ReadyScreen00809,
            "P08" => ActivityName.P08_ProfitShareReport,
            "P09" => ActivityName.P09_YEOraclePayrollProcessing,
            "P10" => ActivityName.P10_LoadOraclePayProfit,
            "P11" => ActivityName.P11_ProfitSharingYTDWagesExtract,
            "P12" => ActivityName.P12_ProfLoadYrEndDemoProfitShare,
            "P13A" => ActivityName.P13A_PayProfitShiftPartTime,
            "P13B" => ActivityName.P13B_PayProfitShiftWeekly,
            "P14" => ActivityName.P14_ZeroPyPdPayProfit,
            "P15" => ActivityName.P15_ProfitSharingYTDWagesExtract2,
            "P16" => ActivityName.P16_ReadyScreen00809Second,
            "P17" => ActivityName.P17_ProfitShareReportEditRun,
            "P18" => ActivityName.P18_ProfitShareReportFinalRun,
            "P19" => ActivityName.P19_GetEligibleEmployees,
            "P20" => ActivityName.P20_ProfitForfeit,
            "P21" => ActivityName.P21_ProfitShareUpdate,
            "P22" => ActivityName.P22_ProfitShareEdit,
            "P23" => ActivityName.P23_ProfitMasterUpdate,
            "P24" => ActivityName.P24_ProfPayMasterUpdate,
            "P24B" => ActivityName.P24B_ProfPayMasterUpdatePartTwo,
            "P25" => ActivityName.P25_ProfShareReportByAge,
            "P26" => ActivityName.P26_ProfShareGrossReport,
            "P27" => ActivityName.P27_ProfShareByStore,
            "P28" => ActivityName.P28_PrintProfitCerts,
            "P29" => ActivityName.P29_SaveProfPayMstr,

            // Test activities
            nameof(TestPayProfitSelectedColumns) => ActivityName.TestPayProfitSelectedColumns,
            nameof(TestProfitDetailSelectedColumns) => ActivityName.TestProfitDetailSelectedColumns,
            nameof(TestEtvaNow) => ActivityName.TestEtvaNow,
            nameof(TestEtvaPrior) => ActivityName.TestEtvaPrior,
            nameof(TestViews) => ActivityName.TestViews,
            nameof(TestMasterInquiry) => ActivityName.TestMasterInquiry,

            // Arrange activities
            "ImportReadyDbToSmartDb" => ActivityName.ImportReadyDbToSmartDb,
            "TrimTo14Employees" => ActivityName.TrimTo14Employees,
            "DropBadBenesReady" => ActivityName.DropBadBenesReady,
            "SanityCheckEmployeeAndBenes" => ActivityName.SanityCheckEmployeeAndBenes,
            "OverwriteBadges" => ActivityName.OverwriteBadges,
            "SetDateOfBirthTo19YearsAgo" => ActivityName.SetDateOfBirthTo19YearsAgo,
            "UpdateNavigation" => ActivityName.UpdateNavigation,
            "MasterInquiryDumper" => ActivityName.MasterInquiryDumper,

            // Integration test activities
            "IntPay443" => ActivityName.IntPay443,
            "IntTerminatedEmployee" => ActivityName.IntTerminatedEmployee,
            "IntTestPay426DataUpdates" => ActivityName.IntTestPay426DataUpdates,
            "IntTestQPay129" => ActivityName.IntTestQPay129,
            "IntPay426N9" => ActivityName.IntPay426N9,
            "IntPay426N" => ActivityName.IntPay426N,
            "IntPay426" => ActivityName.IntPay426,
            "IntProfitMasterUpdateTest" => ActivityName.IntProfitMasterUpdateTest,
            "IntPay450" => ActivityName.IntPay450,
            "IntPay444Test" => ActivityName.IntPay444Test,
            "IntPay447Test" => ActivityName.IntPay447Test,
            "TestEnrollmentComparison" => ActivityName.TestEnrollmentComparison,

            _ => throw new ArgumentException($"Unknown activity name: {name}", nameof(name))
        };
    }
}
