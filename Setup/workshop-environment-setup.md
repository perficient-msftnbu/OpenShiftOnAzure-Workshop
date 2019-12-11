# workshop environment setup guide

> this installation guide is meant for a Windows machine

## table of contents

- [workshop environment setup guide](#workshop-environment-setup-guide)
  - [table of contents](#table-of-contents)
  - [```dotnet``` framework](#dotnet-framework)
    - [```dotnet``` installation walkthrough](#dotnet-installation-walkthrough)
    - [```dotnet``` installation verification](#dotnet-installation-verification)
  - [```node``` package manager](#node-package-manager)
    - [```node``` installation walkthrough](#node-installation-walkthrough)
    - [```node``` installation verification](#node-installation-verification)
  - [OpenShift Origin (command-line tool)](#OpenShift-Origin-command-line-tool)
    - [OpenShift Origin installation walkthrough](#OpenShift-Origin-installation-walkthrough)
    - [OpenShift Origin installation verification](#OpenShift-Origin-installation-verification)

## ```dotnet``` framework

[dotnet installer link](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial/install)

### ```dotnet``` installation walkthrough

1. download the dotnet SDK (64-bit) to an easily-accessible folder
2. double-click the installer and continue through the installer's instructions

### ```dotnet``` installation verification

once the install completes, check everything is installed correctly
   * press the Windows key and enter 'cmd' in order to open the command prompt
   * enter ```dotnet``` in the prompt and wait for it to return with information about how to use dotnet 

## ```node``` package manager

[node.js windows installer link](https://nodejs.org/en/download/)

### ```node``` installation walkthrough

1. download the Node.js installer (64-bit) to an easily-acessible folder
2. double-click the installer and continue through the installer's instructions
    * allow the default components to be installed
   
### ```node``` installation verification

verify that the Node.js installation completed successfully by entering ```node``` in a new command prompt
   * exit Node by pressing the key-combination: ```Ctrl-C``` twice

## OpenShift Origin (command-line tool)

[OpenShift Origin releases download link](https://github.com/openshift/origin/releases/download/v3.11.0/openshift-origin-client-tools-v3.11.0-0cbc58b-windows.zip)

### OpenShift Origin installation walkthrough

1. extract the downloaded .zip archive to a directory that is safe from deletion
2. right-click the .zip archive, select **Extract All...**
   * select **Browse...** and navigate to a directory that is safe from modification
     * it is recommended to install ```Program Files (x86)```
     * navigate to ```<Main Computer Mount (Drive Letter, i.e.: C, D)>\Program Files (x86)```
     * create a new folder in ```\Program Files (x86)``` titled: ```OpenShift Origin```
     * press **Select Folder**
     * select **Extract**
3. navigate to the directory with the extracted OpenShift files with File Explorer
4. at the top of the File Explorer window, select the full path and copy by pressing the key-combination ```Ctrl-C```
5. press the Windows key to bring up the Start menu and begin to type: ```environment variables```
   * select **Edit the system environment variables**
   * in the **System Properties** dialog window that appears, at the bottom right, select **Environment Variables...**
   * select the ```Path``` variable and select **Edit...**
     * select **New** and paste in the full directory copied earlier
   * select **OK** three times in order to close all the newly-opened dialog windows

### OpenShift Origin installation verification

verify that OpenShift Origin was installed correctly by opening a new command-prompt and entering ```oc```, when the prompt returns with the ```--help``` guide, we can rest assured that our machine is finally ready to begin interacting with OpenShift on Azure!