using Discord.Commands;

namespace MitoBDO.Module
{
	public class BasicModule : ModuleBase<SocketCommandContext>
	{
		[Command("hi"), Alias("hello", "안녕", "ㅎㅇ")]
		public async Task HelloWorld()
			=> await Context.Channel.SendMessageAsync("Hello, World!");

		[Command("say")]
		public Task Say([Remainder]string text)
			=> ReplyAsync(text);



		[Command("기립")]
		public async Task Kiritsu()
			=> await Context.Channel.SendMessageAsync("차렷! 반갑습니다. 츠키노 미토입니다!");
		
		[Command("차렷")]
		public async Task KiwoTsuke()
			=> await Context.Channel.SendMessageAsync("착석! 이상, 츠키노 미토가 보내드렸습니다.");



		[Command("起立")]
		public async Task KiritsuJP()
			=> await Context.Channel.SendMessageAsync("起立！ 気をつけ！こんにちは、月ノ美兎です！");

		[Command("気を付け")]
		public async Task KiwoTsukeJP()
			=> await Context.Channel.SendMessageAsync("着席！以上、月ノ美兎がお送りしました。");
	}
}
