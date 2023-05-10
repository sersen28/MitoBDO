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
			if (ulong.TryParse(args[1], out var enLevel) && ulong.TryParse(item.ItemCode, out var code))
			{
				var alarm = new MarketAlarm(code, enLevel, $"{context.User.Id}");

				if (databaseService.DeleteAlarmTable(alarm))
				{
					message = $"{context.User.Mention} 거래소 알림이 삭제되었습니다.";
					var embed = new EmbedBuilder();
					embed.Color = Color.Blue;
					embed.Title = $"{item.ItemName}";
					embed.ThumbnailUrl = MitoConst.GetItemImageURL(item.ItemCode);
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
					+ "명령어를 다시 확인해주십시오.";
				await context.Channel.SendMessageAsync(message);
			}
		}

		public async Task AddMarketAlarm(SocketCommandContext context, string[] args)
		{
			if (args.Length < 2) {
				return;
			}

			var item = FindItemByName(args[0]);
			if (item is null)
			{
				return;
			}

			string message = string.Empty;
			if (ulong.TryParse(args[1], out var enLevel) && ulong.TryParse(item.ItemCode, out var code))
			{
				var alarm = new MarketAlarm(code, enLevel, $"{context.User.Id}");

				if (databaseService.InsertAlarmTable(alarm))
				{
					message = $"{context.User.Mention} 거래소 알림이 등록되었습니다.";
					var embed = new EmbedBuilder();
					embed.Color = Color.Blue;
					embed.Title = $"{item.ItemName}";
					embed.ThumbnailUrl = MitoConst.GetItemImageURL(item.ItemCode);
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
					+"명령어를 다시 확인해주십시오.";
				await context.Channel.SendMessageAsync(message);
			}
		}

		public async Task ReadMyMarketAlarm(SocketCommandContext context)
		{
			var alarms = databaseService.ReadMyAlarmTables($"{context.User.Id}");
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
					var line = reader.ReadLine();
					if (line is null) continue;

					var data = line.Split(',');

					if (data.Length < 6) continue;
					ItemList.Add(new MarketItem
					{
						MainCategory = data[1],
						SubCategory = data[3],
						ItemName = data[4],
						ItemCode = data[5],
					});
				}
			}
		}

		public async Task FindItem(SocketCommandContext context, string code)
		{
			var request = new RestRequest("https://trade.kr.playblackdesert.com/Trademarket/GetMarketPriceInfo", Method.Post);
			request.AddParameter("keyType", "0");
			request.AddParameter("mainKey", code);
			request.AddParameter("subKey", "4");
			var response = APIHandler.restClient.Execute(request);
			var json = JObject.Parse(response.Content);

			await context.Channel.SendMessageAsync(code);
		}

		public async Task MarketTimeCheck()
		{
			var channel = discord.GetChannel(MitoConst.DeveloperTestChannel) as SocketTextChannel;

			var request = new RestRequest("https://trade.kr.playblackdesert.com/Trademarket/GetWorldMarketWaitList ", Method.Post);
			var response = APIHandler.restClient.Execute(request);
			var json = JObject.Parse(response.Content);

			var t = json["resultMsg"].ToString();
			var datas = t.Split('|');

			foreach (var data in datas)
			{
				try
				{
					var i = data.Split('-');
					if (i.Length == 4)
					{
						var code = i[0];
						var en_level = i[1];
						var price = long.Parse(i[2]);
						var timestamp = long.Parse(i[3]);

						var item = ItemList.Where(x => x.ItemCode == code).FirstOrDefault();
						var time = TimeUtil.TimeStampToDateTime(timestamp);

						if (item is null)
						{
							return;
						}

						var embed = new EmbedBuilder();
						embed.Color = Color.Blue;
						embed.Title = $"{item.ItemName}";
						embed.Description = $"가격: {string.Format("{0:#,0}", price)}\n"
							+ $"강화 단계: {en_level}\n"
							+ $"등록 시간: {time:HH}시 {time:mm}분\n";
						embed.ThumbnailUrl = MitoConst.GetItemImageURL(code);
						await channel.SendMessageAsync(embed: embed.Build());
					}
				}
				catch (Exception e)
				{
					continue;
				}
			}
		}

		public async Task WaitItem(SocketCommandContext context)
		{
			var request = new RestRequest("https://trade.kr.playblackdesert.com/Trademarket/GetWorldMarketWaitList ", Method.Post);
			var response = APIHandler.restClient.Execute(request);
			var json = JObject.Parse(response.Content);

			var t = json["resultMsg"].ToString();
			var datas = t.Split('|');

			foreach (var data in datas)
			{
				try
				{
					var i = data.Split('-');
					if (i.Length == 4)
					{
						var code = i[0];
						var en_level = i[1];
						var price = long.Parse(i[2]);
						var timestamp = long.Parse(i[3]);

						var item = ItemList.Where(x => x.ItemCode == code).FirstOrDefault();
						var time = TimeUtil.TimeStampToDateTime(timestamp);

						if (item is null)
						{
							return;
						}

						var embed = new EmbedBuilder();
						embed.Color = Color.Blue;
						embed.Title = $"{item.ItemName}";
						embed.Description = $"가격: {string.Format("{0:#,0}", price)}\n"
							+ $"강화 단계: {en_level}\n"
							+ $"등록 시간: {time:HH}시 {time:mm}분\n";
						embed.ThumbnailUrl = MitoConst.GetItemImageURL(code);
						await context.Channel.SendMessageAsync(embed: embed.Build());
					}
				}
				catch (Exception e)
				{
					continue;
				}
			}
		}
	}
}
