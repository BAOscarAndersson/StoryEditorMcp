namespace StoryEditorMcp;

public static class ServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddStoryTools(IConfiguration configuration)
        {
            string path = configuration["storyPath"] 
                ?? throw new Exception("Need the path for the story.");

            services.AddSingleton(x => new StoryTools(path));

            return services;
        }

        public IServiceCollection AddMcpTools()
        {
            services
                .AddMcpServer()
                .WithHttpTransport(options =>
                {
                    options.Stateless = true;
                })
                .WithTools<McpApplyEdit>()
                .WithTools<McpStoryTools>();

            return services;
        }
    }
}
