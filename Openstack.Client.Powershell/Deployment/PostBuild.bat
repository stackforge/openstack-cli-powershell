echo set DocPath=C:\Users\Travis\Documents

echo  md %DocPath%\WindowsPowershell\
 echo md %DocPath%\OS\
 echo md %DocPath%\OS\

 echo copy ..\..\.\Deployment\DevProfile.ps1                                C:\Users\tplummer\Documents\WindowsPowerShell\Microsoft.PowerShell_profile.ps1
 echo copy ..\..\.\Deployment\OpenStack.config                                    C:\Users\tplummer\Documents\OS\OpenStack.config
 echo copy ..\..\.\Deployment\OpenStack.Client.Powershell.dll-Help.xml      %DocPath%\OS\OpenStack.Client.Powershell.dll-Help.xml
 echo copy ..\..\.\Deployment\OpenstackShell.format.ps1xml                  C:\Users\tplummer\Source\Repos\OpenStack-NewCLI\OpenStack.Client.Powershell\bin\Release\OpenstackShell.format.ps1xml



