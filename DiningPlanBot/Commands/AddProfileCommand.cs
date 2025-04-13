using DiningPlanBot.Profiles;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DiningPlanBot.Commands;

public class AddProfileCommand : ICommand
{
    private readonly DiningClient _diningClient;
    private readonly IProfilesStore _profilesStore;
    public const string Name = "/addProfile";
    public string Key => Name;

    public AddProfileCommand(DiningClient diningClient, IProfilesStore profilesStore)
    {
        _diningClient = diningClient;
        _profilesStore = profilesStore;
    }
    
    public async Task ExecuteAsync(TelegramBotClient bot, Message msg, string args)
    {
        try
        {
            var url = msg.Text.Split(' ')[0];
            var diningPlan = await _diningClient.GetDiningPlanAsync(url);
            await _profilesStore.SaveProfileAsync(msg.Chat.Id, diningPlan);
            await bot.SendMessage(msg.Chat.Id, $"Профиль '{diningPlan.Pupil}' добавлен");
        }
        catch (Exception e)
        {
            await bot.SendMessage(msg.Chat, $"Ошибка: {e.Message}");
        }
    }
}