using System.Globalization;

namespace CashFlow.API.Middleware;

public class CultureMiddleware
{
  private readonly RequestDelegate _next;
  private static readonly bool Invariant =
    (Environment.GetEnvironmentVariable("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT") ?? "")
    .Equals("1", StringComparison.OrdinalIgnoreCase) ||
    (Environment.GetEnvironmentVariable("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT") ?? "")
    .Equals("true", StringComparison.OrdinalIgnoreCase);

  public CultureMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task Invoke(HttpContext context)
  {
    CultureInfo culture;

    if (Invariant)
    {
      // Ambiente está em globalization invariant mode → só podemos usar InvariantCulture
      culture = CultureInfo.InvariantCulture;
    }
    else
    {
      // Pega a cultura do header (ex: "pt-BR,en;q=0.9")
      var requested = context.Request.Headers.AcceptLanguage.FirstOrDefault()
        ?.Split(',').FirstOrDefault(); // só o primeiro idioma

      // Fallback
      var cultureName = string.IsNullOrWhiteSpace(requested) ? "en-US" : requested.Trim();

      try
      {
        culture = CultureInfo.GetCultureInfo(cultureName);
      }
      catch (CultureNotFoundException)
      {
        // Se não achar a cultura, cai para Invariant
        culture = CultureInfo.InvariantCulture;
      }
    }

    CultureInfo.CurrentCulture = culture;
    CultureInfo.CurrentUICulture = culture;

    await _next(context);
  }
}
