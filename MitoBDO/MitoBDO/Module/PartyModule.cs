using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MitoBDO.Constants;

namespace MitoBDO.Module
{
    public class PartyModule : ModuleBase<SocketCommandContext>
	{
		[Command("admin_generate_party")]
		[Discord.Interactions.RequireUserPermission(GuildPermission.Administrator)]
		public async Task PartyGenerate()
		{
			var embed = new EmbedBuilder();
			embed.Color = Color.Blue;
			embed.Title = "파티 모집 생성기";
			embed.Description = "파티 모집을 생성하면 해당 알림을 등록한 멤버들이 호출됩니다.\n"
				+ "만들어진 파티는 별도의 채널에서 확인 가능합니다.\n"
				+ "해당 생성기로 만들어진 파티 모집은 최장 7일까지 유지됩니다.\n";

			var components = new ComponentBuilder();
			var menu = new SelectMenuBuilder()
			{
				CustomId = CustomID.GeneratePartyMenu,
				Placeholder = "생성할 파티를 선택해 주세요.",
				MaxValues = 1,
				MinValues = 1,
			};

			menu.AddOption("가이핀라시아 지상", "가이핀라시아 지상", "5인, 권장 공격력 270")
				.AddOption("툰크타", "툰크타", "2인, 권장 공격력 270")
				.AddOption("올룬의 계곡", "올룬의 계곡", "3인, 권장 공격력 300")
				.AddOption("폐성터", "폐성터", "3인, 권장 공격력 250");
			components.WithSelectMenu(menu);

			await Context.Message.DeleteAsync();
			await Context.Channel.SendMessageAsync(
				embed: embed.Build(),
				components: components.Build());
		}
    }
}
