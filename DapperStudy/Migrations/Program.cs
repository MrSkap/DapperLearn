using System.Text;
using DapperStudy.Application;
using DapperStudy.Application.Auth;
using DapperStudy.Configuration;
using DapperStudy.Infrastructure.Auth;
using DapperStudy.Infrastructure.Auth.Migrator;
using DapperStudy.Infrastructure.Auth.Migrator.AuthSetup;
using DapperStudy.Infrastructure.UnitOfWork;
using DapperStudy.Migrations.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Services
    .AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>(
        provider =>
            new UnitOfWorkFactory(provider.GetService<IConfiguration>()!.GetConnectionString("ServiceConnection")!))
    .AddTransient<IAnimalService, AnimalService>()
    .AddTransient<IAviaryService, AviaryService>()
    .AddTransient<IStatisticService, StatisticService>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<IJwtService, JwtService>()
    .AddScoped<IDbMigrator, EfDbMigrator>()
    .AddScoped<IAuthSetuper, AuthSetuper>();

var authConnectionString = builder.Configuration.GetConnectionString("AuthConnection");

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(
        authConnectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                5,
                TimeSpan.FromSeconds(30),
                null);
            npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }
    ));

builder.Services.AddTransient<JwtConfiguration>(provider =>
    builder.Configuration.GetSection(JwtConfiguration.SectionName).Get<JwtConfiguration>()!);
builder.Services.AddTransient<DefaultAdminConfiguration>(provider =>
    builder.Configuration.GetSection(DefaultAdminConfiguration.SectionName).Get<DefaultAdminConfiguration>()!);

// JWT аутентификация
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:ValidateIssuer"],
            ValidAudience = builder.Configuration["Jwt:ValidateAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]))
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

// EF миграция базы данных
using (var scope = app.Services.CreateScope())
{
    var migrator = scope.ServiceProvider.GetRequiredService<IDbMigrator>();

    try
    {
        await migrator.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during migration");
        throw;
    }

    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    var setuper = scope.ServiceProvider.GetRequiredService<IAuthSetuper>();

    await setuper.SeedDataAsync(context);
}

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultControllerRoute();

// Dapper миграция базы данных
MigrationsRunner.RunMigrations(builder.Configuration);

app.Run();