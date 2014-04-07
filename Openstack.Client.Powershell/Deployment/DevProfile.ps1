﻿# ---------------------------------------------------------------
# Set up the default windows size and color....
# ---------------------------------------------------------------
#$DebugPreference=$VerbosePreference="Continue"
#$DebugPreference = "Continue"
$a = (Get-Host).UI.RawUI
$b = $a.WindowSize
$b.Width = 109
$b.Height = 61
$a.WindowSize = $b
cls
# -----------------------------------------------------------------
# Register known providers, write out welcome and status messages..
# -----------------------------------------------------------------
$a.BackgroundColor = "black"
Echo ''
$a.ForegroundColor = "gray"
Echo '========================================================================================'
$a.ForegroundColor = "yellow"
Echo 'Welcome to the OpenStack Powershell Environment.333'
$a.ForegroundColor = "gray"
Echo '========================================================================================'
Echo ''
$a.ForegroundColor = "green"
echo '   ==> Registering Providers...'

# -----------------------------------------------------------------------------------------------------------------------------------------------------------
# ACTION REQUIRED! ==> Substitute the example CD commands path below to match your setup. This should be the final output of the Solution as dictated by the
#  $(TargetDir) macro in the Post-Build Script.
# -----------------------------------------------------------------------------------------------------------------------------------------------------------

 #cd C:\Users\tplummer\Source\Repos\OpenStack-CLI\Openstack.Client.Powershell\bin\Release
 cd C:\Users\tplummer\Source\Repos\OpenStack-NewCLI\Openstack.Client.Powershell\bin\Release

import-module .\CLIManifest.psd1 -DisableNameChecking
cd Builds:
cd 1-3-4-5


#cd OpenStack:
#cd Networks
#get-sp

# ---------------------------------------------------------------
# Let the User know what's going on..
# ---------------------------------------------------------------

echo '   ==> Applying Command Aliases...'
echo '   ==> Registering Views...'
echo ''

# -----------------------------------------------------------------------------------------------------------------------------------------------------------
# Issue any startup commands you would like to execute after everything loads.
# -----------------------------------------------------------------------------------------------------------------------------------------------------------
#cd BlockStorage
#cd Volumes
#ls

# ---------------------------------------------------------------
# Reset Shell to default colors..
# ---------------------------------------------------------------
#$a.ForegroundColor = "yellow"
echo 'Ready..'
#$a.ForegroundColor = "green"
echo ''














