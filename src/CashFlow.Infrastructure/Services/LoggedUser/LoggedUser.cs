using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Services.LoggedUser;

public class LoggedUser : ILoggedUser
{
  private readonly CashFlowDbContext _dbContext;

  public LoggedUser(CashFlowDbContext dbContext) =>  _dbContext = dbContext; 
  
  public async Task<User> Get()
  {

    string token = "";

    var tokenHandler = new JwtSecurityTokenHandler();
    
    var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
    
    var identifier = jwtSecurityToken.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
    
    return await _dbContext.Users.AsNoTracking().FirstAsync(user => user.UserIdentifier == Guid.Parse(identifier));
    
  }
}
