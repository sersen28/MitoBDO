using System.Data.SQLite;

namespace MitoBDO.Model
{
	public class MarketAlarm
	{
		public uint itemCode;
		public ulong enLevel;
		public string? userID;

		public MarketAlarm()
		{

		}

		public MarketAlarm(uint itemCode, ulong enLevel, string? userID)
		{
			this.itemCode = itemCode;
			this.enLevel = enLevel;
			this.userID = userID;
		}

		public static string AlarmToString(MarketAlarm alarm)
		{
			return $"[코드: {alarm.itemCode}] [강화: {alarm.enLevel}] [UserID: {alarm.userID}]\n";
		}
	}
}
