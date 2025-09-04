# Generates a route → policy coverage CSV by scanning FastEndpoints groups and endpoints.
# Usage:
#   pwsh -File scripts/Generate-EndpointPolicyCoverage.ps1 -EndpointsRoot "src/services/src/Demoulas.ProfitSharing.Endpoints" -OutCsv "reports/endpoint_policy_coverage.csv"

[CmdletBinding()]
param(
  [string]$EndpointsRoot = "d:\source\Demoulas\smart-profit-sharing\src\services\src\Demoulas.ProfitSharing.Endpoints",
  [string]$OutCsv = "d:\source\Demoulas\smart-profit-sharing\test-results\endpoint_policy_coverage.csv"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-GroupInfo {
  param([string]$GroupsDir)
  $map = @{}
  Get-ChildItem -Path $GroupsDir -Filter *.cs -File -Recurse | ForEach-Object {
    $text = Get-Content -Raw -LiteralPath $_.FullName
    $class = [regex]::Match($text, 'class\s+(?<name>\w+)\s*:\s*GroupBase').Groups['name'].Value
    if (-not $class) { return }
    $route = [regex]::Match($text, 'protected\s+override\s+string\s+Route\s*=>\s*"(?<route>[^"]+)"').Groups['route'].Value
    if (-not $route) { $route = $class }
    $policy = [regex]::Match($text, 'ep\.Policies\((?<policy>[^\)]+)\)').Groups['policy'].Value.Trim()
    $map[$class] = [pscustomobject]@{
      GroupClass  = $class
      GroupRoute  = $route
      GroupPolicy = $policy
      File        = $_.FullName
    }
  }
  return $map
}

function Get-EndpointsInfo {
  param([string]$EndpointsDir)
  $list = @()
  Get-ChildItem -Path $EndpointsDir -Filter *.cs -File -Recurse | ForEach-Object {
    $text = Get-Content -Raw -LiteralPath $_.FullName
    # Skip group files
    if ($text -match ':\s*GroupBase') { return }

    $group = [regex]::Match($text, 'Group<(?<grp>\w+)>\s*\(\)').Groups['grp'].Value
    if (-not $group) { return }

    $httpMatch = [regex]::Matches($text, '(?<verb>Get|Post|Put|Delete)\s*\(\s*"(?<route>[^"]*)"')
    if ($httpMatch.Count -eq 0) { return }

    $endpointPolicy = [regex]::Match($text, '(?<!ep\.)Policies\((?<policy>[^\)]+)\)').Groups['policy'].Value.Trim()

    foreach ($m in $httpMatch) {
      $verb = $m.Groups['verb'].Value
      $route = $m.Groups['route'].Value
      $list += [pscustomobject]@{
        Verb          = $verb
        RouteFragment = $route
        GroupClass    = $group
        EndpointPolicy= $endpointPolicy
        File          = $_.FullName
      }
    }
  }
  return $list
}

Write-Host "Scanning groups and endpoints in: $EndpointsRoot"
$groupsDir = Join-Path $EndpointsRoot 'Groups'
$endpointsDir = Join-Path $EndpointsRoot 'Endpoints'
$groupMap = Get-GroupInfo -GroupsDir $groupsDir
$endpoints = Get-EndpointsInfo -EndpointsDir $endpointsDir

if (-not $endpoints) {
  throw "No endpoints found under $endpointsDir"
}

$rows = @()
foreach ($ep in $endpoints) {
  $g = $groupMap[$ep.GroupClass]
  $groupRoute = $g.GroupRoute
  $groupPolicy = $g.GroupPolicy
  $effectivePolicy = if ($ep.EndpointPolicy) { $ep.EndpointPolicy } else { $groupPolicy }

  # Normalize combined route (skip leading/trailing slashes)
  $frag = ($ep.RouteFragment ?? '')
  if ($frag.StartsWith('/')) { $frag = $frag.TrimStart('/') }
  $combined = if ([string]::IsNullOrWhiteSpace($frag)) { $groupRoute } else { "$groupRoute/$frag" }

  $rows += [pscustomobject]@{
    Verb            = $ep.Verb
    Path            = $combined
    Group           = $ep.GroupClass
    GroupRoute      = $groupRoute
    GroupPolicy     = $groupPolicy
    EndpointPolicy  = $ep.EndpointPolicy
    EffectivePolicy = $effectivePolicy
    File            = $ep.File
  }
}

$outDir = Split-Path -Parent $OutCsv
if (-not (Test-Path -LiteralPath $outDir)) { New-Item -ItemType Directory -Force -Path $outDir | Out-Null }

$rows |
  Sort-Object Group, Path, Verb |
  Export-Csv -NoTypeInformation -Force -LiteralPath $OutCsv

Write-Host "Coverage written to: $OutCsv"
