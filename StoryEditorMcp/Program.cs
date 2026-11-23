using StoryEditorMcp;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole().AddSimpleConsole(x => x.TimestampFormat = "[HH:mm:ss] ");

IServiceCollection? services = builder.Services;

string path = builder.Configuration["storyPath"] ?? throw new Exception("Need the path for the story.");
services.AddSingleton(x => new StoryTools(path));

services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithTools<RandomNumberTools>()
    .WithTools<McpStoryTools>();

WebApplication? app = builder.Build();
app.MapMcp();
app.Run();
