﻿using DiningPlanBot.Commands;
using DiningPlanBot.Profiles;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DiningPlanBot;

public class BotService : BackgroundService
{
    private readonly DiningClient _diningClient;
    private readonly IProfilesStore _profileStore;
    private readonly string _token;
    private readonly Dictionary<string, ICommand> _commands;
    private User _me;
    private TelegramBotClient _bot;

    public BotService(IOptions<AppSettings> settings, DiningClient diningClient, IProfilesStore profileStore, IEnumerable<ICommand> commands)
    {
        _diningClient = diningClient;
        _profileStore = profileStore;
        _token = settings.Value.BotToken;
        _commands = commands.ToDictionary(x => x.Key);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("BotService is starting.");

        using var cts = new CancellationTokenSource();
        _bot = new TelegramBotClient(_token, cancellationToken: cts.Token);

        _me = await _bot.GetMe();
        await _bot.DeleteWebhook();
        await _bot.DropPendingUpdates();

        _bot.OnError += OnError;
        _bot.OnMessage += OnMessage;
        _bot.OnUpdate += OnUpdate;

        stoppingToken.WaitHandle.WaitOne();
        Console.WriteLine("Stopping bot");
    }

    private async Task OnUpdate(Update update)
    {
        switch (update)
        {
            case { CallbackQuery: { } callbackQuery }: await OnCallbackQuery(callbackQuery); break;
            case { PollAnswer: { } pollAnswer }: await OnPollAnswer(pollAnswer); break;
            default: Console.WriteLine($"Received unhandled update {update.Type}"); break;
        }

        ;
    }

    async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        await _bot.AnswerCallbackQuery(callbackQuery.Id, $"You selected {callbackQuery.Data}");
        await _bot.SendMessage(callbackQuery.Message!.Chat,
            $"Received callback from inline button {callbackQuery.Data}");
    }

    async Task OnPollAnswer(PollAnswer pollAnswer)
    {
        if (pollAnswer.User != null)
            await _bot.SendMessage(pollAnswer.User.Id,
                $"You voted for option(s) id [{string.Join(',', pollAnswer.OptionIds)}]");
    }

    private Task OnError(Exception exception, HandleErrorSource source)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }

    private async Task OnMessage(Message msg, UpdateType type)
    {
        if (msg.Text is not { } text)
            Console.WriteLine($"Received a message of type {msg.Type}");
        else if (text.StartsWith('/'))
        {
            var space = text.IndexOf(' ');
            if (space < 0) space = text.Length;
            var command = text[..space].ToLower();
            if (command.LastIndexOf('@') is > 0 and int at) // it's a targeted command
                if (command[(at + 1)..].Equals(_me.Username, StringComparison.OrdinalIgnoreCase))
                    command = command[..at];
                else
                    return; // command was not targeted at me
            await OnCommand(command, text[space..].TrimStart(), msg);
        }
        else if (text.StartsWith(Constants.BaseVestaUrl))
            _commands[AddProfileCommand.Name].ExecuteAsync(_bot, msg, null);
        else
            await OnTextMessage(msg);
    }

    private async Task OnNewProvileReceived(Message msg)
    {
    }

    async Task OnTextMessage(Message msg) // received a text message that is not a command
    {
        Console.WriteLine($"Received text '{msg.Text}' in {msg.Chat}");
        await OnCommand("/start", "", msg); // for now we redirect to command /start
    }

    async Task OnCommand(string command, string args, Message msg)
    {
        Console.WriteLine($"Received command: {command} {args}");

        if (_commands.TryGetValue(command, out var commandHandler))
            await commandHandler.ExecuteAsync(_bot, msg, args);
        
        switch (command)
        {
           
            // case "/dining-plan":
            //     if (args.StartsWith("http"))
            //     {
            //         Console.WriteLine($"Dining plan from:{args}");
            //         var plan = await _diningClient.GetDiningPlan(args);
            //         await bot.SendMessage(msg.Chat, plan.Pupil);
            //     }
            //
            //     break;
            // case "/photo":
            //     if (args.StartsWith("http"))
            //         await bot.SendPhoto(msg.Chat, args, caption: "Source: " + args);
            //     else
            //     {
            //         await bot.SendChatAction(msg.Chat, ChatAction.UploadPhoto);
            //         await Task.Delay(2000); // simulate a long task
            //         await using var fileStream = new FileStream("bot.gif", FileMode.Open, FileAccess.Read);
            //         await bot.SendPhoto(msg.Chat, fileStream, caption: "Read https://telegrambots.github.io/book/");
            //     }
            //     break;
            // case "/inline_buttons":
            //     await bot.SendMessage(msg.Chat, "Inline buttons:", replyMarkup: new InlineKeyboardButton[][] {
            //         ["1.1", "1.2", "1.3"],
            //         [("WithCallbackData", "CallbackData"), ("WithUrl", "https://github.com/TelegramBots/Telegram.Bot")]
            //     });
            //     break;
            // case "/keyboard":
            //     await bot.SendMessage(msg.Chat, "Keyboard buttons:", replyMarkup: new[] { "MENU", "INFO", "LANGUAGE" });
            //     break;
            // case "/remove":
            //     await bot.SendMessage(msg.Chat, "Removing keyboard", replyMarkup: new ReplyKeyboardRemove());
            //     break;
            // case "/poll":
            //     await bot.SendPoll(msg.Chat, "Question", ["Option 0", "Option 1", "Option 2"], isAnonymous: false, allowsMultipleAnswers: true);
            //     break;
            // case "/reaction":
            //     await bot.SetMessageReaction(msg.Chat, msg.Id, ["❤"], false);
            //     break;
        }
    }
}