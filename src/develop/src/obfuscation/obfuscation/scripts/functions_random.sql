/*

This is a big ibrary containing many things that help 
obfuscate data:

- A list of schema-wide SQL types that correspond to 
records in tables. This allows us a good interface 
between these functions and the running code. Not 
all fields are part of the obfuscation process, but 
this model means you can just implement more randomized 
fields at any time for any table.

- Functions that are used to come up with random data 
such as first and last names. This is used both by the 
test framework and also by the obfuscation process.

- Functions that are only used by the test framework to 
generate huge amounts of new data, such as the birth date, 
hire date, promotion date, and termination date for a 
synthetic employee.

- Utility functions, such as GET_SPACES_FOR_COLUMN, which 
is used to return a blank string that fills a column with 
the maximum number of spaces for that specific column. 

*/

-- This is for DEMOGRAPHICS table
create or replace
type employee_dates as object
    ( 
        birth_date NUMBER,
        hire_date NUMBER,
        rehire_date NUMBER,
        full_time_date NUMBER,
        termination_date NUMBER,
        class_date NUMBER
    );
/
-- This is for PROFIT_DIST_REQ rows
create or replace
type distribution_request_record as object
    ( 
    REQ_STATUS	VARCHAR2(1),
    REQ_TYPE	VARCHAR2(25),
    REQ_REASON	VARCHAR2(250),
    REQ_REASON_OTHER	VARCHAR2(500),
    REQ_AMT_REQ	NUMBER(10,2),
    REQ_AMT_AUTH	NUMBER(10,2),
    REQ_DATE_ENT	DATE,
    REQ_DATE_AUTH	DATE,
    REQ_TAXCODE	VARCHAR2(1),
    REQ_FREQ	VARCHAR2(1)
    );
/
-- This is for PROFIT_DETAIL rows
create or replace
type profit_detail_record as object
    ( 
        PROFIT_YEAR NUMBER(5,1),
        DETAIL_CLIENT NUMBER(3,0),
        CODE CHAR(1),
        CONT NUMBER(9,2),
        EARN NUMBER(9,2),
        FORT NUMBER(9,2),
        MDTE NUMBER(2,0),
        YDTE NUMBER(2,0),
        CMNT VARCHAR2(16),
        ZEROCONT CHAR(1),
        FED_TAXES NUMBER(9,2),
        STATE_TAXES NUMBER(9,2),
        TAX_CODE CHAR(1)
    );
/
create or replace
type profit_ss_detail_record as object
    ( 
        PROFIT_SS_YEAR NUMBER(5,1),
        PROFIT_SS_CLIENT NUMBER(3),
        PROFIT_SS_CODE CHAR(1),
        PROFIT_SS_CONT NUMBER(9,2),
        PROFIT_SS_EARN NUMBER(9,2),
        PROFIT_SS_FORT NUMBER(9,2),
        PROFIT_SS_MDTE NUMBER(2),
        PROFIT_SS_YDTE NUMBER(2),
        PROFIT_SS_CMNT CHAR(16),
        PROFIT_SS_ZEROCONT CHAR(1),
        PROFIT_SS_FED_TAXES NUMBER(9,2),
        PROFIT_SS_STATE_TAXES NUMBER(9,2),
        PROFIT_SS_TAX_CODE CHAR(1),
        PROFIT_SS_SSNO NUMBER(9),
        PROFIT_SS_NAME CHAR(25),
        PROFIT_SS_ADDRESS CHAR(20),
        PROFIT_SS_CITY CHAR(13),
        PROFIT_SS_STATE CHAR(2),
        PROFIT_SS_ZIP NUMBER(5),
        PROFIT_SS_DET_RECNO NUMBER(9),
        PROFIT_SS_DET_PR_SS_D_S_SEQNUM NUMBER(9),
        PR_SS_D_S_SEC_NUMBER NUMBER(9)
    );
/
-- This is for PROFDIST rows
create or replace
type profit_distribution_record as object
    ( 
        PAYFREQ CHAR(1),
        PAYFLAG	CHAR(1),
        CHECKAMT NUMBER(10,2),
        TAXCODE CHAR(1),
        DECEASED CHAR(1),
        SEX	CHAR(1 BYTE),
        FBOPAYTO CHAR(25)
    );
/
-- This is for PAYPROFIT rows
create or replace
type payprofit_record as object
    (
        PY_PH	NUMBER(6,2),
        PY_PD	NUMBER(8,2),
        PY_WEEKS_WORK NUMBER(2,0),
        PY_PROF_CERT	CHAR(1),
        PY_PS_ENROLLED	NUMBER(2,0),
        PY_PS_YEARS NUMBER(2,0)
    );
/
-- This is for PAYREL rows
create or replace
type payrel_record as object
    (
        PYREL_PSN	NUMBER(9,0),
        PYREL_PAYSSN	NUMBER(9,0),
        PYREL_TYPE	CHAR(1),
        PYREL_PERCENT	NUMBER(3,0),
        PYREL_RELATION	CHAR(10),
        PYREL_PSAMT	NUMBER(9,2),
        PYREL_STATUS	VARCHAR2(10),
        PYREL_REASON	VARCHAR2(20)
    );
/
-- This is for EMPLOYEES_OBFUSCATED rows
create or replace
type employee_obf_record as object
    (
        DEM_SSN	NUMBER(9,0),
        PY_STATE CHAR(2 BYTE),
        PY_ZIP	NUMBER(5,0),
        PY_DOB	NUMBER(8,0),
        PY_CITY	VARCHAR2(25 BYTE),
        PY_ADD2	VARCHAR2(30 BYTE),
        PY_ADD	VARCHAR2(30 BYTE),
        PY_NAM	VARCHAR2(40 BYTE)
    );
/
CREATE OR REPLACE FUNCTION IS_PROBABLY_PSN(CANDIDATE_PSN IN NUMBER) RETURN NUMBER IS

BEGIN
    -- This function, if it finds the last two digits are zero,
    -- will return a 1 to say it is probably a PSN, a zero if not

    IF (MOD(CANDIDATE_PSN, 100) = 0) THEN
        RETURN 1;
    END IF;

    RETURN 0;

END IS_PROBABLY_PSN;
/
CREATE OR REPLACE FUNCTION ASSEMBLE_COMMENT_TRANSFER(INCOMING_PREFIX IN VARCHAR, INCOMING_PSN IN NUMBER) RETURN VARCHAR IS

    L_FIRST_LENGTH NUMBER;
    L_SECOND_LENGTH NUMBER;
    L_FIRST_STRING VARCHAR(8);
    L_NUMBER_STRING VARCHAR(14);
    L_DIRECTION_CHAR CHAR(1);
    L_FOUND_CHAR NUMBER;

