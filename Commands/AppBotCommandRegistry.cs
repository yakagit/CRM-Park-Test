namespace TelegramInnBot.Models;

public static class AppBotCommandRegistry
{
    public static readonly AppBotCommand[] Commands =
    [
        new("/start", "Добро пожаловать в тестового бота для поиска информации об организациях по ИНН!"),
        new("/help", "Узнать список доступных команд"),
        new("/hello", "Узнать информацию о разработчике"),
        new("/inn", "Узнать информацию о компании или компаниях (например, /inn 1234567890 0987654321)"),
        new("/last", "Повторить последнюю команду")
    ];
}
