/*
    These functions will fill the SSN_TASKS and PSN_TASKS 
    tables with rows that indicate what needs to be changed where for each SSN and PSN that is in these tables.

    These rows represent the "work" to be done to change 
    an SSN or PSN across all the tables that might have 
    them. This may seem to be an odd way to approach this, 
    but it has these advantages: since each row as a 'done' 
    flag, it means that if the process is interrupted, you 
    can just re-run and it will start with unfinished rows. 
    Also, you can, after a run, verify from the 'yes/no' 
    columns what tables had or did not have a SSN or PSN 
    for that person. This helps a lot in debugging and also 
    in verification after a run.

    There are four functions: two for SSNs for employees and beneficiaries, and two for PSNs each for employees and beneficiaries
*/

CREATE OR REPLACE FUNCTION FILL_SSN_TABLES_EMPLOYEE RETURN NUMBER IS
 
 -- This is for batch reading of tables into nested tables
  BATCH_READ_SIZE  PLS_INTEGER := 200;
  
 -- We need to set up a cursor to read in the fields we need

    CURSOR GET_DEMOGRAPHICS_SSN_CUR IS
    SELECT
        DEMO.DEM_SSN,
        DEMO.PY_ADD,
        DEMO.PY_NAM,
        DEMO.PY_CITY,
        DEMO.PY_STATE,
        DEMO.PY_ZIP
    FROM DEMOGRAPHICS DEMO;

    TYPE EMPLOYEE_SSN_REC IS RECORD ( 
        SSN NUMBER(9),
        ADDRESS VARCHAR2(30), 
        FULL_NAME VARCHAR2(40),
        CITY VARCHAR2(25), 
        US_STATE CHAR(2), 
        ZIP NUMBER(5) );
    
-- Our Nested Table type
    TYPE EMPLOYEE_SSNs_NT IS TABLE OF EMPLOYEE_SSN_REC INDEX BY PLS_INTEGER;
-- This is the intialized instance of our nested table
    L_EMPLOYEE_SSNs_NT EMPLOYEE_SSNs_NT;
    
    NUM_RECORDS NUMBER := 0;
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);
    
    L_ERROR_COUNT NUMBER;
    L_BATCH_NUMBER NUMBER := 0;

BEGIN
        L_EMPLOYEE_SSNs_NT := EMPLOYEE_SSNs_NT();
        
        OPEN GET_DEMOGRAPHICS_SSN_CUR;
        LOOP FETCH GET_DEMOGRAPHICS_SSN_CUR BULK COLLECT INTO L_EMPLOYEE_SSNs_NT LIMIT BATCH_READ_SIZE;
        EXIT WHEN L_EMPLOYEE_SSNs_NT.COUNT = 0;  
        L_BATCH_NUMBER := L_BATCH_NUMBER + 1;
        --dbms_output.put_line('Batch number : ' || L_BATCH_NUMBER);
        
        -- The odd select statements inside the values of the insert statement
        -- below see if the ID we want is present in the index table for that 
        -- table, indicating that it will need to be changed in that table
        FORALL INDX IN 1 .. L_EMPLOYEE_SSNs_NT.COUNT SAVE EXCEPTIONS
            INSERT INTO SSN_TASKS (
              DEMOGRAPHICS_SSN,
              PAYBEN_PYBEN_PAYSSN,
              PAYREL_PAYSSN,
              PROFDIST_SSN,
              PROFDIST_PAYSSN,
              PROFIT_SHARE_CHECKS_EMPLOYEE_SSN,
              PROFIT_SHARE_CHECKS_SSN_NUMBER,
              PROFIT_DETAIL_PR_DET_S_SEC_NUMBER,
              PR_SS_D_S_SEC_NUMBER,
              PAYPROFIT_SSN,
              SOC_SEC_SSN,
              NEW_ADDRESS,
              NEW_CITY,
              NEW_STATE,
              NEW_ZIP,
              EMP_NAME
            )
              SELECT
                *
              FROM
                (
                  SELECT L_EMPLOYEE_SSNs_NT(INDX).SSN FROM DUAL
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PYBEN_PAYSSN WHERE SSN = L_EMPLOYEE_SSNs_NT(INDX).SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PYREL_PAYSSN WHERE SSN = L_EMPLOYEE_SSNs_NT(INDX).SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PROFDIST_SSN WHERE SSN = L_EMPLOYEE_SSNs_NT(INDX).SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PROFDIST_PAYSSN WHERE SSN = L_EMPLOYEE_SSNs_NT(INDX).SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PS_CHECKS_EMP_SSN WHERE SSN = L_EMPLOYEE_SSNs_NT(INDX).SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PS_CHECKS_SSN WHERE SSN = L_EMPLOYEE_SSNs_NT(INDX).SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM  PROFIT_DETAIL_SSN  WHERE SSN = L_EMPLOYEE_SSNs_NT(INDX).SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM  PROFIT_SS_DETAIL_SSN  WHERE SSN = L_EMPLOYEE_SSNs_NT(INDX).SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM  PAYPROFIT_SSN  WHERE SSN = L_EMPLOYEE_SSNs_NT(INDX).SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM  SOC_SEC_SSN  WHERE SSN = L_EMPLOYEE_SSNs_NT(INDX).SSN
                ),
                (
                  SELECT
                    L_EMPLOYEE_SSNs_NT(INDX).ADDRESS
                  FROM
                    DUAL
                ),
                (
                  SELECT
                    L_EMPLOYEE_SSNs_NT(INDX).CITY
                  FROM
                    DUAL
                ),
                (
                  SELECT
                    L_EMPLOYEE_SSNs_NT(INDX).US_STATE
                  FROM
                    DUAL
                ),
                (
                  SELECT
                    L_EMPLOYEE_SSNs_NT(INDX).ZIP
                  FROM
                    DUAL
                ),
                (
                  SELECT
                   L_EMPLOYEE_SSNs_NT(INDX).FULL_NAME
                  FROM
                    DUAL
                );
                        
        END LOOP;

        CLOSE GET_DEMOGRAPHICS_SSN_CUR;


        
 RETURN 0;
 
