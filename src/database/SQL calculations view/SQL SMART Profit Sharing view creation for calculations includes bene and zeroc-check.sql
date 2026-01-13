CREATE OR REPLACE VIEW PSCALCVIEW AS
WITH DEMO_OR_BENEFICIARY AS (
    SELECT
        p.SSN,
        -- EnrollmentId computed from VestingScheduleId and HasForfeited (enrollment decomposition)
        CASE 
            WHEN p.VESTING_SCHEDULE_ID IS NULL THEN 0  -- NotEnrolled
            WHEN p.HAS_FORFEITED = 1 THEN 
                CASE WHEN p.VESTING_SCHEDULE_ID = 1 THEN 3 ELSE 5 END  -- Old/NewVestingPlanHasForfeitureRecords
            ELSE 
                CASE WHEN p.VESTING_SCHEDULE_ID = 1 THEN 1 ELSE 4 END  -- Old/NewVestingPlanHasContributions
        END AS ENROLLMENT,
        CAST(p.TERMINATION_CODE_ID AS VARCHAR2(1)) AS TERMCD,
        p.TERMINATION_DATE AS TERMDT,
        pp.ZERO_CONTRIBUTION_REASON_ID AS ZEROCD,
        p.DATE_OF_BIRTH AS DOB,
        pp.HOURS_CURRENT_YEAR AS HOURS,
        pp.COMPANY_CONTRIBUTION_YEARS AS YEARS,
        0 AS FROM_BENEFICIARY,
        p.VESTING_SCHEDULE_ID -- Keep for vesting calculations
    FROM DEMOGRAPHIC p
    JOIN PAY_PROFIT pp ON p.SSN = pp.SSN
    UNION ALL
    SELECT
        pb.SSN,
        0 AS ENROLLMENT,   -- Beneficiaries not enrolled
        CAST(' ' AS VARCHAR2(1)) AS TERMCD,
        NULL AS TERMDT,
        0 AS ZEROCD,
        pb.DATE_OF_BIRTH AS DOB,
        0 AS HOURS,
        0 AS YEARS,
        1 AS FROM_BENEFICIARY,
        NULL AS VESTING_SCHEDULE_ID
    FROM BENEFICIARY pb
    WHERE NOT EXISTS (SELECT 1 FROM DEMOGRAPHIC p WHERE p.SSN = pb.SSN)
)
SELECT 
    d.SSN,
    d.ENROLLMENT,
    d.TERMCD,
    d.TERMDT,
    d.ZEROCD,
    d.DOB,
    d.HOURS,
    d.YEARS,
    CASE 
        WHEN d.FROM_BENEFICIARY = 1 THEN 1
        ELSE CASE 
            WHEN TRUNC(MONTHS_BETWEEN(CURRENT_DATE, d.DOB) / 12) >= 65 
             AND (d.TERMDT IS NULL OR EXTRACT(YEAR FROM d.TERMDT) >= EXTRACT(YEAR FROM CURRENT_DATE)) 
               THEN 1
            WHEN d.ENROLLMENT IN (3, 5) THEN 1  -- Has forfeiture records (values 3 and 5)
            WHEN d.TERMCD = 'Z' THEN 1
            WHEN d.ZEROCD = 6 THEN 1
            -- Remove the condition for PY_PROF_ZEROCONT = 7 affecting VESTPCT here
            WHEN (CASE WHEN d.VESTING_SCHEDULE_ID = 2 THEN 1 ELSE 0 END  -- New plan vests 1 year faster
                  + CASE WHEN d.HOURS >= 1000 THEN 1 ELSE 0 END 
                  + d.YEARS) < 3 THEN 0
            WHEN (CASE WHEN d.VESTING_SCHEDULE_ID = 2 THEN 1 ELSE 0 END 
                  + CASE WHEN d.HOURS >= 1000 THEN 1 ELSE 0 END 
                  + d.YEARS) = 3 THEN .2
            WHEN (CASE WHEN d.VESTING_SCHEDULE_ID = 2 THEN 1 ELSE 0 END 
                  + CASE WHEN d.HOURS >= 1000 THEN 1 ELSE 0 END 
                  + d.YEARS) = 4 THEN .4
            WHEN (CASE WHEN d.VESTING_SCHEDULE_ID = 2 THEN 1 ELSE 0 END 
                  + CASE WHEN d.HOURS >= 1000 THEN 1 ELSE 0 END 
                  + d.YEARS) = 5 THEN .6
            WHEN (CASE WHEN d.VESTING_SCHEDULE_ID = 2 THEN 1 ELSE 0 END 
                  + CASE WHEN d.HOURS >= 1000 THEN 1 ELSE 0 END 
                  + d.YEARS) = 6 THEN .8
            WHEN (CASE WHEN d.VESTING_SCHEDULE_ID = 2 THEN 1 ELSE 0 END 
                  + CASE WHEN d.HOURS >= 1000 THEN 1 ELSE 0 END 
                  + d.YEARS) >= 7 THEN 1
            ELSE 0
        END
    END AS VESTPCT,
    -- Add the new ZEROC_CHECK column here
    CASE 
        WHEN d.ZEROCD = 7 AND FLOOR(MONTHS_BETWEEN(CURRENT_DATE, d.DOB)/12) > 64 THEN 'Y'
        ELSE 'N'
    END AS ZEROC_CHECK,
    COALESCE(total_balance_subquery.TOTAL_BALANCE, 0) AS TOTAL_BALANCE,
    COALESCE(etva_subquery.ETVA, 0) AS ETVA,
    COALESCE(distributions_subquery.DISTRIBUTIONS, 0) AS DISTRIBUTIONS,
    COALESCE(grossamt_subquery.GROSSAMT, 0) AS GROSSAMT
