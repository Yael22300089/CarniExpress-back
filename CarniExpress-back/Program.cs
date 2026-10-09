
using CarniExpress_back.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// CONTROLADORES
// =====================================

builder.Services.AddControllers();

// =====================================
// SWAGGER
// =====================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// =====================================
// SERVICIOS EN MEMORIA
// =====================================

builder.Services.AddSingleton<UsuarioService>();

// Servicio para revocar tokens
builder.Services.AddSingleton<TokenRevocationService>();

// =====================================
// CONFIGURACIÓN JWT
// =====================================

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Debes configurar Jwt:Key");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "La clave JWT debe tener al menos 32 bytes");
}

// =====================================
// AUTENTICACIÓN JWT
// =====================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.Zero
            };

        // =====================================
        // VALIDAR TOKENS REVOCADOS
        // =====================================

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var tokenService = context.HttpContext
                    .RequestServices
                    .GetRequiredService<TokenRevocationService>();

                var tokenId = context.Principal?
                    .FindFirst("jti")?.Value;

                if (string.IsNullOrWhiteSpace(tokenId))
                {
                    context.Fail(
                        "El token no contiene un identificador válido");
                    return Task.CompletedTask;
                }

                if (tokenService.EstaRevocado(tokenId))
                {
                    context.Fail(
                        "La sesión ha sido cerrada");
                }

                return Task.CompletedTask;
            }
        };
    });

// =====================================
// AUTORIZACIÓN
// =====================================

builder.Services.AddAuthorization();

// =====================================
// CONSTRUIR APLICACIÓN
// =====================================

var app = builder.Build();

// =====================================
// SWAGGER EN DESARROLLO
// =====================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =====================================
// MIDDLEWARE
// =====================================

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// =====================================
// CONTROLADORES
// =====================================

app.MapControllers();

app.Run();
