using System.Globalization;
using TestYoutube.Models;

namespace TestYoutube.Repository
{
	public interface IChannelRepository
	{
		public ValueTask<bool> AddChannel (string channelId, string channelName,string playlistId);
		public ValueTask<bool> UpdateChannel(string channelId);
		public ValueTask<bool> DeleteChannel(string channelId,string chatId);
		public ValueTask<List<ChannelYoutube>> GetChannels();
		public ValueTask<bool> AddUserChannel(string channelid,string chatId);
		public ValueTask<List<string>> GetChannelsUsers(string channelId);
		public ValueTask<List<UserChannels>> GetUsersChannels(string userId);

	}
}
