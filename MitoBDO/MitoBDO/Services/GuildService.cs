using Discord;
using Discord.WebSocket;
using MitoBDO.Constants;
using MitoBDO.Model;
using System.ComponentModel;
using System.Data;

namespace MitoBDO.Services
{
	public class GuildService
	{
		private const int WaitMilliseconds = 1500;

		public async Task AddRole(SocketMessageComponent component, string roleName)
		{
			var user = component.User as SocketGuildUser;
			var channel = component.Channel as SocketGuildChannel;
			var role = channel.Guild.Roles.Where(x => x.Name == roleName).FirstOrDefault();

			if (role is not null)
			{
				await user.AddRoleAsync(role);
				await component.RespondAsync($"{component.User.Mention} {role.Name} 알림이 등록되었습니다!");
			}
			else
			{
				await component.RespondAsync($"{component.User.Mention} 역할을 찾을 수 없습니다.\n관리자에게 문의하세요.");
			}
			await Task.Delay(WaitMilliseconds);
			await component.DeleteOriginalResponseAsync();
		}

		public async Task RemoveRole(SocketMessageComponent component, string roleName)
		{
			var user = component.User as SocketGuildUser;
			var channel = component.Channel as SocketGuildChannel;
			var role = channel.Guild.Roles.Where(x => x.Name == roleName).FirstOrDefault();

			if (role is not null)
			{
				await user.RemoveRoleAsync(role);
				await component.RespondAsync($"{component.User.Mention} {role.Name} 알림이 차단되었습니다!");
			}
			else
			{
				await component.RespondAsync($"{component.User.Mention} 역할을 찾을 수 없습니다.\n관리자에게 문의하세요.");
			}
			await Task.Delay(WaitMilliseconds);
			await component.DeleteOriginalResponseAsync();
		}

		public async Task ShowPartyModal(SocketMessageComponent component)
		{
			var dest = component.Data.Values.FirstOrDefault();
			if (dest is null)
			{
				await component.RespondAsync($"{component.User.Mention} 오류가 발생했습니다.\n관리자에게 문의해주세요.");
				return;
			}

			var mb = new ModalBuilder();
			mb.WithTitle($"{dest} 파티 모집");
			mb.WithCustomId(dest);
			mb.AddTextInput("파티 제목", CustomID.PartyTitle, placeholder: "파티 제목을 작성해주세요.");
			mb.AddTextInput("파티 설명", CustomID.PartyDescription, TextInputStyle.Paragraph, "사냥 시간, 모집 조건 등을 작성해주세요.");

			await component.RespondWithModalAsync(mb.Build());
		}

		public async Task GenerateParty(SocketModal modal)
		{
			List<SocketMessageComponentData> components = modal.Data.Components.ToList();
			string dest = modal.Data.CustomId;
			var role = (modal.Channel as SocketTextChannel)?.Guild.Roles.Where(x => x.Name == dest).FirstOrDefault();
			string title = components.First(x => x.CustomId == CustomID.PartyTitle).Value;
			string desctription = components.First(x => x.CustomId == CustomID.PartyDescription).Value;
			var user = modal.User as SocketGuildUser;

			if (role is null || user is null)
			{
				await modal.RespondAsync($"{modal.User.Mention} 오류가 발생했습니다.\n관리자에게 문의해주세요.");
				return;
			}

			//Party party = new Party();


			var embed = new EmbedBuilder();
			embed.WithColor(Color.Blue);
			embed.WithTitle(title);
			embed.WithDescription(desctription);
			embed.WithAuthor(user);
			embed.AddField("참가 인원 (1/5)", user.Nickname ?? user.ToString());

			var component = new ComponentBuilder();
			component.WithButton("참가하기", CustomID.JoinParty, ButtonStyle.Primary);
			component.WithButton("탈퇴하기", CustomID.ExitParty, ButtonStyle.Danger);
			component.WithButton("모집완료", CustomID.CompleteParty, ButtonStyle.Success);

			await modal.RespondAsync(
				text: $"{role.Mention} 파티 모집이 게시되었습니다.",
				components: component.Build(),
				embed: embed.Build());
		}

		public async Task JoinParty(SocketMessageComponent component)
		{			
			await component.RespondAsync(
				text: $"{component.User.Mention} 파티에 참가하셨습니다.");

			var token = component.Token;
		}

		public async Task ExitParty(SocketMessageComponent component)
		{
			await component.RespondAsync(
				text: $"{component.User.Mention} 파티에서 탈퇴하셨습니다.");
		}

		public async Task CompeleteParty(SocketMessageComponent component)
		{
			await component.RespondAsync(
				text: $"{component.User.Mention} 파티 모집이 완료되었습니다.");
		}
	}
}
