using AGRONET.Auth.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AGRONET.FichaSalida.Infrastructure;
using AGRONET.Marcacion.Infrastructure;


using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
 

// Swagger + botón Authorize (sin Reference, compatible)
builder.Services.AddSwaggerGen(c =>
{
    const string schemeId = "bearer";

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AGRONET API",
        Version = "v1"
    });

    c.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Ingrese: Bearer {token}"
    });

    //  IMPORTANTE en .NET 10: usar (schemeId, document)
    c.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(schemeId, document)] = []
        });

    // Este filtro debe QUITAR security cuando hay [AllowAnonymous]
    c.OperationFilter<AGRONET.Api.Swagger.AllowAnonymousOperationFilter>();
});




//  registra módulo Auth (Infrastructure + Application)
builder.Services.AddAgronetAuth(builder.Configuration);
builder.Services.AddAgronetFichaSalida(builder.Configuration);
builder.Services.AddAgronetMarcacion(builder.Configuration);
//builder.Services.AddAgronetFichaSalida(builder.Configuration);



//  JWT Bearer
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

        opt.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine("JWT Auth Failed: " + ctx.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                Console.WriteLine("JWT Challenge: " + ctx.Error + " - " + ctx.ErrorDescription);
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors",
        policy =>
        {
            policy
                .AllowAnyOrigin()
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

app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.Value?.Contains("/api/Auth/logout", StringComparison.OrdinalIgnoreCase) == true)
    {
        Console.WriteLine(">>> REQUEST to logout received. Auth header: " + ctx.Request.Headers.Authorization.ToString());
    }
    await next();
});
app.UseCors("DevCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
