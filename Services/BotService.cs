using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramInnBot.Commands;
using TelegramInnBot.Models;

namespace TelegramInnBot.Services;

public class BotService
{
    private readonly TelegramBotClient _botClient;
    private readonly IEnumerable<ICommand> _commands;
    private readonly ILogger<BotService> _logger;
    private readonly LastAction _lastAction = new();

    public BotService(TelegramBotClient botClient, IEnumerable<ICommand> commands, ILogger<BotService> logger)
    {
        _botClient = botClient;
        _commands = commands;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions { AllowedUpdates = { } };
        await _botClient.ReceiveAsync(OnUpdateReceived, OnError, receiverOptions, cancellationToken);

        await _botClient.SetMyCommandsAsync(
            AppBotCommandRegistry.Commands.Select(c =>
                new Telegram.Bot.Types.BotCommand { Command = c.Name, Description = c.Description }),
            cancellationToken: cancellationToken);
    }

    private async Task OnUpdateReceived(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message?.Text is null) return;

        var message = update.Message.Text;
        var chatId = update.Message.Chat.Id;
        _logger.LogInformation("Получено сообщение: {Message} из чата {ChatId}", message, chatId);

        var commandText = message.Split(' ')[0].ToLower();
        var args = message.Contains(' ') ? message[(message.IndexOf(' ') + 1)..] : string.Empty;

        if (commandText == "/last")
        {
            if (!string.IsNullOrEmpty(_lastAction.Command))
            {
                commandText = _lastAction.Command;
                args = _lastAction.Arguments;
                update.Message.Text = $"{commandText} {args}".Trim();
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, "Нет предыдущих действий для их повтора.", cancellationToken: cancellationToken);
                return;
            }
        }

        var command = _commands.FirstOrDefault(c => c.Name.Equals(commandText, StringComparison.OrdinalIgnoreCase));
        if (command != null)
        {
            if (commandText != "/last")
            {
                _lastAction.Command = commandText;
                _lastAction.Arguments = args;
                _lastAction.ChatId = chatId;
            }
            await command.ExecuteAsync(update, _botClient);
        }
        else
        {
            await _botClient.SendTextMessageAsync(chatId, "Неизвестная команда. Чтобы ознакомиться с актуальными командами, Используйте /help.", cancellationToken: cancellationToken);
        }
    }

    private Task OnError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Ошибка в работе бота");
        return Task.CompletedTask;
    }
}
