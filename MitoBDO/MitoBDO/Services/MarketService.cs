using Discord.Commands;
using Discord.Net;
using Discord.Net.Rest;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using RestSharp;
using static System.Net.Mime.MediaTypeNames;

namespace MitoBDO.Services
{
	public class MarketService : ModuleBase
	{
		private readonly DiscordSocketClient discord;
		private readonly APIHandler apiHandler;

		public MarketService(DiscordSocketClient discord, APIHandler apiHandler)
		{
			this.discord = discord;
			this.apiHandler = apiHandler;
		}

		public async Task FindItem(SocketCommandContext context, string code)
		{
			var request = new RestRequest("https://trade.kr.playblackdesert.com/Trademarket/GetMarketPriceInfo", Method.Post);
			request.AddHeader("keyType", "0");
			request.AddHeader("mainKey", code);
			request.AddHeader("subKey", "5");
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
				var i = data.Split('-');
				if (i.Length == 4)
				{ 
					var code = i[0];
					var en_level = i[1];
					var price = i[2];
					var timestamp = i[3];

					string message =
						$"아이템 코드: {code}\n"
						+ $"강화 단계 = {en_level}\n"
						+ $"가격 = {price}\n"
						+ $"타임스탬프 = {timestamp}\n"
						+ $"====================================\n";
					await context.Channel.SendMessageAsync(message);
				}
			}
		}
	}
}
