using System.Runtime.CompilerServices;
using Jab;
using Microsoft.Extensions.Logging;

namespace HelloJab;

internal class MainService(
  HttpClient httpClient,
  EndpointResolver endpointResolver,
  ResponseProcessor processor)
{
  public async Task Run()
  {
    var request = new HttpRequestMessage(HttpMethod.Get, endpointResolver.Endpoint);
    var response = await httpClient.SendAsync(request);
    processor.Process(response);
  }
}

internal class EndpointResolver
{
  public virtual string Endpoint => "https://example.com/";
}

internal class ResponseProcessor(ILogger<ResponseProcessor> logger)
{
  public virtual void Process(HttpResponseMessage response)
  {
    response.EnsureSuccessStatusCode();
    foreach (var i in Enumerable.Range(0, 5))
    {
      logger.LogInformation("{i}", i);
    }
  }
}

[ServiceProviderModule]
[Singleton(typeof(EndpointResolver))]
[Transient(typeof(ResponseProcessor))]
[Transient(typeof(MainService))]
[Transient(typeof(ILogger<>), Factory = nameof(CreateLogger))]
internal interface IMainServiceModule
{
  internal static ILogger<T> CreateLogger<T>() => MyLoggerFactory.CreateLogger<T>();

  private static readonly ILoggerFactory MyLoggerFactory =
    LoggerFactory.Create(builder => builder.AddConsole());
}
