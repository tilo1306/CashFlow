using CashFlow.Application.UseCases.Expenses.Delete;
using CashFlow.Domain.Entities;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Expenses.Delete;

public class DeleteExpenseUseCaseTest
{

  [Fact]
  public async Task Success()
  {
    var loggedUSer = UserBuilder.Build();
    var expense = ExpenseBuilder.Build(loggedUSer);
    
    var useCase = CreateUseCase(loggedUSer, expense);

    var act = async () => await useCase.Execute(expense.Id);
    await act.Should().NotThrowAsync();
  }

  [Fact]
  public async Task Error_Expense_Not_Found()
  {
    var loggedUSer = UserBuilder.Build();
    
    var useCase = CreateUseCase(loggedUSer);

    var act = async () => await useCase.Execute(id:999);

    var result = await act.Should().ThrowAsync<NotFoundException>();
    result.Where(ex => ex.GetErrors().Count() == 1 && ex.GetErrors().Contains(ResourceErrorMessages.EXPENSE_NOT_FOUND));
  }

  private DeleteExpenseUseCase CreateUseCase(User user, Expense? expense = null)
  {
    var repositoruWriteOnly = ExpensesWriteOnlyRepositoryBuilder.Build();
    
    var repository = new ExpensesReadOnlyRepositoryBuilder().GetById(user, expense).Build();
    var unitOfWork = UnitOfWorkBuilder.Build();
    var loggedUser = LoggedUserBuilder.Build(user);
    
    return new DeleteExpenseUseCase(repositoruWriteOnly, unitOfWork, loggedUser, repository );
  }
}
