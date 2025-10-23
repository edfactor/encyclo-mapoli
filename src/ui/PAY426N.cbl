 IDENTIFICATION DIVISION.
 PROGRAM-ID. PAY426N.
********************************************************************
**        PROGRAM:        PAY426N                                 **
**     WRITTEN BY:        R MAISON                                **
**   DATE WRITTEN:        01/03/2002                              **
**    DESCRIPTION:        THIS PROGRAM WILL GENERATE 6            **
**                        PROFIT SHARING REPORTS, OR ONLY         **
**                        ONE OF THOSE 5 REPORTS BASED            **
**                        UPON SWITCHES SET AT JOB SUBMISSION.    **
**         SWITCH 11 ON = REPORT 1:  ALL ACTIVE AND INACTIVE      **
**                                   EMPLOYEES AGE 18-20 WITH     **
**                                   >= 1000 PS HRS               **
**         SWITCH-12 ON = REPORT 2:  ALL ACTIVE AND INACTIVE      **
**                                   EMPLOYEES AGE >= 21 WITH     **
**                                   >= 1000 PS HRS               **
**         SWITCH 13 ON = REPORT 3:  ALL ACTIVE AND INACTIVE      **
**                                   EMPLOYESS AGE < 18           **
**         SWITCH 14 ON = REPORT 4:  ALL ACTIVE AND INACTIVE      **
**                                   EMPLOYEES AGE >= 18 WITH     **
**                                   < 1000 PS HOURS AND PRIOR    **
**                                   PS AMOUNT                    **
**         SWITCH-15 ON = REPORT 5:  ALL ACTIVE AND INACTIVE      **
**                                   EMPLOYEES AGE >= 18 WITH     **
**                                   < 1000 PS HOURS AND NO PRIOR **
**                                   PS AMOUNT                    **
**         SWITCH-16 ON = REPORT 6:  ALL TERMINATED EMPLOYEES     **
**                                   AGE >= 18 WITH >= 1000 PS HRS**
**         SWITCH-17 ON = REPORT 7:  ALL TERMINATED EMPLOYEES     **
**                                   AGE >= 18 WITH < 1000 PS HRS **
**                                   AND NO PRIOR PS AMOUNT       **
**         SWITCH-18 ON = REPORT 8:  ALL TERMINATED EMPLOYEES     **
**                                   AGE >= 18 WITH < 1000 PS HRS **
**                                   AND HAS A PRIOR PS AMOUNT    **
**         SWITCH-19 ON = REPORT 10: ALL Non-Employee Beneficiaries**
**                        REPORT 9:  THIS IS A SUMMARY PAGED OF   **
**                                   THE FIGURES FOR ALL 8 REPORTS**
**                                   ABOVE                        **
**                                   PAGE CONTAINS A SUMMARY OF   **
**                                   ALL 8 REPORTS.               **
**  (WHEN SWITCHES 11 THRU 18 ARE ON THERE WILL BE NO PAYROLL     **
**   MASTER UPDATES DONE (HOWEVER CALCULATIONS FOR POINTS AND     **
**   UPDATE VALUES WILL TAKE PLACE)                               **
**                                                                **
**  THE FOLLOWING FIELDS WILL BE UPDATED ON THE PAYROLL MASTER    **
**  UNLESS SWITCH-3 IS TURNED ON.  WHEN THIS IS THE CASE ALL 6    **
**  REPORTS ARE GENERATED BUT NO UPDATING IS DONE TO THE PAYROLL  **
**  MASTER.                                                       **
**                                                                **
** PY-PROF-CERT    - THIS FIELD IS SET TO A 1 IF THE              **
**                   PY-PROF-POINTS CALCULATED ARE > 0            **
** PY-PROF-NEWEMP  - THIS FIELD IS SET TO A 1 IT'S ORIGINAL VALUE **
**                   = 0 AND THE EMPLOYEE HAS NO PROFIT SHARE     **
**                   DETAILS AND THE EMPLOYEE IS > 20 YEARS OLD   **
**                   AS OF THE PROVIDED CUT OFF DATE.             **
** PY-PROF-POINTS  - ARE CALCULATED BY DIVIDING THE PY-WAGES BY   **
**                   100 AND ROUNDING UP BY 1 WHEN A REMAINDER OF **
**                   50 OR MORE IS CALCULATED.                    **
** PY-PROF-ZEROCONT- THIS FIELD WILL OBTAIN THE FOLLOWING VALUES  **
**                   1 = AGE < 21                                 **
**                   2 = TERMINATED EMPLOYEE WITH PROFIT SHARE    **
**                       HOURS > 0                                **
**                   3 = EMPLOYEES OVER 64 YEARS OF AGE AND       **
**                       PROFIT SHARE HOURS ARE > 0 AND           **
**                       THE OVER-65-SWITCH IS = 0 AND            **
**                       THE EMPLOYEE HAS MORE THAN 1 PROFIT SHARE**
**                       YEARS.
**                   4 = EMPLOYEES OVER 64 YEARS OF AGE AND PROFIT**
**                        SHARE HOURS > 0 AND THE OVER-65-SWITCH  **
**                        = 0 AND THE PY-PS-YEARS = 1             **
**                   5 = EMPLOYEES OVER 64 WITH OVER-65-SWITCH = 1**
**                       AND PY-PS-YEARS = 0                      **
**                   6 = EMPLOYEES OVER 64 AND MORE THAN 5 YEARS  **
**                       SINCE THEIR INITIAL PROFIT SHARING CONT  **
**                       AND ARE ACTIVE EMPLOYEES                 **
**                                                                **
** AGES AND YEARS OF SERVICE ARE CALCULATED AS OF THE USER        **
** SUPPLIED CUT OFF DATE, THE BEGINING OF THE CURRENT YEAR IS     **
** CALCULATED BY SUBTRACTING 1 FROM THE YEAR (WHEN THE CUTOFF     **
** MONTH IS DECEMBER AND ADDING 2 TO THE DAY (WHEN THE CUTOFF DAY **
** IS < 30) OR WHEN THE CUTOFF MONTH IS JANUARY AND THE CUTOFF DAY**
** IS < 3 1 IS SUBTRACTED FROM THE CUTOFF YEAR AND THE MONTH AND  **
** DAY ARE SET TO 12/31 RESPECTIVELY.  IF THE CUTOFF MONTH IS 1   **
** AND THE CUTOFF DAY IS > 2, 2 IS SUBTRACTED FROM THE DAY AND THE**
** YEAR IS LEFT ALONE. THIS BEGINING DATE IS USED TO BYPASS EMP-  **
** LOYEES TERMINATED PRIOR TO THE BEGINING OF THE CURRENT YEAR    **
********************************************************************
**            M O D I F I C A T I O N   H I S T O R Y             **
**            ---------------------------------------             **
**   DATE   NAME:        PROJ # DESCRIPTION                       **
** MM/DD/YY ____________ NNNNNN _________________________________ **
**                                                                **
** 11/06/02 DPRUGH     144600  ADDED NEW PROFIT SHARING CODE TO   **
**                             HANDLE >65 AND >5 YRS SINCE INIT   **
**                             CONTRIBUTION OR TERM = Z.          **
**                                                                **
** 01/11/05 R MAISON #9083     WHEN RUN WITH SWITCH 3 TOTALS AND  **
**                             EMP COUNTS DID NOT MATCH UP WITH   **
**                             END OF YEAR RUN - HAD TO ADD A     **
**                             CALC TO CREATE LY-PY-PS-AMT AND    **
**                             ADDED CODE TO INSURE THAT WHEN SW 2**
**                             IS ON LAST YEAR FIGURES ARE MOVED  **
**                             AND ACCUMULATED APPROPRIATELY      **
**                                                                **
** 12/21/06  DPRUGH    #11887  ADD SUMMARY OF EMPLOYEES WITH NO   **
**                             WAGES.  # OF EMPS AND BALANCE.     **
**                                                                **
** 01/04/07  DPRUGH    #11939  ADD TOTAL OF UNDER 18 EMPS WITH NO **
**                             WAGES UNDER THE X-<UNDER 18 LINE.  **
**                                                                **
**                                                                **
** 03/25/08  DPrugh  P#13152   Made changes to mask the SSN with  **
**                             zeroes leaving only the last 4     **
**                             digits. a SSN that was 123-45-6789 **
**                             will now show as 000-00-6789.      **
**                                                                **
** 04/28/10  DPrugh  P#15226   Added Report of all non-employee   **
**                             Beneficiaries from the PAYBEN      **
**                             system. These will have Balances   **
**                             for profit sharing, buy no hours   **
**                             or wages. They will accrue ONLY    **
**                             earnings points and never have any **
**                             forfeiture or contribution points. **
**                                                                **
** 01/06/10 Ed Stevenson  #16395 Increased HD-VERSION to two      **
**                               digits. Field was being truncated**  
**                                                                **
** Note : if these non-employees become employees, they will come **
**        be reported correctly in the store reports and not show **
**        up on the non-employee report.                          **
**                                                                **
** 3/8/2016 N.Ferrin      #19651  Use 50 for century cutoff       **
**                                                                **
** 1/3//2018 N.Ferrin     MAIN-565  Remove the < $1 logic for     **
**                        ly-py-ps-amt                            **
**                                                                **
** 03/09/21 DSawyer      MAIN-1286  Replaced PAYR with PAYPROFIT  **
** 12/28/21 DSawyer      MAIN-1497  Replaced DEMOGRAPHICS with    **
**                       DEMO_PROFSHARE for yr end profit sharing **
** 01/06/2022 DSawyer    MAIN-1488 Added new parameter YEAREND.   **
**                       If "Y", read DEMO_PROFSHARE table.  If   **
**                       "N" read DEMOGRAPHICS table.  This program*
**                       can be run several times before year end.** 
**                       DEMO_PROFSHARE is not updated until year ** 
**                       end.                                     **
** 10/23/2023 N.Ferrin   MAIN-1956 add executive hours and dollars**
** 8/13/2024  N.Ferrin   MAIN-2090 remove restriction for report  **
**                       4 when printing detail lines             **
********************************************************************

 ENVIRONMENT DIVISION.
 CONFIGURATION SECTION.
  SOURCE-COMPUTER. UNIX-MF.
  OBJECT-COMPUTER. UNIX-MF.

 INPUT-OUTPUT SECTION.
 FILE-CONTROL.
 SELECT SORT-OUT   ASSIGN "H".
 SELECT PRINTFL1   ASSIGN LINE ADVANCING "PR1"
        ORGANIZATION LINE SEQUENTIAL.
 SELECT PRINTFL2   ASSIGN LINE ADVANCING "PR2"
        ORGANIZATION LINE SEQUENTIAL.
 SELECT PRINTFL3   ASSIGN LINE ADVANCING "PR3"
        ORGANIZATION LINE SEQUENTIAL.
 SELECT PRINTFL4   ASSIGN LINE ADVANCING "PR4"
        ORGANIZATION LINE SEQUENTIAL.
 SELECT PRINTFL5   ASSIGN LINE ADVANCING "PR5"
        ORGANIZATION LINE SEQUENTIAL.
 SELECT PRINTFL6   ASSIGN LINE ADVANCING "PR6"
        ORGANIZATION LINE SEQUENTIAL.
 SELECT PRINTFL7   ASSIGN LINE ADVANCING "PR7"
        ORGANIZATION LINE SEQUENTIAL.
 SELECT PRINTFL8   ASSIGN LINE ADVANCING "PR8"
        ORGANIZATION LINE SEQUENTIAL.
 SELECT PRINTFL9   ASSIGN LINE ADVANCING "PR9"
        ORGANIZATION LINE SEQUENTIAL.
 SELECT PRINTFL10   ASSIGN LINE ADVANCING "PR10"
        ORGANIZATION LINE SEQUENTIAL.

 DATA DIVISION.
 FILE SECTION.

 SD  SORT-OUT.
 01  SORTREC.
     03  S-REPORT-CODE                     PIC 9(02).
     03  S-NAME                            PIC X(25).
     03  S-EMP-NUMBER                      PIC 9(07).
     03  S-STORE                           PIC 9(03).
     03  S-TYPE                            PIC X(01).
     03  S-EMP                             PIC X(01).
     03  S-BIRTH-DATE                      PIC 9(08).
     03  DUMMY REDEFINES S-BIRTH-DATE.
         05  S-BIRTH-CCYY                  PIC 9(04).
         05  DUMMY REDEFINES S-BIRTH-CCYY.
             07  S-BIRTH-CC                PIC 9(02).
             07  S-BIRTH-YY                PIC 9(02).
         05  S-BIRTH-MM                    PIC 9(02).
         05  S-BIRTH-DD                    PIC 9(02).
     03  S-AGE                             PIC 9(02).
     03  S-SSN                             PIC 9(09).
     03  S-SSNX REDEFINES S-SSN.
         05  S-SSN-3                       PIC 9(03).
         05  S-SSN-2                       PIC 9(02).
         05  S-SSN-4                       PIC 9(04).
     03  S-WAGES                           PIC S9(06)V99.
     03  S-WAGESX REDEFINES S-WAGES.
         05  S-WDOLLARS                     PIC S9(06).
         05  S-WCENTS                       PIC V9(02).
     03  S-HRS                             PIC 9(04)V99.
     03  S-NEW-EMP                         PIC 9(02).
     03  S-TERM-DATE                       PIC 9(08).
     03  S-TERM-DATEX REDEFINES S-TERM-DATE.
         05  S-TERM-CCYY                   PIC 9(04).
         05  DUMMY REDEFINES S-TERM-CCYY.
             07  S-TERM-CC                 PIC 9(02).
             07  S-TERM-YY                 PIC 9(02).
         05  S-TERM-MM                     PIC 9(02).
         05  S-TERM-DD                     PIC 9(02).
     03  S-CURR-BALANCE                    PIC S9(07)V99.
     03  S-WK-WEEKS                        PIC 9(04).
     03  S-HIRE-DATE                       PIC 9(08).
     03  S-HIRE-DATEX REDEFINES S-HIRE-DATE.
         05  S-HIRE-DATE-CCYY              PIC 9(04).
         05  DUMMY REDEFINES S-HIRE-DATE-CCYY.
             07  S-HIRE-DATE-CC            PIC 9(02).
             07  S-HIRE-DATE-YY            PIC 9(02).
         05  S-HIRE-DATE-MM                PIC 9(02).
         05  S-HIRE-DATE-DD                PIC 9(02).
     03  S-YEARS-OF-SERVICE                PIC 9(02).
     03  S-OVER-65-SW                      PIC 9(01).
     03  S-VESTED-SW                       PIC 9(01).
     03  S-VEST-PRCT                       PIC 9(03).
     03  S-TERM-VESTED                     PIC 9(01).
     03  S-SCOD                            PIC X(01).
     03  S-ELIGIBILITY-SW                  PIC 9(01).
     03  S-PYRS                            PIC 9(02).
     03  S-65PLUS-AND-5PLUS                PIC 9(01).
     03  S-UNUSED-FILLER                   PIC X(10).

 FD  PRINTFL1 LABEL RECORDS OMITTED.
 01  PR1-REC                               PIC X(136).
 FD  PRINTFL2 LABEL RECORDS OMITTED.
 01  PR2-REC                               PIC X(136).
 FD  PRINTFL3 LABEL RECORDS OMITTED.
 01  PR3-REC                               PIC X(136).
 FD  PRINTFL4 LABEL RECORDS OMITTED.
 01  PR4-REC                               PIC X(136).
 FD  PRINTFL5 LABEL RECORDS OMITTED.
 01  PR5-REC                               PIC X(136).
 FD  PRINTFL6 LABEL RECORDS OMITTED.
 01  PR6-REC                               PIC X(136).
 FD  PRINTFL7 LABEL RECORDS OMITTED.
 01  PR7-REC                               PIC X(136).
 FD  PRINTFL8 LABEL RECORDS OMITTED.
 01  PR8-REC                               PIC X(136).
 FD  PRINTFL9 LABEL RECORDS OMITTED.
 01  PR9-REC                               PIC X(136).
 FD  PRINTFL10 LABEL RECORDS OMITTED.
 01  PR10-REC                               PIC X(136).

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 WORKING-STORAGE SECTION.
  COPY METADATA.
  COPY UWA-REC-SOC-SEC-REC.
  COPY UWA-REC-PROFIT-DETAIL.
  COPY UWA-REC-PROFIT-SS-DETAIL.
  COPY SALES-MSTR-RETAINING-FLAGS.
  COPY DB-REGISTERS.
  COPY IDSII-TECH-FIELDS.

 01 UFAS-ALT-KEY-NAME PIC X(20).

 01 UFAS-OPEN-MODE PIC X(6).
 COPY X-FD-DEMOGRAPHICS.
 COPY X-FD-PAYPROFIT.
 COPY X-FD-PAYBEN.
 COPY X-FD-CALDAR.

 01 DEMOGRAPHICS-FILE-STATUS PIC XX.
 01 PAYPROFIT-FILE-STATUS    PIC XX.
 01 PAYBEN-FILE-STATUS       PIC XX.
 01 CALDAR-FILE-STATUS       PIC XX.

 COPY WS-QUE-DISP.

 01  WS-SKIPPED                             PIC 9(8) VALUE 0.
 01  LY-PY-PS-AMT                           PIC S9(07)V99 VALUE 0.
 01  PG1-CNT                                PIC 9(05) VALUE 0.
 01  MNTHLY-CNT                             PIC 9(06) VALUE 0.
 01  MWAGES.
     02  MNTHLY-WAGES                       PIC S9(09)V99 VALUE ZERO.
 01  ws-Payben-count                        PIC 9(08) VALUE 0.
 01  PG2-CNT                                PIC 9(05) VALUE 0.
 01  PG3-CNT                                PIC 9(05) VALUE 0.
 01  PG4-CNT                                PIC 9(05) VALUE 0.
 01  PG5-CNT                                PIC 9(05) VALUE 0.
 01  PG6-CNT                                PIC 9(05) VALUE 0.
 01  PG7-CNT                                PIC 9(05) VALUE 0.
 01  PG8-CNT                                PIC 9(05) VALUE 0.
 01  PG9-CNT                                PIC 9(05) VALUE 0.
 01  PG10-CNT                               PIC 9(05) VALUE 0.
 01  EPG-CNT                                PIC 9(05) VALUE 0.
 01  LN1-CNT                                PIC 9(02) VALUE 57.
 01  LN2-CNT                                PIC 9(02) VALUE 57.
 01  LN3-CNT                                PIC 9(02) VALUE 57.
 01  LN4-CNT                                PIC 9(02) VALUE 57.
 01  LN5-CNT                                PIC 9(02) VALUE 57.
 01  LN6-CNT                                PIC 9(02) VALUE 57.
 01  LN7-CNT                                PIC 9(02) VALUE 57.
 01  LN8-CNT                                PIC 9(02) VALUE 57.
 01  LN9-CNT                                PIC 9(02) VALUE 57.
 01  LN10-CNT                               PIC 9(02) VALUE 57.
 01  ELN-CNT                                PIC 9(02) VALUE 57.
 01  WS-DATE-CURRENT.
     03  WS-CURR-CCYY                       PIC 9(04).
     03  WS-YR-BRKDWN REDEFINES WS-CURR-CCYY.
         05  WS-CURR-CC                     PIC 9(02).
         05  WS-CURR-YY                     PIC 9(02).
     03  WS-CURR-MM                         PIC 9(02).
     03  WS-CURR-DD                         PIC 9(02).
     03  WS-CURR-HR                         PIC 9(02).
     03  WS-CURR-MN                         PIC 9(02).
     03  WS-CURR-SEC                        PIC 9(02).
     03  WS-CURR-HSEC                       PIC 9(02).
     03  FILLER                             PIC X(05).
 01  DUMMY REDEFINES WS-DATE-CURRENT.
     03  WS-CURR-CYMD                       PIC 9(08).
     03  FILLER                             PIC X(13).
 01  WS-START-DATE                          PIC 9(08).
 01  DUMMY REDEFINES WS-START-DATE.
     03  WS-START-CCYY                      PIC 9(04).
     03  WS-START-MM                        PIC 9(02).
     03  WS-START-DD                        PIC 9(02).
 01  WS-END-DATE                            PIC 9(08).
 01  DUMMY REDEFINES WS-END-DATE.
     03  WS-END-CCYY                        PIC 9(04).
     03  WS-END-MM                          PIC 9(02).
     03  WS-END-DD                          PIC 9(02).
 01  CALC-YEARS-X.
     03  CALC-YEARS                         PIC 9(04).
     03  DUMMY REDEFINES CALC-YEARS.
         05  CALC-CC                        PIC 9(02).
         05  CALC-YY                        PIC 9(02).
     03  CALC-MONTHS                        PIC 9(02).
     03  CALC-DAYS                          PIC 9(02).
 01  LEAP-YEAR                              PIC 9(04)V9(02) VALUE 0.
 01  LEAP-YEARX REDEFINES LEAP-YEAR.
     03  LEAP-YY                            PIC 9(04).
     03  LEAP-REM                           PIC 9(02).
 01  NEW-PS-EMPCNT                          PIC 9(06) VALUE 0.
 01  ERR-CNT                                PIC 9(06) VALUE 0.
 01  TOT-ERR-WAGES                          PIC S9(09)V99 VALUE 0.
 01  SORT-CNT                               PIC 9(06) VALUE 0.
 01  UPDATED-CNT                            PIC 9(06) VALUE 0.
 01  RPT-TOTALS.
     03  RPT1-CNT                           PIC 9(06) VALUE 0.
     03  RPT1-WAGES                         PIC S9(09)V99 VALUE 0.
     03  RPT1-BAL                           PIC S9(09)V99 VALUE 0.
     03  RPT1-POINTS                        PIC S9(09)    VALUE 0.
     03  RPT1-CNT-NWAGE                     PIC 9(06) VALUE 0.
     03  RPT1-BAL-NWAGE                     PIC S9(09)V99 VALUE 0.
     03  RPT1-WAGES-NWAGE                   PIC S9(09)V99 VALUE 0.
     03  RPT2-CNT                           PIC 9(06) VALUE 0.
     03  RPT2-WAGES                         PIC S9(09)V99 VALUE 0.
     03  RPT2-BAL                           PIC S9(09)V99 VALUE 0.
     03  RPT2-POINTS                        PIC S9(09)    VALUE 0.
     03  RPT2-CNT-NWAGE                     PIC 9(06) VALUE 0.
     03  RPT2-BAL-NWAGE                     PIC S9(09)V99 VALUE 0.
     03  RPT2-WAGES-NWAGE                   PIC S9(09)V99 VALUE 0.
     03  RPT3-CNT                           PIC 9(06) VALUE 0.
     03  RPT3-WAGES                         PIC S9(09)V99 VALUE 0.
     03  RPT3-BAL                           PIC S9(09)V99 VALUE 0.
     03  RPT3-POINTS                        PIC S9(09) VALUE 0.
     03  RPT3-CNT-NWAGE                     PIC 9(06) VALUE 0.
     03  RPT3-BAL-NWAGE                     PIC S9(09)V99 VALUE 0.
     03  RPT3-WAGES-NWAGE                   PIC S9(09)V99 VALUE 0.
     03  RPT4-CNT                           PIC 9(06) VALUE 0.
     03  RPT4-WAGES                         PIC S9(09)V99 VALUE 0.
     03  RPT4-BAL                           PIC S9(09)V99 VALUE 0.
     03  RPT4-POINTS                        PIC S9(09) VALUE 0.
     03  RPT4-CNT-NWAGE                     PIC 9(06) VALUE 0.
     03  RPT4-BAL-NWAGE                     PIC S9(09)V99 VALUE 0.
     03  RPT4-WAGES-NWAGE                   PIC S9(09)V99 VALUE 0.
     03  RPT5-CNT                           PIC 9(06) VALUE 0.
     03  RPT5-WAGES                         PIC S9(09)V99 VALUE 0.
     03  RPT5-BAL                           PIC S9(09)V99 VALUE 0.
     03  RPT5-POINTS                        PIC S9(09) VALUE 0.
     03  RPT5-CNT-NWAGE                     PIC 9(06) VALUE 0.
     03  RPT5-BAL-NWAGE                     PIC S9(09)V99 VALUE 0.
     03  RPT5-WAGES-NWAGE                   PIC S9(09)V99 VALUE 0.
     03  RPT6-CNT                           PIC 9(06) VALUE 0.
     03  RPT6-WAGES                         PIC S9(09)V99 VALUE 0.
     03  RPT6-BAL                           PIC S9(09)V99 VALUE 0.
     03  RPT6-POINTS                        PIC S9(09)V99 VALUE 0.
     03  RPT6-CNT-NWAGE                     PIC 9(06) VALUE 0.
     03  RPT6-BAL-NWAGE                     PIC S9(09)V99 VALUE 0.
     03  RPT6-WAGES-NWAGE                   PIC S9(09)V99 VALUE 0.
     03  RPT7-CNT                           PIC 9(06) VALUE 0.
     03  RPT7-WAGES                         PIC S9(09)V99 VALUE 0.
     03  RPT7-BAL                           PIC S9(09)V99 VALUE 0.
     03  RPT7-POINTS                        PIC S9(09)V99 VALUE 0.
     03  RPT7-CNT-NWAGE                     PIC 9(06) VALUE 0.
     03  RPT7-BAL-NWAGE                     PIC S9(09)V99 VALUE 0.
     03  RPT7-WAGES-NWAGE                   PIC S9(09)V99 VALUE 0.
     03  RPT8-CNT                           PIC 9(06) VALUE 0.
     03  RPT8-WAGES                         PIC S9(09)V99 VALUE 0.
     03  RPT8-BAL                           PIC S9(09)V99 VALUE 0.
     03  RPT8-POINTS                        PIC S9(09)V99 VALUE 0.
     03  RPT8-CNT-NWAGE                     PIC 9(06) VALUE 0.
     03  RPT8-BAL-NWAGE                     PIC S9(09)V99 VALUE 0.
     03  RPT8-WAGES-NWAGE                   PIC S9(09)V99 VALUE 0.
     03  RPT1-NEW-CNT                       PIC 9(06) VALUE 0.
     03  RPT2-NEW-CNT                       PIC 9(06) VALUE 0.
     03  RPT3-NEW-CNT                       PIC 9(06) VALUE 0.
     03  RPT4-NEW-CNT                       PIC 9(06) VALUE 0.
     03  RPT5-NEW-CNT                       PIC 9(06) VALUE 0.
     03  RPT6-NEW-CNT                       PIC 9(06) VALUE 0.
     03  RPT7-NEW-CNT                       PIC 9(06) VALUE 0.
     03  RPT8-NEW-CNT                       PIC 9(06) VALUE 0.
     03  RPT10-NEW-CNT                      PIC 9(06) VALUE 0.
     03  RPT1-UNDR21                       PIC 9(06) VALUE 0.
     03  RPT1-UNDR21-WAGES                 PIC S9(09)V99 VALUE 0.
     03  RPT1-UNDR21-BAL                   PIC S9(09)V99 VALUE 0.
     03  RPT2-UNDR21                       PIC 9(06) VALUE 0.
     03  RPT2-UNDR21-WAGES                 PIC S9(09)V99 VALUE 0.
     03  RPT2-UNDR21-BAL                   PIC S9(09)V99 VALUE 0.
     03  RPT3-UNDR21                       PIC 9(06) VALUE 0.
     03  RPT3-UNDR21-WAGES                 PIC S9(09)V99 VALUE 0.
     03  RPT3-UNDR21-BAL                   PIC S9(09)V99 VALUE 0.
     03  RPT4-UNDR21                       PIC 9(06) VALUE 0.
     03  RPT4-UNDR21-WAGES                 PIC S9(09)V99 VALUE 0.
     03  RPT4-UNDR21-BAL                   PIC S9(09)V99 VALUE 0.
     03  RPT5-UNDR21                       PIC 9(06) VALUE 0.
     03  RPT5-UNDR21-WAGES                 PIC S9(09)V99 VALUE 0.
     03  RPT5-UNDR21-BAL                   PIC S9(09)V99 VALUE 0.
     03  RPT6-UNDR21                       PIC 9(06) VALUE 0.
     03  RPT6-UNDR21-WAGES                 PIC S9(09)V99 VALUE 0.
     03  RPT6-UNDR21-BAL                   PIC S9(09)V99 VALUE 0.
     03  RPT7-UNDR21                       PIC 9(06) VALUE 0.
     03  RPT7-UNDR21-WAGES                 PIC S9(09)V99 VALUE 0.
     03  RPT7-UNDR21-BAL                   PIC S9(09)V99 VALUE 0.
     03  RPT8-UNDR21                       PIC 9(06) VALUE 0.
     03  RPT8-UNDR21-WAGES                 PIC S9(09)V99 VALUE 0.
     03  RPT8-UNDR21-BAL                   PIC S9(09)V99 VALUE 0.
     03  RPT9-CNT-NWAGE                    PIC 9(06) VALUE 0.
     03  RPT9-BAL-NWAGE                    PIC S9(09)V99 VALUE 0.
     03  RPT9-WAGES-NWAGE                  PIC S9(09)V99 VALUE 0.
     03  RPT10-WAGES                       PIC S9(09)V99 VALUE 0.
     03  RPT10-BAL                         PIC S9(09)V99 VALUE 0.
     03  RPT10-POINTS                      PIC S9(09)V99 VALUE 0.
     03  RPT10-CNT-NWAGE                   PIC 9(06) VALUE 0.
     03  RPT10-BAL-NWAGE                   PIC S9(09)V99 VALUE 0.
     03  RPT10-WAGES-NWAGE                 PIC S9(09)V99 VALUE 0.
     03  RPT10-CNT                         PIC 9(06) VALUE 0.
     03  UNDR18-CNT                        PIC 9(06) VALUE 0.
     03  UNDR18-WAGES                      PIC S9(09)V99 VALUE 0.
     03  UNDR18-BAL                        PIC S9(09)V99 VALUE 0.
     03  UNDR18-CNT-NWAGE                  PIC 9(06) VALUE 0.
 01  GRAND-TOTALS.
     03  RPT-TOT                                PIC 9(09) VALUE 0.
     03  RPT-WAGES                              PIC S9(12)V99 VALUE 0.
     03  RPT-BAL                                PIC S9(12)V99 VALUE 0.
     03  TOT-POINTS                             PIC S9(12)    VALUE 0.
 01  WS-CALCULATE-POINTS.
     03  WS-POINTS                          PIC S9(05) VALUE 0.
     03  WS-REMAINDER                       PIC S9(05) VALUE 0.
     03  WS-TOTAL-POINTS                    PIC S9(07) VALUE 0.
 01  WS-INDICATORS.
     03  WS-START-IND                       PIC 9(01) VALUE 0.
     03  WS-PAYPROFIT-START-SW              PIC 9(01) VALUE 0.
         88  INVALID-START-PAYPROFIT                  VALUE 1.
     03  WS-PAYPROFIT-READ-SW               PIC 9(01) VALUE 0.
         88  INVALID-PAYPROFIT-READ                   VALUE 1.
     03  WS-PAYPROFIT-READ-NEXT-SW          PIC 9(01) VALUE 0.
         88  PYPROFIT-EOF                             VALUE 1.
     03  WS-PAYPROFIT-REWRITE-SW            PIC 9(01) VALUE 0.
         88  INVALID-PYPROFIT-REWRITE                 VALUE 1.
     03  WS-PAYBEN-START-SW                PIC 9(01) VALUE 0.
         88  INVALID-START-PAYBEN                    VALUE 1.
     03  WS-PAYBEN-READ-SW                 PIC 9(01) VALUE 0.
         88  INVALID-PYBEN-READ                      VALUE 1.
     03  WS-PAYBEN-READ-NEXT-SW            PIC 9(01) VALUE 0.
         88  PYBEN-EOF                               VALUE 1.
     03  WS-SUMMARY-INDICATOR               PIC 9(01) VALUE 0.
         88  TIME-FOR-SUMMARY                         VALUE 1.
         88  TIME-FOR-TOTALS                          VALUE 2.
     03  HOLD-REPORT-CODE                   PIC 9(02) VALUE 0.
         88  REPORT-1                                 VALUE 1.
         88  REPORT-2                                 VALUE 2.
         88  REPORT-3                                 VALUE 3.
         88  REPORT-4                                 VALUE 4.
         88  REPORT-5                                 VALUE 5.
         88  REPORT-6                                 VALUE 6.
         88  REPORT-7                                 VALUE 7.
         88  REPORT-8                                 VALUE 8.
         88  REPORT-9                                 VALUE 9.
         88  REPORT-10                                VALUE 10.
     03  WS-ABORT-INDICATOR                 PIC 9(01) VALUE 0.
         88 PAY426N-ABORTS                            VALUE 1.
 01  SUPERVISOR-DATEX.
     03  SCC                                PIC 9(02) VALUE 0.
     03  SUPERVISOR-DATE.
         05  SYY                            PIC 9(02) VALUE 0.
         05  SMM                            PIC 9(02) VALUE 12.
         05  SDD                            PIC 9(02) VALUE 31.
     03  SUPERVISOR-DATEN REDEFINES SUPERVISOR-DATE
                                            PIC 9(06).
 01  SUPER-CYMD REDEFINES SUPERVISOR-DATEX  PIC 9(08).
 01  SUPER-CYMDX REDEFINES SUPERVISOR-DATEX.
     03  SUPER-CCYY                         PIC 9(04).
     03  SUPER-MMDD                         PIC 9(04).
 01  WORK-DATEX.
     03  WCC                                PIC 9(02).
     03  WORK-DATE.
         05  WYY                            PIC 9(02) VALUE 0.
         05  WMM                            PIC 9(02) VALUE 0.
         05  WDD                            PIC 9(02) VALUE 0.
     03  WORK-DATEN REDEFINES WORK-DATE     PIC 9(06).
 01  WORK-CYMD REDEFINES WORK-DATEX         PIC 9(08).
 01  WORK-CYMDX REDEFINES WORK-DATEX.
     03  WCCYY                              PIC 9(04).
     03  WMMDD                              PIC 9(04).
 01  WORK-DATE2X.
     03  WK2-CC                             PIC 9(02).
     03  WORK-DATE2.
         05  WK-DATE2-YY                    PIC 9(02).
         05  WK-DATE2-MM                    PIC 9(02).
         05  WK-DATE2-DD                    PIC 9(02).
     03 WORK-DATE2N REDEFINES WORK-DATE2    PIC 9(06).
 01  WK-DATE2-CYMD REDEFINES WORK-DATE2X    PIC 9(08).
 01  WK-DATE2-CYMDX REDEFINES WORK-DATE2X.
     03  WK-DATE2-CCYY                      PIC 9(04).
     03  WK-DATE2-MMDD                      PIC 9(04).
 01  YEARS                                  PIC 9(04).
 01  DUMMY REDEFINES YEARS.
     03  YEARS-1                            PIC 9(02).
     03  YEARS-2                            PIC 9(02).
 01  WS-HOLD-EMP-HOURS                      PIC 9(06)V99 VALUE 0.
 01  WS-undr18-wages                        PIC 9(06)V99 VALUE 0.
 01  WS-LINE-COUNT                          PIC 9(02) VALUE 0.
 01  EMP-ELIGIBILITY-CHECK                  PIC 9(01) VALUE 0.
 01  FIRST-READ-SWITCH                      PIC 9(01) VALUE 0.
     88  FIRST-READ                                   VALUE 0.
 01  WS-MOVE-DATA                           PIC 9(01) VALUE 0.
     88  DATA-MOVED-FOR-3                             VALUE 3.
     88  DATA-MOVED-FOR50                             VALUE 5.
 01  WS-1-FULL                              PIC 9(01) VALUE 0.
 01  WS-2-FULL                              PIC 9(01) VALUE 0.
 01  WS-3-FULL                              PIC 9(01) VALUE 0.
 01  WS-50-FULL                             PIC 9(01) VALUE 0.
 01  WS-NEXT-ONE                            PIC 9(01) VALUE 0.
 01  CLIENT-WAGES                           PIC S9(09)V99 VALUE 0.
 01  WS-SECT-WAGES                          PIC S9(09)V99 VALUE 0.
 01  WS-HOURS                               PIC 9(04) VALUE 0.
 01  WS-TERM-ALL-CLIENTS                    PIC 9(01) VALUE 0.
 01  WS-TERM                                PIC X VALUE SPACES.
 01  WS-PREV-WRK-REC                        PIC X(90) VALUE SPACES.
 01  WS-RECORD.
     03  WS-REPORT-CODE                     PIC 9(02) VALUE 0.
     03  WS-NAME                            PIC X(25) VALUE SPACES.
     03  WS-EMP-NUMBER                      PIC 9(07) VALUE 0.
     03  WS-STORE                           PIC 9(03) VALUE 0.
     03  WS-TYPE                            PIC X(01) VALUE SPACES.
     03  WS-EMP                             PIC X(01) VALUE SPACES.
     03  WS-BIRTH-DATE                      PIC 9(08) VALUE 0.
     03  DUMMY REDEFINES WS-BIRTH-DATE.
         05  WS-BIRTH-CCYY                  PIC 9(04).
         05  WS-BIRTH-MM                    PIC 9(02).
         05  WS-BIRTH-DD                    PIC 9(02).
     03  WS-AGE                             PIC 9(02) VALUE 0.
     03  WS-SSN                             PIC 9(09) VALUE 0.
     03  WS-SSNX REDEFINES WS-SSN.
         05  WS-SSN-3                       PIC 9(03).
         05  WS-SSN-2                       PIC 9(02).
         05  WS-SSN-4                       PIC 9(04).
     03  WS-WAGES                           PIC S9(06)V99 VALUE 0.
     03  WS-HRS                             PIC 9(04)V99 VALUE 0.
     03  WS-NEW-EMP                         PIC 9(02) VALUE 0.
     03  WS-TERM-DATE                       PIC 9(08) VALUE 0.
     03  WS-TERM-DATEX REDEFINES WS-TERM-DATE.
         05  WS-TERM-CCYY                   PIC 9(04).
         05  WS-TERM-MM                     PIC 9(02).
         05  WS-TERM-DD                     PIC 9(02).
     03  WS-CURR-BALANCE                    PIC S9(07)V99 VALUE 0.
     03  WS-WK-WEEKS                        PIC 9(04) VALUE 0.
     03  WS-HIRE-DATE                       PIC 9(08) VALUE 0.
     03  WS-HIRE-DATEX REDEFINES WS-HIRE-DATE.
         05  WS-HIRE-DATE-CCYY              PIC 9(04).
         05  WS-HIRE-DATE-MM                PIC 9(02).
         05  WS-HIRE-DATE-DD                PIC 9(02).
     03  WS-PS-YEARS                PIC 9(02) VALUE 0.
     03  WS-OVER-64-SW                      PIC 9(01) VALUE 0.
     03  WS-VESTED-SW                       PIC 9(01) VALUE 0.
     03  WS-VEST-PRCT                       PIC 9(03) VALUE 0.
     03  WS-TERM-VESTED                     PIC 9(01) VALUE 0.
     03  WS-SCOD                            PIC X(01) VALUE SPACES.
     03  WS-ELIGIBILITY-SW                  PIC 9(01) VALUE 0.
     03  WS-PYRS                            PIC 9(02) VALUE 0.
     03  WS-65PLUS-AND-5PLUS                PIC 9(01) VALUE ZEROS.
     03  WS-UNUSED-FILLER                   PIC X(10) VALUE SPACES.
 01  PROFIT-SHARE-SWITCH                    PIC 9(01) VALUE 0.
     88  NO-PROFIT-SHARE    VALUE 0.
     88  HAS-PROFIT-SHARE   VALUE 1.
 01  CUTOFFDATE.
     03  CUTOFFDATE-CC                      PIC 9(02) VALUE 0.
     03  IN-DATE-N                          PIC 9(06) VALUE 0.
     03  IN-DATE-X REDEFINES IN-DATE-N.
         05  CUTOFFDATE-YY                  PIC 9(02).
         05  CUTOFFDATE-MM                  PIC 9(02).
         05  CUTOFFDATE-DD                  PIC 9(02).
 01  CUTOFFDATE-CYMDX REDEFINES CUTOFFDATE.
     03  CUTOFFDATE-CCYY                    PIC 9(04).
     03  CUTOFFDATE-MMDD                    PIC 9(04).
 01  CUTOFFDATE-CYMD REDEFINES CUTOFFDATE   PIC 9(08).
 01  YEAREND                                PIC X(01).
 01  BEGINDATE.
     03  BEGINDATE-CC                       PIC 9(02) VALUE 0.
     03  BIN-DATE-N                         PIC 9(06) VALUE 0.
     03  BIN-DATE-X REDEFINES BIN-DATE-N.
         05  BEGINDATE-YY                   PIC 9(02).
         05  BEGINDATE-MM                   PIC 9(02).
         05  BEGINDATE-DD                   PIC 9(02).
 01  BEGINDATE-CYMDX REDEFINES BEGINDATE.
     03  BEGINDATE-CCYY                     PIC 9(04).
     03  BEGINDATE-MMDD                     PIC 9(04).
 01  BEGINDATE-CYMD REDEFINES BEGINDATE    PIC 9(08).
 01  INPUT-DATES                           PIC X(12).
 01  DUMMY REDEFINES INPUT-DATES.
     03  INPUT-START                       PIC 9(06).
     03  INPUT-END                         PIC 9(06).
*
 01  WS-R-53                                PIC 9(01) VALUE 0.
 01  WS-HOLD-INIT-CONT    PIC 9999V9  VALUE ZERO.
 COPY WS-INIT-CONT.
*01 WS-65PLUS-AND-5PLUS         PIC 9 VALUE 0.
 COPY GAC-WS-800.
 01  WS-PAGE-CHAR                           PIC X(01) VALUE X"FF".
 01  WS-PAGE-CTR                            PIC 9(04) VALUE 0.
 01  DUMMY REDEFINES WS-PAGE-CTR.
     03  FILLER                             PIC 9(03).
     03  WS-PAGE-NO                         PIC 9(01).
 01  WS-64-OVER-SW                          PIC 9     VALUE 0.
 01  SUMMARY-SHORT.
     03 SUMMARY-SUBTITLE-FIRST              PIC X(20) VALUE SPACES.
     03 SUMMARY-SUBTITLE-DESC               PIC X(10) VALUE SPACES.
     03 SUMMARY-NOWAGES                     PIC ZZZ9  VALUE ZERO.
     03 FILLER                              PIC X(21) VALUE SPACES.
 01  XEROX-HEADER.
     03  XEROX-REC                          PIC X(32)
         VALUE "DJDE JDE=PAY426,JDL=PAYROL,END,;".
     03  FILLER                             PIC X(100) VALUE SPACES.
 01  HEADING-1.
     03  FILLER                                 PIC X(08)
         VALUE "PAY426N-".
     03  HD-VERSION                             PIC X(02) VALUE SPACES.
     03  FILLER                                 PIC X(15) VALUE SPACES.
     03  FILLER                                 PIC X(10) VALUE SPACES.
     03  FILLER                                 PIC X(02) VALUE SPACES.
     03  HEAD-1-CLIENT-NAME                     PIC X(30) VALUE SPACES.
     03  FILLER                                 PIC X(22)
         VALUE " PROFIT SHARING FOR ".
     03  HEAD-1-YEAR                            PIC 9(04) VALUE 0.
     03  DUMMY REDEFINES HEAD-1-YEAR.
         05  HEAD-1-CC                          PIC 9(02).
         05  HEAD-1-YY                          PIC 9(02).
     03  FILLER                                 PIC X(15)
         VALUE SPACES.
     03  HD-MM                                  PIC 9(02) VALUE 0.
     03  FILLER                                 PIC X(01) VALUE "/".
     03  HD-DD                                  PIC 9(02) VALUE 0.
     03  FILLER                                 PIC X(01) VALUE "/".
     03  HD-YY                                  PIC 9(02) VALUE 0.
     03  FILLER                                 PIC X(04) VALUE SPACES.
     03  HD-TIME-1                              PIC 9(02) VALUE 0.
     03  FILLER                                 PIC X(01) VALUE ":".
     03  HD-TIME-2                              PIC 9(02) VALUE 0.
     03  FILLER                                 PIC X(07)
         VALUE "  PAGE ".
     03  HD-PAGE                                PIC ZZZZ9.
 01  HEADING-2.
     03  FILLER                                 PIC X(39)
         VALUE "    BADGE NAME                      STR".
     03  FILLER                                 PIC X(35)
         VALUE " T BIRTHDTE AGE  SOC.SEC. #        ".
     03  FILLER                                 PIC X(37)
         VALUE "   WAGES    HOURS  POINT NEW  TERM DT".
     03  FILLER                                 PIC X(24)
         VALUE "E      CURR.BALANCE  SVC".
 01  HEADING-3.
     03  FILLER                                 PIC X(62) VALUE SPACES.
     03  HEAD3-A-TITLE                          PIC X(16)
         VALUE "-CLIENT TOTAL-  ".
     03  HEAD-3-WAGES-TOTAL                     PIC ZZZ,ZZZ,ZZZ.
     03  FILLER                                 PIC X(02) VALUE SPACES.
     03  H-3-H                                  PIC X(10) VALUE SPACES.
     03  HEAD-3-HOURS-TOTAL REDEFINES H-3-H     PIC ZZ,ZZZ,ZZZ.
     03  FILLER                                 PIC X(01) VALUE SPACES.
     03  H-3-P                                  PIC X(09) VALUE SPACES.
     03  HEAD-3-PINTS-TOTAL REDEFINES H-3-P     PIC Z,ZZZ,ZZZ.
     03  FILLER                                 PIC X(25).
 01  SUMMARY-HEADER.
     03  FILLER                                 PIC X(63)
         VALUE SPACES.
     03  FILLER                                 PIC X(37)
         VALUE "EMPS    TOTAL WAGES   TOTAL BALANCE  ".
     03  FILLER                                 PIC X(40)
         VALUE SPACES.
 01  SUMMARY-LINE.
     03  SUMMARY-SUBRPT                         PIC X(01) VALUE SPACE.
     03  SUMMARY-SUBDASH                        PIC X(03) VALUE " - ".
     03  SUMMARY-SUBTITLE                       PIC X(55)
         VALUE SPACES.
     03  FILLER                                 PIC X(01)
         VALUE SPACES.
     03  SUM-CNT                                PIC ZZZ,ZZ9.
     03  FILLER                                 PIC X(01)
         VALUE SPACES.
     03  SUM-WAGE                               PIC ZZZ,ZZZ,ZZZ.99-.
     03  FILLER                                 PIC X(01) VALUE SPACES.
     03  SUM-BAL                                PIC ZZZZ,ZZZ,ZZZ.99-.
     03  FILLER                                 PIC X(37) VALUE SPACES.
 01  SUMMARY-LINE-NWAGE.
     03  FILLER                                 PIC X(04) VALUE SPACE.
     03  SUMMARY-SUBTITLE-NWAGE                 PIC X(55)
         VALUE "EMPLOYEES WITH NO WAGES".
     03  FILLER                                 PIC X(01)
         VALUE SPACES.
     03  SUM-CNT-NWAGE                          PIC ZZZ,ZZ9.
     03  FILLER                                 PIC X(01)
         VALUE SPACES.
     03  SUM-WAGE-NWAGE                         PIC ZZZ,ZZZ,ZZZ.99-.
     03  FILLER                                 PIC X(01) VALUE SPACES.
     03  SUM-BAL-NWAGE                          PIC ZZZZ,ZZZ,ZZZ.99-.
     03  FILLER                                 PIC X(37) VALUE SPACES.
 01  TOTAL-LINE.
     03  TFILLER                                 PIC X(12)
         VALUE "TOTAL EMPS: ".
     03  T-EMPS                                 PIC ZZZ,ZZZ.
     03  FILLER                                 PIC X(49)
         VALUE SPACES.
     03  T-WAGES                                PIC ZZZ,ZZZ,ZZZ.99-.
     03  FILLER                                 PIC X(03) VALUE SPACES.
     03  T-POINTS                               PIC ZZZ,ZZZ,ZZZ.
     03  FILLER                                 PIC X(19)
         VALUE SPACES.
     03  T-BAL                                  PIC ZZZ,ZZZ,ZZZ.99-.
     03  FILLER                                 PIC X(01)
         VALUE SPACES.
 01  TOTAL-NON-EMPS-LINE.
     03  TFILLER                                 PIC X(30)
         VALUE "TOTAL NON-EMP BENEFICIAIRIES: ".
     03  TN-EMPS                                 PIC ZZZ,ZZZ.
     03  FILLER                                 PIC X(31)
         VALUE SPACES.
     03  TN-WAGES                                PIC ZZZ,ZZZ,ZZZ.99-.
     03  FILLER                                 PIC X(03) VALUE SPACES.
     03  TN-POINTS                               PIC ZZZ,ZZZ,ZZZ.
     03  FILLER                                 PIC X(19)
         VALUE SPACES.
     03  TN-BAL                                  PIC ZZZ,ZZZ,ZZZ.99-.
     03  FILLER                                 PIC X(01)
         VALUE SPACES.
 01  TOTAL-LINE-NWAGE.
     03  FILLER                                 PIC X(12)
         VALUE "NO WAGES :  ".
     03  T-EMPS-NWAGE                           PIC ZZZ,ZZ9.
     03  FILLER                                 PIC X(49) VALUE SPACES.
     03  T-WAGES-NWAGE                          PIC ZZZ,ZZZ,ZZZ.99-.
     03  FILLER                                 PIC X(33) VALUE SPACES.
     03  T-BAL-NWAGE                            PIC ZZZ,ZZZ,ZZZ.99-.
     03  FILLER                                 PIC X(01) VALUE SPACES.
 01  DETAIL-LINE.
     03  D-SCODE                            PIC X(01) VALUE SPACES.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  D-EMP                              PIC ZZZZZZ9.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  D-NAME                             PIC X(25) VALUE SPACES.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  D-STR                              PIC ZZ9.
*     03  FILLER                             PIC X(02) VALUE SPACES.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  D-TYPE                             PIC X(01) VALUE SPACES.
*     03  FILLER                             PIC X(02) VALUE SPACES.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  D-BM                               PIC 9(02) VALUE 0.
     03  D-BSL1                             PIC X(01) VALUE "/".
     03  D-BD                               PIC 9(02) VALUE 0.
     03  D-BSL2                             PIC X(01) VALUE "/".
     03  D-BY                               PIC 9(02) VALUE 0.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  D-AS1                              PIC X(01) VALUE "(".
     03  D-AGE                              PIC 9(02) VALUE 0.
     03  D-AS2                              PIC X(01) VALUE ")".
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  D-SSN3                             PIC X(03) VALUE SPACES.
     03  D-SSND1                            PIC X(01) VALUE "-".
     03  D-SSN2                             PIC X(02) VALUE SPACES.
     03  D-SSND2                            PIC X(01) VALUE "-".
     03  D-SSN4                             PIC X(04) VALUE SPACES.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  D-WAGES                            PIC ZZZ,ZZZ,ZZZ.99-.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  D-HOURS                            PIC ZZZZ.99-.
     03  D-POINTS                           PIC ZZZZ9.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  D-NB1                              PIC X(01) VALUE "(".
     03  D-NEW                              PIC X(03) VALUE SPACES.
     03  D-NB2                              PIC X(01) VALUE ")".
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  D-TERMX.
         05  D-TERM-M                       PIC X(02).
         05  D-TERM-SL1                     PIC X(01) VALUE "/".
         05  D-TERM-D                       PIC X(02).
         05  D-TERM-SL2                     PIC X(01) VALUE "/".
         05  D-TERM-Y                       PIC X(02).
     03  D-BAL                              PIC ZZZ,ZZZ,ZZZ,ZZZ.99-.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  D-SVC                              PIC ZZ9.
     03  FILLER                             PIC X(01) VALUE SPACES.
 01  ERR-HEAD1.
     03  FILLER                             PIC X(35)
         VALUE "PAY426N                            ".
     03  FILLER                             PIC X(39)
         VALUE "PROFIT SHARE ERROR REPORT CUTOFF DATE: ".
     03  ERR-CUTOFF-M                       PIC X(02).
     03  FILLER                             PIC X(01) VALUE "/".
     03  ERR-CUTOFF-D                       PIC X(02).
     03  FILLER                             PIC X(01) VALUE "/".
     03  ERR-CUTOFF-Y                       PIC X(04).
     03  FILLER                             PIC X(15)
         VALUE "     RUN DATE: ".
     03  ERR-RM                             PIC X(02).
     03  FILLER                             PIC X(01) VALUE "/".
     03  ERR-RD                             PIC X(02).
     03  FILLER                             PIC X(01) VALUE "/".
     03  ERR-RY                             PIC X(02).
     03  FILLER                             PIC X(19)
         VALUE "             PAGE: ".
     03  ERR-PAGE                           PIC ZZZZ9.
 01  ERR-HEAD2.
     03  FILLER                             PIC X(37)
         VALUE "BADGE NAME                      T    ".
     03  FILLER                             PIC X(37)
         VALUE "SSN #    HIRE     TERM     BIRTH  AGE".
     03  FILLER                             PIC X(37)
         VALUE " SVC P HOURS     WAGES     BALANCE   ".
     03  FILLER                             PIC X(25)
         VALUE "WKS                      ".
 01  ERR-LINE.
     03  ERR-EMP                            PIC X(07) VALUE SPACES.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  ERR-NAME                           PIC X(25) VALUE SPACES.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  ERR-TYPE                           PIC X(01) VALUE SPACES.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  ERR-SSN                            PIC X(09) VALUE SPACES.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  ERR-HIRE-DTE                       PIC X(08) VALUE SPACES.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  ERR-TERM-DTE                       PIC X(08) VALUE SPACES.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  ERR-BIRTH-DTE                      PIC X(08) VALUE SPACES.
     03  FILLER                             PIC X(02) VALUE SPACES.
     03  ERR-AGE                            PIC X(02) VALUE SPACES.
     03  FILLER                             PIC X(02) VALUE SPACES.
     03  ERR-SVC                            PIC X(02) VALUE SPACES.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  ERR-HOURS                          PIC 9999.99.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  ERR-WAGES                          PIC ZZZZZ9.99-.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  ERR-BALANCE                        PIC ZZZZZZ9.99-.
     03  FILLER                             PIC X(01) VALUE SPACES.
     03  ERR-WEEKS                          PIC ZZZ9.
     03  FILLER                             PIC X(22) VALUE SPACES.
 01  ERR-TOT-LINE.
     03  FILLER                             PIC X(06) VALUE "TOTAL ".
     03  ERR-TOTAL-EMPS                       PIC ZZZ,ZZ9.
     03  FILLER                             PIC X(71) VALUE SPACES.
     03  ERR-TOTAL-WAGES                    PIC ZZZ,ZZZ,ZZZ.99-.
     03  FILLER                             PIC X(37) VALUE SPACES.
 01  REPORT-TITLE-1.
     03  FILLER                             PIC X(31)
         VALUE "-ALL ACTIVE/INACTIVE EMPLOYEES ".
     03  REPORT-TITLE-1A.
         05  FILLER                         PIC X(35)
             VALUE "   AGE 18-20 WITH >= 1000 PS HOURS".
     03  FILLER                             PIC X(66)
         VALUE SPACES.
 01  REPORT-TITLE-2.
     03  FILLER                             PIC X(31)
         VALUE "-ALL ACTIVE/INACTIVE EMPLOYEES ".
     03  REPORT-TITLE-2A                    PIC X(32)
         VALUE ">= AGE 21 WITH >= 1000 PS HOURS".
     03  FILLER                             PIC X(69)
         VALUE SPACES.
 01  REPORT-TITLE-3.
     03  FILLER                             PIC X(31)
         VALUE "-ALL ACTIVE/INACTIVE EMPLOYEES ".
     03  REPORT-TITLE-3A                    PIC X(33)
         VALUE "<  AGE 18                        ".
     03  FILLER                             PIC X(69)
         VALUE SPACES.
 01  REPORT-TITLE-4.
     03  FILLER                             PIC X(31)
         VALUE "-ALL ACTIVE/INACTIVE EMPLOYEES ".
     03  REPORT-TITLE-4A                    PIC X(54)
         VALUE ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT".
     03  FILLER                             PIC X(47)
         VALUE SPACES.
 01  REPORT-TITLE-5.
     03  FILLER                             PIC X(31)
         VALUE "-ALL ACTIVE/INACTIVE EMPLOYEES ".
     03  REPORT-TITLE-5A                    PIC X(54)
         VALUE ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT".
     03  FILLER                             PIC X(47)
         VALUE SPACES.
 01  REPORT-TITLE-6.
     03  FILLER                             PIC X(26)
         VALUE "-ALL TERMINATED EMPLOYEES ".
     03  REPORT-TITLE-6A                    PIC X(35)
         VALUE ">= AGE 18 WITH >= 1000 PS HOURS    ".
     03  FILLER                             PIC X(71)
         VALUE SPACES.
 01  REPORT-TITLE-7.
     03  FILLER                             PIC X(26)
         VALUE "-ALL TERMINATED EMPLOYEES ".
     03  REPORT-TITLE-7A                    PIC X(53)
         VALUE ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT".
     03  FILLER                             PIC X(57)
         VALUE SPACES.
 01  REPORT-TITLE-8.
     03  FILLER                             PIC X(26)
         VALUE "-ALL TERMINATED EMPLOYEES ".
     03  REPORT-TITLE-8A                    PIC X(52)
         VALUE ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT".
     03  FILLER                             PIC X(56)
         VALUE SPACES.
 01  REPORT-TITLE-10.
     03  FILLER                             PIC X(32)
         VALUE "-ALL NON-EMPLOYEE BENEFICIARIES ".
     03  FILLER                             PIC X(102)
         VALUE SPACES.
 01  SUMMARY-TITLE.
     03  FILLER                             PIC X(37)
         VALUE "- PROFIT SHARING SUMMARY TOTAL PAGE ".
     03  FILLER                             PIC X(99)
         VALUE SPACES.
*
 01  CLIENT-01.
     03  FILLER                             PIC X(30)
         VALUE "DEMOULAS SUPERMARKETS INC     ".
 LINKAGE SECTION.
 01  OPTION-LINK.
     03  LENGTH-OF PIC S9(9) COMP-5.
     03  TEXTE.
         05 OPTION-CHAR PIC X OCCURS 1 TO 80 DEPENDING ON LENGTH-OF.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 PROCEDURE DIVISION USING OPTION-LINK.
 PROC-CBT-SECT SECTION 01.
 000-START.

     DISPLAY "*** PAY426N starting ***".
     UNSTRING TEXTE DELIMITED BY " " INTO IN-DATE-X YEAREND.
        display "in-date-x " in-date-x " yearend " yearend.
     PERFORM 700-OPEN-FILES THRU 700-EXIT.
     WRITE PR9-REC FROM XEROX-HEADER AFTER ADVANCING PAGE.
     MOVE FUNCTION CURRENT-DATE TO WS-DATE-CURRENT.
     MOVE WS-CURR-CCYY TO WCCYY.
     MOVE WS-CURR-YY TO HD-YY ERR-RY.
     MOVE WS-CURR-MM TO WMM HD-MM ERR-RM.
     MOVE WS-CURR-DD TO WDD HD-DD ERR-RD.
     PERFORM 100-DATE-ACCEPT THRU 100-EXIT.
     MOVE CUTOFFDATE-YY TO ERR-CUTOFF-Y.
     MOVE CUTOFFDATE-MM TO ERR-CUTOFF-M.
     MOVE CUTOFFDATE-DD TO ERR-CUTOFF-D.
     MOVE CUTOFFDATE-CYMD TO BEGINDATE-CYMD.
     IF CUTOFFDATE-MM = 12
         IF CUTOFFDATE-DD < 31
             COMPUTE BEGINDATE-DD = BEGINDATE-DD + 2
             COMPUTE BEGINDATE-CCYY = BEGINDATE-CCYY - 1
         ELSE
             MOVE 01 TO BEGINDATE-MM BEGINDATE-DD
         END-IF
     ELSE
             COMPUTE BEGINDATE-DD = CUTOFFDATE-DD + 1.
     IF BEGINDATE-DD > 31
        IF BEGINDATE-MM = 12
           ADD 1 TO BEGINDATE-CCYY
           MOVE 0101 TO BEGINDATE-MMDD
        END-IF
     END-IF.

     display "cutoffdate = " cutoffdate
       "   BEGINDATE  = "   BEGINDATE. 
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "CUTOFFDATE = "
            CUTOFFDATE
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "BEGINDATE  = "
            BEGINDATE
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.

     MOVE WS-CURR-MM TO ERR-RM.
     MOVE WS-CURR-DD TO ERR-RD.
     MOVE WS-CURR-YY TO ERR-RY.
     MOVE WS-CURR-HR TO HD-TIME-1.
     MOVE WS-CURR-MN TO HD-TIME-2.
     MOVE CLIENT-01 TO HEAD-1-CLIENT-NAME.
     IF WS-CURR-MM NOT = 12
         COMPUTE WCCYY = WCCYY - 1.
     MOVE WORK-CYMD TO SUPER-CYMD.
     MOVE WCCYY TO HEAD-1-YEAR.
     MOVE 0 TO PAYPROF-SSN.
     PERFORM 708-START-PAYPROFIT THRU 708-EXIT.
     IF INVALID-START-PAYPROFIT
         PERFORM 702-CLOSE-FILES THRU 702-EXIT
         GO TO 000-STOP-RUN.
     SORT SORT-OUT ON ASCENDING KEY S-REPORT-CODE
                                    S-NAME
           INPUT PROCEDURE IS 200-EXTRACT THRU 299-EXIT
          OUTPUT PROCEDURE 400-OUTPUT-ROUTINE THRU 405-EXIT.
     IF PAY426N-ABORTS
         MOVE "PAY426N" TO DAEMON-DISP-PROG
         MOVE "***   !!!! PAY426N  ABORTS !!!!   ***" TO DAEMON-DISP-MSG
         CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
         GO TO 000-FINAL-CLOSE.
     PERFORM 460-REPORT-TOTALS THRU 460-EXIT.
     PERFORM 466-WRITE-SUMMARY-RPT THRU 466-EXIT.
 000-FINAL-CLOSE.
     PERFORM 706-CLOSE-FILES THRU 706-EXIT.
     GO TO 000-SUMMARY.

 000-STOP-RUN.
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "** PAY426N -  ERRORS ENCOUNTERED : "
            ERR-CNT
            " **"
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
     DISPLAY "** PAY426N -  ERRORS ENCOUNTERED : " ERR-CNT " **".

 000-SUMMARY.

     DISPLAY "number SKIPPED (not on demographics): " WS-SKIPPED.
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "** PAY426N -  EMPS IN REPORT 1   : "
            RPT1-CNT
            " **"
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.
     DISPLAY "** PAY426N -  EMPS IN REPORT 2   : " RPT2-CNT " **".
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "** PAY426N -  EMPS IN REPORT 2   : "
            RPT2-CNT
            " **"
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "** PAY436N -  EMPS IN REPORT 3   : "
            RPT3-CNT
            " **"
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.
     DISPLAY "** PAY436N -  EMPS IN REPORT 3   : " RPT3-CNT " **".
     DISPLAY "** PAY426N -  EMPS IN REPORT 4   : " RPT4-CNT " **".
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "** PAY424N -  EMPS IN REPORT 4   : "
            RPT4-CNT
            " **"
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "** PAY426N -  EMPS IN REPORT 5   : "
            RPT5-CNT
            " **"
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.
     DISPLAY "** PAY426N -  EMPS IN REPORT 5   : " RPT5-CNT " **".
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "** PAY426N -  EMPS IN REPORT 6   : "
            RPT6-CNT
            " **"
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.
     DISPLAY "** PAY426N -  EMPS IN REPORT 6   : " RPT6-CNT " **".
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "** PAY426N -  EMPS IN REPORT 7   : "
            RPT7-CNT
            " **"
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.
     DISPLAY "** PAY426N -  EMPS IN REPORT 7   : " RPT7-CNT " **".
     DISPLAY "** PAY426N -  EMPS IN REPORT 8   : " RPT8-CNT " **".
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "** PAY426N -  EMPS IN REPORT 8   : "
            RPT8-CNT
            " **"
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "** PAY426N -  NON-EMPS IN REPORT 10   : "
            RPT10-CNT
            " **"
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE SPACES TO DAEMON-DISP-MSG
     STRING  "** PAY426N -  PAYPROFIT REC UPDATED: "
            UPDATED-CNT
            " **"
          DELIMITED SIZE INTO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.
     DISPLAY "** PAY426N -  PAYPROFIT REC UPDATED: " UPDATED-CNT
             " **".
     EXIT PROGRAM.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 100-DATE-ACCEPT.
     INSPECT CUTOFFDATE REPLACING ALL SPACES BY ZEROS.
     IF CUTOFFDATE-YY < 50 MOVE 20 TO CUTOFFDATE-CC
     ELSE                  MOVE 19 TO CUTOFFDATE-CC.
     IF CUTOFFDATE-CCYY > WCCYY + 1
         MOVE "PAY426N" TO DAEMON-DISP-PROG
         MOVE "ENTERED YEAR INVALID GREATER THAN  NEXT YEAR" TO DAEMON-DISP-MSG
         CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
         GO TO 100-DATE-ACCEPT.
     IF CUTOFFDATE-CCYY < WCCYY - 1
         MOVE "PAY426N" TO DAEMON-DISP-PROG
         MOVE "ENTERED YEAR INVALID LESS THAN LAST YEAR" TO DAEMON-DISP-MSG
         CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
         GO TO 100-DATE-ERROR.
     IF CUTOFFDATE-MM > 12 OR < 1
         MOVE "PAY426N" TO DAEMON-DISP-PROG
         MOVE "ENTERED MONTH INVALID" TO DAEMON-DISP-MSG
         CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
         GO TO 100-DATE-ERROR.
     IF (CUTOFFDATE-MM = 4 OR 6 OR 9 OR 11) AND
        (CUTOFFDATE-DD < 1 OR CUTOFFDATE-DD > 30)
         MOVE "PAY426N" TO DAEMON-DISP-PROG
         MOVE "ENTERED DAY FOR THE MONTH INVALID, < 1 OR  > 30" TO DAEMON-DISP-MSG
         CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
         GO TO 100-DATE-ERROR.
     IF (CUTOFFDATE-MM = 1 OR 3 OR 5 OR 7 OR 8 OR 10 OR 12) AND
        ((CUTOFFDATE-DD < 1) OR (CUTOFFDATE-DD > 31))
         MOVE "PAY426N" TO DAEMON-DISP-PROG
         MOVE "ENTERED DAY FOR THE MONTH INVALID, < 1 OR > 31" TO DAEMON-DISP-MSG
         CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
         GO TO 100-DATE-ERROR.
     IF CUTOFFDATE-MM = 2
         COMPUTE LEAP-YEAR = CUTOFFDATE-CCYY / 4
         IF (LEAP-REM = 0) AND
            ((CUTOFFDATE-DD < 1) OR (CUTOFFDATE-DD > 29))
             MOVE "PAY426N" TO DAEMON-DISP-PROG
             MOVE "ENTERED DAY FOR LEAP YEAR, < 1 OR > 29" TO DAEMON-DISP-MSG
             CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
                 GO TO 100-DATE-ERROR
         ELSE
             IF (LEAP-REM > 0) AND
                ((CUTOFFDATE-DD < 1) AND (CUTOFFDATE-DD > 28))
                   MOVE "PAY426N" TO DAEMON-DISP-PROG
                   MOVE "ENTERED DAY, < 1 OR > 28" TO DAEMON-DISP-MSG
                   CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
                 GO TO 100-DATE-ERROR.
     GO TO 100-EXIT.
 100-DATE-ERROR.
     MOVE "PAY426N" TO DAEMON-DISP-PROG
     MOVE "ENTER FISCAL YEAR END DATE IN FORMAT YYMMDD" TO DAEMON-DISP-MSG
     CALL "DISPCONS" USING DAEMON-DISP-DISPLAY.
     MOVE "PAY426N" TO DAEMON-ACCEPT-PROG
     CALL "ACCCONS" USING DAEMON-ACCEPT-MSGQ
     MOVE DAEMON-ACCEPT-BACK TO IN-DATE-X.
     GO TO 100-DATE-ACCEPT.
 100-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*  EXTRACT DATA FROM PAYPROFIT AND PAYBEN FILES
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 200-EXTRACT.
     INITIALIZE WS-RECORD.
     MOVE 0 TO WS-AGE, YEARS, CALC-YEARS, CALC-MONTHS, CALC-DAYS.
 205-READ.
     PERFORM 712-READ-PAYPROFIT-NEXT THRU 712-EXIT.

     IF PYPROFIT-EOF
         PERFORM 300-GET-PAYBEN-DATA THRU 300-EXIT
     END-IF.

     IF PYBEN-EOF
         MOVE TOT-ERR-WAGES TO ERR-TOTAL-WAGES
         ADD TOT-ERR-WAGES TO RPT-WAGES
         MOVE ERR-CNT TO ERR-TOTAL-EMPS
         ADD ERR-CNT TO RPT-TOT
         WRITE PR9-REC FROM ERR-TOT-LINE AFTER ADVANCING 2
         MOVE "PAY426N" TO DAEMON-DISP-PROG
         MOVE SPACES TO DAEMON-DISP-MSG
         STRING  "RECORDS RELEASED TO SORT = "
                SORT-CNT
              DELIMITED SIZE INTO DAEMON-DISP-MSG
         CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
         MOVE "PAY426N" TO DAEMON-DISP-PROG
         MOVE SPACES TO DAEMON-DISP-MSG
         STRING  "MONTHLY EMPS = " MNTHLY-CNT
              DELIMITED SIZE INTO DAEMON-DISP-MSG
         CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
         MOVE "PAY426N" TO DAEMON-DISP-PROG
         MOVE SPACES TO DAEMON-DISP-MSG
         STRING  "MONTHLY WAGES = " MWAGES
              DELIMITED SIZE INTO DAEMON-DISP-MSG
         CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
         PERFORM 702-CLOSE-FILES THRU 702-EXIT
         GO TO 299-EXIT
     END-IF.
 
*****    if no demographics record for this employee, skip the employee
     IF DEMOGRAPHICS-FILE-STATUS NOT = "00"
        DISPLAY "DEMOGRAPHICS-FILE-STATUS: " DEMOGRAPHICS-FILE-STATUS
        "  FOR DEM-BADGE " DEM-BADGE
        "  SKIPPING THIS EMPLOYEE"
        ADD 1 TO WS-SKIPPED
        GO TO 200-EXTRACT
     END-IF.        
 

* FIRST I AM GOING TO CALCULATE LAST-YEARS PY-PS-AMT.
     COMPUTE LY-PY-PS-AMT = (PY-PS-AMT -
                            (PY-PROF-CONT + PY-PROF-EARN))
                          + PY-PROF-FORF.
*****************
     IF PY-FREQ = 2
         ADD 1 TO MNTHLY-CNT
         IF META-SW (2) = 1
             ADD PY-PD-LASTYR TO MNTHLY-WAGES
         ELSE
             ADD PY-PD TO MNTHLY-WAGES
         END-IF
         ADD PY-PD-EXEC TO MNTHLY-WAGES
     END-IF.
     MOVE PY-HIRE-DT TO WS-HIRE-DATE.
     MOVE PY-TERM-DT TO WS-TERM-DATE.
     MOVE PY-TERM TO WS-TERM.
     IF WS-TERM-DATE < BEGINDATE-CYMD
         IF META-SW (2) = 1
             IF PY-PD-LASTYR = 0 AND
                PY-PH-LASTYR = 0 AND
                LY-PY-PS-AMT = 0 AND
                PY-PH-EXEC = 0 AND
                PY-PD-EXEC = 0
                    GO TO 200-EXTRACT
             END-IF
         ELSE
             IF PY-PS-AMT = 0  AND
                PY-PD = 0 AND
                PY-PH = 0 AND
                PY-PH-EXEC = 0 AND
                PY-PD-EXEC = 0
                    GO TO 200-EXTRACT
             END-IF
         END-IF
     END-IF.
     IF WS-TERM-DATE > 0
         IF WS-TERM-DATE > CUTOFFDATE-CYMD
             MOVE 0 TO WS-TERM-DATE
             MOVE "A" TO WS-SCOD
         ELSE
             MOVE PY-SCOD TO WS-SCOD
         END-IF
     ELSE
         MOVE PY-SCOD TO WS-SCOD
     END-IF.
     MOVE PY-DOB TO WS-BIRTH-DATE WS-END-DATE.
     MOVE CUTOFFDATE-CYMD TO WS-START-DATE.
     PERFORM 500-CALC-YEARS THRU 500-EXIT.
     MOVE CALC-YY TO WS-AGE.
     IF META-SW (2) = 1
         COMPUTE WS-PS-YEARS = PY-PS-YEARS - 1
     ELSE
         MOVE PY-PS-YEARS TO WS-PS-YEARS
     END-IF.
     IF WS-AGE > 64 
        MOVE 1 TO WS-OVER-64-SW
     END-IF.
     MOVE PY-PS-ENROLLED TO WS-NEW-EMP.
     IF META-SW (2) = 1
*         MOVE PY-PH-LASTYR TO WS-HRS
         COMPUTE WS-HRS = PY-PH-LASTYR + PY-PH-EXEC
     ELSE
*         MOVE PY-PH TO WS-HRS
         COMPUTE WS-HRS = PY-PH + PY-PH-EXEC
     END-IF.
     INITIALIZE WS-VEST-PRCT, WS-VESTED-SW.
     IF WS-PS-YEARS < 3
         MOVE 0 TO WS-VEST-PRCT
         MOVE 0 TO WS-VESTED-SW
     END-IF.
     IF WS-PS-YEARS = 3
         MOVE 20 TO WS-VEST-PRCT
         MOVE 1 TO WS-VESTED-SW
     END-IF.
     IF WS-PS-YEARS = 4
         MOVE 40 TO WS-VEST-PRCT
         MOVE 1 TO WS-VESTED-SW
     END-IF.
     IF WS-PS-YEARS = 5
         MOVE 60 TO WS-VEST-PRCT
         MOVE 1 TO WS-VESTED-SW
     END-IF.
     IF WS-PS-YEARS = 6
         MOVE 80 TO WS-VEST-PRCT
         MOVE 1 TO WS-VESTED-SW
     END-IF.
     IF WS-PS-YEARS > 6
         MOVE 100 TO WS-VEST-PRCT
         MOVE 1 TO WS-VESTED-SW
     END-IF.

***************************************************************************
** DPRUGH 11/06/02 P#144600  ADDED NEW PROFIT SHARING CHECKS FOR ACTIVE
**                           EMPLOYEES 65+ AND 5+ YEARS SINCE INITIAL
**                           PROFIT SHARING CONTRIBUTION MAKING THEM 100%.
**                           ALSO, DECEASED EMPLOYEES (TERM = Z) ARE 100%.
***************************************************************************
     IF WS-TERM = "Z"
        MOVE 100 TO WS-VEST-PRCT
        MOVE 1 TO WS-VESTED-SW
        GO TO 205-CONTINUE
     END-IF.


 205-CONTINUE.
     IF (WS-SCOD = "A" OR WS-SCOD = "I") AND
         WS-AGE < 18
             MOVE 3 TO WS-REPORT-CODE
             GO TO 210-CONTINUE.
     IF (WS-SCOD = "A" OR WS-SCOD = "I")
        IF WS-HRS > 999.99
            IF  WS-AGE > 20
                 MOVE 2 TO WS-REPORT-CODE
                 GO TO 210-CONTINUE
             ELSE
                 IF WS-AGE > 17 AND WS-AGE < 21
                     MOVE 1 TO WS-REPORT-CODE
**                     display " PAYPROF-BADGE " PAYPROF-BADGE  
                     GO TO 210-CONTINUE
                 END-IF
             END-IF
         ELSE
             IF WS-AGE > 17
                 IF PY-PS-AMT > 0
                      MOVE 4 TO WS-REPORT-CODE
                      GO TO 210-CONTINUE
                 ELSE
                      MOVE 5 TO WS-REPORT-CODE
                      GO TO 210-CONTINUE.
     MOVE ZERO TO WS-UNDR18-WAGES.
     IF WS-AGE < 18 AND WS-SCOD = "T"
         ADD 1 TO UNDR18-CNT
         IF META-SW (2) = 1
             ADD PY-PD-LASTYR TO UNDR18-WAGES
             ADD PY-PD-EXEC TO UNDR18-WAGES
             ADD LY-PY-PS-AMT TO UNDR18-BAL
*             MOVE PY-PD-LASTYR TO WS-UNDR18-WAGES
             COMPUTE WS-UNDR18-WAGES = PY-PD-LASTYR + PY-PD-EXEC
         ELSE
             ADD PY-PD TO UNDR18-WAGES
             ADD PY-PS-AMT TO UNDR18-BAL
*             MOVE PY-PD TO WS-UNDR18-WAGES
             COMPUTE WS-UNDR18-WAGES = PY-PD + PY-PD-EXEC
         END-IF
         IF WS-UNDR18-WAGES = 0
            ADD 1 TO UNDR18-CNT-NWAGE
         END-IF
         GO TO 200-EXTRACT
     END-IF.
     IF WS-SCOD = "T"
         IF WS-HRS > 999.99
             IF WS-AGE > 17
                 MOVE 6 TO WS-REPORT-CODE
                 GO TO 210-CONTINUE
             ELSE
                 GO TO 208-ERROR
             END-IF
         ELSE
             IF WS-AGE > 17
                 IF META-SW (2) = 1
                     IF LY-PY-PS-AMT > 0
                         MOVE 8 TO WS-REPORT-CODE
                         GO TO 210-CONTINUE
                     ELSE
                         MOVE 7 TO WS-REPORT-CODE
                         GO TO 210-CONTINUE
                     END-IF
                 ELSE
                     IF PY-PS-AMT > 0
                         MOVE 8 TO WS-REPORT-CODE
                         GO TO 210-CONTINUE
                     ELSE
                         MOVE 7 TO WS-REPORT-CODE
                         GO TO 210-CONTINUE.
 208-ERROR.
     MOVE PAYPROF-BADGE TO ERR-EMP.
     MOVE PY-NAM TO ERR-NAME.
     MOVE PY-SCOD TO ERR-TYPE.
     MOVE PAYPROF-SSN TO ERR-SSN.
     MOVE WS-HIRE-DATE TO ERR-HIRE-DTE.
     MOVE WS-TERM-DATE TO ERR-TERM-DTE.
     MOVE WS-BIRTH-DATE TO ERR-BIRTH-DTE.
     MOVE WS-PS-YEARS TO ERR-SVC.
     MOVE WS-AGE TO ERR-AGE.
     IF META-SW (2) = 1
*         MOVE PY-PH-LASTYR TO ERR-HOURS
*         MOVE PY-PD-LASTYR TO ERR-WAGES
         COMPUTE ERR-HOURS = PY-PH-LASTYR + PY-PH-EXEC
         COMPUTE ERR-WAGES = PY-PD-LASTYR + PY-PD-EXEC
         MOVE PY-WEEKS-WORK-LAST TO ERR-WEEKS
     ELSE
*         MOVE PY-PD TO ERR-WAGES
*         MOVE PY-PH TO ERR-HOURS
         COMPUTE ERR-WAGES = PY-PD + PY-PD-EXEC
         COMPUTE ERR-HOURS = PY-PH + PY-PH-EXEC
         MOVE PY-WEEKS-WORK TO ERR-WEEKS
     END-IF.
     IF META-SW (2) = 1
         MOVE LY-PY-PS-AMT TO ERR-BALANCE
     ELSE
         MOVE PY-PS-AMT TO ERR-BALANCE
     END-IF.
     IF META-SW (2) = 1
         ADD PY-PD-LASTYR TO TOT-ERR-WAGES
     ELSE
         ADD PY-PD TO TOT-ERR-WAGES
     END-IF.
     ADD PY-PD-EXEC TO TOT-ERR-WAGES.
     IF META-SW (2) = 1
*         MOVE PY-PH-LASTYR TO ERR-HOURS
         COMPUTE ERR-HOURS = PY-PH-LASTYR + PY-PH-EXEC
     ELSE
*         MOVE PY-PH TO ERR-HOURS
         COMPUTE ERR-HOURS = PY-PH + PY-PH-EXEC
     END-IF.
     PERFORM 510-ERR-RPT THRU 510-EXIT.
     GO TO 200-EXTRACT.
 210-CONTINUE.
     MOVE PAYPROF-BADGE TO WS-EMP-NUMBER.
     MOVE PY-NAM TO WS-NAME.
     MOVE PAYPROF-SSN TO WS-SSN.
     MOVE PY-STOR TO WS-STORE.
     MOVE PY-TYPE TO WS-TYPE.
     IF META-SW (2) = 1
*         MOVE PY-PH-LASTYR TO WS-HRS
        COMPUTE WS-HRS = PY-PH-LASTYR + PY-PH-EXEC
     ELSE
*         MOVE PY-PH TO WS-HRS
        COMPUTE WS-HRS = PY-PH + PY-PH-EXEC
     END-IF.
     IF META-SW (2) = 1
         COMPUTE WS-PYRS = PY-PS-YEARS - 1
     ELSE
         MOVE PY-PS-YEARS TO WS-PYRS
     END-IF.
     IF META-SW (2) = 1
*         MOVE PY-PD-LASTYR TO WS-WAGES
         COMPUTE WS-WAGES = PY-PD-LASTYR + PY-PD-EXEC
         MOVE PY-WEEKS-WORK-LAST TO WS-WK-WEEKS
     ELSE
*         MOVE PY-PD TO WS-WAGES
         COMPUTE WS-WAGES = PY-PD + PY-PD-EXEC
         MOVE PY-WEEKS-WORK TO WS-WK-WEEKS
     END-IF.
     IF META-SW (2) = 1
         MOVE LY-PY-PS-AMT TO WS-CURR-BALANCE
     ELSE
         MOVE PY-PS-AMT TO WS-CURR-BALANCE
     END-IF.
     PERFORM 240-ELIGIBILITY-CHECK THRU 240-EXIT.

 220-TEST-FOR-REPORTS.
     IF ( META-SW (11) = 1) OR
        ( META-SW (12) = 1) OR
        ( META-SW (13) = 1) OR
        ( META-SW (14) = 1) OR
        ( META-SW (15) = 1) OR
        ( META-SW (16) = 1) OR
        ( META-SW (17) = 1) OR
        ( META-SW (18) = 1) OR
        ( META-SW (19) = 1)
          NEXT SENTENCE
     ELSE
          GO TO 230-WRITE
     END-IF.

     IF META-SW (11) = 1 AND WS-REPORT-CODE = 1
         GO TO 230-WRITE
     END-IF.
     IF META-SW (12) = 1 AND WS-REPORT-CODE = 2
         GO TO 230-WRITE
     END-IF.
     IF META-SW (13) = 1 AND WS-REPORT-CODE = 3
         GO TO 230-WRITE
     END-IF.
     IF META-SW (14) = 1 AND WS-REPORT-CODE = 4
         GO TO 230-WRITE
     END-IF.
     IF META-SW (15) = 1 AND WS-REPORT-CODE = 5
         GO TO 230-WRITE
     END-IF.
     IF META-SW (16) = 1 AND WS-REPORT-CODE = 6
         GO TO 230-WRITE
     END-IF.
     IF META-SW (17) = 1 AND WS-REPORT-CODE = 7
         GO TO 230-WRITE
     END-IF.
     IF META-SW (18) = 1 AND WS-REPORT-CODE = 8
         GO TO 230-WRITE
     END-IF.
     IF META-SW (19) = 1 AND WS-REPORT-CODE = 10
         GO TO 230-WRITE
     END-IF.

     GO TO 200-EXTRACT.
 230-WRITE.
*    IF WS-ELIGIBILITY-SW = 0 GO TO 200-EXTRACT.
     RELEASE SORTREC FROM WS-RECORD.
     ADD 1 TO SORT-CNT.
     GO TO 200-EXTRACT.
 240-ELIGIBILITY-CHECK.
     IF (WS-SCOD = "T") AND (WS-HRS > 999.99)
         MOVE 1 TO WS-ELIGIBILITY-SW WS-TERM-VESTED
     ELSE
         IF WS-SCOD = "T" OR WS-HRS = 0
             MOVE 0 TO WS-ELIGIBILITY-SW
         ELSE
             MOVE 1 TO WS-ELIGIBILITY-SW
         END-IF
     END-IF.
     IF WS-AGE > 64 AND WS-HRS > 999.99
         MOVE 1 TO WS-ELIGIBILITY-SW WS-OVER-64-SW
     ELSE
         IF WS-AGE > 64
            IF WS-HRS > 0 AND WS-PYRS > 0
                MOVE 1 TO WS-ELIGIBILITY-SW
            END-IF
         END-IF
     END-IF.

 240-EXIT.
     EXIT.

 299-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 300-GET-PAYBEN-DATA.
     CALL "READ-NEXT-PAYBEN" USING PAYBEN-FILE-STATUS
                                   PAYBEN-REC
     IF PAYBEN-FILE-STATUS NOT = "00"
        MOVE 1 TO WS-PAYBEN-READ-NEXT-SW
        GO TO 300-EXIT
     END-IF.

     MOVE PYBEN-PAYSSN TO PAYPROF-SSN
     MOVE "PAYPROF-SSN-KEY" TO UFAS-ALT-KEY-NAME
     CALL "READ-ALT-KEY-PAYPROFIT" USING PAYPROFIT-FILE-STATUS          
                                         UFAS-ALT-KEY-NAME
                                         PAYPROF-REC.
     IF PAYPROFIT-FILE-STATUS           = "00"
        GO TO 300-GET-PAYBEN-DATA
     END-IF.

     INITIALIZE WS-RECORD.
     MOVE 10 TO WS-REPORT-CODE.
     MOVE PYBEN-NAME TO WS-NAME.
     MOVE 0 TO WS-EMP-NUMBER.
     MOVE 0 TO WS-STORE.
     MOVE 0 TO WS-TYPE.
     MOVE 0 TO WS-EMP.
     MOVE PYBEN-DOBIRTH TO WS-BIRTH-DATE.
     MOVE 99 TO WS-AGE
     MOVE PYBEN-PAYSSN TO WS-SSN.
     MOVE ZEROS TO WS-SSN-3.
     MOVE ZEROS TO WS-SSN-2.
     MOVE 0 TO WS-WAGES.
     MOVE 0 TO WS-HRS.
     MOVE 0 TO WS-NEW-EMP.
     MOVE 0 TO WS-TERM-DATE.
     MOVE PYBEN-PSAMT TO WS-CURR-BALANCE.
     MOVE 0 TO WS-WK-WEEKS.
     MOVE 0 TO WS-HIRE-DATE.
     MOVE 7 TO WS-PS-YEARS.
     MOVE 0 TO WS-OVER-64-SW.
     MOVE 1 TO WS-VESTED-SW.
     MOVE 100 TO WS-VEST-PRCT.
     MOVE 1 TO WS-TERM-VESTED.
     MOVE 0 TO WS-SCOD.
     MOVE 1 TO WS-ELIGIBILITY-SW.
     MOVE 99 TO WS-PYRS.
     MOVE 0 TO WS-65PLUS-AND-5PLUS.

     ADD 1 TO WS-PAYBEN-COUNT.

     RELEASE SORTREC FROM WS-RECORD.

     ADD 1 TO SORT-CNT.

     GO TO 300-GET-PAYBEN-DATA.

 300-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*  OUTPUT REPORT DATA FOR PAYR AND PAYBEN FILES
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 400-OUTPUT-ROUTINE.
     PERFORM 704-OPEN THRU 704-EXIT.
     PERFORM GAC-CALL.
 402-RETURN-SORT.
     RETURN SORT-OUT
        AT END GO TO 405-EXIT.
     MOVE 0 TO PROFIT-SHARE-SWITCH.
     MOVE 0 TO WS-ABORT-INDICATOR, WS-PAYPROFIT-READ-SW
               WS-POINTS, WS-REMAINDER
               WS-PAYPROFIT-REWRITE-SW.
     IF (S-AGE > 20 AND S-ELIGIBILITY-SW = 1)
               PERFORM 600-CHECK-PROFIT THRU 600-EXIT
               PERFORM 610-UPDATE-PAYPROFIT THRU 649-EXIT
     ELSE
          MOVE S-EMP-NUMBER TO PAYPROF-BADGE
          PERFORM 710-READ-PAYPROFIT THRU 710-EXIT
     END-IF.
     IF PAY426N-ABORTS 
        GO TO 405-EXIT
     END-IF.
     PERFORM 450-WRITE-REPORT THRU 450-EXIT.
     GO TO 402-RETURN-SORT.
 405-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 450-WRITE-REPORT.
     INITIALIZE DETAIL-LINE.
     IF S-REPORT-CODE = HOLD-REPORT-CODE
         NEXT SENTENCE
     ELSE
         PERFORM 460-REPORT-TOTALS THRU 460-EXIT
         MOVE S-REPORT-CODE TO HOLD-REPORT-CODE
              HD-VERSION
         PERFORM 462-WRITE-DJDE-HEADER THRU 462-EXIT
         PERFORM 455-WRITE-REPORT-HEADERS THRU 455-EXIT
     END-IF.
     IF S-SCOD = "I"
         MOVE S-SCOD TO D-SCODE
     ELSE
         MOVE SPACES TO D-SCODE
     END-IF.
     MOVE S-EMP-NUMBER TO D-EMP.
     MOVE S-NAME TO D-NAME.
     MOVE S-STORE TO D-STR.
     MOVE S-TYPE TO D-TYPE.
     MOVE S-BIRTH-MM TO D-BM.
     MOVE "/" TO D-BSL1 D-BSL2 D-TERM-SL1 D-TERM-SL2.
     MOVE S-BIRTH-DD TO D-BD.
     MOVE S-BIRTH-YY TO D-BY.
     MOVE "(" TO D-AS1 D-NB1.
     MOVE ")" TO D-AS2 D-NB2.
     MOVE S-AGE TO D-AGE.
     MOVE "-" TO D-SSND1 D-SSND2.
     MOVE S-SSN-2 TO D-SSN2.
     move zero to d-ssn2.
     MOVE S-SSN-3 TO D-SSN3.
     move zero to d-ssn3.
     MOVE S-SSN-4 TO D-SSN4.
     MOVE S-WAGES TO D-WAGES.
     MOVE S-HRS TO D-HOURS.
     IF S-REPORT-CODE = 2
         MOVE PY-PROF-POINTS TO D-POINTS
     ELSE
         MOVE 0 TO D-POINTS
     END-IF.
     IF (S-NEW-EMP = 0) AND (S-REPORT-CODE = 1 OR 2)
         MOVE "NEW" TO D-NEW
     END-IF.
     IF S-AGE < 21 AND D-NEW = SPACES
         MOVE "<21" TO D-NEW
     END-IF.
     IF S-TERM-DATE > 0
         MOVE S-TERM-YY TO D-TERM-Y
         MOVE S-TERM-MM TO D-TERM-M
         MOVE S-TERM-DD TO D-TERM-D
     ELSE
         MOVE SPACES TO D-TERMX
     END-IF.
     MOVE S-CURR-BALANCE TO D-BAL.
     MOVE S-YEARS-OF-SERVICE TO D-SVC.
     MOVE CLIENT-01 TO HEAD-1-CLIENT-NAME

     IF S-REPORT-CODE = 1
         IF S-AGE < 21
             ADD 1 TO RPT1-UNDR21
             ADD S-CURR-BALANCE TO RPT1-UNDR21-BAL
             ADD S-WAGES TO RPT1-UNDR21-WAGES
         END-IF
         PERFORM 530-REPORT1 THRU 530-EXIT
         IF S-WAGES = 0
            ADD 1 TO RPT1-CNT-NWAGE
            ADD S-CURR-BALANCE TO RPT1-BAL-NWAGE
            ADD S-WAGES TO RPT1-WAGES-NWAGE
         END-IF
         ADD 1 TO RPT1-CNT RPT-TOT
         ADD S-CURR-BALANCE TO RPT1-BAL, RPT-BAL
         ADD S-WAGES TO RPT1-WAGES, RPT-WAGES
     END-IF.

     IF S-REPORT-CODE = 2
         IF S-AGE < 21
             ADD 1 TO RPT2-UNDR21
             ADD S-CURR-BALANCE TO RPT2-UNDR21-BAL
             ADD S-WAGES TO RPT2-UNDR21-WAGES
         END-IF
         PERFORM 540-REPORT2 THRU 540-EXIT
         IF S-WAGES = 0
            ADD 1 TO RPT2-CNT-NWAGE RPT9-CNT-NWAGE
            ADD S-CURR-BALANCE TO RPT2-BAL-NWAGE  RPT9-BAL-NWAGE
            ADD S-WAGES TO RPT2-WAGES-NWAGE  RPT9-WAGES-NWAGE
         END-IF
         ADD 1 TO RPT2-CNT, RPT-TOT
         ADD S-CURR-BALANCE TO RPT2-BAL, RPT-BAL
         ADD S-WAGES TO RPT2-WAGES, RPT-WAGES
     END-IF.

     IF S-REPORT-CODE = 3
         IF S-AGE < 21
             ADD 1 TO RPT3-UNDR21
             ADD S-CURR-BALANCE TO RPT3-UNDR21-BAL
             ADD S-WAGES TO RPT3-UNDR21-WAGES
         END-IF
         PERFORM 550-REPORT3 THRU 550-EXIT
         IF S-WAGES = 0
            ADD 1 TO RPT3-CNT-NWAGE RPT9-CNT-NWAGE
            ADD S-CURR-BALANCE TO RPT3-BAL-NWAGE RPT9-BAL-NWAGE
            ADD S-WAGES TO RPT3-WAGES-NWAGE RPT9-WAGES-NWAGE
         END-IF
         ADD 1 TO RPT3-CNT, RPT-TOT
         ADD S-CURR-BALANCE TO RPT3-BAL, RPT-BAL
         ADD S-WAGES TO RPT3-WAGES, RPT-WAGES
     END-IF.

     IF S-REPORT-CODE = 4
         IF S-AGE < 21
             ADD 1 TO RPT4-UNDR21
             ADD S-CURR-BALANCE TO RPT4-UNDR21-BAL
             ADD S-WAGES TO RPT4-UNDR21-WAGES
         END-IF
         PERFORM 560-REPORT4 THRU 560-EXIT
         IF S-WAGES = 0
            ADD 1 TO RPT4-CNT-NWAGE RPT9-CNT-NWAGE
            ADD S-CURR-BALANCE TO RPT4-BAL-NWAGE RPT9-BAL-NWAGE
            ADD S-WAGES TO RPT4-WAGES-NWAGE RPT9-WAGES-NWAGE
         END-IF
         ADD 1 TO RPT4-CNT, RPT-TOT
         ADD S-CURR-BALANCE TO RPT4-BAL, RPT-BAL
         ADD S-WAGES TO RPT4-WAGES, RPT-WAGES
     END-IF.

     IF S-REPORT-CODE = 5
         IF S-AGE < 21
             ADD 1 TO RPT5-UNDR21
             ADD S-CURR-BALANCE TO RPT5-UNDR21-BAL
             ADD S-WAGES TO RPT5-UNDR21-WAGES
         END-IF
         PERFORM 570-REPORT5 THRU 570-EXIT
         IF S-WAGES = 0
            ADD 1 TO RPT5-CNT-NWAGE RPT9-CNT-NWAGE
            ADD S-CURR-BALANCE TO RPT5-BAL-NWAGE RPT9-BAL-NWAGE
            ADD S-WAGES TO RPT5-WAGES-NWAGE RPT9-WAGES-NWAGE
         END-IF
         ADD 1 TO RPT5-CNT, RPT-TOT
         ADD S-CURR-BALANCE TO RPT5-BAL, RPT-BAL
         ADD S-WAGES TO RPT5-WAGES, RPT-WAGES
     END-IF.

     IF S-REPORT-CODE = 6
         IF S-AGE < 21
             ADD 1 TO RPT6-UNDR21
             ADD S-CURR-BALANCE TO RPT6-UNDR21-BAL
             ADD S-WAGES TO RPT6-UNDR21-WAGES
         END-IF
         PERFORM 580-REPORT6 THRU 580-EXIT
         IF S-WAGES = 0
            ADD 1 TO RPT6-CNT-NWAGE RPT9-CNT-NWAGE
            ADD S-CURR-BALANCE TO RPT6-BAL-NWAGE RPT9-BAL-NWAGE
            ADD S-WAGES TO RPT6-WAGES-NWAGE RPT9-WAGES-NWAGE
         END-IF
         ADD 1 TO RPT6-CNT, RPT-TOT
         ADD S-CURR-BALANCE TO RPT6-BAL, RPT-BAL
         ADD S-WAGES TO RPT6-WAGES, RPT-WAGES
     END-IF.

     IF S-REPORT-CODE = 7
         IF S-AGE < 21
             ADD 1 TO RPT7-UNDR21
             ADD S-CURR-BALANCE TO RPT7-UNDR21-BAL
             ADD S-WAGES TO RPT7-UNDR21-WAGES
         END-IF
         PERFORM 590-REPORT7 THRU 590-EXIT
         IF S-WAGES = 0
            ADD 1 TO RPT7-CNT-NWAGE RPT9-CNT-NWAGE
            ADD S-CURR-BALANCE TO RPT7-BAL-NWAGE RPT9-BAL-NWAGE
            ADD S-WAGES TO RPT7-WAGES-NWAGE RPT9-WAGES-NWAGE
         END-IF
         ADD 1 TO RPT7-CNT, RPT-TOT
         ADD S-CURR-BALANCE TO RPT7-BAL, RPT-BAL
         ADD S-WAGES TO RPT7-WAGES, RPT-WAGES
     END-IF.

     IF S-REPORT-CODE = 8
         IF S-AGE < 21
             ADD 1 TO RPT8-UNDR21
             ADD S-CURR-BALANCE TO RPT8-UNDR21-BAL
             ADD S-WAGES TO RPT8-UNDR21-WAGES
         END-IF
         PERFORM 594-REPORT8 THRU 594-EXIT
         IF S-WAGES = 0
            ADD 1 TO RPT8-CNT-NWAGE RPT9-CNT-NWAGE
            ADD S-CURR-BALANCE TO RPT8-BAL-NWAGE RPT9-BAL-NWAGE
            ADD S-WAGES TO RPT8-WAGES-NWAGE RPT9-WAGES-NWAGE
         END-IF
         ADD 1 TO RPT8-CNT, RPT-TOT
         ADD S-CURR-BALANCE TO RPT8-BAL, RPT-BAL
         ADD S-WAGES TO RPT8-WAGES, RPT-WAGES
     END-IF.

     IF S-REPORT-CODE = 10
         PERFORM  597-REPORT10 THRU 597-EXIT
         ADD 1 TO RPT10-CNT, RPT-TOT
         ADD S-CURR-BALANCE TO RPT10-BAL, RPT-BAL
         ADD S-WAGES TO RPT10-WAGES, RPT-WAGES
     END-IF.

     INITIALIZE DETAIL-LINE.
 450-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 455-WRITE-REPORT-HEADERS.
     IF HOLD-REPORT-CODE = 1
         PERFORM 535-HEADING THRU 535-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 2
         PERFORM 545-HEADING THRU 545-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 3
         PERFORM 555-HEADING THRU 555-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 4
         PERFORM 565-HEADING THRU 565-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 5
         PERFORM 575-HEADING THRU 575-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 6
         PERFORM 585-HEADING THRU 585-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 7
         PERFORM 592-HEADING THRU 592-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 8
         PERFORM 596-HEADING THRU 596-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 10
         PERFORM 596-HEADING THRU 596-EXIT
     END-IF.
 455-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 460-REPORT-TOTALS.
     IF HOLD-REPORT-CODE = 1
         MOVE RPT1-CNT TO T-EMPS
         MOVE RPT1-WAGES TO T-WAGES
         MOVE RPT1-BAL TO T-BAL
         MOVE RPT1-POINTS TO T-POINTS
         WRITE PR1-REC FROM TOTAL-LINE AFTER ADVANCING 2
         MOVE RPT1-CNT-NWAGE TO T-EMPS-NWAGE
         MOVE RPT1-WAGES-NWAGE TO T-WAGES-NWAGE
         MOVE RPT1-BAL-NWAGE TO T-BAL-NWAGE
         WRITE PR1-REC FROM TOTAL-LINE-NWAGE AFTER ADVANCING 2
         GO TO 460-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 2
         MOVE RPT2-CNT TO T-EMPS
         MOVE RPT2-WAGES TO T-WAGES
         MOVE RPT2-BAL TO T-BAL
         MOVE RPT2-POINTS TO T-POINTS
         WRITE PR2-REC FROM TOTAL-LINE AFTER ADVANCING 2
         MOVE RPT2-CNT-NWAGE TO T-EMPS-NWAGE
         MOVE RPT2-WAGES-NWAGE TO T-WAGES-NWAGE
         MOVE RPT2-BAL-NWAGE TO T-BAL-NWAGE
         WRITE PR2-REC FROM TOTAL-LINE-NWAGE AFTER ADVANCING 2
         GO TO 460-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 3
         MOVE RPT3-CNT TO T-EMPS
         MOVE RPT3-WAGES TO T-WAGES
         MOVE RPT3-BAL TO T-BAL
         MOVE RPT3-POINTS TO T-POINTS
         WRITE PR3-REC FROM TOTAL-LINE AFTER ADVANCING 2
         MOVE RPT3-CNT-NWAGE TO T-EMPS-NWAGE
         MOVE RPT3-WAGES-NWAGE TO T-WAGES-NWAGE
         MOVE RPT3-BAL-NWAGE TO T-BAL-NWAGE
         WRITE PR3-REC FROM TOTAL-LINE-NWAGE AFTER ADVANCING 2
         GO TO 460-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 4
         MOVE RPT4-CNT TO T-EMPS
         MOVE RPT4-WAGES TO T-WAGES
         MOVE RPT4-BAL TO T-BAL
         MOVE RPT4-POINTS TO T-POINTS
         WRITE PR4-REC FROM TOTAL-LINE AFTER ADVANCING 2
         MOVE RPT4-CNT-NWAGE TO T-EMPS-NWAGE
         MOVE RPT4-WAGES-NWAGE TO T-WAGES-NWAGE
         MOVE RPT4-BAL-NWAGE TO T-BAL-NWAGE
         WRITE PR4-REC FROM TOTAL-LINE-NWAGE AFTER ADVANCING 2
         GO TO 460-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 5
         MOVE RPT5-CNT TO T-EMPS
         MOVE RPT5-WAGES TO T-WAGES
         MOVE RPT5-BAL TO T-BAL
         MOVE RPT5-POINTS TO T-POINTS
         WRITE PR5-REC FROM TOTAL-LINE AFTER ADVANCING 2
         MOVE RPT5-CNT-NWAGE TO T-EMPS-NWAGE
         MOVE RPT5-WAGES-NWAGE TO T-WAGES-NWAGE
         MOVE RPT5-BAL-NWAGE TO T-BAL-NWAGE
         WRITE PR5-REC FROM TOTAL-LINE-NWAGE AFTER ADVANCING 2
         GO TO 460-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 6
         MOVE RPT6-CNT TO T-EMPS
         MOVE RPT6-WAGES TO T-WAGES
         MOVE RPT6-BAL TO T-BAL
         MOVE RPT6-POINTS TO T-POINTS
         WRITE PR6-REC FROM TOTAL-LINE AFTER ADVANCING 2
         MOVE RPT6-CNT-NWAGE TO T-EMPS-NWAGE
         MOVE RPT6-WAGES-NWAGE TO T-WAGES-NWAGE
         MOVE RPT6-BAL-NWAGE TO T-BAL-NWAGE
         WRITE PR6-REC FROM TOTAL-LINE-NWAGE AFTER ADVANCING 2
         GO TO 460-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 7
         MOVE RPT7-CNT TO T-EMPS
         MOVE RPT7-WAGES TO T-WAGES
         MOVE RPT7-BAL TO T-BAL
         MOVE RPT7-POINTS TO T-POINTS
         WRITE PR7-REC FROM TOTAL-LINE AFTER ADVANCING 2
         MOVE RPT7-CNT-NWAGE TO T-EMPS-NWAGE
         MOVE RPT7-WAGES-NWAGE TO T-WAGES-NWAGE
         MOVE RPT7-BAL-NWAGE TO T-BAL-NWAGE
         WRITE PR7-REC FROM TOTAL-LINE-NWAGE AFTER ADVANCING 2
         GO TO 460-EXIT
     END-IF.
     IF HOLD-REPORT-CODE = 8
         MOVE RPT8-CNT TO T-EMPS
         MOVE RPT8-WAGES TO T-WAGES
         MOVE RPT8-BAL TO T-BAL
         MOVE RPT8-POINTS TO T-POINTS
         WRITE PR8-REC FROM TOTAL-LINE AFTER ADVANCING 2
         MOVE RPT8-CNT-NWAGE TO T-EMPS-NWAGE
         MOVE RPT8-WAGES-NWAGE TO T-WAGES-NWAGE
         MOVE RPT8-BAL-NWAGE TO T-BAL-NWAGE
         WRITE PR8-REC FROM TOTAL-LINE-NWAGE AFTER ADVANCING 2
     END-IF.
     IF HOLD-REPORT-CODE = 10
         MOVE RPT10-CNT TO Tn-EMPS
         MOVE RPT10-WAGES TO Tn-WAGES
         MOVE RPT10-BAL TO Tn-BAL
         MOVE RPT10-POINTS TO Tn-POINTS
         WRITE PR10-REC FROM TOTAL-NON-EMPS-LINE AFTER ADVANCING 2
     END-IF.
 460-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 462-WRITE-DJDE-HEADER.
     IF S-REPORT-CODE = 1
         WRITE PR1-REC FROM XEROX-HEADER
               AFTER ADVANCING PAGE
     END-IF.
     IF S-REPORT-CODE = 2
         WRITE PR2-REC FROM XEROX-HEADER
               AFTER ADVANCING PAGE
     END-IF.
     IF S-REPORT-CODE = 3
         WRITE PR3-REC FROM XEROX-HEADER
               AFTER ADVANCING PAGE
     END-IF.
     IF S-REPORT-CODE = 4
         WRITE PR4-REC FROM XEROX-HEADER
               AFTER ADVANCING PAGE
     END-IF.
     IF S-REPORT-CODE = 5
         WRITE PR5-REC FROM XEROX-HEADER
               AFTER ADVANCING PAGE
     END-IF.
     IF S-REPORT-CODE = 6
         WRITE PR6-REC FROM XEROX-HEADER
               AFTER ADVANCING PAGE
     END-IF.
     IF S-REPORT-CODE = 7
         WRITE PR7-REC FROM XEROX-HEADER
         AFTER ADVANCING PAGE
     END-IF.
     IF S-REPORT-CODE = 8
         WRITE PR8-REC FROM XEROX-HEADER
         AFTER ADVANCING PAGE
     END-IF.
     IF S-REPORT-CODE = 10
         WRITE PR10-REC FROM XEROX-HEADER
         AFTER ADVANCING PAGE
     END-IF.
 462-EXIT.
     EXIT.
 
*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 466-WRITE-SUMMARY-RPT.
     WRITE PR9-REC FROM XEROX-HEADER AFTER ADVANCING 1.
     MOVE 1 TO HD-PAGE.
     MOVE "S" TO HD-VERSION.
     WRITE PR9-REC FROM HEADING-1 AFTER ADVANCING 1.
     WRITE PR9-REC FROM SUMMARY-TITLE AFTER ADVANCING 2.
     WRITE PR9-REC FROM SUMMARY-HEADER AFTER ADVANCING 2.
     MOVE SPACES TO PR9-REC.
     WRITE PR9-REC AFTER ADVANCING 1.
     MOVE "E" TO SUMMARY-SUBRPT.
     MOVE "ERROR REPORT EMPLOYEES" TO SUMMARY-SUBTITLE.
     MOVE ERR-CNT TO SUM-CNT.
     MOVE TOT-ERR-WAGES TO SUM-WAGE.
     MOVE 0 TO SUM-BAL.
     WRITE PR9-REC FROM SUMMARY-LINE AFTER ADVANCING 1.
     MOVE "ACTIVE AND INACTIVE: " TO PR9-REC.
     WRITE PR9-REC AFTER ADVANCING 2.
     MOVE SPACES TO PR9-REC.
     WRITE PR9-REC AFTER ADVANCING 1.
     MOVE "1" TO SUMMARY-SUBRPT.
     MOVE REPORT-TITLE-1A TO SUMMARY-SUBTITLE.
     MOVE RPT1-CNT TO SUM-CNT.
     MOVE RPT1-WAGES TO SUM-WAGE.
     MOVE RPT1-BAL TO SUM-BAL.
     WRITE PR9-REC FROM SUMMARY-LINE AFTER ADVANCING 1.
     MOVE "2" TO SUMMARY-SUBRPT.
     MOVE REPORT-TITLE-2A TO SUMMARY-SUBTITLE.
     MOVE RPT2-CNT TO SUM-CNT.
     MOVE RPT2-WAGES TO SUM-WAGE.
     MOVE RPT2-BAL TO SUM-BAL.
     WRITE PR9-REC FROM SUMMARY-LINE AFTER ADVANCING 1.
     MOVE SPACES TO PR9-REC.
     MOVE "3" TO SUMMARY-SUBRPT.
     MOVE REPORT-TITLE-3A TO SUMMARY-SUBTITLE.
     MOVE RPT3-CNT TO SUM-CNT.
     MOVE RPT3-WAGES TO SUM-WAGE.
     MOVE RPT3-BAL TO SUM-BAL.
     WRITE PR9-REC FROM SUMMARY-LINE AFTER ADVANCING 1.
     MOVE "4" TO SUMMARY-SUBRPT.
     MOVE REPORT-TITLE-4A TO SUMMARY-SUBTITLE.
     MOVE RPT4-CNT TO SUM-CNT.
     MOVE RPT4-WAGES TO SUM-WAGE.
     MOVE RPT4-BAL TO SUM-BAL.
     WRITE PR9-REC FROM SUMMARY-LINE AFTER ADVANCING 1.
     MOVE "5" TO SUMMARY-SUBRPT.
     MOVE REPORT-TITLE-5A TO SUMMARY-SUBTITLE.
     MOVE RPT5-CNT TO SUM-CNT.
     MOVE RPT5-WAGES TO SUM-WAGE.
     MOVE RPT5-BAL TO SUM-BAL.
     WRITE PR9-REC FROM SUMMARY-LINE AFTER ADVANCING 1.
     MOVE "TERMINATED: " TO PR9-REC.
     WRITE PR9-REC AFTER ADVANCING 2.
     MOVE SPACES TO PR9-REC.
     WRITE PR9-REC AFTER ADVANCING 1.
     MOVE "6" TO SUMMARY-SUBRPT.
     MOVE REPORT-TITLE-6A TO SUMMARY-SUBTITLE.
     MOVE RPT6-CNT TO SUM-CNT.
     MOVE RPT6-WAGES TO SUM-WAGE.
     MOVE RPT6-BAL TO SUM-BAL.
     WRITE PR9-REC FROM SUMMARY-LINE AFTER ADVANCING 1.
     MOVE "7" TO SUMMARY-SUBRPT.
     MOVE REPORT-TITLE-7A TO SUMMARY-SUBTITLE.
     MOVE RPT7-CNT TO SUM-CNT.
     MOVE RPT7-WAGES TO SUM-WAGE.
     MOVE RPT7-BAL TO SUM-BAL.
     WRITE PR9-REC FROM SUMMARY-LINE AFTER ADVANCING 1.
     MOVE "8" TO SUMMARY-SUBRPT.
     MOVE REPORT-TITLE-8A TO SUMMARY-SUBTITLE.
     MOVE RPT8-CNT TO SUM-CNT.
     MOVE RPT8-WAGES TO SUM-WAGE.
     MOVE RPT8-BAL TO SUM-BAL.
     WRITE PR9-REC FROM SUMMARY-LINE AFTER ADVANCING 1.
     MOVE "X" TO SUMMARY-SUBRPT.
     MOVE "<  AGE 18 " TO SUMMARY-SUBTITLE-FIRST.
     MOVE "NO WAGES :" TO SUMMARY-SUBTITLE-DESC.
     MOVE UNDR18-CNT-NWAGE TO SUMMARY-NOWAGES.
     MOVE SUMMARY-SHORT TO SUMMARY-SUBTITLE.
     MOVE UNDR18-CNT TO SUM-CNT.
     MOVE UNDR18-WAGES TO SUM-WAGE.
     MOVE UNDR18-BAL TO SUM-BAL.
     ADD UNDR18-CNT TO RPT-TOT.
     ADD UNDR18-WAGES TO RPT-WAGES.
     ADD UNDR18-BAL TO RPT-BAL.
*    MOVE 0 TO SUM-BAL.
     WRITE PR9-REC FROM SUMMARY-LINE AFTER ADVANCING 1.
     ADD UNDR18-CNT-NWAGE TO RPT9-CNT-NWAGE.
     MOVE "N" TO SUMMARY-SUBRPT.
     MOVE "NON-EMPLOYEE BENEFICIARIES" TO SUMMARY-SUBTITLE.
     MOVE RPT10-CNT TO SUM-CNT.
     MOVE RPT10-WAGES TO SUM-WAGE.
     MOVE RPT10-BAL TO SUM-BAL.
     WRITE PR9-REC FROM SUMMARY-LINE AFTER ADVANCING 1.
     MOVE " " TO SUMMARY-SUBRPT.
     MOVE "TOTAL ALL REPORTS:" TO SUMMARY-SUBTITLE.
     MOVE RPT-TOT TO SUM-CNT.
     MOVE RPT-WAGES TO SUM-WAGE.
     MOVE RPT-BAL TO SUM-BAL.
     WRITE PR9-REC FROM SUMMARY-LINE AFTER ADVANCING 3.
     MOVE RPT9-CNT-NWAGE TO SUM-CNT-NWAGE.
     MOVE RPT9-WAGES-NWAGE TO SUM-WAGE-NWAGE.
     MOVE RPT9-BAL-NWAGE TO SUM-BAL-NWAGE.
     WRITE PR9-REC FROM SUMMARY-LINE-NWAGE AFTER ADVANCING 2.
 466-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 500-CALC-YEARS.

     IF WS-START-DD < WS-END-DD
        IF WS-START-MM = 4 OR 6 OR 9 OR 11
           ADD 30 TO WS-START-DD
           SUBTRACT 1 FROM WS-START-MM
        ELSE
           IF WS-START-MM = 3 OR 5 OR 7 OR 8 OR 10 OR 12
              ADD 31 TO WS-START-DD
              SUBTRACT 1 FROM WS-START-MM
           ELSE
              IF WS-START-MM = 1
                  ADD 31 TO WS-START-DD
                  MOVE 12 TO WS-START-MM
                  SUBTRACT 1 FROM WS-START-CCYY
              ELSE
                  COMPUTE LEAP-YEAR = WS-START-CCYY / 4
                  IF LEAP-REM = 0
                      ADD 29 TO WS-START-DD
                  ELSE
                      ADD 28 TO WS-START-DD
                  END-IF
                  SUBTRACT 1 FROM WS-START-MM
              END-IF
           END-IF
        END-IF
     END-IF.
     COMPUTE CALC-DAYS = WS-START-DD - WS-END-DD.
     IF WS-START-MM < WS-END-MM
         ADD 12 TO WS-START-MM
         SUBTRACT 1 FROM WS-START-CCYY
     END-IF.
     COMPUTE CALC-MONTHS = WS-START-MM - WS-END-MM.
     COMPUTE CALC-YEARS = WS-START-CCYY - WS-END-CCYY.

 500-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 510-ERR-RPT.
     IF ELN-CNT > 58
         PERFORM 520-ERR-HEAD THRU 520-EXIT
     END-IF.
     WRITE PR9-REC FROM ERR-LINE AFTER ADVANCING 1.
     ADD 1 TO ERR-CNT ELN-CNT.
 510-EXIT.
     EXIT.
 520-ERR-HEAD.
     ADD 1 TO EPG-CNT.
     MOVE EPG-CNT TO ERR-PAGE.
     MOVE 0 TO ELN-CNT.
     WRITE PR9-REC FROM ERR-HEAD1 AFTER ADVANCING PAGE.
     MOVE SPACES TO PR9-REC.
     WRITE PR9-REC FROM ERR-HEAD2 AFTER ADVANCING 2.
     MOVE SPACES TO PR9-REC.
     WRITE PR9-REC AFTER ADVANCING 1.
     ADD 4 TO ELN-CNT.
 520-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*  REPORT 1: ALL ACTIVE AND INACTIVE EMPLOYEES AGE 18-20 
*            WITH >= 1000 PS HRS
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
* 

 530-REPORT1.
     IF LN1-CNT > 58
         PERFORM 535-HEADING THRU 535-EXIT
     END-IF.
     WRITE PR1-REC FROM DETAIL-LINE AFTER ADVANCING 1.
     ADD 1 TO LN1-CNT.
 530-EXIT.
     EXIT.
 535-HEADING.
     ADD 1 TO PG1-CNT.
     MOVE PG1-CNT TO HD-PAGE.
     MOVE 0 TO LN1-CNT.
     WRITE PR1-REC FROM HEADING-1 AFTER ADVANCING PAGE.
     WRITE PR1-REC FROM REPORT-TITLE-1 AFTER ADVANCING 2.
     WRITE PR1-REC FROM HEADING-2 AFTER ADVANCING 2.
     MOVE SPACES TO PR1-REC.
     WRITE PR1-REC AFTER ADVANCING 2.
     ADD 7 TO LN1-CNT.
 535-EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*  REPORT 2: ALL ACTIVE AND INACTIVE EMPLOYEES AGE >= 21 
*            WITH >= 1000 PS HRS
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 540-REPORT2.
     IF LN2-CNT > 58
        PERFORM 545-HEADING THRU 545-EXIT
     END-IF.
     WRITE PR2-REC FROM DETAIL-LINE AFTER ADVANCING 1.
     ADD 1 TO LN2-CNT.
 540-EXIT.
     EXIT.
 545-HEADING.
     ADD 1 TO PG2-CNT.
     MOVE PG2-CNT TO HD-PAGE.
     MOVE 0 TO LN2-CNT.
     WRITE PR2-REC FROM HEADING-1 AFTER ADVANCING PAGE.
     WRITE PR2-REC FROM REPORT-TITLE-2 AFTER ADVANCING 2.
     WRITE PR2-REC FROM HEADING-2 AFTER ADVANCING 2.
     MOVE SPACES TO PR2-REC.
     WRITE PR2-REC AFTER ADVANCING 2.
     ADD 7 TO LN2-CNT.
 545-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*  REPORT 3: ALL ACTIVE AND INACTIVE EMPLOYESS AGE < 18
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 550-REPORT3.
     IF LN3-CNT > 58
        PERFORM 555-HEADING THRU 555-EXIT
     END-IF.
     WRITE PR3-REC FROM DETAIL-LINE AFTER ADVANCING 1.
     ADD 1 TO LN3-CNT.
 550-EXIT.
     EXIT.
 555-HEADING.
     ADD 1 TO PG3-CNT.
     MOVE PG3-CNT TO HD-PAGE.
     MOVE 0 TO LN3-CNT.
     WRITE PR3-REC FROM HEADING-1 AFTER ADVANCING PAGE.
     WRITE PR3-REC FROM REPORT-TITLE-3 AFTER ADVANCING 2.
     WRITE PR3-REC FROM HEADING-2 AFTER ADVANCING 2.
     MOVE SPACES TO PR3-REC.
     WRITE PR3-REC AFTER ADVANCING 2.
     ADD 7 TO LN3-CNT.
 555-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*  REPORT 4: ALL ACTIVE AND INACTIVE EMPLOYEES AGE >= 18 
*            WITH < 1000 PS HOURS AND PRIOR PS AMOUNT
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 560-REPORT4.
     IF LN4-CNT > 58
        PERFORM 565-HEADING THRU 565-EXIT
     END-IF.
     WRITE PR4-REC FROM DETAIL-LINE AFTER ADVANCING 1
     ADD 1 TO LN4-CNT.
 560-EXIT.
     EXIT.
 565-HEADING.
     ADD 1 TO PG4-CNT.
     MOVE PG4-CNT TO HD-PAGE.
     MOVE 0 TO LN4-CNT.
     WRITE PR4-REC FROM HEADING-1 AFTER ADVANCING PAGE.
     WRITE PR4-REC FROM REPORT-TITLE-4 AFTER ADVANCING 2.
     WRITE PR4-REC FROM HEADING-2 AFTER ADVANCING 2.
     MOVE SPACES TO PR4-REC.
     WRITE PR4-REC AFTER ADVANCING 2.
     ADD 7 TO LN4-CNT.
 565-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*  REPORT 5:  ALL ACTIVE AND INACTIVE EMPLOYEES AGE >= 18 
*             WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 570-REPORT5.
     IF LN5-CNT > 58
        PERFORM 575-HEADING THRU 575-EXIT
     END-IF.
     WRITE PR5-REC FROM DETAIL-LINE AFTER ADVANCING 1.
     ADD 1 TO LN5-CNT.
 570-EXIT.
     EXIT.
 575-HEADING.
     ADD 1 TO PG5-CNT.
     MOVE PG5-CNT TO HD-PAGE.
     MOVE 0 TO LN5-CNT.
     WRITE PR5-REC FROM HEADING-1 AFTER ADVANCING PAGE.
     WRITE PR5-REC FROM REPORT-TITLE-5 AFTER ADVANCING 2.
     WRITE PR5-REC FROM HEADING-2 AFTER ADVANCING 2.
     MOVE SPACES TO PR5-REC.
     WRITE PR5-REC AFTER ADVANCING 2.
     ADD 7 TO LN5-CNT.
 575-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*  REPORT 6:  ALL TERMINATED EMPLOYEES AGE >= 18 
*             WITH >= 1000 PS HRS**
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 580-REPORT6.
     IF LN6-CNT > 58
        PERFORM 585-HEADING THRU 585-EXIT
     END-IF.
     WRITE PR6-REC FROM DETAIL-LINE AFTER ADVANCING 1.
     ADD 1 TO LN6-CNT.
 580-EXIT.
     EXIT.
 585-HEADING.
     ADD 1 TO PG6-CNT.
     MOVE PG6-CNT TO HD-PAGE.
     MOVE 0 TO LN6-CNT.
     WRITE PR6-REC FROM HEADING-1 AFTER ADVANCING PAGE.
     WRITE PR6-REC FROM REPORT-TITLE-6 AFTER ADVANCING 2.
     WRITE PR6-REC FROM HEADING-2 AFTER ADVANCING 2.
     MOVE SPACES TO PR6-REC.
     WRITE PR6-REC AFTER ADVANCING 2.
     ADD 7 TO LN6-CNT.
 585-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*  REPORT 7:  ALL TERMINATED EMPLOYEES AGE >= 18 
*             WITH < 1000 PS HRS AND NO PRIOR PS AMOUNT
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 590-REPORT7.
     IF LN7-CNT > 58
        PERFORM 592-HEADING THRU 592-EXIT
     END-IF.
     WRITE PR7-REC FROM DETAIL-LINE AFTER ADVANCING 1.
     ADD 1 TO LN7-CNT.
 590-EXIT.
     EXIT.
 592-HEADING.
     ADD 1 TO PG7-CNT.
     MOVE PG7-CNT TO HD-PAGE.
     MOVE 0 TO LN7-CNT.
     WRITE PR7-REC FROM HEADING-1 AFTER ADVANCING PAGE.
     WRITE PR7-REC FROM REPORT-TITLE-7 AFTER ADVANCING 2.
     WRITE PR7-REC FROM HEADING-2 AFTER ADVANCING 2.
     MOVE SPACES TO PR7-REC.
     WRITE PR7-REC AFTER ADVANCING 2.
     ADD 7 TO LN7-CNT.
 592-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*  REPORT 8:  ALL TERMINATED EMPLOYEES AGE >= 18 
*             WITH < 1000 PS HRS AND HAS A PRIOR PS AMOUNT 
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 594-REPORT8.
     IF LN8-CNT > 58
        PERFORM 596-HEADING THRU 596-EXIT
     END-IF.
     WRITE PR8-REC FROM DETAIL-LINE AFTER ADVANCING 1.
     ADD 1 TO LN8-CNT.
 594-EXIT.
     EXIT.
 596-HEADING.
     ADD 1 TO PG8-CNT.
     MOVE PG8-CNT TO HD-PAGE.
     MOVE 0 TO LN8-CNT.
     WRITE PR8-REC FROM HEADING-1 AFTER ADVANCING PAGE.
     WRITE PR8-REC FROM REPORT-TITLE-8 AFTER ADVANCING 2.
     WRITE PR8-REC FROM HEADING-2 AFTER ADVANCING 2.
     MOVE SPACES TO PR8-REC.
     WRITE PR8-REC AFTER ADVANCING 2.
     ADD 7 TO LN8-CNT.
 596-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*  REPORT 10: ALL NON-EMPLOYEE BENEFICIARIES
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 597-REPORT10.
     IF LN10-CNT > 56
        PERFORM 598-HEADING THRU 598-EXIT
     END-IF.
     WRITE PR10-REC FROM DETAIL-LINE AFTER ADVANCING 1.
     ADD 1 TO LN10-CNT.
 597-EXIT.
     EXIT.
 598-HEADING.
     ADD 1 TO PG10-CNT.
     MOVE PG10-CNT TO HD-PAGE.
     MOVE 0 TO LN10-CNT.
     WRITE PR10-REC FROM HEADING-1 AFTER ADVANCING PAGE.
     WRITE PR10-REC FROM REPORT-TITLE-10 AFTER ADVANCING 2.
     WRITE PR10-REC FROM HEADING-2 AFTER ADVANCING 2.
     MOVE SPACES TO PR10-REC.
     WRITE PR10-REC AFTER ADVANCING 2.
     ADD 7 TO LN10-CNT.
 598-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 600-CHECK-PROFIT.
     MOVE S-SSN TO SOC-SEC-NUMBER.
     CALL "MSTR-FIND-ANY-SOC-SEC".
     INITIALIZE WS-HOLD-INIT-CONT.
     IF DB-STATUS = ZEROS
         MOVE SPACES TO IDS2-REC-NAME
         CALL "MSTR-GET-REC" USING IDS2-REC-NAME
         MOVE "FIRST" TO IDS2-NAVIGATION-MODE
         MOVE "PR-DET" TO IDS2-REC-NAME
         CALL "MSTR-FIND-WITHIN-PR-DET-S" USING IDS2-NAVIGATION-MODE
                                                               IDS2-REC-NAME
         IF DB-STATUS = ZEROS
            MOVE SPACES TO IDS2-REC-NAME
            CALL "MSTR-GET-REC" USING IDS2-REC-NAME
            MOVE 1 TO PROFIT-SHARE-SWITCH
            MOVE PROFIT-YEAR TO WS-HOLD-INIT-CONT
            GO TO 600-EXIT
         ELSE
            MOVE 0 TO PROFIT-SHARE-SWITCH
            GO TO 600-EXIT
        END-IF
     END-IF.
     MOVE 0 TO PROFIT-SHARE-SWITCH.
 600-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 610-UPDATE-PAYPROFIT.
     MOVE S-EMP-NUMBER TO PAYPROF-BADGE.
     COPY GAC-CHECK-800.
     PERFORM 710-READ-PAYPROFIT THRU 710-EXIT.
     IF S-EMP-NUMBER > 0
        IF INVALID-PAYPROFIT-READ 
            MOVE "PAY426N" TO DAEMON-DISP-PROG
            MOVE SPACES TO DAEMON-DISP-MSG
            STRING  "** PAY426N - CAN NOT FIND EMP "
                   S-EMP-NUMBER
                 DELIMITED SIZE INTO DAEMON-DISP-MSG
            CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
            MOVE "PAY426N" TO DAEMON-DISP-PROG
            MOVE "**         - THIS RECORD IS BYPASSED" TO DAEMON-DISP-MSG
            CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
            MOVE "PAY426N" TO DAEMON-DISP-PROG
            MOVE "**         - NOTIFY OPERATIONS MGR OF PROBLEM" TO DAEMON-DISP-MSG
            CALL "DISPCONS" USING DAEMON-DISP-DISPLAY
            MOVE 1 TO WS-ABORT-INDICATOR
            GO TO 649-EXIT
        END-IF
     END-IF.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 612-CALC-POINTS.
     IF (S-NEW-EMP = ZERO) AND
        (NO-PROFIT-SHARE)
         MOVE 1 TO PY-PROF-NEWEMP
         IF S-REPORT-CODE = 1
            ADD 1 TO RPT1-NEW-CNT
         END-IF
         IF S-REPORT-CODE = 2
            ADD 1 TO RPT2-NEW-CNT
         END-IF
         IF S-REPORT-CODE = 3
            ADD 1 TO RPT3-NEW-CNT
         END-IF
         IF S-REPORT-CODE = 4
            ADD 1 TO RPT4-NEW-CNT
         END-IF
         IF S-REPORT-CODE = 5
            ADD 1 TO RPT5-NEW-CNT
         END-IF
         IF S-REPORT-CODE = 6
            ADD 1 TO RPT6-NEW-CNT
         END-IF
         IF S-REPORT-CODE = 7
            ADD 1 TO RPT7-NEW-CNT
         END-IF
         IF S-REPORT-CODE = 8
            ADD 1 TO RPT8-NEW-CNT
         END-IF
     END-IF.

***************************************************************************
** DPRUGH 11/06/02 P#144600  ADDED NEW PROFIT SHARING CHECKS
***************************************************************************
     IF S-AGE > 64
         IF S-OVER-65-SW = 0 AND PY-PS-YEARS = 0
             GO TO 640-65PLUS
     END-IF.
     IF S-AGE > 64
         IF S-HRS > 0 AND S-OVER-65-SW = 0
             IF PY-PS-YEARS = 1
                MOVE 4 TO PY-PROF-ZEROCONT
                GO TO 640-65PLUS
             ELSE
                IF PY-PS-YEARS > 1
                   MOVE 3 TO PY-PROF-ZEROCONT
                   GO TO 640-65PLUS
                END-IF
             END-IF
         END-IF
     END-IF.
     IF S-AGE > 64
        IF S-OVER-65-SW = 1
           IF PY-PS-YEARS = 1
              MOVE 4 TO PY-PROF-ZEROCONT
              GO TO 620-CALCULATE-POINTS
           ELSE
              IF PY-PS-YEARS = 0
                 MOVE 5 TO PY-PROF-ZEROCONT
                 GO TO 620-CALCULATE-POINTS
              END-IF
           END-IF
        END-IF
     END-IF.

     IF (S-REPORT-CODE = 4 OR 5) AND
        (S-HRS > 0)
          MOVE 2 TO PY-PROF-ZEROCONT
          GO TO 640-65PLUS
     END-IF.
 620-CALCULATE-POINTS.
     DIVIDE S-WAGES BY 100 GIVING WS-POINTS
            REMAINDER  WS-REMAINDER.
     IF WS-REMAINDER < 50
         NEXT SENTENCE
     ELSE
         ADD 1 TO WS-POINTS
     END-IF.
     MOVE WS-POINTS TO PY-PROF-POINTS.
     ADD WS-POINTS TO TOT-POINTS.

     IF S-REPORT-CODE = 1
         ADD WS-POINTS TO RPT1-POINTS
     ELSE
        IF S-REPORT-CODE = 2
            ADD WS-POINTS TO RPT2-POINTS
        ELSE
             IF S-REPORT-CODE = 3
                 ADD WS-POINTS TO RPT3-POINTS
             ELSE
                 IF S-REPORT-CODE = 4
                     ADD WS-POINTS TO RPT4-POINTS
                 ELSE
                     IF S-REPORT-CODE = 5
                         ADD WS-POINTS TO RPT5-POINTS
                     ELSE
                         IF S-REPORT-CODE = 6
                             ADD WS-POINTS TO RPT6-POINTS
                         ELSE
                             IF S-REPORT-CODE = 7
                                 ADD WS-POINTS TO RPT7-POINTS
                             ELSE
                                IF S-REPORT-CODE = 8
                                   ADD WS-POINTS TO RPT7-POINTS
                                ELSE
                                   ADD WS-POINTS TO RPT8-POINTS
                                END-IF
                             END-IF
                         END-IF
                     END-IF
                 END-IF
             END-IF
        END-IF
     END-IF.
     IF PY-PROF-POINTS > ZEROS
         MOVE 1 TO PY-PROF-CERT
     END-IF.
 640-65PLUS.
     MOVE PAYPROF-SSN TO WS-SOCSECNUM.
     INITIALIZE WS-INITIAL-CONTRIBUTION.
     IF PY-SCOD = "A" OR "I"
        PERFORM 889-CHECK-INITIAL-CONTRIBUTION THRU 889-EXIT
     END-IF.

     IF WS-HOLD-INIT-CONT > 0 AND WS-INITIAL-CONTRIBUTION = 0
        MOVE WS-HOLD-INIT-CONT TO WS-INITIAL-CONTRIBUTION
     END-IF.
     IF WS-INITIAL-CONTRIBUTION > 0
        COMPUTE WS-YEARS-SINCE = WS-CURR-CCYY - WS-INITIAL-CONTRIBUTION
        IF WS-YEARS-SINCE >= 5
           IF PY-PROF-ZEROCONT NOT = 6
              MOVE 6 TO PY-PROF-ZEROCONT
           END-IF
        END-IF
     END-IF.
 649-EXIT.
     EXIT.
*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 700-OPEN-FILES.
     MOVE "INPUT" TO UFAS-OPEN-MODE
     IF YEAREND = "Y"
        CALL "OPEN-DEMO-PROFSHARE" USING DEMOGRAPHICS-FILE-STATUS 
                                         UFAS-OPEN-MODE
     ELSE
        CALL "OPEN-DEMOGRAPHICS" USING DEMOGRAPHICS-FILE-STATUS 
                                         UFAS-OPEN-MODE
     END-IF
     CALL "OPEN-PAYPROFIT" USING PAYPROFIT-FILE-STATUS UFAS-OPEN-MODE
     CALL "OPEN-PAYBEN" USING PAYBEN-FILE-STATUS UFAS-OPEN-MODE
     CALL "OPEN-CALENDAR1" USING CALDAR-FILE-STATUS UFAS-OPEN-MODE.
     OPEN OUTPUT PRINTFL9.
 700-EXIT.
     EXIT.
 702-CLOSE-FILES.
     IF YEAREND = "Y"
        CALL "CLOSE-DEMO-PROFSHARE" USING DEMOGRAPHICS-FILE-STATUS 
     ELSE
        CALL "CLOSE-DEMOGRAPHICS"   USING DEMOGRAPHICS-FILE-STATUS 
     END-IF.
     CALL "CLOSE-PAYPROFIT" USING PAYPROFIT-FILE-STATUS          
     CALL "CLOSE-PAYBEN" USING PAYBEN-FILE-STATUS
     CALL "CLOSE-CALENDAR1" USING CALDAR-FILE-STATUS.
 702-EXIT.
     EXIT.
 704-OPEN.
     MOVE "INPUT" TO UFAS-OPEN-MODE
     IF YEAREND = "Y"
        CALL "OPEN-DEMO-PROFSHARE" USING DEMOGRAPHICS-FILE-STATUS 
                                         UFAS-OPEN-MODE
     ELSE
        CALL "OPEN-DEMOGRAPHICS"   USING DEMOGRAPHICS-FILE-STATUS 
                                         UFAS-OPEN-MODE
     END-IF.
     MOVE "I-O" TO UFAS-OPEN-MODE
     CALL "OPEN-PAYPROFIT" USING PAYPROFIT-FILE-STATUS UFAS-OPEN-MODE.
     CALL "OPEN-PAYBEN" USING PAYBEN-FILE-STATUS UFAS-OPEN-MODE
     OPEN OUTPUT PRINTFL1 PRINTFL2 PRINTFL3 PRINTFL4
                 PRINTFL5 PRINTFL6 PRINTFL7 PRINTFL8 PRINTFL10
     MOVE "0000000" TO DB-STATUS.
 704-EXIT.
     EXIT.
 706-CLOSE-FILES.
     CLOSE PRINTFL1
     CLOSE PRINTFL2
     CLOSE PRINTFL3
     CLOSE PRINTFL4
     CLOSE PRINTFL5
     CLOSE PRINTFL6
     CLOSE PRINTFL7
     CLOSE PRINTFL8
     CLOSE PRINTFL9
     CLOSE PRINTFL10
     CALL "CLOSE-PAYPROFIT" USING PAYPROFIT-FILE-STATUS          .
     CALL "CLOSE-PAYBEN" USING PAYBEN-FILE-STATUS
     IF YEAREND = "Y"
        CALL "CLOSE-DEMO-PROFSHARE" USING DEMOGRAPHICS-FILE-STATUS 
     ELSE
        CALL "CLOSE-DEMOGRAPHICS"   USING DEMOGRAPHICS-FILE-STATUS 
     END-IF. 
     MOVE "0000000" TO DB-STATUS.
 706-EXIT.
     EXIT.

*
* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*

 708-START-PAYPROFIT.
     MOVE 0 TO WS-PAYPROFIT-START-SW.
     MOVE "PAYPROF-SSN-KEY" TO UFAS-ALT-KEY-NAME
     CALL "START-ALT-SUP-PAYPROFIT" USING PAYPROFIT-FILE-STATUS          
                                        UFAS-ALT-KEY-NAME
                                        PAYPROF-REC.
     IF PAYPROFIT-FILE-STATUS           NOT = "00"
        MOVE 1 TO WS-PAYPROFIT-START-SW
     END-IF.
 708-EXIT.  EXIT.

 710-READ-PAYPROFIT.
     MOVE 0 TO WS-PAYPROFIT-READ-SW.
     CALL "READ-KEY-PAYPROFIT" USING PAYPROFIT-FILE-STATUS          
                                     PAYPROF-REC.
     IF PAYPROFIT-FILE-STATUS           NOT = "00"
        MOVE 1 TO WS-PAYPROFIT-READ-SW
        GO TO 710-EXIT
     END-IF.
     PERFORM 714-GET-DEMOGRAPHICS THRU 714-EXIT.
     IF DEMOGRAPHICS-FILE-STATUS NOT = "00"
        GO TO 710-EXIT
     END-IF.

 710-EXIT.
     EXIT.

 712-READ-PAYPROFIT-NEXT.
     MOVE 0 TO WS-PAYPROFIT-READ-NEXT-SW.
     CALL "READ-NEXT-PAYPROFIT" USING PAYPROFIT-FILE-STATUS          
                                      PAYPROF-REC.
     IF PAYPROFIT-FILE-STATUS           NOT = "00"
        MOVE 1 TO WS-PAYPROFIT-READ-NEXT-SW
        GO TO 712-EXIT
     END-IF.
     PERFORM 714-GET-DEMOGRAPHICS THRU 714-EXIT.
 712-EXIT.
     EXIT.

 714-GET-DEMOGRAPHICS.
     MOVE PAYPROF-BADGE TO DEM-BADGE.
     IF YEAREND = "Y"
        CALL "READ-KEY-DEMO-PROFSHARE" USING DEMOGRAPHICS-FILE-STATUS
                                             DEM-REC 
     ELSE
        CALL "READ-KEY-DEMOGRAPHICS"   USING DEMOGRAPHICS-FILE-STATUS
                                             DEM-REC 
     END-IF.
     IF DEMOGRAPHICS-FILE-STATUS NOT = "00"
        DISPLAY "FAILED TO GET DEMOGRAPHICS FOR BADGE " DEM-BADGE
        DISPLAY "DEMOGRAPHICS-FILE-STATUS: " DEMOGRAPHICS-FILE-STATUS
        ADD 1 TO WS-SKIPPED
     END-IF.
  
 714-EXIT.
     EXIT.

 COPY COPY-INIT-CONT.
 COPY GAC-CALL.
