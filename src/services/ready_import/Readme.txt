There are 7 SQL statements for copying Ready Schema to Smart PS schema

The first 5 copy individual tables
1. SQL copy beneficiary - This copies from the ready schema PROFITSHARE tables PAYBEN and PAREL 
  to Smart schema you are in TABLE BENEFICIARY.
2. SQL copy demographic - This copies from the ready schema PROFITSHARE table DEMOGRAPHICS
  to Smart schema you are in TABLE DEMOGRAPHIC
3. SQL copy distribution - This copies from the ready schema PROFITSHARE table PROFDIST
  to Smart schema you are in TABLE DISTRIBUTION
4. SQL copy pay_profit - This copies from the ready schema PROFITSHARE table PAYPROFIT
  to Smart schema you are in TABLE PAY_PROFIT
5. SQL copy profit_detail - This copies from the ready schema PROFITSHARE tables PROFIT_DETAIL and PROFIT_SS_DETAIL
  to Smart schema you are in TABLE PROFIT_DETAIL

This combines the first 5 to load all the tables at once.
6. SQL copy all from ready to smart ps - This copies all 5 talbes from the ready schema PROFITSHARE
  to Smart schema you are in
7. SQL counts smart vs ready - this confirms the table copy has the same row counts in both
   The Smart syste and Ready system.
