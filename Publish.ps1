# Paths to the projects
$projects = @(
    "src/Backend/EM.API/EM.API.csproj",
    "src/Backend/EM.McpServer/EM.McpServer.csproj"
)

foreach ($project in $projects) {
    $projectFullPath = Join-Path $PSScriptRoot $project
    $publishDir = Join-Path (Split-Path $projectFullPath -Parent) "bin/publish"
    
    # Remove publish directory for clean publish
    if (Test-Path $publishDir) {
        Remove-Item $publishDir -Recurse -Force
    }
    
    # Publish as Framework Dependent, ignore warnings
    dotnet publish "$projectFullPath" -c Release -o "$publishDir" --self-contained false
}
