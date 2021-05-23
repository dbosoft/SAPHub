
Function  Get-SAPNWRfcSdkFile{
    [CmdletBinding()]
    param(
        [pscredential] $Credential,
        [string] $OsName,
        [string] $FolderName
    )

    $env:SAPSDK_sap__username = $Credential.GetNetworkCredential().UserName
    $env:SAPSDK_sap__password = $Credential.GetNetworkCredential().Password

    Push-Location
    try{
        Set-Location $PSScriptRoot/..

        $cmdOutput = (dotnet sapnwrfc-download --os $OsName --x86 false --outputDir nwrfcsdk --unzipAs $FolderName 2>&1) -join [Environment]::NewLine
        if($LastExitCode -ne 0){
            Write-Host $cmdOutput
            Write-Error "Failed to download the SDK file for $osName."
            return $false
        }

        return $true

    }finally
    {
        Pop-Location
        $env:SAPSDK_sap__username = ""
        $env:SAPSDK_sap__password = ""
    }

}

Function  Get-SAPNWRfcSdk{
[CmdletBinding()]
param(
    [pscredential] $Credential
)

    if($null -eq $Credential ) {
        $Credential | Get-Credential -Prompt "SAP Support Portal Account"

    }

    Push-Location

    try{
        Set-Location $PSScriptRoot/..
        $cmdOutput = (dotnet tool restore) -join [Environment]::NewLine
        if($LastExitCode -ne 0){
            Write-Host $cmdOutput
            Write-Error "Failed to restore dotnet tools"
                        
            return $false
        }

       mkdir nwrfcsdk -ErrorAction SilentlyContinue | Out-Null
    }finally
    {
        Pop-Location
    }

    if((Get-SAPNWRfcSdkFile -Credential $Credential -OsName linux -FolderName linux-x64) -ne $true){
        return $false
    }
    Write-Host "Downloaded SAP NW RFC SDK for Linux."

    if((Get-SAPNWRfcSdkFile -Credential $Credential -OsName windows -FolderName x64) -ne $true){
        return $false
    }
    Write-Host "Downloaded SAP NW RFC SDK for Windows."
 
    return $true

}
