using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebApi.Test;

public class CashFlowClassFixture : IClassFixture<CustomWebApplicationFactory>
{
  private  readonly HttpClient _httpClient;

  public CashFlowClassFixture(CustomWebApplicationFactory webApplicationFactory)
  {
    _httpClient = webApplicationFactory.CreateClient();
  }

  protected async Task<HttpResponseMessage> DoPost(
    string requestUrl, 
    object requestBody,
    string token = "",
    string culture = "en"
    )
  {
    ChangeRequestCulture(culture);
    AuthorizeRequest(token);
    return await _httpClient.PostAsJsonAsync(requestUrl, requestBody);

  }

  private void AuthorizeRequest(string token)
  {

    if (string.IsNullOrWhiteSpace(token))
    {
      return;
    }
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

  }
  
  private void ChangeRequestCulture(string cultureInfo)
  {
    _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
    
    _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(cultureInfo));
  }
}
