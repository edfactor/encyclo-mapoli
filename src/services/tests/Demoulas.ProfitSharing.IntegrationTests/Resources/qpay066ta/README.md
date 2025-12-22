This directory contains the complete "full-report.txt" which is the output of

$ EJR YE-PROF-BREAK FILEONLY=N FRSTDATE=20231231 LASTDATE=20241228 YDATE=2024 LBLOPT=NO

ran on 10 March 2025.

The other files in this directory are different sections of this file. The sections have
been extracted to make testing the SMART endpoints more straight forward.

In order for these tests to pass, they require that SMART be in the exact same state as it was on 10 March 2025.  
This is likely a tall order, getting smart back to the same state can be tricky. In theory, running YE Match for 2024,
should get SMART back to the same state. However, we recently switched to importing as 2025/2024, so it is unclear
if we can execute the 2024 year end.
