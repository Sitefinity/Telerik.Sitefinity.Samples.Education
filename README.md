Telerik.Sitefinity.Samples.Education
====================================

[![Build Status](http://sdk-jenkins-ci.cloudapp.net/buildStatus/icon?job=Telerik.Sitefinity.Samples.Education.CI)](http://sdk-jenkins-ci.cloudapp.net/job/Telerik.Sitefinity.Samples.Education.CI/)

Telerik International University (TIU) is a sample university website, which demonstrates how you can build an interactive and engaging university website using Sitefinity's default functionality. You can develop the website entirely through Sitefinity's web-based UI. 

This website can created by 1 designer and 1 developer for just 1 week - a demostration of the productivity gains offered by Sitefinity. 

Telerik International University (TIU) is a sample university website that can be reused for commercial purposes. To run the sample you must have standard or higher license. 

Using the Education sample, you can:

* Create a _News_ module, which allows for creation and publishing of news 
* Create an _Events_ module, which makes it easy to announce events through their website and display them on a calendar or a list
* Search and information classification 
* Manage images and videos - upload campus tours, online lectures, interviews with professors and students 
* Support localization and multiple languages - have different language versions of the public website and administrative area 
* Manage the publication system - define where, when, and how it gets published on your website to best engage with your visitors 
* Manage RSS - publish updates to Twitter and other websites
* Create a _Donation_ widget which supports your fund-raising efforts


### Requirements

* Sitefinity license
* .NET Framework 4
* Visual Studio 2012
* Microsoft SQL Server 2008R2 or later versions

### Prerequisites

Clear the NuGet cache files. To do this:

1. In Windows Explorer, open the **%localappdata%\NuGet\Cache** folder.
2. Select all files and delete them.

### Nuget package restoration
The solution in this repository relies on NuGet packages with automatic package restore while the build procedure takes place.   
For a full list of the referenced packages and their versions see the [packages.config](https://github.com/Sitefinity-SDK/Telerik.Sitefinity.Samples.Education/blob/master/SitefinityWebApp/packages.config) file.    
For a history and additional information related to package versions on different releases of this repository, see the [Releases page](https://github.com/Sitefinity-SDK/Telerik.Sitefinity.Samples.Education/releases).    


### Installation instructions: SDK Samples from GitHub


1. In Solution Explorer, navigate to _SitefinityWebApp_ -> *App_Data* -> _Sitefinity_ -> _Configuration_ and select the **DataConfig.config** file. 
2. Modify the **connectionString** value to match your server address.
3. Build the solution.

For version-specific details about the required Sitefinity NuGet packages for this sample application, click on [Releases]
 (https://github.com/Sitefinity-SDK/Telerik.Sitefinity.Samples.Education/releases).

### Login

To login to Sitefinity backend, use the following credentials: 

**Username:** admin
**Password:** password

