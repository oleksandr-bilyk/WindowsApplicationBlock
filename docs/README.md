This repo contains demo application implementing advanced Universal Windows Platform (UWP) lifecycle patterns related to ["Launching, resuming, and background tasks"](https://docs.microsoft.com/en-us/windows/uwp/launch-resume/index) MSDN section. Application is organized according to layered architecture using MVVM and Inversion-of-Control/Dependency-Inversion/Dependency-Injection.
# Universal Application architectural topics consideration
There are few important UWP topics that should be reviewed to make architectural decisions.
## UWP as Mobile-First application platform
Microsoft had a chance to build application platform from scratch to meet all potential requirements to mobile applications in XXI century. Developers and other users expect that "Windows 10" will be renamed to "Windows" (number less) and user will not have to buy next OS. That may happen when Win10 market share will reach over 1 billion of users. Windows 10S - is the sample of such free OS where you will pay only for store applications. It is nice to see that Microsoft has a good will to do such innovations. In different Win10 annual builds we see permanent progress in UWP evolution.  
UWP is very different than classic windows application. UWP as Win8-x/Win10 is part of Microsoft's "Mobile First and Cloud First" concept. UWP, as WinRT superset, has strong abstraction from core OS kernel process entity. In comparison to classic windows applications, API calls were referenced to WinRT interfaces. Theoretically, in next 5 years UWP may be ported to other OS platforms as Android.
## Lifecycle Start/Stop rethink
UWP, first of all, differs from other application kinds in its lifetime. Classic UWP application has started/stopping lifecycle points available for handling from process code. To understand UWP lifecycle you should forget everything that you knew about application lifecycle. Basically, UWP has [Not Running, Running and Suspended states](https://docs.microsoft.com/en-us/windows/uwp/launch-resume/app-lifecycle). Additionally we may handle Foreground and Background states.
<br/>
<img src=/docs/images/Lifecycle.PNG width=300 height=200 />

