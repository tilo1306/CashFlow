using CashFlow.Application.UseCases.User.Register;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
      [HttpPost]
      [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status201Created)]
      [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
      public async Task<IActionResult> Register(
        [FromServices] IRegisterUserUseCase useCase,
        [FromBody] RequestRegisterUserJson request)
      {
        var response = await useCase.Execute(request);

        return Created(string.Empty, response);
      }

      [HttpPost]
      [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
      [Authorize]
      public async Task<IActionResult> GetProfile(
        [FromServices] IGetUserProfileUseCase useCase
        )
      {
        var response = await useCase.Execute();
        return Ok(response);
      }
    }
}
