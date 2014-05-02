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
Echo 'Welcome to the OpenStack Powershell Environment.'
$a.ForegroundColor = "gray"
Echo '========================================================================================'
Echo ''
$a.ForegroundColor = "green"
echo '   ==> Registering Providers...'

# -----------------------------------------------------------------------------------------------------------------------------------------------------------
# ACTION REQUIRED! ==> Substitute the example CD commands path below to match your setup. This should be the final output of the Solution as dictated by the
#  $(TargetDir) macro in the Post-Build Script.
# -----------------------------------------------------------------------------------------------------------------------------------------------------------






#Save the current value in the $p variable.
$p = [Environment]::GetEnvironmentVariable("PSModulePath")

#Add the new path to the $p variable. Begin with a semi-colon separator.
$p += ";C:\Users\tplummer\Documents\WindowsPowerShell\Modules\OpenStack\"

#Add the paths in $p to the PSModulePath value.
[Environment]::SetEnvironmentVariable("PSModulePath",$p)











 #cd C:\Users\tplummer\Source\Repos\OpenStack-CLI\OpenStack.Client.Powershell\bin\Release
 #cd C:\Users\tplummer\Source\Repos\OpenStack-NewCLI\OpenStack.Client.Powershell\bin\Release
 #import-module .\CLIManifest.psd1 -DisableNameChecking

 import-module OpenStack-Core -DisableNameChecking


cd Builds:
cd 1-3-4-5

#set-sp Rackspace


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














