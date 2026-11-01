using Microsoft.EntityFrameworkCore;
using RendicionesRegistroCivil.Data;
using RendicionesRegistroCivil.Interfaces;
using RendicionesRegistroCivil.Servicios;
using RendicionesRegistroCivil.Utils.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//el DbContext indicando que usaremos PostgreSQL (Npgsql)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITramite, TramiteService>();
builder.Services.AddScoped<IRendicion, RendicionService>();


builder.Services
    .AddOptions<StorageOptions>()
    .Bind(builder.Configuration.GetSection(StorageOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.RootPath),
        "Storage:RootPath no está configurado.")
    .ValidateOnStart();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
