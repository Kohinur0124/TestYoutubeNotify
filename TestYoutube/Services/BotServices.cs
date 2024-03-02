using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using TestYoutube.Repository;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Telegram.Bot.Types.ReplyMarkups;
using System.Globalization;
using TestYoutube.Models;
using static System.Net.Mime.MediaTypeNames;

namespace TestYoutube.Services
{

	public partial class UpdateHandlerService : IUpdateHandler
	{
		private IChannelRepository _channelrepo;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly string _apikey = "AIzaSyCL7Y8o3kgyU3w3I5mi6cSHsL0rTntCBIk";

		public UpdateHandlerService(IServiceScopeFactory scopeFactory)
			=> _scopeFactory = scopeFactory;

		public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			#region scope orqali servicelarni olib ber
			using var scope = _scopeFactory.CreateAsyncScope();
			_channelrepo = scope.ServiceProvider.GetService<IChannelRepository>();
			#endregion

			if (update.Type == UpdateType.CallbackQuery)
			{
				var query = update.CallbackQuery.Data.Split("##").ToList();
				if (query[1].Length > 0 && query[2].Length > 0)
				{
					bool result = await _channelrepo.AddChannel(query[1], query[2], query[4]);
					bool result1 = await _channelrepo.AddUserChannel(query[1], query[3]);

					if (result && result1)
						await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Ushbu kanal shaxsiy obunalaringizga qo`shildi .",
						cancellationToken: cancellationToken
						);
					else
					{
						await botClient.SendTextMessageAsync(
						chatId: update.CallbackQuery.From.Id,
						text: "Obunalarga qo`shishda xatolik yuz berdi . Qaytadan  urinib ko`ring .",

						cancellationToken: cancellationToken
						);

					}
				};
			}
			else
			{

				var updateHandler = update.Type switch
				{
					UpdateType.Message => HandleMessageAsync(botClient, update, cancellationToken),
					_ => HandleUnknownUpdateAsync(botClient, update, cancellationToken),
				};
			
				try
				{
					await updateHandler;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
			=> throw new NotImplementedException();

		private Task HandleUnknownUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
			=> throw new NotImplementedException();




		private async Task HandleMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			var message = update.Message;

			var messageHandler = message.Type switch
			{
				MessageType.Text => HandleTextMessageAsync(botClient, update, cancellationToken),
				_ => HandleUnknownMessageAsync(botClient, update, cancellationToken),
			};

			try
			{
				await messageHandler;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private async ValueTask HandleUnknownMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
			=> await SendTextMessageAsync("error",botClient,update, cancellationToken);

		private async ValueTask HandleTextMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			var text = update.Message.Text;
			var command = string.Empty;

			try
			{
				if (text.StartsWith("/start"))
				{
					await StartCommandAsync(botClient, update, cancellationToken);
				}
				else if (text.StartsWith("/newchannel "))
				{
					await NewChannelCommandAsync(botClient, update, cancellationToken);
				}
		
				else if (text.StartsWith("/channels"))
				{
					await ChannelsCommandAsync(botClient, update, cancellationToken);
				}
	
				else{
					await UnknownCommandAsync(botClient, update, cancellationToken);
				};
			}

			
			catch (Exception ex)
			{
				await SendTextMessageAsync("Commandlarni to`g`ri kiriting !",botClient,update,cancellationToken);
			}
		}

		private async ValueTask UnknownCommandAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
			=> await SendTextMessageAsync("Unknown Command", botClient, update, cancellationToken);

		/*public async ValueTask AddCommandAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
			var query =  update.CallbackQuery.Data.Split("##").ToList();
			if (query[1].Length >0 && query[2].Length >0)
			{
				bool result = await _channelrepo.AddChannel(query[1], query[2]);
				bool result1 = await _channelrepo.AddUserChannel(query[1], query[3]);

				if(result && result1)
				await botClient.SendTextMessageAsync(
				chatId: update.CallbackQuery.From.Id,
				text: "Ushbu kanal shaxsiy obunalaringizga qo`shildi .",
				cancellationToken: cancellationToken
				);
				else
				{
					await botClient.SendTextMessageAsync(
					chatId: update.CallbackQuery.From.Id,
					text: "Obunalarga qo`shishda xatolik yuz berdi . Qaytadan  urinib ko`ring .",

					cancellationToken: cancellationToken
					);

				}
				
			}
			else
			{

				await botClient.SendTextMessageAsync(
				chatId: update.CallbackQuery.From.Id,
				text: "Obunalarga qo`shishda xatolik yuz berdi . Qaytadan  urinib ko`ring .",
				
				cancellationToken: cancellationToken
				); ; ;
			}

			


		}
*/
		private async ValueTask NewChannelCommandAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			var search = update.Message.Text.Remove(0, 11);

			var youtubeService = new YouTubeService(new BaseClientService.Initializer()
			{
				ApiKey = _apikey,
				ApplicationName = this.GetType().ToString()
			});
			var searchListRequest = youtubeService.Search.List("snippet");
			searchListRequest.Type = "playlist";
			searchListRequest.Q = search;

			// Replace with your search term.
			searchListRequest.MaxResults = 50;
			// Call the search.list method to retrieve results matching the specified query term.
			var searchListResponse = await searchListRequest.ExecuteAsync();
			var channelcommand = searchListResponse.Items[0];



			InlineKeyboardMarkup inlineKeyboard = new(new[]
			{
				InlineKeyboardButton.WithCallbackData(
					 "Obuna" , callbackData :  $"/add##{channelcommand.Snippet.ChannelId}##{channelcommand.Snippet.ChannelTitle}##{update.Message.Chat.Id}##{channelcommand.Id.PlaylistId}")
,


			}); ;

			Message sentMessage = await botClient.SendTextMessageAsync(
							chatId: update.Message.Chat.Id,
							text: $"{channelcommand.Snippet.ChannelTitle}\n Kanaldagi podcastlarni kuzatish uchun Obuna tugmasini bosing.",
							replyMarkup: inlineKeyboard,
							cancellationToken: cancellationToken);


		}



		private async ValueTask ChannelsCommandAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			var allchannels = await _channelrepo.GetUsersChannels(update.Message.Chat.Id.ToString());

			string s = "Obuna bo`lgan kanallaringiz :\n";
			int i = 1;
			foreach ( var channel in  allchannels)
			{
				var title = await _channelrepo.GetChannels();
				
				s+= $"{i++} . "+  title.FirstOrDefault(x => x.ChannelId == channel.ChannelYoutubeId).ChannelTitle + "\n";
				
			}

			await SendTextMessageAsync(s, botClient, update, cancellationToken);

		}


		private async ValueTask SendTextMessageAsync(string text, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			await botClient.SendTextMessageAsync(
				chatId: update.Message.Chat.Id,
				text: text,
				parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,

				cancellationToken: cancellationToken
				); ; ;
		}



		private async ValueTask StartCommandAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			var tgUser = update.Message.From;

			await SendTextMessageAsync($"{tgUser.FirstName } Botimizga Xush kelibsiz . " +
				$"\n/newchannel - Youtube kanallarni kuzatishga qo`shish" +
				$"(/newchannel dan keyin kanal nomini yozib jo`nating)" +
				$"\n/channels - Kuzatuvga qo`shilgan kanallar", botClient, update, cancellationToken);
		}

	}
}
