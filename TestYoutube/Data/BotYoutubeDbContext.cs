using Microsoft.EntityFrameworkCore;
using TestYoutube.Models;

namespace TestYoutube.Data
{
	public class BotYoutubeDbContext: DbContext
	{
		public BotYoutubeDbContext(DbContextOptions<BotYoutubeDbContext> options)
		  : base(options)
		{
			
		}
	
		public  DbSet<ChannelYoutube> ChannelYoutube { get; set; }
		public  DbSet<UserChannels> UserChannels { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<ChannelYoutube>()
				.HasMany(x => x.UserChannels)
				.WithOne(x => x.ChannelYoutube)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
