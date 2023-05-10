using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MitoBDO.Constants;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MitoBDO.Services
{
	public class TimeService : ModuleBase
	{
		private readonly DiscordSocketClient discord;
		private readonly MarketService marketService;
		private const string OfficialChannel = "에페리아 3";

		public static bool GarmothAssembly = true;
		public static bool VellAssembly = true;

		private Timer timer;
		private const double TimerInterval = 60000;

		public TimeService(DiscordSocketClient discord, APIHandler apiHandler, MarketService marketService)
		{
			this.discord = discord;
			this.marketService = marketService;

			timer = new Timer();
			timer.Interval = TimerInterval;
			timer.Elapsed += TimeCheck;
			timer.Start();
		}

		private void ResetTimeService()
		{
			GarmothAssembly = true;
			VellAssembly = true;
		}

		private void TimeCheck(object sender, ElapsedEventArgs e)
		{
			BossTimeCheck();
			Task.Run(marketService.MarketTimeCheck);
		}

		private void BossTimeCheck()
		{
			var now = DateTime.Now;
			var channel = discord.GetChannel(MitoConst.ArcaliveAnnouncChannel) as SocketTextChannel;
			if (channel is null) return;

			if (now.Hour is 7 && now.Minute is 0)
			{
				ResetTimeService();
				return;
			}

			if (now.DayOfWeek is DayOfWeek.Tuesday)
			{
				if (now.Hour is 23 & now.Minute is 10) AssembleAnnounce(now, channel, "가모스");
			}
			else if (now.DayOfWeek is DayOfWeek.Wednesday)
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

			var hour = now.AddHours(13);
			var minute = now.AddMinutes(40);

			if (role is null) return;
			channel.SendMessageAsync($"{role.Mention} 잠시 후 {time:HH}시 {time:mm}분\n{OfficialChannel}채널에서 {bossName} 집결입니다.");
		}

		private void VellAnnounce(DateTime now, SocketTextChannel channel)
		{
			var time = now.AddMinutes(30);
			var role = channel.Guild.Roles.Where(x => x.Name == "벨").FirstOrDefault();

			if (role is null) return;
			channel.SendMessageAsync($"{role.Mention} 잠시 후 {time:HH}시 {time:mm}분\n{OfficialChannel}채널에서 벨리아 출항 대기 바랍니다.");
		}
	}
}
