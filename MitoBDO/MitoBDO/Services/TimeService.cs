using Discord.Commands;
using Discord.WebSocket;
using MitoBDO.Constants;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MitoBDO.Services
{
	public class TimeService : ModuleBase
	{
		protected class BossTime
		{
			public int hour { get; set; }
			public int minute { get; set; }
			public DayOfWeek dayOfWeek { get; set; }

			public BossTime(int hour, int minute, DayOfWeek dayOfWeek)
			{
				this.hour = hour;
				this.minute = minute;
				this.dayOfWeek = dayOfWeek;
			}

			public bool Compare(DateTime time)
			{
				return time.DayOfWeek == this.dayOfWeek
					&& time.Hour == this.hour
					&& time.Minute == this.minute;
			}
		}

		private readonly DiscordSocketClient discord;
		private readonly MarketService marketService;

		private Timer timer;
		private const double TimerInterval = 60000;
		private List<BossTime> VellTime = new();

		/// <summary>
		/// key: 공지 채널(디코) , value: 집결 채널(인게임)
		/// </summary>
		private Dictionary<ulong, string> AnnounceChannelList = new();

		public TimeService(DiscordSocketClient discord, APIHandler apiHandler, MarketService marketService)
		{
			this.discord = discord;
			this.marketService = marketService;

			InitializeBossTime();

			timer = new Timer();
			timer.Interval = TimerInterval;
			timer.Elapsed += TimeCheck;
			timer.Start();
		}

		private void InitializeChannelList()
		{
			VellTime.Clear();
			VellTime.Add(new BossTime(23, 30, DayOfWeek.Wednesday));
			VellTime.Add(new BossTime(16, 15, DayOfWeek.Sunday));
		}

		private void InitializeBossTime()
		{
			AnnounceChannelList.Clear();
			AnnounceChannelList.Add(MitoConst.ArcaliveAnnouncChannel, MitoConst.ArcaliveOfficialChannel);
		}

		private void TimeCheck(object? sender, ElapsedEventArgs e)
		{
			BossTimeCheck();
			Task.Run(marketService.MarketTimeCheck);
		}

		private void BossTimeCheck()
		{
			var now = DateTime.Now;

			foreach (var time in VellTime)
			{
				if (time.Compare(now))
				{
					VellAnnounce(now);
				}
			}
		}

		private void VellAnnounce(DateTime now)
		{
			var time = now.AddMinutes(30);

			foreach (var iter in AnnounceChannelList)
			{
				var channel = discord.GetChannel(iter.Key) as SocketTextChannel;
				if (channel is null) continue;


				var role = channel.Guild.Roles.Where(x => x.Name == "벨").FirstOrDefault();
				if (role is null) continue;

				channel.SendMessageAsync($"{role.Mention} 잠시 후 {time:HH}시 {time:mm}분\n{iter.Value}채널에서 벨리아 출항 대기 바랍니다.");
			}
		}
	}
}
