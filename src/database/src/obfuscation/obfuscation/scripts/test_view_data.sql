DECLARE
    LOOP_FLAG                  BOOLEAN:=FALSE;
    L_SQL_ERROR                NUMBER;
    L_SQL_ERRM                 VARCHAR2(512);
BEGIN
 /*
    This script prints out data about the first six employees
    in the database and selected data related to each employee
*/
    DBMS_OUTPUT.PUT_LINE('First Six Employees');
    DBMS_OUTPUT.PUT_LINE('-----------------------------------------------------------');
    FOR R_EMPLOYEES IN (
        SELECT
            DEM_BADGE,
            DEM_SSN,
            PY_NAM,
            PY_LNAME,
            PY_FNAME,
            PY_MNAME,
            PY_ADD,
            PY_ADD2,
            PY_CITY,
            PY_STATE,
            PY_ZIP,
            PY_DOB,
            PY_HIRE_DT,
            PY_GENDER
        FROM
            DEMOGRAPHICS
        ORDER BY
            DEM_BADGE ASC FETCH FIRST 6 ROWS ONLY
    ) LOOP
        DBMS_OUTPUT.PUT_LINE(R_EMPLOYEES.PY_FNAME
                             || ' '
                             || R_EMPLOYEES.PY_MNAME
                             || ' '
                             || R_EMPLOYEES.PY_LNAME
                             || ' BADGE (PSN): '
                             || R_EMPLOYEES.DEM_BADGE
                             || ' SSN: '
                             || R_EMPLOYEES.DEM_SSN
                             || ' '
                             || R_EMPLOYEES.PY_ADD
                             || ' '
                             || R_EMPLOYEES.PY_CITY
                             || ' '
                             || R_EMPLOYEES.PY_STATE
                             || ' '
                             || R_EMPLOYEES.PY_ZIP
                             || ' '
                             || R_EMPLOYEES.PY_DOB
                             || ' '
                             || R_EMPLOYEES.PY_HIRE_DT
                             || ' '
                             || R_EMPLOYEES.PY_GENDER);
        DBMS_OUTPUT.PUT_LINE('Relatives for: '
                             || R_EMPLOYEES.PY_FNAME);
        FOR R_RELATIVES IN (
            SELECT
                PYREL_PSN,
                PYREL_PAYSSN,
                PYREL_PERCENT,
                PYREL_RELATION
            FROM
                PAYREL
            WHERE
                PYREL_PSN = R_EMPLOYEES.DEM_BADGE
            ORDER BY
                PYREL_PSN ASC
        ) LOOP
            LOOP_FLAG := TRUE;
            DBMS_OUTPUT.PUT_LINE(
                                R_RELATIVES.PYREL_PSN
                                || ' PAY SSN: '
                                 || R_RELATIVES.PYREL_PAYSSN
                                 || ' '
                                 || R_RELATIVES.PYREL_PERCENT
                                 || ' '
                                 || R_RELATIVES.PYREL_RELATION );
        END LOOP;

        IF NOT LOOP_FLAG THEN
            DBMS_OUTPUT.PUT_LINE ('No relatives found...'
                                  || CHR(10));
        ELSE
 -- need to reset flag
            LOOP_FLAG := FALSE;
            DBMS_OUTPUT.PUT_LINE(' '
                                 || CHR(10));
        END IF;

        DBMS_OUTPUT.PUT_LINE('Beneficiaries for: '
                             || R_EMPLOYEES.PY_FNAME);
        FOR R_BENEFICIARIES IN (
            SELECT
                PYBEN_PSN,
                PYBEN_PAYSSN,
                PYBEN_NAME,
                PYBEN_ADD,
                PYBEN_CITY,
                PYBEN_STATE,
                PYBEN_ZIP
            FROM
                PAYBEN
            WHERE
                SUBSTR(PYBEN_PSN, 1, LENGTH(PYBEN_PSN) - 4) = R_EMPLOYEES.DEM_BADGE
            ORDER BY
                PYBEN_NAME ASC
        ) LOOP
            LOOP_FLAG := TRUE;
            DBMS_OUTPUT.PUT_LINE(
                                R_BENEFICIARIES.PYBEN_NAME
                                || ' PSN: '
                                 || R_BENEFICIARIES.PYBEN_PSN
                                 || ' PAY SSN: '
                                 || R_BENEFICIARIES.PYBEN_PAYSSN
                                 || ' '
                                 || R_BENEFICIARIES.PYBEN_ADD
                                 || ' '
                                 || R_BENEFICIARIES.PYBEN_CITY
                                 || ' '
                                 || R_BENEFICIARIES.PYBEN_STATE
                                 || ' '
                                 || R_BENEFICIARIES.PYBEN_ZIP);
            
        END LOOP;

        IF NOT LOOP_FLAG THEN
            DBMS_OUTPUT.PUT_LINE ('No beneficiaries found...'
                                  || CHR(10));
        ELSE
 -- need to reset flag
            LOOP_FLAG := FALSE;
            DBMS_OUTPUT.PUT_LINE(' '
                                 || CHR(10));
        END IF;

        DBMS_OUTPUT.PUT_LINE('Distribution Requests for: '
                             || R_EMPLOYEES.PY_FNAME);
                             
        FOR R_DISTRIBUTIONREQS IN (
            SELECT
                PROFIT_DIST_REQ_SEQ_NUM,
                PROFIT_DIST_REQ_TYPE,
                PROFIT_DIST_REQ_EMP,
                PROFIT_DIST_REQ_PSN,
                PROFIT_DIST_REQ_REASON,
                PROFIT_DIST_REQ_REASON_OTHER
            FROM
                PROFIT_DIST_REQ
            WHERE
                PROFIT_DIST_REQ_EMP = (R_EMPLOYEES.DEM_BADGE)
            ORDER BY
                PROFIT_DIST_REQ_SEQ_NUM ASC
        ) LOOP
            LOOP_FLAG := TRUE;
            DBMS_OUTPUT.PUT_LINE(R_DISTRIBUTIONREQS.PROFIT_DIST_REQ_SEQ_NUM
                                 || ' EMP PSN:'
                                 || R_DISTRIBUTIONREQS.PROFIT_DIST_REQ_EMP
                                 || ' PSN: '
                                 || R_DISTRIBUTIONREQS.PROFIT_DIST_REQ_PSN
                                 || ' '
                                 || R_DISTRIBUTIONREQS.PROFIT_DIST_REQ_TYPE
                                 || ' '
                                 || R_DISTRIBUTIONREQS.PROFIT_DIST_REQ_REASON
                                 || ' '
                                 || R_DISTRIBUTIONREQS.PROFIT_DIST_REQ_REASON_OTHER);
        END LOOP;

        IF NOT LOOP_FLAG THEN
            DBMS_OUTPUT.PUT_LINE ('No distribution requests found...'
                                  || CHR(10));
        ELSE
 --need to reset flag
            LOOP_FLAG := FALSE;
            DBMS_OUTPUT.PUT_LINE(' '
                                 || CHR(10));
        END IF;

        
        

        DBMS_OUTPUT.PUT_LINE('Distributions for: '
                             || R_EMPLOYEES.PY_FNAME
                             || ' and beneficiaries: ');
        FOR R_DISTRIBUTIONS IN (
            SELECT
                PROFDIST_SSN,
                PROFDIST_PAYSEQ,
                PROFDIST_EMPNAME,
                PROFDIST_PAYSSN,
                PROFDIST_PAYNAME,
                PROFDIST_PAYADDR1,
                PROFDIST_PAYCITY,
                PROFDIST_PAYSTATE,
                PROFDIST_PAYZIP1
            FROM
                PROFDIST
            WHERE
                PROFDIST_SSN = (R_EMPLOYEES.DEM_SSN)
        ) LOOP
            LOOP_FLAG := TRUE;
            DBMS_OUTPUT.PUT_LINE(R_DISTRIBUTIONS.PROFDIST_PAYSEQ
                                 || ' NAME: '
                                 || R_DISTRIBUTIONS.PROFDIST_PAYNAME
                                 || ' PAY SSN: '
                                 || R_DISTRIBUTIONS.PROFDIST_PAYSSN
                                 || ' '
                                 || R_DISTRIBUTIONS.PROFDIST_PAYADDR1
                                 || ' '
                                 || R_DISTRIBUTIONS.PROFDIST_PAYCITY
                                 || ' '
                                 || R_DISTRIBUTIONS.PROFDIST_PAYSTATE
                                 || R_DISTRIBUTIONS.PROFDIST_PAYZIP1
                                 || ' EMP NAME: '
                                 || R_DISTRIBUTIONS.PROFDIST_EMPNAME
                                 ||' SSN: '
                                 || R_DISTRIBUTIONS.PROFDIST_SSN);
        END LOOP;

        IF NOT LOOP_FLAG THEN
            DBMS_OUTPUT.PUT_LINE ('No distributions found...'
                                  || CHR(10));
        ELSE
 --need to reset flag
            LOOP_FLAG := FALSE;
            DBMS_OUTPUT.PUT_LINE(' '
                                 || CHR(10));
        END IF;
        

        DBMS_OUTPUT.PUT_LINE('Profit Details for: '
                             || R_EMPLOYEES.PY_FNAME);
        FOR R_DETAILS IN (
            SELECT
                PROFIT_DET_RECNO,
                PROFIT_YEAR,
                PROFIT_CLIENT,
                PROFIT_CODE,
                PR_DET_S_SEC_NUMBER,
                PROFIT_CMNT,
                PROFIT_EARN,
                PROFIT_FORT
            FROM
                PROFIT_DETAIL
            WHERE
                PR_DET_S_SEC_NUMBER = (R_EMPLOYEES.DEM_SSN)
        ) LOOP
            LOOP_FLAG := TRUE;
            DBMS_OUTPUT.PUT_LINE(R_DETAILS.PROFIT_DET_RECNO
                                 || ' '
                                 || R_DETAILS.PROFIT_YEAR
                                 || ' '
                                 || R_DETAILS.PROFIT_CLIENT
                                 || ' '
                                 || R_DETAILS.PROFIT_CODE
                                 || ' SSN: '
                                 || R_DETAILS.PR_DET_S_SEC_NUMBER
                                 || ' COMMENT: '
                                 || R_DETAILS.PROFIT_CMNT
                                 || ' '
                                 || R_DETAILS.PROFIT_EARN
                                 || ' '
                                 || R_DETAILS.PROFIT_FORT);
        END LOOP;

        IF NOT LOOP_FLAG THEN
            DBMS_OUTPUT.PUT_LINE ('No details found...'
                                  || CHR(10));
        ELSE
 --need to reset flag
            LOOP_FLAG := FALSE;
            DBMS_OUTPUT.PUT_LINE(' '
                                 || CHR(10));
        END IF;

         DBMS_OUTPUT.PUT_LINE('Checks for: '
                             || R_EMPLOYEES.PY_FNAME
                             || ' and beneficiaries: ');
        
        FOR R_CHECKS IN (
            SELECT
                PAYABLE_NAME,
                SSN_NUMBER,
                CHECK_NUMBER,
                CHECK_DATE,
                CHECK_AMOUNT,
                EMPLOYEE_NUMBER,
                EMPLOYEE_SSN
            FROM
                PROFIT_SHARE_CHECKS
            WHERE
                EMPLOYEE_SSN = (R_EMPLOYEES.DEM_SSN)
        ) LOOP
            LOOP_FLAG := TRUE;
            DBMS_OUTPUT.PUT_LINE('CHECK #: ' || R_CHECKS.CHECK_NUMBER
                                 || ' '
                                 || R_CHECKS.PAYABLE_NAME
                                 || ' '
                                 || R_CHECKS.CHECK_DATE
                                 || ' '
                                 || R_CHECKS.CHECK_AMOUNT
                                 || ' '
                                 || R_CHECKS.EMPLOYEE_NUMBER
                                 || ' EMP SSN: '
                                 || R_CHECKS.EMPLOYEE_SSN
                                 || ' SSN: '
                                 || R_CHECKS.SSN_NUMBER);
        END LOOP;

        DBMS_OUTPUT.PUT_LINE('Payprofit Records for: '
                             || R_EMPLOYEES.PY_FNAME);
                             
        FOR R_PAYPROFS IN (
            SELECT
                    PAYPROF_BADGE,
                    PAYPROF_SSN,
                    PY_PS_YEARS,    
                    PY_PROF_POINTS,
                    PY_PROF_CONT,
                    PY_PROF_EARN
            FROM
                PAYPROFIT
            WHERE
                PAYPROF_SSN = (R_EMPLOYEES.DEM_SSN)
        ) LOOP
            LOOP_FLAG := TRUE;
            DBMS_OUTPUT.PUT_LINE(R_PAYPROFS.PAYPROF_BADGE
                                 || ' '
                                 || R_PAYPROFS.PAYPROF_SSN
                                 || ' '
                                 || R_PAYPROFS.PY_PS_YEARS
                                 || ' '
                                 || R_PAYPROFS.PY_PROF_POINTS
                                 || ' '
                                 || R_PAYPROFS.PY_PROF_CONT
                                 || ' '
                                 || R_PAYPROFS.PY_PROF_EARN);
        END LOOP;
        IF NOT LOOP_FLAG THEN
            DBMS_OUTPUT.PUT_LINE ('No payprofit records found...'
                                  || CHR(10));
        ELSE
 --need to reset flag
            LOOP_FLAG := FALSE;
            DBMS_OUTPUT.PUT_LINE(' '
                                 || CHR(10));
        END IF;

        DBMS_OUTPUT.PUT_LINE('+++++++++++++++++++');
        
    END LOOP;
    DBMS_OUTPUT.PUT_LINE(' ---- END ---');
EXCEPTION
    WHEN OTHERS THEN
        L_SQL_ERROR := SQLCODE;
        L_SQL_ERRM := SQLERRM;
        DBMS_OUTPUT.PUT_LINE('There was an exeception in the view test data script.');
        DBMS_OUTPUT.PUT_LINE('Error number: '
                             || L_SQL_ERROR
                             || ' Message: '
                             || L_SQL_ERRM);
END;