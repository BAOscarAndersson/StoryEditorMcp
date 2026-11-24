using StoryEditorMcp;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole().AddSimpleConsole(x => x.TimestampFormat = "[HH:mm:ss] ");

builder.Services.AddStoryTools(builder.Configuration);
builder.Services.AddMcpTools();

WebApplication app = builder.Build();

app.MapMcp();

app.Run();
