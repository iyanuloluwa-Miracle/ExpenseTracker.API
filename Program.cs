using Microsoft.Extensions.DependencyInjection;
using Server.Configuration;
using Server.Services;
using Server.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Expense Tracker API", Version = "v1" });

    // Add JWT Bearer Security Definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {token}"
    });

    // Add JWT Security Requirement globally
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(options =>
{
    options.ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") 
        ?? throw new InvalidOperationException("MONGODB_CONNECTION_STRING is not set");
    options.DatabaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME") 
        ?? throw new InvalidOperationException("MONGODB_DATABASE_NAME is not set");
});
builder.Services.AddSingleton<IMongoDbService, MongoDbService>();

// Configure Email Service
builder.Services.Configure<EmailSettings>(options =>
{
    options.SmtpServer = Environment.GetEnvironmentVariable("EMAIL_SMTP_SERVER") 
        ?? throw new InvalidOperationException("EMAIL_SMTP_SERVER is not set");
    options.SmtpPort = int.Parse(Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT") 
        ?? throw new InvalidOperationException("EMAIL_SMTP_PORT is not set"));
    options.SmtpUsername = Environment.GetEnvironmentVariable("EMAIL_SMTP_USERNAME") 
        ?? throw new InvalidOperationException("EMAIL_SMTP_USERNAME is not set");
    options.SmtpPassword = Environment.GetEnvironmentVariable("EMAIL_SMTP_PASSWORD") 
        ?? throw new InvalidOperationException("EMAIL_SMTP_PASSWORD is not set");
    options.FromAddress = Environment.GetEnvironmentVariable("EMAIL_FROM_ADDRESS") 
        ?? throw new InvalidOperationException("EMAIL_FROM_ADDRESS is not set");
    options.FromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") 
        ?? throw new InvalidOperationException("EMAIL_FROM_NAME is not set");
    options.EnableSsl = bool.Parse(Environment.GetEnvironmentVariable("EMAIL_ENABLE_SSL") ?? "true");
});
builder.Services.AddTransient<IEmailService, EmailService>();

// Configure Services
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Configure JWT Authentication
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") 
    ?? throw new InvalidOperationException("JWT_KEY is not set");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") 
                ?? throw new InvalidOperationException("JWT_ISSUER is not set"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") 
                ?? throw new InvalidOperationException("JWT_AUDIENCE is not set"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Add JWT configuration to IConfiguration for use in controllers
builder.Configuration["JwtSettings:Key"] = jwtKey;
builder.Configuration["JwtSettings:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER") 
    ?? throw new InvalidOperationException("JWT_ISSUER is not set");
builder.Configuration["JwtSettings:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE") 
    ?? throw new InvalidOperationException("JWT_AUDIENCE is not set");

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Authentication and Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
