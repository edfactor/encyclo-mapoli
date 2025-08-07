BEGIN
DELETE FROM NAVIGATION_ASSIGNED_ROLES;
DELETE FROM NAVIGATION_TRACKING;
DELETE FROM NAVIGATION;

-- UAT Navigation Data - Limited to December Activities, Unforfeit, Terminations, and Master Inquiry
-- This script includes only the initial UAT routes and can be extended over time

--Main menu - INQUIRIES (required for Master Inquiry)
INSERT INTO NAVIGATION(ID,PARENT_ID, TITLE, SUB_TITLE, URL, STATUS_ID, ORDER_NUMBER, ICON, DISABLED) 
VALUES(50,null, 'INQUIRIES', '','', 1, 1, '', 0);

--Sub values for INQUIRIES
INSERT INTO NAVIGATION(ID,PARENT_ID, TITLE, SUB_TITLE, URL, STATUS_ID, ORDER_NUMBER, ICON, DISABLED) 
VALUES(51,50, 'MASTER INQUIRY', '','master-inquiry', 1, 1, '', 0);

--Main menu - YEAR END (required parent for December Activities)
INSERT INTO NAVIGATION(ID,PARENT_ID, TITLE, SUB_TITLE, URL, STATUS_ID, ORDER_NUMBER, ICON, DISABLED) 
VALUES(55,null, 'YEAR END', '','', 1, 5, '', 0);

--December Activities (main container)
INSERT INTO NAVIGATION(ID,PARENT_ID, TITLE, SUB_TITLE, URL, STATUS_ID, ORDER_NUMBER, ICON, DISABLED) 
VALUES(1,55, 'December Activities', '','december-process-accordion', 1, 1, '', 0);

--December Activities sub-items (UAT Phase 1)
INSERT INTO NAVIGATION(ID,PARENT_ID, TITLE, SUB_TITLE, URL, STATUS_ID, ORDER_NUMBER, ICON, DISABLED) 
VALUES(8,1, 'UnForfeit', 'QPREV-PROF','unforfeitures', 1, 2, '', 0);

INSERT INTO NAVIGATION(ID,PARENT_ID, TITLE, SUB_TITLE, URL, STATUS_ID, ORDER_NUMBER, ICON, DISABLED) 
VALUES(9,1, 'Terminations', 'QPAY066','prof-term', 1, 4, '', 0);


END;
COMMIT ;
