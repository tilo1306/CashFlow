using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CashFlow.Domain.Entities;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using WebApi.Test.InlineData;

namespace WebApi.Test.Users.Register;

public class RegisterUserTest  : CashFlowClassFixture
{
  private const string METHOD = "api/User";

  public RegisterUserTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
  {
  }
  
  [Fact]
  public async Task Success()
  {
    var request = RequestRegisterUserJsonBuilder.Build();

    var result = await DoPost(METHOD,request);

    result.StatusCode.Should().Be(HttpStatusCode.Created);
    
    var body = await  result.Content.ReadAsStreamAsync();

    var response = await JsonDocument.ParseAsync(body);
    
    response.RootElement.GetProperty("name").GetString().Should().Be(request.Name);
    response.RootElement.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
  }
  
  [Theory]
  [ClassData(typeof(CultureInlineDataTest))]
  public async Task Error_Empty_Name(string cultureInfo)
  {
    var request = RequestRegisterUserJsonBuilder.Build();
    request.Name = string.Empty;
    
    var result = await DoPost(requestUrl:METHOD,requestBody: request,culture: cultureInfo);

    result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    
    var body = await  result.Content.ReadAsStreamAsync();

    var response = await JsonDocument.ParseAsync(body);
    
    var erros = response.RootElement.GetProperty("errorMessages").EnumerateArray();

    var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(cultureInfo));
    erros.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));
  }
}
