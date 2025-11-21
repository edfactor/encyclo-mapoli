namespace YEMatch.Activities;

/// <summary>
///     Enumeration of all available activities in the YEMatch testing framework.
///     Provides compile-time safety and IntelliSense support for activity selection.
/// </summary>
public enum ActivityName
{
    // ====== READY Activities (R00-R29) ======
    /// <summary>Creates new database by importing from PROFITSHARE schema</summary>
    R00_BuildDatabase,

    /// <summary>Profit Sharing Clean Up Reports (SSN cleanup)</summary>
    R01_CleanUpReports,

    /// <summary>Military and Rehire (TERM-REHIRE)</summary>
    R02_MilitaryAndRehire,

    /// <summary>Prof Termination (PROF-TERM)</summary>
    R03_ProfTermination,

    /// <summary>Prof Share Loan Balance (QRY-PSLOAN)</summary>
    R04_ProfShareLoanBalance,

    /// <summary>Extract Executive Hours and Dollars (PROF-DOLLAR-EXEC-EXTRACT)</summary>
    R05_ExtractExecutiveHoursAndDollars,

    /// <summary>Clear Executive Hours and Dollars (PAYPROFIT-CLEAR-EXEC)</summary>
    R06_ClearExecutiveHoursAndDollars,

    /// <summary>Ready Screen 008-09 (Enter exec hours)</summary>
    R07_ReadyScreen00809,

    /// <summary>Profit Share Report (PROF-SHARE - duplicated in frozen section)</summary>
    R08_ProfitShareReport,

    /// <summary>YE Oracle Payroll Processing</summary>
    R09_YEOraclePayrollProcessing,

    /// <summary>Load Oracle PAYPROFIT (weekly job)</summary>
    R10_LoadOraclePayProfit,

    /// <summary>Profit Sharing YTD Wages Extract (PROF-DOLLAR-EXTRACT)</summary>
    R11_ProfitSharingYTDWagesExtract,

    /// <summary>PROF LOAD YREND DEMO PROFSHARE</summary>
    R12_ProfLoadYrEndDemoProfitShare,

    /// <summary>PAYPROFIT SHIFT (shift three columns) - Update Part-Time</summary>
    R13A_PayProfitShiftPartTime,

    /// <summary>PAYPROFIT SHIFT (shift three columns) - Update Weekly</summary>
    R13B_PayProfitShiftWeekly,

    /// <summary>ZERO-PY-PD-PAYPROFIT (zero three columns)</summary>
    R14_ZeroPyPdPayProfit,

    /// <summary>Profit Sharing YTD Wages Extract (PROF-DOLLAR-EXTRACT)</summary>
    R15_ProfitSharingYTDWagesExtract2,

    /// <summary>READY Screen 008-09 (Second chance to enter exec hours)</summary>
    R16_ReadyScreen00809Second,

    /// <summary>Profit Share Report Edit Run (PROF-SHARE)</summary>
    R17_ProfitShareReportEditRun,

    /// <summary>Profit Share Report Final Run (PROF-SHARE)</summary>
    R18_ProfitShareReportFinalRun,

    /// <summary>Get Eligible Employees (GET-ELIGIBLE-EMPS)</summary>
    R19_GetEligibleEmployees,

    /// <summary>Profit Forfeit PAY443 (PROF-FORT)</summary>
    R20_ProfitForfeit,

    /// <summary>Profit Share Update PAY444 (PROF-UPD1)</summary>
    R21_ProfitShareUpdate,

    /// <summary>Profit Share Edit PAY477 (PROF-EDIT)</summary>
    R22_ProfitShareEdit,

    /// <summary>Profit Master Update (PROF-DBUPD)</summary>
    R23_ProfitMasterUpdate,

    /// <summary>PROF PAYMASTER UPD (PROF-UPD2)</summary>
    R24_ProfPayMasterUpdate,

    /// <summary>PROF PAYMASTER UPD Part Two (PROF-UPD2)</summary>
    R24B_ProfPayMasterUpdatePartTwo,

    /// <summary>Prof Share Report By Age (PROFSHARE-RPT)</summary>
    R25_ProfShareReportByAge,

    /// <summary>Prof Share Gross Report QPAY501 (PROFGROSS)</summary>
    R26_ProfShareGrossReport,

    /// <summary>Prof Share by Store (PROF-BREAK)</summary>
    R27_ProfShareByStore,

    /// <summary>Print Profit Certs (PROF-CERT01)</summary>
    R28_PrintProfitCerts,

    /// <summary>Save Prof Paymstr - final backup to tape (SAVE-PROF-PAYMSTR)</summary>
    R29_SaveProfPayMstr,

    // ====== SMART Activities (S00-S29) ======
    /// <summary>Initialize Database with Obfuscated data</summary>
    S00_InitializeDatabase,

    /// <summary>Profit Sharing Clean Up Reports</summary>
    S01_CleanUpReports,

    /// <summary>Military and Rehire</summary>
    S02_MilitaryAndRehire,

    /// <summary>Prof Termination</summary>
    S03_ProfTermination,

