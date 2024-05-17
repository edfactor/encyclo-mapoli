# Define the directory where the .buildinfo.json file will be saved
$publishOutputDir = "publishOutput"

# Create the directory if it doesn't already exist
if (-not (Test-Path -Path $publishOutputDir -PathType Container)) {
    New-Item -ItemType Directory -Path $publishOutputDir | Out-Null
}

# Create a hashtable with the build information
$buildInfo = @{
    BuildNumber = (Get-Date).ToUniversalTime().ToString("yyyyMMdd") # Current date in "yyyyMMdd" format
    BuildId = ${bamboo.buildNumber} # Bamboo build number ( no quotes, this is a number )
    PlanName = "${bamboo.planName}" # Bamboo plan name
    PlanRepository = "${bamboo.planRepository.branch}" # Bamboo plan repository branch
    RevisionNumber = "${bamboo.repository.revision.number}" #The revision number of the source that was checked out when the job runs
}

# Convert the hashtable to a JSON string
$jsonContent = $buildInfo | ConvertTo-Json

# Define the path to the .buildinfo.json file within the publishOutput directory
$filePath = Join-Path -Path $publishOutputDir -ChildPath ".buildinfo.json"

# Write the JSON content to the .buildinfo.json file
$jsonContent | Out-File -FilePath $filePath -Encoding UTF8
