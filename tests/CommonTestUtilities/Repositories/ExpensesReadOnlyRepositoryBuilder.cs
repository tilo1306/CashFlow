using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Expenses;
using Moq;

namespace CommonTestUtilities.Repositories;

public class ExpensesReadOnlyRepositoryBuilder
{
  private readonly Mock<IExpensesReadonlyRepository> _repository;

  public ExpensesReadOnlyRepositoryBuilder()
  {
    _repository = new Mock<IExpensesReadonlyRepository>();
  }

  public ExpensesReadOnlyRepositoryBuilder GetAll(User user, List<Expense> expenses)
  {
    _repository.Setup(repository => repository.GetAll(user)).ReturnsAsync(expenses);

    return this;
  }
  
  public IExpensesReadonlyRepository Build() => _repository.Object;
}
