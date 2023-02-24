using Discord.Commands;
using Discord.WebSocket;
using MitoBDO.Constants;

namespace MitoBDO.Services
{
	public class ButtonEventHandler
	{
		public async Task ButtonHandler(SocketMessageComponent component)
		{
			switch (component.Data.CustomId)
			{
				case CustomID.VellPermit:
					await GuildRoleService.AddRole(component, "벨");
					break;
				case CustomID.VellBlock:
					await GuildRoleService.RemoveRole(component, "벨");
					break;
				case CustomID.GarmothPermit:
					await GuildRoleService.AddRole(component, "가모스");
					break;
				case CustomID.GarmothBlock:
					await GuildRoleService.RemoveRole(component, "가모스");
					break;
			}
		}
	}
}
