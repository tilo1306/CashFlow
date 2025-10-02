using CashFlow.Application.UseCases.User;
using CashFlow.Communication.Requests;
using FluentValidation;

namespace CashFlow.Application.UseCases.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<RequestChangePasswordJson>
{
  public ChangePasswordValidator()
  {
    RuleFor(x => x.NewPassword).SetValidator(new PasswordValidator<RequestChangePasswordJson>());
  }
}
