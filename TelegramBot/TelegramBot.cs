using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace TelegramBot
{
    static class TelegramBot
    {
        static string token = "5518542254:AAGeFx8chaqa899geHrnEO7Q1i2TFrRkR4o";
        static ITelegramBotClient bot = new TelegramBotClient(token);
        static MessageController messageController = new MessageController();

        static Dictionary<string, string> Messages = new Dictionary<string, string>();

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
                {
                    var message = update.Message;

                    if (message.Text != null)
                    {
                        string messageText = message.Text.ToString();
                        string Response = messageController.TakeApartMessage(Messages, messageText) ?? "";

                        if (Response != "")
                        {
                            await botClient.SendTextMessageAsync(message.Chat, Response, cancellationToken: cancellationToken);

                            string additionallyMethod = messageController.AdditionallyFunc(messageText, update, botClient);

                            if (additionallyMethod != "")
                            {
                                await botClient.SendPhotoAsync(message.Chat, additionallyMethod, cancellationToken: cancellationToken);
                            }

                            return;
                        }
                        else
                        { 
                            messageController.AdditionallyFunc(messageText, update, botClient);
                            return;
                        }
                    }
                }
            }

            catch (Exception e)
            {
                _ = HandleErrorAsync(bot, e, cancellationToken);
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient,
            Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };

            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            messageController.CreateDictionary(Messages);

            Console.ReadLine();

        }
    }
}