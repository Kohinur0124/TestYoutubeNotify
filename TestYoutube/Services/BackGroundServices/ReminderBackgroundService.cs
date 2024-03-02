using Telegram.Bot;

namespace TestYoutube.Services.BackGroundServices
{

	public class ReminderBackgroundService : BackgroundService
	{
		private TelegramBotClient _botClient;

		public ReminderBackgroundService(TelegramBotClient botClient)
			=> _botClient = botClient;

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Delay(500);

			while (!stoppingToken.IsCancellationRequested)
			{
				
			}
		}

		private async ValueTask SendTextToUsers(string text, long chatId, ITelegramBotClient botClient, CancellationToken cancellationToken)
		{
			await _botClient.SendTextMessageAsync(
				chatId: chatId,
				text: text,
				cancellationToken: cancellationToken);
		}
	}

}