EXCEPTION
    WHEN OTHERS
    THEN
        DBMS_OUTPUT.put_line ('Rows inserted : ' || SQL%ROWCOUNT);
        l_error_count := SQL%BULK_EXCEPTIONS.count;
        DBMS_OUTPUT.put_line('Number of failures: ' || l_error_count);
        FOR indx IN 1 .. SQL%BULK_EXCEPTIONS.COUNT
          LOOP
             DBMS_OUTPUT.put_line (
                   'Error '
                || indx
                || ' occurred on index '
                || SQL%BULK_EXCEPTIONS (indx).ERROR_INDEX
                || ' attempting to fill SSN tasks tables for employees. ');
             DBMS_OUTPUT.put_line (
                   'Oracle error is : '
                || SQLERRM(-SQL%BULK_EXCEPTIONS(indx).ERROR_CODE));
                   
          END LOOP;
        
          ROLLBACK;

 
END;
/
CREATE OR REPLACE FUNCTION FILL_SSN_TABLES_BENEFICIARY RETURN NUMBER IS
  
 -- This is for batch reading of tables into nested tables
  BATCH_READ_SIZE  PLS_INTEGER := 200;
  
 -- We need to set up a cursor to read in the fields we need

    CURSOR GET_BENEFICIARY_SSN_CUR IS
    SELECT
        BEN.PYBEN_PAYSSN,
        BEN.PYBEN_NAME,
        BEN.PYBEN_ADD,
        BEN.PYBEN_CITY,
        BEN.PYBEN_STATE,
        BEN.PYBEN_ZIP
    FROM PAYBEN BEN;

    TYPE BENEFICIARY_SSN_REC IS RECORD ( 
        PAY_SSN NUMBER(9),
        FULL_NAME CHAR(25),
        ADDRESS CHAR(20), 
        CITY CHAR(13), 
        US_STATE CHAR(2), 
        ZIP NUMBER(5) );
    
-- Our Nested Table type
    TYPE BENEFICIARY_SSNs_NT IS TABLE OF BENEFICIARY_SSN_REC INDEX BY PLS_INTEGER;
-- This is the intialized instance of our nested table
    L_BENEFICIARY_SSNs_NT BENEFICIARY_SSNs_NT;
    
-- Number of records
    NUM_RECORDS NUMBER := 0;
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);
    
    L_ERROR_COUNT NUMBER;
    L_BATCH_NUMBER NUMBER := 0;

