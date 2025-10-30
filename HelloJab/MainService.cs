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

internal class ResponseProcessor(ILogger logger, int times)
{
  public virtual void Process(HttpResponseMessage response)
  {
    response.EnsureSuccessStatusCode();
    foreach (var i in Enumerable.Range(0, times))
    {
      logger.LogInformation("{i}", i);
    }
  }
}

[ServiceProviderModule]
[Singleton(typeof(EndpointResolver))]
[Transient(typeof(ResponseProcessor), Factory = nameof(ResponseProcessorFactory))]
[Transient(typeof(MainService))]
internal interface IMainServiceModule
{
  private static readonly ILogger Logger =
    LoggerFactory
      .Create(builder => builder.AddConsole())
      .CreateLogger("default");

  public static ResponseProcessor ResponseProcessorFactory() => new(Logger, 5);
}
