/* 
    This script creatse the following records for a test run of
    the obfuscation process:

    6 employees
    3 beneficiaries (1 is beneficiary of another beneficiary)
    2 relatives
    6 profit sharing distribution requests 
    3 profit sharing checks 
    2 profit distributions
    3 profit sharing transactions 
    1 pay profit record for 1 employee
    2 profit ss detail legacy record
    9 SSN records in SOC_SEC_REC
*/

/* First, in table Demographics */


INSERT ALL INTO DEMOGRAPHICS (
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
    PY_FULL_DT,
    PY_REHIRE_DT,
    PY_GENDER,
    PY_ASSIGN_ID,
    PY_ASSIGN_DESC,
    PY_STOR,
    PY_SHOUR,
    PY_SET_PWD,
    PY_SET_PWD_DT,
    PY_CLASS_DT,
    PY_GUID
) VALUES (
    -34567,
    -100000000,
    'LYONS, EDWARD J',
    'LYONS',
    'EDWARD',
    'J',
    '1 ELM STREET',
    '',
    'LOWELL',
    'MA',
    '01850',
    '19780101',
    '20000101',
    '20050101',
    0,
    'M',
    8765309,
    'PASSWORD',
    100,
    0,
    'N',
    NULL,
    '20050101',
    1
) INTO DEMOGRAPHICS (
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
    PY_FULL_DT,
    PY_REHIRE_DT,
    PY_GENDER,
    PY_ASSIGN_ID,
    PY_ASSIGN_DESC,
    PY_STOR,
    PY_SHOUR,
    PY_SET_PWD,
    PY_SET_PWD_DT,
    PY_CLASS_DT,
    PY_GUID
) VALUES (
    -34568,
    -100000001,
    'RODRIGUEZ, ANGEL R',
    'RODRIGUEZ',
    'ANGEL',
    'R',
    '15 GRAND STREET',
    '',
    'LAWRENCE',
    'MA',
    '01840',
    '19820101',
    '20000103',
    0,
    0,
    'M',
    8765309,
    'PASSWORD',
    101,
    0,
    'N',
    NULL,
    '20000103',
    3
) INTO DEMOGRAPHICS (
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
    PY_FULL_DT,
    PY_REHIRE_DT,
    PY_GENDER,
    PY_ASSIGN_ID,
    PY_ASSIGN_DESC,
    PY_STOR,
    PY_SHOUR,
    PY_SET_PWD,
    PY_SET_PWD_DT,
    PY_CLASS_DT,
    PY_GUID
) VALUES (
    -34569,
    -100000002,
    'WHITTAKER, MARY J',
    'WHITTAKER',
    'MARY',
    'J',
    '12111 SUNSET BOULEVARD',
    '',
    'HOLLYWOOD',
    'CA',
    '33044',
    '19820101',
    '20000103',
    '20020101',
    '20060103',
    'O',
    8765309,
    'PASSWORD',
    102,
    0,
    'N',
    NULL,
    '20060103',
    3
) INTO DEMOGRAPHICS (
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
    PY_FULL_DT,
    PY_REHIRE_DT,
    PY_GENDER,
    PY_ASSIGN_ID,
    PY_ASSIGN_DESC,
    PY_STOR,
    PY_SHOUR,
    PY_SET_PWD,
    PY_SET_PWD_DT,
    PY_CLASS_DT,
    PY_GUID
) VALUES (
    -34570,
    -100000003,
    'SWAN, CHARLES O',
    'SWAN',
    'CHARLES',
    'O',
    '10 1/2 FRONT STREET',
    '',
    'MARBLEHEAD',
    'MA',
    '01945',
    '19720229',
    '20100305',
    0,
    0,
    'F',
    8765309,
    'PASSWORD',
    103,
    0,
    'N',
    NULL,
    '20100305',
    3
) INTO DEMOGRAPHICS (
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
    PY_FULL_DT,
    PY_REHIRE_DT,
    PY_GENDER,
    PY_ASSIGN_ID,
    PY_ASSIGN_DESC,
    PY_STOR,
    PY_SHOUR,
    PY_SET_PWD,
    PY_SET_PWD_DT,
    PY_CLASS_DT,
    PY_GUID
) VALUES (
    -34571,
    -100000004,
    'SMART, MARCUS M',
    'SMART',
    'MARCUS',
    'M',
    '21 MAIN STREET',
    '',
    'LYNN',
    'MA',
    '01901',
    '19920806',
    '20020201',
    '20040806',
    '20060201',
    'M',
    8765309,
    'PASSWORD',
    104,
    0,
    'N',
    NULL,
    '20060201',
    7
) INTO DEMOGRAPHICS (
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
    PY_FULL_DT,
    PY_REHIRE_DT,
    PY_GENDER,
    PY_ASSIGN_ID,
    PY_ASSIGN_DESC,
    PY_STOR,
    PY_SHOUR,
    PY_SET_PWD,
    PY_SET_PWD_DT,
    PY_CLASS_DT,
    PY_GUID
) VALUES (
    -34572,
    -100000005,
    'WINSTON, HENRIETTA A',
    'WINSTON',
    'HENRIETTA',
    'A',
    '20000 MARKET STREET',
    '',
    'NORWOOD',
    'MA',
    '02062',
    '19781001',
    '20110603',
    '20131001',
    '20180603',
    'F',
    8765309,
    'PASSWORD',
    105,
    0,
    'N',
    NULL,
    '20180603',
    6
)
    SELECT
        1
    FROM
        DUAL;

