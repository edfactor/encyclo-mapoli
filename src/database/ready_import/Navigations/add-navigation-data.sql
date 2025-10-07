
DECLARE

    -- Process for adding a new menu item:
    -- 1. If you need a new menu, pleast add that first in the MENUS section
    -- 2. Add a new constant for the new page from 1xx and up
    -- 3. Decide what the parent menu is going to be out of the existing constants
    -- 4. Add the insert_navigation_item call to add the new item to the parent menu
    -- 5. Add the assign_navigation_role calls to assign roles to the new item


    -- MENUS (ids from 1 to 99)

    -- This isn't a menu, it is a container for top-level items
    TOP_LEVEL_MENU CONSTANT NUMBER := NULL;
    -- These are the nav bar top-level menus
    INQUIRIES_MENU CONSTANT NUMBER := 2;
    BENEFICIARIES_MENU CONSTANT NUMBER := 3;
    DISTRIBUTIONS_MENU CONSTANT NUMBER := 4;
    RECONCILIATION_MENU CONSTANT NUMBER := 5;
    YEAR_END_MENU CONSTANT NUMBER := 6;
    IT_DEVOPS_MENU CONSTANT NUMBER := 7;


    -- These are the secondary drawer top-level menus
    FISCAL_CLOSE CONSTANT NUMBER := 8;
    DECEMBER_ACTIVITIES CONSTANT NUMBER := 9;
    
    -- Third-level under December Activities
    CLEANUP_REPORTS CONSTANT NUMBER := 10;
    -- Third-level under Fiscal Close
    PROF_SHARE_REPORT_BY_AGE CONSTANT NUMBER := 11;
    PROF_SHARE_BY_STORE CONSTANT NUMBER := 12;

    -- AVAILABLE PAGES (ids starting at 100)

    MASTER_INQUIRY_PAGE CONSTANT NUMBER := 100;
    ADJUSTMENTS_PAGE CONSTANT NUMBER := 101;
    DEMOGRAPHIC_FREEZE_PAGE CONSTANT NUMBER := 102;
    DISTRIBUTION_INQUIRY_PAGE CONSTANT NUMBER := 103;
    MANAGE_EXECUTIVE_HOURS_PAGE CONSTANT NUMBER := 104;
    YTD_WAGES_EXTRACT CONSTANT NUMBER := 105;
    FORFEITURES CONSTANT NUMBER := 106;
    DISTRIBUTIONS_AND_FORFEITURES CONSTANT NUMBER := 107;
    PROFIT_SHARE_REPORT CONSTANT NUMBER := 108;
    GET_ELIGIBLE_EMPLOYEES CONSTANT NUMBER := 109;
    PROFIT_SHARE_FORFEIT CONSTANT NUMBER := 110;
    MASTER_UPDATE CONSTANT NUMBER := 111;
    PROFIT_MASTER_UPDATE CONSTANT NUMBER := 112;
    PAYMASTER_UPDATE CONSTANT NUMBER := 113;
    PROF_CONTROL_SHEET CONSTANT NUMBER := 114;
    DISTRIBUTIONS_BY_AGE CONSTANT NUMBER := 115;
    CONTRIBUTIONS_BY_AGE CONSTANT NUMBER := 116;
    FORFEITURES_BY_AGE CONSTANT NUMBER := 117;
    BALANCE_BY_AGE CONSTANT NUMBER := 118;
    VESTED_AMOUNTS_BY_AGE CONSTANT NUMBER := 119;
    BALANCE_BY_YEARS CONSTANT NUMBER := 120;
    PROF_SHARE_GROSS_RPT CONSTANT NUMBER := 121;
    UNDER_21_REPORT CONSTANT NUMBER := 122;
    QPAY066_UNDR21 CONSTANT NUMBER := 123;
    QPAY066TA_UNDR21 CONSTANT NUMBER := 124;
    QPAY066B CONSTANT NUMBER := 125;
    QPAY066TA CONSTANT NUMBER := 126;
    QNEWPROFLBL CONSTANT NUMBER := 127;
    PROFNEW CONSTANT NUMBER := 128;
    PROFALL CONSTANT NUMBER := 129;
    REPRINT_CERTIFICATES CONSTANT NUMBER := 130;
    SAVE_PROF_PAYMSTR CONSTANT NUMBER := 131;
    QPAY066_AD_HOC_REPORTS CONSTANT NUMBER := 132;
    RECENTLY_TERMINATED CONSTANT NUMBER := 133;
    PAY_BENEFICIARY_REPORT CONSTANT NUMBER := 134;
    ADHOC_BENEFICIARIES_REPORT CONSTANT NUMBER := 135;
    TERMINATED_LETTERS CONSTANT NUMBER := 136;
    QPAY600 CONSTANT NUMBER := 137;
    PAY426N_LIVE CONSTANT NUMBER := 138;
    PROFIT_SUMMARY CONSTANT NUMBER := 139;
    PAY426_2 CONSTANT NUMBER := 140;
    PAY426_3 CONSTANT NUMBER := 141;
    DEMOGRAPHIC_BADGES_NOT_IN_PAYPROFIT CONSTANT NUMBER := 142;
    DUPLICATE_SSNS_DEMOGRAPHICS CONSTANT NUMBER := 143;
    NEGATIVE_ETVA CONSTANT NUMBER := 144;
    TERMINATIONS CONSTANT NUMBER := 145;
    DUPLICATE_NAMES_BIRTHDAYS CONSTANT NUMBER := 146;
    MILITARY_CONTRIBUTIONS CONSTANT NUMBER := 147;
    UNFORFEIT CONSTANT NUMBER := 148;

    PROFIT_SHARE_REPORT_FINAL_RUN CONSTANT NUMBER := 149;
    PRINT_PROFIT_CERTS CONSTANT NUMBER := 150;
    PROFIT_SHARE_REPORT_EDIT_RUN CONSTANT NUMBER := 151;
    PAY_BEN_REPORT CONSTANT NUMBER := 152;
    PAY426N_FROZEN CONSTANT NUMBER := 153;
    PROFIT_DETAILS_REVERSAL CONSTANT NUMBER := 154;
    PRINT_PS_JOBS CONSTANT NUMBER :=155;


    --- These are the role IDs from the ROLES table
    SYSTEM_ADMINISTRATOR CONSTANT NUMBER := 1;
    FINANCE_MANAGER CONSTANT NUMBER := 2;
    DISTRIBUTIONS_CLERK CONSTANT NUMBER := 3;
    HARDSHIP_ADMINISTRATOR CONSTANT NUMBER := 4;
    IMPERSONATION CONSTANT NUMBER := 5;
    IT_DEVOPS CONSTANT NUMBER := 6;
    IT_OPERATIONS CONSTANT NUMBER := 7;
    EXECUTIVE_ADMINISTRATOR CONSTANT NUMBER := 8;
    AUDITOR CONSTANT NUMBER := 9;
    BENEFICIARY_ADMINISTRATOR CONSTANT NUMBER := 10;

    IS_NAVIGABLE CONSTANT NUMBER := 1;
    NOT_NAVIGABLE CONSTANT NUMBER := 0;
    DISABLED CONSTANT NUMBER := 1;
    ENABLED CONSTANT NUMBER := 0;
    
    STATUS_NORMAL CONSTANT NUMBER := 1;
    
    -- These are order numbers to help with readability in calls
    ORDER_FIRST CONSTANT NUMBER := 1;
    ORDER_SECOND CONSTANT NUMBER := 2;
    ORDER_THIRD CONSTANT NUMBER := 3;
    ORDER_FOURTH CONSTANT NUMBER := 4;
    ORDER_FIFTH CONSTANT NUMBER := 5;
    ORDER_SIXTH CONSTANT NUMBER := 6;
    ORDER_SEVENTH CONSTANT NUMBER := 7;
    ORDER_EIGHTH CONSTANT NUMBER := 8;
    ORDER_NINTH CONSTANT NUMBER := 9;
    ORDER_TENTH CONSTANT NUMBER := 10;
    ORDER_ELEVENTH CONSTANT NUMBER := 11;
    ORDER_TWELFTH CONSTANT NUMBER := 12;
    ORDER_THIRTEENTH CONSTANT NUMBER := 13;
    ORDER_FOURTEENTH CONSTANT NUMBER := 14;
    ORDER_FIFTEENTH CONSTANT NUMBER := 15;
    ORDER_SIXTEENTH CONSTANT NUMBER := 16;
    ORDER_SEVENTEENTH CONSTANT NUMBER := 17;
    ORDER_EIGHTEENTH CONSTANT NUMBER := 18;
    ORDER_NINETEENTH CONSTANT NUMBER := 19;
    ORDER_TWENTIETH CONSTANT NUMBER := 20;
    ORDER_NINETY_NINTH CONSTANT NUMBER := 99;

    -- These are the helper functions to reduce boilerplate
    -- and reduce errors

    
    PROCEDURE insert_navigation_item(
        p_id NUMBER,
        p_parent_id NUMBER,
        p_title VARCHAR2,
        p_sub_title VARCHAR2,
        p_url VARCHAR2,
        p_status_id NUMBER,
        p_order_number NUMBER,
        p_icon VARCHAR2,
        p_disabled NUMBER,
        p_is_navigable NUMBER
    ) IS
    BEGIN
        INSERT INTO NAVIGATION(ID, PARENT_ID, TITLE, SUB_TITLE, URL, STATUS_ID, ORDER_NUMBER, ICON, DISABLED, IS_NAVIGABLE)
        VALUES(p_id, p_parent_id, p_title, p_sub_title, p_url, p_status_id, p_order_number, p_icon, p_disabled, p_is_navigable);
    END insert_navigation_item;


    PROCEDURE assign_navigation_role(p_navigation_id NUMBER, p_required_role_id NUMBER) IS
    BEGIN
        INSERT INTO NAVIGATION_ASSIGNED_ROLES(NAVIGATIONID, REQUIREDROLESID)
        VALUES (p_navigation_id, p_required_role_id);
    END assign_navigation_role;


    PROCEDURE add_navigation_prerequisite(p_navigation_id NUMBER, p_prerequisite_id NUMBER) IS
    BEGIN
        INSERT INTO NAVIGATION_PREREQUISITES(NAVIGATION_ID, PREREQUISITE_ID)
        VALUES (p_navigation_id, p_prerequisite_id);
    END add_navigation_prerequisite;


