using System.Text.RegularExpressions;
using CashFlow.Exception;
using FluentValidation;
using FluentValidation.Validators;

namespace CashFlow.Application.UseCases.User;

public class PasswordValidator<T> : PropertyValidator<T, string >
{
  private const string ERROR_MESSAGE_KEY = "ErrorMessage";
  public override string Name => "PasswordValidator";

  protected override string GetDefaultMessageTemplate(string errorCode)
  {
    return $"{{{ERROR_MESSAGE_KEY}}}";
  }

  public override bool IsValid(ValidationContext<T> context, string password)
  {
    if (string.IsNullOrWhiteSpace(password))
    {
      context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);
      return false;
    }

    var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$");

    if (!regex.IsMatch(password))
    {
      context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY,
        "Your password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.");
      return false;
    }

    return true;
  }

}
