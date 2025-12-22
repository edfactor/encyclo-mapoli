-- DEV/QA ONLY
-- Creates (or updates) a Demographic to be age 18-21 and ensures a non-zero balance
-- by inserting a PROFIT_DETAIL row that is included by the TotalService balance formula.
--
-- WARNING:
-- - Do NOT run in production.
-- - This script directly updates DEMOGRAPHIC.DATE_OF_BIRTH and inserts into PROFIT_DETAIL.
-- - It is intended only for local/dev/test data seeding.
--
-- How TotalService defines balance (EmbeddedSqlService.GetBalanceSubquery):
-- balance = SUM(code0 contribution)
--         + SUM(code0 earnings)
--         + SUM(code0 forfeiture)
--         - SUM(code1/3/5 forfeiture)
--         - SUM(code2 forfeiture)
--         + SUM(code6 contribution)
--         + SUM(code8 earnings)
--         - SUM(code9 forfeiture)
--
-- This script inserts a PROFIT_CODE_ID=0 contribution so total != 0.

-- DBeaver: use the DBMS Output view to see DBMS_OUTPUT lines.
-- (SET SERVEROUTPUT ON is SQL*Plus-specific and can be omitted.)

DECLARE
    v_profit_year        NUMBER;
    v_target_age_years   NUMBER := 20; -- choose 18..21; 20 is a safe default

  v_seed_marker        NVARCHAR2(192) := 'seed_under21_member_with_balance.sql';

    v_member_id          NUMBER;
    v_badge_number       NUMBER;
    v_ssn                NUMBER;

    v_new_dob            DATE;
    v_rows               NUMBER;
BEGIN
    SELECT MAX(pd.profit_year)
      INTO v_profit_year
      FROM profit_detail pd;

    -- Pick a "member" row to mutate.
    -- Prefer an existing demographic that is NOT already under-21 to avoid accidental collisions with other tests.
    -- If you want a specific person, replace this SELECT with a WHERE clause on BADGE_NUMBER.
    SELECT id, badge_number, ssn
      INTO v_member_id, v_badge_number, v_ssn
      FROM (
          SELECT d.id, d.badge_number, d.ssn
            FROM demographic d
           WHERE d.date_of_death IS NULL
             AND d.date_of_birth < ADD_MONTHS(TRUNC(SYSDATE), -12 * 21)
           ORDER BY d.id
      )
     WHERE ROWNUM = 1;

    v_new_dob := ADD_MONTHS(TRUNC(SYSDATE), -12 * v_target_age_years);

    UPDATE demographic
       SET date_of_birth = v_new_dob,
         user_name = v_seed_marker,
         modified_at_utc = SYSTIMESTAMP
     WHERE id = v_member_id;

    -- If there are already PROFIT_DETAIL rows for this SSN in the current profit year with PROFIT_CODE_ID=0,
    -- do not add another. Otherwise insert one.
    SELECT COUNT(1)
      INTO v_rows
      FROM profit_detail pd
     WHERE pd.ssn = v_ssn
       AND pd.profit_year = v_profit_year
       AND pd.profit_year_iteration = 0
       AND pd.profit_code_id = 0;

    IF v_rows = 0 THEN
        INSERT INTO profit_detail (
            ssn,
            profit_year,
            profit_year_iteration,
            distribution_sequence,
            profit_code_id,
            contribution,
            earnings,
            forfeiture,
            month_to_date,
            year_to_date,
            federal_taxes,
            state_taxes,
            years_of_service_credit,
            created_at_utc,
            user_name
        ) VALUES (
            v_ssn,
            v_profit_year,
            0,
            0,
            0,
            1000.00, -- contribution creates a positive balance
            0.00,
            0.00,
            0,
            0,
            0.00,
            0.00,
            0,
            SYSTIMESTAMP,
            v_seed_marker
        );
    END IF;

    COMMIT;

    DBMS_OUTPUT.PUT_LINE('Seed complete:');
    DBMS_OUTPUT.PUT_LINE('  Profit Year : ' || v_profit_year);
    DBMS_OUTPUT.PUT_LINE('  Member ID   : ' || v_member_id);
    DBMS_OUTPUT.PUT_LINE('  Badge       : ' || v_badge_number);
    DBMS_OUTPUT.PUT_LINE('  SSN         : ' || v_ssn);
    DBMS_OUTPUT.PUT_LINE('  New DOB     : ' || TO_CHAR(v_new_dob, 'YYYY-MM-DD'));
    DBMS_OUTPUT.PUT_LINE('  Inserted PD : ' || CASE WHEN v_rows = 0 THEN 'YES' ELSE 'NO (already had code0 row)' END);
    DBMS_OUTPUT.PUT_LINE('  Seed Marker : ' || v_seed_marker);
END;
/

-- Verification query (shows DOB and computed balance using the same formula as TotalService)
SELECT
    d.badge_number,
    d.ssn,
    d.date_of_birth,
    TRUNC(MONTHS_BETWEEN(TRUNC(SYSDATE), d.date_of_birth) / 12) AS age_years,
    (
        SUM(CASE WHEN pd.profit_code_id = 0 THEN pd.contribution ELSE 0 END)
      + SUM(CASE WHEN pd.profit_code_id IN (0) THEN pd.earnings ELSE 0 END)
      + SUM(CASE WHEN pd.profit_code_id = 0 THEN pd.forfeiture ELSE 0 END)
      + SUM(CASE WHEN pd.profit_code_id IN (1, 3, 5) THEN pd.forfeiture * -1 ELSE 0 END)
      + SUM(CASE WHEN pd.profit_code_id = 2 THEN pd.forfeiture * -1 ELSE 0 END)
      + (
            SUM(CASE WHEN pd.profit_code_id = 6 THEN pd.contribution ELSE 0 END)
          + SUM(CASE WHEN pd.profit_code_id = 8 THEN pd.earnings ELSE 0 END)
          + SUM(CASE WHEN pd.profit_code_id = 9 THEN pd.forfeiture * -1 ELSE 0 END)
        )
    ) AS total_balance
FROM demographic d
JOIN profit_detail pd
  ON pd.ssn = d.ssn
WHERE d.id = (
    SELECT id
    FROM (
        SELECT d2.id
        FROM demographic d2
        WHERE d2.user_name = 'seed_under21_member_with_balance.sql'
        ORDER BY d2.modified_at_utc DESC
    )
    WHERE ROWNUM = 1
)
  AND pd.profit_year <= (
      SELECT MAX(pd2.profit_year)
      FROM profit_detail pd2
  )
GROUP BY d.badge_number, d.ssn, d.date_of_birth;
