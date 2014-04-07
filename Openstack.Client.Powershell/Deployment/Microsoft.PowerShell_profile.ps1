# ---------------------------------------------------------------
# Set up support methods first..
# ---------------------------------------------------------------

		 function is64bit() {
		  return ([IntPtr]::Size -eq 8)
		}

		function get-programfilesdir() {
		  if (is64bit -eq $true) {
			(Get-Item "Env:ProgramFiles(x86)").Value
		  }
		  else {
			(Get-Item "Env:ProgramFiles").Value
		  }
		}
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
		$a.BackgroundColor = "black"
	    $a.ForegroundColor = "green"

		cls
# ---------------------------------------------------------------
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

	$tempvar = get-programfilesdir
	$tempvar = $tempvar  + "\Openstack\OpenStack-Powershell"
	cd $tempvar
	import-module .\CLIManifest.psd1  -DisableNameChecking  

	echo '   ==> Applying Command Aliases...'
	echo '   ==> Registering Views...'
	echo ''

# ---------------------------------------------------------------
# Reset Shell to default colors.. 
# ---------------------------------------------------------------

	$a.ForegroundColor = "yellow"
	echo 'Ready..'
	$a.ForegroundColor = "green"
	echo ''
















 