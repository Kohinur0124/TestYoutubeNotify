using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestYoutube.Models
{
	public class UserChannels
	{
		[Key]
		public int Id { get; set; }
		public string ChatIdTelegram { get; set; }
		[ForeignKey("ChannelYoutubeId")]
		public string ChannelYoutubeId { get; set; }
		public DateTimeOffset StartDate { get; set; }

		public ChannelYoutube ChannelYoutube { get; set; }
	}
}