BEGIN   

    -- So... we have a lot of situations to deal with
    -- We need to know the direction char. It's either
    -- one or the other
    SELECT INSTR(INCOMING_PREFIX,'<', 1, 1)
        "Instring" INTO L_FOUND_CHAR FROM DUAL;

    IF (L_FOUND_CHAR != 0) THEN
        L_DIRECTION_CHAR := '<';
    ELSE
        L_DIRECTION_CHAR := '>';
    END IF;

    -- We really just need the XFER or QDRO to start
    L_FIRST_STRING := SUBSTR(INCOMING_PREFIX, 1, 4) || L_DIRECTION_CHAR;
    L_NUMBER_STRING := TO_CHAR(INCOMING_PSN);

    --DBMS_OUTPUT.put_line('First was: ' || L_FIRST_STRING);
    --DBMS_OUTPUT.put_line('Second was: ' || L_NUMBER_STRING);

    L_FIRST_LENGTH := LENGTH(L_FIRST_STRING);
    L_SECOND_LENGTH := LENGTH(L_NUMBER_STRING);

    --DBMS_OUTPUT.put_line('First len was: ' || L_FIRST_LENGTH);
    --DBMS_OUTPUT.put_line('Second len was: ' || L_SECOND_LENGTH);

    -- If greater than 16, return an error
    IF (L_FIRST_LENGTH + L_SECOND_LENGTH > 16) THEN
        -- So we already have the end result. Just return the combo
        RETURN L_FIRST_STRING || TO_CHAR(-100000);
    END IF;

    -- Do we already have 16?
    IF (L_FIRST_LENGTH + L_SECOND_LENGTH = 16) THEN
        -- So we already have the end result. Just return the combo
        RETURN L_FIRST_STRING || L_NUMBER_STRING;
    END IF;

    -- So, if we are here, our number is too short. We need to pad with zeros
 
    FOR I IN 1..6 LOOP
        L_NUMBER_STRING := '0' || L_NUMBER_STRING;
        EXIT WHEN LENGTH(L_NUMBER_STRING) = 11;
    END LOOP;

    --DBMS_OUTPUT.put_line('Number string now: ' || L_NUMBER_STRING);
    
    RETURN L_FIRST_STRING || L_NUMBER_STRING;

END ASSEMBLE_COMMENT_TRANSFER;
/
CREATE OR REPLACE PROCEDURE EXTRACT_COMMENT_PARTS
   ( 
     ORIGINAL IN CHAR,
     EXTRACT_PREFIX OUT VARCHAR, 
     EXTRACT_PSN OUT NUMBER
   )
is
    L_TRIM_CMNT VARCHAR(25);
    L_FIRST_PART VARCHAR(16);
    L_SECOND_PART VARCHAR(16);
    L_PART_LENGTH NUMBER;
begin



    -- So, what we want to do is separate the PSN from everything else
    -- It is probably 11 characters
    -- But we do not want whitespace at the end
    L_TRIM_CMNT := TRIM(ORIGINAL);
    IF  (LENGTH(L_TRIM_CMNT) > 16) THEN
        EXTRACT_PREFIX := '';
        EXTRACT_PSN := 0;
        RETURN;
    END IF;
    L_SECOND_PART := REGEXP_SUBSTR(L_TRIM_CMNT, '-?[0-9]+');

    -- If it is NULL, we did not find any numbers
    IF (L_SECOND_PART = NULL) THEN
        EXTRACT_PREFIX := '';
        EXTRACT_PSN := 0;
        RETURN;
    END IF;

    L_PART_LENGTH := LENGTH(L_SECOND_PART);
    -- It needs to be 5+ digits long
    IF (L_PART_LENGTH < 5) THEN
        EXTRACT_PREFIX := '';
        EXTRACT_PSN := 0;
        RETURN;
    END IF;

    -- Now we need to get the first part, which is everything
    -- before the first digit
    EXTRACT_PREFIX := REGEXP_SUBSTR(L_TRIM_CMNT, '(XFER|QDRO)[^0-9-]*');
    IF (EXTRACT_PREFIX IS NULL) THEN
        EXTRACT_PREFIX := '';
        EXTRACT_PSN := 0;
        RETURN;
    END IF;
    EXTRACT_PSN := TO_NUMBER(L_SECOND_PART);
    RETURN;
end;
/
CREATE OR REPLACE FUNCTION GET_RANDOM_DATE_INSIDE_YEAR(INCOMING_DATE IN NUMBER) RETURN NUMBER IS
    NEW_DATE NUMBER(8);
    NEW_YEAR NUMBER(4);
    NEW_MONTH NUMBER(2);
    NEW_DAY NUMBER(2);

BEGIN   

    IF (INCOMING_DATE IS NULL) OR (INCOMING_DATE = 0) OR (INCOMING_DATE < 1850) THEN
        RETURN 0;
    END IF;

    -- First we need to get the first four digits for the year
    NEW_YEAR := TO_NUMBER(SUBSTR(TO_CHAR(INCOMING_DATE), 1, 4));
    NEW_MONTH := FLOOR(DBMS_RANDOM.VALUE(1, 13));
    NEW_DAY := FLOOR(DBMS_RANDOM.VALUE(1, 29));

    NEW_DATE := CREATE_YYYYMMDD(NEW_YEAR, NEW_MONTH, NEW_DAY);
        
    RETURN NEW_DATE;

END GET_RANDOM_DATE_INSIDE_YEAR;
/
CREATE OR REPLACE FUNCTION GET_A_FIRST_NAME RETURN  VARCHAR IS

-- If you want to add more entries, you have to update the amount of slots in the array
TYPE ARRAY_T IS VARRAY(251) OF VARCHAR2(25);
FIRST_NAMES_ARRAY  ARRAY_T := ARRAY_T(
    'AVERY', 'BLAKE', 'CAMERON', 'MORGAN', 'HARPER', 
    'ANGEL', 'SAWYER', 'DREW', 'CRUZ', 'VARGAS', 
    'DARCY', 'SEAN', 'BRIANNA', 'ANTHONY', 'EMMANUEL',
    'KAREN', 'CHRISTOPHER', 'KAYLA', 'MAYA', 'RICHARD', 
    'DIEGO', 'RUTH', 'JENNIFER', 'GEORGE', 'MOSES', 
    'ALICIA', 'SALLY', 'BRIANNA', 'KRYSTAL', 'GREGORY',
    'JONATHAN', 'EMMA', 'PAMELA', 'NICOLE', 'RAYMOND',
    'OLIVIA', 'PATRICK', 'KATHERINE', 'BRANDON', 'CHE',
    'SCOTT', 'BRENDA', 'HARPER', 'FIONA', 'CHRISTINE',
    'DENNIS', 'HEATHER', 'MAUREEN', 'ALFONZO', 'CINDY',
    'LUKE','SAM', 'MARCUS', 'HONDO', 'JUNE', 
    'ALICE', 'ETHEL', 'ROSE', 'GARRETT', 'SANDRA',
    'ANNA', 'RICARDO', 'KRYTSAL', 'CHER', 'MARCO',
    'PEETA', 'HERALDO', 'WARREN', 'CLIFF', 'RORY',
    'BRENDAN', 'BREE', 'ERICA', 'JASON', 'CATHY',
    'OLIVIA', 'NOAH','EMMA','LIAM','AMELIA',
    'OLIVER','ELIJAH','CHARLOTTE','MATEO','AVA',
    'LUCAS','ISABELLA',	'LEVI','MIA','LEO',
    'LUNA',	'EZRA','EVELYN','LUCA', 'GIANNA',
    'ASHER','LILY',	'ARIA',	'ETHAN','AURORA',	
    'SEBASTIAN','ELLIE','HENRY','MUHAMMAD','MILA',	
    'HUDSON','SOFIA','MAVERICK', 'CAMILA','BENJAMIN',
    'LAYLA', 'THEO', 'NOVA','KAI', 'JEM',
    'NORA', 'JULIAN', 'ABIGAIL', 'JAYDEN','EMILY', 
    'DAVID','MAYA', 'LOGAN','ISLA', 'THEODORE',
    'DELILAH', 'WYATT','NAOMI', 'CARTER','ELIZABETH', 
    'SAMUEL','GRACE', 'OWEN','ZOEY', 'WILLIAM',
    'EMILIA', 'WAYLON','RILEY', 'EZEKIEL','ZOE', 
    'MILES','PAISLEY', 'MATTHEW','ATHENA', 'ISAIAH',
    'LEILANI', 'JACOB', 'MADISON', 'SANTIAGO', 'SETH',
    'VICTORIA', 'CALEB', 'AYLA', 'JOHN', 'JAY',
    'STELLA', 'JOSEPH','LUCY', 'THOMAS','KINSLEY',
    'IRIS', 'AMIR','GABRIELLA', 'ISAAC','LAINEY', 
    'NOLAN','AALIYAH', 'NATHAN','SERENITY', 'ZION',
    'ADDISON', 'JEREMIAH', 'ADAM','ALICE', 'LINCOLN',
    'BELLA', 'ADRIAN','SADIE', 'ANTHONY','SOPHIE', 
    'COOPER','AMARA', 'ELI','AUTUMN', 'MICAH',
    'SUMMER', 'CAMERON','EMERY', 'ENZO','EVERLY', 
    'RYAN','VALENTINA', 'ROMAN','HANNAH', 'ROWAN',
    'BROOKLYN', 'XAVIER','MADELYN', 'JAXON','NATALIE', 
    'WESLEY','LEAH', 'WESTON','MARIA', 'ANDREW',
    'SAVANNAH', 'JOSHUA','AMIRA', 'SILAS','AUBREY', 
    'COLTON','JADE', 'CHARLIE','JASMINE', 'PARKER',
    'EDEN', 'CHRISTOPHER','SKYLAR', 'GREYSON','JOSIE', 
    'LUKA','CLARA', 'ATLAS','ADELINE', 'DOMINIC',
    'ELLIANA', 'CHRISTIAN','MILLIE', 'MYLES','LILLIAN', 
    'BROOKS','MELODY', 'KAYDEN','SARAH', 'AUSTIN',
    'EVA', 'AARON','RUBY', 'OMAR','FREYA', 
    'AMARI','LYLA', 'AXEL', 'ADALYNN', 'BEAU',
    'LILIANA', 'JASPER','DAISY', 'LANDON','NEVAEH', 
    'MAX'
);
    
