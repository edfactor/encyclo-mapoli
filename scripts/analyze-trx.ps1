[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$TrxPath,

    [int]$Top = 15,

    [ValidateSet('MessageFirstLine', 'StackFirstLine')]
    [string]$Signature = 'MessageFirstLine'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $TrxPath)) {
    throw "TRX not found: $TrxPath"
}

$xml = New-Object System.Xml.XmlDocument
$xml.Load($TrxPath)

$ns = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
$ns.AddNamespace('t', $xml.DocumentElement.NamespaceURI) | Out-Null

$fails = $xml.SelectNodes("//t:UnitTestResult[@outcome='Failed']", $ns)

$rows = foreach ($f in $fails) {
    $testName = $f.GetAttribute('testName')

    $msgNode = $f.SelectSingleNode('./t:Output/t:ErrorInfo/t:Message', $ns)
    $stackNode = $f.SelectSingleNode('./t:Output/t:ErrorInfo/t:StackTrace', $ns)

    $msg = if ($msgNode) { $msgNode.InnerText } else { '' }
    $stack = if ($stackNode) { $stackNode.InnerText } else { '' }

    $sigText = switch ($Signature) {
        'StackFirstLine' {
            if ($stack -match '(?s)^([^\r\n]+)') { $Matches[1] } else { '<no stack>' }
        }
        default {
            if ($msg -match '(?s)^([^\r\n]+)') { $Matches[1] } else { '<no message>' }
        }
    }

    [pscustomobject]@{
        Signature = $sigText
        Test      = $testName
    }
}

$groups = $rows |
Group-Object Signature |
Sort-Object Count -Descending

$groups |
Select-Object -First $Top |
ForEach-Object {
    $samples = ($_.Group | Select-Object -First 2 -ExpandProperty Test) -join ' | '
    "{0,4}  {1}  ::  {2}" -f $_.Count, $_.Name, $samples
}
