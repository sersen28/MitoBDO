using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MitoBDO.Constants;

namespace MitoBDO.Services
{
	public class DiscordEventHandler
	{
		private readonly IServiceProvider provider;
		private readonly GuildRoleService roleService;
		public DiscordEventHandler(IServiceProvider provider)
		{
			this.provider = provider;
			roleService = this.provider.GetRequiredService<GuildRoleService>();
		}


		public async Task MenuHandler(SocketMessageComponent component)
		{
			var roleName = component.Data.Values.FirstOrDefault();
			if (roleName is null) return;

			switch (component.Data.CustomId)
			{
				case CustomID.PartyPermit:
					await roleService.AddRole(component, roleName);
					break;
				case CustomID.PartyBlock:
					await roleService.RemoveRole(component, roleName);
					break;
			}
		}


		public async Task ButtonHandler(SocketMessageComponent component)
		{
			switch (component.Data.CustomId)
			{
				case CustomID.VellPermit:
					await roleService.AddRole(component, "벨");
					break;
				case CustomID.VellBlock:
					await roleService.RemoveRole(component, "벨");
					break;
				case CustomID.GarmothPermit:
					await roleService.AddRole(component, "가모스");
					break;
				case CustomID.GarmothBlock:
					await roleService.RemoveRole(component, "가모스");
					break;
				case CustomID.GenerateParty:
					this.provider.GetRequiredService<PartyService>().PartyGenerateModal(component);
					break;
				case CustomID.PartyPermit:
					break;
			}
		}
	}
}
