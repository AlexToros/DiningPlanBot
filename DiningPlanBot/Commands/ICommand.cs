using Telegram.Bot;
using Telegram.Bot.Types;

namespace DiningPlanBot.Commands;

public interface ICommand
{
    public string Key { get; }

    Task ExecuteAsync(TelegramBotClient bot, Message message, string? args);
}