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
		private readonly NodeWarService nodeWarService;
		public DiscordEventHandler(IServiceProvider provider)
		{
			this.provider = provider;
			guildService = this.provider.GetRequiredService<GuildService>();
			nodeWarService = this.provider.GetRequiredService<NodeWarService>();
		}

		public async Task ModalHandler(SocketModal modal)
		{
			switch (modal.Data.CustomId)
			{
				case CustomID.NodeWarSurvey:
					await guildService.SurveyResult(modal);
					break;
			}
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
				case CustomID.NodeWarSurvey:
					await guildService.ShowPartyModal(component);
					break;
				case CustomID.Tier1Node:
					await nodeWarService.GeneratorHandler(component);
					break;
				case CustomID.Tier2Node:
					await nodeWarService.GeneratorHandler(component);
					break;
			}
		}

		public async Task ButtonHandler(SocketMessageComponent component)
		{
			switch (component.Data.CustomId)
			{
				case CustomID.SailboatPermit:
					await guildService.AddRole(component, RoleName.Sailboat);
					break;
				case CustomID.SailboatBlock:
					await guildService.RemoveRole(component, RoleName.Sailboat);
					break;
				case CustomID.VellPermit:
					await guildService.AddRole(component, RoleName.Vell);
					break;
				case CustomID.VellBlock:
					await guildService.RemoveRole(component, RoleName.Vell);
					break;
				case CustomID.GarmothPermit:
					await guildService.AddRole(component, RoleName.Garmoth);
					break;
				case CustomID.GarmothBlock:
					await guildService.RemoveRole(component, RoleName.Garmoth);
					break;
				case CustomID.JoinParty:
					await guildService.JoinParty(component);
					break;
				case CustomID.CompleteParty:
					await guildService.ExitParty(component);
					break;
				case CustomID.ExitParty:
					await guildService.CompeleteParty(component);
					break;
				case CustomID.CallSailBoat:
					await nodeWarService.CallSailBoat(component);
					break;
				case CustomID.NeedCompass:
					await nodeWarService.NeedCompass(component);
					break;
				case CustomID.GenerateSurvey:
					await guildService.GenerateSurvey(component);
					break;
			}
		}
	}
}
