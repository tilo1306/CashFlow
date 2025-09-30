using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Infrastructure.DataAccess;
using CommonTestUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{

  private CashFlow.Domain.Entities.User _user = null!;
  private string _password = null!;
  private string _token = string.Empty;
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Test")
      .ConfigureServices(services =>
      {
        var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
        services.AddDbContext<CashFlowDbContext>(config =>
        {
          config.UseInMemoryDatabase("InMemoryDbForTesting");
          config.UseInternalServiceProvider(provider);
        });
        
        var scope = services.BuildServiceProvider().CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<CashFlowDbContext>();
        var passwordEncripter = scope.ServiceProvider.GetRequiredService<IPasswordEncripter>();
        var tokenGenerator = scope.ServiceProvider.GetRequiredService<IAccessTokenGenerator>();

        StartDatabase(dbContext, passwordEncripter);

        _token = tokenGenerator.Generate(_user);
      });
  }

  public string GetEmail() => _user.Email; 
  public string GetName() => _user.Name;
  public string GetPassword() => _password;
  public string GetToken() => _token;
  
  private void StartDatabase(CashFlowDbContext dbContext, IPasswordEncripter passwordEncripter)
  {
    var validPassword = "!Aa1Test123"; // Mesma senha usada no UserBuilder
    _user = UserBuilder.Build();
    _password = validPassword; // Armazenar a senha original para o teste
    _user.Password = passwordEncripter.Encrypt(validPassword);
    
    dbContext.Users.Add(_user);
    dbContext.SaveChanges();
  }
}
