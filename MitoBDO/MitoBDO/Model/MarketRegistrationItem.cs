namespace MitoBDO.Model
{
	public class WaitItem
	{
		public uint ItemCode { get; set; }
		public string? EnhancedLevel { get; set; }
		public ulong Price { get; set; }
		public long TimeStamp { get; set; }

		public static WaitItem? Convert(string data)
		{
			var info = data.Split('-');
			if (info.Length != 4)
			{
				return null;
			}

			try
			{
				return new WaitItem
				{
					ItemCode = uint.Parse(info[0]),
					EnhancedLevel = info[1],
					Price = ulong.Parse(info[2]),
					TimeStamp = long.Parse(info[3])
				};
			}
			catch(Exception e)
			{
				Console.Out.WriteLineAsync(e.Message);
				return null;
			}
		}
	}
}
