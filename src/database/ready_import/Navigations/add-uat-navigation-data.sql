
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
    -- DISTRIBUTIONS_MENU CONSTANT NUMBER := 4; -- REMOVED
    -- RECONCILIATION_MENU CONSTANT NUMBER := 5; -- REMOVED
    YEAR_END_MENU CONSTANT NUMBER := 6;
    IT_DEVOPS_MENU CONSTANT NUMBER := 7;


    -- These are the secondary drawer top-level menus
    FISCAL_CLOSE CONSTANT NUMBER := 8;
    DECEMBER_ACTIVITIES CONSTANT NUMBER := 9;
    
    -- Third-level under December Activities
    CLEANUP_REPORTS CONSTANT NUMBER := 10;
    -- Third-level under Fiscal Close
    -- PROF_SHARE_REPORT_BY_AGE CONSTANT NUMBER := 11; -- REMOVED (was under FISCAL_CLOSE)
    -- PROF_SHARE_BY_STORE CONSTANT NUMBER := 12; -- REMOVED (was under FISCAL_CLOSE)
    
    -- Groups under INQUIRIES
    INQUIRIES_GROUP CONSTANT NUMBER := 13;
    -- ADJUSTMENTS_GROUP CONSTANT NUMBER := 14; -- REMOVED
    ADHOC_GROUP CONSTANT NUMBER := 15;

    -- AVAILABLE PAGES (ids starting at 100)

    MASTER_INQUIRY_PAGE CONSTANT NUMBER := 100;
    -- ADJUSTMENTS_PAGE CONSTANT NUMBER := 101; -- REMOVED
    DEMOGRAPHIC_FREEZE_PAGE CONSTANT NUMBER := 102;
    -- DISTRIBUTION_INQUIRY_PAGE CONSTANT NUMBER := 103; -- REMOVED
    MANAGE_EXECUTIVE_HOURS_PAGE CONSTANT NUMBER := 104;
    YTD_WAGES_EXTRACT CONSTANT NUMBER := 105;
    FORFEITURES CONSTANT NUMBER := 106;
    DISTRIBUTIONS_AND_FORFEITURES CONSTANT NUMBER := 107;
    PROFIT_SHARE_REPORT CONSTANT NUMBER := 108;
    GET_ELIGIBLE_EMPLOYEES CONSTANT NUMBER := 109;
    PROFIT_SHARE_FORFEIT CONSTANT NUMBER := 110;
    -- MASTER_UPDATE CONSTANT NUMBER := 111; -- REMOVED (was under FISCAL_CLOSE)
    -- PROFIT_MASTER_UPDATE CONSTANT NUMBER := 112; -- REMOVED (was under FISCAL_CLOSE)
    -- PAYMASTER_UPDATE CONSTANT NUMBER := 113; -- REMOVED (was under FISCAL_CLOSE)
    -- PROF_CONTROL_SHEET CONSTANT NUMBER := 114; -- REMOVED (was under FISCAL_CLOSE)
    -- DISTRIBUTIONS_BY_AGE CONSTANT NUMBER := 115; -- REMOVED (was under FISCAL_CLOSE)
    -- CONTRIBUTIONS_BY_AGE CONSTANT NUMBER := 116; -- REMOVED (was under FISCAL_CLOSE)
    -- FORFEITURES_BY_AGE CONSTANT NUMBER := 117; -- REMOVED (was under FISCAL_CLOSE)
    -- BALANCE_BY_AGE CONSTANT NUMBER := 118; -- REMOVED (was under FISCAL_CLOSE)
    -- VESTED_AMOUNTS_BY_AGE CONSTANT NUMBER := 119; -- REMOVED (was under FISCAL_CLOSE)
    -- BALANCE_BY_YEARS CONSTANT NUMBER := 120; -- REMOVED (was under FISCAL_CLOSE)
    -- PROF_SHARE_GROSS_RPT CONSTANT NUMBER := 121; -- REMOVED (was under FISCAL_CLOSE)
    -- UNDER_21_REPORT CONSTANT NUMBER := 122; -- REMOVED (was under FISCAL_CLOSE)
    -- QPAY066_UNDR21 CONSTANT NUMBER := 123; -- REMOVED (was under FISCAL_CLOSE)
    -- QPAY066TA_UNDR21 CONSTANT NUMBER := 124; -- REMOVED (was under FISCAL_CLOSE)
    -- QPAY066B CONSTANT NUMBER := 125; -- REMOVED (was under FISCAL_CLOSE)
    -- QPAY066TA CONSTANT NUMBER := 126; -- REMOVED (was under FISCAL_CLOSE)
    -- QNEWPROFLBL CONSTANT NUMBER := 127; -- REMOVED (was under FISCAL_CLOSE)
    -- PROFNEW CONSTANT NUMBER := 128; -- REMOVED (was under FISCAL_CLOSE)
    -- PROFALL CONSTANT NUMBER := 129; -- REMOVED (was under FISCAL_CLOSE)
    -- REPRINT_CERTIFICATES CONSTANT NUMBER := 130; -- REMOVED (was under FISCAL_CLOSE)
    -- SAVE_PROF_PAYMSTR CONSTANT NUMBER := 131; -- REMOVED (was under FISCAL_CLOSE)
    -- QPAY066_AD_HOC_REPORTS CONSTANT NUMBER := 132; -- REMOVED (was under FISCAL_CLOSE)
    -- RECENTLY_TERMINATED CONSTANT NUMBER := 133; -- REMOVED (was under FISCAL_CLOSE)
    -- PAY_BENEFICIARY_REPORT CONSTANT NUMBER := 134; -- REMOVED (was under FISCAL_CLOSE)
    -- ADHOC_BENEFICIARIES_REPORT CONSTANT NUMBER := 135; -- REMOVED (was under FISCAL_CLOSE)
    -- TERMINATED_LETTERS CONSTANT NUMBER := 136; -- REMOVED (was under FISCAL_CLOSE)
    -- QPAY600 CONSTANT NUMBER := 137; -- REMOVED (was under FISCAL_CLOSE)
    PAY426N_DECEMBER CONSTANT NUMBER := 138;
    PROFIT_SUMMARY CONSTANT NUMBER := 139;
    -- PAY426_2 CONSTANT NUMBER := 140;
    -- PAY426_3 CONSTANT NUMBER := 141;
    DEMOGRAPHIC_BADGES_NOT_IN_PAYPROFIT CONSTANT NUMBER := 142;
    DUPLICATE_SSNS_DEMOGRAPHICS CONSTANT NUMBER := 143;
    NEGATIVE_ETVA CONSTANT NUMBER := 144;
    TERMINATIONS CONSTANT NUMBER := 145;
    DUPLICATE_NAMES_BIRTHDAYS CONSTANT NUMBER := 146;
    MILITARY_CONTRIBUTIONS CONSTANT NUMBER := 147;
    UNFORFEIT CONSTANT NUMBER := 148;
    MANAGE_EXECUTIVE_HOURS_FISCAL_CLOSE CONSTANT NUMBER := 162;
    PAY426N_FISCAL_CLOSE CONSTANT NUMBER := 153;

    -- PROFIT_SHARE_REPORT_FINAL_RUN CONSTANT NUMBER := 149; -- REMOVED (was under FISCAL_CLOSE)
    -- PRINT_PROFIT_CERTS CONSTANT NUMBER := 150; -- REMOVED (was under FISCAL_CLOSE)
    -- PROFIT_SHARE_REPORT_EDIT_RUN CONSTANT NUMBER := 151; -- REMOVED (was under FISCAL_CLOSE)
    -- PAY_BEN_REPORT CONSTANT NUMBER := 152; -- REMOVED (was under FISCAL_CLOSE)
    -- PAY426N_FROZEN CONSTANT NUMBER := 153; -- REMOVED (was under FISCAL_CLOSE)
    -- PROFIT_DETAILS_REVERSAL CONSTANT NUMBER := 154; -- REMOVED (was under FISCAL_CLOSE)
    -- PRINT_PS_JOBS CONSTANT NUMBER :=155; -- REMOVED

    -- Adhoc Reports
    QPAY600 CONSTANT NUMBER := 137;
    TERMINATED_LETTERS CONSTANT NUMBER := 136;
    RECENTLY_TERMINATED CONSTANT NUMBER := 133;
    -- QPAY066_AD_HOC_REPORTS CONSTANT NUMBER := 132; -- REMOVED
    QPAY066_UNDR21 CONSTANT NUMBER := 123;
    DIVORCE_REPORT CONSTANT NUMBER := 161;

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
    insert_navigation_item(INQUIRIES_MENU, TOP_LEVEL_MENU, 'INQUIRIES & ADJUSTMENTS', 'Inquiries & Adjustments', '', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);

    insert_navigation_item(BENEFICIARIES_MENU, TOP_LEVEL_MENU, 'BENEFICIARIES', 'Beneficiaries', '', STATUS_NORMAL, ORDER_SECOND, '', DISABLED, IS_NAVIGABLE);
    -- DISTRIBUTIONS_MENU REMOVED
    -- insert_navigation_item(DISTRIBUTIONS_MENU, TOP_LEVEL_MENU, 'DISTRIBUTIONS', 'Distributions', '', STATUS_NORMAL, ORDER_THIRD, '', ENABLED, IS_NAVIGABLE);
    -- RECONCILIATION REMOVED
    insert_navigation_item(YEAR_END_MENU, TOP_LEVEL_MENU, 'YEAR END', 'Year End', '', STATUS_NORMAL, ORDER_FIFTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(IT_DEVOPS_MENU, TOP_LEVEL_MENU, 'IT DEVOPS', 'IT DevOps', '', STATUS_NORMAL, ORDER_SIXTH, '', ENABLED, IS_NAVIGABLE);

--Sub values for INQUIRIES
    insert_navigation_item(INQUIRIES_GROUP, INQUIRIES_MENU, 'Inquiries', '', '', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(MASTER_INQUIRY_PAGE, INQUIRIES_GROUP, 'MASTER INQUIRY', '', 'master-inquiry', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);
    
    -- ADJUSTMENTS_GROUP and ADJUSTMENTS_PAGE - REMOVED
    -- insert_navigation_item(ADJUSTMENTS_GROUP, INQUIRIES_MENU, 'Adjustments', '', '', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, IS_NAVIGABLE);
    -- insert_navigation_item(ADJUSTMENTS_PAGE, ADJUSTMENTS_GROUP, 'ADJUSTMENTS', '', 'adjustments', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);


--distribution items - REMOVED
    -- insert_navigation_item(DISTRIBUTION_INQUIRY_PAGE, DISTRIBUTIONS_MENU, 'Distribution Inquiry (008-14l)', '', 'distributions-inquiry', STATUS_NORMAL, ORDER_ELEVENTH, '', DISABLED, IS_NAVIGABLE);


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
    insert_navigation_item(MANAGE_EXECUTIVE_HOURS_PAGE, DECEMBER_ACTIVITIES, 'Manage Executive Hours', 'PROF-DOLLAR-EXEC-EXTRACT, TPR008-09', 'manage-executive-hours-and-dollars', STATUS_NORMAL, ORDER_SEVENTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PROFIT_SHARE_REPORT, DECEMBER_ACTIVITIES, 'Profit Share Report', 'PAY426', 'profit-share-report', STATUS_NORMAL, ORDER_EIGHTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PAY426N_DECEMBER, DECEMBER_ACTIVITIES, 'Profit Sharing Report', 'PAY426N', 'pay426n', STATUS_NORMAL, ORDER_NINTH, '', ENABLED, IS_NAVIGABLE);

-- Adhoc Reports Group
    insert_navigation_item(ADHOC_GROUP, INQUIRIES_MENU, 'Adhoc Reports', '', '', STATUS_NORMAL, ORDER_THIRD, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(QPAY600, ADHOC_GROUP, 'QPAY600', '', 'qpay600', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(TERMINATED_LETTERS, ADHOC_GROUP, 'Terminated Letters', 'QPROF003-1', 'terminated-letters', STATUS_NORMAL, ORDER_THIRD, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(RECENTLY_TERMINATED, ADHOC_GROUP, 'Recently Terminated', 'PROF-VESTED|PAY508', 'recently-terminated', STATUS_NORMAL, ORDER_FOURTH, '', ENABLED, IS_NAVIGABLE);
    -- insert_navigation_item(QPAY066_AD_HOC_REPORTS, ADHOC_GROUP, 'QPAY066* Ad Hoc Reports', 'QPAY066*', 'qpay066-adhoc', STATUS_NORMAL, ORDER_FIFTH, '', ENABLED, IS_NAVIGABLE); -- REMOVED
    insert_navigation_item(QPAY066_UNDR21, ADHOC_GROUP, 'QPAY066-UNDR21', '', 'qpay066-under21', STATUS_NORMAL, ORDER_SIXTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(DIVORCE_REPORT, ADHOC_GROUP, 'Account History Report', '', 'divorce-report', STATUS_NORMAL, ORDER_SEVENTH, '', ENABLED, IS_NAVIGABLE);

-- Fiscal Close
    insert_navigation_item(FISCAL_CLOSE, YEAR_END_MENU, 'Fiscal Close', '', 'fiscal-close', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(MANAGE_EXECUTIVE_HOURS_FISCAL_CLOSE, FISCAL_CLOSE, 'Manage Executive Hours', 'PROF-DOLLAR-EXEC-EXTRACT, TPR008-09', 'manage-executive-hours-and-dollars', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(YTD_WAGES_EXTRACT, FISCAL_CLOSE, 'YTD Wages Extract', 'PROF-DOLLAR-EXTRACT', 'ytd-wages-extract', STATUS_NORMAL, ORDER_SECOND, '', ENABLED, IS_NAVIGABLE);

-- Profit Summary (PAY426 summary)
    insert_navigation_item(PROFIT_SUMMARY, FISCAL_CLOSE, 'Profit Summary (PAY426 summary)', '', 'pay426-9', STATUS_NORMAL, ORDER_THIRD, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PAY426N_FISCAL_CLOSE, FISCAL_CLOSE, 'Profit Sharing Report', 'PAY426N', 'pay426n', STATUS_NORMAL, ORDER_FOURTH, '', ENABLED, IS_NAVIGABLE);

    insert_navigation_item(GET_ELIGIBLE_EMPLOYEES, FISCAL_CLOSE, 'Get Eligible Employees', 'GET-ELIGIBLE-EMPS', 'eligible-employees', STATUS_NORMAL, ORDER_FIFTH, '', ENABLED, IS_NAVIGABLE);
    insert_navigation_item(PROFIT_SHARE_FORFEIT, FISCAL_CLOSE, 'Profit Share Forfeit', 'PAY443', 'forfeit', STATUS_NORMAL, ORDER_SIXTH, '', ENABLED, IS_NAVIGABLE);

-- Inserting value for IT Operation for role management
--  NOTE: IT-DevOps navigation should be accessible only to members of the IT-DevOps role (role id 6).
--  Remove other role assignments so the IT menu is exclusive to IT-DevOps.
    assign_navigation_role(IT_DEVOPS_MENU, IT_DEVOPS);

-- Print PS Jobs - REMOVED
    -- assign_navigation_role(PRINT_PS_JOBS, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PRINT_PS_JOBS, FINANCE_MANAGER);
    -- assign_navigation_role(PRINT_PS_JOBS, DISTRIBUTIONS_CLERK);

-- Distribution Inquiry - REMOVED
    -- assign_navigation_role(DISTRIBUTIONS_MENU, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(DISTRIBUTIONS_MENU, FINANCE_MANAGER);
    -- assign_navigation_role(DISTRIBUTIONS_MENU, DISTRIBUTIONS_CLERK);

    -- assign_navigation_role(DISTRIBUTION_INQUIRY_PAGE, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(DISTRIBUTION_INQUIRY_PAGE, FINANCE_MANAGER);
    -- assign_navigation_role(DISTRIBUTION_INQUIRY_PAGE, DISTRIBUTIONS_CLERK);

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

-- Assign roles for INQUIRIES_GROUP
    assign_navigation_role(INQUIRIES_GROUP, SYSTEM_ADMINISTRATOR); 
    assign_navigation_role(INQUIRIES_GROUP, FINANCE_MANAGER); 
    assign_navigation_role(INQUIRIES_GROUP, DISTRIBUTIONS_CLERK);

-- Assign roles for ADHOC_GROUP
    assign_navigation_role(ADHOC_GROUP, SYSTEM_ADMINISTRATOR); 
    assign_navigation_role(ADHOC_GROUP, FINANCE_MANAGER); 
    assign_navigation_role(ADHOC_GROUP, DISTRIBUTIONS_CLERK);

-- Assign roles for MASTER INQUIRY (Endpoints base -> Navigation.Constants.MasterInquiry; Policy -> CanRunMasterInquiry)
    assign_navigation_role(MASTER_INQUIRY_PAGE, SYSTEM_ADMINISTRATOR); 
    assign_navigation_role(MASTER_INQUIRY_PAGE, FINANCE_MANAGER); 
    assign_navigation_role(MASTER_INQUIRY_PAGE, DISTRIBUTIONS_CLERK);
    assign_navigation_role(MASTER_INQUIRY_PAGE, HR_READONLY);

-- Assign roles for ADJUSTMENTS_GROUP - REMOVED
    -- assign_navigation_role(ADJUSTMENTS_GROUP, SYSTEM_ADMINISTRATOR); 
    -- assign_navigation_role(ADJUSTMENTS_GROUP, FINANCE_MANAGER);
    -- assign_navigation_role(ADJUSTMENTS_GROUP, DISTRIBUTIONS_CLERK);

-- Assign roles for ADJUSTMENTS - REMOVED
    -- assign_navigation_role(ADJUSTMENTS_PAGE, SYSTEM_ADMINISTRATOR); 
    -- assign_navigation_role(ADJUSTMENTS_PAGE, FINANCE_MANAGER);
    -- assign_navigation_role(ADJUSTMENTS_PAGE, DISTRIBUTIONS_CLERK);

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
    assign_navigation_role(DUPLICATE_NAMES_BIRTHDAYS, HR_READONLY);
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
    assign_navigation_role(PAY426N_DECEMBER, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PAY426N_DECEMBER, FINANCE_MANAGER);

-- FISCAL_CLOSE role assignments
    assign_navigation_role(FISCAL_CLOSE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(FISCAL_CLOSE, FINANCE_MANAGER);
    assign_navigation_role(MANAGE_EXECUTIVE_HOURS_FISCAL_CLOSE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(MANAGE_EXECUTIVE_HOURS_FISCAL_CLOSE, FINANCE_MANAGER);
    assign_navigation_role(YTD_WAGES_EXTRACT, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(YTD_WAGES_EXTRACT, FINANCE_MANAGER);
    assign_navigation_role(GET_ELIGIBLE_EMPLOYEES, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(GET_ELIGIBLE_EMPLOYEES, FINANCE_MANAGER);
    assign_navigation_role(PROFIT_SHARE_FORFEIT, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROFIT_SHARE_FORFEIT, FINANCE_MANAGER);
    assign_navigation_role(PROFIT_SUMMARY, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PROFIT_SUMMARY, FINANCE_MANAGER);
    assign_navigation_role(PAY426N_FISCAL_CLOSE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(PAY426N_FISCAL_CLOSE, FINANCE_MANAGER);
    -- assign_navigation_role(PAYMASTER_UPDATE, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PAYMASTER_UPDATE, FINANCE_MANAGER);
    -- assign_navigation_role(PROF_SHARE_REPORT_BY_AGE, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PROF_SHARE_REPORT_BY_AGE, FINANCE_MANAGER);
    -- assign_navigation_role(CONTRIBUTIONS_BY_AGE, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(CONTRIBUTIONS_BY_AGE, FINANCE_MANAGER);
    -- assign_navigation_role(DISTRIBUTIONS_BY_AGE, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(DISTRIBUTIONS_BY_AGE, FINANCE_MANAGER);
    -- assign_navigation_role(FORFEITURES_BY_AGE, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(FORFEITURES_BY_AGE, FINANCE_MANAGER);
    -- assign_navigation_role(BALANCE_BY_AGE, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(BALANCE_BY_AGE, FINANCE_MANAGER);
    -- assign_navigation_role(VESTED_AMOUNTS_BY_AGE, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(VESTED_AMOUNTS_BY_AGE, FINANCE_MANAGER);
    -- assign_navigation_role(BALANCE_BY_YEARS, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(BALANCE_BY_YEARS, FINANCE_MANAGER);
    -- assign_navigation_role(PROF_SHARE_GROSS_RPT, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PROF_SHARE_GROSS_RPT, FINANCE_MANAGER);
    -- assign_navigation_role(PROF_SHARE_BY_STORE, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PROF_SHARE_BY_STORE, FINANCE_MANAGER);
    -- assign_navigation_role(QPAY066_UNDR21, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(QPAY066_UNDR21, FINANCE_MANAGER);
    -- assign_navigation_role(QPAY066TA_UNDR21, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(QPAY066TA_UNDR21, FINANCE_MANAGER);
    -- assign_navigation_role(UNDER_21_REPORT, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(UNDER_21_REPORT, FINANCE_MANAGER);
    -- assign_navigation_role(QPAY066B, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(QPAY066B, FINANCE_MANAGER);
    -- assign_navigation_role(QPAY066TA, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(QPAY066TA, FINANCE_MANAGER);
    -- assign_navigation_role(PROFALL, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PROFALL, FINANCE_MANAGER);
    -- assign_navigation_role(QNEWPROFLBL, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(QNEWPROFLBL, FINANCE_MANAGER);
    -- assign_navigation_role(PROFNEW, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PROFNEW, FINANCE_MANAGER);
    -- assign_navigation_role(REPRINT_CERTIFICATES, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(REPRINT_CERTIFICATES, FINANCE_MANAGER);
    -- assign_navigation_role(MASTER_UPDATE, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(MASTER_UPDATE, FINANCE_MANAGER);
    -- assign_navigation_role(PROFIT_MASTER_UPDATE, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PROFIT_MASTER_UPDATE, FINANCE_MANAGER);
    -- assign_navigation_role(SAVE_PROF_PAYMSTR, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(SAVE_PROF_PAYMSTR, FINANCE_MANAGER);
    -- assign_navigation_role(PROF_CONTROL_SHEET, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PROF_CONTROL_SHEET, FINANCE_MANAGER);
    -- assign_navigation_role(QPAY066_AD_HOC_REPORTS, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(QPAY066_AD_HOC_REPORTS, FINANCE_MANAGER);
    -- assign_navigation_role(RECENTLY_TERMINATED, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(RECENTLY_TERMINATED, FINANCE_MANAGER);
    -- assign_navigation_role(PAY_BENEFICIARY_REPORT, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PAY_BENEFICIARY_REPORT, FINANCE_MANAGER);
    -- assign_navigation_role(ADHOC_BENEFICIARIES_REPORT, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(ADHOC_BENEFICIARIES_REPORT, FINANCE_MANAGER);
    -- assign_navigation_role(TERMINATED_LETTERS, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(TERMINATED_LETTERS, FINANCE_MANAGER);
    -- assign_navigation_role(PROFIT_SUMMARY, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PROFIT_SUMMARY, FINANCE_MANAGER);
    -- assign_navigation_role(QPAY600, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(QPAY600, FINANCE_MANAGER);
    -- assign_navigation_role(PAY426N_FROZEN, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PAY426N_FROZEN, FINANCE_MANAGER);
    -- assign_navigation_role(PAY426_2, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PAY426_2, FINANCE_MANAGER);
    -- assign_navigation_role(PAY426_3, SYSTEM_ADMINISTRATOR);
    -- assign_navigation_role(PAY426_3, FINANCE_MANAGER);

    assign_navigation_role(MANAGE_EXECUTIVE_HOURS_PAGE, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(MANAGE_EXECUTIVE_HOURS_PAGE, FINANCE_MANAGER);

    -- Adhoc Reports role assignments
    assign_navigation_role(QPAY600, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(QPAY600, FINANCE_MANAGER);
    assign_navigation_role(QPAY600, DISTRIBUTIONS_CLERK);
    
    assign_navigation_role(TERMINATED_LETTERS, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(TERMINATED_LETTERS, FINANCE_MANAGER);
    assign_navigation_role(TERMINATED_LETTERS, DISTRIBUTIONS_CLERK);
    
    assign_navigation_role(RECENTLY_TERMINATED, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(RECENTLY_TERMINATED, FINANCE_MANAGER);
    assign_navigation_role(RECENTLY_TERMINATED, DISTRIBUTIONS_CLERK);
    
    -- assign_navigation_role(QPAY066_AD_HOC_REPORTS, SYSTEM_ADMINISTRATOR); -- REMOVED
    -- assign_navigation_role(QPAY066_AD_HOC_REPORTS, FINANCE_MANAGER); -- REMOVED
    -- assign_navigation_role(QPAY066_AD_HOC_REPORTS, DISTRIBUTIONS_CLERK); -- REMOVED
    
    assign_navigation_role(QPAY066_UNDR21, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(QPAY066_UNDR21, FINANCE_MANAGER);
    assign_navigation_role(QPAY066_UNDR21, DISTRIBUTIONS_CLERK);
    
    assign_navigation_role(DIVORCE_REPORT, SYSTEM_ADMINISTRATOR);
    assign_navigation_role(DIVORCE_REPORT, FINANCE_MANAGER);
    assign_navigation_role(DIVORCE_REPORT, DISTRIBUTIONS_CLERK);

    -- IT Devops role assignments
    assign_navigation_role(INQUIRIES_MENU, IT_DEVOPS);
    assign_navigation_role(INQUIRIES_GROUP, IT_DEVOPS);
    -- assign_navigation_role(ADJUSTMENTS_GROUP, IT_DEVOPS); -- REMOVED
    assign_navigation_role(DEMOGRAPHIC_FREEZE_PAGE, IT_DEVOPS);
    assign_navigation_role(MASTER_INQUIRY_PAGE, IT_DEVOPS);
    assign_navigation_role(BENEFICIARIES_MENU, IT_DEVOPS);
    -- assign_navigation_role(DISTRIBUTIONS_MENU, IT_DEVOPS); -- REMOVED
    -- assign_navigation_role(DISTRIBUTION_INQUIRY_PAGE, IT_DEVOPS); -- REMOVED
    -- assign_navigation_role(RECONCILIATION_MENU, IT_DEVOPS); -- REMOVED
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
    assign_navigation_role(PAY426N_DECEMBER, IT_DEVOPS);
    assign_navigation_role(MANAGE_EXECUTIVE_HOURS_PAGE, IT_DEVOPS);

    -- Fiscal Close IT_DEVOPS role assignments
    assign_navigation_role(FISCAL_CLOSE, IT_DEVOPS);
    assign_navigation_role(MANAGE_EXECUTIVE_HOURS_FISCAL_CLOSE, IT_DEVOPS);
    assign_navigation_role(YTD_WAGES_EXTRACT, IT_DEVOPS);
    assign_navigation_role(GET_ELIGIBLE_EMPLOYEES, IT_DEVOPS);
    assign_navigation_role(PROFIT_SHARE_FORFEIT, IT_DEVOPS);
    assign_navigation_role(PROFIT_SUMMARY, IT_DEVOPS);
    assign_navigation_role(PAY426N_FISCAL_CLOSE, IT_DEVOPS);

    -- Adhoc Reports IT_DEVOPS role assignments
    assign_navigation_role(ADHOC_GROUP, IT_DEVOPS);
    assign_navigation_role(QPAY600, IT_DEVOPS);
    assign_navigation_role(TERMINATED_LETTERS, IT_DEVOPS);
    assign_navigation_role(RECENTLY_TERMINATED, IT_DEVOPS);
    -- assign_navigation_role(QPAY066_AD_HOC_REPORTS, IT_DEVOPS); -- REMOVED
    assign_navigation_role(QPAY066_UNDR21, IT_DEVOPS);
    assign_navigation_role(DIVORCE_REPORT, IT_DEVOPS);

    -- FISCAL_CLOSE IT_DEVOPS role assignments - REMOVED
    -- assign_navigation_role(FISCAL_CLOSE, IT_DEVOPS);
    -- assign_navigation_role(YTD_WAGES_EXTRACT, IT_DEVOPS);
    -- assign_navigation_role(QPAY066_AD_HOC_REPORTS, IT_DEVOPS);
    -- assign_navigation_role(RECENTLY_TERMINATED, IT_DEVOPS);
    -- assign_navigation_role(GET_ELIGIBLE_EMPLOYEES, IT_DEVOPS);
    -- assign_navigation_role(PROFIT_SHARE_FORFEIT, IT_DEVOPS);
    -- assign_navigation_role(MASTER_UPDATE, IT_DEVOPS);
    -- assign_navigation_role(PROFIT_MASTER_UPDATE, IT_DEVOPS);
    -- assign_navigation_role(PROF_CONTROL_SHEET, IT_DEVOPS);
    -- assign_navigation_role(PROF_SHARE_REPORT_BY_AGE, IT_DEVOPS);
    -- assign_navigation_role(PROF_SHARE_GROSS_RPT, IT_DEVOPS);
    -- assign_navigation_role(PROF_SHARE_BY_STORE, IT_DEVOPS);
    -- assign_navigation_role(REPRINT_CERTIFICATES, IT_DEVOPS);
    -- assign_navigation_role(SAVE_PROF_PAYMSTR, IT_DEVOPS);
    -- assign_navigation_role(DISTRIBUTIONS_BY_AGE, IT_DEVOPS);
    -- assign_navigation_role(CONTRIBUTIONS_BY_AGE, IT_DEVOPS);
    -- assign_navigation_role(FORFEITURES_BY_AGE, IT_DEVOPS);
    -- assign_navigation_role(BALANCE_BY_AGE, IT_DEVOPS);
    -- assign_navigation_role(VESTED_AMOUNTS_BY_AGE, IT_DEVOPS);
    -- assign_navigation_role(BALANCE_BY_YEARS, IT_DEVOPS);
    -- assign_navigation_role(QPAY066_UNDR21, IT_DEVOPS);
    -- assign_navigation_role(QPAY066TA_UNDR21, IT_DEVOPS);
    -- assign_navigation_role(QPAY066TA, IT_DEVOPS);
    -- assign_navigation_role(QNEWPROFLBL, IT_DEVOPS);
    -- assign_navigation_role(PROFNEW, IT_DEVOPS);
    -- assign_navigation_role(PROFALL, IT_DEVOPS);
    -- assign_navigation_role(PAY_BENEFICIARY_REPORT, IT_DEVOPS);
    -- assign_navigation_role(ADHOC_BENEFICIARIES_REPORT, IT_DEVOPS);
    -- assign_navigation_role(UNDER_21_REPORT, IT_DEVOPS);
    -- assign_navigation_role(QPAY600, IT_DEVOPS);
    -- assign_navigation_role(PROFIT_SUMMARY, IT_DEVOPS);


    -- Define prerequisites: Master Update - REMOVED (MASTER_UPDATE is part of FISCAL_CLOSE)
    -- add_navigation_prerequisite(MASTER_UPDATE, MANAGE_EXECUTIVE_HOURS_PAGE);
    -- add_navigation_prerequisite(MASTER_UPDATE, MILITARY_CONTRIBUTIONS);
    -- add_navigation_prerequisite(MASTER_UPDATE, PROFIT_SHARE_REPORT);
    -- add_navigation_prerequisite(MASTER_UPDATE, UNFORFEIT);
    -- add_navigation_prerequisite(MASTER_UPDATE, FORFEITURES);

    -- MAKE THESE ROLES READ-ONLY 
    UPDATE NAVIGATION_ROLE SET IS_READ_ONLY=1 where ID IN ( IT_DEVOPS, AUDITOR );

END;
COMMIT ;

