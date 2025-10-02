using CashFlow.Communication.Requests;

namespace CashFlow.Application.UseCases.ChangePassword;

public interface IChangePasswordUseCase
{
  Task Execute(RequestChangePasswordJson request);
}