-- This is a convenience variable so that only the two lines above need to be changed
L_FIRST_NAMES_LENGTH NUMBER := FIRST_NAMES_ARRAY.COUNT;

BEGIN
   RETURN FIRST_NAMES_ARRAY(ROUND(DBMS_RANDOM.VALUE(1, L_FIRST_NAMES_LENGTH)));

END GET_A_FIRST_NAME;
/
CREATE OR REPLACE FUNCTION GET_A_LAST_NAME
RETURN  VARCHAR IS

-- If you want to add more entries, you have to update the amount of slots in the array
TYPE ARRAY_T IS VARRAY(250) OF VARCHAR2(25);
    LAST_NAMES_ARRAY   ARRAY_T := ARRAY_T(
    'GARCIA', 'SMITH', 'LOPEZ', 'JOHNSON', 'JONES', 
    'TAYLOR', 'MILLER', 'WILLIAMS', 'WILSON', 'HARRIS', 
    'BELL', 'GOMEZ', 'MURPHY', 'WASHINGTON', 'SANCHEZ',
    'RIVERA', 'MARTIN', 'WORTHINGTON', 'CHANG', 'ROBERTS',
    'MACDONALD', 'PARKER', 'DOUGLAS', 'MASTERS', 'BAKER',
    'ORTIZ', 'BUCKLEY', 'RAMOS', 'CONNORS', 'PACHECO',
    'LEE', 'PEREZ', 'THOMPSON', 'WHITE', 'HARRIS',
    'CLARK', 'LEWIS', 'ROBINSON', 'WALKER', 'YOUNG',
    'ALLEN', 'MACDONALD', 'FINN', 'DOUGHTY', 'TEDESCHI',
    'MASTERS', 'WRIGHT', 'DANIELS', 'GEORGE', 'MILLS',
    'KING', 'SCOTT', 'TORRES', 'NGUYEN', 'HILL',
    'FLORES', 'GREEN', 'CRANEY', 'NELSON', 'HALL',
    'MITCHELL', 'CARTER', 'ROBERTS', 'PHILLIPS', 'FEATHERS',
    'EVANS', 'TOMASO', 'GORE', 'CLINTON', 'NIXON',
    'RICHARDS', 'MICHAELS', 'MURPHY', 'JOHANNS', 'RICKY',
    'BARTLETT', 'SCHNEIDER', 'JOHNS', 'RIVERS', 'BIFF',
    'MARQUEZ', 'FIELDS', 'PARK', 'YANG', 'LITTLE',
    'BANKS','PADILLA','DAY','BOWMAN','SCHULTZ',
    'LUNA','FOWLER','MEJIA','DAVIDSON','ACOSTA',
    'BREWER','MAY','HOLLAND','JUAREZ','NEWMAN',
    'PEARSON','CURTIS','CORTEZ','JOSEPH','BARRETT',
    'NAVARRO','FIGUEROA','KELLER','AVILA','WADE',
    'MOLINA','STANLEY','HOPKINS','CAMPOS','BARNETT',
    'BATES','CHAMBERS','CALDWELL','BECK','LAMBERT',
    'BYRD','CRAIG','AYALA','LOWE','FRAZIER',
    'POWERS','NEAL','LEONARD','CARRILLO','SUTTON',
    'FLEMING','RHODES','SHELTON','SCHWARTZ','NORRIS',
    'JENNINGS','WATTS', 'FERGUSON', 'MACIAS','VILLANUEVA',
    'ZAMORA','PRATT','STOKES','OWEN','BALLARD',
    'LANG','BROCK','VILLARREAL','CHARLES','DRAKE',
    'BARRERA','CAIN','PATRICK','PINEDA','BURNETT',
    'MERCADO','SANTANA','SHEPHERD','BAUTISTA','ALI',
    'SHAFFER','LAMB','TREVINO','McKENZIE','HESS',
    'BEIL','OLSEN','COCHRAN','MORTON','NASH',
    'WILKINS','PETERSEN', 'MIYAZAKI', 'SEIKO', 'SONY',
    'BRIGGS','SHAH','ROTH','NICHOLSON', 'HOLLOWAY',
    'LOZANO','RANGEL','FLOWERS','HOOVER','SHORT',
    'ARIAS','MORA','VALENZUELA','BRYAN', 'BRYANT',
    'MEDRANO','HAHN','McMILLAN','SMALL','BENTLEY',
    'FELIX','PECK','LUCERO','BOYLE','HANNA',
    'PACE','RUSH','HURLEY','HARDING','McCONNELL',
    'BERNAL','NAVA','AYERS','EVERETT','VENTURA',
    'AVERY','PUGH','MAYER','BENDER','SHEPARD',
    'McMAHON','LANDRY','CASE','SAMPSON', 'LOURDES',
    'MOSES','MAGANA','BLACKBURN','DUNLAP','GOULD',
    'DUFFY','VAUGHAN','HERRING','McKAY','ESPINOSA',
    'FARLEY','BERNARD','ASHLEY','FRIEDMAN','VILLALOBOS',
    'POTTS','TRUONG','COSTA','CORREA','BLEVINS',
    'CLEMENTS','FRY','DELAROSA','BEST','BENTON',
    'PORTILLO','DOUGHERTY','CRANE','HALEY','PHAN'
    );
