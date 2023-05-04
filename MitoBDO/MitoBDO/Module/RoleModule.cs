using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using MitoBDO.Constants;
using MitoBDO.Services;

namespace MitoBDO.Module
{
	public class RoleModule : ModuleBase<SocketCommandContext>
	{
		private readonly IServiceProvider provider;
		public RoleModule(IServiceProvider provider)
		{
			this.provider = provider;
		}

		[Command("admin_party_role_permit")]
		[Discord.Interactions.RequireUserPermission(GuildPermission.Administrator)]		
		public async Task PermitPartyRole()
		{
			var embed = new EmbedBuilder();
			embed.Color = Color.Blue;
			embed.Title = "파티 모집 알림 등록";
			embed.Description = "알림을 등록하면 파티 모집시 호출됩니다.\n"
				+ "현재 설정되어 있는 역할은 서버 내 자신의 프로필에서 확인 가능합니다.";

			var components = new ComponentBuilder();
			var menu = new SelectMenuBuilder()
			{
				CustomId = CustomID.PartyPermit,
				Placeholder = "등록할 알림을 선택해 주세요.",
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

		[Command("admin_party_role_block")]
		[Discord.Interactions.RequireUserPermission(GuildPermission.Administrator)]
		public async Task BlockPartyRole()
		{
			var embed = new EmbedBuilder();
			embed.Color = Color.Red;
			embed.Title = "파티 모집 알림 해제";
			embed.Description = "알림을 해제하면 파티 모집시 호출되지 않습니다.\n"
				+ "현재 설정되어 있는 역할은 서버 내 자신의 프로필에서 확인 가능합니다.";

			var components = new ComponentBuilder();
			var menu = new SelectMenuBuilder()
			{
				CustomId = CustomID.PartyBlock,
				Placeholder = "해제할 알림을 선택해 주세요.",
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

		[Command("admin_worldboss_role")]
		[Discord.Interactions.RequireUserPermission(GuildPermission.Administrator)]
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


		[Command("admin_sailboat_role")]
		[Discord.Interactions.RequireUserPermission(GuildPermission.Administrator)]
		public async Task HeavySailboatRole()
		{
			var embed = new EmbedBuilder();
			embed.Color = Color.Blue;
			embed.Title = "중범선 오너 역할 설정";
			embed.Description = "중범선 오너 여부를 설정할 수 있습니다.\n"
				+ "현재 설정되어 있는 역할은 서버 내 자신의 프로필에서 확인 가능합니다.";

			var components = new ComponentBuilder();
			components.WithButton("중범선 역할 받기", CustomID.SailboatPermit, ButtonStyle.Primary);
			components.WithButton("중범선 역할 삭제", CustomID.SailboatBlock, ButtonStyle.Danger);

			await Context.Message.DeleteAsync();
			await Context.Channel.SendMessageAsync(
				embed: embed.Build(),
				components: components.Build());
		}
	}
}
