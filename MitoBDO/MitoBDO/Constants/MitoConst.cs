namespace MitoBDO.Constants
{
	public static class MitoConst
	{
		private const string ItemImageBaseURL = "https://s1.pearlcdn.com/NAEU/TradeMarket/Common/img/BDO/item/";

		public const int MessageDeleteWaitMilliseconds = 1500;		

		// Channel ID
		public const ulong ArcaliveAnnouncChannel = 1036557769353986058;
		public const ulong ArcaliveTestChannel = 1078553582359543849;
		public const ulong DeveloperTestChannel = 1072018886217715786;
		public const ulong ArcaliveNodeWarChannel = 1061649286128992356;

		public static string GetItemImageURL(string itemCode)
		{
			return $"{ItemImageBaseURL}{itemCode}.png";
		}
	}
}
