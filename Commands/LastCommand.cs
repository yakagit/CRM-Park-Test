using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramInnBot.Commands;

public class LastCommand : ICommand
{
    public string Name => "/last";

    public async Task ExecuteAsync(Update update, ITelegramBotClient botClient)
    {
        // Обработка команды /last реализована в BotService
        await Task.CompletedTask;
    }
}