BEGIN
        L_BENEFICIARY_SSNs_NT := BENEFICIARY_SSNs_NT();
        OPEN GET_BENEFICIARY_SSN_CUR;
        LOOP FETCH GET_BENEFICIARY_SSN_CUR BULK COLLECT INTO L_BENEFICIARY_SSNs_NT LIMIT BATCH_READ_SIZE;
        EXIT WHEN L_BENEFICIARY_SSNs_NT.COUNT = 0;  
        L_BATCH_NUMBER := L_BATCH_NUMBER + 1;
        
        -- The odd select statements below see if the ID we want is present in the index table for that table, indicating
        -- that it will need to be changed in that table
        FORALL INDX IN 1 .. L_BENEFICIARY_SSNs_NT.COUNT SAVE EXCEPTIONS
            INSERT INTO SSN_TASKS (
              PYBEN_PAYSSN,
              PAYREL_PAYSSN,
              PROFDIST_SSN,
              PROFDIST_PAYSSN,
              PROFIT_SHARE_CHECKS_SSN_NUMBER,
              PROFIT_DETAIL_PR_DET_S_SEC_NUMBER,
              SOC_SEC_SSN,
              PAYPROFIT_SSN,
              BEN_NAME,
              NEW_ADDRESS,
              NEW_CITY,
              NEW_STATE,
              NEW_ZIP
            )
              SELECT
                *
              FROM
                (
                  SELECT L_BENEFICIARY_SSNs_NT(INDX).PAY_SSN FROM DUAL
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                   FROM PYREL_PAYSSN WHERE SSN = L_BENEFICIARY_SSNs_NT(INDX).PAY_SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                FROM PROFDIST_SSN WHERE SSN = L_BENEFICIARY_SSNs_NT(INDX).PAY_SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PROFDIST_PAYSSN WHERE SSN = L_BENEFICIARY_SSNs_NT(INDX).PAY_SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                   FROM PS_CHECKS_SSN WHERE SSN = L_BENEFICIARY_SSNs_NT(INDX).PAY_SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PROFIT_DETAIL_SSN WHERE SSN = L_BENEFICIARY_SSNs_NT(INDX).PAY_SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM SOC_SEC_SSN WHERE SSN = L_BENEFICIARY_SSNs_NT(INDX).PAY_SSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PAYPROFIT_SSN WHERE SSN = L_BENEFICIARY_SSNs_NT(INDX).PAY_SSN
                ),
                (
                  SELECT
                    L_BENEFICIARY_SSNs_NT(INDX).FULL_NAME
                  FROM
                    DUAL
                ),
                (
                  SELECT
                    L_BENEFICIARY_SSNs_NT(INDX).ADDRESS
                  FROM
                    DUAL
                ),
                (
                  SELECT
                    L_BENEFICIARY_SSNs_NT(INDX).CITY
                  FROM
                    DUAL
                ),
                (
                  SELECT
                    L_BENEFICIARY_SSNs_NT(INDX).US_STATE
                  FROM
                    DUAL
                ),
                (
                  SELECT
                   L_BENEFICIARY_SSNs_NT(INDX).ZIP
                  FROM
                    DUAL
                );
                        
        END LOOP;

        CLOSE GET_BENEFICIARY_SSN_CUR;


 RETURN 0;
 
EXCEPTION
    WHEN OTHERS
    THEN
        DBMS_OUTPUT.put_line ('Rows inserted : ' || SQL%ROWCOUNT);
        l_error_count := SQL%BULK_EXCEPTIONS.count;
        DBMS_OUTPUT.put_line('Number of failures: ' || l_error_count);
        FOR indx IN 1 .. SQL%BULK_EXCEPTIONS.COUNT
          LOOP
             DBMS_OUTPUT.put_line (
                   'Error '
                || indx
                || ' occurred on index '
                || SQL%BULK_EXCEPTIONS (indx).ERROR_INDEX
                || ' attempting to fill SSN tasks table for beneficiaries. ');
             DBMS_OUTPUT.put_line (
                   'Oracle error is : '
                || SQLERRM(-SQL%BULK_EXCEPTIONS(indx).ERROR_CODE));
                   
          END LOOP;
        
          ROLLBACK;

 
END;
/

CREATE OR REPLACE FUNCTION FILL_PSN_TABLES_EMPLOYEE RETURN NUMBER IS
 
 -- This is for batch reading of tables into nested tables
    BATCH_READ_SIZE  PLS_INTEGER := 200;
  
 -- We need to set up a cursor to read in the fields we need

    CURSOR GET_DEMOGRAPHICS_PSN_CUR IS
    SELECT 
        DEMO.DEM_BADGE
    FROM 
        DEMOGRAPHICS DEMO;
    
-- Our Nested Table type
    TYPE EMPLOYEE_PSNs_NT IS TABLE OF NUMBER(9) INDEX BY PLS_INTEGER;
-- This is the intialized instance of our nested table
    L_EMPLOYEE_PSNs_NT EMPLOYEE_PSNs_NT;
    
