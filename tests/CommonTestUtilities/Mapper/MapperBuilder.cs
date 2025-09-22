using AutoMapper;
using CashFlow.Application.AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CommonTestUtilities.Mapper;

public abstract class MapperBuilder
{
  public static IMapper Build()
  {
    var expression = new MapperConfigurationExpression();
    expression.AddProfile<AutoMapping>();

    var loggerFactory = NullLoggerFactory.Instance;

    var mapperConfig = new MapperConfiguration(expression, loggerFactory);
    return mapperConfig.CreateMapper();
  }
}
