using Discord.Commands;
using Discord.WebSocket;

namespace MitoBDO.Services
{
	public class CommandHandler
	{
		private readonly DiscordSocketClient _discord;
		private readonly CommandService _commands;
		private readonly IServiceProvider _provider;

		public CommandHandler(
			DiscordSocketClient discord,
			CommandService commands,
			IServiceProvider provider)
		{
			_discord = discord;
			_commands = commands;
			_provider = provider;
			_discord.MessageReceived += OnMessageReceivedAsync;
		}

		private async Task OnMessageReceivedAsync(SocketMessage s)
		{
			var msg = s as SocketUserMessage;
			if (msg == null) return;
			if (msg.Author.Id == _discord.CurrentUser.Id) return;

			var context = new SocketCommandContext(_discord, msg);
			int argPos = 0;
#if DEBUG
			if ((msg.HasCharPrefix('-', ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos)) && !msg.Author.IsBot)
#else
			if ((msg.HasCharPrefix('$', ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos)) && !msg.Author.IsBot)
#endif
			{
				var result = await _commands.ExecuteAsync(context, argPos, _provider);

				if (!result.IsSuccess)
					await context.Channel.SendMessageAsync(result.ToString());
			}
		}
	}
}
