using Discord;
using Discord.WebSocket;

namespace MitoBDO.Services
{
	public static class GuildRoleService
	{
		public static async Task AddRole(SocketMessageComponent component, string roleName)
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
			await Task.Delay(10000);
			await component.DeleteOriginalResponseAsync();
		}

		public static async Task RemoveRole(SocketMessageComponent component, string roleName)
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
			await Task.Delay(10000);
			await component.DeleteOriginalResponseAsync();
		}
	}
}
