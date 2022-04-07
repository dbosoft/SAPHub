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

function Step1{
[CmdletBinding()]
Param()
Write-Host "

If you confirm the next question, we will open a browser window to the SAP Support Launchpad.

In that window, navigate to: 

  --> SAP NW RFC SDK
      --> SAP NW RFC SDK 7.50

Please download latest version for ""Windows on x64 64 BIT"" and ""Linux on X86_64 64 BIT"".

When you are done, please close the browser window and press enter.
"
    $success = $PSCmdlet.ShouldContinue("Please confirm that you have read the download information.","Ready to proceed?")
    if($success -eq $false){
        return $false 
    }

    Write-Host "
    
We have opened a browser window to the SAP Support Launchpad. Please download the RFC SDK from that location."    
    Start-Process "https://launchpad.support.sap.com/#/softwarecenter/template/products/_APP=00200682500000001943&_EVENT=DISPHIER&HEADER=N&FUNCTIONBAR=Y&EVENT=TREE&TMPL=INTRO_SWDC_SP_AD&V=MAINT&REFERER=CATALOG-PATCHES&ROUTENAME=products/By%20Category%20-%20Additional%20Components"
    
    $success = $PSCmdlet.ShouldContinue("Please confirm that you have downloaded the files.","Download complete?")
    if($success -eq $false){
        return $false 
    }
    Write-Host "
    
Great! Now please extract the files of the WINDOWS SDK zip file to directory '$PSScriptRoot\nwrfcsdk\x64'."    
    $success = $PSCmdlet.ShouldContinue("Please confirm that you have copied the Windows SDK files.","Files copied?")

    if($success -eq $false){
        return $false 
    }

    Write-Host "
    
Alomost done! Finally: please extract the files of the Linux SDK zip file to directory '$PSScriptRoot\nwrfcsdk\linux-x64'."    
    $success = $PSCmdlet.ShouldContinue("Please confirm that you have copied the Linux SDK files.","Files copied?")

    if($success -eq $false){
        return $false 
    }    

    return $success

}



Clear-Host

Write-Host "
Step 1 - Download SAP NW RFC SDK
---------------------------------------------------------------------------

First of all we need the SAP NW RFC SDK. You can only get this directly from SAP. 
This script will guide you to the necessary steps to download the SDK.
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

