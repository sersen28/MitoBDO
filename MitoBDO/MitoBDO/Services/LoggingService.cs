﻿using Discord.Commands;
using Discord.WebSocket;
using Discord;

namespace MitoBDO.Services
{
	public class LoggingService
	{
		private readonly DiscordSocketClient _discord;
		private readonly CommandService _commands;

		private string _logDirectory { get; }
		private string _logFile => Path.Combine(_logDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt");

		public LoggingService(DiscordSocketClient discord, CommandService commands)
		{
			_logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");

			_discord = discord;
			_commands = commands;

			_discord.Log += OnLogAsync;
			_commands.Log += OnLogAsync;
		}

		private Task OnLogAsync(LogMessage msg)
		{
			if (!Directory.Exists(_logDirectory))
				Directory.CreateDirectory(_logDirectory);
			if (!File.Exists(_logFile))
				File.Create(_logFile).Dispose();

			string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";

			try
			{
				File.AppendAllText(_logFile, logText + "\n");
			}
			catch (Exception ex)
			{
				return Console.Out.WriteLineAsync(ex.Message);
			}

			return Console.Out.WriteLineAsync(logText);
		}
	}
}
