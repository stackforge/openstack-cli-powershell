
  # Install the Extensions Package...
  
  $packageName    = 'acme-cli-powershell'  
  $installerType  = 'msi'  
  #$url           = 'https://onedrive.live.com/redir?resid=4BB76CA4826F85D5%2119977'  
  $url            = 'C:\Users\plummert\Source\Builds\AcmeInc.Openstack.Deployment.msi'  
  $silentArgs     = '/quiet'  
  $validExitCodes = @(0) 

  Install-ChocolateyPackage "$packageName" "$installerType" "$silentArgs" "$url" -validExitCodes $validExitCodes
  
try   
  {   
    # Add the extensions location to PSModulePath for easy loading later on..  

    $p = [Environment]::GetEnvironmentVariable("PSModulePath")
    $p += ";" +  $env:USERPROFILE + "\Documents\WindowsPowerShell\Modules\AcmeInc"
    [Environment]::SetEnvironmentVariable("PSModulePath", $p)

    # Import our Openstack.Client.Powershell assembly. This has the ConfigurationMAnager type which lets you
    # register your Service Provider information.

    $configManagerAsmPath = $env:USERPROFILE + "\Documents\WindowsPowerShell\Modules\Openstack-core\Openstack.Client.Powershell.dll"
	add-type -Path ($configManagerAsmPath)
	
	# Register the Service Provider. This requires the Extension Author to have a ServiceProvider.xml definition file in the 
	# Deployment Folder.
	
	$serviceProviderPath = $env:USERPROFILE + "\Documents\WindowsPowerShell\Modules\AcmeInc\Deployment\ServiceProvider.xml"
	$manager = New-Object -TypeName OpenStack.Client.Powershell.Utility.ConfigurationManager
    $manager.WriteServiceProvider($serviceProviderPath)     
    Write-ChocolateySuccess "$packageName"    
  } 
catch
 {
    Write-ChocolateyFailure "$packageName" "$($_.Exception.Message)"
    throw
 }
