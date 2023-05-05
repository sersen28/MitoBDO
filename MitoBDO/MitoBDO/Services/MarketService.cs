using Discord.Commands;
using Discord.Net;
using Discord.Net.Rest;
using Discord.WebSocket;
using MitoBDO.Model;
using MitoBDO.Utility;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace MitoBDO.Services
{
	public class MarketService : ModuleBase
	{
		private const string CsvPath = "Data/ItemCodes.csv";
		private readonly DiscordSocketClient discord;
		private readonly APIHandler apiHandler;
		private List<MarketItem> ItemList = new List<MarketItem>();

		public MarketService(DiscordSocketClient discord, APIHandler apiHandler)
		{
			this.discord = discord;
			this.apiHandler = apiHandler;

			Task.Run(InitItemCodes);
		}

		private void InitItemCodes()
		{
			var reader = new StreamReader(CsvPath, Encoding.UTF8);
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

						string message =
							$"아이템 코드: {item.ItemName}\n"
							+ $"강화 단계: {en_level}\n"
							+ $"가격: {string.Format("{0:#,0}", price)}\n"
							+ $"등록 시간: {time:HH}시 {time:mm}분\n"
							+ $"====================================\n";
						await context.Channel.SendMessageAsync(message);
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
