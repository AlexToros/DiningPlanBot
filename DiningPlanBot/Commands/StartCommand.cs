using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DiningPlanBot.Commands;

public class StartCommand : ICommand
{
    public string Key => "/start";
    public async Task ExecuteAsync(TelegramBotClient bot, Message msg, string args)
    {
        await bot.SendMessage(msg.Chat, """
                                      <b><u>Как пользоваться ботом</u></b>:
                                      Для добавления меню ученика, пришлите ссылку на то меню, которое хотите добавить
                                      Пример ссылки - http://94.181.168.69:4483/Vesta/hs/Vesta/[ID_меню]
                                      /dishes - Если у вас всего одно меню - выведет его, если несколько - выведет список доступных для выбора
                                      
                                      """, parseMode: ParseMode.Html, linkPreviewOptions: true,
        replyMarkup: new ReplyKeyboardRemove()); // also remove keyboard to clean-up things
    }
}