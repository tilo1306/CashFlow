using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Login.DoLogin;

public class DoLoginUseCase : IDoLoginUseCase
{
  private readonly IUserReadOnlyRepository  _userRepository;
  private readonly IPasswordEncripter  _passwordEncripter;
  private readonly IAccessTokenGenerator _accessTokenGenerator;

  public DoLoginUseCase( 
    IUserReadOnlyRepository  userRepository, 
    IPasswordEncripter passwordEncripter,
    IAccessTokenGenerator accessTokenGenerator)
  {
    _userRepository = userRepository;
    _passwordEncripter = passwordEncripter;
    _accessTokenGenerator = accessTokenGenerator;
  }
  
  public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
  {
    Validate(request);
    
    var user = await _userRepository.GetUserByEmail(request.Email);

    if (user is null)
    {
      throw new InvalidLoginException();
    }

    var passwordMatch = _passwordEncripter.Verify(request.Password, user.Password);
    
    if(passwordMatch == false)
    {
      throw new InvalidLoginException();
    }

    return new ResponseRegisteredUserJson
    {
      Name = user.Name,
      Token = _accessTokenGenerator.Generate(user)
    };
  }


  private void Validate(RequestLoginJson request)
  {
    var result = new LoginValidator().Validate(request);

    if (result.IsValid == false)
    {
      var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
      throw new ErrorOnValidationException(errorMessages);
    }
  }
}
