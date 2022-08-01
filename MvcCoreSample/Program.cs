using AspNetCore.Proxy;
using MvcCoreSample;
using OSGeo.MapGuide.MaestroAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Register helper delegates that allow us to obtain the right IServerConnection
// based on whether we have a username/password pair or session id
builder.Services.AddTransient<MgConnectionLoginFactory>(sp =>
{
    var conf = sp.GetRequiredService<IConfiguration>();
    return (user, pass) => ConnectionProviderRegistry.CreateConnection("Maestro.Http",
        "Url", conf["MapGuideWebTierBaseUrl"],
        "Username", user,
        "Password", pass);
});
builder.Services.AddTransient<MgConnectionSessionFactory>(sp =>
{
    var conf = sp.GetRequiredService<IConfiguration>();
    return (session) => ConnectionProviderRegistry.CreateConnection("Maestro.Http",
        "Url", conf["MapGuideWebTierBaseUrl"],
        "SessionId", session);
});
// Add proxy middleware. See UseProxies() below for why this sample project needs this
builder.Services.AddProxies();

var app = builder.Build();

var mgBaseUrl = new Uri(app.Configuration["MapGuideWebTierBaseUrl"]);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// The new asp.net (core/5.0+) way of building web applications does not play very nice with the default
// web tier setup of a typical MapGuide installation.
//
// The MapGuide Web Tier is hosted by either the bundled Apache http server or (on Windows) the system installed IIS
// This sample application is expected to be hosted by the Kestrel http server that listens on a different port
//
// Same origin browser policy means that javascript in HTML content served by this sample cannot talk to the client-side
// APIs of any map viewer hosted on the MG Web Tier due to this sample project being in one origin (Kestrel) while the viewer
// content is in a different origin (IIS/Apache). Thus this policy would break InvokeURL commands in this application that
// emit javascript to refresh the map or update the active selection set client-side.
//
// To rectify this you could have Apache be configured to reverse proxy requests of a certain sub-path to this Kestrel
// http server, or for IIS to install the ASP.net core hosting bundle. This is the Microsoft-recommended deployment practice
// for asp.net core/5.0+ applications:
//
//  httpd: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-apache?view=aspnetcore-6.0
//  IIS: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-6.0
//
// Such a concern is beyond the scope of this example project and breaks our "this should work out of the box" developer
// experience if we need you to do a lot of manual messing around with MG Web Tier configurations before you can even spin 
// this thing up!
//
// So in that respect, we are going to do the opposite. This asp.net core application is the definitive point of contact for
// not just your application, but all access to MapGuide's viewer content and mapagent. We do this by proxying every request that
// start with "/mapguide" to the underlying MapGuide Web Tier. The appconfig setting "MapGuideWebTierBaseUrl" determines where
// we will be proxying all these requests to.
//
// This approach is not recommended for production as Kestrel lacks features you may want in a reverse proxying http server.
//
// Consult this link to determine if you need to front Kestrel with a reverse proxying server or not:
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/when-to-use-a-reverse-proxy?view=aspnetcore-6.0
//
// Either way, both the MapGuide Web Tier and this example application will be stood up and hosted from 2 different http servers
// with one needing to proxy requests to the other. Which one does the proxying on production is for you to decide.
//
// For this example application, we assume [kestrel] -> [MG Web Tier on iis/apache]

app.UseProxies(proxies =>
{
    // Proxy all requests whose path segment starts with "/mapguide" to the underlying MG Web Tier
    proxies.Map("mapguide/{**rest}", proxy => proxy.UseHttp((context, args) =>
    {
        var ub = new UriBuilder(new Uri(mgBaseUrl, args["rest"].ToString()));
        // Make sure to capture all query string parameters too!
        ub.Query = context.Request.QueryString.ToString();
        var fullUrl = ub.ToString();
        return fullUrl;
    }));
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
