using Discord;
using Discord.Commands;
using MitoBDO.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MitoBDO.Module
{
	public class MarketModule : ModuleBase<SocketCommandContext>
	{
		private readonly MarketService marketService;
		public MarketModule(MarketService marketService) 
		{
			this.marketService = marketService;
		}

		[Command("db")]
		public async Task WaitItem3()
			=> await marketService.ReadAllMarketAlarm(Context);

		[Command("알람확인")]
		public async Task myalsr()
			=> await marketService.ReadMyMarketAlarm(Context);

		[Command("알람등록")]
		public async Task WaitItem2(params string[] args)
			=> await marketService.AddMarketAlarm(Context, args);

		[Command("알람삭제")]
		public async Task WaitItem4(params string[] args)
			=> await marketService.DeleteMarketAlarm(Context, args);

		[Command("등록대기")]
		public async Task WaitItem()
			=> await marketService.WaitItem(Context);
	}
}
