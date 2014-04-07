Openstack Powershell CLI - Getting Set up to Contribute

I - Pre-requisits
  
 Before we get started we need to make sure that our Development environment reflects what a machine looks like when we install the CLI. The following steps should get the job done.

 A. Set the Required Execution Policy

    To use the Openstack CLI Software for Windows PowerShell, you must make sure that your Powershell environment is capable of executing 3rd party modules.
    Note: If you are performing a re-installation of the software package, you can skip this step. This step is applicable only for a fresh installation.
    Open a PowerShell window as the administrator and issue the command set-executionpolicy -ExecutionPolicy Unrestricted:

	-------------------------------------------------------------------------------------------------------------------
    PS C:\Projects\Outgoing\OS> set-executionpolicy -ExecutionPolicy Unrestricted

    Execution policy change

    The execution policy helps protect you from scripts that you do not trust. Changing the execution policy
    might expose you to the security risks described in the about_Execution_Policies help topic. Do you want to
    change the execution policy?
    [Y] Yes  [N] No  [S] Suspend  [?] Help (default is "Y"): y
    PS C:\Projects\Outgoing\OS> 
	-------------------------------------------------------------------------------------------------------------------
	Close the Powershell window and reopen as administrator for the policy changes to take effect.
 
 B. A Quick Setting in Visual Studio
  
   After Cloning the repo and loading the solution within Visual Studio, ensure that the "Enable Nuget Package Restore"  option has been turned 
   on at the Solution level. This will retreive all of the neccessary external dependencies for the Solution to build properly. 
   
C. - Supplying your Credentials

Our next step is to make sure that we supply the proper credentials when logging on to the system. If you ran the CLI after your initial
install, you'll have noticed that you were prompted for this information. The result is stored in the CLI.config file located within the Users Personal Data Folder
under the OS directory. With each Build we must update that file with the version in our solution. This is so that when new config file changes are introduced, you 
have the CLI picking them up at runtime from the expected location (instead of residing in just your project directory). Complete these steps to supply this information.

1. Navigate to the Openstack.Client.Powershell project and look into the Deployment folder.
2. Within that folder you will see a file called CLI.config.example
3. Copy the example file into the same folder but remove the .example on the filename.
4. Open up the CLI.config file that you just created.
5. Find the config section entitled "IdentityServices".
6. Within this section supply values for Username, Password, and Default Tenenat Id. All of this can be found within your account information on you providers portal.

Each time that we build, this CLI.config file will be moved to the correct runtime location via Post-Build Scripts outlined in the next section.

D. - Edit Post Build Scripts

Within the Openstack.Client.Powershell projects Post-Build Event you'll notice that we call a PostBuild.bat file. This
batch file will move important run-time files required by the CLI to their proper locations if you decide to edit them.
Without this, all changes to important files (like CLI.config) would go unnoticed because they would sit in our project
folders on disk (rather than the location the CLI expects). To create tis batch file, follow these steps.

1. Navigate to the Openstack.Client.Powershell project and look into the Deployment folder.
2. Within that folder you will see a file called PostBuild.example
3. Open up that file and copy the contents to the clipboard. 
4. Within that same folder create a file called PostBuild.bat
5. Paste the script on the clipboard into that file. 
6. Notice the Action Required section of the file. Modify the path you see there to reflect your development machine.
7. Save your Project and do a test build (Rebuild All). The Output Window will show any Echo results from the script and show any script errors that may break the build.

E. - Modifying you Developer Profile 

 When the Powershell runtime starts it looks for a file called Microsoft.PowerShell_profile.ps1. This script gets executed right after
Powershell loads. Here we can set up the look of the environment, issue some default commands (useful for testing!) 
and loading Powershell Modules. The Powershell module is where all of our CLI code resides and must get loaded by this script. 
The only problem here is that the path to that module is specific to your machine. To address this follow these steps

 1. Open up the DevProfile.ps1.example file within the Solutions [Openstack.Client.Powershell]\Deployment folder.
 2. Copy the contents of this file into a new text file called DevProfile.ps1
 3. Search this document for "ACTION REQUIRED" and follow the instructions given.   
 
II - Debugging 

Debugging the CLI is a bit more challenging than with say a normal executable. In this case the entry point or main process is the Powershell runtime itself.
This requires us to load our work into that environment and attach the VS debugger to the Powershell host at runtime. Follow these steps to get the debugger running.

	1. Set your breakpoint.
	2. Compile the Solution. 
	3. Load Powershell. With everything in place this should automatically Import the module you just compiled. 
	3. From within VS select Debug\Attach To Process
	4. Attach to the Powershell.exe process.
	5. Issue a command in the CLI that triggers your breakpoint.

A. Debugging Provider Code

When Powershell modules get Imported, any providers found in that Module will be executed first. This leaves us with a problem in that 
any code that we need to debug in the provider during startup needs to be paused until we have time to actually set a break point properly.
To do this complete the following actions.
	 	 
	1. Set you breakpoint in the providers InitializeDefaultDrives() method. This is the first stage in the Providers Life-Cycle.
	2. Open the DevProfile.ps1 file.
	3. Comment out the Import-Module statement (Copy it to the clipboard for your convience).
	2. Compile the Solution. 
	3. Load Powershell.  
	3. From within VS select Debug\Attach To Process
	4. Attach to the Powershell.exe process.
	5. From the command line paste the Import-Module statement into the CLI. This will trigger the breakpoint that you set in step 1.

B. Getting to the Action - A Quick Tip
	
Sometimes we're trying to debug something in the CLI and we need to get to a certain location quickly. For example if you are testing the 
list view for Servers and your sick of typing this after you restart each time
	
	cd\
	cd Servers
	ls
	cd 1
	ls
	
Just remember that this can be placed in your Devprofile.ps1 file and Powershell will issue those commands for you. This makes debugging a specific 
section of the CLI much easier.. 

III. Testing 

  Object Storage Integration Test Notes

   These unit test require a particular directory structure to be present on your machine to complete the test cases where we copy files and or folders to the server. 
   Make sure that you have something like the following on your machine.

   Folder1 (contains files)
       ->  Folder2 (contains files)
                -> Folder3 (contains NO files)

   Note : Go to the Testing section in the CLI.Config file. Make sure that LocalTestDirectory 
   element points to the root of the test folder hierarchy (Folder1 in the example above).	  