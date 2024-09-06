-- This script creates a temporary table that is used 
-- when employees are also beneficiaries, and we need 
-- to preserve some obfuscated fields to repeat them 
-- for that employee when in the beneficiary table

CREATE TABLE EMPLOYEES_OBFUSCATED (
    DEM_SSN	NUMBER(9,0),
    PY_NAM	VARCHAR2(40 BYTE),
    PY_ADD	VARCHAR2(30 BYTE),
    PY_ADD2	VARCHAR2(30 BYTE),
    PY_CITY	VARCHAR2(25 BYTE),
    PY_STATE CHAR(2 BYTE),
    PY_ZIP	NUMBER(5,0),
    PY_DOB	NUMBER(8,0)
);

CREATE INDEX EMPLOYEES_OBFUSCATED_I ON EMPLOYEES_OBFUSCATED ( DEM_SSN );

COMMIT;