/* Now in PAYBEN, we need to first recreate that person. Only PSN and SSN are required here */

INSERT ALL INTO PAYBEN (
    PYBEN_PSN,
    PYBEN_PAYSSN,
    PYBEN_NAME,
    PYBEN_ADD,
    PYBEN_CITY,
    PYBEN_STATE,
    PYBEN_ZIP,
    PYBEN_DOBIRTH
) VALUES (
    -345671000,
    -100000010,
    'LYONS, MAUREEN A',
    '1 ELM STREET',
    'LOWELL',
    'MA',
    '10001',
    '19800101'
) INTO PAYBEN (
    PYBEN_PSN,
    PYBEN_PAYSSN,
    PYBEN_NAME,
    PYBEN_ADD,
    PYBEN_CITY,
    PYBEN_STATE,
    PYBEN_ZIP,
    PYBEN_DOBIRTH
) VALUES (
    -345681000,
    -100000011,
    'RODRIGUEZ, CARLA A',
    '15 GRAND STREET',
    'LAWRENCE',
    'MA',
    '018400',
    '19840201'
)
    SELECT
        1
    FROM
        DUAL;

/* This is a beneficiary of a beneficiary */
INSERT ALL INTO PAYBEN (
    PYBEN_PSN,
    PYBEN_PAYSSN,
    PYBEN_NAME,
    PYBEN_ADD,
    PYBEN_CITY,
    PYBEN_STATE,
    PYBEN_ZIP,
    PYBEN_DOBIRTH
) VALUES (
    -345671100,
    -100000016,
    'LYONS, MARCIA C',
    '1 ELM STREET',
    'LOWELL',
    'MA',
    '10001',
    '20100101'
)
    SELECT
        1
    FROM
        DUAL;

/* Now in PAYREL, we will add two spouses. Only PSN and SSN are required here, but they need to be in PAYBEN or else they will be treated as orphans */

INSERT ALL INTO PAYREL (
    PYREL_PSN,
    PYREL_PAYSSN,
    PYREL_PERCENT,
    PYREL_RELATION
) VALUES (
    -34567,
    -100000010,
    100,
    'WIFE'
) INTO PAYREL (
    PYREL_PSN,
    PYREL_PAYSSN,
    PYREL_PERCENT,
    PYREL_RELATION
) VALUES (
    -34568,
    -100000011,
    100,
    'WIFE'
)
    SELECT
        1
    FROM
        DUAL;


/* 
    We now need some profit sharing distribution requests 
*/

