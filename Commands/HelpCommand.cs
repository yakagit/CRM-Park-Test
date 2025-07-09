using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramInnBot.Models;

namespace TelegramInnBot.Commands;

public class HelpCommand : ICommand
{
    public string Name => "/help";

    public async Task ExecuteAsync(Update update, ITelegramBotClient botClient)
    {
        var chatId = update.Message!.Chat.Id;
        var helpText = "Доступные команды:\n" +
                       string.Join("\n", AppBotCommandRegistry.Commands.Select(c => $"{c.Name} - {c.Description}"));
        await botClient.SendTextMessageAsync(chatId, helpText);
    }
}
