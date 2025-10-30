// See https://aka.ms/new-console-template for more information

using HelloJab;
using Jab;

var provider = new ServiceProvider();
var service = provider.GetService<MainService>();
await service.Run();

[ServiceProvider]
[Import(typeof(IMainServiceModule))]
[Singleton(typeof(HttpClient), typeof(HttpClient))]
internal partial class ServiceProvider;