INSERT ALL INTO PROFIT_DIST_REQ (
    PROFIT_DIST_REQ_SEQ_NUM,
    PROFIT_DIST_REQ_TYPE,
    PROFIT_DIST_REQ_PSN,
    PROFIT_DIST_REQ_EMP,
    PROFIT_DIST_REQ_REASON,
    PROFIT_DIST_REQ_REASON_OTHER
) VALUES (
    1,
    'MONTHLY',
   null,
    -34567,
    NULL,
    NULL
) INTO PROFIT_DIST_REQ (
    PROFIT_DIST_REQ_SEQ_NUM,
    PROFIT_DIST_REQ_TYPE,
    PROFIT_DIST_REQ_PSN,
    PROFIT_DIST_REQ_EMP,
    PROFIT_DIST_REQ_REASON,
    PROFIT_DIST_REQ_REASON_OTHER
) VALUES (
    2,
    'PAYOUT',
    null,
    -34568,
    NULL,
    NULL
) INTO PROFIT_DIST_REQ (
    PROFIT_DIST_REQ_SEQ_NUM,
    PROFIT_DIST_REQ_TYPE,
    PROFIT_DIST_REQ_PSN,
    PROFIT_DIST_REQ_EMP,
    PROFIT_DIST_REQ_REASON,
    PROFIT_DIST_REQ_REASON_OTHER
) VALUES (
    3,
    'HARDSHIP',
    null,
    -34569,
    'Home Repair',
    NULL
) INTO PROFIT_DIST_REQ (
    PROFIT_DIST_REQ_SEQ_NUM,
    PROFIT_DIST_REQ_TYPE,
    PROFIT_DIST_REQ_PSN,
    PROFIT_DIST_REQ_EMP,
    PROFIT_DIST_REQ_REASON,
    PROFIT_DIST_REQ_REASON_OTHER
) VALUES (
    4,
    'HARDSHIP',
    null,
    -34570,
    'Funeral Exp',
    'My Aunt Ethel died'
)
    SELECT
        1
    FROM
        DUAL;

/* 
    We need two more profit distribution requests: one for a beneficiary and a beneficiary of THAT beneficiary
*/

INSERT ALL INTO PROFIT_DIST_REQ (
    PROFIT_DIST_REQ_SEQ_NUM,
    PROFIT_DIST_REQ_TYPE,
    PROFIT_DIST_REQ_PSN,
    PROFIT_DIST_REQ_EMP,
    PROFIT_DIST_REQ_REASON,
    PROFIT_DIST_REQ_REASON_OTHER
) VALUES (
    5,
    'MONTHLY',
    -345671000,
    -34567,
    NULL,
    NULL
) INTO PROFIT_DIST_REQ (
    PROFIT_DIST_REQ_SEQ_NUM,
    PROFIT_DIST_REQ_TYPE,
    PROFIT_DIST_REQ_PSN,
    PROFIT_DIST_REQ_EMP,
    PROFIT_DIST_REQ_REASON,
    PROFIT_DIST_REQ_REASON_OTHER
) VALUES (
    6,
    'PAYOUT',
    -345671100,
    -34567,
    NULL,
    NULL
)
    SELECT
        1
    FROM
        DUAL;

/* Now let us create two checks */

INSERT ALL INTO PROFIT_SHARE_CHECKS (
    PAYABLE_NAME,
    SSN_NUMBER,
    CHECK_NUMBER,
    CHECK_DATE,
    CHECK_AMOUNT,
    EMPLOYEE_SSN
) VALUES (
    'Maureen Lyons',
    -100000010,
    101,
    DATE '2023-10-01',
    345.00,
    -100000000
) INTO PROFIT_SHARE_CHECKS (
    PAYABLE_NAME,
    SSN_NUMBER,
    CHECK_NUMBER,
    CHECK_DATE,
    CHECK_AMOUNT,
    EMPLOYEE_SSN
) VALUES (
    'Marcia Lyons',
    -100000016,
    102,
    DATE '2023-10-01',
    345.00,
    -100000000
) INTO PROFIT_SHARE_CHECKS (
    PAYABLE_NAME,
    SSN_NUMBER,
    CHECK_NUMBER,
    CHECK_DATE,
    CHECK_AMOUNT,
    EMPLOYEE_SSN
) VALUES (
    'Edward Lyons',
    -100000000,
    102,
    DATE '2023-09-01',
    341.00,
    -100000000
) INTO PROFIT_SHARE_CHECKS (
 PAYABLE_NAME,
    SSN_NUMBER,
    CHECK_NUMBER,
    CHECK_DATE,
    CHECK_AMOUNT,
    EMPLOYEE_SSN
) VALUES (
 'CARLA A RODRIGUEZ',
    -100000011,
    103,
    DATE '2023-11-11',
    125.00,
   -100000001
)
    SELECT
        1
    FROM
        DUAL;


