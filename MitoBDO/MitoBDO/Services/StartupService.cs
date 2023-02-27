﻿using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System.Reflection;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

		public async Task StartAsync(string TsukinoMito)
		{
#if DEBUG
			await _discord.LoginAsync(TokenType.Bot, ConfigurationManager.AppSettings["UranoMito"]);
#else
			await _discord.LoginAsync(TokenType.Bot, ConfigurationManager.AppSettings["TsukinoMito"]);
#endif
			await _discord.StartAsync();
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
			_discord.ButtonExecuted += _provider.GetService<DiscordEventHandler>().ButtonHandler;
			_discord.SelectMenuExecuted += _provider.GetService<DiscordEventHandler>().MenuHandler;
		}
	}
}
