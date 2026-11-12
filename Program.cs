using CarniExpress.Auth.Data;
using CarniExpress.Auth.Models;
using CarniExpress.Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();builder.Services.AddSwaggerGen();
var connection=builder.Configuration.GetConnectionString("CarniExpress") ?? throw new InvalidOperationException("Configura ConnectionStrings:CarniExpress con user-secrets o variables de entorno");
builder.Services.AddDbContext<CarniDbContext>(o=>o.UseSqlServer(connection));
builder.Services.AddScoped<IPasswordHasher<Usuario>,PasswordHasher<Usuario>>();builder.Services.AddScoped<JwtService>();
var key=builder.Configuration["Jwt:Key"]??throw new InvalidOperationException("Configura Jwt:Key con user-secrets o variables de entorno");
if(Encoding.UTF8.GetByteCount(key)<32)throw new InvalidOperationException("Jwt:Key requiere al menos 32 bytes");
builder.Services.AddCors(o=>o.AddPolicy("React",p=>p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o=>{
 o.TokenValidationParameters=new TokenValidationParameters{ValidateIssuer=true,ValidIssuer=builder.Configuration["Jwt:Issuer"],ValidateAudience=true,ValidAudience=builder.Configuration["Jwt:Audience"],ValidateLifetime=true,ValidateIssuerSigningKey=true,IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),ClockSkew=TimeSpan.Zero,NameClaimType=System.Security.Claims.ClaimTypes.NameIdentifier,RoleClaimType=System.Security.Claims.ClaimTypes.Role};
 o.Events=new JwtBearerEvents{OnTokenValidated=async ctx=>{
  var jti=ctx.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value ?? ctx.Principal?.FindFirst("jti")?.Value;
  var userId=ctx.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
  if(string.IsNullOrWhiteSpace(jti)||!int.TryParse(userId,out var id)){ctx.Fail("Token inválido");return;}
  var db=ctx.HttpContext.RequestServices.GetRequiredService<CarniDbContext>();
  if(await db.TokensRevocados.AnyAsync(t=>t.Jti==jti)||!await db.Usuarios.AnyAsync(u=>u.Id==id&&u.Activo)){ctx.Fail("Sesión no válida");}
 }};
});
builder.Services.AddAuthorization();
var app=builder.Build();
if(app.Environment.IsDevelopment()){app.UseSwagger();app.UseSwaggerUI();}
app.UseHttpsRedirection();app.UseCors("React");app.UseAuthentication();app.UseAuthorization();app.MapControllers();app.Run();
