CREATE OR REPLACE VIEW vested_percent AS
SELECT
    d.ssn,
    DATE_OF_BIRTH,
    trunc(months_between(CURRENT_DATE, DATE_OF_BIRTH) / 12) age,
    TERMINATION_DATE,
    TERMINATION_CODE_ID,
    ENROLLMENT_ID,
    ZERO_CONTRIBUTION_REASON_ID,
    CURRENT_HOURS_YEAR,                        -- if you have over > 1000 your years gets a bump 
    nvl(years.years_in_plan,0) YEARS_IN_PLAN,  -- probably better named PRIOR_YEARS_IN_PLAN as it assumes the current profit_year is excluded
    INITIAL_CONTR_YEAR,
    CASE WHEN 
         trunc(months_between(CURRENT_DATE, DATE_OF_BIRTH) / 12) >= 65
        AND initial_contr_year < (EXTRACT(YEAR FROM CURRENT_DATE) - 5) 
        AND (TERMINATION_DATE IS NULL
            OR EXTRACT(YEAR FROM TERMINATION_DATE) >= EXTRACT(YEAR FROM CURRENT_DATE)) THEN
             1
         WHEN ENROLLMENT_ID IN (3, 4) THEN
             1
         WHEN TERMINATION_CODE_ID = 'Z' THEN
             1
         WHEN ZERO_CONTRIBUTION_REASON_ID = 6 THEN
             1
         WHEN (
                  CASE WHEN ENROLLMENT_ID = 2 THEN  -- The 2 means the newer plan which vests 1 year faster than the old plan.  
                           1
                       ELSE
                           0
                      END + CASE WHEN CURRENT_HOURS_YEAR >= 1000 THEN -- If the employee has more than 1000 hours in this year, then bumped up the vesting year
                                     1
                                 ELSE
                                     0
                      END + YEARS_IN_PLAN) < 3 THEN
             0  -- if the total years is less than 3, then 0 is the vested percent.
         WHEN (
                  CASE WHEN ENROLLMENT_ID = 2 THEN
                           1
                       ELSE
                           0
                      END + CASE WHEN CURRENT_HOURS_YEAR >= 1000 THEN
                                     1
                                 ELSE
                                     0
                      END + YEARS_IN_PLAN) = 3 THEN
             0.2
         WHEN (
                  CASE WHEN ENROLLMENT_ID = 2 THEN
                           1
                       ELSE
                           0
                      END + CASE WHEN CURRENT_HOURS_YEAR >= 1000 THEN
                                     1
                                 ELSE
                                     0
                      END + YEARS_IN_PLAN) = 4 THEN
             0.4
         WHEN (
                  CASE WHEN ENROLLMENT_ID = 2 THEN
                           1
                       ELSE
                           0
                      END + CASE WHEN CURRENT_HOURS_YEAR >= 1000 THEN
                                     1
                                 ELSE
                                     0
                      END + YEARS_IN_PLAN) = 5 THEN
             0.6
         WHEN (
                  CASE WHEN ENROLLMENT_ID = 2 THEN
                           1
                       ELSE
                           0
                      END + CASE WHEN CURRENT_HOURS_YEAR >= 1000 THEN
                                     1
                                 ELSE
                                     0
                      END + YEARS_IN_PLAN) = 6 THEN
             0.8
         WHEN (
                  CASE WHEN ENROLLMENT_ID = 2 THEN
                           1
                       ELSE
                           0
                      END + CASE WHEN CURRENT_HOURS_YEAR >= 1000 THEN
                                     1
                                 ELSE
                                     0
                      END + YEARS_IN_PLAN) >= 7 THEN
             1
         ELSE
             0
        END AS vested_percent
FROM
    demographic d
        JOIN pay_profit p ON d.id = p.demographic_id
        left outer join (select ssn, sum(YEARS_OF_SERVICE_CREDIT) YEARS_IN_PLAN from profit_detail where profit_year < (select max(profit_year) from pay_profit) group by ssn) years on years.ssn = d.ssn
        LEFT JOIN (
            SELECT ssn, MIN(profit_year) INITIAL_CONTR_YEAR
            FROM PROFIT_DETAIL
            WHERE PROFIT_CODE_ID = 0
              AND CONTRIBUTION  !=0
            group by ssn) initcontrib on d.ssn = initcontrib.ssn
where profit_year = (select max(profit_year) from pay_profit)
