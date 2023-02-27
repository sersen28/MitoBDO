using Discord.Commands;
using Discord.WebSocket;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MitoBDO.Services
{
	public class TimeService : ModuleBase
	{
		private readonly DiscordSocketClient discord;

		public static bool GarmothAssembly = true;
		public static bool VellAssembly = true;

		private Timer timer;
		private double interval = 60000;
		private ulong channelID = 1036557769353986058;

		public TimeService(DiscordSocketClient discord)
		{
			this.discord = discord;

			timer = new Timer();
			timer.Interval = interval;
			timer.Elapsed += TimeCheck;
			timer.Start();
		}

		private void ResetTimeService()
		{
			GarmothAssembly = true;
			GarmothAssembly = true;
		}

		private void TimeCheck(object sender, ElapsedEventArgs e)
		{
			var now = DateTime.Now;
			var channel = discord.GetChannel(channelID) as SocketTextChannel;
			if (channel is null) return;

			if (now.Hour is 7 && now.Minute is 0)
			{
				ResetTimeService();
				return;
			}

			if (now.DayOfWeek is DayOfWeek.Wednesday)
			{
				if (now.Hour is 23 & now.Minute is 30) VellAnnounce(now, channel);
			}
			else if (now.DayOfWeek is DayOfWeek.Thursday)
			{
				if (now.Hour is 23 & now.Minute is 10) AssembleAnnounce(now, channel, "가모스");
			}
			else if (now.DayOfWeek is DayOfWeek.Saturday)
			{
				if (now.Hour is 23 & now.Minute is 40) AssembleAnnounce(now, channel, "가모스");
			}
			else if (now.DayOfWeek is DayOfWeek.Sunday)
			{
				if (now.Hour is 16 & now.Minute is 15) VellAnnounce(now, channel);
			}
		}

		private void AssembleAnnounce(DateTime now, SocketTextChannel channel, string bossName)
		{
			var time = now.AddMinutes(30);
			var role = channel.Guild.Roles.Where(x => x.Name == bossName).FirstOrDefault();

			if (role is null) return;
			channel.SendMessageAsync($"{role.Mention} 잠시 후 {time.Hour}시 {time.Minute}분 {bossName} 집결입니다.");
		}

		private void VellAnnounce(DateTime now, SocketTextChannel channel)
		{
			var time = now.AddMinutes(30);
			var role = channel.Guild.Roles.Where(x => x.Name == "벨").FirstOrDefault();

			if (role is null) return;
			channel.SendMessageAsync($"{role.Mention} 잠시 후 {time.Hour}시 {time.Minute}분 벨리아 출항 대기 바랍니다.");
		}
	}
}
