using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramInnBot.Commands;

public class StartCommand : ICommand
{
    public string Name => "/start";

    public async Task ExecuteAsync(Update update, ITelegramBotClient botClient)
    {
        var chatId = update.Message!.Chat.Id;
        await botClient.SendTextMessageAsync(chatId, "Добро пожаловать в тестового бота для поиска информации об организациях! Чтобы ознакомиться со списком команд, наберите /help.");
    }
}
