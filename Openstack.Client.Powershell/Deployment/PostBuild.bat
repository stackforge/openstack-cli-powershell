
=====================================================================================================================
ACTION REQUIRED : Change the DocPath Variable below to reflect your Personal Documents Folder..
=====================================================================================================================
set DocPath=C:\Users\Travis\Documents

 md %DocPath%\WindowsPowershell\
 md %DocPath%\OS\
 md %DocPath%\OS\

echo copy ..\..\.\Deployment\DevProfile.ps1                                %DocPath%\WindowsPowershell\Microsoft.PowerShell_profile.ps1
copy ..\..\.\Deployment\DevProfile.ps1                                C:\Users\tplummer\Documents\WindowsPowerShell\Microsoft.PowerShell_profile.ps1
copy ..\..\.\Deployment\CLI.config                                    C:\Users\tplummer\Documents\OS\CLI.config
copy ..\..\.\Deployment\Openstack.Client.Powershell.dll-Help.xml      %DocPath%\OS\Openstack.Client.Powershell.dll-Help.xml
copy ..\..\.\Deployment\OpenstackShell.format.ps1xml                  C:\Users\tplummer\Source\Repos\Openstack-NewCLI\Openstack.Client.Powershell\bin\Release\OpenstackShell.format.ps1xml


echo copy   C:\Users\tplummer\Source\Repos\Openstack-NewCLI\Openstack.Client.Powershell\bin\Release\Openstack.Client.Powershell.dll        C:\Users\tplummer\Source\Repos\OpenStack.HP.Extensions\Openstack.HP.Extensions\bin\Debug


echo  %DocPath%\OS\OpenstackShell.format.ps1xml