L_LAST_NAMES_LENGTH NUMBER := LAST_NAMES_ARRAY.COUNT;

    
BEGIN
   RETURN LAST_NAMES_ARRAY(ROUND(DBMS_RANDOM.VALUE(1, L_LAST_NAMES_LENGTH)));
END GET_A_LAST_NAME;
/
CREATE OR REPLACE FUNCTION GET_A_MIDDLE_NAME
RETURN  VARCHAR IS

-- If you want to add more entries, you have to update the amount of slots in the array
    TYPE ARRAY_T IS
        VARRAY(12) OF VARCHAR2(25);
    MIDDLE_NAMES_ARRAY   ARRAY_T := ARRAY_T('A', 'C', 'L', 'M', 'P', 'T', 'V', 'J', 'F', 'I', 'S', 'U');
    L_MIDDLE_NAMES_LENGTH NUMBER := MIDDLE_NAMES_ARRAY.COUNT;
    
BEGIN
   RETURN MIDDLE_NAMES_ARRAY(ROUND(DBMS_RANDOM.VALUE(1, L_MIDDLE_NAMES_LENGTH)));

END GET_A_MIDDLE_NAME;
/
CREATE OR REPLACE FUNCTION GET_AN_ADDRESS
RETURN  VARCHAR IS

-- If you want to add more entries, you have to update the amount of slots in the array
TYPE ARRAY_T IS VARRAY(15) OF VARCHAR2(25);
    
STREET_NAMES_ARRAY ARRAY_T := ARRAY_T('PINE', 'MAPLE', 'CEDAR', 'EIGHTH', 'ELM', 
    'ARTHUR', 'WASHINGTON', 'NINTH', 'LAKE', 'HILL',
    'SECOND', 'OAK', 'LIBERTY', 'LINCOLN', 'REDWOOD');
STREET_TYPES_ARRAY ARRAY_T := ARRAY_T('STREET', 'ROAD', 'CIRCLE', 'COURT', 'BOULEVARD', 
    'TERRACE', 'AVENUE', 'LANE', 'WAY', 'HIGHWAY',
    'DRIVE', 'PARKWAY', 'PLACE', 'PLAZA', 'SQUARE');

L_STREET_NAMES_LENGTH NUMBER := STREET_NAMES_ARRAY.COUNT;
L_STREET_TYPES_LENGTH NUMBER := STREET_TYPES_ARRAY.COUNT;

    
BEGIN
   RETURN SUBSTR(
        TO_CHAR(
        ROUND(DBMS_RANDOM.VALUE(1, 375)) || 
        ' ' || STREET_NAMES_ARRAY(ROUND(DBMS_RANDOM.VALUE(1, L_STREET_NAMES_LENGTH))) || 
        ' ' || STREET_TYPES_ARRAY(ROUND(DBMS_RANDOM.VALUE(1, L_STREET_TYPES_LENGTH)))), 1, 20 
    );
END GET_AN_ADDRESS;
/
CREATE OR REPLACE FUNCTION GET_A_CITY RETURN  VARCHAR IS

-- If you want to add more entries, you have to update the amount of slots in the array
TYPE ARRAY_T IS VARRAY(25) OF VARCHAR2(25);
TOWN_NAMES_ARRAY   ARRAY_T := ARRAY_T('BOXFORD', 'LYNN', 'LAWRENCE', 'HARVARD', 'STOW', 
    'TEWKSBURY', 'LEE', 'ANDOVER', 'HOLDEN', 'LOWELL', 
    'SAUGUS', 'PEABODY', 'MELROSE', 'WAKEFIELD', 'WOBURN',
    'LYNNFIELD', 'DANVERS', 'MIDDLETON', 'PEPPERELL', 'HAVERHILL',
    'NORTH READING', 'BOXFORD', 'NORTH ANDOVER', 'BILLERICA', 'BURLINGTON');
L_TOWN_NAMES_LENGTH NUMBER := TOWN_NAMES_ARRAY.COUNT;
   
BEGIN
   RETURN TOWN_NAMES_ARRAY(ROUND(DBMS_RANDOM.VALUE(1, L_TOWN_NAMES_LENGTH)));
END GET_A_CITY;
/
CREATE OR REPLACE FUNCTION CREATE_YYYYMMDD(INCYEAR IN NUMBER, INCMONTH IN NUMBER, INCDAY NUMBER) RETURN NUMBER IS

    NEW_DATE NUMBER(8);
    DATE_STRING VARCHAR(8);
    MONTH_STRING CHAR(2);
    DAY_STRING CHAR(2);

BEGIN   

    -- We need to do this as a string, in order to get the zeros in
    -- the right place, then return the number when we are done

    IF (INCMONTH) < 10 THEN
        MONTH_STRING := '0' || TO_CHAR(INCMONTH);
    ELSE
        MONTH_STRING := TO_CHAR(INCMONTH);
    END IF;

    IF (INCDAY) < 10 THEN
        DAY_STRING := '0' || TO_CHAR(INCDAY);
    ELSE
        DAY_STRING := TO_CHAR(INCDAY);
    END IF;

    DATE_STRING := TO_CHAR(INCYEAR) || MONTH_STRING || DAY_STRING;

    NEW_DATE := TO_NUMBER(DATE_STRING);

    RETURN NEW_DATE;
END CREATE_YYYYMMDD;
/

CREATE OR REPLACE FUNCTION GET_A_BIRTH_DATE RETURN NUMBER IS
    L_BIRTH_DATE NUMBER(8);
    L_CURRENT_YEAR NUMBER(4);
    L_BIRTH_YEAR NUMBER(4);

BEGIN   

    L_CURRENT_YEAR := TO_NUMBER(TO_CHAR(sysdate, 'yyyy'));
    L_BIRTH_YEAR := L_CURRENT_YEAR - FLOOR(DBMS_RANDOM.VALUE(20, 75));
    
    -- Now we can leverage another function by passing in oct 10 of the birth year to get back
    -- a random month and dat also
    
    L_BIRTH_DATE := GET_RANDOM_DATE_INSIDE_YEAR((L_BIRTH_YEAR * 10000) + 1010);
        
    RETURN L_BIRTH_DATE;

END GET_A_BIRTH_DATE;
/
CREATE OR REPLACE FUNCTION GET_A_PHONE_NUMBER RETURN NUMBER IS
    NEW_PHONE NUMBER(10);

BEGIN   
    -- This will make all phone numbers in the 6xx-xx-xxxx range.
    NEW_PHONE := FLOOR(DBMS_RANDOM.VALUE(6000000000, 7000000000));
        
    RETURN NEW_PHONE;

END GET_A_PHONE_NUMBER;
/

CREATE OR REPLACE FUNCTION GET_A_GENDER RETURN CHAR IS

    L_RANDOM_RESULT NUMBER;

BEGIN   
    
    -- We need a gender that will return male about 47 percent
    -- of the time, female about 47 percent of the time, and 
    -- other 6 percent
    L_RANDOM_RESULT:= FLOOR(DBMS_RANDOM.VALUE(1, 100));
        
    IF (L_RANDOM_RESULT) < 48 THEN
        RETURN 'M';
    ELSIF (L_RANDOM_RESULT) < 94 THEN
        RETURN 'F';
    ELSE
        RETURN 'O';
    END IF;


