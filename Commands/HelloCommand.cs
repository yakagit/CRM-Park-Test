using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramInnBot.Commands;

public class HelloCommand : ICommand
{
    private readonly IConfiguration _configuration;

    public HelloCommand(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Name => "/hello";

    public async Task ExecuteAsync(Update update, ITelegramBotClient botClient)
    {
        var chatId = update.Message!.Chat.Id;
        var info = _configuration.GetSection("PersonalInfo");
        var response = $"Имя и фамилия: {info["Name"]}\n" +
                       $"Эл. почта: {info["Email"]}\n" +
                       $"GitHub: {info["GitHub"]}\n" +
                       $"Резюме: {info["Resume"]}";
        await botClient.SendTextMessageAsync(chatId, response);
    }
}
