using Bogus;
using CashFlow.Domain.Entities;
using CommonTestUtilities.Cryptography;

namespace CommonTestUtilities.Entities;

public class UserBuilder
{
  public static User Build()
  {
    var passwordEncripter = new PasswordEncrypterBuilder().Build();
    var validPassword = "!Aa1Test123"; // Senha válida que atende aos critérios

    var user = new Faker<User>()
      .RuleFor(u => u.Id, _ => 1)
      .RuleFor(u => u.Name, faker => faker.Person.FirstName)
      .RuleFor(u => u.Email, (faker, user) => faker.Internet.Email(user.Name))
      .RuleFor(u => u.Password, _ => passwordEncripter.Encrypt(validPassword))
      .RuleFor(u => u.UserIdentifier, _ => Guid.NewGuid());

    return user;
  }
}