END GET_A_GENDER;
/
CREATE OR REPLACE FUNCTION GET_A_STATE RETURN VARCHAR IS

    L_RANDOM_RESULT NUMBER;

BEGIN   
    
    L_RANDOM_RESULT:= FLOOR(DBMS_RANDOM.VALUE(1, 100));
        
    IF (L_RANDOM_RESULT) < 40 THEN
        RETURN 'MA';
    ELSIF (L_RANDOM_RESULT) < 52 THEN
        RETURN 'VT';
    ELSIF (L_RANDOM_RESULT) < 65 THEN
        RETURN 'ME';
    ELSIF (L_RANDOM_RESULT) < 78 THEN
        RETURN 'NH';
    ELSIF (L_RANDOM_RESULT) < 94 THEN
        RETURN 'FL';
    ELSIF (L_RANDOM_RESULT) < 97 THEN
        RETURN 'OH';
    ELSE
        RETURN 'AZ';
    END IF;


END GET_A_STATE;
/
CREATE OR REPLACE FUNCTION GET_ZIP_FOR_STATE(STATE_VALUE CHAR) RETURN NUMBER IS

BEGIN   
    
    CASE STATE_VALUE
        WHEN 'NH' THEN
            RETURN FLOOR(DBMS_RANDOM.VALUE(3031, 3899));
        WHEN 'VT' THEN
            RETURN FLOOR(DBMS_RANDOM.VALUE(5001, 5999));
        WHEN 'ME' THEN
            RETURN FLOOR(DBMS_RANDOM.VALUE(1001, 02999));
    
        -- Everything else, inlcuding MA, is treated as Massachusetts
        ELSE RETURN FLOOR(DBMS_RANDOM.VALUE(1001, 02999));
    END CASE;    

END GET_ZIP_FOR_STATE;
/
CREATE OR REPLACE FUNCTION ADJUST_DATE(DATE_VALUE NUMBER) RETURN NUMBER IS

    -- This function moves a date a little in a random way, but not too much
    -- so that date-related calculations will not be completely thrown off
    L_NEW_DATE NUMBER;
    L_YEAR NUMBER;
    L_NEW_MONTH NUMBER;
    L_NEW_DAY NUMBER;
    L_RANDOM_TINY NUMBER;
    L_RANDOM_MEDIUM NUMBER;

BEGIN   

    IF (DATE_VALUE IS NULL) OR (DATE_VALUE = 0) THEN
        RETURN 0;
    END IF;
    
    L_YEAR := TO_NUMBER(SUBSTR(TO_CHAR(DATE_VALUE), 1, 4));

    IF (L_YEAR = 0) OR (L_YEAR < 1850) THEN
        RETURN 0;
    END IF;

    L_NEW_MONTH := TO_NUMBER(SUBSTR(TO_CHAR(DATE_VALUE), 5, 2));
    IF (L_NEW_MONTH = 0) OR (L_NEW_MONTH > 12) THEN
        RETURN 0;
    END IF;

    L_NEW_DAY := TO_NUMBER(SUBSTR(TO_CHAR(DATE_VALUE), 5, 2));
    IF (L_NEW_DAY = 0) OR (L_NEW_DAY > 31) THEN
        RETURN 0;
    END IF;
    
    L_RANDOM_TINY := ROUND(DBMS_RANDOM.VALUE(1, 2));
    L_RANDOM_MEDIUM := ROUND(DBMS_RANDOM.VALUE(1, 5));

    IF (L_NEW_MONTH > 10) THEN
        L_NEW_MONTH := L_NEW_MONTH - L_RANDOM_TINY;
    ELSE 
        L_NEW_MONTH := L_NEW_MONTH + L_RANDOM_TINY;
    END IF;

    IF (L_NEW_DAY > 24) THEN
        L_NEW_DAY := L_NEW_DAY - L_RANDOM_TINY;
    ELSE 
        L_NEW_DAY := L_NEW_DAY + L_RANDOM_MEDIUM;
    END IF;

    L_NEW_DATE := CREATE_YYYYMMDD(L_YEAR, L_NEW_MONTH, L_NEW_DAY);

    RETURN L_NEW_DATE;

END ADJUST_DATE;
/
CREATE OR REPLACE FUNCTION GET_EMPLOYEE_DATES RETURN EMPLOYEE_DATES IS

    -- The four dates will be eight-digit numbers, as they are
    -- in the database
    L_BIRTH_DATE NUMBER;
    L_HIRE_DATE NUMBER;
    L_REHIRE_DATE NUMBER;
    L_FULL_TIME_DATE NUMBER;
    L_TERMINATED_DATE NUMBER;
    L_CLASS_DATE NUMBER;

    L_CURRENT_YEAR NUMBER; -- This is yyyy
    L_CURRENT_DATE NUMBER; -- This is yyyymmdd
    L_FIRST_COIN_FLIP NUMBER;
    L_SECOND_COIN_FLIP NUMBER;
    L_AGE NUMBER;
    L_ADULT_YEARS NUMBER;
    L_YEAR_BORN NUMBER;
    L_CHANCE_TERMINATED NUMBER;

