using AGRONET.Auth.Infrastructure;
using AGRONET.Bienes.Infrastructure;
using AGRONET.FichaSalida.Infrastructure;
using AGRONET.Marcacion.Infrastructure;
using AGRONET.Menus.Infrastructure;
using AGRONET.Roles.Infrastructure;
using AGRONET.Users.Infrastructure;
using AGRONET.Catalogos.Infrastructure;
using AGRONET.Boletas.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AGRONET API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

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

    c.OperationFilter<AGRONET.Api.Swagger.AllowAnonymousOperationFilter>();
});

builder.Services.AddAgronetAuth(builder.Configuration);
builder.Services.AddAgronetFichaSalida(builder.Configuration);
builder.Services.AddAgronetMarcacion(builder.Configuration);
builder.Services.AddMenusModule();
builder.Services.AddRolesModule();
builder.Services.AddUsersModule();
builder.Services.AddCatalogosModule();
builder.Services.AddBoletasModule();
builder.Services.AddBienesModule();

var jwt = builder.Configuration.GetSection("Jwt");
var key = jwt["SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey no configurado.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.MapInboundClaims = false;

        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AGRONET API v1");
        c.EnablePersistAuthorization();
    });
}

app.UseHttpsRedirection();

app.UseCors("DevCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();