    /// <summary>Prof Share Loan Balance (Distributions and Forfeitures QPAY129)</summary>
    S04_ProfShareLoanBalance,

    /// <summary>Extract Executive Hours and Dollars</summary>
    S05_ExtractExecutiveHoursAndDollars,

    /// <summary>Clear Executive Hours and Dollars</summary>
    S06_ClearExecutiveHoursAndDollars,

    /// <summary>Ready Screen 008-09 (Enter exec hours)</summary>
    S07_ReadyScreen00809,

    /// <summary>Profit Share Report (duplicated in frozen section) (pay426)</summary>
    S08_ProfitShareReport,

    /// <summary>YE Oracle HCM Payroll Processing</summary>
    S09_YEOraclePayrollProcessing,

    /// <summary>Load Oracle PAYPROFT (weekly job)</summary>
    S10_LoadOraclePayProfit,

    /// <summary>Profit Sharing YTD Wages Extract</summary>
    S11_ProfitSharingYTDWagesExtract,

    /// <summary>PROF LOAD YREND DEMO PROFSHARE</summary>
    S12_ProfLoadYrEndDemoProfitShare,

    /// <summary>PAYPROFIT SHIFT (shift three columns) - Part-Time</summary>
    S13A_PayProfitShiftPartTime,

    /// <summary>PAYPROFIT SHIFT (shift three columns) - Weekly</summary>
    S13B_PayProfitShiftWeekly,

    /// <summary>ZERO-PY-PD-PAYPROFIT (zero three columns)</summary>
    S14_ZeroPyPdPayProfit,

    /// <summary>Profit Sharing YTD Wages Extract</summary>
    S15_ProfitSharingYTDWagesExtract2,

    /// <summary>READY Screen 008-09</summary>
    S16_ReadyScreen00809Second,

    /// <summary>Profit Share Report (Edit Run)</summary>
    S17_ProfitShareReportEditRun,

    /// <summary>Profit Share Report (Final Run)</summary>
    S18_ProfitShareReportFinalRun,

    /// <summary>Get Eligible Employees</summary>
    S19_GetEligibleEmployees,

    /// <summary>Profit Share Forfeit (pay443)</summary>
    S20_ProfitForfeit,

    /// <summary>Profit Share Update (pay444)</summary>
    S21_ProfitShareUpdate,

    /// <summary>Profit Share Edit (pay447)</summary>
    S22_ProfitShareEdit,

    /// <summary>Profit Master Update</summary>
    S23_ProfitMasterUpdate,

    /// <summary>PROF PAYMASTER UPD</summary>
    S24_ProfPayMasterUpdate,

    /// <summary>PROF PAYMASTER UPD part two</summary>
    S24B_ProfPayMasterUpdatePartTwo,

    /// <summary>Prof Share Report By Age (prof130 prof130b prof130v prof130y)</summary>
    S25_ProfShareReportByAge,

    /// <summary>Prof Share Gross Rpt (qpay501)</summary>
    S26_ProfShareGrossReport,

    /// <summary>Prof Share by Store (qpay066)</summary>
    S27_ProfShareByStore,

    /// <summary>Print Profit Certs (paycert)</summary>
    S28_PrintProfitCerts,

    /// <summary>Save Prof Paymstr final job to backup tables to tape</summary>
    S29_SaveProfPayMstr,

    // ====== Parallel Activities (P00-P29) ======
    /// <summary>Parallel: Build Database (R00 + S00)</summary>
    P00_BuildDatabase,

    /// <summary>Parallel: Clean Up Reports (R01 + S01)</summary>
    P01_CleanUpReports,

    /// <summary>Parallel: Military and Rehire (R02 + S02)</summary>
    P02_MilitaryAndRehire,

    /// <summary>Parallel: Prof Termination (R03 + S03)</summary>
    P03_ProfTermination,

    /// <summary>Parallel: Prof Share Loan Balance (R04 + S04)</summary>
    P04_ProfShareLoanBalance,

    /// <summary>Parallel: Extract Executive Hours and Dollars (R05 + S05)</summary>
    P05_ExtractExecutiveHoursAndDollars,

    /// <summary>Parallel: Clear Executive Hours and Dollars (R06 + S06)</summary>
    P06_ClearExecutiveHoursAndDollars,

    /// <summary>Parallel: Ready Screen 008-09 (R07 + S07)</summary>
    P07_ReadyScreen00809,

    /// <summary>Parallel: Profit Share Report (R08 + S08)</summary>
    P08_ProfitShareReport,

    /// <summary>Parallel: YE Oracle Payroll Processing (R09 + S09)</summary>
    P09_YEOraclePayrollProcessing,

    /// <summary>Parallel: Load Oracle PAYPROFIT (R10 + S10)</summary>
    P10_LoadOraclePayProfit,

    /// <summary>Parallel: Profit Sharing YTD Wages Extract (R11 + S11)</summary>
    P11_ProfitSharingYTDWagesExtract,

    /// <summary>Parallel: PROF LOAD YREND DEMO PROFSHARE (R12 + S12)</summary>
    P12_ProfLoadYrEndDemoProfitShare,

