namespace MitoBDO.Model
{
	public class MarketAlarm
	{
		public ulong itemCode;
		public ulong enLevel;
		public string userID;

		public MarketAlarm(ulong itemCode, ulong enLevel, string userID)
		{
			this.userID = userID;
			this.itemCode = itemCode;
			this.enLevel = enLevel;
		}

		public static string AlarmToString(MarketAlarm alarm)
		{
			return $"[코드: {alarm.itemCode}] [강화: {alarm.enLevel}] [UserID: {alarm.userID}]\n";
		}
	}
}
