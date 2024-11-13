namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;


public class PAY444
{
    string DAEMON_ACCEPT_BACK;
    string DAEMON_DISP_PROG;
    string DAEMON_DISP_MSG;
    string DAEMON_DISP_DISPLAY = "Dummy";
    string? DAEMON_ACCEPT_PROG;

    Dictionary<int, int> META_SW = new Dictionary<int, int>();

    string SPACES = " ";
    ClientTot client_tot = new();
    CONSOLE_RESPONSE console_response = new();
    CONSOLE_RESPONSE_MASKS console_response_masks = new();
    DEM_REC dem_rec = new();
    EMPLOYEE_COUNT_TOT employee_count_tot = new();
    EMPLOYEE_COUNT_TOT_PAYBEN employee_count_tot_payben = new();
    GRAND_TOT grand_tot = new();
    HEADER_1 header_1 = new();
    HEADER_2 header_2 = new();
    HEADER_3 header_3 = new();
    HOLD_KEY hold_key = new();
    INPUT_DATES input_dates = new();
    INTERMEDIATE_VALUES intermediate_values = new();
    PAYBEN_REC payben_rec = new();
    PAYBEN1_REC payben1_rec = new();
    PAYPROF_REC payprof_rec = new();
    POINT_VALUES point_values = new();
    PRFT prft = new();
    PRINT_ADJ_LINE1 print_adj_line1 = new();
    PROFIT_DETAIL profit_detail = new();
    PROFIT_SS_DETAIL profit_ss_detail = new();
    REPORT_LINE report_line = new();
    REPORT_LINE_2 report_line_2 = new();
    RERUN_TOT rerun_tot = new();
    SD_PRFT sd_prft = new();
    TOTAL_HEADER_1 total_header_1 = new();
    TOTAL_HEADER_2 total_header_2 = new();
    TOTAL_HEADER_3 total_header_3 = new();
    WS_CLIENT_TOTALS ws_client_totals = new();
    WS_COMPUTE_TOTALS ws_compute_totals = new();
    WS_COUNTERS ws_counters = new();
    WS_DATE_TIME ws_date_time = new();
    WS_ENDING_BALANCE ws_ending_balance = new();
    WS_GRAND_TOTALS ws_grand_totals = new();
    WS_INDICATORS ws_indicators = new();
    WS_MAXCONT_TOTALS ws_maxcont_totals = new();
    WS_PAYPROFIT ws_payprofit = new();
    WS_PROFIT_YEAR ws_profit_year = new();
    WS_SOCIAL_SECURITY ws_social_security = new();
    XEROX_HEADER xerox_header = new();
    //- -___- COPY X_FD_DEMOGRAPHICS.
    //- * ---------------------------------------------------------------
    //- * X-FD-DEMOGRAPHICS.cpy
    //- *     Modification History
    //- *  Date                 Description
    //- * 1/19/21  NFerrin
    //- * 8/27/21  Phil Giuffrida  MAIN-1431 Added PY-SHOUR
    //- * 10/7/21  NFerrin         MAIN-1461 Added PY-GUID,
    //- *                          PY-SET-PWD, PY-SET-PWD-DT, PY-CLASS-DT
    //- * ---------------------------------------------------------------
    //- -___- END of COPY X_FD_DEMOGRAPHICS.
    //- -___- COPY X_FD_PAYPROFIT.
    //- * ------------------------------------------------------
    //- * X-FD-PAYPROFIT.cpy
    //- *     Modification History
    //- *  Date                 Description
    //- * 1/19/21  NFerrin
    //- * 10/11/2023  N.Ferrin  MAIN-1956 Adding PY-PH-EXEC
    //- *                       and PY-PD-EXEC
    //- * -------------------------------------------------------
    //- -___- END of COPY X_FD_PAYPROFIT.
    //- -___- COPY X_FD_PAYBEN.
    //- -___- END of COPY X_FD_PAYBEN.
    //- 77  IDS2_CURRENT             PIC X(7) IS GLOBAL.
    //- 77  IDS2_REC_NAME            PIC X(30) IS GLOBAL.
    //- 77  IDS2_SET_NAME            PIC X(30) IS GLOBAL.
    //- 88 DB_CONDITION_TRUE     VALUE 0.
    //- 88 DB_CONDITION_FALSE    VALUE 1.
    //- 77  IDS2_NAVIGATION_MODE     PIC X(30) IS GLOBAL.
    //- 77  RELATIVE_POS             PIC S9(9) IS GLOBAL.

    //- -___- COPY UWA_REC_profit_ss_detail.

    //- *******  UWA RECORD FOR SALES-MSTR

    //- -___- END of COPY UWA_REC_profit_ss_detail.
    //- -___- COPY UWA_REC_profit_detail.

    //- *******  UWA RECORD FOR SALES-MSTR

    //- -___- END of COPY UWA_REC_profit_detail.
    //- -___- COPY UWA_REC_SOC_SEC_REC.

    //- *******  UWA RECORD FOR SALES-MSTR

    //- -___- END of COPY UWA_REC_SOC_SEC_REC.
    //- $set fcdreg
    //- IDENTIFICATION DIVISION.
    //- PROGRAM_ID.  PAY444.
    //- AUTHOR.  J.D.MAGEE.
    //- DATE_WRITTEN.
    //- ******************************************************************
    //- *
    //- *    M O D I F I C A T I O N S
    //- *
    //- *  01/04/91  M.MCTEAGUE   TOOK LEE DRUG CLIENT 61 QUESTIONS OUT
    //- *                         AND INITIALIZED THE VALUES FOR IT
    //- *
    //- *  11/21/94  ED STEVENSON  ADDED FILE PROFT-A1 AND YEAR LOGIC. REMOVE
    //- *                          FORF LOGIC.
    //- *
    //- *  3/8/96  LU DION      TOOK CLIENT 50 QUESTIONS OUT AND INITIALIZED THE
    //- *                       VALUES FOR IT.  SEE PROJECT CONTROL SHEET # 341300
    //- *
    //- *  5/5/97  J.WILKIN     CHANGED/ADDED 'YEAR 2000' CODE.
    //- *
    //- * 01/28/98 R. MAISON    ALTERED PROGRAM TO TAKE IN A SEQUENTIAL
    //- *                       PAYROLL MASTER IN CLIENT SSN AND PS-AMT SEQ IN
    //- *                       ORDER TO ACCUMULATE THE TOTAL PS-AMT FOR MATCHING
    //- *                       SSNS AND THEN WALKING THE PROFIT SHARE DATA BASE
    //- *                       ONLY ONCE UPDATING THE PROFIT SHARING DATA BASE
    //- *                       AND PAYROLL MASTER  TO CORRECT INACCURATE NEGATIVE
    //- *                       EARNINGS
    //- *
    //- * 02/15/99   ED STEVENSON    ADD CHECK FOR PY-PS-YEARDS > ZERO TO
    //- *                            ELIGIBILITY CHECK, AND COMMENTED OUT
    //- *                            QUESTIONS FOR CLIENTS OTHER THAN 1.
    //- *
    //- * 2/23/99  R.LYON            REMOVED PRINTOUT FOR CLIENTS OTHER THAN 1.
    //- * 02/15/02  BOB CASON  PROJ. 90600 ADD PAGE NUMBER TO HEADING, ADD
    //- *                           COUNT OF EMPLOYEES PRINTED TO TOTAL PAGE.
    //- * 05/07/02  BOB CASON  PROJ. 92000 SUPRESS EMPLOYEES WITH ALL ZERO
    //- *                           AMOUNTS.
    //- *
    //- * 02/07/03  BOB CASON  PROJ. 4970  CHANGE LIMIT FOR WS-MAX (MAXIMUM
    //- *                            CONTRIBUTION) FROM 30000 TO 40000.
    //- *                            CHANGED AMOUNT IN "RERUN-TOT" LINE ALSO.
    //- *                            MOVE ZERO TO PY-PROF-MAXCONT BEFORE TEST
    //- *                            FOR LIMIT ON WS-MAX SO EMPLOYEES OVER
    //- *                            LIMIT ON FIRST PASS WILL BE PROCESSED IF
    //- *                            LIMIT IS INCREASED.
    //- * 01/22/04 J.MITCHELL PROJ 7567 INCREASE PROF-EARN, PROF-CONT,
    //- *                                  PROF-FORF FIELDS TO S9(06)V99.
    //- * 01/28/05 LINDA IRELAND PROJ #9147 ADJUSTMENT LOGIC:
    //- *                        1) ACCEPT ADJUSTMENT EMPLOYEE NUMBER
    //- *                           AND VALUES
    //- *                        2) ADJUST DATA BY ENTERED AMOUNTS AND
    //- *                        3) WRITE NEW REPORT OF ADJUSTMENTS
    //- *                        NEW FD:         PRINT-FILE2
    //- *                        NEW PARAGRAPHS: 118-ADJUST AND
    //- *                                        1000-ADJUSTMENT-REPORT
    //- *                        CHANGED PARA:   230-COMPUTE-CONTRIBUTION
    //- *                                        240-COMPUTE-FORFEITURES
    //- *                                        250-COMPUTE-EARNINGS
    //- *                        ALSO ENLARGED LOAN FIELDS FROM 9(6)V99
    //- *                                                    TO 9(7)V99
    //- *                             ON FILE AND WORKING STORAGE FIELDS
    //- *
    //- *  03/28/05  Becky Maison  Added function NUMVAL code as well as
    //- *  PROJ #9396              some cosmetic fixes to the displays
    //- *                          by adding a mask for the values that
    //- *                          display.
    //- *
    //- * DPrugh  #9492  11/30/05   Military Profit Sharing Entries will now be
    //- *                           indicated as CCYY.1 records.
    //- *                           These entries will be entered with specific
    //- *                           entry dates corresponding to when they are
    //- *                           to be calculated.
    //- *                           (PROFIT-MDTE and PROFIT-YDTE)
    //- *                           If the entry date is 2004 it is a 2004.0
    //- *                           record and should be counted as a year of
    //- *                           vesting. If the entry date is 1204 it is an
    //- *                           additional (supplemental) record for 2004
    //- *                           and  should not be counted as a year of
    //- *                           vesting. Utilize the LOAN2 column for
    //- *                           MILITARY entries.
    //- *
    //- * 12/07/05 BOB CASON   PROJ. 10443 CHANGE LIMIT ON WS-MAX (MAXIMUM
    //- *                            CONTRIBUTIONS) FROM 40000 TO 42000.
    //- *                            CHANGED AMOUNT IN "RERUN-TOT" LINE ALSO.
    //- *
    //- * 01/20/2006 L IRELAND      PROJECT # 10627; IN PARA 260-MAXCONT,
    //- *                           TAKE AMOUNT OVER MAX CONTRIBUTION
    //- *                           OUT OF FORFEITURES, NOT CONTRIBUTIONS
    //- *                           AND CHANGE DESCRIPTION OF THIS ON REPORT
    //- *
    //- * 19/19/2006 S Finnigan     PROJECT #11541 - INCREADED FILLER ON
    //- *                           HARD CODED PAYMSTR1 FD FROM 193 TO 258.
    //- *
    //- * 10/02/2007 A Mahoney      Project 12548 - Add 3 new fields to the
    //- *                           hard coded PAYMSTR1 FD. PY-PS-ETVA1,
    //- *                           PY-PRIOR-ETVA1 and PY-PROF-ETVA1.
    //- *
    //- * 10/05/2007 DPrugh         Project 12548 - Added code to handle the
    //- *                           ETVA fields and create/add '8' records
    //- *                           for the profit sharing file.
    //- *                           Commented out old client code.
    //- *
    //- * 2/07/08 Renda   PROJ. 13027 MAKE WS-MAX (MAXIMUM CONTRIBUTIONS)
    //- *                           A USER ENTERED FIELD
    //- *
    //- * 03/26/08  DPrugh    P#13152   Made changes to mask the SSN with
    //- *                               zeroes leaving only the last 4
    //- *                               digits. a SSN that was 123456789
    //- *                               will now show as 000006789
    //- *
    //- * 07/01/08  DPrugh    P#13379   Process profit-code 9 records just
    //- *                               like profit-code 1 and 3. Profit
    //- *                               code 9 records are offsetting records
    //- *                               to profit-code 8 earnings records and
    //- *                               are partial payments with another 1 or 3
    //- *                               record.
    //- *
    //- * 12/08/08  DPrugh    P#13834   Process Earnings 2 if switch[5] is on
    //- *                               these earnings will be stored in the
    //- *                               payroll master file in a field called
    //- *                               py-prof-earn2. these earnings are calculated
    //- *                               exactly like the other earnings except
    //- *                               with a different $/point value.
    //- *                               Removed SSN to make room for earnings2
    //- *
    //- * 10/05/09  DPrugh    #14096    Added Payben support for beneficiary
    //- *                               records. Print a separate report at the end
    //- *                               of all beneficiaries that are not employees.
    //- *
    //- * 11/09/09  Renda     P14845    Change filler in PAYMSTR SEQ
    //- *                                from 361 to 376
    //- *
    //- * 05/21/10  DPrugh    #15226    Changed LOAN to DIST and enlarged fields
    //- *                               to handle 1M.
    //- *
    //- * 09/08/10  DPrugh    #15226    Added ALLOC totals to bottom of report
    //- *                               which will total the contribution alloc
    //- *                               and PAID alloc monies which stayed in
    //- *                               the plan. Paid alloc's are in the same
    //- *                               column as the military entries but are
    //- *                               negative numbers since they are paid out.
    //- *
    //- * 01/26/11 Ed Stevenson	#16435  Added PY-QFIC-EMPLOYER1 & PY-YFIC-EMPLOYER1
    //- *                               to the end of hard coded workfile PAYMSTR1.
    //- *
    //- * 01/17/12 Ed Stevenson #17592  The hard-coded fd for the payroll master
    //- *                               work file was mis-aligned from the actual
    //- *                               file causing there to be non-numeric
    //- *                               data in the py-ps-etva and py-ps-amt fields.
    //- *                               Added 2 to the filler and added
    //- *                               PY-HEALTH-BEN-CUR1, PY-HEALTH-BEN-PRIOR1, and
    //- *                               PY-PAYMENT-TYPE1 to the end of the record.
    //- *
    //- * 03/07/12 Guy Sheldon  #17613  Replace hard-coded "PAYMSTR1" FD with
    //- *                               copylib "FD-PAYMSTR.cpy" for ease of
    //- *                               maintenance. Had to append "OF PAYR" or
    //- *                               "OF PAYR1" to all references of payroll
    //- *                               master fields, as appropriate.
    //- *
    //- * 01/14/15  DPrugh      #20205  Force Beneficiaries on to the PAY444 report
    //- *                               by making them py-prof-newemp = 2 and
    //- *                               giving them 0.01 ETVA if they have none
    //- *
    //- * 05/21/21  DSawyer   MAIN-1280 replaced PAYR with PAYPROFIT and
    //- *                               DEMOGRAPHICS table.
    //- *
    //- * 11/12/21  DSawyer   MAIN-1476 restored edit for profit-client = 1
    //- *                               and edit for ws-rewrite-which
    //- * 12/28/21 DSawyer      MAIN-1488  Replaced DEMOGRAPHICS with
    //- *                       DEMO_PROFSHARE for yr end profit sharing;
    //- *                       Added class action fund (CAF) to profit
    //- *                       file and earnings2 on the report.  Class action
    //- *                       fund records are year 2021.2 and profit-code 8.
    //- *                       This amount is for information only and
    //- *                       is not added to any other totals except
    //- *                       the earnings2 grand total.  Earnings2 was
    //- *                       used in 2008 only. Earnings2 is initialized
    //- *                       every year before year end profit sharing is
    //- *                       run.  See PAY456.cbl in PROF-SHARE.ksh.  Fixed
    //- *                       alignment for expanded PSN number.
    //- *
    //- * 11/10/22  C.Jiang  MAIN-1668 Correct the allocation totals on the report.
    //- *
    //- * 01/16/23  C.Jiang  MAIN-1701 Added "XFER>", "QDRO>" in allocation calculation.
    //- *                    This will correct the allocation and distirbution totals.
    //- *
    //- * 02/09/23  C.Jiang  MAIN-1768 Fix the truncated fields on Total line,
    //- *                    BEGINNING BALANCE, DISTRIB, Earnings and ENDING
    //- *                    BALANCE.
    //- ******************************************************************
    //- *   SWITCHES
    //- * Meta-sw (2) = 1 : Special Run
    //- * Meta-sw (3) = 1 : Do NOT ask for Input Values.
    //- * Meta-sw (4) = 1 : Manual Adjustments
    //- * Meta-sw (5) = 1 : Secondary Earnings
    //- * Meta-sw (8) = 1 : Do NOT update PAYR/PAYBEN
    //- ******************************************************************

    //- ENVIRONMENT DIVISION.
    //- CONFIGURATION SECTION.
    //- SOURCE_COMPUTER. UNIX_MF.
    //- OBJECT_COMPUTER. UNIX_MF.
    //- INPUT_OUTPUT SECTION.
    //- FILE_CONTROL.


    //- SELECT PAYPROFIT_FILE ASSIGN "PAYPROF1"
    //- ORGANIZATION LINE SEQUENTIAL.
    PayProfRecTableHelper PAYPROFIT_FILE = new PayProfRecTableHelper();

    //- SELECT PAYBEN1 ASSIGN "PAYBEN1"
    //- ORGANIZATION LINE SEQUENTIAL.
    PayBenReader PAYBEN1 = new PayBenReader();

    //- SELECT PRINT_FILE ASSIGN LINE ADVANCING "P1"
    //- ORGANIZATION LINE SEQUENTIAL.

    //- SELECT PRINT_FILE2 ASSIGN LINE ADVANCING "P2"
    //- ORGANIZATION LINE SEQUENTIAL.

    //- SELECT PROFIT ASSIGN "PROFIT"
    //- ORGANIZATION LINE SEQUENTIAL.

    //- SELECT SORT_FILE ASSIGN "SORTFL".

    //- /
    //- DATA DIVISION.
    //- FILE SECTION.

    //- *17613*
    //- COPY FD_PAYPROFIT REPLACING PAYPROF_REC BY payprof_rec1.
    PAYPROF_REC payprof_rec1 = new();

    //- FD  PAYBEN1
    //- BLOCK CONTAINS 3500 CHARACTERS
    //- LABEL RECORDS ARE STANDARD
    //- DATA RECORD IS payben1_rec.
    //- FD PRINT_FILE
    //- LABEL RECORDS ARE STANDARD
    //- DATA RECORD IS P_REC.
    //- 01  P_REC                       PIC X(136).
    public string? P_REC { get; set; }

    //- FD PRINT_FILE2
    //- LABEL RECORDS ARE STANDARD
    //- DATA RECORD IS P_REC2.
    //- 01  P_REC2                       PIC X(136).
    //- /

    //- FD PROFIT
    //- LABEL RECORDS ARE STANDARD.

    //- SD  SORT_FILE.

    //- /
    //- WORKING_STORAGE SECTION.
    //- COPY METADATA.
    //- COPY UWA_REC_SOC_SEC_REC.
    //- COPY UWA_REC_profit_detail.
    //- COPY UWA_REC_profit_ss_detail.
    //- COPY SALES_MSTR_RETAINING_FLAGS.
    //- COPY DB_REGISTERS.
    //- COPY IDSII_TECH_FIELDS.

    //- 01 UFAS_OPEN_MODE PIC X(6).
    //- COPY X_FD_PAYBEN.
    //- COPY X_FD_PAYPROFIT.
    //- COPY X_FD_DEMOGRAPHICS.

    //- 01 PAYPROFIT_FILE_STATUS PIC XX.
    public string? PAYPROFIT_FILE_STATUS { get; set; }
    //- 01 PAYBEN_FILE_STATUS  PIC XX.
    public string? PAYBEN_FILE_STATUS { get; set; }
    //- 01 DEMO_PROFSHARE_FILE_STATUS PIC XX.
    public string? DEMO_PROFSHARE_FILE_STATUS { get; set; } = "00";

