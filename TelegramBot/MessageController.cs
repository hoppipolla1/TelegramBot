using System.Net;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot
{
    class MessageController
    {
        public string TakeApartMessage(string text)
        {
            string value;
            Messages.TryGetValue(text, out value);

            if (value == null)
            {
                if (text.Contains("woof"))
                {
                    string translateMessage;
                    WebRequest request = WebRequest.Create("https://api.funtranslations.com/translate/doge.json" + "?text=$" + text);

                    WebResponse response = request.GetResponse();

                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        translateMessage = sr.ReadToEnd();
                    }

                    var json = JObject.Parse(translateMessage);

                    string s = json["contents"].First.ToString();
                    int startIndex = s.IndexOf("$") + 1;
                    int endIndex = s.Length - 2;

                    value = s.Substring(startIndex, endIndex - startIndex);

                }
            }

            return value ?? "";
        }

        public string AdditionallyFunc(string responseText, Update update, ITelegramBotClient botClient)
        {
            if (responseText == "Да")
            {
                return "https://upload.wikimedia.org/wikipedia/commons/7/71/English_setter.jpg";
            }
            else if (responseText == "woof")
            {
                return "WooF";
            }
            else if (responseText.Contains("/set"))
            {
                MessageTrigger(update, botClient);
                return "";
            }
            else
                return "";
        }

        private async static void MessageTrigger(Update update, ITelegramBotClient botClient)
        {
            var message = update.Message;
            if (message == null || message.Type != MessageType.Text) return;

            if (message.Text.StartsWith("/set"))
            {
                string resultString = Regex.Match(message.Text, @"\d+").Value;
                bool isNumeric = int.TryParse(resultString, out int time);
                if (isNumeric && time > 0 && time <= 24)
                {
                    Console.WriteLine("Добавлено напоминание для " + message.From + " через " + time.ToString() + " часов.");
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Добавлено напоминание, которое оповестит вас через {time} часов.") ;
                    await Task.Delay(time * 1000);
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Время кормить собаку woof!");
                    Console.WriteLine("Напоминание для " + message.From + " выполнено.");
                }
                else
                {
                    string returnMessage = "ℹ️ <b> Гайд:</b>" + Environment.NewLine;
                    returnMessage += Environment.NewLine;
                    returnMessage += "  /set 8   --   Напомнить через 8 часов" + Environment.NewLine;
                    returnMessage += Environment.NewLine;
                    returnMessage += "Максимальное значение 24" + Environment.NewLine;
                    await botClient.SendTextMessageAsync(message.Chat.Id, returnMessage, ParseMode.Html);
                }
            }
        }

        public void CreateDictionary(Dictionary<string, string> Messages)
        {
            Messages.Add("/start", "Привет, woof woof! Чтобы узнать список команд введи /help woof!");
            Messages.Add("/help", "На данный момент доступны команды: " +
                "\n /facts - Факты о собаках! woof! " +
                "\n /translite - Перевод сообщений на собачий! woof!  " +
                "\n /set - Напоминание о кормлении вашей собаки woof!");
            Messages.Add("/facts", "Могу тебе рассказать про англйского сеттера! Если хочешь, напиши 'Да'");
            Messages.Add("/translite", "Теперь напиши любой текст НА АНГЛИЙСКОМ, а в конце напиши woof");
            Messages.Add("/set", "");

            Messages.Add("Да", "Англи́йский се́ттер (англ. english setter) — порода собак из группы легавых, прямой потомок старых европейских собак, использовавшихся в Средние века для охоты на птиц с сетью. В XVII—XVIII веках, в связи с распространением охотничьего огнестрельного оружия, эти собаки подверглись преобразованию: убыстрился их ход, более красивой стала стойка. Своим современным обликом собаки породы в значительной степени обязаны Э. Лавераку, который, начав свою деятельность в 1825 году, в течение 50 лет изменял и улучшал качества породы путём инбридинга, подбора и отбора. Английский сеттер — наиболее известная и распространённая порода среди сеттеров.");
            Messages.Add("woof", "woof woof! WOF!");
        }
    }
}
