// using FinAxisLeaseBudgeting.Data;
// using FinAxisLeaseBudgeting.Interfaces;
// using FinAxisLeaseBudgeting.Models;
// using FinAxisLeaseBudgeting.RepositorieS;
// using Microsoft.EntityFrameworkCore;

// var builder = WebApplication.CreateBuilder(args);

// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<FinAxisDbContext>(options =>
//     options.UseNpgsql(connectionString));

// // Add services to the container.

// builder.Services.AddControllers();


// //Register the repository dependency

// builder.Services.AddScoped<ICommLeaseRepository, CommLeaseRepository>();
// builder.Services.AddScoped<ICommContactRepository, CommContactRepository>();
// builder.Services.AddScoped<ICommCustomerRepository, CommCustomerRepository>();
// builder.Services.AddScoped<ICommLeaseUnitRepository, CommLeaseUnitRepository>();


// // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
// builder.Services.Configure<PowerBISettings>(builder.Configuration.GetSection("PowerBI"));

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi(); // <-- Must be here
//     app.UseSwaggerUI(options =>
//     {
//         options.SwaggerEndpoint("/openapi/v1.json", "FinAxis API v1");
//     });
// }

// app.UseHttpsRedirection();

// app.UseAuthorization();

// app.MapControllers();

// app.Run();


using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using FinAxisLeaseBudgeting.RepositorieS;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FinAxisDbContext>(options =>
    options.UseNpgsql(connectionString));

// 1. ADD THIS LINE: Configure CORS to allow your local environment and Swagger
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

builder.Services.AddOpenApi();
builder.Services.Configure<PowerBISettings>(builder.Configuration.GetSection("PowerBI"));

var app = builder.Build();

// 2. ADD THIS LINE: Enable CORS middleware right after app builds
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