    //- COPY WS_QUE_DISP.
    //- 01 INDATE             PIC X(06) VALUE SPACES.
    public string? INDATE { get; set; }
    //- 01  INDATE_CYMD REDEFINES INDATEX PIC 9(8).
    //- 01  YEARS         PIC 9(4) VALUE IS ZEROES.
    public long YEARS { get; set; }
    //- 01  AGE         PIC 9(4) VALUE IS ZEROES.
    public long AGE { get; set; }
    //- 01  WORK_CYMD REDEFINES WORK_DATEX PIC 9(8).
    //- 01  WS1_DOB               PIC 9(08) VALUE IS ZEROES.
    //- 01  FIRST_REC                                PIC 9    VALUE 0.
    public long FIRST_REC { get; set; }
    //- 01  FIRST_PAYBEN_REC                         PIC 9    VALUE 0.
    public long FIRST_PAYBEN_REC { get; set; }
    //- 01  HOLD_SSN                                 PIC 9(9) VALUE 0.
    public long HOLD_SSN { get; set; }
    //- 01  HOLD_PAYSSN                              PIC 9(9) VALUE 0.
    public long HOLD_PAYSSN { get; set; }
    //- 01  HOLD_KEY1                                PIC 9(9) VALUE 0.
    //- 01  HOLD_KEY                                 PIC 9(7) VALUE 0.
    public long HOLD_KEY { get; set; }
    //- 01  INVALID_CNT                              PIC 9(6) VALUE 0.
    public long INVALID_CNT { get; set; }
    //- 01  WK_PROF_POINTS                           PIC S9(5) VALUE 0.
    //- 01  HOLD_EFF_DATE        PIC 9(4)          VALUE ZERO.
    public long HOLD_EFF_DATE { get; set; }

    //- 01  WS_PROFIT_YEAR                  PIC 9(04)V9(01)  VALUE ZEROS.
    // also have ws_profit_year, so boot this -->    public decimal WS_PROFIT_YEAR { get; set; }

    //- 01  HOLD_PROF_FORF     PIC S9(6)V99.
    //- COPY GAC_WS.

    //- 01  ws_temp_psn                  pic x(11)      value "00000000000".
    public string WS_TEMP_PSN { get; set; }
    //- 01  WS_COUNTER                   PIC 9(4)      VALUE ZERO.
    public long WS_COUNTER { get; set; }
    //- 01  WS_REWRITE_WHICH             PIC 9(1)      VALUE ZERO.
    public long WS_REWRITE_WHICH { get; set; }
    //- 01  WS_ERROR                     PIC 9(1)      VALUE ZERO.
    public long WS_ERROR { get; set; }
    //- 01  WS_CONTR_MAX                 PIC 9(7)      VALUE ZERO.
    public long WS_CONTR_MAX { get; set; }
    //- 01  WS_PY_PS_ETVA                PIC S9(8)V99  VALUE ZERO.
    public decimal WS_PY_PS_ETVA { get; set; }
    //- 01  WS_ETVA_PERCENT              PIC S9(3)V999999  VALUE ZERO.
    public decimal WS_ETVA_PERCENT { get; set; }
    //- 01  WS_ETVA_AMT                  PIC S9(8)V99  VALUE ZERO.
    public decimal WS_ETVA_AMT { get; set; }
    //- 01  WS_ETVA2_AMT                 PIC S9(8)V99  VALUE ZERO.
    public decimal WS_ETVA2_AMT { get; set; }
    //- 01  DIST_TOTAL                   PIC S9(7)V99  VALUE ZERO.
    public decimal DIST_TOTAL { get; set; }
    //- 01  FORFEIT_TOTAL                  PIC S9(7)V99  VALUE ZERO.
    public decimal FORFEIT_TOTAL { get; set; }
    //- 01  ALLOCATION_TOTAL               PIC S9(7)V99  VALUE ZERO.
    public decimal ALLOCATION_TOTAL { get; set; }
    //- 01  PALLOCATION_TOTAL              PIC S9(7)V99  VALUE ZERO.

    public decimal PALLOCATION_TOTAL { get; set; }


    public int DB_STATUS { get; set; }

    //- /
    //- * INPUT FROM CONSOLE.ff

    //- LINKAGE SECTION.
    //- DEPENDING ON LENGTH_OF.

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- PROCEDURE DIVISION USING OPTION_LINK.

    //- 010_MAIN_PROCESSING SECTION.
    //- *     display "at 010-MAIN-PROCESSING".

    //- 015_MAIN_PROCESSING.
    public void m015MainProcessing(Dictionary<int, int> META_SW, string ETEXT)
    {
        // This connection is bound late.
        PAYBEN1.Connection = connection;
        PAYPROFIT_FILE.connection = connection;

        this.META_SW = META_SW;
        //- set META_CHKPT_TAB_FCD_PTR to address of TAB_FCD.

        //- INITIALIZE POINT_VALUES.
        //- PERFORM 025_ACCEPT_DATE THRU 025_EXIT.
        m025AcceptDate(ETEXT);
        //- IF META_SW(8) == 1
        if (META_SW[8] == 1)
        {
            //- DISPLAY "DO NOT UPDATE PAYPROFIT/PAYBEN"
        } //- END-IF.
          //- IF META_SW (3) == 1     GO TO 016_BYPASS_VALUES.
        if (META_SW[3] == 1)
            goto l016_BYPASS_VALUES;

        //- PERFORM 110_ACCEPT_VALUES_01 THRU 110_EXIT.
        m110AcceptValues01();

    //- 016_BYPASS_VALUES.
    l016_BYPASS_VALUES:

        //- OPEN INPUT PAYPROFIT_FILE.
        //- OPEN INPUT PAYBEN1.
        //- OPEN OUTPUT PROFIT
        //- MOVE "I_O" TO UFAS_OPEN_MODE
        //- CALL "OPEN_PAYPROFIT" USING PAYPROFIT_FILE_STATUS UFAS_OPEN_MODE.
        //- CALL "OPEN_PAYBEN" USING PAYBEN_FILE_STATUS UFAS_OPEN_MODE.
        //- MOVE "INPUT" TO UFAS_OPEN_MODE
        //- CALL "OPEN_DEMO_PROFSHARE" USING DEMO_PROFSHARE_FILE_STATUS
        //- PERFORM GAC_CALL.
        //- MOVE "0000000" TO DB_STATUS.
        DB_STATUS = 0;
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "200_PROFIT" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "200_PROFIT";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE ZERO TO HOLD_PAYSSN.
        HOLD_PAYSSN = 0;
        //- PERFORM 200_PROCESS_PROFIT THRU 205_EXIT.
        m200ProcessProfit();
        //- CLOSE PROFIT
        //- CLOSE PAYBEN1.
        //- CLOSE PAYPROFIT_FILE.
        //- CALL "CLOSE_PAYPROFIT" USING PAYPROFIT_FILE_STATUS.
        //- CALL "CLOSE_DEMO_PROFSHARE" USING DEMO_PROFSHARE_FILE_STATUS.
        //- MOVE "0000000" TO DB_STATUS.
        DB_STATUS = 0;
        //- IF INVALID_CNT > 0
        if (INVALID_CNT > 0)
        {
            //- DISPLAY INVALID_CNT " = TOTAL RECORDS NOT UPDATED DUE TO BAD KEYS".
        }
        //- IF WS_RERUN_IND == 1
        if (ws_indicators.WS_RERUN_IND == 1)
        {
            //- PERFORM 020_RERUN THRU 020_EXIT.
            m020Rerun();
        }
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "SORT" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "SORT";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- SORT SORT_FILE ON ASCENDING SD_NAME
        //- USING PROFIT
        //- OUTPUT PROCEDURE 800_PRINT_SEQUENCE THRU 899_EXIT.
        m805PrintSequence();

        //- CALL "CLOSE_PAYBEN" USING PAYBEN_FILE_STATUS.
        //- PERFORM 1000_ADJUSTMENT_REPORT THRU 1000_EXIT.
        m1000AdjustmentReport();
        //- EXIT PROGRAM.
    }

    private void DISPCONS(object _)
    {
        Console.WriteLine(DAEMON_DISP_MSG);
    }


    //- COPY GAC_CALL.

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 020_RERUN.
    public void m020Rerun()
    {
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "******************************" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "******************************";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                              " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                              ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "A RERUN OF PAY444 IS REQUIRED " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "A RERUN OF PAY444 IS REQUIRED ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "PLEASE NOTIFY DP SUPERVISOR.  " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "PLEASE NOTIFY DP SUPERVISOR.  ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                              " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                              ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "******************************" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "******************************";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- 020_EXIT.
        //- EXIT.

    }

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *  ACCEPT VALUES FOR DATE
    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 110_ACCEPT_VALUES SECTION 40.

    //- 025_ACCEPT_DATE.
    public void m025AcceptDate(string arg)
    {
        //- *     display "at 025-ACCEPT-DATE".
        //- ACCEPT WS_DATE FROM DATE.
        ws_date_time.WS_DATE = DateOnly.FromDateTime(DateTime.Today);
        //- IF WS_YY > 16
        if (ws_date_time.WS_DATE.Year > 2016)
        {
            //- MOVE 19 TO WS_CC
            ws_date_time.WS_CC = 19;
        }
        else //- ELSE
        {
            //- MOVE 20 TO WS_CC.
            ws_date_time.WS_CC = 20;
        }

        //- UNSTRING TEXTE DELIMITED BY "*" INTO EFFECTIVE_DATE.
        // TEXTE is passed in on the command line
        var yearText = arg.Split('*')[0];
        input_dates.EFFECT_DATE = str2long(yearText);
        input_dates.EFFECTIVE_DATE = yearText;

        input_dates.EFFECT_CC = input_dates.EFFECT_DATE / 100;
        input_dates.EFFECT_YEAR = input_dates.EFFECT_DATE % 100;
        input_dates.EFFECTIVE_YEAR = yearText[2..4];
        input_dates.EFFECTIVE_CENT = yearText[0..2];

        //- IF EFFECTIVE_DATE NUMERIC
        if (isDecimal(input_dates.EFFECTIVE_DATE))
        {
            //- PERFORM 026_ACCEPT_DATE THRU 026_EXIT.
            m026AcceptDate();
        }

        //- IF HOLD_EFF_DATE > ZERO
        if (HOLD_EFF_DATE > 0)
        {
            //- GO TO 025_EXIT.
            goto l025_EXIT;
        }

        //- IF WS_MM == 01 OR 02 OR 03
        if (ws_date_time.WS_MM == 01 || ws_date_time.WS_MM == 02 || ws_date_time.WS_MM == 03)
        {
            //- COMPUTE EFFECT_DATE = WS_CCYY - 1
            input_dates.EFFECT_DATE = ws_date_time.WS_CCYY - 1;
        }
        else //- ELSE
        {
            //- MOVE WS_CCYY TO EFFECT_YEAR.
            input_dates.EFFECT_YEAR = ws_date_time.WS_CCYY;
        }
        //- MOVE EFFECT_DATE TO HOLD_EFF_DATE.
        HOLD_EFF_DATE = input_dates.EFFECT_DATE;
    //- 025_EXIT.
    l025_EXIT:;
        //- EXIT.
        ;
    }

    private bool isDecimal(string x)
    {
        return decimal.TryParse(x, out _);
    }

    //- 026_ACCEPT_DATE.
    public void m026AcceptDate()
    {
        //- PERFORM 027_VALIDATE_INPUT_DATE THRU 027_EXIT.
        m027ValidateInputDate();
    //- 026_EXIT.
    l026_EXIT:; ;
        //- EXIT.
    }

    //- 027_VALIDATE_INPUT_DATE.
    public void m027ValidateInputDate()
    {
    l027_VALIDATE_INPUT_DATE:
        //- *     display "at 027-VALIDATE-INPUT-DATE".
        //- IF EFFECTIVE_YEAR NUMERIC
        if (isDecimal(input_dates.EFFECTIVE_YEAR))
        {
            //- MOVE EFFECTIVE_YEAR TO EFFECT_YEAR
            input_dates.EFFECT_YEAR = str2long(input_dates.EFFECTIVE_YEAR);
        }
        else //- ELSE
        {
            //- GO TO 028_MANUALLY_ENTER_DATE.
            goto l028_MANUALLY_ENTER_DATE;
        }
        //- IF EFFECTIVE_CENT == "19" OR "20"
        if (input_dates.EFFECTIVE_CENT == "19" || input_dates.EFFECTIVE_CENT == "20")
        {
            //- MOVE EFFECTIVE_CENT TO EFFECT_CC
            input_dates.EFFECT_CC = str2long(input_dates.EFFECTIVE_CENT);
        }
        else //- ELSE
        {
            //- GO TO 028_MANUALLY_ENTER_DATE.
            goto l028_MANUALLY_ENTER_DATE;
        }
        //- MOVE EFFECT_DATE TO HOLD_EFF_DATE.
        HOLD_EFF_DATE = input_dates.EFFECT_DATE;
        //- *    MOVE EFFECT-DATE TO HDR1-YEAR.
        //- 027_EXIT.
        if (true)
        {
            return;
        }

    //- 028_MANUALLY_ENTER_DATE.
    l028_MANUALLY_ENTER_DATE:

        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "RE_ENTER YEAR 9999" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "RE-ENTER YEAR 9999";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        Console.WriteLine(DAEMON_ACCEPT_PROG); // To Prevent compiler warning BOBH
        Console.WriteLine(DAEMON_DISP_PROG); // ditto
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE DAEMON_ACCEPT_BACK TO EFFECTIVE_DATE.
        input_dates.EFFECTIVE_DATE = DAEMON_ACCEPT_BACK;
        //- GO TO 027_VALIDATE_INPUT_DATE.
        goto l027_VALIDATE_INPUT_DATE;
        //- 028_EXIT. EXIT.
    }

    private long str2long(string x)
    {
        return long.Parse(x);
    }

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *  ACCEPT VALUES FOR CLIENT 1 (CONT,FORT,EARN,EARN2,ADJUST,MAX)
    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 110_ACCEPT_VALUES_01.
    public void m110AcceptValues01()
    {
        //- PERFORM 112_CONT_01 THRU 112_EXIT.
        m112Cont01();
        //- PERFORM 114_FORF_01 THRU 114_EXIT.
        m114Forf01();
        //- PERFORM 116_EARN_01 THRU 116_EXIT.
        m116Earn01();
        //- IF META_SW (5) == 1 THEN
        if (META_SW[5] == 1)
        {
            //- PERFORM 117_EARN2_01 THRU 117_EXIT
            m117Earn201();
            //- IF PV_EARN2_01 == 0
            if (point_values.PV_EARN2_01 == 0)
            {
                //- MOVE ZERO TO META_SW(5)
                META_SW[5] = 0;
            } //- END-IF
        } //- END-IF.
          //- IF META_SW (4) == 1 THEN
        if (META_SW[4] == 1)
        {
            //- NEXT SENTENCE
        }
        else //- ELSE
        {
            //- PERFORM 118_ADJUST THRU 118_EXIT
            m118Adjust();

            //- IF META_SW (5) == 1 THEN
            if (META_SW[5] == 1)
            {
                //- PERFORM 118_EMP_2_ADJUST THRU 118_EMP_2_EXIT
                m118Emp2Adjust();
            } //- END-IF
        } //- END-IF.

        //- PERFORM 119_MAX_01 THRU 119_EXIT.
        m119Max01();
    //- 110_EXIT.
    l110_EXIT:; ;
        //- EXIT.
    }

