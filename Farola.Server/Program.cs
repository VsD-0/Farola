using Farola.Server.Components;
using FluentValidation.AspNetCore;
using FluentValidation;
using MediatR;
using System.Reflection;
using Farola.Server.Infrastructure.Behaviors;
using Farola.Database.Models;
using Microsoft.EntityFrameworkCore;
using Farola.Server.Infrastructure.Extensions;
using Farola.Server.Infrastructure.Exceptions;
using Farola.Server.Infrastructure.Middlewares;
using Hellang.Middleware.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddControllers();

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("OpenCorsPolicy", opt =>
                opt.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.ConfigureJwtAuthentication(jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings), "Отсутствуют настройки jwt в конфигурации"));

builder.Services.AddProblemDetails(setup =>
{
    setup.IncludeExceptionDetails = (ctx, env) =>
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ||
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Staging";

    setup.Map<CustomException>(exception => new CustomDetails
    {
        Title = exception.Title,
        Detail = exception.Detail,
        Status = StatusCodes.Status500InternalServerError,
        Type = exception.Type,
        Instance = exception.Instance,
        AdditionalInfo = exception.AdditionalInfo
    });
});

builder.Services.AddDbContext<FarolaContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseProblemDetails();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseCors("OpenCorsPolicy");
app.UseAuthorization();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
