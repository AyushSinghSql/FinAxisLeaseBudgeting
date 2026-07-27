

// using FinAxisLeaseBudgeting.Data;
// using FinAxisLeaseBudgeting.Interfaces;
// using FinAxisLeaseBudgeting.Models;
// using FinAxisLeaseBudgeting.RepositorieS;
// using Microsoft.EntityFrameworkCore;

// var builder = WebApplication.CreateBuilder(args);

// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<FinAxisDbContext>(options =>
//     options.UseNpgsql(connectionString));

// // 1. ADD THIS LINE: Configure CORS to allow your local environment and Swagger
// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policy =>
//     {
//         policy.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//     });
// });

// builder.Services.AddControllers();

// builder.Services.AddScoped<ICommLeaseRepository, CommLeaseRepository>();
// builder.Services.AddScoped<ICommContactRepository, CommContactRepository>();
// builder.Services.AddScoped<ICommCustomerRepository, CommCustomerRepository>();
// builder.Services.AddScoped<ICommLeaseUnitRepository, CommLeaseUnitRepository>();

// builder.Services.AddOpenApi();
// builder.Services.Configure<PowerBISettings>(builder.Configuration.GetSection("PowerBI"));

// var app = builder.Build();

// // 2. ADD THIS LINE: Enable CORS middleware right after app builds
// app.UseCors();

// app.MapOpenApi(); 
// app.UseSwaggerUI(options =>
// {
//     options.SwaggerEndpoint("/openapi/v1.json", "FinAxis API v1");
//     options.RoutePrefix = "swagger";
// });

// app.UseAuthorization();
// app.MapControllers();
// app.Run();



using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Middleware;
using FinAxisLeaseBudgeting.Models;
using FinAxisLeaseBudgeting.RepositorieS;
using FinAxisLeaseBudgeting.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddControllers();

builder.Services.AddScoped<ICommLeaseRepository, CommLeaseRepository>();
builder.Services.AddScoped<ICommContactRepository, CommContactRepository>();
builder.Services.AddScoped<ICommCustomerRepository, CommCustomerRepository>();
builder.Services.AddScoped<ICommLeaseUnitRepository, CommLeaseUnitRepository>();
//Unit Master Interface
builder.Services.AddScoped<IUnitRepository, UnitMasterRepository>();
// Lease Master Interface
builder.Services.AddScoped<ILeaseRepository, LeaseMasterRepository>();
//Property Master Services / Interface
builder.Services.AddScoped<IPropertyRepository, PropertyMasterRepository>();
builder.Services.AddScoped<IPropertyService, PropertyMasterService>();
//Lease Charge Services / Interface
builder.Services.AddScoped<ILeaseChargeRepository, LeaseChargeRepository>();
builder.Services.AddScoped<ILeaseChargeService, LeaseChargeService>();

//Exception Middleware
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // Extract missing field names and error messages automatically
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
// -------------------------------------------------------------------

builder.Services.Configure<PowerBISettings>(builder.Configuration.GetSection("PowerBI"));

var app = builder.Build();

//Exception handling (Error Handling)
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors();

app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "FinAxis API v1");
    options.RoutePrefix = "swagger";
});

app.UseAuthorization();
app.MapControllers();
app.Run();