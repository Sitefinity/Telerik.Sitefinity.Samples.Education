Telerik.Sitefinity.Samples.Education
====================================

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

* Sitefinity 6.3 license

* .NET Framework 4

* Visual Studio 2012

* Microsoft SQL Server 2008R2 or later versions


### Installation instructions: SDK Samples from GitHub

1. Clone the [Telerik.Sitefinity.Samples.Dependencies](https://github.com/Sitefinty-SDK/Telerik.Sitefinity.Samples.Dependencies) repo to get all assemblies necessary to run for the samples.
2. Fix broken references in the class libraries, for example in **SitefinityWebApp** and **Telerik.Sitefinity.Samples.Common**:

  1. In Solution Explorer, open the context menu of your project node and click _Properties_.  
  
    The Project designer is displayed.
  2. Select the _Reference Paths_ tab page.
  3. Browse and select the folder where **Telerik.Sitefinity.Samples.Dependencies** folder is located.
  4. Click the _Add Folder_ button.


3. In Solution Explorer, navigate to _SitefinityWebApp_ -> *App_Data* -> _Sitefinity_ -> _Configuration_ and select the **DataConfig.config** file. 
4. Modify the **connectionString** value to match your server address.
5. Build the solution.

### Login

To login to Sitefinity backend, use the following credentials: 

**Username:** admin

**Password:** password


