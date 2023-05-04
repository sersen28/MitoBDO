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

		[Command("아이템")]
		public async Task FindItem([Remainder] string command)
			=> await marketService.FindItem(Context, command);

		[Command("등록대기")]
		public async Task WaitItem()
			=> await marketService.WaitItem(Context);
	}
}
