﻿using AutoMapper;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;

namespace CashFlow.Application.UseCases.Expenses.GetAll;

public class GetAllExpenseUseCase : IGetAllExpenseUseCase
{
    private readonly IExpensesReadonlyRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;


    public GetAllExpenseUseCase(IExpensesReadonlyRepository repository,
      IMapper mapper,
      ILoggedUser loggedUser)
    {
        _repository = repository;
        _mapper = mapper;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseExpensesJson> Execute()
    {
        var loggedUser = await _loggedUser.Get();
      
        var result = await _repository.GetAll(loggedUser);
        return new ResponseExpensesJson
        {
            Expenses = _mapper.Map<List<ResponseShortExpenseJson>>(result)
        };
    }
}
