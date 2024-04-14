using Farola;
using Farola.API;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;

// API https://localhost:7091
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddRefitClient<IProfessionalClient>().ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri("https://localhost:7091/api");
});

builder.Services.AddRefitClient<IUserClient>().ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri("https://localhost:7091/api");
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
