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

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
