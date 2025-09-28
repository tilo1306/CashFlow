using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.Delete;

public class DeleteExpenseUseCase : IDeleteExpenseUseCase
{
    private readonly IExpensesWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;
    private readonly IExpensesReadonlyRepository _expensesReadonlyRepository;

    public DeleteExpenseUseCase(
      IExpensesWriteOnlyRepository repository, 
      IUnitOfWork unitOfWork,
      ILoggedUser loggedUser,
      IExpensesReadonlyRepository expensesReadonlyRepository)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
        _expensesReadonlyRepository = expensesReadonlyRepository;
    }

    public async Task Execute(long id)
    {
        var loggerUser = await _loggedUser.Get();

        var expense = await _expensesReadonlyRepository.GetById(loggerUser, id);
      
        if (expense is null)
        {
            throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);
        }
        
        await _repository.Delete(id);


        await _unitOfWork.Commit();
    }
}
