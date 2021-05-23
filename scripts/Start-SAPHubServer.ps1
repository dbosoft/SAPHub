[CmdletBinding()]
Param()


Push-Location
Set-Location $PSScriptRoot\..
Set-Location src\SAPHub.Server
Write-Output "Building SAPHub.Server"
dotnet build  | Tee-Object -Variable cmdOutput
if($LastExitCode -ne 0){
    Write-Error "Failed to build SAPHub.Server"            
}

dotnet run

Pop-Location



