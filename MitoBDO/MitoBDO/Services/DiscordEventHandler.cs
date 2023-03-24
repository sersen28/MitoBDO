using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MitoBDO.Constants;
using System.ComponentModel;

namespace MitoBDO.Services
{
	public class DiscordEventHandler
	{
		private readonly IServiceProvider provider;
		private readonly GuildService guildService;
		public DiscordEventHandler(IServiceProvider provider)
		{
			this.provider = provider;
			guildService = this.provider.GetRequiredService<GuildService>();
		}

		public async Task ModalHandler(SocketModal modal)
		{
			await guildService.GenerateParty(modal);
		}

		public async Task MenuHandler(SocketMessageComponent component)
		{
			var roleName = component.Data.Values.FirstOrDefault();
			if (roleName is null) return;

			switch (component.Data.CustomId)
			{
				case CustomID.PartyPermit:
					await guildService.AddRole(component, roleName);
					break;
				case CustomID.PartyBlock:
					await guildService.RemoveRole(component, roleName);
					break;
				case CustomID.GeneratePartyMenu:
					await guildService.ShowPartyModal(component);
					break;
			}
		}

		public async Task ButtonHandler(SocketMessageComponent component)
		{
			switch (component.Data.CustomId)
			{
				case CustomID.VellPermit:
					guildService.AddRole(component, "벨");
					break;
				case CustomID.VellBlock:
					guildService.RemoveRole(component, "벨");
					break;
				case CustomID.GarmothPermit:
					guildService.AddRole(component, "가모스");
					break;
				case CustomID.GarmothBlock:
					guildService.RemoveRole(component, "가모스");
					break;
				case CustomID.JoinParty:
					guildService.JoinParty(component);
					break;
				case CustomID.CompleteParty:
					guildService.ExitParty(component);
					break;
				case CustomID.ExitParty:
					guildService.CompeleteParty(component);
					break;
			}
		}
	}
}
