namespace TelegramInnBot.Models;

public class LastAction
{
    public string Command { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;
    public long ChatId { get; set; }
}
