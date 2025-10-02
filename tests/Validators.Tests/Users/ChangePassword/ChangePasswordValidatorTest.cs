using CashFlow.Application.UseCases.ChangePassword;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using Xunit;

namespace Validators.Tests.Users.Register.ChangePassword;

public class ChangePasswordValidatorTest
{
  [Fact]
  public void Success()
  {
    var validator = new ChangePasswordValidator();

    var request = RequestChangePasswordJsonBuilder.Build();

    var result = validator.Validate(request);

    result.IsValid.Should().BeTrue();
  }

  [Theory]
  [InlineData("")]
  [InlineData("    ")]
  [InlineData(null)]
  public void Error_NewPassword_Empty(string newPassword)
  {
    var validator = new ChangePasswordValidator();

    var request = RequestChangePasswordJsonBuilder.Build();
    request.NewPassword = newPassword;

    var result = validator.Validate(request);

    result.IsValid.Should().BeFalse();
  }
}
