using DapperStudy.Application;
using DapperStudy.Infrastructure.UnitOfWork;
using DapperStudy.Migrations.Runner;
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
    .AddTransient<IAviaryService, AviaryService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultControllerRoute();

// app.UseHttpsRedirection();
// app.UseEndpoints(endpoinds => endpoinds.MapControllers());
MigrationsRunner.RunMigrations(builder.Configuration);

app.Run();