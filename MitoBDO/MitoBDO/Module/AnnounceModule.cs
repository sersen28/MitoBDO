using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MitoBDO.Services;
using System.Xml.Linq;

namespace MitoBDO.Module
{
	[Name("Announce")]
	public class AnnounceModule : ModuleBase<SocketCommandContext>
	{
		[Group("취소"), Name("Announce")]
		[RequireContext(ContextType.Guild)]
		public class Cancel : ModuleBase
		{
			[Command("가모스")]
			[RequireUserPermission(GuildPermission.Administrator)]
			public async Task CancelGarmoth()
			{
				TimeService.GarmothAssembly = false;
				await ReplyAsync($"다음 가모스 공지를 취소합니다. 현재 상태는 {TimeService.GarmothAssembly}입니다.");
			}

			[Command("벨")]
			[RequireUserPermission(GuildPermission.Administrator)]
			public async Task CancelVell()
			{
				TimeService.VellAssembly = false;
				await ReplyAsync($"다음 벨 공지를 취소합니다. 현재 상태는 {TimeService.VellAssembly}입니다.");
			}
		}

		[Group("등록"), Name("Announce")]
		[RequireContext(ContextType.Guild)]
		public class Add : ModuleBase
		{
			[Command("가모스")]
			[RequireUserPermission(GuildPermission.Administrator)]
			public async Task CancelGarmoth()
			{
				TimeService.GarmothAssembly = true;
				await ReplyAsync($"다음 가모스 공지를 등록합니다. 현재 상태는 {TimeService.GarmothAssembly}입니다.");
			}

			[Command("벨")]
			[RequireUserPermission(GuildPermission.Administrator)]
			public async Task CancelVell()
			{
				TimeService.VellAssembly = true;
				await ReplyAsync($"다음 벨 공지를 등록합니다. 현재 상태는 {TimeService.VellAssembly}입니다.");
			}
		}
	}
}