-- Number of records
    NUM_RECORDS NUMBER := 0;
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);
    
    L_ERROR_COUNT NUMBER;
    
    L_BATCH_NUMBER NUMBER := 0;

BEGIN
        L_EMPLOYEE_PSNs_NT := EMPLOYEE_PSNs_NT();
        OPEN GET_DEMOGRAPHICS_PSN_CUR;
        LOOP FETCH GET_DEMOGRAPHICS_PSN_CUR BULK COLLECT INTO L_EMPLOYEE_PSNs_NT LIMIT BATCH_READ_SIZE;
        EXIT WHEN L_EMPLOYEE_PSNs_NT.COUNT = 0;  
        L_BATCH_NUMBER := L_BATCH_NUMBER + 1;
        
        
        
        -- The odd select statements below see if the ID we want is present in the index table for that table, indicating
        -- that it will need to be changed in that table
        FORALL INDX IN 1 .. L_EMPLOYEE_PSNs_NT.COUNT SAVE EXCEPTIONS
            INSERT INTO PSN_TASKS (
              DEMOGRAPHICS_PSN,
              PROF_DIST_REQ_PSN,
              PROF_DIST_REQ_EMP,
              PAYPROFIT_PSN,
              PAYREL_PSN,
              PROFIT_CMNT_PSN,
              PROFIT_SS_CMNT_PSN --,
              --PROFIT_SHARE_CHECKS_EMPLOYEE_NUMBER
            )
              SELECT
                *
              FROM
                (
                  SELECT L_EMPLOYEE_PSNs_NT(INDX) FROM DUAL
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PROF_DIST_REQ_PSN 
                  WHERE PSN = L_EMPLOYEE_PSNs_NT(INDX)
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PROF_DIST_REQ_EMP
                  WHERE PSN = L_EMPLOYEE_PSNs_NT(INDX)
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PAYPROFIT_PSN
                  WHERE PSN = L_EMPLOYEE_PSNs_NT(INDX)
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PYREL_PSN
                  WHERE PSN = L_EMPLOYEE_PSNs_NT(INDX)
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PROFIT_CMNT_PSN
                  WHERE PROFIT_PSN = (L_EMPLOYEE_PSNs_NT(INDX) * 10000)
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PROFIT_SS_CMNT_PSN
                  WHERE PROFIT_SS_PSN = (L_EMPLOYEE_PSNs_NT(INDX) * 10000)
                );
                /*
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PS_CHECKS_EMP_NUM
                  WHERE PSN = L_EMPLOYEE_PSNs_NT(INDX)
                );
                */
                        
        END LOOP;

        CLOSE GET_DEMOGRAPHICS_PSN_CUR;


    RETURN 0;
 
EXCEPTION
    WHEN OTHERS
    THEN
        DBMS_OUTPUT.put_line ('Rows inserted : ' || SQL%ROWCOUNT);
        l_error_count := SQL%BULK_EXCEPTIONS.count;
        DBMS_OUTPUT.put_line('Number of failures: ' || l_error_count);
        FOR indx IN 1 .. SQL%BULK_EXCEPTIONS.COUNT
          LOOP
             DBMS_OUTPUT.put_line (
                   'Error '
                || indx
                || ' occurred on index '
                || SQL%BULK_EXCEPTIONS (indx).ERROR_INDEX
                || ' attempting to fill PSN tasks table for employees ');
             DBMS_OUTPUT.put_line (
                   'Oracle error is : '
                || SQLERRM(-SQL%BULK_EXCEPTIONS(indx).ERROR_CODE));
                   
          END LOOP;
        
          ROLLBACK; 
END;
/
CREATE OR REPLACE FUNCTION FILL_PSN_TABLES_BENEFICIARY RETURN NUMBER IS
 
 -- This is for batch reading of tables into nested tables
    BATCH_READ_SIZE  PLS_INTEGER := 200;

 /* 
  We need to set up a cursor to read in the fields we need. Note that
  we do not only need the PSN that we intend to replace, we need to look 
  up the employee in the tasks table to see what the replacement PSN is
  for that employee so we can take our suffix (1000, 1100, 2000, etc)
  and add it to that for the replacement for the beneficiary

*/
    CURSOR GET_PAYBEN_PSN_CUR IS
    SELECT 
        PAYBEN.PYBEN_PSN,
        -- Getting the replacement
        PSN_TASKS.REPLACEMENT_PSN
    FROM 
        PAYBEN, PSN_TASKS
    WHERE
        -- Where the original PSN to be replaced matches our base
        PSN_TASKS.DEMOGRAPHICS_PSN = SUBSTR(PAYBEN.PYBEN_PSN, 1, LENGTH(PAYBEN.PYBEN_PSN) - 4);
    
    TYPE BENEFICIARY_PSN_REC IS RECORD ( 
        PSN NUMBER(11),
        BASE_PSN NUMBER(7) );
