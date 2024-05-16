using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.Expenses.GetId;

public interface IGetIdExpenseUseCase
{
    Task<ResponseExpenseJson> Execute(long id);
}