/* A pair of profit distributions  */

INSERT ALL INTO PROFDIST(
    PROFDIST_SSN,
    PROFDIST_PAYSEQ,
    PROFDIST_EMPNAME,
    PROFDIST_PAYSSN,
    PROFDIST_PAYNAME,
    PROFDIST_PAYADDR1,
    PROFDIST_PAYCITY,
    PROFDIST_PAYSTATE,
    PROFDIST_PAYZIP1,
    PROFDIST_3RDPAYTO,
    PROFDIST_3RDNAME,
    PROFDIST_3RDADDR1,
    PROFDIST_3RDADDR2,
    PROFDIST_3RDCITY,
    PROFDIST_3RDSTATE,
    PROFDIST_3RDZIP1
) VALUES (
    -100000000,
    1,
    'Edward Lyons',
    null,
    'Edward Lyons',
    '8 Main Street',
    'Bellhaven',
    'MA',
    10001,
    'Someone',
    'Mysterious Partner',
    '9 Shady Lane',
    'Suite 1',
    'Lynn',
    'MA',
    61233
) INTO PROFDIST(
    PROFDIST_SSN,
    PROFDIST_PAYSEQ,
    PROFDIST_EMPNAME,
    PROFDIST_PAYSSN,
    PROFDIST_PAYNAME,
    PROFDIST_PAYADDR1,
    PROFDIST_PAYCITY,
    PROFDIST_PAYSTATE,
    PROFDIST_PAYZIP1,
    PROFDIST_3RDPAYTO,
    PROFDIST_3RDNAME,
    PROFDIST_3RDADDR1,
    PROFDIST_3RDADDR2,
    PROFDIST_3RDCITY,
    PROFDIST_3RDSTATE,
    PROFDIST_3RDZIP1
) VALUES (
    -100000001,
    2,
    'Angel Rodriguez',
    null,
    'Angel Rodriguez',
    '11 Short Street',
    'Stowe',
    'MA',
    10001,
    ' ',
    ' ',
    ' ',
    ' ',
    ' ',
    ' ',
    0
)
    SELECT
        1
    FROM
        DUAL;
/* Create annual profit sharing transactions  */


INSERT ALL INTO PROFIT_DETAIL (
    PROFIT_DET_RECNO,
    PROFIT_YEAR,
    PROFIT_CLIENT,
    PROFIT_CODE,
    PR_DET_S_SEC_NUMBER,
    PROFIT_CMNT,
    PROFIT_EARN,
    PROFIT_FORT
) VALUES (
    1,
    2023,
    1,
    0,
    -100000000,
    'HRDSHIP 03212',
    100.00,
    75.00
) INTO PROFIT_DETAIL (
    PROFIT_DET_RECNO,
    PROFIT_YEAR,
    PROFIT_CLIENT,
    PROFIT_CODE,
    PR_DET_S_SEC_NUMBER,
    PROFIT_CMNT,
    PROFIT_EARN,
    PROFIT_FORT
) VALUES (
    2,
    2023,
    1,
    0,
    -100000001,
    'MILITARY',
    342.10,
    222.00
) INTO PROFIT_DETAIL (
    PROFIT_DET_RECNO,
    PROFIT_YEAR,
    PROFIT_CLIENT,
    PROFIT_CODE,
    PR_DET_S_SEC_NUMBER,
    PROFIT_CMNT,
    PROFIT_EARN,
    PROFIT_FORT
) VALUES (
    3,
    2023,
    1,
    0,
    -100000002,
    'PAYOFF',
    888.01,
    11.00
)
    SELECT
        1
    FROM
        DUAL;
        
