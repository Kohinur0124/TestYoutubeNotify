using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Requests;
using TestYoutube.Repository;

namespace TestYoutube.Services.BackGroundServices
{

	public class ReminderBackgroundService : BackgroundService
	{
		private TelegramBotClient _botClient;

		public ReminderBackgroundService(TelegramBotClient botClient, IServiceScopeFactory serviceScope)
		{
			_botClient = botClient;
			_scopeFactory = serviceScope;
		}
		private IChannelRepository _channelrepo;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly string _apikey = "AIzaSyCL7Y8o3kgyU3w3I5mi6cSHsL0rTntCBIk";


		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			#region scope orqali servicelarni olib ber
			using var scope = _scopeFactory.CreateAsyncScope();
			_channelrepo = scope.ServiceProvider.GetService<IChannelRepository>();
			#endregion

			await Task.Delay(86400000);
			var youtubeService = new YouTubeService(new BaseClientService.Initializer()
			{
				ApiKey = "AIzaSyCL7Y8o3kgyU3w3I5mi6cSHsL0rTntCBIk",
				ApplicationName = this.GetType().ToString()
			});
			while (!stoppingToken.IsCancellationRequested)
			{
				var channels = await _channelrepo.GetChannels();
				foreach (var channel in channels)
				{
					var searchListRequest = youtubeService.Search.List("snippet");

					searchListRequest.ChannelId = channel.ChannelId;
					searchListRequest.PublishedAfterDateTimeOffset = channel.LastCheckDate;
					
					// Replace with your search term.
					searchListRequest.MaxResults = 100;
					// Call the search.list method to retrieve results matching the specified query term.
					var searchListResponse = await searchListRequest.ExecuteAsync();
					List<string> videos = new List<string>();
					
					// Add each result to the appropriate list, and then display the lists of
					// matching videos, channels, and playlists.
					foreach (var searchResult in searchListResponse.Items)
					{
						switch (searchResult.Id.Kind)
						{

							case "youtube#video":
								if(searchResult.Id.PlaylistId == channel.PlaylistId)
								{
									videos.Add(string.Format("{0}\nhttps://www.youtube.com/watch?v={1}",searchResult.Snippet.Title, searchResult.Id.VideoId));
								}
								break;

						}
					}
					if(videos.Count > 0)
					{
						var users = await  _channelrepo.GetChannelsUsers(channel.ChannelId);
						foreach (var user in users)
						{

							var s = $"{channel.ChannelTitle}\nKanalga yangi qo`shilgan videolar :\n";
							int i = 0;
							foreach(var video in videos)
							{
								s += $"{i++} . {video}\n";
							
							}
							await SendTextToUsers(s, long.Parse(user), _botClient,stoppingToken );
						}
 					}
					await _channelrepo.UpdateChannel(channel.ChannelId);
				}
				
				
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
