using AutoMapper;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Services.LoggedUser;
using ClosedXML;

namespace CashFlow.Application.UseCases.User.Profile;

public class GetUserProfileUseCase :IGetUserProfileUseCase
{
  private readonly ILoggedUser _loggerUser;
  private readonly IMapper _mapper;
  
  public GetUserProfileUseCase(ILoggedUser loggerUser, IMapper mapper)
  {
    _loggerUser = loggerUser;
    _mapper = mapper;
  }

  public async Task<ResponseUserProfileJson> Execute()
  {
    var user = await _loggerUser.Get();
    return _mapper.Map<ResponseUserProfileJson>(user);
    
  }
}
