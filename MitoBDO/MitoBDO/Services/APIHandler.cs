using Discord.Net.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace MitoBDO.Services
{
	public class APIHandler
	{
		private const string APIPath = ""; 
		public static RestClient restClient;
		public APIHandler()
		{
			restClient = new RestClient();
		}
	}
}
