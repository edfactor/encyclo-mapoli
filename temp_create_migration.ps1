Set-Location "d:\source\Demoulas\smart-profit-sharing\src\services\src\Demoulas.ProfitSharing.Data"
dotnet ef migrations add SimplifyEnrollmentModel --context ProfitSharingDbContext --verbose
$exitCode = $LASTEXITCODE
Write-Host "Exit code: $exitCode"
Set-Location "d:\source\Demoulas\smart-profit-sharing"
exit $exitCode
