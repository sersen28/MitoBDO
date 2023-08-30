using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MitoBDO.Constants;
using MitoBDO.Model;
using MitoBDO.Utility;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Data.SQLite;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;

namespace MitoBDO.Services
{
	public class MarketService : ModuleBase
	{
		private const string CsvPath = "Data/ItemCodes.csv";
		private List<MarketItem> ItemList = new List<MarketItem>();

		private readonly DiscordSocketClient discord;
		private readonly APIHandler apiHandler;
		private readonly DatabaseService databaseService;

		public MarketService(DiscordSocketClient discord, APIHandler apiHandler, DatabaseService databaseService)
		{
			this.discord = discord;
			this.apiHandler = apiHandler;
			this.databaseService = databaseService;

			Task.Run(InitItemCodes);
		}

		public async Task DeleteMarketAlarm(SocketCommandContext context, string[] args)
		{
			if (args.Length < 2)
			{
				return;
			}

			var item = FindItemByName(args[0]);
			if (item is null)
			{
				return;
			}

			string message = string.Empty;
			if (ulong.TryParse(args[1], out var enLevel))
			{
				var alarm = new MarketAlarm(item.ItemCode, enLevel, $"{context.User.Id}");

				if (databaseService.DeleteAlarmTable(alarm))
				{
					message = $"{context.User.Mention} 거래소 알림이 삭제되었습니다.";
					var embed = new EmbedBuilder();
					embed.Color = Color.Blue;
					embed.Title = $"{item.ItemName}";
					embed.ThumbnailUrl = MitoConst.GetItemImageURL($"{item.ItemCode}");
					embed.Description = $"아이템 코드: {item.ItemCode}\n"
						+ $"카테고리: {item.MainCategory}\n"
						+ $"서브 카테고리: {item.SubCategory}\n";
					await context.Channel.SendMessageAsync(message, embed: embed.Build());
				}
				else
				{
					message = $"{context.User.Mention} 거래소 알림 삭제가 실패했습니다.\n"
						+ "등록되지 않은 알람입니다.";
					await context.Channel.SendMessageAsync(message);
				}
			}
			else
			{
				message = $"{context.User.Mention} 거래소 알림 삭제가 실패했습니다.\n"
					+ "명령어를 다시 확인해주십시오."
					+ "명령어 양식: !알람등록 [아이템이름] [강화단계]\n"
					+ "[아이템이름]: 공백을 @로 대신하여 정확히 입력해주셔야 합니다.\n"
					+ "[강화단계]: 0~5 혹은 0~20까지의 강화단계를 숫자로 입력하시면 됩니다.\n\n"
					+ "ex) !알람등록 죽은신의@갑옷 4\n"
					+ "ex) !알람등록 데보레카@목걸이 5\n"
					+ "ex) !알람등록 쿠툼@단검 20\n"
					;
				await context.Channel.SendMessageAsync(message);
			}
		}

		public async Task AddMarketAlarm(SocketCommandContext context, string[] args)
		{
			if (args.Length != 2)
			{
				var error = $"{context.User.Mention} 거래소 알림 등록이 실패했습니다.\n"
					+ "명령어 양식이 다릅니다.\n\n"
					+ "명령어 양식: !알람등록 [아이템이름] [강화단계]\n"
					+ "[아이템이름]: 공백을 @로 대신하여 정확히 입력해주셔야 합니다.\n"
					+ "[강화단계]: 0~5 혹은 0~20까지의 강화단계를 숫자로 입력하시면 됩니다.\n\n"
					+ "ex) !알람등록 죽은신의@갑옷 4\n"
					+ "ex) !알람등록 데보레카@목걸이 5\n"
					+ "ex) !알람등록 쿠툼@단검 20\n"
					;
				await context.Channel.SendMessageAsync(error);
				return;
			}

			var item = FindItemByName(args[0]);
			if (item is null)
			{
				var error = $"{context.User.Mention} 거래소 알림 등록이 실패했습니다.\n"
					+ "요청하신 아이템을 찾을 수 없습니다.\n\n"
					+ "명령어 양식: !알람등록 [아이템이름] [강화단계]\n"
					+ "[아이템이름]: 공백을 @로 대신하여 정확히 입력해주셔야 합니다.\n"
					+ "[강화단계]: 0~5 혹은 0~20까지의 강화단계를 숫자로 입력하시면 됩니다.\n\n"
					+ "ex) !알람등록 죽은신의@갑옷 4\n"
					+ "ex) !알람등록 데보레카@목걸이 5\n"
					+ "ex) !알람등록 쿠툼@단검 20\n"
					;
				await context.Channel.SendMessageAsync(error);
				return;
			}

			string message = string.Empty;
			if (ulong.TryParse(args[1], out var enLevel))
			{
				var alarm = new MarketAlarm(item.ItemCode, enLevel, $"{context.User.Id}");

				if (databaseService.InsertAlarmTable(alarm))
				{
					message = $"{context.User.Mention} 거래소 알림이 등록되었습니다.";
					var embed = new EmbedBuilder();
					embed.Color = Color.Blue;
					embed.Title = $"{item.ItemName}";
					embed.ThumbnailUrl = MitoConst.GetItemImageURL($"{item.ItemCode}");
					embed.Description = $"아이템 코드: {item.ItemCode}\n"
						+ $"카테고리: {item.MainCategory}\n"
						+ $"서브 카테고리: {item.SubCategory}\n";
					await context.Channel.SendMessageAsync(message, embed: embed.Build());
				}
				else
				{
					message = $"{context.User.Mention} 거래소 알림 등록이 실패했습니다.\n"
						+ "이미 등록된 알람입니다.";
					await context.Channel.SendMessageAsync(message);
				}
			}
			else
			{
				message = $"{context.User.Mention} 거래소 알림 등록이 실패했습니다.\n"
					+ "명령어를 다시 확인해주십시오."
					+ "명령어 양식: !알람등록 [아이템이름] [강화단계]\n"
					+ "[아이템이름]: 공백을 @로 대신하여 정확히 입력해주셔야 합니다.\n"
					+ "[강화단계]: 0~5 혹은 0~20까지의 강화단계를 숫자로 입력하시면 됩니다.\n\n"
					+ "ex) !알람등록 죽은신의@갑옷 4\n"
					+ "ex) !알람등록 데보레카@목걸이 5\n"
					+ "ex) !알람등록 쿠툼@단검 20\n"
					;
				await context.Channel.SendMessageAsync(message);
			}
		}

