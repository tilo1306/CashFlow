using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CashFlow.Exception;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Timeouts;
using WebApi.Test.InlineData;

namespace WebApi.Test.Expenses.Register;

public class RegisterExpensesTest: CashFlowClassFixture
{
  private const string METHOD = "api/Expenses";
  private readonly string _token;

  public RegisterExpensesTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
  {
    _token = webApplicationFactory.User_Team_Member.GetToken();
  }

  [Fact]
  public async Task Success()
  {
    var request = RequestExpenseJsonBuilder.Build();
    
    var result = await DoPost(requestUrl:METHOD,requestBody: request, token:_token);

    result.StatusCode.Should().Be(HttpStatusCode.Created);
    
    var body = await result.Content.ReadAsStreamAsync();

    var response = await JsonDocument.ParseAsync(body);
    response.RootElement.GetProperty("title").GetString().Should().Be(request.Title);
  }

  [Theory]
  [ClassData(typeof(CultureInlineDataTest))]
  public async Task Error_Title_Empty(string cultureInfo)
  {
    var request = RequestExpenseJsonBuilder.Build();
    request.Title = string.Empty;
    
    var result = await DoPost(requestUrl:METHOD, requestBody:request, token:_token, culture: cultureInfo);

    result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    
    var body = await result.Content.ReadAsStreamAsync();

    var response = await JsonDocument.ParseAsync(body);
    var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();
    var expectedMessage =
      ResourceErrorMessages.ResourceManager.GetString("TITLE_REQUIRED", new CultureInfo(cultureInfo));
    
    errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));
    
  }

}
