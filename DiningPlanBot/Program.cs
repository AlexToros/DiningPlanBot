namespace DiningPlanBot;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddHostedService<BotService>();
        builder.Services.AddHttpClient();

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        
        builder.Services.Configure<TelegramBotSettings>(config.GetSection(nameof(TelegramBotSettings)));

        builder.Services.AddSingleton<DiningClient>();
        builder.Services.AddSingleton<DiningParser>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapGet("/", () => "Hello");

        app.Run();
    }
}