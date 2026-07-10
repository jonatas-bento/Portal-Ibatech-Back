// Ibatech.API/Program.cs
using System.Text;
using Ibatech.API.Middlewares;
using Ibatech.Domain.Enums;
using Ibatech.Infra.Context;
using Ibatech.Repository.UnitOfWork;
using Ibatech.Services.Implementations;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Repository.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


using Microsoft.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Ibatech.API;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ── 1. EF Core + MySQL ────────────────────────────────────────────────────────
builder.Services.AddDbContext<IbatechDbContext>(opts =>
    opts.UseMySql(
        config.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(config.GetConnectionString("DefaultConnection")),
        mySqlOpts => mySqlOpts.EnableRetryOnFailure(3)
    ));

// ── 2. JWT Authentication ─────────────────────────────────────────────────────
var jwtKey = config["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret não configurado.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// ── 3. Autorização por Role ───────────────────────────────────────────────────
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("ApenasAdmin",
        p => p.RequireRole(nameof(RoleUsuario.Admin)));
    opts.AddPolicy("AdminOuFuncionario",
        p => p.RequireRole(
            nameof(RoleUsuario.Admin),
            nameof(RoleUsuario.Funcionario)));
    opts.AddPolicy("Autenticado",
        p => p.RequireAuthenticatedUser());
});

// ── 4. CORS ───────────────────────────────────────────────────────────────────
builder.Services.AddCors(opts =>
    opts.AddPolicy("AngularClient", policy =>
        policy
            .WithOrigins(
                config["Cors:AllowedOrigin"] ?? "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

builder.Services.AddScoped<Ibatech.Services.Security.JwtTokenGenerator>();

// ── 5. Injeção de Dependência — Repositórios ─────────────────────────────────
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IEstoqueRepository, EstoqueRepository>();
builder.Services.AddScoped<IFinanceiroRepository, FinanceiroRepository>();

// ── 6. Injeção de Dependência — Serviços ─────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProjetoService, ProjetoService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IFinanceiroService, FinanceiroService>();

// ── 7. Controllers + JSON ─────────────────────────────────────────────────────
builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ── 8. OpenAPI Nativo .NET 10 ────────────────────────────────────────────────
builder.Services.AddOpenApi();

// ── 9. Health Checks ──────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddDbContextCheck<IbatechDbContext>("mysql");

var app = builder.Build();

// ── 10. Pipeline HTTP ─────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AngularClient");

// Middleware global de exceções (implementado em Middlewares/)
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// ── 11. Auto-Migration em dev ─────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<IbatechDbContext>();
    await db.Database.MigrateAsync();
}

await DataSeeder.SeedAsync(app.Services); // ← adicionar esta linha

await app.RunAsync();