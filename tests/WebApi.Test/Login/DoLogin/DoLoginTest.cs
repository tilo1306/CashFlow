using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CashFlow.Communication.Requests;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;
using WebApi.Test.InlineData;

namespace WebApi.Test.Login.DoLogin;

public class DoLoginTest : CashFlowClassFixture
{
  private const string METHOD = "api/login";
  private readonly string _email;
  private readonly string _password;

  public DoLoginTest( CustomWebApplicationFactory webApplicationFactory ) : base( webApplicationFactory )
  {
    _email = webApplicationFactory.GetEmail();
    _password = webApplicationFactory.GetPassword();
    
  }

  [Fact]
  public async Task Success()
  {
    var request = new RequestLoginJson()
    {
      Email = _email,
      Password = _password,
    };
    
    var response = await DoPost(requestUrl:METHOD, requestBody:request);

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    
    var responseBody = await response.Content.ReadAsStringAsync();
    
    var responseData = JsonDocument.Parse(responseBody);
    
    responseData.RootElement.GetProperty("name").GetString().Should().NotBeNullOrWhiteSpace();
    responseData.RootElement.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
  }

  [Theory]
  [ClassData(typeof(CultureInlineDataTest))]
  public async Task Error_Login_Invalid(string culture)
  {
    var request = RequestLoginJsonBuilder.Build();
    
    
    var response = await DoPost(requestUrl:METHOD, requestBody: request, culture: culture);
    
    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    
    var responseBody = await response.Content.ReadAsStringAsync();
    
    var responseData = JsonDocument.Parse(responseBody);
    var erros = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();
    
    var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(culture));
    
    erros.Should().HaveCount(1).And.Contain(c=> c.GetString()!.Equals(expectedMessage));

  }
}
