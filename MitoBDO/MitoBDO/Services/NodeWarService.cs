using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MitoBDO.Constants;
using MitoBDO.Model;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading.Channels;

namespace MitoBDO.Services
{
	public class NodeWarService : ModuleBase
	{
		private const string CsvPath = "Data/NodeWarData.csv";
		private readonly DiscordSocketClient discord;
		private List<Node> NodeList = new List<Node>();

		public NodeWarService(DiscordSocketClient discord)
		{
			this.discord = discord;
			InitNodeList();
		}

		private void InitNodeList()
		{
			var reader = new StreamReader(CsvPath, Encoding.UTF8);
			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine();
				if (line is null) continue;

				var data = line.Split(',');
				NodeList.Add(new Node(data));
			}
		}

		public Node? FindNodeByName(string nodeName)
		{
			return NodeList.Where(x => x.Name == nodeName).FirstOrDefault();
		}

		public IEnumerable<Node> GetNodes()
		{
			return NodeList;
		}

		public async Task GeneratorHandler(SocketMessageComponent component)
		{
#if DEBUG
			var channel = discord.GetChannel(MitoConst.DeveloperTestChannel) as SocketTextChannel;
#else
			var channel = discord.GetChannel(MitoConst.ArcaliveAnnouncChannel) as SocketTextChannel;
#endif
			var nodeName = component.Data.Values.FirstOrDefault();

			if (channel is null || string.IsNullOrEmpty(nodeName)) return;
			var node = FindNodeByName(nodeName);
			await SendAnnounce(node, channel);

			await component.RespondAsync($"{component.User.Mention} 공지를 전송하였습니다.\n확인 바랍니다.");
			await Task.Delay(MitoConst.MessageDeleteWaitMilliseconds);
			await component.DeleteOriginalResponseAsync();
		}

		private async Task SendAnnounce(Node? node, SocketTextChannel channel)
		{
			if (node is null) return;

			var time = DateTime.Now.Hour > 21 ? DateTime.Now : DateTime.Now.AddDays(1);
			var date = DateTime.Now.ToString(string.Format("yyyy년 MM월 dd일 ddd요일", CultureInfo.CreateSpecificCulture("ko-KR")));

			var embed = new EmbedBuilder();
			embed.Color = Color.Blue;
			embed.Title = $"[{node.Nation}] - {node.Name}";
			embed.Description =
				  $"일시: {date} 21시\n"
				+ $"단계: {node.Stage}\n"
				+ $"참여 가능 인원: {node.Num}\n"
				+ $"20시 55분까지 {node.Nation} 1채널에서 대기 바랍니다.\n";

			var nodeChannel = discord.GetChannel(MitoConst.ArcaliveNodeWarChannel) as SocketTextChannel;
			var guideMessage =
				$"{channel.Guild.EveryoneRole.Mention} **{date}** 거점전 공지입니다.\n"
				+ $"거점전 세팅 & 준비물은 {nodeChannel?.Mention} 에서 확인하실 수 있습니다.\n"
				+ $"초행일 경우 필독 바랍니다.";

			await channel.SendMessageAsync(guideMessage, embed: embed.Build());
		}

		public async Task CallSailBoat(SocketMessageComponent component)
		{
			var channel = discord.GetChannel(MitoConst.ArcaliveAnnouncChannel) as SocketTextChannel;
			if (channel is null) return;

			var role = channel.Guild.Roles.Where(x => x.Name == RoleName.Sailboat).FirstOrDefault();
			var message = $"{role.Mention}\n```해안 성채가 예상됩니다. 중범선 동원 바랍니다.```";
			await channel.SendMessageAsync(message);

			await component.RespondAsync($"{component.User.Mention} 공지를 전송하였습니다.\n확인 바랍니다.");
			await Task.Delay(MitoConst.MessageDeleteWaitMilliseconds);
			await component.DeleteOriginalResponseAsync();
		}

		public async Task NeedCompass(SocketMessageComponent component)
		{
			var channel = discord.GetChannel(MitoConst.ArcaliveAnnouncChannel) as SocketTextChannel;
			if (channel is null) return;

			var message = $"```[사막 거점입니다. 나침반 챙겨주세요.]```";
			await channel.SendMessageAsync(message);

			await component.RespondAsync($"{component.User.Mention} 공지를 전송하였습니다.\n확인 바랍니다.");
			await Task.Delay(MitoConst.MessageDeleteWaitMilliseconds);
			await component.DeleteOriginalResponseAsync();
		}
	}
}