		public async Task ReadMyMarketAlarm(SocketCommandContext context)
		{
			var alarms = databaseService.ReadMyAlarmTables($"{context.User.Id}");
			string message = $"{context.User.Mention} 님이 등록하신 알람 목록입니다.\n\n";
			foreach (var alarm in alarms)
			{
				var item = ItemList.Where(x => x.ItemCode == alarm.itemCode).FirstOrDefault();
				message += $"{item.ItemName}(강화 단계: {alarm.enLevel})\n";
			}

			if (alarms.Count <= 0)
			{
				message = $"{context.User.Mention} 등록된 알람이 없습니다.";
			}
			await context.Channel.SendMessageAsync(message);
		}

		public async Task ReadAllMarketAlarm(SocketCommandContext context)
		{
			var alarms = databaseService.ReadAllAlarmTables();
			string message = string.Empty;
			foreach (var alarm in alarms)
			{
				message += MarketAlarm.AlarmToString(alarm);
			}

			if (string.IsNullOrEmpty(message))
			{
				message = $"{context.User.Mention} 등록된 알람이 없습니다.";
			}
			await context.Channel.SendMessageAsync(message);
		}

		private MarketItem? FindItemByName(string itemName)
		{
			itemName = itemName.Replace('@', ' ');
			return ItemList.Where(x => x.ItemName == itemName).FirstOrDefault();
		}


		private void InitItemCodes()
		{
			using (var reader = new StreamReader(CsvPath, Encoding.UTF8))
			{
				while (!reader.EndOfStream)
				{
					var data = reader.ReadLine()?.Split(',');
					if (data is null || data.Length < 6) continue;

					if (uint.TryParse(data[5], out var code))
					{
						ItemList.Add(new MarketItem
						{
							MainCategory = data[1],
							SubCategory = data[3],
							ItemName = data[4],
							ItemCode = code
						});
					}

				}
			}
		}

		public async void MarketTimeCheck()
		{
			var waitList = apiHandler.GetMarketWaitList();
			if (waitList is null)
			{
				// 데이터 오류
				return;
			}

			foreach (var wait in waitList)
			{
				var alarms = databaseService.ReadAlarmTableFromWaitItem(wait);
				await SendWaitItemNotify(wait, alarms);
			}
		}

		private async Task SendWaitItemNotify(WaitItem wait, List<MarketAlarm> alarms)
		{
			var embed = BuildWaitItemEmbed(wait);
			foreach (var alarm in alarms)
			{
				if(ulong.TryParse(alarm.userID, out var id))
				{
					await discord.GetUser(id).SendMessageAsync(embed: embed);
				}
			}
		}

		public async Task WaitItem(SocketCommandContext context)
		{
			var waitList = apiHandler.GetMarketWaitList();
			if (waitList is null) return;

			foreach (var waitItem in waitList)
			{
				var embed = BuildWaitItemEmbed(waitItem);
				if (embed is not null)
				{
					await context.Channel.SendMessageAsync(embed: embed);
				}
			}
		}

		private Embed? BuildWaitItemEmbed(WaitItem waitItem)
		{
			var embed = new EmbedBuilder();
			var item = ItemList.Where(x => x.ItemCode == waitItem.ItemCode).FirstOrDefault();
			var time = TimeUtil.TimeStampToDateTime(waitItem.TimeStamp);

			if (item is null) return null;

			embed.Color = Color.Blue;
			embed.Title = $"{item.ItemName} 등록 대기";
			embed.Description = $"가격: {string.Format("{0:#,0}", waitItem.Price)}\n"
				+ $"강화 단계: {waitItem.EnhancedLevel}\n"
				+ $"등록 시간: {time:HH}시 {time:mm}분\n";
			embed.ThumbnailUrl = MitoConst.GetItemImageURL($"{item.ItemCode}");

			return embed.Build();
		}
	}
}