BEGIN   
    
    L_TERMINATED_DATE := 0;
    L_REHIRE_DATE := 0;

    -- First, we need to figure out: how old is this person?
    -- So we need a number from 18 to 70
    L_AGE := FLOOR(DBMS_RANDOM.VALUE(18, 71));

    -- We need to reduce the current year by one to avoid some dates
    -- being in the future
    L_CURRENT_YEAR := TO_NUMBER(TO_CHAR(sysdate, 'yyyy')) - 1;
    L_CURRENT_DATE := TO_NUMBER(TO_CHAR(sysdate, 'yyyymmdd'));

    L_YEAR_BORN := L_CURRENT_YEAR - L_AGE;

    -- So when were they hired? Let's say between 0 and 30 years
    -- after their age. So let's say we have, in the year 2024,
    -- someone born in 2004, and is age 20. 
    -- The calculation below will get a random number between 0 
    -- and the number of years they have been an adult.
    L_ADULT_YEARS := FLOOR(DBMS_RANDOM.VALUE(0, L_AGE - 18));

    -- We can now set this one
    L_BIRTH_DATE := GET_RANDOM_DATE_INSIDE_YEAR((L_YEAR_BORN * 10000) + 1010);

    L_FIRST_COIN_FLIP := FLOOR(DBMS_RANDOM.VALUE(0, 2));
    L_SECOND_COIN_FLIP := FLOOR(DBMS_RANDOM.VALUE(0, 2));
    L_CHANCE_TERMINATED := FLOOR(DBMS_RANDOM.VALUE(1, 100));
    
    CASE 
        WHEN (L_ADULT_YEARS < 6) THEN 
           
            -- Let's say they got hired 2 years after turning 18
            L_HIRE_DATE := GET_RANDOM_DATE_INSIDE_YEAR(
                ((L_YEAR_BORN + 18 + 2) * 10000) + 1010);

            -- For young people a simple result, with half
            -- of them hired as part timers
            
            IF (L_FIRST_COIN_FLIP = 0) THEN
                L_FULL_TIME_DATE := L_HIRE_DATE;
            ELSE
                L_FULL_TIME_DATE := 0;
            END IF;

            -- No rehire
            L_REHIRE_DATE := 0;
            
        WHEN (L_ADULT_YEARS < 12) THEN
            -- So this person is at least 30.
            -- Let us say they were hired in their mid 20s
            L_HIRE_DATE := GET_RANDOM_DATE_INSIDE_YEAR(
                (((L_YEAR_BORN + 18 + FLOOR(L_ADULT_YEARS / 2)) * 10000) + 1010));


            -- Give them a 50% chance of having started full time
            IF (L_FIRST_COIN_FLIP = 0) THEN
                L_FULL_TIME_DATE := L_HIRE_DATE;
            ELSE
                -- Maybe they became full time a year after that
                L_FULL_TIME_DATE := L_HIRE_DATE + 10000;
            END IF;
            
            -- No rehire
            L_REHIRE_DATE := 0;

        WHEN (L_ADULT_YEARS < 25) THEN
            -- So this person is middle aged.

            -- Let us say they were hired late 20s
            L_HIRE_DATE := GET_RANDOM_DATE_INSIDE_YEAR( 
                ((L_YEAR_BORN + 18 + FLOOR(L_ADULT_YEARS / 4) + L_SECOND_COIN_FLIP) *10000) + 1010);

            -- Give them a 50% chance of having started full time
            IF (L_FIRST_COIN_FLIP = 0) THEN
                L_FULL_TIME_DATE := L_HIRE_DATE;
            ELSE
                -- Maybe they became full time a few years after that
                L_FULL_TIME_DATE := L_HIRE_DATE + ((2 + L_FIRST_COIN_FLIP + L_SECOND_COIN_FLIP) * 10000);
            END IF;
            
            -- Perhaps one in 4 has a rehire date around eight years after their full
            -- time date, otherwise 0
            IF (L_FIRST_COIN_FLIP = 0) AND (L_SECOND_COIN_FLIP = 0) THEN
                L_FULL_TIME_DATE := L_HIRE_DATE + (8 * 10000);
            ELSE
                L_REHIRE_DATE := 0;
            END IF;
            
        ELSE -- Over 25 years as an adult

            L_HIRE_DATE := GET_RANDOM_DATE_INSIDE_YEAR( 
                ((L_YEAR_BORN + 18 + FLOOR(L_ADULT_YEARS / 6) + L_SECOND_COIN_FLIP) * 10000) + 1010);

            -- Give them a 50% chance of having started full time
            IF (L_FIRST_COIN_FLIP = 0) THEN
                L_FULL_TIME_DATE := L_HIRE_DATE;
            ELSE
                -- Maybe they became full time several years after that
                L_FULL_TIME_DATE := L_HIRE_DATE + ((7 + L_FIRST_COIN_FLIP + L_SECOND_COIN_FLIP) * 10000);
            END IF;
            
            -- Perhaps one in 4 has a rehire date around ei11ght years after their full
            -- time date, otherwise null
            IF (L_FIRST_COIN_FLIP = 0) AND (L_SECOND_COIN_FLIP = 0) THEN
                L_REHIRE_DATE := L_HIRE_DATE + (11 * 10000);
            ELSE
                L_REHIRE_DATE := 0;
            END IF;
            
        END CASE;
        
        IF (L_CHANCE_TERMINATED > 95) THEN
            -- This makes someone fired yesterday
            L_TERMINATED_DATE := TO_NUMBER(TO_CHAR(SYSDATE - 1, 'YYYYMMDD') );
        END IF;

    -- So now that we have all the other dates, we need to determine
    -- class date, which is when their position changed. It is either
    -- their hire or rehire date, or sometime after that is before a 
    -- termination date. We will have these possibilities:
    -- #1 It's the same as their hire date
    -- #2 It's the same as their hire date because they were terminated
    -- #3 They were rehired, so it is moved up to the rehire date
    -- #4 They were rehired, so it is two years after that
    -- #5 They were not terminated or rehired, so it was 5 years after hire
    -- #6 They were not terminated or rehired, so it was 10 years after hire

    -- Everyone starts with hire date
    L_CLASS_DATE := L_HIRE_DATE;

    -- If you were rehired, that is your starting point
    IF (L_REHIRE_DATE != 0) THEN
        L_CLASS_DATE := L_REHIRE_DATE;
    END IF;

    -- If you were not terminated, more changes possible
    IF (L_TERMINATED_DATE = 0) THEN

        -- What we need now is to know if you were rehired
        IF (L_REHIRE_DATE != 0) THEN
            -- In half the cases, move the date two years ahead
            IF (L_FIRST_COIN_FLIP = 0) THEN
                -- We can add two years unless it's too recent
                IF (L_REHIRE_DATE + (2 * 10000) < L_CURRENT_DATE) THEN
                    L_CLASS_DATE := L_REHIRE_DATE + (2 * 10000);
                END IF;
            END IF;
        ELSE
            -- In the no-rehire cases, move the date either
            -- five or ten years ahead

            IF (L_FIRST_COIN_FLIP = 0) THEN
                IF ((L_HIRE_DATE + (5 * 10000)) < L_CURRENT_DATE) THEN
                    L_CLASS_DATE := L_HIRE_DATE + (5 * 10000);
                END IF;
            ELSE
                IF ((L_HIRE_DATE + (10 * 10000)) < L_CURRENT_DATE) THEN
                    L_CLASS_DATE := L_HIRE_DATE + (10 * 10000);
                END IF;
            END IF;

        END IF; -- end of if rehired

    END IF; -- End of if not terminated

    RETURN EMPLOYEE_DATES(L_BIRTH_DATE, L_HIRE_DATE, 
        L_REHIRE_DATE, L_FULL_TIME_DATE, L_TERMINATED_DATE, L_CLASS_DATE);

END GET_EMPLOYEE_DATES;
/
CREATE OR REPLACE FUNCTION GET_PAYPROFIT_RECORD RETURN PAYPROFIT_RECORD IS

    L_PY_PH	NUMBER(6,2);
    L_PY_PD	NUMBER(8,2);
    L_PY_WEEKS_WORK NUMBER(2,0);
    L_PY_PROF_CERT	CHAR(1);
    L_PY_PS_ENROLLED	NUMBER(2,0);
    L_PY_PS_YEARS NUMBER(2,0);

BEGIN   
    
    -- At this time, everyone gets the same hardcoded record
    L_PY_PH := 0;
    L_PY_PD := 0;
    L_PY_WEEKS_WORK := 1;
    L_PY_PROF_CERT := 1;
    L_PY_PS_ENROLLED := 2;
    L_PY_PS_YEARS := 5;
    
    
    RETURN PAYPROFIT_RECORD(
        L_PY_PH,
        L_PY_PD,
        L_PY_WEEKS_WORK,
        L_PY_PROF_CERT,
        L_PY_PS_ENROLLED,
        L_PY_PS_YEARS
    );

END GET_PAYPROFIT_RECORD;
/
CREATE OR REPLACE FUNCTION GET_PROFDIST_RECORD RETURN PROFIT_DISTRIBUTION_RECORD IS

    L_PAYFREQ CHAR(1);
    L_PAYFLAG CHAR(1);
    L_CHECKAMT NUMBER(10,2);
    L_TAXCODE CHAR(1);
    L_DECEASED CHAR(1);
    L_SEX CHAR(1);
    L_FBOPAYTO CHAR(25);

    L_RANDOM_RESULT NUMBER;

    TYPE ARRAY_T IS
        VARRAY(4) OF CHAR(1);
    PAYFREQ_ARRAY ARRAY_T := ARRAY_T('A', 'P', 'M', 'H');

    TYPE ARRAY_F IS
        VARRAY(2) OF CHAR(1);
    PAYFLAG_ARRAY ARRAY_F := ARRAY_F('Y', 'C');

