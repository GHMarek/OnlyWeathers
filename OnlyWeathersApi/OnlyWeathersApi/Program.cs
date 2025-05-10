using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlyWeathersApi.Data;
using OnlyWeathersApi.Models;
using OnlyWeathersApi.Services;
using OnlyWeathersAPI.Services; // Upewnij siê, ¿e potrzebne – mo¿e byæ powielone
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------
// 1. Kontrolery i Swagger
// ---------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Konfiguracja Swaggera z obs³ug¹ JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OnlyWeathers API", Version = "v1" });

    // Definicja schematu bezpieczeñstwa JWT w Swaggerze
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Wpisz: Bearer {twój_token}"
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
});

// ---------------------------------------------
// 2. CORS – zezwolenie na dostêp z frontendu
// ---------------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5255") // frontend lokalny
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ---------------------------------------------
// 3. Baza danych (SQLite)
// ---------------------------------------------

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=onlyweathers.db"));

// ---------------------------------------------
// 4. JWT – konfiguracja uwierzytelniania
// ---------------------------------------------

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var secretKey = jwtSettings!.SecretKey;

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateLifetime = true
    };
});

// ---------------------------------------------
// 5. Rejestracja serwisów i klientów HTTP
// ---------------------------------------------

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();


builder.Services.AddHttpClient<IGeoDbService, GeoDbService>();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddHttpClient<ICountryService, CountryService>();

// ---------------------------------------------
// 6. Budowa i konfiguracja aplikacji
// ---------------------------------------------

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlyWeathers API v1");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
