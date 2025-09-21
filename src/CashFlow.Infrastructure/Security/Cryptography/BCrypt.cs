using CashFlow.Domain.Security.Cryptography;
using BCryptNet = BCrypt.Net.BCrypt;

namespace CashFlow.Infrastructure.Security;

public class BCrypt : IPasswordEncripter
{
  public string Encrypt(string password)
  {
    var passwordHash = BCryptNet.HashPassword(password);
    return passwordHash;
  }

  public bool Verify(string password, string passwordHash)
  {
    return BCryptNet.Verify(password, passwordHash);
    
  }
}
