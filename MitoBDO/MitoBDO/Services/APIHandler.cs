using Discord;
using Discord.Net.Rest;
using MitoBDO.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace MitoBDO.Services
{
	public class APIHandler
	{
		private const string APIPath = "https://trade.kr.playblackdesert.com/Trademarket/"; 
		private static RestClient restClient;

		public APIHandler()
		{
			restClient = new RestClient();
		}

		public IEnumerable<WaitItem>? GetMarketWaitList()
		{
			var request = new RestRequest($"{APIPath}GetWorldMarketWaitList ", Method.Post); 
			var response = restClient.Execute(request);

			if (response?.Content is null || response.IsSuccessful is false)
			{
				// 응답 없음
				return null;
			}

			var json = JObject.Parse(response.Content);
			var datas = json["resultMsg"]?.ToString()?.Split('|');
			if (datas is null)
			{
				// 비정상적인 데이터
				return null;
			}

			var ret = new List<WaitItem>();
			foreach (var data in datas)
			{
				var item = WaitItem.Convert(data);
				if (item is not null)
				{
					ret.Add(item);
				}
			}
			return ret;
		}
	}
}
