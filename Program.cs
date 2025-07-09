using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TelegramInnBot.Commands;
using TelegramInnBot.Services;
using TelegramInnBot.Models;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var telegramToken = configuration["TelegramBot:Token"];
        var apiKey = configuration["InnApi:ApiKey"];

        if (string.IsNullOrEmpty(telegramToken))
            throw new InvalidOperationException("TelegramBot:Token is missing in appsettings.json");
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("InnApi:ApiKey is missing in appsettings.json");

        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole()
                   .AddConfiguration(configuration.GetSection("Logging"))
                   .AddFilter("Telegram.Bot", LogLevel.Warning);
        });

        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<InnApiService>();
        services.AddSingleton(new TelegramBotClient(telegramToken));
        services.AddSingleton<BotService>();

        var commandTypes = new[]
        {
            typeof(StartCommand),
            typeof(HelpCommand),
            typeof(HelloCommand),
            typeof(InnCommand),
            typeof(LastCommand)
        };
        foreach (var type in commandTypes)
        {
            services.AddSingleton(typeof(ICommand), type);
        }

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var botService = scope.ServiceProvider.GetRequiredService<BotService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            logger.LogInformation("Shutting down the bot...");
            e.Cancel = true;
            cts.Cancel();
        };

        try
        {
            logger.LogInformation("Starting the bot...");
            await botService.StartAsync(cts.Token);
            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Bot stopped gracefully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Bot stopped unexpectedly.");
            throw;
        }
        finally
        {
            if (serviceProvider is IDisposable disposable)
                disposable.Dispose();

            Environment.Exit(0);
        }
    }
}
