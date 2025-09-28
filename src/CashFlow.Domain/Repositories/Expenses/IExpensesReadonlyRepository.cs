using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses;

public interface IExpensesReadonlyRepository
{
    Task<List<Expense>> GetAll(Entities.User user);

    Task<Expense?> GetById(Entities.User user, long id);

    Task<List<Expense>> FilterByMonth(DateOnly date);
}
