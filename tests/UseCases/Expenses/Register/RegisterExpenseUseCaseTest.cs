using CashFlow.Application.UseCases.Expenses.Register;
using CashFlow.Domain.Entities;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Expenses.Register;

public class RegisterExpenseUseCaseTest
{
  [Fact]
  public async Task Success()
  {
    var loggerUser = UserBuilder.Build();
    var request = RequestExpenseJsonBuilder.Build();
    var useCase = CreateUseCase(loggerUser); 
    
    var result = await useCase.Execute(request);
    
    result.Should().NotBeNull();
    result.Title.Should().Be(request.Title);
  }

  [Fact]
  public async Task Error_Title_Empty()
  {
    var loggerUser = UserBuilder.Build();
    var request = RequestExpenseJsonBuilder.Build();
    
    request.Title = string.Empty;
    var useCase = CreateUseCase(loggerUser); 
    
    var act = async () => await useCase.Execute(request);
    
    var result = await act.Should().ThrowAsync<ErrorOnValidationException>();

    result.Where(ex => ex.GetErrors().Count == 1 && ex.GetErrors().Contains(ResourceErrorMessages.TITLE_REQUIRED));

  }

  private IRegisterExpenseUseCase CreateUseCase(User user)
  {
    var repository = ExpensesWriteOnlyRepositoryBuilder.Build();
    var mapper = MapperBuilder.Build();
    var unitOfWork = UnitOfWorkBuilder.Build();
    var loggerUser = LoggedUserBuilder.Build(user);

    return new RegisterExpenseUseCase(repository, unitOfWork,  mapper,loggerUser);

  }
}
