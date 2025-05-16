using Microsoft.EntityFrameworkCore;
using api_econsulta.Data;
using api_econsulta.Configurations;
using api_econsulta.Services;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;
using api_econsulta.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(options =>
{
    // Cria o documento da API
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "eConsulta API",
        Version = "v1",
        Description = "API para o sistema eConsulta",
        Contact = new OpenApiContact
        {
            Name = "Equipe de Desenvolvimento",
            Email = "dev@econsulta.com"
        }
    });
    
    // Define o esquema de segurança para JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    
    // Adiciona o requisito de segurança global (todas as operações serão protegidas)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    
    // Opcional: Adiciona suporte para incluir comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});


// Configuração do Serilog para logging estruturado
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Adicionar serviços ao container
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Configuração do OpenAPI/Swagger
builder.Services.AddOpenApi();

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder
                .WithOrigins(
                    // Adicione aqui os domínios permitidos
                    "http://localhost:5173/",
                    "https://www.localhost:5173/")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    
    // Política de desenvolvimento - remover em produção
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("Development",
            builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    }
});

// Configuração do JWT
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// Configuração do Entity Framework Core com PostgreSQL
builder.Services.AddDbContext<EconsultaDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
            npgsqlOptions.CommandTimeout(30);
        });
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Cache distribuído
builder.Services.AddDistributedMemoryCache();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings section is missing or invalid.");

if (string.IsNullOrWhiteSpace(jwtSettings.Key) || jwtSettings.Key.Length < 32)
    throw new InvalidOperationException("JwtSettings.Key is missing or too short (min 32 characters).");

var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

// Configuração da autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = !string.IsNullOrEmpty(jwtSettings.Issuer),
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = !string.IsNullOrEmpty(jwtSettings.Audience),
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Remove a tolerância padrão de 5 minutos
    };
    
    // Tratamento de eventos JWT
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

// Políticas de autorização
builder.Services.AddAuthorizationBuilder()
                               // Políticas de autorização
                               .AddPolicy("DoctorPolicy", policy =>
        policy.RequireClaim(ClaimTypes.Role, "medico"))
                               // Políticas de autorização
                               .AddPolicy("PatientPolicy", policy =>
        policy.RequireClaim(ClaimTypes.Role, "paciente"))
                               // Políticas de autorização
                               .AddPolicy("AdminPolicy", policy =>
        policy.RequireClaim(ClaimTypes.Role, "admin"))
                               // Políticas de autorização
                               .AddPolicy("DoctorOrAdminPolicy", policy =>
        policy.RequireAssertion(context => 
            context.User.HasClaim(c => 
                c.Type == ClaimTypes.Role && (c.Value == "medico" || c.Value == "admin"))));

// Health check
builder.Services.AddHealthChecks()
    .AddDbContextCheck<EconsultaDbContext>();

// Registrar serviços na DI
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AvailabilityService>();
builder.Services.AddScoped<AppointmentService>();

// Rate limiting
builder.Services.AddRateLimiter(options => {
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "localhost",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
            
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

// Configure o HTTP request pipeline
if (app.Environment.IsDevelopment())
{
      app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "eConsulta API v1");
        c.RoutePrefix = string.Empty; // Coloca o Swagger UI na raiz
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.EnableDeepLinking();
        c.DisplayRequestDuration();
    });
    app.UseCors("Development");
}
else
{
    // Middleware personalizado para tratamento global de exceções
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    app.UseCors("AllowSpecificOrigins");
    
    // Configurações de segurança para produção
    app.UseHsts();
}

// Configurações de forwarded headers (para uso com proxy reverso)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseRateLimiter();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

try
{
    Log.Information("Iniciando a API eConsulta");
    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}