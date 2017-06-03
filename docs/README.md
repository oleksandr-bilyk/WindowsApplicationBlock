This repo contains demo application implementing advanced Universal Windows Platform (UWP) lifecycle patterns related to ["Launching, resuming, and background tasks"](https://docs.microsoft.com/en-us/windows/uwp/launch-resume/index) MSDN section. Application is organized according to layered architecture using MVVM and Inversion-of-Control/Dependency-Inversion/Dependency-Injection.
# Universal Application architectural topics consideration
There are few important UWP topics that should be reviewed to make architectural decisions.
## UWP as Mobile-First application platform
Microsoft had a chance to build application platform from scratch to meet all potential requirements to mobile applications in XXI century. Developers and other users expect that "Windows 10" will be renamed to "Windows" (number less) and user will not have to buy next OS. That may happen when Win10 market share will reach over 1 billion of users. Windows 10S - is the sample of such free OS where you will pay only for store applications. It is nice to see that Microsoft has a good will to do such innovations. In different Win10 annual builds we see permanent progress in UWP evolution.  
UWP is very different than classic windows application. UWP as Win8-x/Win10 is part of Microsoft's "Mobile First and Cloud First" concept. UWP, as WinRT superset, has strong abstraction from core OS kernel process entity. In comparison to classic windows applications, API calls were referenced to WinRT interfaces. Theoretically, in next 5 years UWP may be ported to other OS platforms as Android.
## Lifecycle Start/Stop rethink
UWP, first of all, differs from other application kinds in its lifetime. Classic UWP application has started/stopping lifecycle points available for handling from process code. To understand UWP lifecycle you should forget everything that you knew about application lifecycle. Basically, UWP has [Not Running, Running and Suspended states](https://docs.microsoft.com/en-us/windows/uwp/launch-resume/app-lifecycle). Additionally we may handle Foreground and Background states.

<img src=/docs/images/Lifecycle.PNG width=300 height=200 />

What did inspire Microsoft engineers to make such [lifecycle](https://docs.microsoft.com/en-us/windows/uwp/launch-resume/app-lifecycle)? I may say that classic Windows/Unix application lifecycle invitation was inspired by some first electric equipment that could be turned ON or OFF. UWP lifecycle is something like lifecycle of human thoughts that exists in creative human head. They may be suspended if computer doesn't have enough resources (Battery or RAM) and be resumed later. When app moves to suspend it should release OS resources (like file handles) because process cannot know if it will be resumed at all. Win10 OS on different devices (Xbox or Desktop) and different builds (10.0 Build 10586, Anniversary Update - 10.0 Build 14393, eg.)  has different heuristic behavior to manage application lifecycle. Some Windows 10 versions may freeze and close process without suspension. Other OS versions may suspend, hibernate and reload your application after computer restart. UWP developer should carefully read all  ["Launching, resuming, and background tasks" MSDN section](https://docs.microsoft.com/en-us/windows/uwp/launch-resume/index) and test different form factors and Win10 builds.
## Run while minimized with extended execution
[Extended execution](https://docs.microsoft.com/en-us/windows/uwp/launch-resume/run-minimized-with-extended-execution) is related to application lifecycle but becomes separate serious topic in UWP application architecture. Extended execution is very important technique for any application logic that executes during one second and longer. Theoretically its usage is simple for basic case but there are many practical questions:
* Developer wants to write logic in the way maximally decoupled from extended execution session manner according to Dependency Inversion principle.
* Applications often have multiple parallel running processes. It incites developer to open two parallel extended execution sessions. However UWP allows requesting only one extended execution session in the same time. Attempt to open the second session before disposing the first one will rise `InvalidOperationException`.
* Application logic needs some flexible enough abstraction over single extended execution session. Application logic needs some manager that controls single extended execution session lifetime. Such manager should be able to host multiple tasks.
* Depending on windows battery charge, energy save mode enable, ["Battery usage by app"](http://www.howto-connect.com/customize-battery-usage-by-app-in-windows-10/) settings and other OS factors, application may call `RequestExtensionAsync()` method and get `ExtendedExecutionResult.Denied`. It doesn't mean that without extended execution application should not allow user to execution some regular long running context that may be denied or revoked after start.
* Every aggregated under extended execution manager task should be notified about shared extended execution session revoke and may handle such event in different ways:
  * Implement task cancellation using `CancellationToken`.
  * return from task's incremental long running loop.
  * notify user with toast notification and continue execution without suspension protection.
* End user knows nothing neither about sophisticated UWP lifecycle no about extended execution nuances. Regular user may change ["Battery usage by app"](http://www.howto-connect.com/customize-battery-usage-by-app-in-windows-10/) settings but that should not change foreground application execution user experience.

Developer should care about all these aspects if application may do some work after it was minimized.
## Free memory when your app moves to the background
Memory pleasure handling for me is advanced programming topic that was described in few good books like:
* Jeffrey Richter "Windows via C/C++" [book](https://www.amazon.com/Windows-via-Jeffrey-M-Richter/dp/0735624240/ref=sr_1_1?s=books&ie=UTF8&qid=1496045667&sr=1-1&keywords=windows+via+c%2Fc)
* Joe Duffy "Concurrent Programming on Windows" [book](https://www.amazon.com/Concurrent-Programming-Windows-Joe-Duffy/dp/032143482X/ref=asap_bc?ie=UTF8)
* Kalen Delaney "Microsoft SQL Server 2012 Internals" [book](https://www.amazon.com/Microsoft-Server-Internals-Developer-Reference-ebook/dp/B00JDMQJYC/ref=asap_bc?ie=UTF8)

In classic Win application when your application is under memory pleasure, you may start getting `OutOfMomoryExceptions`. Additionally, UWP application may be suspended after rising [MemoryManager](https://docs.microsoft.com/en-us/uwp/api/Windows.System.MemoryManager).`AppMemoryUsageLimitChanging` event during background execution. The best sample of application where such approach may be used is SQL Server (extremely complex software) where memory is allocated as blocks (extends) and managed in very advanced way. When we are developing much simpler UWP application that attempts to free resources to don’t be suspended, application handle such case:
* According to MSDN guidelines app may [unload some data and View Layer](https://docs.microsoft.com/en-us/windows/uwp/launch-resume/reduce-memory-usage) if application is in background executions state.
* Try to avoid executing critical work sections in Mobile applications to avoid problems. It is natural for UWP application to be suspended/resumed and most of application tasks execution will not be harmed by suspend.
* Consider how your application layered architecture will work with releasing consumed RAM.

Idea of unloading View layers (setting windows content to null and сollecting all related memory) is very new. In UWP application View layer memory may be collected without [Application](https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Application) class instance with permanently live objects.
How real is such situation in real world mobile application? On the beginning of 2017 Microsoft releases new Surface Pro and Surface Laptop devices with 4GB RAM. It is very real that some student will run some heavy application like Auto Cad and your application RAM limit will be decreased to 250MB. Other, more frequent sample, is when Xbox runs some heavy game that takes maximum RAM and the box will allow your application be alive only if your application will take 100MB maximum. Does it makes sense for your application to stay alive with minimal amount of RAM? Sometimes YES when you are developing Skype competitor that should hold minimal communication with online server. If your application also requires lots of ram and is not designed to release majority of it and restore it later on the move to foreground, maybe, it will make sense to let Windows to suspend your app and store its RAM to the disk.
Anyway, there are few ways of what application can do when [MemoryManager](https://docs.microsoft.com/en-us/uwp/api/Windows.System.MemoryManager).`AppMemoryUsageLimitChanging` event will be riced with arguments warning about memory usage overflow:
* Ignore this fact and expect that application will be suspended and even terminated.
* Free some not required for minimal execution data controlled by Application Logic.
* Unload View level objects according to [Guidelines](https://docs.microsoft.com/en-us/windows/uwp/launch-resume/reduce-memory-usage).
## MVVM in UWP context
[MVVM as a pattern](https://msdn.microsoft.com/en-us/library/hh848246.aspx) is very popular in XAML world.

<img src=/docs/images/Mvvm.PNG width=250 height=200 />

In UWP on Windows 10 University Update (Build 14393) MVVM bindings may be implemented using new [x:Bind (Compiled Binding) markup extension](https://docs.microsoft.com/en-us/windows/uwp/xaml-platform/x-bind-markup-extension). Compiled bindings usage improves performance and debugging experience but doesn't change MVVM principles. Compiled Event Binding usage is important nuance for layered MVVM architecture. MVVM ViewModel can be .NET Standard class without `System.Windows.Input.ICommand` interface usage from `Windows.Foundation.UniversalApiContract` UWP API contract.
## Inversion-of-Control/Dependency Inversion/Dependency-Injection
Dependency Inversion is one of fundamental SOLID principles to make application development process agile (in adjective meaning). The best source of understanding difference and relations between Inversion-of-Control/Dependency Inversion/Dependency-Injection temps is the book by Mark Seemann "Dependency Injection in .NET". Such strong books contains lost of nuances and basic DI implementation samples for different platforms e.g. WPF. However when we build DI for real UWP application we have much more complex relations like relation between some ViewModel that wants to show UWP OK/Cancel modal dialog.
# UWP Application Block
This article describes how Universal Application architectural topics, described above, may be combined in one solution.
## Demo application requirements
Demo application has following features:
* Application domain logic should be built according to Inversion-of-Control and Dependency-Injection principles and has no coupling to UWP.
* Ideally if application logic will reside in .NET Standard projects.
* [Dependency Injection Composition Root](http://blog.ploeh.dk/2015/01/06/composition-root-reuse/) should be compatible with .NET Native that removes type metadata required for reflection. DI Containers use reflection to construct dependencies. That's why in UWP it is better to use Manual Composition Root implementation or register types and namespaces for reflection.
* Application logic should be aware about application lifecycle events.
* Application logic should be able o work with [Extended Execution Session](https://docs.microsoft.com/en-us/windows/uwp/launch-resume/run-minimized-with-extended-execution).
* Application logic is the long living part of process in comparison to View layer that [may be unloaded](https://docs.microsoft.com/en-us/windows/uwp/launch-resume/reduce-memory-usage).
* Adapt ViewModel for Compiled bindings.
* Control [primary and secondary UWP Views (Windows)](https://docs.microsoft.com/en-us/windows/uwp/layout/show-multiple-views) from application logic.
* View Model should be able to control View Frame/Pages navigation.
* Logic that may be shared between few UWP applications should be extracted to application blocks.
## Solution Structure
Application Logic term used in this article means everything related to application code what is not related to View layer and UWP. We may pick few synonyms as: Business Logic, Core Logic, Domain Logic. Application Logic contains ViewModel infrastructure. In this sample ViewModel has no coupling neither to UWP packages no View layer project.
Solution contains 4 projects:

<img src=/docs/images/SolutionDependency.PNG width=450 height=300 />

Application block contains two reusable projects:
1. `ApplicationLogicAbstractions` - Interfaces used by application logic to control environment.
1. `ApplicationLogicEnvironment` - Simplifies UWP application initialization. Controls application execution starting from IApplicationLogicFactory object.

Solution also contains two Demo application projects:
1. `Demo.ApplicationLogic` - resides application logic. Application Logic starts its execution from `IApplicationLogicFactory` interface implementation.
1. `Demo.UniversalWindowsApplication` - UWP application startup project contains:
  * XAML and code behind files.
  * Dependency Injection composition root.
  * Map providing to get XAML Page by view model.
# From `IApplicationLogicFactory` to `IPageViewModelFactory` implementation
* `IApplicationLogicFactory` is the starting point of application used by `ApplicationLogicEnvironment`. `IApplicationLogicFactory` provides `IApplicationLogic` - the root application logic object. `IApplicationLogic` object may be constructed using `IApplicationLogicAgent`. The simplest `IApplicationLogicFactory` may have default constructor and construct `IApplicaitonLogic` instance. In `Demo.UniversalWindowsApplication` project sample `ApplicationLogicFactory` is constructed by Dependency Injection Composition Root and gets additional arguments like `ISemanticLogger` interface.
* `IApplicationLogicAgent` provides all control over Application Logic Environment like application lifecycle. `ApplicationLogicAgent` also allows to open new UWP Views/Windows.
* `IApplicationLogic` - is the most long leaving application logic object because it exists until UWP application termination. That's why it may store shared data state that may survive even after View layer unload. `IApplicationLogic` provides `IWindowFrameControllerFactory` only for primary UWP View/Window. Application logic may start secondary windows using `IApplicationLogicAgent.OpenNewSecondaryViewAsync` method later according to [Show multiple views guide](https://docs.microsoft.com/en-us/windows/uwp/layout/show-multiple-views).
* `IWindowFrameControllerFactory` provides `IWindowFrameController`. `IWindowFrameController` object may be constructed using `IWindowFrameControllerAgent`.
* `IWindowFrameControllerAgent` - manages window content and executes tasks in View/Windows thread [dispatcher](https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Core.CoreDispatcher). The agent also may show View dialogs.
* `IWindowFrameController` is constructed to control window content. `IWindowFrameController` provides `IPageViewModelFactory` to provide initial window page. Application block assumes that UWP window always has [Frame](https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.Frame) as root visual tree element and it needs [Page](https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.Page) for initial navigation.
* `IPageViewModelFactory` provides page view model and identifies view to associate with view model type. This interface is used to navigate [Frame](https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.Frame) to [Page](https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.Page) using `Windows.UI.Xaml.Controls.Frame.Navigate(Type sourcePageType, object parameter)` method.

Essence of relations between main abstractions may be described as:
1. DI composition root provides `IApplicationLogicFactory`
1. `IApplicationLogicFactory` + `IApplicationLogicAgent` =>  `IApplicationLogic`
1. `IApplicationLogic.PrimaryWindowFrameControllerFactory` + `IWindowFrameControllerAgent` => `IWindowFrameController`
1. `IWindowFrameController.StartPageViewModelFactory` provides ViewModel mand Page View identifier.
## Using ApplicationManager to control application execution
To use this application block you should instantiate `ApplicationManager` and call its `ApplicationManager.OnLaunched` method from `Windows.UI.Xaml.Application.OnLaunched` method. `ApplicationManager` will subscribe to `Application` object events and will control application execution. `ApplicationManager` constructor takes following arguments:
1. `IApplicationLogicFactory` implementation to build application logic object.
1. `Windows.UI.Xaml.Application` instance.
1. `Func\<Guid, Type>` to get view type by view identifier associated in `IPageViewModelFactory`.
```cs
/// <summary>
/// Invoked when the application is launched normally by the end user.  Other entry points
/// will be used such as when the application is launched to open a specific file.
/// </summary>
/// <param name="e">Details about the launch request and process.</param>
protected override void OnLaunched(LaunchActivatedEventArgs e)
{
    Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);

    if (applicationManager == null)
    {
        applicationManager = new ApplicationManager(
            () => CompositionRoot.Instance.GetApplicationLogicFactory(),
            this,
            (Guid key) => pageViewMap.Value[key]
        );
    }
    applicationManager.OnLaunched(e);
}
```
`IApplicationLogicFactory` is built by Dependency Injection Composition Root located in `CompositionRoot` class. 
"The term Inversion of Control (IoC) originally meant any sort of programming style where an overall framework or runtime controlled the program flow." ([Martin Fowler, “InversionOfControl,” 2005](http://martinfowler.com/bliki/InversionOfControl.html))
`ApplicationManager` class in combination with `IApplicationLogicFactory` implementation provides Inversion of Control mechanism because `ApplicationManager` controls `IApplicationLogic` execution flow. 
`ApplicationManager` injects main UWP dependencies into `IApplicationLogicFactory` using agents e.g. `IApplicationLogicAgent` and `IWindowFrameControllerAgent`. If application logic needs some UWP specific functionality like `Windows.System.MemoryManager` class, developer may create some abstraction to get memory information in `Demo.ApplicationLogic` project, use it from application logic but implementation will reside it in `Demo.UniversalWindowsApplication` project.
```cs
public interface IApplicationMemoryManager
{
    ulong AppMemoryUsage { get; }
    event EventHandler<AppMemoryUsageLimitChangingEventArgs> AppMemoryUsageLimitChanging;
}

...

internal sealed class ApplicationMemoryManger : IApplicationMemoryManager
{
    public ApplicationMemoryManger()
    {
        MemoryManager.AppMemoryUsageLimitChanging += 
            (sender, e) => OnAppMemoryUsageLimitChanging(e.NewLimit, e.OldLimit);
    }

    public ulong AppMemoryUsage => MemoryManager.AppMemoryUsage;

    public event EventHandler<ApplicationLogic.AppMemoryUsageLimitChangingEventArgs> AppMemoryUsageLimitChanging;

    private void OnAppMemoryUsageLimitChanging(ulong newLimit, ulong oldLimit) => 
        AppMemoryUsageLimitChanging?.Invoke(
            this, 
            new ApplicationLogic.AppMemoryUsageLimitChangingEventArgs(newLimit, oldLimit)
        );
}
```
`IPageViewModelFactory` interface implementation resides in `Demo.ApplicationLogic` project that doesn’t have reference to Page view but has to provide some [Guid](https://msdn.microsoft.com/en-us/library/system.guid(v=vs.110).aspx) identifier. `Demo.UniversalWindowsApplication` project contains [Page](https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.Page) View XAML and can provide type save association between view identifier and View type.
private static Dictionary<Guid, Type> GetPageViewMap() => new Dictionary<Guid, Type>
```cs
{
    {
        ApplicationLogic.MainPage.MainPageViewModelFactory.PageTypeId,
        typeof(MainPage)
    },
    {
        ApplicationLogic.OrganisationCentric.OrganisationCentricViewModelFactory.PageTypeId,
        typeof(OrganisationCentric.OrganisationCentricPageForSecondaryWindow)
    },
    {
        ApplicationLogic.OrganisationCentric.OrganisationCentricPageViewModelFactory.PageTypeId,
        typeof(OrganisationCentric.OrganisationCentricPageForMainWindow)
    },
};
```
Such map should contain key-value pairs for all possible `IPageViewModelFactory` that may be navigated to.
## XAML Page to ViewModel association
Application block makes lots of windows and Frame Page navigation but few things like applying ViewModel to out page should be done manually in code behind.
```cs
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    public MainPageViewModel ViewModel { get; private set; }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        var prameter = (PageNavitedToParameters)e.Parameter;
        ViewModel = (MainPageViewModel)prameter.ViewModel;
    }
}
```
## Lifecycle and Extended execution
Application logic may be notified about all application lifecycle events from `IApplicationLogicAgent` evens: `Suspension`, `Resument`, `EnteredBackground`, `LeavingBackground`. These events are called directly from `Windows.UI.Xaml.Application` event handlers.
`IApplicationLogicAgent` provides `IExtendedExecutionSessionFactory` used to request Extended Execution Session. All `ApplicationLogicAbstractions` contains abstraction to don't have direct coupling to UWP Extended Execution Session API. However `IExtendedExecutionSessionFactory` has the same limitation as UWP application and allows to create only one Extended Execution Session at one period of time. Attempt to open the second session before disposing the first one will rise `InvalidOperationException`. However application logic layer may create some utility to host multiple tasks. Depending on windows battery charge, energy save mode enable, "[Battery usage by app](http://www.howto-connect.com/customize-battery-usage-by-app-in-windows-10/)" settings and other OS factors, application may call `RequestExtensionAsync()` method and get `ExtendedExecutionResult.Denied`. It doesn't mean that without extended execution application should not allow user to execution some regular long running context that may be denied or revoked after start. `Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ExtendedExecutionTaskAgrigation` class provides such functionality to execute tasks under requested extended execution. Demo application contains sample of Report Generation logic that may be called with required or optional extended execution session. Every demo report generation  process may be observer on ViewModel and View layers with notification about changed revoked Extended Execution Session. Please see source code of `Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ReportGeneration.ReportGenerationAgent` class.
## Memory pressure simulation when your app is in background
[MSDN contains documentation](https://docs.microsoft.com/en-us/windows/uwp/launch-resume/reduce-memory-usage) how to handle [MemoryManager.AppMemoryUsageLimitChanging](https://docs.microsoft.com/en-us/uwp/api/Windows.System.MemoryManager) event during background execution. `Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ApplicationLogic` class uses injected `IApplicationMemoryManager` to monitor `AppMemoryUsageLimitChanging` event. Unfortunately it is very difficult to reproduce such event initiated by Windows in background execution. To simulate such process, demo application main window has Simulate button that will run memory pleasure handling logic in 10 seconds when application will be minimized.

<img src=/docs/images/ApplicationSimulateButton.png width=450 height=300 />
