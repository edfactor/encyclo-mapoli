

This is code for running the YE Match testing.

See this page for a description of that effort;

https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/272007173/READY+SMART+Year+End+Exact+Match+Testing+YE+Match


Configuration

Go to the checkout directory of the project and add your credetials for READY;

$  dotnet user-secrets set "YEMatchHost:Username" myusernam
$  dotnet user-secrets set "YEMatchHost:Password" 'mypasswd'

Alter your local smart-profit-sharing application to use the test credentials

> git diff src/Demoulas.ProfitSharing.Api/Program.cs

var rolePermissionService = new RolePermissionService();
+#if false
if (!builder.Environment.IsTestEnvironment())
{
-    builder.Services.AddOktaSecurity(builder.Configuration, rolePermissionService);
 } else {
-    builder.Services.AddTestingSecurity(builder.Configuration, rolePermissionService);
 }
+#endif
+builder.Services.AddTestingSecurity(builder.Configuration, rolePermissionService);