    decimal str2dec(string x)
    {
        return decimal.Parse(x);
    }

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 112_CONT_01.
    public void m112Cont01()
    {
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " ENTER CONTRIBUTION POINT VALUE  " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " ENTER CONTRIBUTION POINT VALUE  ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                 999V999999                " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                 999V999999                ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE function numval(DAEMON_ACCEPT_BACK) TO PV_CONT_01.
        point_values.PV_CONT_01 = str2dec(DAEMON_ACCEPT_BACK);
        //- MOVE PV_CONT_01 TO PV_MASK.
        console_response_masks.PV_MASK = point_values.PV_CONT_01;
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE SPACES TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = SPACES;
        //- STRING  "     VALUE ENTERED IS  "
        //- PV_MASK
        //- DELIMITED SIZE INTO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = $" VALUE ENTERED IS  {console_response_masks.PV_MASK}";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                         " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                         ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " IS VALUE ENTERED CORRECT: ENTER YES OR NO" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " IS VALUE ENTERED CORRECT: ENTER YES OR NO";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE DAEMON_ACCEPT_BACK TO CONSOLE_ANSWER.
        console_response.CONSOLE_ANSWER = DAEMON_ACCEPT_BACK;
        //- IF CONSOLE_ANSWER == CONSOLE_YES NEXT SENTENCE
        if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES)
        {

        }
        else //- ELSE GO TO 112-CONT-01.
        {
            m112Cont01();
        }
    //- 112_EXIT.
    l112_EXIT:;
        //- EXIT.
    }

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 114_FORF_01.
    public void m114Forf01()
    {
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " ENTER FORFEITURE POINT VALUE  " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " ENTER FORFEITURE POINT VALUE  ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                 999V999999                " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                 999V999999                ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE function numval(DAEMON_ACCEPT_BACK) TO PV_FORF_01.
        point_values.PV_FORF_01 = str2dec(DAEMON_ACCEPT_BACK);
        //- MOVE PV_FORF_01 TO PV_MASK.
        console_response_masks.PV_MASK = point_values.PV_FORF_01;
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE SPACES TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = SPACES;
        //- STRING  "     VALUE ENTERED IS  "
        //- PV_MASK
        //- DELIMITED SIZE INTO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = $"     VALUE ENTERED IS  {console_response_masks.PV_MASK}";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                         " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                         ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " IS VALUE ENTERED CORRECT: ENTER YES OR NO" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " IS VALUE ENTERED CORRECT: ENTER YES OR NO";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE DAEMON_ACCEPT_BACK TO CONSOLE_ANSWER.
        console_response.CONSOLE_ANSWER = DAEMON_ACCEPT_BACK;
        //- IF CONSOLE_ANSWER == CONSOLE_YES NEXT SENTENCE
        if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES)
        {
        }
        else //- ELSE GO TO 114-FORF-01.
        {
        //- 114_EXIT.
        l114_EXIT:; ;
            //- EXIT.
        }
    }

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 116_EARN_01.
    public void m116Earn01()
    {
        //- IF META_SW (2) == 1 GO TO 116_EXIT.
        if (META_SW[2] == 1)
        {
            goto l116_EXIT;
        }
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " ENTER EARNINGS POINT VALUE  " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " ENTER EARNINGS POINT VALUE  ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                 999V999999                " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                 999V999999                ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE function numval(DAEMON_ACCEPT_BACK) TO PV_EARN_01.
        point_values.PV_EARN_01 = str2dec(DAEMON_ACCEPT_BACK);
        //- MOVE PV_EARN_01 TO PV_MASK.
        console_response_masks.PV_MASK = point_values.PV_EARN_01;
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE SPACES TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = SPACES;
        //- STRING  "     VALUE ENTERED IS  "
        //- PV_MASK
        //- DELIMITED SIZE INTO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = $"     VALUE ENTERED IS  {console_response_masks.PV_MASK}";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                         " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                         ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " IS VALUE ENTERED CORRECT: ENTER YES OR NO" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " IS VALUE ENTERED CORRECT: ENTER YES OR NO";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE DAEMON_ACCEPT_BACK TO CONSOLE_ANSWER.
        console_response.CONSOLE_ANSWER = DAEMON_ACCEPT_BACK;
        //- IF CONSOLE_ANSWER == CONSOLE_YES NEXT SENTENCE
        if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES)
        {
        }
        else //- ELSE GO TO 116-EARN-01.
        {
            m116Earn01();
        }
    //- 116_EXIT.
    l116_EXIT:;
        //- EXIT.
    }

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 117_EARN2_01.
    public void m117Earn201()
    {
        //- IF META_SW (2) == 1 GO TO 117_EXIT.
        if (META_SW[2] == 1)
        {
            goto l117_EXIT;
        }
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " ENTER EARNINGS2 POINT VALUE  " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " ENTER EARNINGS2 POINT VALUE  ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                 999V999999                " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                 999V999999                ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE function numval(DAEMON_ACCEPT_BACK) TO PV_EARN2_01.
        point_values.PV_EARN2_01 = str2dec(DAEMON_ACCEPT_BACK);
        //- MOVE PV_EARN2_01 TO PV_MASK.
        console_response_masks.PV_MASK = point_values.PV_EARN2_01;
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE SPACES TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = SPACES;
        //- STRING  "     VALUE ENTERED IS  "
        //- PV_MASK
        DAEMON_DISP_MSG = $"     VALUE ENTERED IS  {console_response_masks.PV_MASK}";
        //- DELIMITED SIZE INTO DAEMON_DISP_MSG
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                         " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                         ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " IS VALUE ENTERED CORRECT: ENTER YES OR NO" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " IS VALUE ENTERED CORRECT: ENTER YES OR NO";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- IF PV_EARN2_01 == 0
        if (point_values.PV_EARN2_01 == 0)
        {
            //- MOVE "PAY444" TO DAEMON_DISP_PROG
            DAEMON_DISP_PROG = "PAY444";
            //- MOVE "                                          "
            //- TO DAEMON_DISP_MSG
            DAEMON_DISP_MSG = "";
            //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY
            DISPCONS(DAEMON_DISP_DISPLAY);
            //- MOVE "PAY444" TO DAEMON_DISP_PROG
            DAEMON_DISP_PROG = "PAY444";
            //- MOVE " BECAUSE THIS IS ZERO, EARNINGS2 WILL BE  "
            //- TO DAEMON_DISP_MSG
            DAEMON_DISP_MSG = " BECAUSE THIS IS ZERO, EARNINGS2 WILL BE  ";
            //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY
            DISPCONS(DAEMON_DISP_DISPLAY);
            //- MOVE "PAY444" TO DAEMON_DISP_PROG
            DAEMON_DISP_PROG = "PAY444";
            //- MOVE " BYPASSED AND NOT PROCESSED.              "
            //- TO DAEMON_DISP_MSG
            //- TO DAEMON_DISP_MSG
            DAEMON_DISP_MSG = " BYPASSED AND NOT PROCESSED.              ";
            //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY
            DISPCONS(DAEMON_DISP_DISPLAY);
        } //- END-IF.
          //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE DAEMON_ACCEPT_BACK TO CONSOLE_ANSWER.
        console_response.CONSOLE_ANSWER = DAEMON_ACCEPT_BACK;
        //- IF CONSOLE_ANSWER == CONSOLE_YES NEXT SENTENCE
        if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES)
        {
        }
        else //- ELSE GO TO 117-EARN2-01.
        {
            m117Earn201();
        }
    //- 117_EXIT.
    l117_EXIT:;
        //- EXIT.
    }

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 118_ADJUST.
    public void m118Adjust()
    {
        //- IF META_SW (2) == 1 GO TO 118_EXIT.
        if (META_SW[2] == 1)
        {
            goto l118_EXIT;
        }
    l118_ADJUST:
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " ENTER EMPLOYEE BADGE NUMBER FOR ADJUSTMENT " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " ENTER EMPLOYEE BADGE NUMBER FOR ADJUSTMENT ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE function numval(DAEMON_ACCEPT_BACK) TO PV_ADJUST_BADGE.
        point_values.PV_ADJUST_BADGE = str2long(DAEMON_ACCEPT_BACK);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- IF PV_ADJUST_BADGE == 0 THEN
        if (point_values.PV_ADJUST_BADGE == 0)
        {
            //- MOVE "PAY444" TO DAEMON_DISP_PROG
            DAEMON_DISP_PROG = "PAY444";
            //- MOVE " NO ADJUSTMENT THIS RUN. CORRECT: ENTER YES OR NO " TO DAEMON_DISP_MSG
            DAEMON_DISP_MSG = " NO ADJUSTMENT THIS RUN. CORRECT: ENTER YES OR NO ";
            //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY
            DISPCONS(DAEMON_DISP_DISPLAY);
        }
        else //- ELSE
        {
            //- MOVE "PAY444" TO DAEMON_DISP_PROG
            DAEMON_DISP_PROG = "PAY444";
            //- MOVE SPACES TO DAEMON_DISP_MSG
            DAEMON_DISP_MSG = SPACES;
            //- STRING  
            //- DELIMITED SIZE INTO DAEMON_DISP_MSG
            //- PV_ADJUST_BADGE
            DAEMON_DISP_MSG = $"     VALUE ENTERED IS  {point_values.PV_ADJUST_BADGE}";
            //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY
            DISPCONS(DAEMON_DISP_DISPLAY);
            //- MOVE "PAY444" TO DAEMON_DISP_PROG
            DAEMON_DISP_PROG = "PAY444";
            //- MOVE " IS VALUE ENTERED CORRECT: ENTER YES OR NO" TO DAEMON_DISP_MSG
            DAEMON_DISP_MSG = " IS VALUE ENTERED CORRECT: ENTER YES OR NO";
            //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
            DISPCONS(DAEMON_DISP_DISPLAY);
        }
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE DAEMON_ACCEPT_BACK TO CONSOLE_ANSWER.
        console_response.CONSOLE_ANSWER = DAEMON_ACCEPT_BACK;
        //- IF CONSOLE_ANSWER == CONSOLE_YES AND PV_ADJUST_BADGE == 0
        if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES && point_values.PV_ADJUST_BADGE == 0)
        {
            //- GO TO 118_EXIT
            goto l118_EXIT;
        }
        else //- ELSE     
             //- IF CONSOLE_ANSWER == CONSOLE_YES
            if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES)
        {
            //- NEXT SENTENCE
        }
        else //- ELSE
        {
            //- GO TO 118_ADJUST.
            goto l118_ADJUST;
        }

    //- 118_CONTRIB.
    l118_CONTRIB:
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " ENTER CONTRIBUTION ADJUSTMENT -999V99 " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " ENTER CONTRIBUTION ADJUSTMENT -999V99 ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE function numval(DAEMON_ACCEPT_BACK) TO PV_ADJ_CONTRIB.
        point_values.PV_ADJ_CONTRIB = str2dec(DAEMON_ACCEPT_BACK);
        //- MOVE PV_ADJ_CONTRIB TO PV_ADJ_MASK.
        console_response_masks.PV_ADJ_MASK = point_values.PV_ADJ_CONTRIB;
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE SPACES TO DAEMON_DISP_MSG    
        DAEMON_DISP_MSG = SPACES;
        //- STRING  "     VALUE ENTERED IS  : "
        //- PV_ADJ_MASK
        //- DELIMITED SIZE INTO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = $"     VALUE ENTERED IS  : {console_response_masks.PV_ADJ_MASK}";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                            " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                            ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " IS VALUE ENTERED CORRECT: ENTER YES OR NO " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " IS VALUE ENTERED CORRECT: ENTER YES OR NO ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                            " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                            ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " ***************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " ***************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE DAEMON_ACCEPT_BACK TO CONSOLE_ANSWER.
        console_response.CONSOLE_ANSWER = DAEMON_ACCEPT_BACK;
        //- IF CONSOLE_ANSWER == CONSOLE_YES NEXT SENTENCE
        if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES)
        {
        }
        else //- ELSE GO TO 118-CONTRIB.
        {
            goto l118_CONTRIB;
        }
    //- 118_EARNINGS.
    l118_EARNINGS:
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " ENTER EARNINGS ADJUSTMENT FOR EMPLOYEE 9V99" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " ENTER EARNINGS ADJUSTMENT FOR EMPLOYEE 9V99";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE function numval(DAEMON_ACCEPT_BACK) TO PV_ADJ_EARN.
        point_values.PV_ADJ_EARN = str2dec(DAEMON_ACCEPT_BACK);
        //- MOVE PV_ADJ_EARN TO PV_ADJ_MASK.
        console_response_masks.PV_ADJ_MASK = point_values.PV_ADJ_EARN;
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE SPACES TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = SPACES;
        //- STRING  "     VALUE ENTERED IS  "
        //- PV_ADJ_MASK
        //- DELIMITED SIZE INTO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = $"     VALUE ENTERED IS  {console_response_masks.PV_ADJ_MASK}";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.     
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " IS VALUE ENTERED CORRECT: ENTER YES OR NO" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " IS VALUE ENTERED CORRECT: ENTER YES OR NO";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE DAEMON_ACCEPT_BACK TO CONSOLE_ANSWER.
        console_response.CONSOLE_ANSWER = DAEMON_ACCEPT_BACK;
        //- IF CONSOLE_ANSWER == CONSOLE_YES NEXT SENTENCE
        if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES)
        {
        }
        else //- ELSE GO TO 118-EARNINGS.
        {
            goto l118_EARNINGS;
        }
    //- 118_FORFEIT.
    l118_FORFEIT:
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " ENTER FORFEITURE ADJUSTMENT FOR EMPLOYEE 9V99" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " ENTER FORFEITURE ADJUSTMENT FOR EMPLOYEE 9V99";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE function numval(DAEMON_ACCEPT_BACK) TO PV_ADJ_FORFEIT.
        point_values.PV_ADJ_FORFEIT = str2dec(DAEMON_ACCEPT_BACK);
        //- MOVE PV_ADJ_FORFEIT TO PV_ADJ_MASK.
        console_response_masks.PV_ADJ_MASK = point_values.PV_ADJ_FORFEIT;
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE SPACES TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = SPACES;
        //- STRING  "     VALUE ENTERED IS  "
        //- PV_ADJ_MASK
        //- DELIMITED SIZE INTO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = $"     VALUE ENTERED IS  {console_response_masks.PV_ADJ_MASK}";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " IS VALUE ENTERED CORRECT: ENTER YES OR NO" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " IS VALUE ENTERED CORRECT: ENTER YES OR NO";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE DAEMON_ACCEPT_BACK TO CONSOLE_ANSWER.
        console_response.CONSOLE_ANSWER = DAEMON_ACCEPT_BACK;
        //- IF CONSOLE_ANSWER == CONSOLE_YES NEXT SENTENCE
        if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES)
        {
        }
        else //- ELSE
        {
            //- GO TO 118_FORFEIT.
            goto l118_FORFEIT;
        }

    //- 118_EXIT.
    l118_EXIT:;
        //- EXIT.
    }

    static List<string> answers = ["15", "YES", "1", "YES", "2", "YES", "0", "YES", "20000", "YES"];
    static int answerIndex = 0;

    private void ACCCONS()
    {
        //console_response.CONSOLE_ANSWER = Console.ReadLine();
        console_response.CONSOLE_ANSWER = answers[answerIndex++];

        DAEMON_ACCEPT_BACK = console_response.CONSOLE_ANSWER;
    }

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 118_EMP_2_ADJUST.
    public void m118Emp2Adjust()
    {
    l118_EMP_2_ADJUST:
        //- IF META_SW (5) == 0 GO TO 118_EMP_2_EXIT.
        if (META_SW[5] == 0)
        {
            goto l118_EMP_2_EXIT;
        }
        //- IF META_SW (2) == 1 GO TO 118_EMP_2_EXIT.
        if (META_SW[2] == 1)
        {
            goto l118_EMP_2_EXIT;
        }
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " ENTER EMPLOYEE BADGE NUMBER FOR EARNINGS2 ADJUSTMENT " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " ENTER EMPLOYEE BADGE NUMBER FOR EARNINGS2 ADJUSTMENT ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE function numval(DAEMON_ACCEPT_BACK) TO PV_ADJUST_BADGE2.
        point_values.PV_ADJUST_BADGE2 = str2long(DAEMON_ACCEPT_BACK);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- IF PV_ADJUST_BADGE2 == 0 THEN
        if (point_values.PV_ADJUST_BADGE2 == 0)
        {
            //- MOVE "PAY444" TO DAEMON_DISP_PROG
            DAEMON_DISP_PROG = "PAY444";
            //- MOVE " NO ADJUSTMENT THIS RUN. CORRECT: ENTER YES OR NO " TO DAEMON_DISP_MSG
            DAEMON_DISP_MSG = " NO ADJUSTMENT THIS RUN. CORRECT: ENTER YES OR NO ";
            //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY
            DISPCONS(DAEMON_DISP_DISPLAY);
        }
        else //- ELSE
        {
            //- MOVE "PAY444" TO DAEMON_DISP_PROG
            DAEMON_DISP_PROG = "PAY444";
            //- MOVE SPACES TO DAEMON_DISP_MSG
            DAEMON_DISP_MSG = SPACES;
            //- STRING  "     VALUE ENTERED IS  "
            //- PV_ADJUST_BADGE2             
            //- DELIMITED SIZE INTO DAEMON_DISP_MSG
            DAEMON_DISP_MSG = $"     VALUE ENTERED IS  {point_values.PV_ADJUST_BADGE2}";
            //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY
            DISPCONS(DAEMON_DISP_DISPLAY);
            //- MOVE "PAY444" TO DAEMON_DISP_PROG
            DAEMON_DISP_PROG = "PAY444";
            //- MOVE " IS VALUE ENTERED CORRECT: ENTER YES OR NO" TO DAEMON_DISP_MSG
            DAEMON_DISP_MSG = " IS VALUE ENTERED CORRECT: ENTER YES OR NO";
            //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
            DISPCONS(DAEMON_DISP_DISPLAY);
        }
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** "
        //- TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE DAEMON_ACCEPT_BACK TO CONSOLE_ANSWER.
        console_response.CONSOLE_ANSWER = DAEMON_ACCEPT_BACK;
        //- IF CONSOLE_ANSWER == CONSOLE_YES AND PV_ADJUST_BADGE2 == 0
        if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES && point_values.PV_ADJUST_BADGE2 == 0)
        {
            //- GO TO 118_EMP_2_EXIT
            goto l118_EMP_2_EXIT;
        }
        else //- ELSE
        {
            //- IF CONSOLE_ANSWER == CONSOLE_YES
            if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES)
            {
                //- NEXT SENTENCE
            }
            else //- ELSE
            {
                //- GO TO 118_EMP_2_ADJUST.
                goto l118_EMP_2_ADJUST;
            }
        }

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 118_EMP_2_EARNINGS.
    l118_EMP_2_EARNINGS:
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " ENTER EARNINGS2 ADJUSTMENT FOR EMPLOYEE 9V99" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " ENTER EARNINGS2 ADJUSTMENT FOR EMPLOYEE 9V99";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE function numval(DAEMON_ACCEPT_BACK) TO PV_ADJ_EARN2.
        point_values.PV_ADJ_EARN2 = str2dec(DAEMON_ACCEPT_BACK);
        //- MOVE PV_ADJ_EARN2 TO PV_ADJ_MASK.
        console_response_masks.PV_ADJ_MASK = point_values.PV_ADJ_EARN2;
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE SPACES TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = SPACES;
        //- STRING  "     VALUE ENTERED IS  "
        //- PV_ADJ_MASK
        //- DELIMITED SIZE INTO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = $"     VALUE ENTERED IS  ${console_response_masks.PV_ADJ_MASK}";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " IS VALUE ENTERED CORRECT: ENTER YES OR NO" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " IS VALUE ENTERED CORRECT: ENTER YES OR NO";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE DAEMON_ACCEPT_BACK TO CONSOLE_ANSWER.
        console_response.CONSOLE_ANSWER = DAEMON_ACCEPT_BACK;
        //- IF CONSOLE_ANSWER == CONSOLE_YES
        if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES)
        {
            //- NEXT SENTENCE
        }
        else //- ELSE
        {
            //- GO TO 118_EMP_2_EARNINGS
            goto l118_EMP_2_EARNINGS;
        } //- END-IF.

    //- 118_EMP_2_EXIT.
    l118_EMP_2_EXIT:;
        //- EXIT.
    }



    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 119_MAX_01.
    public void m119Max01()
    {
    l119_MAX_01:
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " ENTER MAXIMUM CONTRIBUTION AMOUNT 999999" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " ENTER MAXIMUM CONTRIBUTION AMOUNT 999999";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG.
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ.
        ACCCONS();
        //- MOVE function numval(DAEMON_ACCEPT_BACK) TO WS_CONTR_MAX.
        WS_CONTR_MAX = str2long(DAEMON_ACCEPT_BACK);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE SPACES TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = SPACES;
        //- STRING  "     VALUE ENTERED IS  "
        //- WS_CONTR_MAX
        //- DELIMITED SIZE INTO DAEMON_DISP_MSG.
        DAEMON_DISP_MSG = $"     VALUE ENTERED IS  {WS_CONTR_MAX}";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " IS VALUE ENTERED CORRECT: ENTER YES OR NO" TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " IS VALUE ENTERED CORRECT: ENTER YES OR NO";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE "                                          " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = "                                          ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_DISP_PROG
        DAEMON_DISP_PROG = "PAY444";
        //- MOVE " **************************************** " TO DAEMON_DISP_MSG
        DAEMON_DISP_MSG = " **************************************** ";
        //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY.
        DISPCONS(DAEMON_DISP_DISPLAY);
        //- MOVE "PAY444" TO DAEMON_ACCEPT_PROG
        DAEMON_ACCEPT_PROG = "PAY444";
        //- CALL "ACCCONS" USING DAEMON_ACCEPT_MSGQ
        ACCCONS();
        //- MOVE DAEMON_ACCEPT_BACK TO CONSOLE_ANSWER.
        console_response.CONSOLE_ANSWER = DAEMON_ACCEPT_BACK;
        //- IF CONSOLE_ANSWER == CONSOLE_YES NEXT SENTENCE
        if (console_response.CONSOLE_ANSWER == console_response.CONSOLE_YES)
        {
        }
        else //- ELSE
        {
            //- GO TO 119_MAX_01.
            goto l119_MAX_01;
        }
    //- 119_EXIT.
    l119_EXIT:;
        //- EXIT.
    }

    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *    PROCESS SEQUENTIAL INPUT FILES PAYPROFIT/PAYBEN
    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 200_PROCESS_PROFIT SECTION 45.

    //- 201_PROCESS_PAYPROFIT.
    public void m200ProcessProfit()
    {
        m201ProcessPayProfit();
        m202_PROCESS_PAYBEN();
    }

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *    PROCESS PAYPROFIT-FILE
    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    public void m201ProcessPayProfit()
        {
    l201_PROCESS_PAYPROFIT:
        //- *     display "at 201-PROCESS-PAYPROFIT".
        //- READ PAYPROFIT_FILE NEXT AT END
        payprof_rec1 = PAYPROFIT_FILE.Read();
        if (PAYPROFIT_FILE.isEOF())
        {
            //- MOVE HOLD_KEY TO PAYPROF_KEY OF PAYPROF_REC
            payprof_rec.PAYPROF_BADGE = HOLD_KEY;
            if (FIRST_REC != 0) // BOBH
            {
                //- PERFORM 210_PAYPROFIT_COMPUTATION THRU 210_EXIT
                m210PayprofitComputation();
            }

            //- MOVE 0 to FD_MAXOVER
            prft.FD_MAXOVER = 0;
            //- FD_MAXPOINTS
            prft.FD_MAXPOINTS = 0;
            //- WS_PROF_POINTS
            ws_payprofit.WS_PROF_POINTS = 0;
            //- WS_OVER
            ws_maxcont_totals.WS_OVER = 0;
            //- GO TO 201_EXIT.
            goto l201_EXIT;
        }

        //- IF FIRST_REC == 0
        if (FIRST_REC == 0)
        {
            //- INITIALIZE WS_COMPUTE_TOTALS WS_PAYPROFIT
            ws_compute_totals = new WS_COMPUTE_TOTALS();
            ws_payprofit = new WS_PAYPROFIT();
            //- MOVE 1 TO FIRST_REC
            FIRST_REC = 1;
            //- MOVE PAYPROF_SSN OF PAYPROF_REC1 TO HOLD_SSN.
            HOLD_SSN = payprof_rec1.PAYPROF_SSN;
        }

        //- IF PAYPROF_SSN OF PAYPROF_REC1 NOT == HOLD_SSN
        if (payprof_rec1.PAYPROF_SSN != HOLD_SSN)
        {
            //- MOVE HOLD_KEY TO PAYPROF_KEY OF PAYPROF_REC
            payprof_rec.PAYPROF_BADGE = HOLD_KEY;
            //- PERFORM 210_PAYPROFIT_COMPUTATION THRU 210_EXIT
            m210PayprofitComputation();
            //- INITIALIZE WS_COMPUTE_TOTALS
            ws_compute_totals = new WS_COMPUTE_TOTALS();
            //- INITIALIZE WS_PAYPROFIT
            ws_payprofit = new WS_PAYPROFIT();
            //- MOVE PAYPROF_SSN OF PAYPROF_REC1 TO HOLD_SSN.
            HOLD_SSN = payprof_rec1.PAYPROF_SSN;
        }

        //- **  IF NO DEMOGRAPHICS RECORD FOR THE SSN, READ THE NEXT RECORD
        //- IF DEMO_PROFSHARE_FILE_STATUS NOT == "00"
        if (DEMO_PROFSHARE_FILE_STATUS != "00")
        {
            //- MOVE PAYPROF_KEY OF PAYPROF_REC1 TO HOLD_KEY
            HOLD_KEY = payprof_rec1.PAYPROF_BADGE;
            //- GO TO 201_PROCESS_PAYPROFIT
            goto l201_PROCESS_PAYPROFIT;
        } //- END-IF.

        //- COMPUTE WS_PS_AMT = WS_PS_AMT + PY_PS_AMT OF payprof_rec1.
        ws_payprofit.WS_PS_AMT = ws_payprofit.WS_PS_AMT + payprof_rec1.PY_PS_AMT;
        //- COMPUTE WS_PROF_POINTS = WS_PROF_POINTS + PY_PROF_POINTS
        ws_payprofit.WS_PROF_POINTS = ws_payprofit.WS_PROF_POINTS + payprof_rec1.PY_PROF_POINTS;
        //- OF payprof_rec1.
        //- MOVE PAYPROF_KEY OF PAYPROF_REC1 TO hold_key.
        HOLD_KEY = payprof_rec1.PAYPROF_BADGE;
        //- GO TO 201_PROCESS_PAYPROFIT.
        goto l201_PROCESS_PAYPROFIT;
    //- 201_EXIT.
    l201_EXIT:;
        //- EXIT.
    }

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *    PROCESS PAYBEN
    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 202_PROCESS_PAYBEN.
    void m202_PROCESS_PAYBEN()
    {
    l202_PROCESS_PAYBEN:
        //- *     DISPLAY "AT 202-PROCESS-PAYBEN".
        //- READ PAYBEN1 AT END
        PAYBEN1.Read(payben1_rec);
        if (PAYBEN1.isEOF())
        {
            //- GO TO 202_EXIT.
            goto l202_EXIT;
        }
        //- IF PYBEN_PAYSSN1 == HOLD_PAYSSN
        if (payben1_rec.PYBEN_PAYSSN1 == HOLD_PAYSSN)
        {
            //- GO TO 202_PROCESS_PAYBEN
            goto l202_PROCESS_PAYBEN;
        }
        else //- ELSE
        {
            //- MOVE PYBEN_PAYSSN1 TO HOLD_PAYSSN
            HOLD_PAYSSN = payben1_rec.PYBEN_PAYSSN1;
        } //- END-IF.

        //- PERFORM 208_CHECK_PAYPROFIT_FROM_PAYBEN THRU 208_EXIT.
        m208CheckPayprofitFromPayben();
        //- IF WS_ERROR == 1
        if (WS_ERROR == 1)
        {
            //- GO TO 202_PROCESS_PAYBEN
            goto l202_PROCESS_PAYBEN;
        } //- END-IF.

        //- INITIALIZE WS_COMPUTE_TOTALS ws_payprofit.
        ws_compute_totals = new WS_COMPUTE_TOTALS();
        ws_payprofit = new WS_PAYPROFIT();

        //- PERFORM 220_PAYBEN_COMPUTATION THRU 220_EXIT.
        m220PaybenComputation();

        //- INITIALIZE ws_compute_totals.
        //- INITIALIZE ws_payprofit.
        ws_compute_totals = new WS_COMPUTE_TOTALS();
        ws_payprofit = new WS_PAYPROFIT();

        //- GO TO 202_PROCESS_PAYBEN.
        goto l202_PROCESS_PAYBEN;

    //- 202_EXIT.
    l202_EXIT:;
    //- EXIT.


    //- 205_EXIT.
    l205_EXIT:;
        //- EXIT.
    }

    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- * Check to see if it is an employee and do NOT process if it is.
    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 208_CHECK_PAYPROFIT_FROM_PAYBEN.
    public void m208CheckPayprofitFromPayben()
    {
        //- *     DISPLAY "AT 208-CHECK-PAYPROFIT-FROM-PAYBEN".
        //- MOVE PYBEN_PAYSSN1 TO PAYPROF_SSN OF payprof_rec.
        payprof_rec.PAYPROF_SSN = payben1_rec.PYBEN_PAYSSN1;
        //- MOVE "PAYPROF_SSN_KEY" TO UFAS_ALT_KEY_NAME
        //- CALL "READ_ALT_KEY_PAYPROFIT" USING PAYPROFIT_FILE_STATUS
        PAYPROFIT_FILE_STATUS = READ_ALT_KEY_PAYPROFIT(payprof_rec);
        //- UFAS_ALT_KEY_NAME payprof_rec.
        //- MOVE ZERO TO WS_ERROR.
        WS_ERROR = 0;

        //- * IF the Beneficiary is an employee they should be on the report
        //- IF PAYPROFIT_FILE_STATUS == "00"
        if (PAYPROFIT_FILE_STATUS == "00")
        {
            //- MOVE 1 TO WS_ERROR
            WS_ERROR = 1;
            //- MOVE 2 TO PY_PROF_NEWEMP OF PAYPROF_REC
            payprof_rec.PY_PROF_NEWEMP = 2;
        } //- END-IF.
          //- 208_EXIT.
    l208_EXIT:;
        //- EXIT.
    }

    private string? READ_ALT_KEY_PAYPROFIT(PAYPROF_REC payprof_rec)
    {
        if (PAYPROFIT_FILE.HasRecordBySsn(payprof_rec.PAYPROF_SSN))
        {
            return "00";
        }

        return "Something else";
    }



    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *    Read the PAYPROFIT table to update it with the points
    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 210_PAYPROFIT_COMPUTATION.
    public void m210PayprofitComputation()
    {
        //- *     display "at 210-PAYPROFIT-COMPUTATION".

        //- COPY GAC_CHECK_350.

        //- CALL "READ_KEY_PAYPROFIT" USING PAYPROFIT_FILE_STATUS    
        //- payprof_rec.
        PAYPROFIT_FILE_STATUS = READ_KEY_PAYPROFIT(payprof_rec);
        //- IF PAYPROFIT_FILE_STATUS NOT == "00"
        if (PAYPROFIT_FILE_STATUS != "00")
        {
            //- ADD 1 TO INVALID_CNT
            INVALID_CNT += 1;
            //- DISPLAY "INVALID PAYPROFIT RECORD NOT UPDATED :"
            //- PAYPROFIT_FILE_STATUS
            DISPLAY("INVALID PAYPROFIT RECORD NOT UPDATED :" + PAYPROFIT_FILE_STATUS);
            //- DISPLAY PAYPROF_KEY OF PAYPROF_REC
            //- " = INVALID PAYPROFIT RECORD NOT UPDATED"
            DISPLAY($"{payprof_rec.PAYPROF_BADGE} = INVALID PAYPROFIT RECORD NOT UPDATED");
            //- GO TO 210_EXIT
            goto l210_EXIT;
        } //- END-IF.

        //- * If an employee has an ETVA amount and no years on the plan he is a
        //- * beneficiary and should get earnings on the etva amt (8 record)
        //- if py_prof_newemp OF PAYPROF_REC = 0
        if (payprof_rec.PY_PROF_NEWEMP == 0)
        {
            //- if py_ps_etva OF PAYPROF_REC > 0 and py_ps_amt OF PAYPROF_REC = 0
            if (payprof_rec.PY_PS_ETVA > 0 && payprof_rec.PY_PS_AMT == 0)
            {
                //- move 2 to py_prof_newemp OF PAYPROF_REC
                payprof_rec.PY_PROF_NEWEMP = 2;
            } //- end-if
        } //- end-if.

        //- IF PY_PS_ENROLLED OF PAYPROF_REC > 0 OR
        //- PY_PROF_NEWEMP OF PAYPROF_REC  > 0  OR
        //- WS_PS_AMT      > 0  OR PY_PS_YEARS OF PAYPROF_REC > 0
        if (payprof_rec.PY_PS_ENROLLED > 0 ||
           payprof_rec.PY_PROF_NEWEMP > 0 ||
           ws_payprofit.WS_PS_AMT > 0 || payprof_rec.PY_PS_YEARS > 0)
        {
            //- NEXT SENTENCE
        }
        else //- ELSE
        {
            //- GO TO 210_EXIT
            goto l210_EXIT;
        } //- END-IF.

        //- **  read DEMOGRAPHICS table to get employee name, etc.
        //- MOVE PAYPROF_BADGE OF PAYPROF_REC TO DEM_BADGE.
        dem_rec.DEM_BADGE = payprof_rec.PAYPROF_BADGE;
        //- CALL "READ_KEY_DEMO_PROFSHARE" USING
        //- DEMO_PROFSHARE_FILE_STATUS DEM_REC.
        DEMO_PROFSHARE_FILE_STATUS = READ_KEY_DEMO_PROFSHARE(dem_rec);
        //- IF DEMO_PROFSHARE_FILE_STATUS NOT == "00"
        if (DEMO_PROFSHARE_FILE_STATUS != "00")
        {
            //- DISPLAY "NO DEMO_PROFSHARE RECORD FOR EMP: "
            //- PAYPROF_BADGE OF PAYPROF_REC
            //- GO TO 210_EXIT
            goto l210_EXIT;
        } //- END-IF.

        //- MOVE 1 TO WS_REWRITE_WHICH.
        WS_REWRITE_WHICH = 1;

        //- INITIALIZE intermediate_values.
        intermediate_values = new INTERMEDIATE_VALUES();

        //- PERFORM 500_GET_DB_INFO THRU 500_EXIT.
        m500GetDbInfo();

        //- INITIALIZE PRFT.
        prft = new PRFT();

        //- * 02/07/03 PROJ. 4970  ADDED FOLLOWING LINE.
        //- MOVE ZERO TO PY_PROF_MAXCONT OF payprof_rec.
        payprof_rec.PY_PROF_MAXCONT = 0;
        //- IF PY_PROF_MAXCONT OF PAYPROF_REC == 1 OR META_SW (2) == 1
        if (payprof_rec.PY_PROF_MAXCONT == 1 || META_SW[2] == 1)
        {
            //- PERFORM 410_LOAD_PROFIT THRU 410_EXIT
            m410LoadProfit();
            //- GO TO 210_EXIT
            goto l210_EXIT;
        } //- END-IF.

        //- PERFORM 230_COMPUTE_CONTRIBUTION THRU 230_EXIT.
        m230ComputeContribution();
        //- PERFORM 240_COMPUTE_FORFEITURES   THRU 240_EXIT.
        m240ComputeForfeitures();

        //- ***** MAIIN-1488   ADD CAF AND IF EARNINGS BALANCE <= 0, MOVE 0 TO
        //- **       EARN POINTS AND WS-DOLLARS-POINTS (USED IN CALC FOR ETVA LATER.)

        //- COMPUTE WS_EARNINGS_BALANCE = ALLOCATION_TOTAL +  WS_PROF_CAF +
        //-     (WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL)        
        //-     - (DIST_TOTAL).
        ws_compute_totals.WS_EARNINGS_BALANCE = ALLOCATION_TOTAL + ws_payprofit.WS_PROF_CAF + (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL;

        //- SUBTRACT WS_PROF_CAF FROM  WS_EARNINGS_BALANCE.
        ws_compute_totals.WS_EARNINGS_BALANCE -= ws_payprofit.WS_PROF_CAF;   // BOBH WHAT?  Add it in then remove it???

        //- IF WS_EARNINGS_BALANCE <== 0
        if (ws_compute_totals.WS_EARNINGS_BALANCE <= 0)
        {
            //- MOVE 0 TO WS_EARN_POINTS
            ws_compute_totals.WS_EARN_POINTS = 0;
            //- MOVE 0 TO WS_POINTS_DOLLARS
            ws_compute_totals.WS_POINTS_DOLLARS = 0;
            //- GO TO 210_CONTINUE
            goto l210_CONTINUE;
        } //- END-IF.

        //- *****  MAIN-1488 END

        //- COMPUTE WS_POINTS_DOLLARS = ALLOCATION_TOTAL +
        //- (WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL)        
        //- - (DIST_TOTAL).

        ws_compute_totals.WS_POINTS_DOLLARS = ALLOCATION_TOTAL + (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL;

        //- DIVIDE WS_POINTS_DOLLARS BY 100 GIVING WS_EARN_POINTS
        //- REMAINDER  WS_REMAINDER.
        ws_compute_totals.WS_EARN_POINTS = (long)Round(ws_compute_totals.WS_POINTS_DOLLARS / 100);

    //- 210_CONTINUE.
    l210_CONTINUE:

        //- MOVE PAYPROF_BADGE OF PAYPROF_REC  TO WS_FD_BADGE.
        intermediate_values.WS_FD_BADGE = payprof_rec.PAYPROF_BADGE;
        //- MOVE PY_NAM                   TO WS_FD_NAME.
        intermediate_values.WS_FD_NAME = dem_rec.PY_NAM;
        //- MOVE PAYPROF_SSN OF PAYPROF_REC TO WS_FD_SSN.
        intermediate_values.WS_FD_SSN = payprof_rec.PAYPROF_SSN;
        //- MOVE "00000000000"            TO WS_TEMP_PSN.
        //- MOVE PAYPROF_BADGE OF PAYPROF_REC  TO WS_TEMP_PSN(1:7).
        //- INSPECT WS_TEMP_PSN REPLACING ALL SPACES BY ZERO.
        //- MOVE WS_TEMP_PSN     TO WS_FD_PSN.
        intermediate_values.WS_FD_PSN = payprof_rec.PAYPROF_BADGE;

        //- PERFORM 250_COMPUTE_EARNINGS     THRU 250_EXIT.
        m250ComputeEarnings();

        //- MOVE ALLOCATION_TOTAL TO WS_FD_XFER.
        intermediate_values.WS_FD_XFER = ALLOCATION_TOTAL;
        //- MOVE PALLOCATION_TOTAL TO WS_FD_PXFER.

        intermediate_values.WS_FD_PXFER = PALLOCATION_TOTAL;

        //- MOVE WS_PS_AMT       TO WS_FD_AMT.
        intermediate_values.WS_FD_AMT = ws_payprofit.WS_PS_AMT;
        //- MOVE DIST_TOTAL      TO WS_FD_DIST1.
        intermediate_values.WS_FD_DIST1 = DIST_TOTAL;
        //- MOVE WS_PROF_MIL     TO WS_FD_MIL.
        intermediate_values.WS_FD_MIL = ws_payprofit.WS_PROF_MIL;
        //- MOVE WS_PROF_CAF     TO WS_FD_CAF.
        intermediate_values.WS_FD_CAF = ws_payprofit.WS_PROF_CAF;
        //- MOVE PY_PROF_NEWEMP OF PAYPROF_REC TO WS_FD_NEWEMP.
        intermediate_values.WS_FD_NEWEMP = payprof_rec.PY_PROF_NEWEMP;
        //- MOVE WS_PROF_POINTS  TO WS_FD_POINTS.
        intermediate_values.WS_FD_POINTS = ws_payprofit.WS_PROF_POINTS;
        //- MOVE WS_EARN_POINTS  TO WS_FD_POINTS_EARN.
        intermediate_values.WS_FD_POINTS_EARN = ws_compute_totals.WS_EARN_POINTS;
        //- MOVE WS_PROF_CONT TO WS_FD_CONT.
        intermediate_values.WS_FD_CONT = ws_payprofit.WS_PROF_CONT;
        //- MOVE WS_PROF_FORF TO WS_FD_FORF.
        intermediate_values.WS_FD_FORF = ws_payprofit.WS_PROF_FORF;
        //- COMPUTE WS_FD_FORF = WS_FD_FORF - FORFEIT_TOTAL.
        intermediate_values.WS_FD_FORF = intermediate_values.WS_FD_FORF - FORFEIT_TOTAL;
        //- MOVE PY_PROF_EARN OF PAYPROF_REC    TO ws_FD_EARN.
        intermediate_values.WS_FD_EARN = payprof_rec.PY_PROF_EARN;
        //- ADD PY_PROF_ETVA OF PAYPROF_REC     TO ws_FD_EARN.
        intermediate_values.WS_FD_EARN += payprof_rec.PY_PROF_ETVA;
        //- MOVE PY_PROF_EARN2 OF PAYPROF_REC   TO ws_FD_EARN2.
        intermediate_values.WS_FD_EARN2 = payprof_rec.PY_PROF_EARN2;
        //- ADD PY_PROF_ETVA2 OF PAYPROF_REC    TO ws_FD_EARN2.
        intermediate_values.WS_FD_EARN2 += payprof_rec.PY_PROF_ETVA2;

        //- COMPUTE WS_MAX = WS_PROF_CONT + WS_PROF_MIL + WS_PROF_FORF.
        ws_maxcont_totals.WS_MAX = ws_payprofit.WS_PROF_CONT + ws_payprofit.WS_PROF_MIL + ws_payprofit.WS_PROF_FORF;

        //- IF WS_MAX > WS_CONTR_MAX
        if (ws_maxcont_totals.WS_MAX > WS_CONTR_MAX)
        {
            //- PERFORM 260_MAXCONT            THRU 260_EXIT
            m260Maxcont();
        }
        else //- ELSE
        {
            //- MOVE 0 TO FD_MAXOVER
            prft.FD_MAXOVER = 0;
        } //- END-IF.

        //- PERFORM 400_LOAD_PAYPROFIT        THRU 400_EXIT.
        m400LoadPayprofit();
        //- PERFORM 410_LOAD_PROFIT           THRU 410_EXIT.
        m410LoadProfit();
        //- IF META_SW (8) == 0
        if (META_SW[8] == 0)
        {
            //- PERFORM 420_REWRITE_PAYPROFIT  THRU 420_EXIT
            m420RewritePayprofit();
        } //- END-IF.
          //- 210_EXIT.
    l210_EXIT:;
        //- EXIT.
    }

    private decimal Round(decimal v)
    {
        return Math.Round(v, MidpointRounding.AwayFromZero);
    }

    public OracleConnection connection = null;
    public DemRecTableHelper DemRecTableHelper = null;

    private string? READ_KEY_DEMO_PROFSHARE(DEM_REC dem_rec)
    {
        if (DemRecTableHelper == null)
        {
            DemRecTableHelper = new DemRecTableHelper(connection, dem_rec);
        }

        return DemRecTableHelper.getByBadge(dem_rec);

    }

    private string? READ_KEY_PAYPROFIT(PAYPROF_REC payprof_rec)
    {
        PAYPROF_REC one = PAYPROFIT_FILE.findByBadge(payprof_rec.PAYPROF_BADGE);
        payprof_rec.PAYPROF_BADGE = one.PAYPROF_BADGE;
        payprof_rec.PAYPROF_SSN = one.PAYPROF_SSN;
        payprof_rec.PY_PH = one.PY_PH;
        payprof_rec.PY_PD = one.PY_PD;
        payprof_rec.PY_WEEKS_WORK = one.PY_WEEKS_WORK;
        payprof_rec.PY_PROF_CERT = one.PY_PROF_CERT;
        payprof_rec.PY_PS_ENROLLED = one.PY_PS_ENROLLED;
        payprof_rec.PY_PS_YEARS = one.PY_PS_YEARS;
        payprof_rec.PY_PROF_BENEFICIARY = one.PY_PROF_BENEFICIARY;
        payprof_rec.PY_PROF_INITIAL_CONT = one.PY_PROF_INITIAL_CONT;
        payprof_rec.PY_PS_AMT = one.PY_PS_AMT;
        payprof_rec.PY_PS_VAMT = one.PY_PS_VAMT;
        payprof_rec.PY_PH_LASTYR = one.PY_PH_LASTYR;
        payprof_rec.PY_PD_LASTYR = one.PY_PD_LASTYR;
        payprof_rec.PY_PROF_NEWEMP = one.PY_PROF_NEWEMP;
        payprof_rec.PY_PROF_POINTS = one.PY_PROF_POINTS;
        payprof_rec.PY_PROF_CONT = one.PY_PROF_CONT;
        payprof_rec.PY_PROF_FORF = one.PY_PROF_FORF;
        payprof_rec.PY_VESTED_FLAG = one.PY_VESTED_FLAG;
        payprof_rec.PY_PROF_MAXCONT = one.PY_PROF_MAXCONT;
        payprof_rec.PY_PROF_ZEROCONT = one.PY_PROF_ZEROCONT;
        payprof_rec.PY_WEEKS_WORK_LAST = one.PY_WEEKS_WORK_LAST;
        payprof_rec.PY_PROF_EARN = one.PY_PROF_EARN;
        payprof_rec.PY_PS_ETVA = one.PY_PS_ETVA;
        payprof_rec.PY_PRIOR_ETVA = one.PY_PRIOR_ETVA;
        payprof_rec.PY_PROF_ETVA = one.PY_PROF_ETVA;
        payprof_rec.PY_PROF_EARN2 = one.PY_PROF_EARN2;
        payprof_rec.PY_PROF_ETVA2 = one.PY_PROF_ETVA2;
        payprof_rec.PY_PH_EXEC = one.PY_PH_EXEC;
        payprof_rec.PY_PD_EXEC = one.PY_PD_EXEC;

        return "00";
    }


    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- * Read the PAYBEN table to update it with the Earnings points
    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 220_PAYBEN_COMPUTATION.
    public void m220PaybenComputation()
    {
        //- *     DISPLAY "AT 220-PAYBEN-COMPUTATION [" PYBEN-PAYSSN1 "]" pyben-pay-ssn1.
        //- MOVE PYBEN_PSN1 TO PSKEY.
        // <same key> PSKEY = payben1_rec.PYBEN_PSN1;
        payben_rec.PYBEN_PSN =  payben1_rec.PYBEN_PSN1;
        //- CALL "READ_KEY_PAYBEN" USING PAYBEN_FILE_STATUS
        //- PAYBEN_REC.
        PAYBEN_FILE_STATUS = READ_KEY_PAYBEN(payben_rec);
        //- IF PAYBEN_FILE_STATUS NOT == "00"
        if (PAYBEN_FILE_STATUS != "00")
        {
            //- ADD 1 TO INVALID_CNT
            INVALID_CNT += 1;
            //- DISPLAY PAYBEN_FILE_STATUS " " PYBEN_PSN " " PYBEN_PAYSSN
            //- " = INVALID PAYBEN RECORD NOT UPDATED"
            DISPLAY($"{PAYBEN_FILE_STATUS} {payben_rec.PYBEN_PSN} {payben_rec.PYBEN_PAYSSN} = INVALID PAYBEN RECORD NOT UPDATED");
            //- GO TO 220_EXIT
            goto l220_EXIT;
        } //- END-IF.

        //- MOVE 2 TO WS_REWRITE_WHICH.
        WS_REWRITE_WHICH = 2;

        //- INITIALIZE intermediate_values
        intermediate_values = new INTERMEDIATE_VALUES();

        //- MOVE PYBEN_PAYSSN TO PAYPROF_SSN OF payprof_rec.
        payprof_rec.PAYPROF_SSN = payben_rec.PYBEN_PAYSSN;

        //- MOVE PYBEN_PSAMT TO WS_PS_AMT.
        ws_payprofit.WS_PS_AMT = payben_rec.PYBEN_PSAMT;

        //- MOVE ZERO TO WS_POINTS_DOLLARS.
        ws_compute_totals.WS_POINTS_DOLLARS = 0m;
        //- MOVE ZERO TO WS_EARNINGS_BALANCE.
        ws_compute_totals.WS_EARNINGS_BALANCE = 0m;
        //- MOVE ZERO TO WS_EARN_POINTS.
        ws_compute_totals.WS_EARN_POINTS = 0;
        //- MOVE ZERO TO PAYPROF_BADGE OF payprof_rec.
        payprof_rec.PAYPROF_BADGE = 0;

        //- PERFORM 500_GET_DB_INFO THRU 500_EXIT.
        m500GetDbInfo();

        //- *****  MAIN-1488   ADD CAF AND IF EARNINGS BALANCE <= 0, MOVE 0 TO
        //- **       EARN POINTS AND WS-DOLLARS-POINTS (USED IN CALC FOR ETVA LATER.

        //- COMPUTE WS_EARNINGS_BALANCE = ALLOCATION_TOTAL +  WS_PROF_CAF +
        //-    (WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL)        
        //-     - (DIST_TOTAL).
        ws_compute_totals.WS_EARNINGS_BALANCE = ALLOCATION_TOTAL + ws_payprofit.WS_PROF_CAF + (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL;

        //- SUBTRACT WS_PROF_CAF FROM  WS_EARNINGS_BALANCE.
        ws_compute_totals.WS_EARNINGS_BALANCE -= ws_payprofit.WS_PROF_CAF; // BOBH What!??!

        //- IF WS_EARNINGS_BALANCE <== 0
        if (ws_compute_totals.WS_EARNINGS_BALANCE <= 0)
        {
            //- MOVE 0 TO WS_EARN_POINTS
            ws_compute_totals.WS_EARN_POINTS = 0;
            //- MOVE 0 TO WS_POINTS_DOLLARS
            ws_compute_totals.WS_POINTS_DOLLARS = 0;
            //- GO TO 220_CONTINUE
            goto l220_CONTINUE;
        } //- END-IF.

        //- *****  MAIN-1488 END

        //- COMPUTE WS_POINTS_DOLLARS = ALLOCATION_TOTAL +
        //-    (WS_PS_AMT - FORFEIT_TOTAL  - PALLOCATION_TOTAL)        
        //-     - (DIST_TOTAL).
        ws_compute_totals.WS_POINTS_DOLLARS = ALLOCATION_TOTAL + (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL;

        //- DIVIDE WS_POINTS_DOLLARS BY 100 GIVING WS_EARN_POINTS
        //- REMAINDER  WS_REMAINDER.
        ws_compute_totals.WS_EARN_POINTS = (long)Round(ws_compute_totals.WS_POINTS_DOLLARS / 100);

    //- 220_CONTINUE.
    l220_CONTINUE:
        //- * Payben people are py-prof-newemp = 2 and need ETVA and an Allocation this year
        //- IF ALLOCATION_TOTAL <> 0
        if (ALLOCATION_TOTAL != 0)
        {
            //- MOVE 2 TO PY_PROF_NEWEMP OF PAYPROF_REC
            payprof_rec.PY_PROF_NEWEMP = 2;
            //- IF PY_PROF_ETVA OF PAYPROF_REC == 0
            if (payprof_rec.PY_PROF_ETVA == 0)
            {
                //- MOVE 0.01 TO PY_PROF_ETVA OF PAYPROF_REC
                payprof_rec.PY_PROF_ETVA = 0.01m;
            } //- END-IF
        } //- END-IF.

        //- MOVE PYBEN_NAME TO PY_NAM.
        dem_rec.PY_NAM = payben_rec.PYBEN_NAME;
        //- MOVE ZERO TO PAYPROF_KEY OF payprof_rec.
        payprof_rec.PAYPROF_BADGE = 0;
        //- PERFORM 250_COMPUTE_EARNINGS     THRU 250_EXIT.
        m250ComputeEarnings();

        //- MOVE 0                    TO WS_FD_BADGE.
        intermediate_values.WS_FD_BADGE = 0;
        //- MOVE PYBEN_NAME           TO WS_FD_NAME.
        intermediate_values.WS_FD_NAME = payben_rec.PYBEN_NAME;
        //- MOVE PYBEN_PAYSSN         TO WS_FD_SSN.
        intermediate_values.WS_FD_SSN = payben_rec.PYBEN_PAYSSN;
        //- MOVE PYBEN_PSN            TO WS_FD_PSN.
        intermediate_values.WS_FD_PSN = payben_rec.PYBEN_PSN;
        //- MOVE DIST_TOTAL           TO WS_FD_DIST1.
        intermediate_values.WS_FD_DIST1 = DIST_TOTAL;
        //- MOVE 0 TO WS_FD_MIL.
        intermediate_values.WS_FD_MIL = 0;
        //- MOVE 0 TO WS_FD_NEWEMP.
        intermediate_values.WS_FD_NEWEMP = 0;
        //- MOVE 0 TO WS_FD_POINTS.
        intermediate_values.WS_FD_POINTS = 0;
        //- MOVE 0 TO WS_FD_CONT.
        intermediate_values.WS_FD_CONT = 0;
        //- MOVE 0 TO WS_FD_FORF.
        intermediate_values.WS_FD_FORF = 0;

        //- IF WS_PROF_CAF > 0
        if (ws_payprofit.WS_PROF_CAF > 0)
        {
            //- MOVE WS_PROF_CAF     TO WS_FD_CAF
            intermediate_values.WS_FD_CAF = ws_payprofit.WS_PROF_CAF;
        }
        else //- ELSE
        {
            //- MOVE 0 TO WS_FD_CAF
            intermediate_values.WS_FD_CAF = 0;
        } //- END-IF.

        //- MOVE ALLOCATION_TOTAL     TO WS_FD_XFER.
        intermediate_values.WS_FD_XFER = ALLOCATION_TOTAL;
        //- MOVE PALLOCATION_TOTAL    TO WS_FD_PXFER.        
        intermediate_values.WS_FD_PXFER = PALLOCATION_TOTAL;

        //- MOVE WS_PS_AMT            TO WS_FD_AMT.
        intermediate_values.WS_FD_AMT = ws_payprofit.WS_PS_AMT;
        //- MOVE WS_EARN_POINTS       TO WS_FD_POINTS_EARN.
        intermediate_values.WS_FD_POINTS_EARN = ws_compute_totals.WS_EARN_POINTS;
        //- COMPUTE WS_FD_FORF = WS_FD_FORF - FORFEIT_TOTAL.
        intermediate_values.WS_FD_FORF = intermediate_values.WS_FD_FORF - FORFEIT_TOTAL;

        //- MOVE PYBEN_PROF_EARN      TO WS_FD_EARN.
        intermediate_values.WS_FD_EARN = payben_rec.PYBEN_PROF_EARN;
        //- MOVE PYBEN_PROF_EARN2     TO WS_FD_EARN2.
        intermediate_values.WS_FD_EARN2 = payben_rec.PYBEN_PROF_EARN2;

        //- COMPUTE WS_MAX = WS_PROF_CONT + WS_PROF_MIL + WS_PROF_FORF.
        ws_maxcont_totals.WS_MAX = ws_payprofit.WS_PROF_CONT + ws_payprofit.WS_PROF_MIL + ws_payprofit.WS_PROF_FORF;

        //- IF WS_MAX > WS_CONTR_MAX
        if (ws_maxcont_totals.WS_MAX > WS_CONTR_MAX)
        {
            //- PERFORM 260_MAXCONT            THRU 260_EXIT
            m260Maxcont();
        } //- END-IF.

        //- PERFORM 410_LOAD_PROFIT          THRU 410_EXIT.
        m410LoadProfit();

        //- IF META_SW (8) == 0
        if (META_SW[8] == 0)
        {
            //- PERFORM 430_REWRITE_PAYBEN      THRU 430_EXIT
            m430RewritePayben();
        } //- END-IF.

    //- 220_EXIT.
    l220_EXIT:;
        //- EXIT.        
    }

    private string? READ_KEY_PAYBEN(PAYBEN_REC payben_rec)
    {
        return PAYBEN1.findByPSN(payben_rec);
    }

    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *     COMPUTE CONTRIBUTIONS
    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 230_COMPUTE_CONTRIBUTION.
    public void m230ComputeContribution()
    {
        //- COMPUTE WS_CONT_AMT= ROUNDED (PV_CONT_01 * WS_PROF_POINTS)
        ws_compute_totals.WS_CONT_AMT = Round(point_values.PV_CONT_01 * ws_payprofit.WS_PROF_POINTS);

        //- IF PV_ADJUST_BADGE > 0 AND
        //-     PV_ADJUST_BADGE = HOLD_BADGE THEN
        if (point_values.PV_ADJUST_BADGE > 0 && point_values.PV_ADJUST_BADGE == hold_key.HOLD_BADGE)
        {
            //- MOVE HOLD_SSN TO SV_SSN
            point_values.SV_SSN = HOLD_SSN;
            //- MOVE WS_CONT_AMT TO SV_CONT_AMT
            point_values.SV_CONT_AMT = ws_compute_totals.WS_CONT_AMT;
            //- ADD PV_ADJ_CONTRIB TO WS_CONT_AMT
            ws_compute_totals.WS_CONT_AMT += point_values.PV_ADJ_CONTRIB;
            //- MOVE WS_CONT_AMT TO SV_CONT_ADJUSTED
            point_values.SV_CONT_ADJUSTED = ws_compute_totals.WS_CONT_AMT;
        } //- END-IF.

        //- IF META_SW (2) == 1
        if (META_SW[2] == 1)
        {
            //- ADD WS_CONT_AMT TO WS_PROF_CONT
            ws_payprofit.WS_PROF_CONT += ws_compute_totals.WS_CONT_AMT;
        }
        else //- ELSE
        {
            //- MOVE WS_CONT_AMT TO WS_PROF_CONT
            ws_payprofit.WS_PROF_CONT = ws_compute_totals.WS_CONT_AMT;
        } //- END-IF.
          //- 230_EXIT.
    l230_EXIT:;
        //- EXIT.
    }

    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *     COMPUTE FORFEITURES/EARNINGS
    //- *
    //- *   IF FORFEIT-TOTAL > ZERO THEN THE RECORD IS A NEGATIVE
    //- *   CODE 2 RECORD AND SHOULD BE BYPASSED.
    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 240_COMPUTE_FORFEITURES.
    public void m240ComputeForfeitures()
    {
        //- COMPUTE WS_FORF_AMT= ROUNDED (PV_FORF_01 * WS_PROF_POINTS)
        ws_compute_totals.WS_FORF_AMT = Round(point_values.PV_FORF_01 * ws_payprofit.WS_PROF_POINTS);
        //- IF PV_ADJUST_BADGE > 0 AND
        //-     PV_ADJUST_BADGE = HOLD_BADGE THEN
        if (point_values.PV_ADJUST_BADGE > 0 && point_values.PV_ADJUST_BADGE == hold_key.HOLD_BADGE)
        {
            //- MOVE WS_FORF_AMT TO SV_FORF_AMT
            point_values.SV_FORF_AMT = ws_compute_totals.WS_FORF_AMT;
            //- ADD PV_ADJ_FORFEIT TO WS_FORF_AMT
            ws_compute_totals.WS_FORF_AMT += point_values.PV_ADJ_FORFEIT;
            //- MOVE WS_FORF_AMT TO SV_FORF_ADJUSTED
            point_values.SV_FORF_ADJUSTED = ws_compute_totals.WS_FORF_AMT;
        } //- end-if.

        //- IF META_SW (2) == 1
        if (META_SW[2] == 1)
        {
            //- ADD WS_FORF_AMT TO WS_PROF_FORF
            ws_payprofit.WS_PROF_FORF += ws_compute_totals.WS_FORF_AMT;
        }
        else //- ELSE
        {
            //- MOVE WS_FORF_AMT TO WS_PROF_FORF
            ws_payprofit.WS_PROF_FORF = ws_compute_totals.WS_FORF_AMT;
        } //- END-IF.
          //- 240_EXIT.
    l240_EXIT:;
        //- EXIT.
    }
    //- *
    //- 250_COMPUTE_EARNINGS.
    public void m250ComputeEarnings()
    {
        //- IF META_SW (2) == 1  GO TO 250_EXIT.
        if (META_SW[2] == 1)
        {
            goto l250_EXIT;
        }
        //- IF WS_EARN_POINTS > 0 OR WS_REWRITE_WHICH == 2
        if (ws_compute_totals.WS_EARN_POINTS > 0 || WS_REWRITE_WHICH == 2)
        {
            //- NEXT SENTENCE
        }
        else //- ELSE
        {
            //- IF WS_EARN_POINTS <= 0
            if (ws_compute_totals.WS_EARN_POINTS <= 0)
            {
                //- MOVE 0 TO WS_EARN_POINTS
                ws_compute_totals.WS_EARN_POINTS = 0;
                //- MOVE 0 TO PY_PROF_EARN OF PAYPROF_REC
                //-    WS_EARN_AMT                     
                //-    PYBEN_PROF_EARN
                payprof_rec.PY_PROF_EARN = 0;
                ws_compute_totals.WS_EARN_AMT = 0;
                payben_rec.PYBEN_PROF_EARN = 0;
                //- MOVE 0 TO PY_PROF_EARN2 OF PAYPROF_REC
                //-    WS_EARN2_AMT
                //-    PYBEN_PROF_EARN2
                payprof_rec.PY_PROF_EARN2 = 0;
                ws_compute_totals.WS_EARN2_AMT = 0;
                payben_rec.PYBEN_PROF_EARN2 = 0;
            } //- END-IF
        } //- END-IF.

        //- IF ws_rewrite_which == 1 or 2
        if (WS_REWRITE_WHICH == 1 || WS_REWRITE_WHICH == 2)
        {
            //- COMPUTE WS_EARN_AMT= ROUNDED PV_EARN_01 * WS_EARN_POINTS
            ws_compute_totals.WS_EARN_AMT = Round(point_values.PV_EARN_01 * ws_compute_totals.WS_EARN_POINTS);
            //- IF PV_ADJUST_BADGE > 0 AND
            //-    PV_ADJUST_BADGE = HOLD_BADGE THEN
            if (point_values.PV_ADJUST_BADGE > 0 && point_values.PV_ADJUST_BADGE == hold_key.HOLD_BADGE)
            {
                //- MOVE WS_EARN_AMT TO SV_EARN_AMT
                point_values.SV_EARN_AMT = ws_compute_totals.WS_EARN_AMT;
                //- ADD PV_ADJ_EARN TO WS_EARN_AMT
                ws_compute_totals.WS_EARN_AMT += point_values.PV_ADJ_EARN;
                //- MOVE WS_EARN_AMT TO SV_EARN_ADJUSTED
                point_values.SV_EARN_ADJUSTED = ws_compute_totals.WS_EARN_AMT;
            } //- END-IF
            //- IF META_SW(5) == 1
            if (META_SW[5] == 1)
            {
                //- COMPUTE WS_EARN2_AMT= ROUNDED PV_EARN2_01 * WS_EARN_POINTS
                ws_compute_totals.WS_EARN2_AMT = Round(point_values.PV_EARN2_01 * ws_compute_totals.WS_EARN_POINTS);
                //- IF PV_ADJUST_BADGE2 > 0 AND
                //-    PV_ADJUST_BADGE2 = HOLD_BADGE THEN
                if (point_values.PV_ADJUST_BADGE2 > 0 && point_values.PV_ADJUST_BADGE2 == hold_key.HOLD_BADGE)
                {
                    //- MOVE WS_EARN2_AMT TO SV_EARN2_AMT
                    point_values.SV_EARN2_AMT = ws_compute_totals.WS_EARN2_AMT;
                    //- ADD PV_ADJ_EARN2 TO WS_EARN2_AMT
                    ws_compute_totals.WS_EARN2_AMT += point_values.PV_ADJ_EARN2;
                    //- MOVE WS_EARN2_AMT TO SV_EARN2_ADJUSTED
                    point_values.SV_EARN2_ADJUSTED = ws_compute_totals.WS_EARN2_AMT;
                } //- END-IF
            }
            else //- ELSE
            {
                //- MOVE ZERO TO WS_EARN2_AMT
                ws_compute_totals.WS_EARN2_AMT = 0m;
            } //- END-IF
        } //- END-IF.

        //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //- *  ETVA EARNINGS ARE CALCULATED AND WRITTEN TO PY-PROF-ETVA
        //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //- **  need to subtract CAF out of PY-PS-ETVA for people not fully vested
        //- **  because  we can't give earnings for 2021 on class action funds -
        //- **  they were added in 2021.   CAF was added to PY-PS-ETVA for
        //- **  PY-PS-YEARS < 6.

        //- MOVE 0 TO WS_PY_PS_ETVA.
        WS_PY_PS_ETVA = 0;
        //- IF PY_PS_ETVA OF PAYPROF_REC > 0
        if (payprof_rec.PY_PS_ETVA > 0)
        {
            //- IF PY_PS_YEARS OF PAYPROF_REC < 6
            if (payprof_rec.PY_PS_YEARS < 6)
            {
                //- COMPUTE WS_PY_PS_ETVA = PY_PS_ETVA  OF PAYPROF_REC
                //-     - WS_PROF_CAF
                WS_PY_PS_ETVA = payprof_rec.PY_PS_ETVA - ws_payprofit.WS_PROF_CAF;
            }
            else //- ELSE
            {
                //- MOVE PY_PS_ETVA OF PAYPROF_REC TO WS_PY_PS_ETVA
                payprof_rec.PY_PS_ETVA = WS_PY_PS_ETVA;
            } //- END-I
        } //- END-IF.

        //- IF WS_PY_PS_ETVA > 0 OR WS_REWRITE_WHICH == 2
        if (WS_PY_PS_ETVA > 0 || WS_REWRITE_WHICH == 2)
        {
            //- NEXT SENTENCE
        }
        else //- ELSE
        {
            //- MOVE WS_EARN_AMT TO PY_PROF_EARN OF PAYPROF_REC
            payprof_rec.PY_PROF_EARN = ws_compute_totals.WS_EARN_AMT;
            //- IF META_SW(5) == 1
            if (META_SW[5] == 1)
            {
                //- MOVE WS_EARN2_AMT TO PY_PROF_EARN2 OF PAYPROF_REC
                payprof_rec.PY_PROF_EARN2 = ws_compute_totals.WS_EARN2_AMT;
            }
            else //- ELSE
            {
                //- MOVE ZERO TO PY_PROF_EARN2 OF PAYPROF_REC
                payprof_rec.PY_PROF_EARN2 = 0m;
            } //- END-IF
            //- MOVE ZERO TO PY_PROF_ETVA OF PAYPROF_REC
            payprof_rec.PY_PROF_ETVA = 0m;
            //- MOVE ZERO TO PYBEN_PROF_EARN
            payben_rec.PYBEN_PROF_EARN = 0m;
            //- MOVE ZERO TO PY_PROF_ETVA2 OF PAYPROF_REC
            payprof_rec.PY_PROF_ETVA2 = 0m;
            //- MOVE ZERO TO PYBEN_PROF_EARN2
            payben_rec.PYBEN_PROF_EARN2 = 0m;
            //- GO TO 250_EXIT
            goto l250_EXIT;
        } //- END-IF.

        //- IF WS_REWRITE_WHICH == 1
        if (WS_REWRITE_WHICH == 1 && ws_compute_totals.WS_POINTS_DOLLARS > 0)
        {
            //- COMPUTE WS_ETVA_PERCENT =
            //-     WS_PY_PS_ETVA / WS_POINTS_DOLLARS
            WS_ETVA_PERCENT = WS_PY_PS_ETVA / ws_compute_totals.WS_POINTS_DOLLARS;
            //- COMPUTE WS_ETVA_AMT= ROUNDED (WS_EARN_AMT * WS_ETVA_PERCENT)
            WS_ETVA_AMT = Round(ws_compute_totals.WS_EARN_AMT * WS_ETVA_PERCENT);
            //- COMPUTE WS_EARN_AMT = WS_EARN_AMT - WS_ETVA_AMT
            ws_compute_totals.WS_EARN_AMT = ws_compute_totals.WS_EARN_AMT - WS_ETVA_AMT;
            //- MOVE WS_EARN_AMT TO PY_PROF_EARN OF PAYPROF_REC
            payprof_rec.PY_PROF_EARN = ws_compute_totals.WS_EARN_AMT;
            //- MOVE WS_ETVA_AMT TO PY_PROF_ETVA OF PAYPROF_REC
            payprof_rec.PY_PROF_ETVA = WS_ETVA_AMT;
        } //- END-IF.

        //- IF WS_REWRITE_WHICH == 2
        if (WS_REWRITE_WHICH == 2)
        {
            //- MOVE ZERO TO PYBEN_PROF_EARN
            payben_rec.PYBEN_PROF_EARN = 0m;
            //- MOVE WS_EARN_AMT TO PYBEN_PROF_EARN
            payben_rec.PYBEN_PROF_EARN = ws_compute_totals.WS_EARN_AMT;
        } //- END-IF.

        //- IF META_SW(5) == 1
        if (META_SW[5] == 1)
        {
            //- COMPUTE WS_ETVA2_AMT= ROUNDED (WS_EARN2_AMT * WS_ETVA_PERCENT)
            WS_ETVA2_AMT = Round(ws_compute_totals.WS_EARN2_AMT * WS_ETVA_PERCENT);
            //- COMPUTE WS_EARN2_AMT = WS_EARN2_AMT - WS_ETVA2_AMT
            ws_compute_totals.WS_EARN2_AMT = ws_compute_totals.WS_EARN2_AMT - WS_ETVA2_AMT;
            //- MOVE WS_EARN2_AMT TO PY_PROF_EARN2 OF PAYPROF_REC
            payprof_rec.PY_PROF_EARN2 = ws_compute_totals.WS_EARN2_AMT;
            //- MOVE WS_ETVA2_AMT TO PY_PROF_ETVA2 OF PAYPROF_REC
            payprof_rec.PY_PROF_ETVA2 = WS_ETVA2_AMT;
            //- IF WS_REWRITE_WHICH == 2
            if (WS_REWRITE_WHICH == 2)
            {
                //- MOVE WS_ETVA2_AMT TO PYBEN_PROF_EARN2
                payben_rec.PYBEN_PROF_EARN2 = WS_ETVA2_AMT;
            } //- END-IF
        } //- END-IF.

    //- 250_EXIT.
    l250_EXIT:;
        //- EXIT.
    }

    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *     MAXIMUM CONTRIBUTION
    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 260_MAXCONT.
    public void m260Maxcont()
    {
        //- *     display "at 260-MAXCONT".
        //- COMPUTE WS_OVER = WS_MAX - WS_CONTR_MAX.
        ws_maxcont_totals.WS_OVER = ws_maxcont_totals.WS_MAX - WS_CONTR_MAX;

        //- IF WS_OVER NOT GREATER THAN WS_PROF_FORF
        if (ws_maxcont_totals.WS_OVER < ws_payprofit.WS_PROF_FORF)
        {
            //- SUBTRACT WS_OVER FROM WS_PROF_FORF
            //-    GIVING WS_PROF_FORF
            ws_payprofit.WS_PROF_FORF = ws_payprofit.WS_PROF_FORF - ws_maxcont_totals.WS_OVER;
        }
        else //- ELSE
        {
            //- DISPLAY "FORFEITURES NOT ENOUGH FOR AMOUNT OVER MAX "
            //- "FOR EMPLOYEE BADGE #" HOLD_BADGE
            DISPLAY($"FORFEITURES NOT ENOUGH FOR AMOUNT OVER MAX FOR EMPLOYEE BADGE #{hold_key.HOLD_BADGE}");
            //- MOVE 0 TO WS_PROF_FORF
            ws_payprofit.WS_PROF_FORF = 0;
        } //- END-IF.
        //- MOVE WS_OVER TO FD_MAXOVER.
        prft.FD_MAXOVER = ws_maxcont_totals.WS_OVER;
        //- MOVE WS_PROF_POINTS TO FD_MAXPOINTS.
        prft.FD_MAXPOINTS = ws_payprofit.WS_PROF_POINTS;
        //- MOVE 1 TO PY_PROF_MAXCONT OF payprof_rec.
        payprof_rec.PY_PROF_MAXCONT = 1;
        //- MOVE 1 TO WS_RERUN_IND.
        ws_indicators.WS_RERUN_IND = 1;
    //- 260_EXIT.
    l260_EXIT:;
        //- EXIT.
    }

    private void DISPLAY(string v)
    {
        Console.WriteLine("DISPLAY: " + v);
    }


    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *     PROCESS/WRITE/REWRITE FILES
    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 400_LOAD_PAYPROFIT.
    public void m400LoadPayprofit()
    {
        //- *     display "at 400-LOAD-PAYPROFIT".
        //- MOVE WS_PROF_CONT TO PY_PROF_CONT OF payprof_rec.
        payprof_rec.PY_PROF_CONT = ws_payprofit.WS_PROF_CONT;
        //- MOVE WS_PROF_FORF TO PY_PROF_FORF OF payprof_rec.
        payprof_rec.PY_PROF_FORF = ws_payprofit.WS_PROF_FORF;
    //- 400_EXIT.
    l400_EXIT:;
        //- EXIT.
    }

    //- 410_LOAD_PROFIT.
    public void m410LoadProfit()
    {
        //- *     display "at 410-LOAD-PROFIT".
        //- MOVE WS_FD_BADGE   TO FD_BADGE.
        prft.FD_BADGE = intermediate_values.WS_FD_BADGE;
        //- MOVE WS_FD_NAME    TO FD_NAME.
        prft.FD_NAME = intermediate_values.WS_FD_NAME;
        //- MOVE WS_FD_SSN     TO FD_SSN.
        prft.FD_SSN = intermediate_values.WS_FD_SSN;
        //- MOVE WS_FD_PSN     TO FD_PSN.
        prft.FD_PSN = intermediate_values.WS_FD_PSN;
        //- MOVE WS_FD_AMT     TO FD_AMT.
        prft.FD_AMT = intermediate_values.WS_FD_AMT;
        //- MOVE WS_FD_DIST1   TO FD_DIST1.
        prft.FD_DIST1 = intermediate_values.WS_FD_DIST1;
        //- MOVE WS_FD_MIL     TO FD_MIL.
        prft.FD_MIL = intermediate_values.WS_FD_MIL;
        //- MOVE WS_FD_XFER    TO FD_XFER.
        prft.FD_XFER = intermediate_values.WS_FD_XFER;
        //- MOVE WS_FD_PXFER   TO FD_PXFER.
        prft.FD_PXFER = intermediate_values.WS_FD_PXFER;
        //- MOVE WS_FD_NEWEMP  TO FD_NEWEMP.
        prft.FD_NEWEMP = intermediate_values.WS_FD_NEWEMP;
        //- MOVE WS_FD_POINTS  TO FD_POINTS.
        prft.FD_POINTS = intermediate_values.WS_FD_POINTS;
        //- MOVE WS_FD_POINTS_EARN  TO FD_POINTS_EARN.
        prft.FD_POINTS_EARN = intermediate_values.WS_FD_POINTS_EARN;
        //- MOVE WS_FD_CONT    TO FD_CONT .
        prft.FD_CONT = intermediate_values.WS_FD_CONT;
        //- MOVE WS_FD_FORF    TO FD_FORF.
        prft.FD_FORF = intermediate_values.WS_FD_FORF;
        //- MOVE WS_FD_EARN    TO FD_EARN.
        prft.FD_EARN = intermediate_values.WS_FD_EARN;
        //- MOVE WS_FD_CAF     TO FD_CAF.
        prft.FD_CAF = intermediate_values.WS_FD_CAF;
        //- ADD WS_FD_ETVA     TO FD_EARN.
        prft.FD_EARN += intermediate_values.WS_FD_ETVA;
        //- MOVE WS_FD_EARN2   TO FD_EARN2.
        prft.FD_EARN2 = intermediate_values.WS_FD_EARN2;
        //- ADD WS_FD_ETVA2    TO FD_EARN2.
        prft.FD_EARN2 += intermediate_values.WS_FD_ETVA2;
        //- WRITE PRFT.
        prfts.Add(prft);
        prft = new PRFT();

    //- 410_EXIT.
    l410_EXIT:;
        //- EXIT.
    }

    List<PRFT> prfts = new();

    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *     REWRITE PAYPROFIT
    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 420_REWRITE_PAYPROFIT.
    public void m420RewritePayprofit()
    {
        //- *     display "at 420-REWRITE-PAYPROFIT".
        //- IF META_SW(8) == 1
        if (META_SW[8] == 1)
        {
            //- GO TO 420_EXIT
            goto l420_EXIT;
        } //- END-IF.

        //- *set the newemp flag back to zero for underage employee beneficiaries
        //- IF PY_PROF_NEWEMP OF PAYPROF_REC == 2
        if (payprof_rec.PY_PROF_NEWEMP == 2)
        {
            //- MOVE ZERO TO PY_PROF_NEWEMP OF PAYPROF_REC
            payprof_rec.PY_PROF_NEWEMP = 0;
        } //- END-IF.

        //- CALL "REWRITE_KEY_PAYPROFIT" USING PAYPROFIT_FILE_STATUS PAYPROF_REC
        PAYPROFIT_FILE_STATUS = REWRITE_KEY_PAYPROFIT(payprof_rec);
        //- IF PAYPROFIT_FILE_STATUS NOT == "00"
        if (PAYPROFIT_FILE_STATUS != "00")
        {
            //- MOVE "PAY444" TO DAEMON_DISP_PROG
            DAEMON_DISP_PROG = "PAY444";
            //- MOVE SPACES TO DAEMON_DISP_MSG
            DAEMON_DISP_MSG = SPACES;
            //- STRING  "BAD REWRITE OF PAYPROFIT EMPLOYEE BADGE # "
            //- PAYPROF_BADGE OF PAYPROF_REC
            //- DELIMITED SIZE INTO DAEMON_DISP_MSG
            DAEMON_DISP_DISPLAY = $"BAD REWRITE OF PAYPROFIT EMPLOYEE BADGE # {payprof_rec.PAYPROF_BADGE}";
            //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY
            DISPCONS(DAEMON_DISP_DISPLAY);
            //- CLOSE PROFIT
            //- CALL "CLOSE_PAYPROFIT" USING PAYPROFIT_FILE_STATUS
            //- EXIT PROGRAM
        } //- END-IF.
          //- 420_EXIT.
    l420_EXIT:;
        //- EXIT.
    }

    private string? REWRITE_KEY_PAYPROFIT(PAYPROF_REC payprof_rec)
    {
        throw new NotImplementedException();
    }

    public long SOC_SEC_NUMBER { get; set; }


    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *     REWRITE PAYBEN
    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 430_REWRITE_PAYBEN.
    public void m430RewritePayben()
    {
        //- *     display "at 430-REWRITE-PAYBEN".
        //- IF (PYBEN_PROF_EARN == 0 AND PYBEN_PROF_EARN2 == 0)
        //-    or META_SW(8) = 1
        if ((payben_rec.PYBEN_PROF_EARN == 0 && payben_rec.PYBEN_PROF_EARN2 == 0 || META_SW[8] == 1))
        {
            //- GO TO 430_EXIT
            goto l430_EXIT;
        } //- END-IF.
          //- CALL "REWRITE_KEY_PAYBEN" USING PAYBEN_FILE_STATUS PAYBEN_REC.
        PAYBEN_FILE_STATUS = REWRITE_KEY_PAYBEN(payben_rec);
        //- IF PAYBEN_FILE_STATUS NOT == "00"
        if (PAYBEN_FILE_STATUS != "00")
        {
            //- MOVE "PAY444" TO DAEMON_DISP_PROG
            DAEMON_DISP_PROG = "PAY444";
            //- MOVE SPACES TO DAEMON_DISP_MSG
            DAEMON_DISP_MSG = SPACES;
            //- STRING  "BAD REWRITE OF PAYBEN # " PAYPROF_BADGE OF PAYPROF_REC
            //- DELIMITED SIZE INTO DAEMON_DISP_MSG
            //- CALL "DISPCONS" USING DAEMON_DISP_DISPLAY
            DISPCONS(DAEMON_DISP_DISPLAY);
            //- CLOSE PROFIT
            //- CALL "CLOSE_PAYBEN" USING PAYBEN_FILE_STATUS
            //- EXIT PROGRAM
        } //- END-IF.
          //- 430_EXIT.
    l430_EXIT:;
        //- EXIT.
    }

    private string? REWRITE_KEY_PAYBEN(PAYBEN_REC payben_rec)
    {
        throw new NotImplementedException();
    }

    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *     PROCESS PROFIT SHARING FILES
    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 500_GET_DB_INFO.
    public void m500GetDbInfo()
    {
        //- *    display "at 500-GET-DB-INFO [" PAYPROF-SSN OF PAYPROF-REC "}".
        //- MOVE ZERO TO DIST_TOTAL.
        DIST_TOTAL = 0m;
        //- MOVE ZERO TO FORFEIT_TOTAL.
        FORFEIT_TOTAL = 0m;
        //- MOVE ZERO TO ALLOCATION_TOTAL.
        ALLOCATION_TOTAL = 0m;
        //- MOVE ZERO TO PALLOCATION_TOTAL.
        PALLOCATION_TOTAL = 0m;

        //- MOVE PAYPROF_SSN OF PAYPROF_REC TO SOC_SEC_NUMBER.
        SOC_SEC_NUMBER = payprof_rec.PAYPROF_SSN;
        //- CALL "MSTR_FIND_ANY_SOC_SEC".
        MSTR_FIND_ANY_SOC_SEC();
        //- IF DB_STATUS NOT == ZERO
        if (DB_STATUS != 0)
        {
            //- GO TO 500_EXIT.
            goto l500_EXIT;
        }
        //- PERFORM 510_GET_DETAILS THRU 510_EXIT
        //- UNTIL DB_STATUS NOT = ZERO.
        while (DB_STATUS == 0)
        {
            m510GetDetails();
        }

    //- 500_EXIT.
    l500_EXIT:;
        //- EXIT.
    }

    private void MSTR_FIND_ANY_SOC_SEC()
    {
        // Always move forward to finders
        DB_STATUS = 0; // aka not now
    }

    //- 510_GET_DETAILS.
    public void m510GetDetails()
    {
        //- *     display "at 510-GET-DETAILS ["  SOC-SEC-NUMBER "]".

        //- MOVE "FIRST" TO IDS2_NAVIGATION_MODE
        //- MOVE "PR_DET" TO IDS2_REC_NAME
        //- CALL "MSTR_FIND_WITHIN_PR_DET_S" USING IDS2_NAVIGATION_MODE
        //- IDS2_REC_NAME.
        DB_STATUS = MSTR_FIND_WITHIN_PR_DET_S();
        //- IF DB_STATUS == 0
        if (DB_STATUS == 0)
        {
            //- PERFORM 520_TOTAL_UP_DETAILS THRU 520_EXIT UNTIL
            //-     DB_STATUS NOT = ZERO.
            while (DB_STATUS == 0)
            {
                m520TotalUpDetails();
            }
        }

        //- MOVE "FIRST" TO IDS2_NAVIGATION_MODE
        //- CALL "MSTR_FIND_WITHIN_PR_SS_D_S" USING IDS2_NAVIGATION_MODE
        //- IDS2_REC_NAME.
        DB_STATUS = MSTR_FIND_WITHIN_PR_SS_D_S();
        //- IF DB_STATUS == 0
        if (DB_STATUS == 0)
        {
            //- PERFORM 530_TOTAL_UP_SS_DETAILS THRU 530_EXIT UNTIL
            //-    DB_STATUS NOT = ZERO.
            // Seems dumb, because m530 is already looping.
            while (DB_STATUS != 0)
            {
                m530TotalUpSsDetails();
            }
        }
    //- 510_EXIT.
    l510_EXIT:;
        //- EXIT.
    }

    private int MSTR_FIND_WITHIN_PR_SS_D_S()
    {
        // throw new NotImplementedException();
        Debug.WriteLine("WARNING: READING PROFIT_SS_DETAILS IS DISABLED.");
        return 77;   // BOBH THIS IS DISABLED!!!
    }

    private ProfitDetailTableHelper profitDetailTable = null;

    private int MSTR_FIND_WITHIN_PR_DET_S()
    {
        if (profitDetailTable == null || profitDetailTable.ssn != SOC_SEC_NUMBER)
        {
            profitDetailTable = new ProfitDetailTableHelper(connection, profit_detail, SOC_SEC_NUMBER);
        }
        // Load profit_detail using SOC_SEC_NUMBER;
        return profitDetailTable.LoadNextRecord();
    }


    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *     PROCESS DETAILS
    //- * =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 520_TOTAL_UP_DETAILS.
    public void m520TotalUpDetails()
    {
    l520_TOTAL_UP_DETAILS:
        //- *        display "at 520-TOTAL-UP-DETAILS".
        //- MOVE SPACES TO IDS2_REC_NAME
        //- CALL "MSTR_GET_REC" USING IDS2_REC_NAME.
        // not needed?   DB_STATUS = MSTR_GET_REC(IDS2_REC_NAME);

        //- MOVE PROFIT_YEAR TO ws_profit_year.
        ws_profit_year.WS_PROFIT_YEAR_FIRST_4 = (long) profit_detail.PROFIT_YEAR;
        string[] parts = profit_detail.PROFIT_YEAR.ToString().Split('.');
        long decimalPart = parts.Length > 1 ? long.Parse(parts[1]) : 0;
        ws_profit_year.WS_PROFIT_YEAR_EXTENSION = decimalPart;

        //- * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //- IF WS_PROFIT_YEAR_FIRST_4 == EFFECT_DATE
        //-     AND PROFIT_CLIENT = 1
        if (ws_profit_year.WS_PROFIT_YEAR_FIRST_4 == input_dates.EFFECT_DATE)
        {
            //- * Distributions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //- IF PROFIT_CODE == "1" OR "3"
            if (profit_detail.PROFIT_CODE == "1" || profit_detail.PROFIT_CODE == "3")
            {
                //- COMPUTE DIST_TOTAL = DIST_TOTAL + PROFIT_FORT
                DIST_TOTAL = DIST_TOTAL + profit_detail.PROFIT_FORT;
            } //- END-IF    

            //- * Main-1668 - 9 WITH XFER/QDRO IS AN ALLOCATION OTHERS ARE DISBURSEMENTS
            //- IF PROFIT_CODE == "9"
            if (profit_detail.PROFIT_CODE == "9")
            {
                //- IF (PROFIT_CMNT (1:6) == "XFER >" OR
                //- PROFIT_CMNT (1:6) = "QDRO >" OR
                //- PROFIT_CMNT (1:5) = "XFER>"  OR
                //- PROFIT_CMNT (1:5) = "QDRO>")
                if (profit_detail.PROFIT_CMNT![0..6] == "XFER >" ||
                    profit_detail.PROFIT_CMNT[0..6] == "QDRO >" ||
                    profit_detail.PROFIT_CMNT[0..5] == "XFER>" ||
                    profit_detail.PROFIT_CMNT[0..5] == "QDRO>")
                {
                    //- COMPUTE PALLOCATION_TOTAL = PALLOCATION_TOTAL +
                    //- PROFIT_FORT
                    PALLOCATION_TOTAL = PALLOCATION_TOTAL + profit_detail.PROFIT_FORT;
                }
                else //- ELSE
                {
                    //- COMPUTE DIST_TOTAL = DIST_TOTAL + PROFIT_FORT
                    DIST_TOTAL = DIST_TOTAL + profit_detail.PROFIT_FORT;
                } //- END-IF
            } //- END-IF

            //- * Paid Forfeitures  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //- IF PROFIT_CODE == "2"
            if (profit_detail.PROFIT_CODE == "2")
            {
                //- COMPUTE FORFEIT_TOTAL = FORFEIT_TOTAL + PROFIT_FORT
                FORFEIT_TOTAL = FORFEIT_TOTAL + profit_detail.PROFIT_FORT;
            } //- END-IF

            //- * Paid Allocations ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //- IF PROFIT_CODE == "5"
            if (profit_detail.PROFIT_CODE == "5")
            {
                //- COMPUTE PALLOCATION_TOTAL = PALLOCATION_TOTAL +
                //-     PROFIT_FORT
                PALLOCATION_TOTAL = PALLOCATION_TOTAL + profit_detail.PROFIT_FORT;
            } //- END-IF

            //- * Contributions from Allocations ~~~~~~~~~~~~~~~~~
            //- IF PROFIT_CODE == "6"
            if (profit_detail.PROFIT_CODE == "6")
            {
                //- COMPUTE ALLOCATION_TOTAL = ALLOCATION_TOTAL + PROFIT_CONT
                ALLOCATION_TOTAL = ALLOCATION_TOTAL + profit_detail.PROFIT_CONT;
            } //- END-IF

            //- * Military Contributions ~~~~~~~~~~~~~~~~~~~~~~~~~
            //- IF WS_PROFIT_YEAR_EXTENSION == 1
            if (ws_profit_year.WS_PROFIT_YEAR_EXTENSION == 1)
            {
                //- COMPUTE WS_PROF_MIL = WS_PROF_MIL + PROFIT_CONT
                ws_payprofit.WS_PROF_MIL = ws_payprofit.WS_PROF_MIL + profit_detail.PROFIT_CONT;
            } //- END-IF

            //- * Class Action Fund for 2021 ~~~~~~~~~~~~~~~~~~~~~
            //- IF WS_PROFIT_YEAR_EXTENSION == 2
            if (ws_profit_year.WS_PROFIT_YEAR_EXTENSION == 2)
            {
                //- COMPUTE WS_PROF_CAF = WS_PROF_CAF + PROFIT_EARN
                ws_payprofit.WS_PROF_CAF = ws_payprofit.WS_PROF_CAF + profit_detail.PROFIT_EARN;
            } //- END-IF
        } //- END-IF.
          //- * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //- MOVE "NEXT" TO IDS2_NAVIGATION_MODE
        //- MOVE "PR_DET" TO IDS2_REC_NAME
        //- CALL "MSTR_FIND_WITHIN_PR_DET_S" USING IDS2_NAVIGATION_MODE
        //- IDS2_REC_NAME.
        DB_STATUS = MSTR_FIND_WITHIN_PR_DET_S();

        //- IF DB_STATUS == 0
        if (DB_STATUS == 0)
        {
            //- GO TO 520_TOTAL_UP_DETAILS.
            goto l520_TOTAL_UP_DETAILS;
        }
    //- 520_EXIT.
    l520_EXIT:;
        //- EXIT.
    }

    //- 530_TOTAL_UP_SS_DETAILS.
    public void m530TotalUpSsDetails()
    {
    l530_TOTAL_UP_SS_DETAILS:
        //- *     display "at 530-TOTAL-UP-SS-DETAILS".
        //- MOVE SPACES TO IDS2_REC_NAME
        //- CALL "MSTR_GET_REC" USING IDS2_REC_NAME.
        // MSTR_GET_REC(IDS2_REC_NAME); 

        //- MOVE PROFIT_SS_YEAR TO ws_profit_year.
        ws_profit_year.WS_PROFIT_YEAR_FIRST_4 = (long) profit_ss_detail.PROFIT_SS_YEAR;
        string[] parts = profit_ss_detail.PROFIT_SS_YEAR.ToString().Split('.');
        long decimalPart = parts.Length > 1 ? long.Parse(parts[1]) : 0;
        ws_profit_year.WS_PROFIT_YEAR_EXTENSION = decimalPart;

        //- IF WS_PROFIT_YEAR_FIRST_4 == EFFECT_DATE
        //-    AND PROFIT_SS_CLIENT = 1
        if (ws_profit_year.WS_PROFIT_YEAR_FIRST_4 == input_dates.EFFECT_DATE)
        {
            //- IF PROFIT_SS_CODE == "1" OR "3" OR "9"
            if (profit_ss_detail.PROFIT_SS_CODE == "1" || profit_ss_detail.PROFIT_SS_CODE == "3" || profit_ss_detail.PROFIT_SS_CODE == "9")
            {
                //- ADD PROFIT_SS_FORT TO DIST_TOTAL
                DIST_TOTAL += profit_ss_detail.PROFIT_SS_FORT;
            } //- END-IF
        } //- END-IF.
          //- MOVE "NEXT" TO IDS2_NAVIGATION_MODE
        //- MOVE "PR_SD" TO IDS2_REC_NAME
        //- CALL "MSTR_FIND_WITHIN_PR_SS_D_S" USING IDS2_NAVIGATION_MODE
        //- IDS2_REC_NAME.
        MSTR_FIND_WITHIN_PR_SS_D_S();

        //- IF DB_STATUS == 0
        if (DB_STATUS == 0)
        {
            //- GO TO 530_TOTAL_UP_SS_DETAILS.
            goto l530_TOTAL_UP_SS_DETAILS;
        }
    //- 530_EXIT.
    l530_EXIT:;
        //- EXIT.
    }

    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *     PRINT REPORT
    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 800_PRINT_SEQUENCE SECTION 60.

    //- 805_PRINT_SEQUENCE.
    public void m805PrintSequence()
    {
        //- *     display "at 805-PRINT-SEQUENCE".
        //- INITIALIZE WS_END_IND ws_maxcont_totals.
        ws_indicators.WS_END_IND = 0;
        ws_maxcont_totals = new WS_MAXCONT_TOTALS();

        //- OPEN OUTPUT PRINT_FILE.
        //- MOVE SPACES TO P_REC.
        P_REC = SPACES;
        //- WRITE P_REC FROM XEROX_HEADER AFTER ADVANCING PAGE.
        WRITE("\f"+xerox_header.ToString().Trim());
        //- MOVE SPACES TO P_REC.
        P_REC = SPACES;
        //- ACCEPT WS_DATE FROM DATE.

        // 11/11/24    14:17

        //- MOVE WS_YY TO HDR1_YY, TOT_HDR1_YY.
        header_1.HDR1_YY = 24; //ws_date_time.WS_YY;
        //- MOVE WS_MM TO HDR1_MM, TOT_HDR1_MM.
        header_1.HDR1_MM = 11; //ws_date_time.WS_MM;
        //- MOVE WS_DD TO HDR1_DD, TOT_HDR1_DD.
        header_1.HDR1_DD = 12; //ws_date_time.WS_DD;
        //- IF WS_MM NOT == 12
        if (ws_date_time.WS_MM != 12)
        {
            //- SUBTRACT 1 FROM WS_CCYY.
            ws_date_time.WS_CCYY = ws_date_time.WS_CCYY - 1;

        }
        //- MOVE WS_YY TO WS_YEAR.
        ws_date_time.WS_YEAR = ws_date_time.WS_YY; // 2 digits

        //- MOVE HOLD_EFF_DATE TO HDR1_YEAR1, TOT_HDR1_YEAR1.
        header_1.HDR1_YEAR1 = HOLD_EFF_DATE;
        total_header_1.TOT_HDR1_YEAR1 = HOLD_EFF_DATE;
        total_header_1.TOT_HDR1_DD = 12;
        total_header_1.TOT_HDR1_MM = 11;
            total_header_1.TOT_HDR1_YY = 24;
        //- ACCEPT WS_TIME FROM TIME.
        //?

        //- MOVE WS_HR TO HDR1_HR, TOT_HDR1_HR.
        header_1.HDR1_HR = 9; // ws_date_time.WS_HR;
        //- MOVE WS_MN TO HDR1_MN, TOT_HDR1_MN.
        header_1.HDR1_MN = 43; // ws_date_time.WS_MN;

        total_header_1.TOT_HDR1_HR = 9;
        total_header_1.TOT_HDR1_MN = 43;


        //- PERFORM 810_WRITE_REPORT THRU 810_EXIT
        //- UNTIL WS_END_IND = 1.
        prfts.Sort((a, b) =>
        {
            int nameComparison = StringComparer.Ordinal.Compare(a.FD_NAME, b.FD_NAME);
            return nameComparison != 0 ? nameComparison : StringComparer.Ordinal.Compare(a.FD_BADGE, b.FD_BADGE);
        });

        foreach (var sd_sorted in prfts.Select(p => new SD_PRFT(p)).ToList())
        {
            sd_prft = sd_sorted;
            m810WriteReport();
        }

        //- PERFORM 850_PRINT_TOTALS THRU 850_EXIT.
        m850PrintTotals();

        //- PERFORM 855_GRAND_TOTALS THRU 855_EXIT.
        m855GrandTotals();

        //- CLOSE PRINT_FILE.
        // CLOSE(PRINT_FILE);
        //- GO TO 899_EXIT.
    }

    public List<string> outputLines = new();

    private void WRITE(Object obj)
    {
        Console.WriteLine(obj.ToString());
        outputLines.Add(obj.ToString().TrimEnd());
    }


    //- 810_WRITE_REPORT.
    public void m810WriteReport()
    {
        //- *     display "at 810-WRITE-REPORT".

        //- RETURN SORT_FILE END
        //- MOVE 1 TO WS_END_IND
        //- GO TO 810_EXIT.

        //- IF WS_LINE_CTR > 60
        if (ws_counters.WS_LINE_CTR > 60)
        {
            //- PERFORM 830_PRINT_HEADER THRU 830_EXIT.
            m830PrintHeader();
        }

        //- IF SD_BADGE > 0
        if (sd_prft.SD_BADGE > 0)
        {
            //- MOVE SD_BADGE   TO BADGE_NBR
            report_line.BADGE_NBR = sd_prft.SD_BADGE;
            //- MOVE SD_NAME(1:24) TO EMP_NAME
            report_line.EMP_NAME = sd_prft.SD_NAME?.Length > 24 ? sd_prft.SD_NAME.Substring(0, 24) : sd_prft.SD_NAME;

            //- MOVE SD_SSN     TO WS_SSN
            ws_social_security.WS_SSN = sd_prft.SD_SSN;
            //- MOVE SD_AMT     TO BEG_BAL
            report_line.BEG_BAL = sd_prft.SD_AMT;
            //- MOVE SD_DIST1   TO PR_DIST1
            report_line.PR_DIST1 = sd_prft.SD_DIST1;

            //- EVALUATE SD_NEWEMP
            //- WHEN 1 MOVE "NEW" TO PR_NEWEMP
            //- WHEN 2 PERFORM 820_CHECK_PAYBEN THRU 820_EXIT
            //- WHEN OTHER MOVE "   " TO PR_NEWEMP
            //- END_EVALUATE
            if (sd_prft.SD_NEWEMP == 1)
            {
                report_line.PR_NEWEMP = "NEW";
            }
            else if (sd_prft.SD_NEWEMP == 2){
                m820CheckPayben();
            }
            else
            {
                report_line.PR_NEWEMP = " ";
            }

            //- * Add the payben transfers to the contribution for reporting
            //- * *** It MUST NOT be included in the beginning balance
            //- IF SD_XFER NOT == 0
            if (sd_prft.SD_XFER != 0)
            {
                //- COMPUTE SD_CONT = SD_CONT + SD_XFER
                sd_prft.SD_CONT = sd_prft.SD_CONT + sd_prft.SD_XFER;
            } //- END-IF
              //- IF SD_PXFER NOT == 0
            if (sd_prft.SD_PXFER != 0)
            {
                //- COMPUTE SD_MIL = SD_MIL - SD_PXFER
                sd_prft.SD_MIL = sd_prft.SD_MIL - sd_prft.SD_PXFER;
            } //- END-IF
              //- MOVE SD_CONT    TO PR_CONT
            report_line.PR_CONT = sd_prft.SD_CONT;
            //- MOVE SD_MIL     TO PR_MIL
            report_line.PR_MIL = sd_prft.SD_MIL;
            //- MOVE SD_FORF    TO PR_FORF
            report_line.PR_FORF = sd_prft.SD_FORF;
            //- MOVE SD_EARN    TO PR_EARN
            report_line.PR_EARN = sd_prft.SD_EARN;

            //- if sd_earn2 not = 0
            if (sd_prft.SD_EARN2 != 0)
            {
                //- display "badge " sd_badge " earnings2 " sd_earn2
                DISPLAY($"badge {sd_prft.SD_BADGE} earnings2 ${sd_prft.SD_EARN2}");
            } //- end-if
              //- MOVE SD_EARN2   TO PR_EARN2
            report_line.PR_EARN2 = sd_prft.SD_EARN2;
            //- ***     move caf to earn2 - field was only used in 2008
            //- move sd_caf     to pr_earn2
            report_line.PR_EARN2 = sd_prft.SD_CAF;
        } //- END-IF.

        //- COMPUTE WS_ENDBAL =
        //- **        (SD-AMT + SD-CONT + SD-EARN + SD-EARN2 + SD-FORF + SD-MIL)
        //-           (SD_AMT + SD_CONT + SD_EARN + SD_EARN2 + SD_FORF + SD_MIL + SD_CAF)
        //-           - (SD_DIST1).
        ws_ending_balance.WS_ENDBAL = (sd_prft.SD_AMT + sd_prft.SD_CONT + sd_prft.SD_EARN + sd_prft.SD_EARN2 + sd_prft.SD_FORF + sd_prft.SD_MIL + sd_prft.SD_CAF) - sd_prft.SD_DIST1;
        //- MOVE WS_ENDBAL TO END_BAL.
        report_line.END_BAL = ws_ending_balance.WS_ENDBAL;

        //- IF SD_BADGE == 0
        if (sd_prft.SD_BADGE == 0)
        {
            //- MOVE SD_NAME(1:24) TO PR2_EMP_NAME
            report_line_2.PR2_EMP_NAME = sd_prft.SD_NAME[0..24];
            //- MOVE SD_PSN        TO PR2_PSN
            report_line_2.PR2_PSN = sd_prft.SD_PSN;
            //- MOVE SD_AMT        TO PR2_BEG_BAL
            report_line_2.PR2_BEG_BAL = sd_prft.SD_AMT;
            //- MOVE SD_DIST1      TO PR2_DIST1
            report_line_2.PR2_DIST1 = sd_prft.SD_DIST1;
            //- MOVE "BEN"         TO PR2_NEWEMP
            report_line_2.PR2_NEWEMP = "BEN";
            //- * Add the payben transfers to the contribution for reporting
            //- * *** It MUST NOT be included in the beginning balance.
            //- ADD SD_XFER        TO SD_CONT
            sd_prft.SD_CONT += sd_prft.SD_XFER;
            //- subtract SD_PXFER   from SD_MIL
            //- MOVE SD_CONT       TO PR2_CONT
            report_line_2.PR2_CONT = sd_prft.SD_CONT;
            //- MOVE SD_MIL        TO PR2_MIL
            report_line_2.PR2_MIL = sd_prft.SD_MIL;
            //- MOVE SD_FORF       TO PR2_FORF
            report_line_2.PR2_FORF = sd_prft.SD_FORF;
            //- MOVE SD_EARN       TO PR2_EARN
            report_line_2.PR2_EARN = sd_prft.SD_EARN;
            //- MOVE SD_EARN2      TO PR2_EARN2
            report_line_2.PR2_EARN2 = sd_prft.SD_EARN2;
            //- MOVE SD_CAF        TO PR2_EARN2
            report_line_2.PR2_EARN2 = sd_prft.SD_CAF;

            //- *****    add  class action to ending balance
            //- COMPUTE WS_ENDBAL =
            //- (SD_AMT + SD_CONT + SD_EARN + SD_EARN2 + SD_FORF + SD_MIL + SD_CAF)
            //- - (SD_DIST1)
            ws_ending_balance.WS_ENDBAL = (sd_prft.SD_AMT + sd_prft.SD_CONT + sd_prft.SD_EARN + sd_prft.SD_EARN2 + sd_prft.SD_FORF + sd_prft.SD_MIL + sd_prft.SD_CAF) - (sd_prft.SD_DIST1);
            //- MOVE WS_ENDBAL TO PR2_END_BAL
            report_line_2.PR2_END_BAL = ws_ending_balance.WS_ENDBAL;
        } //- END-IF.
          //- ADD SD_AMT    TO WS_TOT_BEGBAL.
        ws_client_totals.WS_TOT_BEGBAL += sd_prft.SD_AMT;
        //- * Remove the payben transfers from the contribution/military for totalling
        //- IF SD_XFER NOT == 0
        if (sd_prft.SD_XFER != 0)
        {
            //- SUBTRACT SD_XFER FROM SD_CONT
            sd_prft.SD_CONT -= sd_prft.SD_XFER;
        } //- END-IF.
          //- IF SD_PXFER NOT == 0
        if (sd_prft.SD_PXFER != 0)
        {
            //- ADD SD_PXFER to SD_MIL
            sd_prft.SD_MIL += sd_prft.SD_PXFER;
        } //- END-IF.
          //- ADD SD_DIST1  TO WS_TOT_DIST1.
        ws_client_totals.WS_TOT_DIST1 += sd_prft.SD_DIST1;
        //- ADD SD_CONT   TO WS_TOT_CONT.
        ws_client_totals.WS_TOT_CONT += sd_prft.SD_CONT;
        //- ADD SD_MIL    TO WS_TOT_MIL.
        ws_client_totals.WS_TOT_MIL += sd_prft.SD_MIL;
        //- ADD SD_FORF   TO WS_TOT_FORF.
        ws_client_totals.WS_TOT_FORF += sd_prft.SD_FORF;
        //- ADD SD_EARN   TO WS_TOT_EARN.
        ws_client_totals.WS_TOT_EARN += sd_prft.SD_EARN;
        //- ADD SD_EARN2  TO WS_TOT_EARN2.
        ws_client_totals.WS_TOT_EARN2 += sd_prft.SD_EARN2;
        //- ADD WS_ENDBAL TO WS_TOT_ENDBAL.
        ws_client_totals.WS_TOT_ENDBAL += ws_ending_balance.WS_ENDBAL;
        //- ADD SD_XFER   TO WS_TOT_XFER.
        ws_client_totals.WS_TOT_XFER += sd_prft.SD_XFER;
        //- subtract SD_PXFER  from WS_TOT_PXFER.
        ws_client_totals.WS_TOT_PXFER -= sd_prft.SD_PXFER;
        //- ADD SD_POINTS_EARN TO WS_EARN_PTS_TOTAL.
        ws_client_totals.WS_EARN_PTS_TOTAL += sd_prft.SD_POINTS_EARN;
        //- ADD SD_POINTS TO WS_PROF_PTS_TOTAL.
        ws_client_totals.WS_PROF_PTS_TOTAL += sd_prft.SD_POINTS;
        //- ADD SD_CAF    TO WS_TOT_CAF.
        ws_client_totals.WS_TOT_CAF += sd_prft.SD_CAF;
        //- ADD SD_MAXOVER   TO WS_TOT_OVER.
        ws_maxcont_totals.WS_TOT_OVER += sd_prft.SD_MAXOVER;
        //- ADD SD_MAXPOINTS TO WS_TOT_POINTS.
        ws_maxcont_totals.WS_TOT_POINTS += sd_prft.SD_MAXPOINTS;

        //- *     if sd-maxover not =  0
        //- *        display  " ws-tot-over "      ws-tot-over
        //- *      end-if.
        //- ***   add check for CLASS ACTION FUND
        //- IF  ((SD_AMT    == ZERO)
        //- AND (SD_DIST1  = ZERO)
        //- AND (SD_CONT   = ZERO)
        //- AND (SD_XFER   = ZERO)
        //- AND (SD_PXFER  = ZERO)
        //- AND (SD_MIL    = ZERO)
        //- AND (SD_FORF   = ZERO)
        //- AND (SD_EARN   = ZERO)
        //- AND (SD_EARN2  = ZERO)
        //- ***
        //- **   causes blank line on report   AND (SD-CAF    = ZERO)
        //- ***
        //- AND (WS_ENDBAL = ZERO))
        if (sd_prft.SD_AMT == 0m
           && (sd_prft.SD_DIST1 == 0m)
            && (sd_prft.SD_CONT == 0m)
         && (sd_prft.SD_XFER == 0m)
         && (sd_prft.SD_PXFER == 0m)
         && (sd_prft.SD_MIL == 0m)
         && (sd_prft.SD_FORF == 0m)
         && (sd_prft.SD_EARN == 0m)
         && (sd_prft.SD_EARN2 == 0m))
        {
            //- GO TO 810_EXIT
            goto l810_EXIT;
        } //- END-IF.

        //- PERFORM 840_PRINT_REPORT THRU 840_EXIT.
        m840PrintReport();
    //- 810_EXIT.
    l810_EXIT:;
        //- EXIT.
    }

    //- 820_CHECK_PAYBEN.
    public void m820CheckPayben()
    {
        //- ******  check if emp is on payben (CAF triggered newemp to be set to 2,
        //- ******       causing emp to be erroneously reported as beneficiary)
        //- INITIALIZE PAYBEN_REC.
        //- MOVE SD_SSN TO PYBEN_PAYSSN.
        payben_rec.PYBEN_PAYSSN = sd_prft.SD_SSN;
        //- MOVE "PAYBEN_SK" TO UFAS_ALT_KEY_NAME.
        //- CALL "READ_ALT_KEY_PAYBEN" USING PAYBEN_FILE_STATUS
        PAYBEN_FILE_STATUS = READ_ALT_KEY_PAYBEN(payben_rec);
        //- IF PAYBEN_FILE_STATUS  == "00"
        if (PAYBEN_FILE_STATUS == "00")
        {
            //- MOVE "BEN" TO PR_NEWEMP
            report_line.PR_NEWEMP = "BEN";
        } //- END-IF.
          //- 820_EXIT.
    l820_EXIT:;
        //- EXIT.
    }

    private string? READ_ALT_KEY_PAYBEN(PAYBEN_REC payben_rec)
    {
        throw new NotImplementedException();
    }

    //- 830_PRINT_HEADER.
    public void m830PrintHeader()
    {
        //- *     display "at 830-PRINT-HEADER".
        //- ADD 1 TO WS_PAGE_CTR.
        ws_counters.WS_PAGE_CTR += 1;
        //- MOVE WS_PAGE_CTR TO HDR1_PAGE.
        header_1.HDR1_PAGE = ws_counters.WS_PAGE_CTR;
        //- WRITE P_REC FROM HEADER_1 AFTER  PAGE
        WRITE("\f"+header_1);
        //- WRITE P_REC FROM HEADER_2 AFTER  2
        WRITE("");
        WRITE(header_2);
        //- WRITE P_REC FROM HEADER_3 AFTER  1
        WRITE(header_3);
        //- MOVE 4 TO WS_LINE_CTR.
        ws_counters.WS_LINE_CTR = 4;
    //- 830_EXIT.
    l830_EXIT:;
        //- EXIT.
    }
    //- 840_PRINT_REPORT.
    public void m840PrintReport()
    {
        //- IF SD_BADGE > 0
        if (sd_prft.SD_BADGE > 0)
        {
            //- ADD 1 TO WS_EMPLOYEE_CTR
            ws_counters.WS_EMPLOYEE_CTR += 1;
            //- WRITE P_REC FROM REPORT_LINE AFTER 1
            WRITE(report_line);
        } //- END-IF.
          //- IF SD_BADGE == 0
        if (sd_prft.SD_BADGE == 0)
        {
            //- ADD 1 TO WS_EMPLOYEE_CTR_PAYBEN
            ws_counters.WS_EMPLOYEE_CTR_PAYBEN += 1;
            //- WRITE P_REC FROM REPORT_LINE_2 AFTER 1
            WRITE(report_line_2);
        } //- END-IF.
          //- ADD 1 TO WS_LINE_CTR.
        ws_counters.WS_LINE_CTR += 1;
    //- 840_EXIT.
    l840_EXIT:;
        //- EXIT.
    }

    //- 850_PRINT_TOTALS.
    public void m850PrintTotals()
    {
        //- *     display "at 850-PRINT-TOTALS".
        //- MOVE WS_TOT_BEGBAL TO BEG_BAL_TOT.
        client_tot.BEG_BAL_TOT = ws_client_totals.WS_TOT_BEGBAL;
        //- MOVE WS_TOT_DIST1  TO DIST1_TOT.
        client_tot.DIST1_TOT = ws_client_totals.WS_TOT_DIST1;
        //- MOVE WS_TOT_MIL    TO MIL_TOT.
        client_tot.MIL_TOT = ws_client_totals.WS_TOT_MIL;
        //- MOVE WS_TOT_CONT   TO CONT_TOT.
        client_tot.CONT_TOT = ws_client_totals.WS_TOT_CONT;
        //- MOVE WS_TOT_FORF   TO FORF_TOT.
        client_tot.FORF_TOT = ws_client_totals.WS_TOT_FORF;
        //- MOVE WS_TOT_EARN   TO EARN_TOT.
        client_tot.EARN_TOT = ws_client_totals.WS_TOT_EARN;
        //- MOVE WS_TOT_EARN2  TO EARN2_TOT.
        client_tot.EARN2_TOT = ws_client_totals.WS_TOT_EARN2;
        //- IF WS_TOT_EARN2 NOT == 0
        if (ws_client_totals.WS_TOT_EARN2 != 0)
        {
            //- DISPLAY "WS_TOT_EARN2 NOT 0 " WS_TOT_EARN2
            DISPLAY("WS_TOT_EARN2 NOT 0 " + ws_client_totals.WS_TOT_EARN2);
        } //- END-IF
          //- ***  USE EARN2 FIELD FOR CAF FOR 2021
          //- MOVE WS_TOT_CAF    TO EARN2_TOT.
        client_tot.EARN2_TOT = ws_client_totals.WS_TOT_CAF;
        //- MOVE WS_TOT_ENDBAL TO END_BAL_TOT.
        client_tot.END_BAL_TOT = ws_client_totals.WS_TOT_ENDBAL;

        //- ADD WS_TOT_BEGBAL  TO WS_GRTOT_BEGBAL.
        ws_grand_totals.WS_GRTOT_BEGBAL += ws_client_totals.WS_TOT_BEGBAL;
        //- ADD WS_TOT_DIST1   TO WS_GRTOT_DIST1.
        ws_grand_totals.WS_GRTOT_DIST1 += ws_client_totals.WS_TOT_DIST1;
        //- ADD WS_TOT_MIL     TO WS_grtot_MIL.
        ws_grand_totals.WS_GRTOT_MIL += ws_client_totals.WS_TOT_MIL;
        //- ADD WS_TOT_CONT    TO WS_GRTOT_CONT.
        ws_grand_totals.WS_GRTOT_CONT += ws_client_totals.WS_TOT_CONT;
        //- ADD WS_TOT_FORF    TO WS_GRTOT_FORF.
        ws_grand_totals.WS_GRTOT_FORF += ws_client_totals.WS_TOT_FORF;
        //- ADD WS_TOT_EARN    TO WS_GRTOT_EARN.
        ws_grand_totals.WS_GRTOT_EARN += ws_client_totals.WS_TOT_EARN;
        //- ADD WS_TOT_EARN2   TO WS_GRTOT_EARN2.
        ws_grand_totals.WS_GRTOT_EARN2 += ws_client_totals.WS_TOT_EARN2;
        //- ADD WS_TOT_ENDBAL  TO WS_GRTOT_ENDBAL.
        ws_grand_totals.WS_GRTOT_ENDBAL += ws_client_totals.WS_TOT_ENDBAL;
        //- ADD WS_TOT_CAF     TO WS_GRTOT_CAF.
        ws_grand_totals.WS_GRTOT_CAF += ws_client_totals.WS_TOT_CAF;
        //- WRITE P_REC FROM TOTAL_HEADER_1 AFTER PAGE.
        WRITE("\f"+total_header_1);
        //- WRITE P_REC FROM TOTAL_HEADER_2 AFTER 2.
        WRITE("");
        WRITE(total_header_2);
        //- WRITE P_REC FROM TOTAL_HEADER_3 AFTER 1.
        WRITE(total_header_3);
        //- WRITE P_REC FROM ClientTot     AFTER 2.
        WRITE("");
        WRITE(client_tot);

        //- * Allocations line in totals
        //- MOVE ZERO TO BEG_BAL_TOT DIST1_TOT CONT_TOT
        //- MIL_TOT FORF_TOT EARN_TOT END_BAL_TOT EARN2_TOT.
        client_tot.BEG_BAL_TOT = 0m;
        client_tot.DIST1_TOT = 0m;
        client_tot.CONT_TOT = 0m;
        client_tot.MIL_TOT = 0m;
        client_tot.FORF_TOT = 0m;
        client_tot.EARN_TOT = 0m;
        client_tot.END_BAL_TOT = 0m;
        client_tot.EARN2_TOT = 0m;

        //- MOVE WS_TOT_XFER TO CONT_TOT.
        client_tot.CONT_TOT = ws_client_totals.WS_TOT_XFER;
        //- MOVE WS_TOT_PXFER TO MIL_TOT.
        client_tot.MIL_TOT = ws_client_totals.WS_TOT_PXFER;
        //- add WS_TOT_PXFER to WS_TOT_XFER GIVING END_BAL_TOT.
        client_tot.END_BAL_TOT = ws_client_totals.WS_TOT_PXFER + ws_client_totals.WS_TOT_XFER;
        //- MOVE "ALLOC   " TO TOT_FILLER.
        client_tot.TOT_FILLER = "ALLOC   ";
        //- WRITE P_REC FROM ClientTot AFTER 1.
        WRITE(client_tot);

        //- * Points line in totals
        //- MOVE ZERO TO BEG_BAL_TOT DIST1_TOT CONT_TOT
        //- MIL_TOT FORF_TOT EARN_TOT END_BAL_TOT EARN2_TOT.
        client_tot.BEG_BAL_TOT = 0m;
        client_tot.DIST1_TOT = 0m;
        client_tot.CONT_TOT = 0m;
        client_tot.MIL_TOT = 0m;
        client_tot.FORF_TOT = 0m;
        client_tot.EARN_TOT = 0m;
        client_tot.END_BAL_TOT = 0m;
        client_tot.EARN2_TOT = 0m;

        //- MOVE WS_PROF_PTS_TOTAL TO CONT_PTS_TOT.
        client_tot.CONT_TOT = ws_client_totals.WS_PROF_PTS_TOTAL; // Weird redefine CONT_TOT <-> CONT_PTS_TOT
                                                                  //- MOVE WS_EARN_PTS_TOTAL TO EARN_PTS_TOT.
        client_tot.EARN_TOT = ws_client_totals.WS_EARN_PTS_TOTAL;  // Weird redefine EARN_TOT <-> EARN_PTS_TOT
                                                                   //- move "POINTS " to tot_filler.
        client_tot.useRedefineFormatting = true;
        client_tot.TOT_FILLER = "POINT";
        //- WRITE P_REC FROM ClientTot AFTER 2.
        WRITE("");
        WRITE(client_tot);

        //- MOVE WS_EMPLOYEE_CTR TO PR_TOT_EMPLOYEE_COUNT.
        employee_count_tot.PR_TOT_EMPLOYEE_COUNT = ws_counters.WS_EMPLOYEE_CTR;
        //- WRITE P_REC FROM EMPLOYEE_COUNT_TOT AFTER 2.
        WRITE("");
        WRITE(employee_count_tot);
        //- MOVE WS_EMPLOYEE_CTR_payben TO PB_TOT_EMPLOYEE_COUNT.
        employee_count_tot_payben.PB_TOT_EMPLOYEE_COUNT = ws_counters.WS_EMPLOYEE_CTR_PAYBEN;
        //- WRITE P_REC FROM EMPLOYEE_COUNT_TOT_PAYBEN AFTER 2.
        WRITE("");
        WRITE(employee_count_tot_payben);

        //- INITIALIZE WS_COMPUTE_TOTALS WS_CLIENT_TOTALS.
        ws_compute_totals = new WS_COMPUTE_TOTALS();
        ws_client_totals = new WS_CLIENT_TOTALS();

        //- MOVE WS_TOT_OVER  TO RERUN_OVER.
        rerun_tot.RERUN_OVER = ws_maxcont_totals.WS_TOT_OVER;
        //- MOVE WS_TOT_POINTS TO RERUN_POINTS.
        rerun_tot.RERUN_POINTS = ws_maxcont_totals.WS_TOT_POINTS;
        //- MOVE WS_CONTR_MAX TO RERUN_MAX.
        rerun_tot.RERUN_MAX = WS_CONTR_MAX;

        //- WRITE P_REC FROM RERUN_TOT AFTER 10.
        outputLines.Add("\n\n\n\n\n\n\n\n\n");
        WRITE(rerun_tot);

        //- INITIALIZE WS_MAXCONT_TOTALS.
        ws_maxcont_totals = new WS_MAXCONT_TOTALS();
    //- 850_EXIT.
    l850_EXIT:;
        //- EXIT.
    }

    //- 855_GRAND_TOTALS.
    public void m855GrandTotals()
    {
        //- MOVE WS_EMPLOYEE_CTR TO PR_TOT_EMPLOYEE_COUNT.
        employee_count_tot.PR_TOT_EMPLOYEE_COUNT = ws_counters.WS_EMPLOYEE_CTR;
        //- MOVE WS_EMPLOYEE_CTR_payben TO PB_TOT_EMPLOYEE_COUNT.
        employee_count_tot_payben.PB_TOT_EMPLOYEE_COUNT = ws_counters.WS_EMPLOYEE_CTR_PAYBEN;
        //- MOVE WS_GRTOT_BEGBAL TO BEG_BAL_GRTOT.
        grand_tot.BEG_BAL_GRTOT = ws_grand_totals.WS_GRTOT_BEGBAL;
        //- MOVE WS_GRTOT_DIST1  TO DIST1_GRTOT.
        grand_tot.DIST1_GRTOT = ws_grand_totals.WS_GRTOT_DIST1;
        //- MOVE WS_GRTOT_MIL    TO MIL_GRTOT.
        grand_tot.MIL_GRTOT = ws_grand_totals.WS_GRTOT_MIL;
        //- MOVE WS_GRTOT_CONT   TO CONT_GRTOT.
        grand_tot.CONT_GRTOT = ws_grand_totals.WS_GRTOT_CONT;
        //- MOVE WS_GRTOT_FORF   TO FORF_GRTOT.
        grand_tot.FORF_GRTOT = ws_grand_totals.WS_GRTOT_FORF;
        //- MOVE WS_GRTOT_EARN   TO EARN_GRTOT.
        grand_tot.EARN_GRTOT = ws_grand_totals.WS_GRTOT_EARN;
        //- MOVE WS_GRTOT_EARN2  TO EARN2_GRTOT.
        grand_tot.EARN2_GRTOT = ws_grand_totals.WS_GRTOT_EARN2;
        //- IF WS_GRTOT_EARN2 NOT == 0
        if (ws_grand_totals.WS_GRTOT_EARN2 != 0)
        {
            //- DISPLAY "WS_GROTOT_EARN2 NOT 0 " WS_GRTOT_EARN2
            DISPLAY("WS_GROTOT_EARN2 NOT 0 " + ws_grand_totals.WS_GRTOT_EARN2);
        } //- END-IF.
          //- MOVE WS_GRTOT_CAF    TO EARN2_GRTOT.
        grand_tot.EARN2_GRTOT = ws_grand_totals.WS_GRTOT_CAF;
        //- MOVE WS_GRTOT_ENDBAL TO END_BAL_GRTOT.
        grand_tot.END_BAL_GRTOT = ws_grand_totals.WS_GRTOT_ENDBAL;
    //- 855_EXIT.
    l855_EXIT:;
    //- EXIT.

    //- 899_EXIT SECTION 60.
    //- 899_EXIT_SECTION.
    l899_EXIT_SECTION:;
        //- EXIT.
    }

    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //- *     PRINT ADJUSTMENTS REPORT
    //- * -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    //- 1000_PRINT_SECTION SECTION 70.
    //- 1000_ADJUSTMENT_REPORT.
    public void m1000AdjustmentReport()
    {
        //- IF PV_ADJUST_BADGE == 0 THEN
        if (point_values.PV_ADJUST_BADGE == 0)
        {
            //- GO TO 1000_EXIT.
            goto l1000_EXIT;
        }
        //- OPEN OUTPUT PRINT_FILE2.
        //- MOVE 1 TO HDR1_PAGE.
        header_1.HDR1_PAGE = 1;
        //- MOVE "PAY444A" TO HDR1_RPT.
        header_1.HDR1_RPT = "PAY444A";
        //- WRITE P_REC2 FROM HEADER_1 AFTER ADVANCING PAGE.
        //- WRITE P_REC2 FROM HEADER_4 AFTER ADVANCING 2 LINES.
        //- WRITE P_REC2 FROM HEADER_5 AFTER ADVANCING 2 LINES.
        //- MOVE PV_ADJUST_BADGE TO PL_ADJUST_BADGE.
        print_adj_line1.PL_ADJUST_BADGE = point_values.PV_ADJUST_BADGE;
        //- MOVE "INITIAL" TO PL_ADJ_DESC.
        print_adj_line1.PL_ADJ_DESC = "INITIAL";
        //- MOVE SV_CONT_AMT TO PL_CONT_AMT.
        print_adj_line1.PL_CONT_AMT = point_values.SV_CONT_AMT;
        //- MOVE SV_FORF_AMT TO PL_FORF_AMT.
        print_adj_line1.PL_FORF_AMT = point_values.SV_FORF_AMT;
        //- MOVE SV_EARN_AMT TO PL_EARN_AMT.
        print_adj_line1.PL_EARN_AMT = point_values.SV_EARN_AMT;
        //- MOVE SV_EARN2_AMT TO PL_EARN2_AMT.
        print_adj_line1.PL_EARN2_AMT = point_values.SV_EARN2_AMT;
        //- WRITE P_REC2 FROM PRINT_ADJ_LINE1 AFTER ADVANCING 2 LINES.
        //- *
        //- MOVE 0 TO PL_ADJUST_BADGE.
        print_adj_line1.PL_ADJUST_BADGE = 0;
        //- MOVE "ADJUSTMENT" TO PL_ADJ_DESC.
        print_adj_line1.PL_ADJ_DESC = "ADJUSTMENT";
        //- MOVE PV_ADJ_CONTRIB TO PL_CONT_AMT.
        print_adj_line1.PL_CONT_AMT = point_values.PV_ADJ_CONTRIB;
        //- MOVE PV_ADJ_EARN TO PL_EARN_AMT.
        print_adj_line1.PL_EARN_AMT = point_values.PV_ADJ_EARN;
        //- MOVE PV_ADJ_EARN2 TO PL_EARN2_AMT.
        print_adj_line1.PL_EARN2_AMT = point_values.PV_ADJ_EARN2;
        //- MOVE PV_ADJ_FORFEIT TO PL_FORF_AMT.
        print_adj_line1.PL_FORF_AMT = point_values.PV_ADJ_FORFEIT;
        //- WRITE P_REC2 FROM PRINT_ADJ_LINE1 AFTER ADVANCING 2 LINES.
        //- *
        //- MOVE "FINAL" TO PL_ADJ_DESC.
        print_adj_line1.PL_ADJ_DESC = "FINAL";
        //- MOVE SV_CONT_ADJUSTED TO PL_CONT_AMT.
        print_adj_line1.PL_CONT_AMT = point_values.SV_CONT_ADJUSTED;
        //- MOVE SV_FORF_ADJUSTED TO PL_FORF_AMT.
        print_adj_line1.PL_FORF_AMT = point_values.SV_FORF_ADJUSTED;
        //- MOVE SV_EARN_ADJUSTED TO PL_EARN_AMT.
        print_adj_line1.PL_EARN_AMT = point_values.SV_EARN_ADJUSTED;
        //- MOVE SV_EARN2_ADJUSTED TO PL_EARN2_AMT.
        print_adj_line1.PL_EARN2_AMT = point_values.SV_EARN2_ADJUSTED;
        //- WRITE P_REC2 FROM PRINT_ADJ_LINE1 AFTER ADVANCING 2 LINES.
        //- IF SV_FORF_AMT == 0 AND SV_EARN_AMT == 0 THEN
        if (point_values.SV_FORF_AMT == 0 && point_values.SV_EARN_AMT == 0)
        {
            //- WRITE P_REC2 FROM PRINT_ADJ_LINE2 AFTER ADVANCING 2 LINES.
        }
    //- CLOSE PRINT_FILE2.
    //- 1000_EXIT.
    l1000_EXIT:;
        //- EXIT.
    }
}
