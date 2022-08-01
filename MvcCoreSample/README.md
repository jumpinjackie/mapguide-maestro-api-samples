# Maestro API samples for ASP.net core

This is the port of the existing `SamplesWeb` application from legacy ASP.net to ASP.net (6.0 as of this writing)

This application demonstrates the following:

 * Querying the layer structure of the current runtime map being viewed
 * Querying the state of the active selection of the map being viewed
 * Manipulating layer/group visibility
 * Adding/removing layers of the current runtime map being viewed
 * Querying selected features
 * Applying new active selections programmatically

 ## Requirements

You must have an existing MapGuide installation with IIS or the bundled Apache already running. Although Maestro API has a wide-range of supported MapGuide versions, for best results you should have either the latest stable 3.1.2 release or the current 4.0 Preview 3 release installed.

This application runs on top of a separate http server (Kestrel) and proxies all relevant MapGuide Web Tier requests to IIS/Apache

You need [Visual Studio](https://visualstudio.microsoft.com/vs/) 2022 (with .net 6.0 workloads enabled) or the [.net 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) to build and run this application.