
Function Set-SAPConnection{
[CmdletBinding()]
param()

    $saphostname = Read-Host -Prompt 'SAP Application Server Hostname'
    $instanceNo = Read-Host -Prompt 'SAP Application Server Instance Number'

    if($instanceNo.Length-ne 2){
        Write-Host -ForegroundColor Red "Instance number doesn't look right (2 numbers)"
        return $false
    }

    $client = Read-Host -Prompt 'SAP System Client'
    if($client.Length-ne 3){
        Write-Host -ForegroundColor Red "System Client doesn't look right (3 characters)"
        return $false
    }

    $credentials = Get-Credential -Message "SAP Username"

    Write-Host ""
    Write-Host "You have entered following settings:"
    Write-Host "SAP Application Server: $saphostname, Instance $instanceNo"
    Write-Host "Logon Client: $client, UserName: $($credentials.GetNetworkCredential().UserName)"
    Write-Host ""


    if ($PSCmdlet.ShouldContinue("Are these settings correct?", "Please Confirm")) {
        Write-Host "The connection test Tool will now be build and run to check the settings."
        Push-Location
        Set-Location $PSScriptRoot\..
        Set-Location src\ConnectionTestTool
        dotnet user-secrets set "saprfc:ashost" "$saphostname" | Write-Verbose
        dotnet user-secrets set "saprfc:sysnr" "$instanceNo"| Write-Verbose
        dotnet user-secrets set "saprfc:client" "$client"| Write-Verbose
        dotnet user-secrets set "saprfc:user" "$($credentials.GetNetworkCredential().UserName)"| Write-Verbose
        dotnet user-secrets set "saprfc:passwd" "$($credentials.GetNetworkCredential().Password)"  | Out-Null
        dotnet user-secrets set "saprfc:lang" "EN"| Write-Verbose

        $cmdOutput = (dotnet build ) -join [Environment]::NewLine
        if($LastExitCode -ne 0){
            Write-Host $cmdOutput
            Write-Error "Failed to build connection test tool."            
            return $false
        }

        $cmdOutput = (dotnet run ) -join [Environment]::NewLine
        if($LastExitCode -ne 0){
            Write-Host $cmdOutput
            Write-Error "Test Connection failed. Please check your connection settings."
            return $false
        }
        Pop-Location
    

        Write-Host "Connection was sucessfully tested."

        Push-Location
        Set-Location $PSScriptRoot\..
        Set-Location src\SAPHub.Server
        Write-Host "Storing settings for SAPHub.Server..."
        dotnet user-secrets set "saprfc:ashost" "$saphostname" | Write-Verbose
        dotnet user-secrets set "saprfc:sysnr" "$instanceNo"| Write-Verbose
        dotnet user-secrets set "saprfc:client" "$client"| Write-Verbose
        dotnet user-secrets set "saprfc:user" "$($credentials.GetNetworkCredential().UserName)"| Write-Verbose
        dotnet user-secrets set "saprfc:passwd" "$($credentials.GetNetworkCredential().Password)"  | Out-Null
        dotnet user-secrets set "saprfc:lang" "EN"| Write-Verbose
        Set-Location ..\SAPHub.SAPConnector
        Write-Host "Storing settings for SAPHub.SAPConnector..."
        dotnet user-secrets set "saprfc:ashost" "$saphostname" | Write-Verbose
        dotnet user-secrets set "saprfc:sysnr" "$instanceNo"| Write-Verbose
        dotnet user-secrets set "saprfc:client" "$client"| Write-Verbose
        dotnet user-secrets set "saprfc:user" "$($credentials.GetNetworkCredential().UserName)"| Write-Verbose
        dotnet user-secrets set "saprfc:passwd" "$($credentials.GetNetworkCredential().Password)"  | Out-Null
        dotnet user-secrets set "saprfc:lang" "EN"| Write-Verbose
        Pop-Location
    }
    return $true
}

Set-SAPConnection