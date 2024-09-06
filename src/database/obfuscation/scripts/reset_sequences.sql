/*
  Repeated use or testing of large numbers of records 
  results in the automated SSN and PSN replacement 
  values becoming too large after 10 million records.
  
  Therefore it is a good idea to reset them every so often
  
*/

ALTER sequence PSN_REPL restart start with 700000;
ALTER sequence SSN_REPL restart start with 700000001;
ALTER sequence CHECK_REPL restart START WITH 888000;
ALTER sequence PSN_RANDOM restart START WITH 9000000;

COMMIT;