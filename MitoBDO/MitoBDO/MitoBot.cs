using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using MitoBDO.Services;

namespace MitoBDO
{
	public class MitoBot
	{
		private string UranoMito;

		public MitoBot(string[] args)
		{
			if (args is not null && args.Length > 0)
			{
				UranoMito = args[0];
			}
		}

		public static async Task RunAsync(string[] args)
		{
			var startup = new MitoBot(args);
			await startup.RunAsync();
		}

		public async Task RunAsync()
		{
			var services = new ServiceCollection();
			ConfigureServices(services);

			var provider = services.BuildServiceProvider();
			provider.GetRequiredService<LoggingService>();
			provider.GetRequiredService<CommandHandler>();

			await provider.GetRequiredService<StartupService>().StartAsync(UranoMito);
			await Task.Delay(-1);
		}

		private void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
			{
				GatewayIntents = GatewayIntents.All,
				LogLevel = LogSeverity.Verbose,
			}))
			.AddSingleton(new CommandService(new CommandServiceConfig
			{
				LogLevel = LogSeverity.Verbose,
			}))
			.AddSingleton<CommandHandler>()
			.AddSingleton<StartupService>()
			.AddSingleton<LoggingService>()
			.AddSingleton<Random>();
		}
	}
}
