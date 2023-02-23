using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System.Reflection;

namespace MitoBDO.Services
{
	public class StartupService
	{
		private readonly IServiceProvider _provider;
		private readonly DiscordSocketClient _discord;
		private readonly CommandService _commands;

		public StartupService(
			IServiceProvider provider,
			DiscordSocketClient discord,
			CommandService commands)
		{
			_provider = provider;
			_discord = discord;
			_commands = commands;
		}

		public async Task StartAsync()
		{
#if DEBUG
			await _discord.LoginAsync(TokenType.Bot, "MTA3ODEyOTIyNzkwNTM4ODU2NA.GNfziY.TWQBsL8-l-JWq8XXewYPI9-9RAw0SxWXZ_Dj3Y");
#else
			await _discord.LoginAsync(TokenType.Bot, "MTA3ODEyODQ0MDQ2Njc1MTYwOQ.GHZ3dD.YnxtBPC5orKbK5PAysJG7OLCn-wtIEENAJ6v4U");
#endif
			await _discord.StartAsync();
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
		}
	}
}
