using Agenda.Application;
using Agenda.Application.Interface;
using Agenda.Infrastructure.Persistence;
using Agenda.Infrastructure.Security;
using Agenda.Web.Endpoints;
using Agenda.Web.Exceptions;
using Agenda.Web.Security;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var conn = builder.Configuration.GetConnectionString("Agenda");
builder.Services.AddDbContext<AgendaDbContext>( options => options.UseNpgsql( conn ) );
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IDisponibilidadeLeitura, DisponibilidadeLeitura>();
builder.Services.AddScoped<ConsultarHorariosLivres>();
builder.Services.AddScoped<IAgendamentoEscrita, AgendamentoEscrita>();
builder.Services.AddScoped<CriarAgendamento>();
builder.Services.AddScoped<ICatalogoLeitura, CatalogoLeitura>();
builder.Services.AddScoped<ListarAgendaDoDia>();
builder.Services.AddScoped<CancelarAgendamento>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// CORS: deixa o front React (outra porta) chamar a API pelo navegador.
// Origens em "Cors:Origens"; sem config, libera o Vite padrão (localhost:5173).
var origens = builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? ["http://localhost:5173"];
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(origens).AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Banco vazio na máquina local? Cria barbeiros, serviços e expedientes de exemplo.
    using var scope = app.Services.CreateScope();
    var senhaDev = app.Configuration["Seed:SenhaDev"] ?? throw new InvalidOperationException("Configure Seed:SenhaDev no user-secrets");

    await SeedDesenvolvimento.Executar(scope.ServiceProvider.GetRequiredService<AgendaDbContext>(), senhaDev);
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseCors();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
app.MapAutenticacao();
app.MapDisponibilidade();
app.MapAgendamentos();
app.MapCatalogo();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