BEGIN   
    
    -- At this time, these fields are unused
    L_TAXCODE := NULL;
    L_DECEASED := 0;

    L_FBOPAYTO := 'Hari Seldon';

    L_CHECKAMT := (ROUND(DBMS_RANDOM.VALUE(50, 1000)) * 10);
    L_RANDOM_RESULT:= ROUND(DBMS_RANDOM.VALUE(1, 4));

    L_PAYFREQ := PAYFREQ_ARRAY(L_RANDOM_RESULT);
    IF (L_RANDOM_RESULT < 3) THEN
        L_PAYFLAG := PAYFLAG_ARRAY(1);
    ELSE
        L_PAYFLAG := PAYFLAG_ARRAY(2);
    END IF;

    L_SEX := GET_A_GENDER();
    
    
    RETURN PROFIT_DISTRIBUTION_RECORD(
        L_PAYFREQ,
        L_PAYFLAG,
        L_CHECKAMT,
        L_TAXCODE,
        L_DECEASED,
        L_SEX,
        L_FBOPAYTO
    );

END GET_PROFDIST_RECORD;
/
CREATE OR REPLACE FUNCTION GET_PROF_DIST_REQ_RECORD RETURN DISTRIBUTION_REQUEST_RECORD IS

    L_REQ_STATUS VARCHAR2(1);
    L_REQ_TYPE  VARCHAR2(25);
    L_REQ_REASON VARCHAR2(250);
    L_REQ_REASON_OTHER VARCHAR2(500);
    L_REQ_AMT_REQ NUMBER(10,2);
    L_REQ_AMT_AUTH NUMBER(10,2);
    L_REQ_DATE_ENT DATE;
    L_REQ_DATE_AUTH	DATE;
    L_REQ_TAXCODE VARCHAR2(1);
    L_REQ_FREQ VARCHAR2(1);

    TYPE ARRAY_RTY IS
        VARRAY(6) OF VARCHAR2(8);
    REQ_TYPE_ARRAY ARRAY_RTY := ARRAY_RTY(
        'HARDSHIP',
	    'MONTHLY',
	    'YEARLY',
	    'ONE_TIME',
	    'PAYOUT',
	    'ROLLOVER'
    );

    TYPE ARRAY_HARD IS
        VARRAY(8) OF VARCHAR2(20);
    HARDSHIP_TYPE_ARRAY ARRAY_HARD := ARRAY_HARD(
        'Home Purchase',
        'Home Repair',
        'Car Repair/Payment',
        'Education Exp',
        'Funeral Exp',
        'Medical/Dental',
        'Eviction/Foreclose',  
        'Other'
    );

BEGIN   

    -- We are not using these fields at this time
    L_REQ_STATUS := NULL;
    L_REQ_REASON_OTHER := NULL;
    
    L_REQ_AMT_AUTH := NULL;
    L_REQ_DATE_ENT := NULL;
    L_REQ_DATE_AUTH := NULL;
    L_REQ_TAXCODE := NULL;
    L_REQ_FREQ := NULL;
    L_REQ_REASON := NULL;

    
    L_REQ_AMT_REQ := (ROUND(DBMS_RANDOM.VALUE(50, 1200)) * 10);
    L_REQ_TYPE := REQ_TYPE_ARRAY(ROUND(DBMS_RANDOM.VALUE(1, 6)));
    IF (L_REQ_TYPE = 'HARDSHIP') THEN
        L_REQ_REASON := HARDSHIP_TYPE_ARRAY(ROUND(DBMS_RANDOM.VALUE(1, 8)));
        
    END IF;
    
    RETURN DISTRIBUTION_REQUEST_RECORD(
        L_REQ_STATUS,
        L_REQ_TYPE,
        L_REQ_REASON,
        L_REQ_REASON_OTHER,
        L_REQ_AMT_REQ,
        L_REQ_AMT_AUTH,
        L_REQ_DATE_ENT,
        L_REQ_DATE_AUTH,
        L_REQ_TAXCODE,
        L_REQ_FREQ
    );

END GET_PROF_DIST_REQ_RECORD;
/
CREATE OR REPLACE FUNCTION GET_PAYREL_RECORD RETURN PAYREL_RECORD IS

    L_PYREL_PSN	NUMBER(9,0);
    L_PYREL_PAYSSN	NUMBER(9,0);
    L_PYREL_TYPE	CHAR(1);
    L_PYREL_PERCENT	NUMBER(3,0);
    L_PYREL_RELATION	CHAR(10);
    L_PYREL_PSAMT	NUMBER(9,2);
    L_PYREL_STATUS	VARCHAR2(10);
    L_PYREL_REASON	VARCHAR2(20);

    TYPE ARRAY_RELATION IS
        VARRAY(6) OF VARCHAR2(8);
        
    ARRAY_REL ARRAY_RELATION := ARRAY_RELATION(
        'HUSBAND',
	    'WIFE',
	    'FATHER',
	    'DAUGHTER',
	    'MOTHER',
	    'SON'
    );

BEGIN   

    -- We are not setting these fields at this time
    L_PYREL_PSN	:= 0;
    L_PYREL_PAYSSN	:= 0;
    L_PYREL_TYPE	:= GET_SPACES_FOR_COLUMN('PAYREL', 'PYREL_TYPE');
    L_PYREL_PSAMT	:= 0;
    L_PYREL_STATUS	:= GET_SPACES_FOR_COLUMN('PAYREL', 'PYREL_STATUS');
    L_PYREL_REASON	:= GET_SPACES_FOR_COLUMN('PAYREL', 'PYREL_REASON');

    L_PYREL_PERCENT	:= 100;
    L_PYREL_RELATION := ARRAY_REL(ROUND(DBMS_RANDOM.VALUE(1, 6)));
 
    RETURN PAYREL_RECORD(
        L_PYREL_PSN,
        L_PYREL_PAYSSN,
        L_PYREL_TYPE,
        L_PYREL_PERCENT,
        L_PYREL_RELATION,
        L_PYREL_PSAMT,
        L_PYREL_STATUS,
        L_PYREL_REASON
    );
END GET_PAYREL_RECORD;
/
CREATE OR REPLACE FUNCTION GET_PROFIT_DETAIL_RECORD (PSN_NUMBER NUMBER) RETURN PROFIT_DETAIL_RECORD IS

    L_PROFIT_YEAR NUMBER(5,1);
    L_DETAIL_CLIENT NUMBER(3,0);
    L_CODE CHAR(1);
    L_CONT NUMBER(9,2);
    L_EARN NUMBER(9,2);
    L_FORT NUMBER(9,2);
    L_MDTE NUMBER(2,0);
    L_YDTE NUMBER(2,0);
    L_CMNT VARCHAR2(16);
    L_ZEROCONT CHAR(1);
    L_FED_TAXES NUMBER(9,2);
    L_STATE_TAXES NUMBER(9,2);
    L_TAX_CODE CHAR(1);

    L_PSN NUMBER(11);

    L_RANDOM_CHECK_NUMBER NUMBER;
    L_RANDOM_PREFIX_NUMBER NUMBER;

    TYPE ARRAY_PRE IS
        VARRAY(10) OF VARCHAR2(13);
    COM_PREF_ARRAY ARRAY_PRE := ARRAY_PRE('DIRECT PAY ',
        'DISTRIBUTION', 'XFER<', 'VOIDED ', 'V_ONLY',
        'DIRPAY ', 'PAY OFF ', 'UN-FORFEIT', 'ROLOVR ', 
        'QDRO<');

