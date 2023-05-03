using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using MitoBDO.Constants;
using MitoBDO.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MitoBDO.Module
{
	public class NodeWarModule : ModuleBase<SocketCommandContext>
	{
		private readonly NodeWarService nodeWarService;
		public NodeWarModule(IServiceProvider serviceProvider)
		{
			this.nodeWarService = serviceProvider.GetRequiredService<NodeWarService>();
		}

		[Command("거점공지")]
		public async Task NodeWarAnnounce([Remainder] string command)
		{
			var node = nodeWarService.FindNodeByName(command);
			if (node is null) return;

			var date = DateTime.Now.ToString(string.Format("yyyy년 MM월 dd일 ddd요일", CultureInfo.CreateSpecificCulture("ko-KR")));

			var embed = new EmbedBuilder();
			embed.Color = Color.Blue;
			embed.Title = $"{date} 거점전";
			embed.Description = $"거점: {node.Name}\n"
				+ $"단계: {node.Stage}\n"
				+ $"참여 가능 인원: {node.Num}\n"
				+ $"20시 55분까지 {node.Nation} 1채널에서 대기 바랍니다.\n";

			await Context.Message.DeleteAsync();
			await Context.Channel.SendMessageAsync(
				embed: embed.Build());
		}


		[Command("admin_nodewar")]
		public async Task NodeWarGenerator()
		{
			var embed = new EmbedBuilder();
			embed.Color = Color.Blue;
			embed.Title = "거점전 공지 생성기";
			embed.Description = "1단계 거점전 공지를 생성합니다.\n"
				+ "2단계, 3단계 공지 생성이 필요하거나 거점 정보 변경시 관리자에게 문의 바랍니다.\n";

			var components = new ComponentBuilder();
			var Tier1Menu = new SelectMenuBuilder()
			{
				CustomId = CustomID.Tier1Node,
				Placeholder = "1단계 거점",
				MaxValues = 1,
				MinValues = 1,
			};

			var nodes = nodeWarService.GetNodes();

			var Tier1Nodes = nodes.Where(x => x.Stage.Contains("1단"));
			if (Tier1Nodes.Any())
			{
				foreach (var node in Tier1Nodes)
				{
					Tier1Menu.AddOption(node.Name, node.Name, $"{node.Day}, {node.Num}인 거점");
				}
			}
			components.WithSelectMenu(Tier1Menu);

			await Context.Message.DeleteAsync();
			await Context.Channel.SendMessageAsync(
				embed: embed.Build(),
				components: components.Build());
		}
	}
}
