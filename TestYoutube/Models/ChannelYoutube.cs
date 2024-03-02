using System.ComponentModel.DataAnnotations;

namespace TestYoutube.Models
{
	public class ChannelYoutube
	{
		[Key]
		public string ChannelId { get; set; }
		public string ChannelTitle { get; set; }
		public string PlaylistId { get; set; }
		public DateTimeOffset LastCheckDate { get; set; }

		public ICollection<UserChannels> UserChannels { get; set; }
	}
}
