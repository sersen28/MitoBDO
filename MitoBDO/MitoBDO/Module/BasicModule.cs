using Discord.Commands;

namespace MitoBDO.Module
{
	public class BasicModule : ModuleBase<SocketCommandContext>
	{
		[Command("hi"), Alias("hello", "안녕", "ㅎㅇ")]
		[Summary("Bot say \"Hello, World\"")]
		public async Task test()
			=> await Context.Channel.SendMessageAsync("Hello, World!");

		[Command("say")]
		public Task Say([Remainder]string text)
			=> ReplyAsync(text);
	}
}