BEGIN
    
    
    DELETE FROM NAVIGATION_PREREQUISITES;
    DELETE FROM NAVIGATION_ASSIGNED_ROLES;
    DELETE FROM NAVIGATION_TRACKING;
    DELETE FROM NAVIGATION;
    

--Top level menus
    insert_navigation_item(INQUIRIES_MENU, TOP_LEVEL_MENU, 'INQUIRIES', '', '', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);

    insert_navigation_item(BENEFICIARIES_MENU, TOP_LEVEL_MENU, 'BENEFICIARIES', '', '', STATUS_NORMAL, ORDER_SECOND, '', DISABLED, IS_NAVIGABLE);
    insert_navigation_item(DISTRIBUTIONS_MENU, TOP_LEVEL_MENU, 'DISTRIBUTIONS', '', '', STATUS_NORMAL, ORDER_THIRD, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(RECONCILIATION_MENU, TOP_LEVEL_MENU, 'RECONCILIATION', '', '', STATUS_NORMAL, ORDER_FOURTH, '', DISABLED, IS_NAVIGABLE);
    insert_navigation_item(YEAR_END_MENU, TOP_LEVEL_MENU, 'YEAR END', '', '', STATUS_NORMAL, ORDER_FIFTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(IT_DEVOPS_MENU, TOP_LEVEL_MENU, 'IT DEVOPS', '', '', STATUS_NORMAL, ORDER_SIXTH, '', ENABLED, IS_NAVIGABLE);

--Sub values for INQUIRIES
    insert_navigation_item(MASTER_INQUIRY_PAGE, INQUIRIES_MENU, 'MASTER INQUIRY', '', 'master-inquiry', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(ADJUSTMENTS_PAGE, INQUIRIES_MENU, 'ADJUSTMENTS', '', 'adjustments', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);


--distribution items
    insert_navigation_item(DISTRIBUTION_INQUIRY_PAGE, DISTRIBUTIONS_MENU, 'Distribution Inquiry (008-14l)', '', 'distributions-inquiry', STATUS_NORMAL, ORDER_ELEVENTH, '', DISABLED, IS_NAVIGABLE);

--It Operations
    insert_navigation_item(DEMOGRAPHIC_FREEZE_PAGE, IT_DEVOPS_MENU, 'Demographic Freeze', '', 'demographic-freeze', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);

--December Activities
    insert_navigation_item(DECEMBER_ACTIVITIES, YEAR_END_MENU, 'December Activities', '','december-process-accordion', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(CLEANUP_REPORTS, DECEMBER_ACTIVITIES, 'Clean up Reports', '','', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);

--sub values for Clean up Reports
    insert_navigation_item(DEMOGRAPHIC_BADGES_NOT_IN_PAYPROFIT, CLEANUP_REPORTS, 'Demographic Badges Not In PayProfit', '','demographic-badges-not-in-payprofit', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(DUPLICATE_SSNS_DEMOGRAPHICS, CLEANUP_REPORTS, 'Duplicate SSNs in Demographics', '','duplicate-ssns-demographics', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(NEGATIVE_ETVA, CLEANUP_REPORTS, 'Negative ETVA', '','negative-etva-for-ssns-on-payprofit', STATUS_NORMAL, ORDER_THIRD, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(DUPLICATE_NAMES_BIRTHDAYS, CLEANUP_REPORTS, 'Duplicate Names and Birthdays', '','duplicate-names-and-birthdays', STATUS_NORMAL, ORDER_FOURTH, '', ENABLED, IS_NAVIGABLE);
--sub values for December Activities
    insert_navigation_item(MILITARY_CONTRIBUTIONS, DECEMBER_ACTIVITIES, 'Military Contributions', '008-13','military-contribution', STATUS_NORMAL, ORDER_THIRD, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(UNFORFEIT, DECEMBER_ACTIVITIES, 'Unforfeit', 'QPREV-PROF','unforfeitures', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(TERMINATIONS, DECEMBER_ACTIVITIES, 'Terminations', 'QPAY066','prof-term', STATUS_NORMAL, ORDER_FOURTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(FORFEITURES, DECEMBER_ACTIVITIES, 'Forfeitures', '008-12', 'forfeitures-adjustment', STATUS_NORMAL, ORDER_FIFTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(DISTRIBUTIONS_AND_FORFEITURES, DECEMBER_ACTIVITIES, 'Distributions and Forfeitures', 'QPAY129', 'distributions-and-forfeitures', STATUS_NORMAL, ORDER_SIXTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PROFIT_SHARE_REPORT, DECEMBER_ACTIVITIES, 'Profit Share Report', 'PAY426', 'profit-share-report', STATUS_NORMAL, ORDER_NINTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PAY426N_LIVE, DECEMBER_ACTIVITIES, 'Profit Sharing Report', 'PAY426N', 'pay426n_live', STATUS_NORMAL, ORDER_TENTH, '', ENABLED, IS_NAVIGABLE);
                          
-- Profit Share Totals (Year End)
    insert_navigation_item(FISCAL_CLOSE, YEAR_END_MENU, 'Fiscal Close', '', 'fiscal-close', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, IS_NAVIGABLE);           

-- Fiscal Close menu items updated according to ticket requirements
    insert_navigation_item(MANAGE_EXECUTIVE_HOURS_PAGE, FISCAL_CLOSE, 'Manage Executive Hours', 'PROF-DOLLAR-EXEC-EXTRACT, TPR008-09', 'manage-executive-hours-and-dollars', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(YTD_WAGES_EXTRACT, FISCAL_CLOSE, 'YTD Wages Extract', 'PROF-DOLLAR-EXTRACT', 'ytd-wages-extract', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, IS_NAVIGABLE);


-- Dev-only: PAY426N entries (not assigned to roles in SQL)
    insert_navigation_item(PAY426_2, FISCAL_CLOSE, 'PAY426-2', '', 'pay426-2', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, NOT_NAVIGABLE);
    insert_navigation_item(PAY426_3, FISCAL_CLOSE, 'PAY426-3', '', 'pay426-3', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, NOT_NAVIGABLE);

-- Profit Summary (PAY426 summary)
    insert_navigation_item(PROFIT_SUMMARY, FISCAL_CLOSE, 'Profit Summary (PAY426 summary)', '', 'pay426-9', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PAY426N_FROZEN, FISCAL_CLOSE, 'Profit Sharing Report', 'PAY426N', 'pay426n_frozen', STATUS_NORMAL, ORDER_THIRD, '', ENABLED, IS_NAVIGABLE);

    insert_navigation_item(QPAY066_AD_HOC_REPORTS, FISCAL_CLOSE, 'QPAY066* Ad Hoc Reports', 'QPAY066*', 'qpay066-adhoc', STATUS_NORMAL, ORDER_SEVENTEENTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(RECENTLY_TERMINATED, FISCAL_CLOSE, 'Recently Terminated', 'PROF-VESTED|PAY508', 'recently-terminated', STATUS_NORMAL, ORDER_EIGHTEENTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(TERMINATED_LETTERS, FISCAL_CLOSE, 'Terminated Letters', 'QPROF003-1', 'terminated-letters', STATUS_NORMAL, ORDER_NINETEENTH, '', ENABLED, IS_NAVIGABLE);

-- Pay Beneficiary Report (Year End / Fiscal Close)
    insert_navigation_item(PAY_BENEFICIARY_REPORT, FISCAL_CLOSE, 'Pay Beneficiary Report', '', 'payben-report', STATUS_NORMAL, ORDER_TENTH, '', ENABLED, IS_NAVIGABLE);

-- Adhoc Beneficiaries Report (Pay Be Next) (Year End / Fiscal Close)
    insert_navigation_item(ADHOC_BENEFICIARIES_REPORT, FISCAL_CLOSE, 'Adhoc Beneficiaries Report (Pay Be Next)', '', 'adhoc-beneficiaries-report', STATUS_NORMAL, ORDER_NINETEENTH, '', ENABLED, IS_NAVIGABLE);

-- QPAY600 (Year End child)
    insert_navigation_item(QPAY600, FISCAL_CLOSE, 'QPAY600', '', 'qpay600', STATUS_NORMAL, ORDER_TWENTIETH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(GET_ELIGIBLE_EMPLOYEES, FISCAL_CLOSE, 'Get Eligible Employees', 'GET-ELIGIBLE-EMPS', 'eligible-employees', STATUS_NORMAL, ORDER_FIFTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PROFIT_SHARE_FORFEIT, FISCAL_CLOSE, 'Profit Share Forfeit', 'PAY443', 'forfeit', STATUS_NORMAL, ORDER_SIXTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(MASTER_UPDATE, FISCAL_CLOSE, 'Master Update', 'PAY444|PAY447', 'profit-share-update', STATUS_NORMAL, ORDER_SEVENTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PROFIT_MASTER_UPDATE, FISCAL_CLOSE, 'Profit Master Update', 'PAY460, PROFTLD', 'profit-master-update', STATUS_NORMAL, ORDER_EIGHTH, '', DISABLED, IS_NAVIGABLE);
    insert_navigation_item(PAYMASTER_UPDATE, FISCAL_CLOSE, 'Paymaster Update', 'PAY450', 'pay450-summary', STATUS_NORMAL, ORDER_NINTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PROF_CONTROL_SHEET, FISCAL_CLOSE, 'Prof Control Sheet', 'PROF-CNTRL-SHEET', 'prof-control-sheet', STATUS_NORMAL, ORDER_ELEVENTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PROF_SHARE_REPORT_BY_AGE, FISCAL_CLOSE, 'Prof Share Report By Age', 'Prof130', '', STATUS_NORMAL, ORDER_TWELFTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PROF_SHARE_GROSS_RPT, FISCAL_CLOSE, 'Prof Share Gross Rpt', 'QPAY501', 'profit-share-gross-report', STATUS_NORMAL, ORDER_THIRTEENTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PROF_SHARE_BY_STORE, FISCAL_CLOSE, 'Prof Share by Store', 'QPAY066TA', 'profit-share-by-store', STATUS_NORMAL, ORDER_FOURTEENTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(REPRINT_CERTIFICATES, FISCAL_CLOSE, 'Reprint Certificates / Print Profit Certs', 'PAYCERT', 'reprint-certificates', STATUS_NORMAL, ORDER_FIFTEENTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(SAVE_PROF_PAYMSTR, FISCAL_CLOSE, 'Save Prof Paymstr', '', 'save-prof-paymstr', STATUS_NORMAL, ORDER_SIXTEENTH, '', ENABLED, IS_NAVIGABLE);
    
    insert_navigation_item(PROFIT_SHARE_REPORT_FINAL_RUN, FISCAL_CLOSE, 'Profit Share Report Final Run', '', 'profit-share-report-final-run', STATUS_NORMAL, ORDER_SEVENTEENTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PROFIT_SHARE_REPORT_EDIT_RUN, FISCAL_CLOSE, 'Profit Share Report Edit Run', '', 'profit-share-report-edit-run', STATUS_NORMAL, ORDER_EIGHTEENTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PAY_BEN_REPORT, BENEFICIARIES_MENU, 'Pay Ben Report', '', 'payben-report', STATUS_NORMAL, ORDER_NINETEENTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PRINT_PROFIT_CERTS, FISCAL_CLOSE, 'Reprint Certificates / Print Profit Certs', 'PAYCERT', 'reprint-certificates', STATUS_NORMAL, ORDER_TWENTIETH, '', ENABLED, IS_NAVIGABLE);



--sub values for Report by Age - updated per requirements
    insert_navigation_item(DISTRIBUTIONS_BY_AGE, PROF_SHARE_REPORT_BY_AGE, 'DISTRIBUTIONS BY AGE', 'PROF130', 'distributions-by-age', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(CONTRIBUTIONS_BY_AGE, PROF_SHARE_REPORT_BY_AGE, 'CONTRIBUTIONS BY AGE', 'PROF130', 'contributions-by-age', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(FORFEITURES_BY_AGE, PROF_SHARE_REPORT_BY_AGE, 'FORFEITURES BY AGE', 'PROF130', 'forfeitures-by-age', STATUS_NORMAL, ORDER_THIRD, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(BALANCE_BY_AGE, PROF_SHARE_REPORT_BY_AGE, 'BALANCE BY AGE', 'PROF130B', 'balance-by-age', STATUS_NORMAL, ORDER_FOURTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(VESTED_AMOUNTS_BY_AGE, PROF_SHARE_REPORT_BY_AGE, 'VESTED AMOUNTS BY AGE', 'PROF130V', 'vested-amounts-by-age', STATUS_NORMAL, ORDER_FIFTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(BALANCE_BY_YEARS, PROF_SHARE_REPORT_BY_AGE, 'BALANCE BY YEARS', 'PROF130Y', 'balance-by-years', STATUS_NORMAL, ORDER_SIXTH, '', ENABLED, IS_NAVIGABLE);

--sub values of Profit Share by Store - updated per requirements
-- Under-21 Report container for Profit Share by Store
    insert_navigation_item(UNDER_21_REPORT, PROF_SHARE_BY_STORE, 'Under-21 Report', '', 'under-21-report', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(QPAY066_UNDR21, UNDER_21_REPORT, 'QPAY066-UNDR21', '', 'qpay066-under21', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(QPAY066TA_UNDR21, UNDER_21_REPORT, 'QPAY066TA-UNDR21', '', 'qpay066ta-under21', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, IS_NAVIGABLE);

-- QPAY066B (child of Prof Share by Store)
    insert_navigation_item(QPAY066B, PROF_SHARE_BY_STORE, 'QPAY066B', '', 'qpay066b', STATUS_NORMAL, ORDER_SEVENTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(QPAY066TA, PROF_SHARE_BY_STORE, 'QPAY066TA', '', 'qpay066ta', STATUS_NORMAL, ORDER_THIRD, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(QNEWPROFLBL, PROF_SHARE_BY_STORE, 'QNEWPROFLBL', '', 'new-ps-labels', STATUS_NORMAL, ORDER_FOURTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PROFNEW, PROF_SHARE_BY_STORE, 'PROFNEW', '', 'profnew', STATUS_NORMAL, ORDER_FIFTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PROFALL, PROF_SHARE_BY_STORE, 'PROFALL', '', 'profall', STATUS_NORMAL, ORDER_SIXTH, '', ENABLED, IS_NAVIGABLE);

-- Print PS Jobs
insert_navigation_item(PRINT_PS_JOBS, YEAR_END_MENU, 'Print PS Jobs', '', 'print-ps-jobs', STATUS_NORMAL, ORDER_THIRD, '', ENABLED, IS_NAVIGABLE);

-- Inserting value for IT Operation for role management
--  NOTE: IT-DevOps navigation should be accessible only to members of the IT-DevOps role (role id 6).
--  Remove other role assignments so the IT menu is exclusive to IT-DevOps.
    assign_navigation_role(IT_DEVOPS_MENU, IT_DEVOPS);

-- Print PS Jobs
    assign_navigation_role(PRINT_PS_JOBS, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PRINT_PS_JOBS, FINANCE_MANAGER);
    assign_navigation_role(PRINT_PS_JOBS, DISTRIBUTIONS_CLERK);

-- Distribution Inquiry
    assign_navigation_role(DISTRIBUTIONS_MENU, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(DISTRIBUTIONS_MENU, FINANCE_MANAGER);
    assign_navigation_role(DISTRIBUTIONS_MENU, DISTRIBUTIONS_CLERK);

    assign_navigation_role(DISTRIBUTION_INQUIRY_PAGE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(DISTRIBUTION_INQUIRY_PAGE, FINANCE_MANAGER);
    assign_navigation_role(DISTRIBUTION_INQUIRY_PAGE, DISTRIBUTIONS_CLERK);

-- Assign roles for INQUIRIES (Master Inquiry endpoints -> CanRunMasterInquiry)
    assign_navigation_role(INQUIRIES_MENU, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(INQUIRIES_MENU, FINANCE_MANAGER);
    assign_navigation_role(INQUIRIES_MENU, DISTRIBUTIONS_CLERK);

-- Assign roles for YEAR END (YearEndGroup -> CanViewYearEndReports)
    assign_navigation_role(YEAR_END_MENU, SYSTEM_ADMINISTRATOR); 
    assign_navigation_role(YEAR_END_MENU, FINANCE_MANAGER); 
-- Assign roles for BENEFICIARIES (BeneficiariesGroup -> CanMaintainBeneficiaries)
    assign_navigation_role(BENEFICIARIES_MENU, DISTRIBUTIONS_CLERK); 
    assign_navigation_role(BENEFICIARIES_MENU, HARDSHIP_ADMINISTRATOR); 

-- Assign roles for MASTER INQUIRY (Endpoints base -> Navigation.Constants.MasterInquiry; Policy -> CanRunMasterInquiry)
    assign_navigation_role(MASTER_INQUIRY_PAGE, SYSTEM_ADMINISTRATOR); 
    assign_navigation_role(MASTER_INQUIRY_PAGE, FINANCE_MANAGER); 
    assign_navigation_role(MASTER_INQUIRY_PAGE, DISTRIBUTIONS_CLERK); 

-- Assign roles for ADJUSTMENTS (Endpoints base -> Navigation.Constants.Adjustments; Policy -> CanRunMasterInquiry)
    assign_navigation_role(ADJUSTMENTS_PAGE, SYSTEM_ADMINISTRATOR); 
    assign_navigation_role(ADJUSTMENTS_PAGE, FINANCE_MANAGER);
    assign_navigation_role(ADJUSTMENTS_PAGE, DISTRIBUTIONS_CLERK);

    assign_navigation_role(DECEMBER_ACTIVITIES, SYSTEM_ADMINISTRATOR);  
    assign_navigation_role(DECEMBER_ACTIVITIES, FINANCE_MANAGER);
    assign_navigation_role(CLEANUP_REPORTS, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(CLEANUP_REPORTS, FINANCE_MANAGER);
    assign_navigation_role(DEMOGRAPHIC_BADGES_NOT_IN_PAYPROFIT, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(DEMOGRAPHIC_BADGES_NOT_IN_PAYPROFIT, FINANCE_MANAGER);
    assign_navigation_role(DUPLICATE_SSNS_DEMOGRAPHICS, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(DUPLICATE_SSNS_DEMOGRAPHICS, FINANCE_MANAGER);
    assign_navigation_role(NEGATIVE_ETVA, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(NEGATIVE_ETVA, FINANCE_MANAGER);
    assign_navigation_role(TERMINATIONS, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(TERMINATIONS, FINANCE_MANAGER);
    assign_navigation_role(DUPLICATE_NAMES_BIRTHDAYS, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(DUPLICATE_NAMES_BIRTHDAYS, FINANCE_MANAGER);
    assign_navigation_role(MILITARY_CONTRIBUTIONS, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(MILITARY_CONTRIBUTIONS, FINANCE_MANAGER);
    assign_navigation_role(UNFORFEIT, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(UNFORFEIT, FINANCE_MANAGER);
    assign_navigation_role(FORFEITURES, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(FORFEITURES, FINANCE_MANAGER);
    assign_navigation_role(DISTRIBUTIONS_AND_FORFEITURES, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(DISTRIBUTIONS_AND_FORFEITURES, FINANCE_MANAGER);
    assign_navigation_role(PROFIT_SHARE_REPORT, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROFIT_SHARE_REPORT, FINANCE_MANAGER);

    assign_navigation_role(PROFIT_SHARE_REPORT_FINAL_RUN, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROFIT_SHARE_REPORT_FINAL_RUN, FINANCE_MANAGER);
    assign_navigation_role(PROFIT_SHARE_REPORT_EDIT_RUN, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROFIT_SHARE_REPORT_EDIT_RUN, FINANCE_MANAGER);
    assign_navigation_role(PRINT_PROFIT_CERTS, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PRINT_PROFIT_CERTS, FINANCE_MANAGER);


    assign_navigation_role(FISCAL_CLOSE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(FISCAL_CLOSE, FINANCE_MANAGER);
    assign_navigation_role(MANAGE_EXECUTIVE_HOURS_PAGE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(MANAGE_EXECUTIVE_HOURS_PAGE, FINANCE_MANAGER);
    assign_navigation_role(YTD_WAGES_EXTRACT, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(YTD_WAGES_EXTRACT, FINANCE_MANAGER);   
    assign_navigation_role(GET_ELIGIBLE_EMPLOYEES, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(GET_ELIGIBLE_EMPLOYEES, FINANCE_MANAGER);
    assign_navigation_role(PROFIT_SHARE_FORFEIT, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROFIT_SHARE_FORFEIT, FINANCE_MANAGER);
    assign_navigation_role(PAYMASTER_UPDATE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PAYMASTER_UPDATE, FINANCE_MANAGER);
    assign_navigation_role(PROF_SHARE_REPORT_BY_AGE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROF_SHARE_REPORT_BY_AGE, FINANCE_MANAGER);
    assign_navigation_role(CONTRIBUTIONS_BY_AGE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(CONTRIBUTIONS_BY_AGE, FINANCE_MANAGER);
    assign_navigation_role(DISTRIBUTIONS_BY_AGE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(DISTRIBUTIONS_BY_AGE, FINANCE_MANAGER);
    assign_navigation_role(FORFEITURES_BY_AGE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(FORFEITURES_BY_AGE, FINANCE_MANAGER);
    assign_navigation_role(BALANCE_BY_AGE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(BALANCE_BY_AGE, FINANCE_MANAGER);
    assign_navigation_role(VESTED_AMOUNTS_BY_AGE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(VESTED_AMOUNTS_BY_AGE, FINANCE_MANAGER);
    assign_navigation_role(BALANCE_BY_YEARS, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(BALANCE_BY_YEARS, FINANCE_MANAGER);
    assign_navigation_role(PROF_SHARE_GROSS_RPT, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROF_SHARE_GROSS_RPT, FINANCE_MANAGER);
    assign_navigation_role(PROF_SHARE_BY_STORE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROF_SHARE_BY_STORE, FINANCE_MANAGER);
    assign_navigation_role(QPAY066_UNDR21, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(QPAY066_UNDR21, FINANCE_MANAGER);
    assign_navigation_role(QPAY066TA_UNDR21, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(QPAY066TA_UNDR21, FINANCE_MANAGER);
    assign_navigation_role(UNDER_21_REPORT, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(UNDER_21_REPORT, FINANCE_MANAGER);
    assign_navigation_role(QPAY066B, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(QPAY066B, FINANCE_MANAGER);
    assign_navigation_role(QPAY066TA, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(QPAY066TA, FINANCE_MANAGER);
    assign_navigation_role(PROFALL, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROFALL, FINANCE_MANAGER);
    assign_navigation_role(QNEWPROFLBL, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(QNEWPROFLBL, FINANCE_MANAGER);
    assign_navigation_role(PROFNEW, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROFNEW, FINANCE_MANAGER);
    assign_navigation_role(REPRINT_CERTIFICATES, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(REPRINT_CERTIFICATES, FINANCE_MANAGER);
    assign_navigation_role(MASTER_UPDATE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(MASTER_UPDATE, FINANCE_MANAGER);
    assign_navigation_role(PROFIT_MASTER_UPDATE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROFIT_MASTER_UPDATE, FINANCE_MANAGER);
    assign_navigation_role(SAVE_PROF_PAYMSTR, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(SAVE_PROF_PAYMSTR, FINANCE_MANAGER);
    assign_navigation_role(PROF_CONTROL_SHEET, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROF_CONTROL_SHEET, FINANCE_MANAGER);
    assign_navigation_role(QPAY066_AD_HOC_REPORTS, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(QPAY066_AD_HOC_REPORTS, FINANCE_MANAGER);
    assign_navigation_role(RECENTLY_TERMINATED, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(RECENTLY_TERMINATED, FINANCE_MANAGER);
    assign_navigation_role(PAY_BENEFICIARY_REPORT, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PAY_BENEFICIARY_REPORT, FINANCE_MANAGER);
    assign_navigation_role(ADHOC_BENEFICIARIES_REPORT, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(ADHOC_BENEFICIARIES_REPORT, FINANCE_MANAGER);
    assign_navigation_role(TERMINATED_LETTERS, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(TERMINATED_LETTERS, FINANCE_MANAGER);
    assign_navigation_role(PROFIT_SUMMARY, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROFIT_SUMMARY, FINANCE_MANAGER);
    assign_navigation_role(QPAY600, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(QPAY600, FINANCE_MANAGER);
    assign_navigation_role(PAY426N_LIVE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PAY426N_LIVE, FINANCE_MANAGER);
    assign_navigation_role(PAY426N_FROZEN, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PAY426N_FROZEN, FINANCE_MANAGER);
    assign_navigation_role(PAY426_2, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PAY426_2, FINANCE_MANAGER);
    assign_navigation_role(PAY426_3, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PAY426_3, FINANCE_MANAGER);


    -- IT Devops role assignments
    assign_navigation_role(INQUIRIES_MENU, IT_DEVOPS);
    assign_navigation_role(DEMOGRAPHIC_FREEZE_PAGE, IT_DEVOPS);
    assign_navigation_role(MASTER_INQUIRY_PAGE, IT_DEVOPS);
    assign_navigation_role(BENEFICIARIES_MENU, IT_DEVOPS);
    assign_navigation_role(DISTRIBUTIONS_MENU, IT_DEVOPS);
    assign_navigation_role(DISTRIBUTION_INQUIRY_PAGE, IT_DEVOPS);
    assign_navigation_role(RECONCILIATION_MENU, IT_DEVOPS);
    assign_navigation_role(YEAR_END_MENU, IT_DEVOPS);
    assign_navigation_role(DECEMBER_ACTIVITIES, IT_DEVOPS);
    assign_navigation_role(CLEANUP_REPORTS, IT_DEVOPS);
    assign_navigation_role(DEMOGRAPHIC_BADGES_NOT_IN_PAYPROFIT, IT_DEVOPS);
    assign_navigation_role(DUPLICATE_SSNS_DEMOGRAPHICS, IT_DEVOPS);
    assign_navigation_role(NEGATIVE_ETVA, IT_DEVOPS);
    assign_navigation_role(TERMINATIONS, IT_DEVOPS);
    assign_navigation_role(DUPLICATE_NAMES_BIRTHDAYS, IT_DEVOPS);
    assign_navigation_role(MILITARY_CONTRIBUTIONS, IT_DEVOPS);
    assign_navigation_role(UNFORFEIT, IT_DEVOPS);
    assign_navigation_role(FORFEITURES, IT_DEVOPS);
    assign_navigation_role(DISTRIBUTIONS_AND_FORFEITURES, IT_DEVOPS);
    assign_navigation_role(PROFIT_SHARE_REPORT, IT_DEVOPS);
    assign_navigation_role(FISCAL_CLOSE, IT_DEVOPS);
    assign_navigation_role(MANAGE_EXECUTIVE_HOURS_PAGE, IT_DEVOPS);
    assign_navigation_role(YTD_WAGES_EXTRACT, IT_DEVOPS);
    assign_navigation_role(QPAY066_AD_HOC_REPORTS, IT_DEVOPS);
    assign_navigation_role(RECENTLY_TERMINATED, IT_DEVOPS);
    assign_navigation_role(GET_ELIGIBLE_EMPLOYEES, IT_DEVOPS);
    assign_navigation_role(PROFIT_SHARE_FORFEIT, IT_DEVOPS);
    assign_navigation_role(MASTER_UPDATE, IT_DEVOPS);
    assign_navigation_role(PROFIT_MASTER_UPDATE, IT_DEVOPS);
    assign_navigation_role(PROF_CONTROL_SHEET, IT_DEVOPS);
    assign_navigation_role(PROF_SHARE_REPORT_BY_AGE, IT_DEVOPS);
    assign_navigation_role(PROF_SHARE_GROSS_RPT, IT_DEVOPS);


    assign_navigation_role(PROF_SHARE_BY_STORE, IT_DEVOPS);
    assign_navigation_role(REPRINT_CERTIFICATES, IT_DEVOPS);
    assign_navigation_role(SAVE_PROF_PAYMSTR, IT_DEVOPS);
    assign_navigation_role(DISTRIBUTIONS_BY_AGE, IT_DEVOPS);
    assign_navigation_role(CONTRIBUTIONS_BY_AGE, IT_DEVOPS);
    assign_navigation_role(FORFEITURES_BY_AGE, IT_DEVOPS);
    assign_navigation_role(BALANCE_BY_AGE, IT_DEVOPS);
    assign_navigation_role(VESTED_AMOUNTS_BY_AGE, IT_DEVOPS);
    assign_navigation_role(BALANCE_BY_YEARS, IT_DEVOPS);
    assign_navigation_role(QPAY066_UNDR21, IT_DEVOPS);
    assign_navigation_role(QPAY066TA_UNDR21, IT_DEVOPS);
    assign_navigation_role(QPAY066TA, IT_DEVOPS);
    assign_navigation_role(QNEWPROFLBL, IT_DEVOPS);
    assign_navigation_role(PROFNEW, IT_DEVOPS);
    assign_navigation_role(PROFALL, IT_DEVOPS);
    assign_navigation_role(PAY_BENEFICIARY_REPORT, IT_DEVOPS);
    assign_navigation_role(ADHOC_BENEFICIARIES_REPORT, IT_DEVOPS);
    assign_navigation_role(UNDER_21_REPORT, IT_DEVOPS);
    assign_navigation_role(QPAY600, IT_DEVOPS);
    assign_navigation_role(PROFIT_SUMMARY, IT_DEVOPS);


    -- Define prerequisites: Master Update
    add_navigation_prerequisite(MASTER_UPDATE, MANAGE_EXECUTIVE_HOURS_PAGE);
    add_navigation_prerequisite(MASTER_UPDATE, MILITARY_CONTRIBUTIONS);
    add_navigation_prerequisite(MASTER_UPDATE, PROFIT_SHARE_REPORT);
    add_navigation_prerequisite(MASTER_UPDATE, UNFORFEIT);
    add_navigation_prerequisite(MASTER_UPDATE, FORFEITURES);

    -- MAKE THESE ROLES READ-ONLY 
    UPDATE NAVIGATION_ROLE SET IS_READ_ONLY=1 where ID IN ( IT_DEVOPS, AUDITOR );

END;
COMMIT ;

