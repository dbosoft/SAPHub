[CmdletBinding()]
Param(
)

Write-Host "Welcome to SAPHub!
=============================================================

This script will guide you through the steps to start SAPHub. 
All you need is 
- an access to the SAP Support Portal with Software download authorization
- an access to a SAP ERP or S/4 system.

"

if(!$PSCmdlet.ShouldContinue("Start setup of SAPHub?", "Ready? Than confirm next question and we will start.")){
    Write-Host "Ok, hope you come back soon. Bye."
    return
}

# loading sub scripts
Push-Location
Set-Location $PSScriptRoot
. ./scripts/Get-SAPNWRfcSdk.ps1
Pop-Location

function CheckSupportCredentials{
    param(
        [pscredential] $Credentials
    )

    if($null -eq $Credentials){
        return $false
    }

    if([string]::IsNullOrWhiteSpace($Credentials.GetNetworkCredential().UserName)){        
        return $false
    }

    if([string]::IsNullOrWhiteSpace($Credentials.GetNetworkCredential().Password)){        
        return $false
    }

    return $true
}
    

function Step1{
[CmdletBinding()]
Param()
    $supportCredentials = Get-Credential -Message "SAP Support Portal Account"
    $checkResult = CheckSupportCredentials($supportCredentials)
    if($checkResult -ne $true) {
        Write-Host "Mmmhm, we need both username and password."
        
        return $false
    }

    Write-Host "Ok, now trying to download SAPNWRFC SDK. Please wait and watch for errors..."
    $success = Get-SAPNWRfcSdk -Credential $supportCredentials -ErrorAction Continue

    return $success

}



Clear-Host

Write-Host "
Step 1 - Download SAP NW RFC SDK
---------------------------------------------------------------------------

First of all we need the SAP NW RFC SDK. You can only get this directly from SAP. 
To download it now, enter the username and the password of the SAP Support Portal account in the following password prompt. 
"
while($true){

    $step1Result = Step1 

    if($step1Result -ne $true){
        if($PSCmdlet.ShouldContinue("You can cancel setup now or repeat step 1.","Try again?")){
            continue
        } else {
            Write-Host "Ok, hope you come back soon. Bye."
            return
        }
    }
    break   
}

if($PSCmdlet.ShouldContinue("You can cancel setup now or continue with the next step.","Continue?") -eq $false){
    Write-Host "Ok, hope you come back soon. Bye."
    return
}

Clear-Host

Write-Host "
Step 2 - Setup SAP Connection
---------------------------------------------------------------------------

The next step is to ask for and test the connection settings for the SAP system. 

"

while($true){
    
    Push-Location
    Set-Location $PSScriptRoot
    $step2Result = ./scripts/Set-SAPConnection.ps1
    Pop-Location

    if($step2Result -ne $true){
        if($PSCmdlet.ShouldContinue("You can cancel setup now or repeat step 2.","Try again?")){
            continue
        } else {
            Write-Host "Ok, hope you come back soon. Bye."
            return
        }
    }   
    break   
}
Write-Host "
All necessary settings are now set up. 
If you want to change your connection data later, you can also start the script Set-SAPConnection.ps1 from the scripts folder directly. 

If you now would like to build and run SAPHub.Server you can start the following script:

./scripts/Start-SAPHubServer.ps1

SAPHub.Server default Url: http://localhost:5000
"

