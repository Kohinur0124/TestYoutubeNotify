using Microsoft.EntityFrameworkCore;
using TestYoutube.Data;
using TestYoutube.Models;

namespace TestYoutube.Repository
{
	public class ChannelRepository : IChannelRepository
	{

		private BotYoutubeDbContext _context {  get; set; }
        public ChannelRepository(BotYoutubeDbContext context)
        {
            _context = context;
        }


        public async ValueTask<bool> AddChannel(string channelId, string channelName)
		{
			try
			{

				var check = _context.ChannelYoutube.FirstOrDefault(x => x.ChannelId == channelId);
				if(check is null)
				{

					var chan = new ChannelYoutube()
					{
						ChannelId = channelId,
						ChannelTitle = channelName,
						LastCheckDate = DateTimeOffset.Now,
					};
					await _context.ChannelYoutube.AddAsync(chan);
					await _context.SaveChangesAsync();
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				return false;
			}
			
		}

		public async ValueTask<bool> AddUserChannel(string channelid,string chatId)
		{
			try
			{

				var check = _context.UserChannels.FirstOrDefault(x => x.ChannelYoutubeId == channelid &&  x.ChatIdTelegram == chatId);
				if (check == null)
				{

					var chan = new UserChannels()
					{
						ChannelYoutubeId = channelid,
						ChatIdTelegram = chatId,
						StartDate = DateTimeOffset.Now,
					};
					await _context.UserChannels.AddAsync(chan);
					await _context.SaveChangesAsync();
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async ValueTask<List<ChannelYoutube>> GetChannels()
		{
			var channels = await _context.ChannelYoutube.ToListAsync();
			return  channels ;
		}

		public async ValueTask<bool> UpdateChannel(string channelId)
		{
			try
			{

			var updatechannel = await _context.ChannelYoutube.FirstOrDefaultAsync(x => x.ChannelId == channelId);
			if(updatechannel is not null)
			{
				updatechannel.LastCheckDate = DateTimeOffset.Now; 

			}
			_context.ChannelYoutube.Update(updatechannel);
			await _context.SaveChangesAsync();	
			return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async ValueTask<List<string>> GetChannelsUsers(string channelId)
		{
			var users = await _context.UserChannels.Where(x => x.ChannelYoutubeId == channelId).Select(x => x.ChatIdTelegram).ToListAsync();
			return users;
		}
		public async ValueTask<List<UserChannels>> GetUsersChannels(string userId)
		{
			var users = await _context.UserChannels.Where(x => x.ChatIdTelegram == userId).ToListAsync();

			return users;
		}
	}
}
