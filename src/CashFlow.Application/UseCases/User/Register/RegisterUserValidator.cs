using CashFlow.Communication.Requests;
using CashFlow.Exception;
using FluentValidation;

namespace CashFlow.Application.UseCases.User.Register;

public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
{
  public RegisterUserValidator()
  {
    RuleFor(user => user.Name)
      .NotEmpty()
      .WithMessage(ResourceErrorMessages.NAME_EMPTY);

    RuleFor(user => user.Email)
      .NotEmpty()
      .WithMessage(ResourceErrorMessages.EMAIL_EMPTY)
      .EmailAddress()
      .WithMessage(ResourceErrorMessages.EMAIL_INVALID)
      .When(user => !string.IsNullOrWhiteSpace(user.Email), ApplyConditionTo.CurrentValidator);

    RuleFor(user => user.Password)
      .Cascade(CascadeMode.Stop) 
      .NotEmpty()
      .WithMessage(ResourceErrorMessages.INVALID_PASSWORD)
      .SetValidator(new PasswordValidator<RequestRegisterUserJson>()); 
  }
}
