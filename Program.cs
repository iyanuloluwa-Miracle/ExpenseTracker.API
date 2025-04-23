using Server.Configuration;
using Server.Services;
using Server.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(options =>
{
    options.ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ?? throw new InvalidOperationException("MONGODB_CONNECTION_STRING is not set");
    options.DatabaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME") ?? throw new InvalidOperationException("MONGODB_DATABASE_NAME is not set");
});
builder.Services.AddSingleton<IMongoDbService, MongoDbService>();

// Configure Email Service
builder.Services.Configure<EmailSettings>(options =>
{
    options.SmtpServer = Environment.GetEnvironmentVariable("EMAIL_SMTP_SERVER") ?? throw new InvalidOperationException("EMAIL_SMTP_SERVER is not set");
    options.SmtpPort = int.Parse(Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT") ?? throw new InvalidOperationException("EMAIL_SMTP_PORT is not set"));
    options.SmtpUsername = Environment.GetEnvironmentVariable("EMAIL_SMTP_USERNAME") ?? throw new InvalidOperationException("EMAIL_SMTP_USERNAME is not set");
    options.SmtpPassword = Environment.GetEnvironmentVariable("EMAIL_SMTP_PASSWORD") ?? throw new InvalidOperationException("EMAIL_SMTP_PASSWORD is not set");
    options.FromAddress = Environment.GetEnvironmentVariable("EMAIL_FROM_ADDRESS") ?? throw new InvalidOperationException("EMAIL_FROM_ADDRESS is not set");
    options.FromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") ?? throw new InvalidOperationException("EMAIL_FROM_NAME is not set");
    options.EnableSsl = bool.Parse(Environment.GetEnvironmentVariable("EMAIL_ENABLE_SSL") ?? "true");
});
builder.Services.AddTransient<IEmailService, EmailService>();

// Configure JWT Authentication
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ??
    throw new InvalidOperationException("JWT_KEY is not set");

// Ensure compatible versions of System.IdentityModel.Tokens.Jwt and Microsoft.IdentityModel.Tokens are installed.
// Update NuGet packages if necessary to resolve version conflicts.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                throw new InvalidOperationException("JWT_ISSUER is not set"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
                throw new InvalidOperationException("JWT_AUDIENCE is not set"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Add JWT configuration to IConfiguration for use in controllers
builder.Configuration["JwtSettings:Key"] = jwtKey;
builder.Configuration["JwtSettings:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
    throw new InvalidOperationException("JWT_ISSUER is not set");
builder.Configuration["JwtSettings:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
    throw new InvalidOperationException("JWT_AUDIENCE is not set");




builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});




// Add Authentication and Authorization services
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

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();