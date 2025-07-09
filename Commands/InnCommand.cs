using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramInnBot.Models;
using TelegramInnBot.Services;

namespace TelegramInnBot.Commands;

public class InnCommand : ICommand
{
    public string Name => "/inn";
    public string Description => "Поиск информации по ИНН";

    private readonly InnApiService _innApiService;

    public InnCommand(InnApiService innApiService)
    {
        _innApiService = innApiService;
    }

    public async Task ExecuteAsync(Update update, ITelegramBotClient botClient)
    {
        var chatId = update.Message!.Chat.Id;
        var messageText = update.Message.Text ?? "";

        var parts = messageText.Split(' ', 2);
        if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
        {
            await botClient.SendTextMessageAsync(chatId, "Пожалуйста, укажите ИНН после команды. Если их несколько, то разделите их пробелом. Пример: /inn 7707083893 7707083894");
            return;
        }

        var innList = parts[1]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct()
            .ToArray();

        var results = new List<InnResponse>();

        foreach (var inn in innList)
        {
            var result = await _innApiService.GetCompanyInfoAsync(inn);
            if (result != null)
                results.Add(result);
        }

        if (results.Count == 0)
        {
            await botClient.SendTextMessageAsync(chatId, "Ни по одному ИНН не найдена информация.");
            return;
        }

        var distinctResults = results
            .GroupBy(r => r.Inn)
            .Select(g => g.First())
            .OrderBy(r => r.SortName)
            .ToList();

        var sb = new StringBuilder();

        foreach (var company in distinctResults)
        {
            sb.AppendLine("🏢 Название: " + company.Name);
            sb.AppendLine("ИНН: " + company.Inn);
            sb.AppendLine("Адрес: " + company.Address);
            sb.AppendLine(new string('-', 40));
        }

        await botClient.SendTextMessageAsync(chatId, sb.ToString().Trim());
    }
}
