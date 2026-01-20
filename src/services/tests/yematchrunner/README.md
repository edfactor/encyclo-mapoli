This is code for running the YE Match testing.

See this page for a description of that effort;

https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/272007173/READY+SMART+Year+End+Exact+Match+Testing+YE+Match

# Configuration

Go to the checkout directory of the project and add your credentials for READY;

$ dotnet user-secrets set "YEMatchHost:Username" myusernam
$ dotnet user-secrets set "YEMatchHost:Password" 'mypasswd'

Also set the connection strings for READY and SMART databases

$ dotnet user-secrets set SmartConnectionString "Data Source= (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = tdcexa-scan1.mainoffice.demoulas.corp)(PORT = 1521)) ) (CONNECT_DATA = (SERVICE_NAME = test10d) (SERVER = DEDICATED)));User Id=smartUser;Password=smartPass"
$ dotnet user-secrets set ReadyConnectionString "Data Source= (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL =
TCP)(HOST = tdcexa-scan1.mainoffice.demoulas.corp)(PORT = 1521)) ) (CONNECT_DATA = (SERVICE_NAME = test10d) (SERVER = DEDICATED)));User
Id=readyUser;Password=readyPass"

You can configure the "log" directory that YE match uses;

it defaults to /tmp/ye/<date>

The READY program output is piled up there.

## Configuration on READY

setup the "setyematch" script. Sets credentials for READY to use.

# Running

1. Run your local smart-profit-sharing application to use the test credentials, the launch configuration "API YE Match (
   Test Certs)".

2. Run the Program.cs
