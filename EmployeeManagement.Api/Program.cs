using EmployeeManagement.Api.Middlewares;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------
// ✅ 1️⃣ Setup Serilog Logging
// ---------------------------
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console()
          .WriteTo.File("Logs/ems-.log", rollingInterval: RollingInterval.Day)
          .Enrich.FromLogContext();
});

// ---------------------------
// ✅ 2️⃣ Add Services
// ---------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------------------------
// ✅ 3️⃣ Configure InMemory EF Core
// ---------------------------
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseInMemoryDatabase("EmployeeDb"));

// ---------------------------
// ✅ 4️⃣ Register Repository (DI)
// ---------------------------
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

// ---------------------------
// ✅ 5️⃣ Add JWT Authentication (BEFORE Build)
// ---------------------------
var jwtKey = builder.Configuration["Jwt:Key"] ?? "super_secret_key_12345";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "https://localhost";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "https://localhost";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// ---------------------------
// ✅ 6️⃣ Build Application
// ---------------------------
var app = builder.Build();

// ---------------------------
// ✅ 7️⃣ Seed the InMemory DB (optional)
// ---------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
    db.Database.EnsureCreated();
}

// ---------------------------
// ✅ 8️⃣ Configure Middleware Pipeline
// ---------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "Handled {RequestPath} in {Elapsed:0.0000} ms - {StatusCode}";
});

app.MapControllers();

app.Run();
