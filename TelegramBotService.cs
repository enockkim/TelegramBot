using budget_tracker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot
{
    public class TelegramBotService : BackgroundService
    {
        private readonly ILogger _logger;
        private Timer? _timer;
        private readonly AppSettings settings;

        private static TelegramBotClient bot;

        public TelegramBotService(ILogger<TelegramBotService> logger, IOptionsMonitor<AppSettings> _settings)
        {
            _logger = logger;
            settings = _settings.CurrentValue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Telegram Bot started at: {time}", DateTimeOffset.Now);

            bot = new TelegramBotClient(settings.TelegramToken);

            while (!stoppingToken.IsCancellationRequested)
            {

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = new UpdateType[]
                    {
                        UpdateType.Message,
                        UpdateType.EditedMessage
                    }
                };

                bot.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions);

                await Task.Delay(15000, stoppingToken);
            }
            _logger.LogInformation("Telegram Bot stopped at: {time}", DateTimeOffset.Now);
        }

        private static async Task ErrorHandler(ITelegramBotClient telegramBotClient, Exception exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private static async Task UpdateHandler(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
        {
            if(update.Type == UpdateType.Message)
            {
                if(update.Message.Type == MessageType.Text)
                {
                    var text = update.Message.Text;
                    var id = update.Message.Chat.Id;
                    string? username = update.Message.Chat.Username;

                    if (!username.Equals("enock_kim"))
                    {
                        bot.SendTextMessageAsync(id, "Sorry, you are not authorized to acces this bot.");
                    }
                    else
                    {
                        switch (text)
                        {
                            case "/chatid":
                                bot.SendTextMessageAsync(id, id.ToString());
                                break;
                            case "/help":
                                bot.SendTextMessageAsync(id, "Sorry, i havent created a list of commands yet :).");
                                break;
                            default:
                                bot.SendTextMessageAsync(id, "Unknown command, send /help to see a list of commands.");
                                break;
                        }
                    }
                }
            }
        }
    }
}