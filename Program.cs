using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Middleware;
using FinAxisLeaseBudgeting.Models;
using FinAxisLeaseBudgeting.RepositorieS;
using FinAxisLeaseBudgeting.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using PlanningAPI.Repositories;
using Serilog;
using System.Text.Json;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting FinAxis Lease Budgeting Web Application...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers(options =>
    {
        // 1. Create a policy that requires authenticated users
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        // 2. Apply this policy globally to every single controller action
        options.Filters.Add(new AuthorizeFilter(policy));
    });

    // Attach Serilog as the main logging provider
    builder.Host.UseSerilog();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<FinAxisDbContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddHttpClient();

    // Configure CORS
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<FinAxisDbContext>("PostgreSQL Database");

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler =
                System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var errorResponse = new
                {
                    statusCode = 400,
                    message = "Validation failed for one or more fields.",
                    errors = errors,
                    timestamp = DateTime.UtcNow
                };

                return new BadRequestObjectResult(errorResponse);
            };
        });

    builder.Services.AddScoped<ICommLeaseRepository, CommLeaseRepository>();
    builder.Services.AddScoped<ICommContactRepository, CommContactRepository>();
    builder.Services.AddScoped<ICommCustomerRepository, CommCustomerRepository>();
    builder.Services.AddScoped<ICommLeaseUnitRepository, CommLeaseUnitRepository>();

    // Unit Master Interface
    builder.Services.AddScoped<IUnitRepository, UnitMasterRepository>();

    // Lease Master Interface
    builder.Services.AddScoped<ILeaseRepository, LeaseMasterRepository>();

    // Property Master Services / Interface
    builder.Services.AddScoped<IPropertyRepository, PropertyMasterRepository>();
    builder.Services.AddScoped<IPropertyService, PropertyMasterService>();

    // Lease Charge Services / Interface
    builder.Services.AddScoped<ILeaseChargeRepository, LeaseChargeRepository>();
    builder.Services.AddScoped<ILeaseChargeService, LeaseChargeService>();

    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();

    builder.Services.AddScoped<IReportGroupRepository, ReportGroupRepository>();

    // OpenAPI Specification Config
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            if (builder.Environment.IsDevelopment())
            {
                document.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
                {
                    new Microsoft.OpenApi.Models.OpenApiServer
                    {
                        Url = "https://localhost:7000",
                        Description = "Local Development"
                    }
                };
            }
            else
            {
                document.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
                {
                    new Microsoft.OpenApi.Models.OpenApiServer
                    {
                        Url = "https://finaxisleasebudgeting.onrender.com",
                        Description = "Production Server"
                    }
                };
            }
            return Task.CompletedTask;
        });
    });

    builder.Services.Configure<PowerBISettings>(builder.Configuration.GetSection("PowerBI"));

    var app = builder.Build();

    // Custom Error Handling Middleware
    app.UseMiddleware<ExceptionMiddleware>();

    // Serilog Request Logging
    app.UseSerilogRequestLogging();

    app.UseCors();
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "FinAxis API v1");
        options.RoutePrefix = "swagger";
    });

    app.UseAuthorization();

    // Map Health Check Endpoint
    app.MapHealthChecks("/health");

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start correctly!");
}
finally
{
    Log.CloseAndFlush();
}