BEGIN   

    -- We are not using these fields at this time
    L_PROFIT_YEAR := NULL;
    L_DETAIL_CLIENT := NULL;
    L_CODE := NULL;
    L_CONT := NULL;
    L_FORT := NULL;
    L_MDTE := NULL;
    L_YDTE := NULL;
    
    L_ZEROCONT := NULL;
    L_FED_TAXES := NULL;
    L_STATE_TAXES := NULL;
    L_TAX_CODE := NULL;

    L_RANDOM_CHECK_NUMBER := ROUND(DBMS_RANDOM.VALUE(100000, 500000));
    L_EARN := (ROUND(DBMS_RANDOM.VALUE(50, 1000)) * 10);
    L_RANDOM_PREFIX_NUMBER := ROUND(DBMS_RANDOM.VALUE(1, 10));
    L_CMNT := COM_PREF_ARRAY(L_RANDOM_PREFIX_NUMBER);

    -- Now we need to see what else to add to comment
    IF L_CMNT IN ('XFER<', 'QDRO<') THEN
        L_PSN := PSN_NUMBER;
        L_CMNT := L_CMNT || TO_CHAR(L_PSN);
        L_CODE := '6';
    ELSIF L_CMNT IN ('VOIDED ', 'DIRPAY ', 'ROLOVR ') THEN
        L_CMNT := L_CMNT || L_RANDOM_CHECK_NUMBER || ' ' || get_a_state();
    END IF;

    RETURN PROFIT_DETAIL_RECORD(
        L_PROFIT_YEAR,
        L_DETAIL_CLIENT ,
        L_CODE,
        L_CONT,
        L_EARN,
        L_FORT ,
        L_MDTE,
        L_YDTE,
        L_CMNT,
        L_ZEROCONT,
        L_FED_TAXES,
        L_STATE_TAXES,
        L_TAX_CODE
    );

END GET_PROFIT_DETAIL_RECORD;
/
CREATE OR REPLACE FUNCTION GET_SPACES_FOR_COLUMN(TABLE_NAME VARCHAR, COLUMN_NAME VARCHAR) RETURN VARCHAR IS

    RETURN_VALUE VARCHAR2(50);
    SPACES_REQUIRED NUMBER;
    SQL_BLOCK VARCHAR(200);

BEGIN   
    
    -- Text fields need to be filled with maximum spaces for each column.
    -- This function will build up a string with the proper length and
    -- return it.    
    RETURN_VALUE := '';

    SQL_BLOCK := 'select DATA_LENGTH from USER_TAB_COLUMNS where TABLE_NAME = :1 and COLUMN_NAME = :2';

    execute immediate SQL_BLOCK INTO SPACES_REQUIRED using TABLE_NAME, COLUMN_NAME;

    FOR L_INDEX IN 1..SPACES_REQUIRED
    LOOP
        RETURN_VALUE := RETURN_VALUE || ' ';
    END LOOP;

    -- DBMS_OUTPUT.put_line('TABLE:' || TABLE_NAME || ' COLUMN: ' || COLUMN_NAME || ' SPACES: ' || SPACES_REQUIRED);


    RETURN RETURN_VALUE;

END GET_SPACES_FOR_COLUMN;
/
CREATE OR REPLACE FUNCTION GET_A_STORE_NUMBER RETURN NUMBER IS

    
    TYPE ARRAY_STORES IS
        VARRAY(99) OF NUMBER(3);
        
    -- These are actual store numbers
    STORES_ARRAY ARRAY_STORES := ARRAY_STORES(
        '1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', 
        '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30', 
        '31', '32', '33', '34', '35', '36', '37', '38', '39', '40', '41', '42', '43', '44', '45', 
        '46', '47', '48', '49', '50', '51', '52', '53', '54', '55', '56', '57', '58', '59', '60', 
        '61', '62', '63', '64', '65', '66', '67', '68', '69', '70', '71', '72', '73', '74', '76',
        '77', '78', '79', '80', '81', '82', '83', '84', '85', '86', '87', '88', '89', '986', '988', 
        '990', '991', '992', '993', '994', '995', '996', '997','998'
    );

BEGIN   
    
    RETURN STORES_ARRAY(ROUND(DBMS_RANDOM.VALUE(1, 99)));

END GET_A_STORE_NUMBER;
/
CREATE OR REPLACE FUNCTION GET_PROFIT_SS_DETAIL_RECORD(PSN_NUMBER NUMBER) RETURN PROFIT_SS_DETAIL_RECORD IS

    l_profit_detail_record PROFIT_DETAIL_RECORD;

    FULL_NAME CHAR(25);
    PS_ADDRESS CHAR(20);
    CITY CHAR(13);
    PS_STATE CHAR(2);
    ZIP NUMBER(5);
    SS_NO NUMBER(9);

BEGIN   

    -- We can get most of what we need from the oroginal PD record
    l_profit_detail_record := GET_PROFIT_DETAIL_RECORD(PSN_NUMBER);

    -- Now we just need the other 5 fields

    FULL_NAME := GET_A_FIRST_NAME() || ' ' || GET_A_LAST_NAME();
    PS_ADDRESS := GET_AN_ADDRESS();
    CITY := GET_A_CITY();
    PS_STATE := 'MA';
    ZIP := GET_ZIP_FOR_STATE('MA');
    -- Get a new SSN from our established sequence
    SS_NO := SSN_REPL.NEXTVAL;

    RETURN PROFIT_SS_DETAIL_RECORD(
        l_profit_detail_record.PROFIT_YEAR,
        l_profit_detail_record.DETAIL_CLIENT,
        l_profit_detail_record.CODE,
        l_profit_detail_record.CONT,
        l_profit_detail_record.EARN,
        l_profit_detail_record.FORT ,
        l_profit_detail_record.MDTE,
        l_profit_detail_record.YDTE,
        l_profit_detail_record.CMNT,
        l_profit_detail_record.ZEROCONT,
        l_profit_detail_record.FED_TAXES,
        l_profit_detail_record.STATE_TAXES,
        l_profit_detail_record.TAX_CODE,
        SS_NO,
        FULL_NAME,
        PS_ADDRESS,
        CITY,
        PS_STATE,
        ZIP,
        NULL,
        NULL,
        NULL
    );

END GET_PROFIT_SS_DETAIL_RECORD;
/
CREATE OR REPLACE FUNCTION GET_HOURS RETURN NUMBER IS

    
    TYPE ARRAY_HOURS IS
        VARRAY(5) OF NUMBER(2);
        
    -- These are actual store numbers
    HOURS_ARRAY ARRAY_HOURS := ARRAY_HOURS(
        '40', '43', '44', '45', '46'
    );

BEGIN   
    
    RETURN HOURS_ARRAY(ROUND(DBMS_RANDOM.VALUE(1, 5)));

END GET_HOURS;
