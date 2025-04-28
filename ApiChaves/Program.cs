using Microsoft.IdentityModel.Tokens;
using ApiChaves.Application.Services;
using ApiChaves.Infrastructure.Data.Context;
using ApiChaves.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ApiChaves.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<APIDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") // Obt�m a string de conex�o do appsettings.json
    )
);

builder.Services.AddDistributedMemoryCache();

// Configura��o de CORS para permitir apenas requisi��es do front-end do painel
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder
            .WithOrigins("http://127.0.0.1:5501") // Origem do front-end do painel
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// Configura��o da sess�o para autentica��o de usu�rios (admin)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Define o tempo de expira��o da sess�o
    options.Cookie.HttpOnly = true; // Impede o acesso ao cookie por JavaScript
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Garante que o cookie seja transmitido apenas por HTTPS
    options.Cookie.SameSite = SameSiteMode.None; // Permite o envio de cookies em requisi��es cross-origin
    options.Cookie.IsEssential = true; // Assegura que o cookie � considerado essencial
});

// Configura��o da autentica��o com cookies para usu�rios
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tempo de expira��o do cookie
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false; // Defina como true em produ��o
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtSettings>>().Value);
// Registro dos servi�os e reposit�rios
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ChaveRepository>();
builder.Services.AddScoped<ChaveService>();
builder.Services.AddScoped<ApplicacaoRepository>();
builder.Services.AddScoped<ApplicacaoService>();

var app = builder.Build();

// Configura��o do pipeline de middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost");

app.UseSession();
app.UseAuthentication(); // Adiciona a autentica��o para gerenciar JWT e sess�es
app.UseHttpsRedirection();
app.UseAuthorization();

// Mapeamento dos controladores da API
app.MapControllers();

app.Run();