INSERT ALL INTO PROFIT_SS_DETAIL (
    PROFIT_SS_DET_RECNO,
    PROFIT_SS_DET_PR_SS_D_S_SEQNUM,
    PROFIT_SS_YEAR,
    PROFIT_SS_CLIENT,
    PR_SS_D_S_SEC_NUMBER,
    PROFIT_SS_CMNT,
    PROFIT_SS_EARN,
    PROFIT_SS_FORT,
    PROFIT_SS_SSNO,
    PROFIT_SS_NAME
) VALUES (
    1,
    3,
    2023,
    1,
    -100000000,
    'HRDSHIP 03212',
    100.00,
    75.00,
    818991111,
    'John Carter'
) 
    SELECT
        1
    FROM
        DUAL;

    
 INSERT ALL INTO PAYPROFIT (
    PAYPROF_BADGE,
    PAYPROF_SSN,
    PY_PH, 
    PY_PD,
    PY_WEEKS_WORK,
    PY_PROF_CERT,
    PY_PS_ENROLLED,
    PY_PS_YEARS,
    PY_PROF_BENEFICIARY,
    PY_PROF_INITIAL_CONT,
    PY_PS_AMT,
    PY_PS_VAMT,
    PY_PH_LASTYR,
    PY_PD_LASTYR,
    PY_PROF_NEWEMP,
    PY_PROF_POINTS,
    PY_PROF_CONT,
    PY_PROF_FORF,
    PY_VESTED_FLAG,
    PY_PROF_MAXCONT,
    PY_PROF_ZEROCONT,
    PY_WEEKS_WORK_LAST,
    PY_PROF_EARN,
    PY_PS_ETVA,
    PY_PRIOR_ETVA,
    PY_PROF_ETVA,
    PY_PROF_EARN2,
    PY_PROF_ETVA2
) VALUES (
    -34567,
    -100000000,
    0,
    0,
    1,
    1,
    2,
    5,
    0,
    2015,
    38786.22,
    31028.9,
    2044.75,
    56777.46,
    0,
    568,
    0,
    0,
    'N',
    0,
    0,
    40,
    0,
    0,
    0,
    0,
    0,
    0
)
    SELECT
        1
    FROM
        DUAL;

INSERT ALL INTO SOC_SEC_REC (
    SOC_SEC_NUMBER,
    SOC_SEC_RECNO
) VALUES (
    -100000000,
    1
) INTO SOC_SEC_REC (
    SOC_SEC_NUMBER,
    SOC_SEC_RECNO
) VALUES (
    -100000001,
    2
)
INTO SOC_SEC_REC (
    SOC_SEC_NUMBER,
    SOC_SEC_RECNO
) VALUES (
    -100000002,
    3
) INTO SOC_SEC_REC (
    SOC_SEC_NUMBER,
    SOC_SEC_RECNO
) VALUES (
    -100000003,
    4
) INTO SOC_SEC_REC (
    SOC_SEC_NUMBER,
    SOC_SEC_RECNO
) VALUES (
    -100000004,
    5
) INTO SOC_SEC_REC (
    SOC_SEC_NUMBER,
    SOC_SEC_RECNO
) VALUES (
    -100000005,
    6
) INTO SOC_SEC_REC (
    SOC_SEC_NUMBER,
    SOC_SEC_RECNO
) VALUES (
    -100000010,
    7
) INTO SOC_SEC_REC (
    SOC_SEC_NUMBER,
    SOC_SEC_RECNO
) VALUES (
    -100000011,
    8
) INTO SOC_SEC_REC (
    SOC_SEC_NUMBER,
    SOC_SEC_RECNO
) VALUES (
    -100000016,
    9
) 
   
    SELECT
        1
    FROM
        DUAL;

COMMIT;
