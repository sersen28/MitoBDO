using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MitoBDO.Constants;
using System.Xml.Linq;

namespace MitoBDO.Module
{
	public class RoleModule : ModuleBase<SocketCommandContext>
	{
		[Command("role")]
		public async Task WorldBossRole()
		{
			var embed = new EmbedBuilder();
			embed.Color = Color.Blue;
			embed.Title = "월드 보스 집결 알림 설정";
			embed.Description = "가모스, 벨 집결 공지시 알림 수신 여부를 설정할 수 있습니다.\n"
				+"현재 설정되어 있는 역할은 서버 내 자신의 프로필에서 확인 가능합니다.";

			embed.AddField("벨", "목요일 AM 00:15\n일요일 PM 05:00", true);
			embed.AddField("가모스", "목요일 PM 11:45\n일요일 AM 00:15", true);

			var components = new ComponentBuilder();
			components.WithButton("벨 알림 받기", CustomID.VellPermit, ButtonStyle.Primary, Emoji.Parse(":whale:"));
			components.WithButton("가모스 알림 받기", CustomID.GarmothPermit, ButtonStyle.Primary, Emoji.Parse(":dragon:"));
			components.WithButton("벨 알림 차단", CustomID.VellBlock, ButtonStyle.Danger, Emoji.Parse(":whale:"), row: 1);
			components.WithButton("가모스 알림 차단", CustomID.GarmothBlock, ButtonStyle.Danger, Emoji.Parse(":dragon:"), row: 1);

			await Context.Message.DeleteAsync();
			await Context.Channel.SendMessageAsync(
				embed: embed.Build(),
				components: components.Build());
		}
	}
}