    /// <summary>Parallel: PAYPROFIT SHIFT Part-Time (R13A + S13A)</summary>
    P13A_PayProfitShiftPartTime,

    /// <summary>Parallel: PAYPROFIT SHIFT Weekly (R13B + S13B)</summary>
    P13B_PayProfitShiftWeekly,

    /// <summary>Parallel: ZERO-PY-PD-PAYPROFIT (R14 + S14)</summary>
    P14_ZeroPyPdPayProfit,

    /// <summary>Parallel: Profit Sharing YTD Wages Extract (R15 + S15)</summary>
    P15_ProfitSharingYTDWagesExtract2,

    /// <summary>Parallel: READY Screen 008-09 Second (R16 + S16)</summary>
    P16_ReadyScreen00809Second,

    /// <summary>Parallel: Profit Share Report Edit Run (R17 + S17)</summary>
    P17_ProfitShareReportEditRun,

    /// <summary>Parallel: Profit Share Report Final Run (R18 + S18)</summary>
    P18_ProfitShareReportFinalRun,

    /// <summary>Parallel: Get Eligible Employees (R19 + S19)</summary>
    P19_GetEligibleEmployees,

    /// <summary>Parallel: Profit Forfeit (R20 + S20)</summary>
    P20_ProfitForfeit,

    /// <summary>Parallel: Profit Share Update (R21 + S21)</summary>
    P21_ProfitShareUpdate,

    /// <summary>Parallel: Profit Share Edit (R22 + S22)</summary>
    P22_ProfitShareEdit,

    /// <summary>Parallel: Profit Master Update (R23 + S23)</summary>
    P23_ProfitMasterUpdate,

    /// <summary>Parallel: PROF PAYMASTER UPD (R24 + S24)</summary>
    P24_ProfPayMasterUpdate,

    /// <summary>Parallel: PROF PAYMASTER UPD Part Two (R24B + S24B)</summary>
    P24B_ProfPayMasterUpdatePartTwo,

    /// <summary>Parallel: Prof Share Report By Age (R25 + S25)</summary>
    P25_ProfShareReportByAge,

    /// <summary>Parallel: Prof Share Gross Report (R26 + S26)</summary>
    P26_ProfShareGrossReport,

    /// <summary>Parallel: Prof Share by Store (R27 + S27)</summary>
    P27_ProfShareByStore,

    /// <summary>Parallel: Print Profit Certs (R28 + S28)</summary>
    P28_PrintProfitCerts,

    /// <summary>Parallel: Save Prof Paymstr (R29 + S29)</summary>
    P29_SaveProfPayMstr,

    // ====== Test/Assert Activities ======
    /// <summary>Test selected columns from PayProfit table match between READY and SMART</summary>
    TestPayProfitSelectedColumns,

    /// <summary>Test selected columns from ProfitDetail table match between READY and SMART</summary>
    TestProfitDetailSelectedColumns,

    /// <summary>Test ETVA Now values match between systems</summary>
    TestEtvaNow,

    /// <summary>Test ETVA Prior values match between systems</summary>
    TestEtvaPrior,

    /// <summary>Test database views match between systems</summary>
    TestViews,

    /// <summary>Test Master Inquiry report output matches</summary>
    TestMasterInquiry,

    // ====== Arrange Activities ======
    /// <summary>Import data from READY database to SMART database</summary>
    ImportReadyDbToSmartDb,

    /// <summary>Trim dataset to 14 employees for testing</summary>
    TrimTo14Employees,

    /// <summary>Drop bad beneficiary records from READY</summary>
    DropBadBenesReady,

    /// <summary>Sanity check for employee and beneficiary data integrity</summary>
    SanityCheckEmployeeAndBenes,

    /// <summary>Overwrite badge numbers for testing</summary>
    OverwriteBadges,

    /// <summary>Set date of birth to 19 years ago for all employees</summary>
    SetDateOfBirthTo19YearsAgo,

    /// <summary>Update navigation tree in SMART system</summary>
    UpdateNavigation,

    /// <summary>execute profit master update integration test - runs contributions on SMART and compares them to READY.</summary>
    IntProfitMasterUpdateTest,

    /// <summary>Dump Master Inquiry report from READY</summary>
    MasterInquiryDumper,

    // ====== Integration Test Activities ======
    /// <summary>Integration test for PAY443 (Forfeitures and Points)</summary>
    IntPay443,

    /// <summary>Integration test for terminated employee processing</summary>
    IntTerminatedEmployee,

    /// <summary>Integration test for PAY426 data updates</summary>
    IntTestPay426DataUpdates,

    /// <summary>Integration test for QPAY129 (Distributions and Forfeitures)</summary>
    IntTestQPay129,

    /// <summary>Integration test for PAY426N variant 9</summary>
    IntPay426N9,

    /// <summary>Integration test for PAY426N report</summary>
    IntPay426N,

    /// <summary>Integration test for PAY426 report</summary>
    IntPay426,

    IntPay450,

    IntPay444Test,

    IntPay447Test
}
