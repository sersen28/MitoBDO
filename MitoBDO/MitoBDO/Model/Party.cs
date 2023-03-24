using Discord.WebSocket;

namespace MitoBDO.Model
{
	public class Party
	{
		public string Title;
		public string Description;
		public string Destination;
		public DateTime CreatedAt;

		public List<SocketUser> Members;

		public Party(string Title, string Description, string Destination, DateTime CreatdeAt, List<SocketUser> Members)
		{
			this.Title = Title;
			this.Description = Description;
			this.Destination = Destination;
			this.CreatedAt = CreatedAt;
			this.Members = Members;
		}

		public void MakeMemberList()
		{
			
		}
	}
}
