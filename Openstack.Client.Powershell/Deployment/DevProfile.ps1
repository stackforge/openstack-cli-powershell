# ---------------------------------------------------------------
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
Echo 'Welcome to the OpenStack Powershell Environment.11'
$a.ForegroundColor = "gray"
Echo '========================================================================================'
Echo ''
$a.ForegroundColor = "green"
echo '   ==> Registering Providers...'

#Save the current value in the $p variable.
$p = [Environment]::GetEnvironmentVariable("PSModulePath")

#Add the new path to the $p variable. Begin with a semi-colon separator.
$p += ";C:\Users\tplummer\Documents\WindowsPowerShell\Modules\OpenStack\"

#Add the paths in $p to the PSModulePath value.
[Environment]::SetEnvironmentVariable("PSModulePath",$p)

import-module Openstack-Core -DisableNameChecking

cd Builds:

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














