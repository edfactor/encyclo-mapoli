CREATE OR REPLACE VIEW vested_percent AS
SELECT
    dem_ssn ssn,
    TO_DATE(py_dob, 'yyyymmdd') DATE_OF_BIRTH,
    trunc(months_between (CURRENT_DATE, TO_DATE(py_dob, 'yyyymmdd')) / 12) age,    
    case when (py_term_dt IS NULL
        OR py_term_dt = '19000101'
        OR py_term_dt = '0')
             then null
         else TO_DATE(py_term_dt, 'yyyymmdd')
        end as TERMINATION_DATE,
    case when py_term =' ' then null else TO_NCHAR(py_term) end as TERMINATION_CODE_ID,
    py_ps_enrolled,
    py_prof_zerocont,
    py_ph,
    py_ps_years,
    initial_contr_year,
    CASE WHEN
             trunc(months_between (CURRENT_DATE, TO_DATE(py_dob, 'yyyymmdd')) / 12) >= 65
                 AND initial_contr_year < (EXTRACT(YEAR FROM CURRENT_DATE) - 5 )
                 AND (py_term_dt IS NULL
                 OR py_term_dt = '19000101'
                 OR py_term_dt = '0'
                 OR EXTRACT(YEAR FROM TO_DATE(py_term_dt, 'yyyymmdd')) >= EXTRACT(YEAR FROM CURRENT_DATE)) THEN
             1
         WHEN py_ps_enrolled IN (3, 4) THEN
             1
         WHEN py_term = 'Z' THEN
             1
         WHEN py_prof_zerocont = 6 THEN
             1
         WHEN (
                  CASE WHEN py_ps_enrolled = 2 THEN  -- The 2 means the newer plan which vests 1 year faster than the old plan.  
                           1
                       ELSE
                           0
                      END + CASE WHEN py_ph >= 1000 THEN -- If the employee has more than 1000 hours in this year, then get bumped up
                                     1
                                 ELSE
                                     0
                      END + py_ps_years) < 3 THEN
             0
         WHEN (
                  CASE WHEN py_ps_enrolled = 2 THEN
                           1
                       ELSE
                           0
                      END + CASE WHEN py_ph >= 1000 THEN
                                     1
                                 ELSE
                                     0
                      END + py_ps_years) = 3 THEN
             0.2
         WHEN (
                  CASE WHEN py_ps_enrolled = 2 THEN
                           1
                       ELSE
                           0
                      END + CASE WHEN py_ph >= 1000 THEN
                                     1
                                 ELSE
                                     0
                      END + py_ps_years) = 4 THEN
             0.4
         WHEN (
                  CASE WHEN py_ps_enrolled = 2 THEN
                           1
                       ELSE
                           0
                      END + CASE WHEN py_ph >= 1000 THEN
                                     1
                                 ELSE
                                     0
                      END + py_ps_years) = 5 THEN
             0.6
         WHEN (
                  CASE WHEN py_ps_enrolled = 2 THEN
                           1
                       ELSE
                           0
                      END + CASE WHEN py_ph >= 1000 THEN
                                     1
                                 ELSE
                                     0
                      END + py_ps_years) = 6 THEN
             .8
         WHEN (
                  CASE WHEN py_ps_enrolled = 2 THEN
                           1
                       ELSE
                           0
                      END + CASE WHEN py_ph >= 1000 THEN
                                     1
                                 ELSE
                                     0
                      END + py_ps_years) >= 7 THEN
             1
         ELSE
             0
        END AS vested_percent
FROM
    demographics d
  LEFT OUTER JOIN (
          SELECT PR_DET_S_SEC_NUMBER ssn, MIN(floor(profit_year)) INITIAL_CONTR_YEAR
            FROM PROFIT_DETAIL
            WHERE PROFIT_CODE = 0 AND PROFIT_CONT  !=0 
            group by PR_DET_S_SEC_NUMBER
    ) initcontrib on d.dem_ssn = initcontrib.ssn           
    JOIN payprofit p ON d.dem_ssn = p.payprof_ssn