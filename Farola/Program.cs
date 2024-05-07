using Farola;
using Farola.API;
using Farola.Infrastructure.Handlers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;

// API https://localhost:7091
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddTransient<TokenHandler>();

builder.Services.AddRefitClient<IProfessionalClient>().ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri("https://localhost:7091/api");
});//.AddHttpMessageHandler<TokenHandler>();

builder.Services.AddRefitClient<IApplicantClient>().ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri("https://localhost:7091/api");
});//.AddHttpMessageHandler<TokenHandler>();

builder.Services.AddRefitClient<IUserClient>().ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri("https://localhost:7091/api");
});//.AddHttpMessageHandler<TokenHandler>();

builder.Services.AddRefitClient<IStatementClient>().ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri("https://localhost:7091/api");
});//.AddHttpMessageHandler<TokenHandler>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
