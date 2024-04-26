using Farola.API.Infrastructure.Behaviors;
using Farola.API.Infrastructure.Exceptions;
using Farola.API.Infrastructure.Extensions;
using Farola.API.Infrastructure.Middlewares;
using Farola.Database.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Farola API",
        Description = "API для сервиса поиска специалистов Farola",
        Contact = new OpenApiContact
        {
            Email = "vsdmitri@gmail.com",
            Name = "Dmitry",
            Url = new Uri("https://github.com/VsD-0")
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Введите JWT токен авторизации.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
            },
            new List<string>()
        }
    });
});

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("OpenCorsPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();
