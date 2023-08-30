using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MitoBDO.Constants;

namespace MitoBDO.Module
{
    public class SurveyModule : ModuleBase<SocketCommandContext>
	{
		[Command("admin_generate_nodewar_survey")]
		[Discord.Interactions.RequireUserPermission(GuildPermission.Administrator)]
		public async Task PartyGenerate()
		{
			var embed = new EmbedBuilder();
			embed.Color = Color.Blue;
			embed.Title = "익명 건의함";
			embed.Description = "이 채널에서 익명으로 건의할 수 있습니다.\n"
				+ "작성하신 내용을 봇이 읽어드립니다.\n";

			var components = new ComponentBuilder();
			components.WithButton("건의하기", CustomID.GenerateSurvey, ButtonStyle.Primary);

			await Context.Message.DeleteAsync();
			await Context.Channel.SendMessageAsync(
				embed: embed.Build(),
				components: components.Build());
		}
    }
}