FROM 
    DEMO_OR_BENEFICIARY d
LEFT JOIN (
    SELECT 
        pd1.SSN AS PD1SSN,
        SUM(CASE 
            WHEN pd1.PROFIT_CODE_ID = 9 THEN - pd1.FORFEITURE
            WHEN pd1.PROFIT_CODE_ID IN (1, 2, 3, 5) THEN - pd1.FORFEITURE + pd1.CONTRIBUTION + pd1.EARNINGS
            ELSE pd1.CONTRIBUTION + pd1.EARNINGS + pd1.FORFEITURE
        END) AS TOTAL_BALANCE
    FROM PROFIT_DETAIL pd1
    GROUP BY pd1.SSN
) total_balance_subquery ON d.SSN = total_balance_subquery.PD1SSN
LEFT JOIN (
    SELECT 
        pd2.SSN AS PD2SSN,
        SUM(CASE 
            WHEN pd2.PROFIT_CODE_ID = 6 THEN pd2.CONTRIBUTION
            WHEN pd2.PROFIT_CODE_ID = 8 THEN pd2.EARNINGS
            WHEN pd2.PROFIT_CODE_ID = 9 THEN - pd2.FORFEITURE
        END) AS ETVA
    FROM PROFIT_DETAIL pd2
    GROUP BY pd2.SSN
) etva_subquery ON d.SSN = etva_subquery.PD2SSN
LEFT JOIN (
    SELECT 
        pd3.SSN AS PD3SSN,
        SUM(CASE 
            WHEN pd3.PROFIT_CODE_ID IN (1, 2, 3, 5) THEN pd3.FORFEITURE
        END) AS DISTRIBUTIONS
    FROM PROFIT_DETAIL pd3
    GROUP BY pd3.SSN
) distributions_subquery ON d.SSN = distributions_subquery.PD3SSN
LEFT JOIN (
    SELECT 
        ds.SSN AS DSSSN,
        SUM(CASE 
            WHEN ds.STATUS_ID NOT IN ('D', 'P') THEN ds.GROSS_AMOUNT
        END) AS GROSSAMT
    FROM DISTRIBUTION ds
    GROUP BY ds.SSN
) grossamt_subquery ON d.SSN = grossamt_subquery.DSSSN;
