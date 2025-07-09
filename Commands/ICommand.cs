using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramInnBot.Commands;

public interface ICommand
{
    string Name { get; }
    Task ExecuteAsync(Update update, ITelegramBotClient botClient);
}
