
rem Used to generate the swagger ApiClient
rem

dotnet tool install --global NSwag.ConsoleCore 

rem
nswag openapi2csclient /classname:ApiClient /namespace:YEMatch /input:https://localhost:7141/swagger/Release%%201.0/swagger.json /output:YEMatch\SmartActivities\ApiClient.cs

rem this makes the swagger endpoint names less wordy
powershell -Command "(Get-Content YEMatch\SmartActivities\ApiClient.cs) -replace 'DemoulasProfitSharingEndpointsEndpoints', '$1' | Set-Content YEMatch\SmartActivities\ApiClient.cs"
