namespace MitoBDO.Model
{
	public class PartyIdentifier
	{
		public Guid Id { get; set; }
		public string CustomId { get; set; }
		public Party party;		

		public string Build()
		{
			return String.Empty;
		}

		public string CreateID()
		{
			return String.Empty;
		}
	}
}
