using ImageShrinkerWebJob.Utils.Extensions;
using Microsoft.Extensions.Hosting;

IHostBuilder builder = new HostBuilder();
builder.ConfigureServices();

using var app = builder.Build();
app.Configure();
app.Run();