-- Our Nested Table type
    TYPE BENEFICIARY_PSNs_NT IS TABLE OF BENEFICIARY_PSN_REC INDEX BY PLS_INTEGER;
-- This is the intialized instance of our nested table
    L_BENEFICIARY_PSNs_NT BENEFICIARY_PSNs_NT;
    
-- Number of records
    NUM_RECORDS NUMBER := 0;
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);
    
    L_ERROR_COUNT NUMBER;
    
    L_BATCH_NUMBER NUMBER := 0;

BEGIN
        L_BENEFICIARY_PSNs_NT := BENEFICIARY_PSNs_NT();
       
        OPEN GET_PAYBEN_PSN_CUR;
        LOOP FETCH GET_PAYBEN_PSN_CUR BULK COLLECT INTO L_BENEFICIARY_PSNs_NT LIMIT BATCH_READ_SIZE;
        EXIT WHEN L_BENEFICIARY_PSNs_NT.COUNT = 0;  
        L_BATCH_NUMBER := L_BATCH_NUMBER + 1;
        --dbms_output.put_line('About to do batch: ' || L_BATCH_NUMBER);
        
 
       -- The odd select statements below see if the ID we want is present in the index table for that table, indicating
        -- that it will need to be changed in that table
        FORALL INDX IN 1 .. L_BENEFICIARY_PSNs_NT.COUNT SAVE EXCEPTIONS
            INSERT INTO PSN_TASKS (
              PAYBEN_PSN,
              PROF_DIST_REQ_PSN,
              PAYREL_PSN,
              REPLACEMENT_BEN_PSN,
              PROFIT_CMNT_PSN,
              PROFIT_SS_CMNT_PSN
            )
              SELECT
                *
              FROM
                (
                  SELECT L_BENEFICIARY_PSNs_NT(INDX).PSN FROM DUAL
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PROF_DIST_REQ_PSN
                  WHERE PSN = L_BENEFICIARY_PSNs_NT(INDX).PSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PYREL_PSN
                  WHERE PSN = L_BENEFICIARY_PSNs_NT(INDX).PSN
                ),
                (
                  SELECT
                  TO_NUMBER(L_BENEFICIARY_PSNs_NT(INDX).BASE_PSN
                      || SUBSTR(L_BENEFICIARY_PSNs_NT(INDX).PSN, -4))
                  FROM
                  DUAL
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PROFIT_CMNT_PSN
                  WHERE PROFIT_PSN = L_BENEFICIARY_PSNs_NT(INDX).PSN
                ),
                (
                  SELECT
                    CASE
                      WHEN COUNT(*) > 0 THEN
                        1
                      ELSE
                        0
                    END
                  FROM PROFIT_SS_CMNT_PSN
                  WHERE PROFIT_SS_PSN = L_BENEFICIARY_PSNs_NT(INDX).PSN
                );
                        
        END LOOP;

        CLOSE GET_PAYBEN_PSN_CUR;
 

    RETURN 0;
 
EXCEPTION
    WHEN OTHERS
    THEN
        dbms_output.put_line('ERROR_STACK: ' || DBMS_UTILITY.FORMAT_ERROR_STACK);
        dbms_output.put_line('ERROR_BACKTRACE: ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
        DBMS_OUTPUT.put_line ('Rows inserted : ' || SQL%ROWCOUNT);
        l_error_count := SQL%BULK_EXCEPTIONS.count;
        DBMS_OUTPUT.put_line('Number of failures: ' || l_error_count);
        FOR indx IN 1 .. SQL%BULK_EXCEPTIONS.COUNT
          LOOP
             DBMS_OUTPUT.put_line (
                   'Error '
                || indx
                || ' occurred on index '
                || SQL%BULK_EXCEPTIONS (indx).ERROR_INDEX
                || ' attempting to fill PSN tasks table for beneficiaries ');
             DBMS_OUTPUT.put_line (
                   'Oracle error is : '
                || SQLERRM(-SQL%BULK_EXCEPTIONS(indx).ERROR_CODE));
                   
          END LOOP;
        
          ROLLBACK; 
END;