using CredipathAPI.Data;
using CredipathAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection") ?? throw new InvalidOperationException("Connection string 'YourDbContext' not found.")));

builder.Services.AddDbContext<DataContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// 🔥 Configurar CORS
var corsPolicyName = "AllowFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName,
        policy =>
        {
            var allowedOrigins = builder.Configuration.GetSection("MyHost:Issuers").Get<string[]>();
            policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
});

builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    }
);


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CredipathAPI", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            new string[] {}
        }
    });
    
    // Configuraciones adicionales para evitar problemas con referencias circulares
    c.CustomSchemaIds(type => type.FullName);
    c.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.RelativePath));
});

builder.Services.AddHttpContextAccessor();


// Configurar JSON para evitar referencias circulares
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

//Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("usertype", "admin"));
});


//Dependencies
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<RouteServices>();
builder.Services.AddScoped<InterestTypeService>();
builder.Services.AddScoped<PaymentFrequencyService>();
builder.Services.AddScoped<PermissionServices>();
builder.Services.AddScoped<LoansService>();
builder.Services.AddScoped<ExcludedDaysService>();
builder.Services.AddScoped<ViewExpectedvsRealityService>();
builder.Services.AddScoped<LoanAmortizationService>();
builder.Services.AddScoped<UserRouteService>();
builder.Services.AddScoped<CollaboratorService>();




var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CredipathAPI v1");
});

app.UseHttpsRedirection();

// 🔥 Habilitar CORS antes de autenticación y autorización
app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Inicializar permisos para la funcionalidad de colaboradores (dentro de un try-catch para evitar errores de arranque)
try
{
    await PermissionsInitializer.InitializePermissionsAsync(app.Services);
}
catch (Exception ex)
{
    // Loguear pero permitir que la aplicación continúe
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error al inicializar permisos, pero la aplicación continuará.");
}

app.Run();
