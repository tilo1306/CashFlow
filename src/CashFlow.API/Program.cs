using System.Text;
using CashFlow.API.Filters;
using CashFlow.API.Middleware;
using CashFlow.Application;
using CashFlow.Infrastructure;
using CashFlow.Infrastructure.Migrations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
  config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Name = "Authorization",
    Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
    In = ParameterLocation.Header,
    Scheme = "Bearer",
    Type = SecuritySchemeType.ApiKey
  });
  config.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        },
        Scheme = "oauth2",
        Name = "Bearer",
        In = ParameterLocation.Header
      },
      new List<string>()
    }
  });
});

builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var signingKey = builder.Configuration.GetValue<string>("Settings:Jwt:SigningKey");

builder.Services.AddAuthentication(config =>
{
  config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(config =>
{
  config.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = false,
    ValidateAudience = false,
    ClockSkew = new TimeSpan(0),
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey!))
  };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

await MigrateDatabaseAsync();

app.Run();

async Task MigrateDatabaseAsync()
{
  const int maxRetries = 5;
  var delay = TimeSpan.FromSeconds(2);

  for (var attempt = 1; attempt <= maxRetries; attempt++)
  {
    try
    {
      using var scope = app.Services.CreateScope();
      await DataBaseMigration.MigrateDataBase(scope.ServiceProvider);
      app.Logger.LogInformation("✅ Migrações aplicadas com sucesso.");
      return;
    }
    catch (Exception ex)
    {
      app.Logger.LogWarning(ex, "Tentativa {Attempt}/{Max} ao aplicar migrações falhou.", attempt, maxRetries);

      if (attempt == maxRetries)
      {
        app.Logger.LogError(ex, "❌ Não foi possível aplicar migrações após {Max} tentativas.", maxRetries);
        throw; // subir erro para parar a app (melhor do que rodar sem tabelas)
      }

      await Task.Delay(delay);
    }
  }
}
