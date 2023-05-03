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
			var channel = discord.GetChannel(MitoConst.ArcaliveTestChannel) as SocketTextChannel;
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

			var date = DateTime.Now.ToString(string.Format("yyyy년 MM월 dd일 ddd요일", CultureInfo.CreateSpecificCulture("ko-KR")));
			var embed = new EmbedBuilder();
			embed.Color = Color.Blue;
			embed.Title = $"{date} 거점전";
			embed.Description = $"거점: {node.Name}\n"
				+ $"단계: {node.Stage}\n"
				+ $"참여 가능 인원: {node.Num}\n"
				+ $"20시 55분까지 {node.Nation} 1채널에서 대기 바랍니다.\n";

			await channel.SendMessageAsync(
				embed: embed.Build());
		}
	}
}
